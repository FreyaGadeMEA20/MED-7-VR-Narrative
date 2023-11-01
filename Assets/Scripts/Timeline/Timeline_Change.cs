using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Timeline_Change : MonoBehaviour
{

    public int switchTime;
    public int buffer;
    private int cd = 0;

    public List<PlayableDirector> playableDirectors;
    private PlayableDirector playableDirector_Current;

    // Start is called before the first frame update
    void Start()
    {
        playableDirector_Current = playableDirectors[cd];
        playableDirector_Current.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("The scene has been running for " + (int)Time.time + " seconds");
        //Debug.Log("The timeline has been running for " + (int)playableDirector_Current.time + " seconds");
        
        if(playableDirector_Current.time > buffer && cd != 7 || cd != 8 || cd != 13 || cd != 14 || cd != 19 || cd != 20)
        {
            SwitchTimeline();
        } else if (playableDirector_Current.time > buffer)
        {
            SwitchTimelineOBR();
        }
    }

    public void SwitchTimeline()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playableDirector_Current.time < switchTime)
        {
            Debug.Log ("Timeline switched to" + cd + " at " + playableDirector_Current.time + " seconds");
            cd += 1;
            if (cd % 2 == 0)
            {
                cd +=1;
            }
            playableDirector_Current = playableDirectors[cd];
            playableDirector_Current.Play();
        }   
        else if (Input.GetKeyDown(KeyCode.Space) && playableDirector_Current.time > switchTime)
        {
            Debug.Log ("Timeline switched to" + cd + " at " + playableDirector_Current.time + " seconds");
            cd += 2;
            playableDirector_Current = playableDirectors[cd];
            playableDirector_Current.Play();
        }
    }

    public void SwitchTimelineOBR()
    {

    }
}
