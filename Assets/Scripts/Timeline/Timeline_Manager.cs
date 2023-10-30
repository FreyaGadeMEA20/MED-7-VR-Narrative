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
    string path = "Directors/Sequence" + ct + "/Timeline" + ct + "ADirector";
    Object loadedObject = Resources.Load(path);

    if (loadedObject != null)
    {
        currentTimeline = loadedObject as PlayableDirector; // Cast the loaded object to PlayableDirector

        if (currentTimeline != null)
        {
            // Stop the currently playing timeline
            currentTimeline.Stop();
            currentTimeline.gameObject.SetActive(false);

            nextTimeline.gameObject.SetActive(true);
            nextTimeline.Play();
        }
        else
        {
            Debug.LogError("Loaded object is not a PlayableDirector at path: " + path);
        }
    }
    else
    {
        Debug.LogError("Timeline not found at path: " + path);
    }
    }

    public void SwitchToTimelineB()
    {
    // Load the new timeline using Resources.Load
    string path = "Directors/Sequence" + ct + "/Timeline" + ct + "BDirector";
    Object loadedObject = Resources.Load(path);

    if (loadedObject != null)
    {
        currentTimeline = loadedObject as PlayableDirector; // Cast the loaded object to PlayableDirector

        if (currentTimeline != null)
        {
            // Stop the currently playing timeline
            currentTimeline.Stop();
            currentTimeline.gameObject.SetActive(false);

            nextTimeline.gameObject.SetActive(true);
            nextTimeline.Play();
        }
        else
        {
            Debug.LogError("Loaded object is not a PlayableDirector at path: " + path);
        }
    }
    else
    {
        Debug.LogError("Timeline not found at path: " + path);
    }
    }
}
