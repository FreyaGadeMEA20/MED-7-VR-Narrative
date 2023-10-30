using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{

    public int ct = 0;
    public int nt = 1;

    public PlayableDirector currentTimeline;
    public PlayableDirector nextTimeline;

    public void SwitchToTimelineA()
    {  
        
        // Load the new timeline using Resources.Load
        string path =  "Sequence" + ct + "/Timeline" + ct + "Adirector";
        currentTimeline = Resources.Load(path);

        // Stop the currently playing timeline
        currentTimeline.Stop();
        currentTimeline.gameObject.SetActive(false);


        nextTimeline.gameObject.SetActive(true);
        nextTimeline.Play();
        if (nextTimeline = null)
        {
            Debug.LogError("Timeline not found at path: " + path);
        }
    }
    public void SwitchToTimelineB()
    {
        
        // Load the new timeline using Resources.Load
        string path =  "Sequence" + ct + "/Timeline" + ct + "Bdirector";
        currentTimeline = Resources.Load(path);

        // Stop the currently playing timeline
        currentTimeline.Stop();
        currentTimeline.gameObject.SetActive(false);


        nextTimeline.gameObject.SetActive(true);
        nextTimeline.Play();

        if (nextTimeline = null)
        {
            Debug.LogError("Timeline not found at path: " + path);
        }
    }
}
