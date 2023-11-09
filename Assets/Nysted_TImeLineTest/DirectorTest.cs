using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DirectorTest : MonoBehaviour
{
    public List<PlayableDirector> playableDirectors;
    private PlayableDirector playableDirector_main;
    // Start is called before the first frame update
    void Start()
    {
        playableDirector_main = playableDirectors[0];
    }

    // Update is called once per frame
    void Update()
    {
        setDirector();
        playDirector();

    }

    void setDirector()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            playableDirector_main = playableDirectors[0];
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playableDirector_main = playableDirectors[1];
        }
    }

    void playDirector()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playableDirector_main.Play();
        }
    }
}
