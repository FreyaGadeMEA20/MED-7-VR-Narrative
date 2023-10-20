using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Timeline_Change : MonoBehaviour
{
    [SerializeField]
    private Playable[] Directors;
   
    public GameObject[] Timelines;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimelineChange());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TimelineChange()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(2);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time +". Now ready for Input!");
    }
}
