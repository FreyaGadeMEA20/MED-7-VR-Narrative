using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

public class Timeline_Change : MonoBehaviour
{

    public int switchTime;
    public int buffer;
    private int cd = 0;
    private int pd;
    
    public List<PlayableDirector> playableDirectors;
    private PlayableDirector playableDirector_Current;
    private PlayableDirector playableDirector_Previous;
    
    private float pressRate;
    private float averagePressRate;
    private List<float> pressTimestamps;
    private List<float> pressRates;

    private StreamWriter csvWriter;

    // Start is called before the first frame update
    void Start()
    {
        playableDirector_Current = playableDirectors[cd];
        playableDirector_Current.Play();

        pressTimestamps = new List<float>();
        pressRates = new List<float>();

        // Initialize the StreamWriter to write to a CSV file
        string filePath = @"Assets\CSV Data\BlinkrateData.csv"; // Set your desired file path
        csvWriter = new StreamWriter(filePath, true);

         // Check if the file is empty (indicating the start of a new session) and write headers
        if (csvWriter.BaseStream.Length == 0)
        {
            csvWriter.WriteLine("Time, ButtonPressRate, AveragePressRate"); // Write header
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("The scene has been running for " + (int)Time.time + " seconds");
        //Debug.Log("The timeline has been running for " + (int)playableDirector_Current.time + " seconds");
        
        DirectorStop();

        if (playableDirector_Current.time > buffer && Input.GetKeyDown(KeyCode.Space))
        {
            float currentTimestamp = Time.time;
            pressTimestamps.Add(currentTimestamp);
            CalculateButtonPressRate();

            csvWriter.WriteLine($"{currentTimestamp}, {pressRate}, {averagePressRate}");
            csvWriter.Flush(); // Flush to ensure data is written immediately
        }

        if (Input.GetKeyDown(KeyCode.Space) && playableDirector_Current.time > buffer && (cd == 5 || cd == 6 || cd == 11 || cd == 12 || cd == 17 || cd == 18 || cd == 21 || cd == 22))
        {
            SwitchTimelineOBR();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && playableDirector_Current.time > buffer)
        {
            SwitchTimeline();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Close the StreamWriter when the game is finished
            csvWriter.Close();
        }
    }

    public void SwitchTimeline()
    {
        if (playableDirector_Current.time < switchTime)
        {
            cd += 1;
            if (cd % 2 == 0)
            {
                cd +=1;
            }
            playableDirector_Current = playableDirectors[cd];
            playableDirector_Current.Play();
            Debug.Log ("Timeline switched to " + cd + " at " + playableDirector_Current.time + " seconds");
        }   
        else if (playableDirector_Current.time > switchTime)
        {
            cd += 2;
            if (cd % 2 == 1)
            {
                cd +=1;
            }
            playableDirector_Current = playableDirectors[cd];
            playableDirector_Current.Play();
            Debug.Log ("Timeline switched to " + cd + " at " + playableDirector_Current.time + " seconds");
        }
    }

    public void SwitchTimelineOBR()
    {      
        if (pressRate < averagePressRate)
        {
            cd += 1;
            if (cd % 2 == 0)
            {
                cd +=1;
            }
            playableDirector_Current = playableDirectors[cd];
            playableDirector_Current.Play();
            Debug.Log ("Timeline switched to " + cd + " at " + playableDirector_Current.time + " seconds with blink rate");
        }   
        else if (pressRate > averagePressRate)
        {
            cd += 2;
            if (cd % 2 == 1)
            {
                cd += 1;
            }
            playableDirector_Current = playableDirectors[cd];
            playableDirector_Current.Play();
            Debug.Log ("Timeline switched to " + cd + " at " + playableDirector_Current.time + " seconds with blink rate");
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
        foreach (var director in playableDirectors)
        {
            if (director != playableDirector_Current)
            {
                director.Stop();
            }
        }
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