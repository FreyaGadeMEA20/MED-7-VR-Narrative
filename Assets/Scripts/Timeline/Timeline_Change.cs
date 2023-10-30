using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Timeline_Change : MonoBehaviour
{

    public int switchTime;

    public List<PlayableDirector> playableDirectors;
    private PlayableDirector playableDirector_Current;

    // Start is called before the first frame update
    void Start()
    {
        playableDirector_Current = playableDirectors[0];
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log((int)Time.time);

        StartCoroutine(Timeline_Switch());
    }

    IEnumerator Buffer()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 2 seconds.
        yield return new WaitForSeconds(2);

        //After we have waited 2 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time +". Now ready for Input!");

    }

    IEnumerator Timeline_Switch()
    {
        yield return new WaitForSeconds(2);

        if (Input.GetKeyDown(KeyCode.Space) && playableDirector_Current.time < switchTime)
        {
            Debug.Log ("Logged input result A");
            playableDirector_Current = playableDirectors[1];
            
        } 
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log ("Logged input result B");
            playableDirector_Current = playableDirectors[2];
        }
    }
}
