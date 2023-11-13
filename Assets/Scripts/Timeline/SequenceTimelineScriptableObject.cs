using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SequenceTimelineScriptableObject", order = 1)]
public class SequenceTimelineScriptableObject : ScriptableObject
{
    public PlayableDirector currentDirector;
    public PlayableDirector directorUnder;
    public PlayableDirector directorOver;
    public SequenceTimelineScriptableObject nextSequence;

    // POI timestamp.
    public int switchTime;
    public int switchTimeU;
    public int switchTimeO;

    public bool OBR;

    public void SetNext(bool over){
        if(over){
            nextSequence.currentDirector = directorOver;
            nextSequence.switchTime = switchTimeO;
        } else {
            nextSequence.currentDirector = directorUnder;
            nextSequence.switchTime = switchTimeU;
        }
    }
}
