using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "Sequence_", menuName = "ScriptableObjects/SequenceTimelineScriptableObject", order = 1)]
public class SequenceTimelineScriptableObject : ScriptableObject
{
    //public PlayableDirector currentDirector;
    public TimelineAsset currentTimeline;
    public TimelineAsset directorUnder;
    public TimelineAsset directorOver;
    public SequenceTimelineScriptableObject nextSequence;

    // POI timestamp.
    public int switchTime;
    public int switchTimeU;
    public int switchTimeO;

    public bool OBR;

    public void SetNext(bool over){
        if(over){
            nextSequence.currentTimeline = directorOver;
            nextSequence.switchTime = switchTimeO;
        } else {
            nextSequence.currentTimeline = directorUnder;
            nextSequence.switchTime = switchTimeU;
        }
    }
}
