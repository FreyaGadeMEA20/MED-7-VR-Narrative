using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CromaAb : MonoBehaviour
{

    float currentSceneTime;
    float NormalizedCurrentSceneTime;
    [SerializeField] GameObject CromaBox;
    [SerializeField] Transform CromaBoxStartPos;
    [SerializeField] Transform CromaBoxEndPos;
    int Step = 0;
    float timer = 0;
    float NormalizedTimer = 0;
    float PosDistanceInPercent;
    float NormalizedPosDistanceInPercent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        switch (Step)
        {
            case 0:
                CromaBox.transform.position = CromaBoxStartPos.position;

                currentSceneTime = 60f;
                NormalizedCurrentSceneTime = (currentSceneTime - 0) / (1 - 0);

                Step = 1;
                break;
            case 1:

                NormalizedTimer = (timer - 0) / (1 - 0);

                PosDistanceInPercent = (NormalizedTimer / currentSceneTime) * 100;

                NormalizedPosDistanceInPercent = (PosDistanceInPercent - 0) / (1 - 0);

                CromaBox.transform.position = Vector3.MoveTowards(CromaBox.transform.position, CromaBoxEndPos.position, NormalizedPosDistanceInPercent);

                if(NormalizedTimer >= NormalizedCurrentSceneTime)
                {
                    Step = 0;
                }

                break;
        }

    }
}
