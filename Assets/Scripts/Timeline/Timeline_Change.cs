using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

public class Timeline_Change : MonoBehaviour
{

    public int SwitchTime{
        get{return switchTime;}
    }
    public int buffer;
    private int directorIndex = 0;
    private int switchTime = 0;
    
    public List<int> switchTimes;
    //private PlayableDirector playableDirector_Current;
    [SerializeField] PlayableDirector pd;

    [SerializeField] List<SequenceTimelineScriptableObject> sequences;

    [SerializeField] SequenceTimelineScriptableObject currentSequence;

    private float pressRate;
    private float averagePressRate;

    private List<float> pressTimestamps;
    private List<float> pressRates;

    private StreamWriter csvWriter;

    // Start is called before the first frame update
    void Start()
    {
        currentSequence = sequences[0];
        pd.playableAsset = currentSequence.currentTimeline; //playableDirectors[directorIndex];
        pd.Play();

        pressTimestamps = new List<float>();
        pressRates = new List<float>();

        // Initialize the StreamWriter to write to a CSV file
        string filePath = @"Assets\CSV Data\BlinkrateData.csv"; // Set your desired file path
        csvWriter = new StreamWriter(filePath, true);

         // Check if the file is empty (indicating the start of a new session) and write headers
        if (csvWriter.BaseStream.Length == 0)
        {
            csvWriter.WriteLine("Time, CurrentScene, BlinkRate, AveragePressRate"); // Write header
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
        if(pd.time < pd.duration/2){
            return;
        }
        print("HELLO");


        //Debug.Log("The scene has been running for " + (int)Time.time + " seconds");
        //Debug.Log("The timeline has been running for " + (int)playableDirector_Current.time + " seconds");
        DirectorStop();

        // Here we get the information needed in the CSV file.
        WriteToCSV();

        if (currentSequence.OBR){
            SwitchTimelineOBR();

            Debug.Log ("Timeline switched to " + directorIndex+ " at " + pd.time + " seconds");
        }
        else
        {
            SwitchTimelineBR();

            Debug.Log ("Timeline switched to " +directorIndex+ " at " + pd.time + " seconds with blink rate");
        }

        currentSequence = currentSequence.nextSequence;
        pd.playableAsset = currentSequence.currentTimeline;//playableDirectors[directorIndex];
        pd.Play();
        Debug.Log ("Switchtime is now at " + currentSequence.switchTime);

        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Close the StreamWriter when the game is finished
            csvWriter.Close();
        }*/
    }

    private void WriteToCSV()
    {
        float currentTimestamp = Time.time;
        pressTimestamps.Add(currentTimestamp);
        CalculateButtonPressRate();

        csvWriter.WriteLine($"{currentTimestamp}, {pressRate}, {averagePressRate}");
        csvWriter.Flush(); // Flush to ensure data is written immediately
    }

    private void SwitchTimelineBR()
    {
        if (pd.time < currentSequence.switchTime)
        {
            currentSequence.SetNext(true);
            /*directorIndex+= 1;
            switchTime += 1;
            if (directorIndex % 2 == 0)
            {
               directorIndex+= 1;
                switchTime += 1;
            }*/
        }   
        else
        {
            currentSequence.SetNext(false);
            /*directorIndex+= 2;
            switchTime += 2;
            if (directorIndex % 2 == 1)
            {
               directorIndex+= 1;
                switchTime += 1;
            }*/
        }
    }

    private void SwitchTimelineOBR()
    {   
        if (pressRate < averagePressRate)
        {
            currentSequence.SetNext(true);
            /*directorIndex+= 1;
            switchTime += 1;
            if (directorIndex % 2 == 0)
            {
               directorIndex+= 1;
                switchTime += 1;
            }*/
        }   
        else 
        {
            currentSequence.SetNext(false);
            /*directorIndex+= 2;
            switchTime += 2;
            if (directorIndex % 2 == 1)
            {
                directorIndex+= 1;
                switchTime += 1;
            }*/
        }
        Debug.Log("OBR Method was called");
    }

    void CalculateButtonPressRate()
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