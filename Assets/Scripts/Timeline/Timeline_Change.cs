using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Timeline_Change : MonoBehaviour
{
    public int buffer = 3;    
    public PlayableDirector pd;

    [SerializeField] SequenceTimeline[] sequencesSerialized;
    LinkedList<SequenceTimeline> sequences;

    [SerializeField] EyeController eyeShaderEffect;
    [Serializable]
    public class SequenceTimeline{
        [Header("Current Timeline")]
        public PlayableDirector currentTimeline;
        public int switchTime;
        [Header("Next timeline for before the switch point")]
        public PlayableDirector directorUnder;
        public int switchTimeU;
        [Header("Next timeline for after the switch point")]
        public PlayableDirector directorOver;
        public int switchTimeO;
        //public SequenceTimeline nextSequence;

        // POI timestamp.
        [Header("Average Difference in BlinkTime-POI")]
        public bool UsesADB;


        [Header("Final")]
        public bool final;
    }

    [SerializeField] LinkedListNode<SequenceTimeline> currentSequence;


    // Variables for CSV
    private StreamWriter csvWriter;
    [SerializeField] string id;
    private float totalTimeExperienced;
    private bool POIAF;
    private int blinks;
    float ADB;

    //private float averagePressRate;
    //private float pressRate;
    //private List<float> pressTimestamps;
    //private List<float> pressRates;

    private List<float> ADBList = new List<float>();




    void Awake(){
        sequences = new LinkedList<SequenceTimeline>();

        foreach (SequenceTimeline item in sequencesSerialized)
        {
            sequences.AddLast(item);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        currentSequence = sequences.First;
        pd = currentSequence.Value.currentTimeline; //playableDirectors[directorIndex];
        pd.Play();

        // Initialize the StreamWriter to write to a CSV file
        string filePath = @"Assets\CSV Data\BlinkrateData_"+id+".csv"; // Set your desired file path
        csvWriter = new StreamWriter(filePath, true);

         // Check if the file is empty (indicating the start of a new session) and write headers
        if (csvWriter.BaseStream.Length == 0)
        {
            csvWriter.WriteLine("User/ID, SceneNR, Blink, Green/Blue, TotalTime, TimeBeforeBlink, SceneTime, POI, DiffFromPOI, AverageTotalDiff"); // Write header
        }
    }

    // Update is called once per frame
    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            SwitchTimeline();
            //pd.playableAsset = currentSequence.currentTimeline;
            //pd.Play();
        }
    }
    public void SwitchTimeline()
    {
        // Will not continue if the buffer time has not been reached.
        // Can be made unique for each sequence if needed
        //buffer = (int) pd.duration/2;
        if(pd.time < buffer){
            return;
        }

        blinks++;
        //eyeShaderEffect.EyeAnim_Close();
        //StartCoroutine("Blink");

        // Here we save the BlinkTimeDiff value in ADBList and get the information needed in the CSV file.
        AddBlinkDiff();
        totalTimeExperienced += (float)pd.time;
        WriteToCSV();

        if (currentSequence.Value.UsesADB){
            print("ADB");
            SwitchTimelineADB();
        }
        else
        {
            print("Non ADB");
            SwitchTimelineBR();
        }
        //Debug.Log("The scene has been running for " + (int)Time.time + " seconds");
        //Debug.Log("The timeline has been running for " + (int)playableDirector_Current.time + " seconds");
        DirectorStop();
        currentSequence = currentSequence.Next;
        pd = currentSequence.Value.currentTimeline;//playableDirectors[directorIndex];
        pd.Play();
        eyeShaderEffect.EyeAnim_Open();
        Debug.Log ("Switchtime is now at " + currentSequence.Value.switchTime);

        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Close the StreamWriter when the game is finished
            csvWriter.Close();
        }*/
    }

    private void WriteToCSV()
    {
        AddBlinkDiff();
        ADB = GetAverageDifferenceBlink();
        
        //CalculateButtonPressRate();
        //"User/ID, SceneNR, Blink, Green/Blue, TotalTime, TimeBeforeBlink, SceneTime, POI, DiffFromPOI, AverageTotalDiff"
        //Debug.Log();
        csvWriter.WriteLine($"{id}, {currentSequence.Value.currentTimeline}, {blinks}, {POIAF}, {totalTimeExperienced}, {pd.time}, {currentSequence.Value.currentTimeline.duration}, {currentSequence.Value.switchTime}, {pd.time - currentSequence.Value.switchTime}, {ADB}");
        csvWriter.Flush(); // Flush to ensure data is written immediately
    }

    private void SwitchTimelineBR()
    {
        print(pd.time + " | " + currentSequence.Value.switchTime);
        POIAF = pd.time > currentSequence.Value.switchTime;
        SetNext(POIAF);
    }

    private void SwitchTimelineADB()
    {   
        Debug.Log("ADB Method was called");
        POIAF = IsADBPositive();
        SetNext(POIAF);
    }

    public void SetNext(bool over){
        if(currentSequence.Value.final)
            return;
        LinkedListNode<SequenceTimeline> nextNode = currentSequence.Next;
        nextNode = nextNode == null ? sequences.First : nextNode;
        if(over){
            nextNode.Value.currentTimeline = currentSequence.Value.directorOver;
            nextNode.Value.switchTime = currentSequence.Value.switchTimeO;
        } else {
            nextNode.Value.currentTimeline = currentSequence.Value.directorUnder;
            nextNode.Value.switchTime = currentSequence.Value.switchTimeU;
        }
    }

    /*void CalculateButtonPressRate()
    {
        if (pressTimestamps.Count < 2)
        {
            // Not enough data to calculate a rate yet.
            return;
        }

        float deltaTime = pressTimestamps[pressTimestamps.Count - 1] - pressTimestamps[0];
        int pressCount = pressTimestamps.Count - 1; // Subtract 1 to exclude the first press as it's not between presses.

        pressRate = pressCount / deltaTime;

        pressRates.Add(pressRate);

        // Calculate the average press rate
        float totalRate = 0f;
        foreach (float rate in pressRates)
        {
            totalRate += rate;
        }

        averagePressRate = totalRate / pressRates.Count;

        Debug.Log("Button Press Rate: " + pressRate);
        Debug.Log("Average Press Rate: " + averagePressRate);   
    }*/


    bool IsADBPositive() //checks if the average of the timestamps from Blink-POI is larger than 0 (and you would go the BL/Blue direction in story)
    {
        return GetAverageDifferenceBlink() >= 0f;
        /*if(ADBList.Average()>= 0f){
            return true;
        } else{
            return false;
        }*/
    }

    float GetAverageDifferenceBlink() // a bit much since ADBList.Average() seems shorter to write
    {
        return ADBList.Average();
    }
    
    void AddBlinkDiff() //Adds the BlinkTime-POI to a List of  Differences in BlinkTime-POI
    {
        ADBList.Add(Convert.ToSingle(pd.time - currentSequence.Value.switchTime));
    }

    public void DirectorStop()
    {
        //foreach (var director in playableDirectors)
        //{
            //if (director != playableDirector_Current)
            //{
        pd.Stop();
    }

    private void OnApplicationQuit()
    {
        // Close the StreamWriter when the application is exiting
        if (csvWriter != null)
        {
            csvWriter.Close();
        }
    }
}