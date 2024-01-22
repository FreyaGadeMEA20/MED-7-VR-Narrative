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
        public bool overUnder;

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

        [Header("Override timeline change")]
        public bool Override;

        [Header("Final")]
        public bool finalSeq;
    }

    // Keeping track of the current selected sequence
    [SerializeField] LinkedListNode<SequenceTimeline> currentSequence;

    // Keeping track of whether the program is mid switching
    bool switching = false;

    // Variables for CSV
    private StreamWriter csvWriter;     // Writing to a CSV File
    [SerializeField] string id;         // Variable to keep track of the user
    [SerializeField] string batch;      // Batch name
    private float totalTimeExperienced; // The total time experienced in the experience
    private bool POIAF;                 // Tracking whether they crossed the POI
    private int blinks;                 // Amount of blinks
    float ADB;                          // Average Difference in Blinks

    // List to keep track of the Average Difference in Blinks
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
        // TODO: needs to be updated so it doesn't play it immediately
        currentSequence = sequences.First;
        pd = currentSequence.Value.currentTimeline; //playableDirectors[directorIndex];
        pd.Play();

        // Initialize the StreamWriter to write to a CSV file
        string filePath = @"Assets\CSV Data\BlinkrateData_"+batch+".csv"; // Set your desired file path
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
        if((pd.time < buffer) || switching || currentSequence.Value.finalSeq){
            Debug.Log("Can't continue");
            return;
        }

        blinks++;

        // Here we save the BlinkTimeDiff value in ADBList and get the information needed in the CSV file.
        AddBlinkDiff();
        totalTimeExperienced += (float)pd.time;

        if(!currentSequence.Value.finalSeq)
            WriteToCSV();

        StartCoroutine(Blink());
    }

    IEnumerator Blink(){
        eyeShaderEffect.EyeAnim_Close();
        switching = true;
        if(currentSequence.Value.Override) {
            print("Overriding switcher.");
        } else {
            if (currentSequence.Value.UsesADB){
                print("ADB");
                SwitchTimelineADB();
            }
            else
            {
                print("Non ADB");
                SwitchTimelineBR();
            }
        }
        yield return new WaitForSeconds(1f);
        DirectorStop();
        if(currentSequence.Value.Override){
            if(currentSequence.Value.overUnder){
                SetNext(true);
            } else {
                SetNext(false);
            }
        } 
        currentSequence = currentSequence.Next;
        pd = currentSequence.Value.currentTimeline;
        
        pd.Play();
        switching = false;
        eyeShaderEffect.EyeAnim_Open();
    }

    private void WriteToCSV()
    {
        AddBlinkDiff();
        ADB = GetAverageDifferenceBlink();
        
        //"User/ID, SceneNR, Blink, Green/Blue, TotalTime, TimeBeforeBlink, SceneTime, POI, DiffFromPOI, AverageTotalDiff"
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
        if(currentSequence.Value.finalSeq)
            return;
        LinkedListNode<SequenceTimeline> nextNode = currentSequence.Next;
        nextNode = nextNode == null ? sequences.First : nextNode;
        nextNode.Value.overUnder = POIAF;
        if(over){
            Debug.Log("True");
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

    static int GetNodeIndex<T>(LinkedList<T> list, LinkedListNode<T> targetNode)
    {
        int index = 0;
        foreach (var node in list)
        {
            if (EqualityComparer<T>.Default.Equals(node, targetNode.Value))
            {
                return index;
            }
            index++;
        }

        return -1; // Node not found
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