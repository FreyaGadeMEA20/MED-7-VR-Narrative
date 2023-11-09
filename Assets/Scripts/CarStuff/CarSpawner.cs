using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> Cars = new List<GameObject>();
    [SerializeField] bool carDir;
    [SerializeField] float initalTime;
    float timeLeft;
    GameObject spawnedCar;

    // Start is called before the first frame update
    void Start()
    {
        timeLeft = initalTime;
    }

    // Update is called once per frame
    void Update()
    {
        

            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                initalTime = Random.Range(2.0f, 8f);
                timeLeft = initalTime;
                
                spawnedCar= Instantiate(Cars[Random.Range(0,4)], this.gameObject.transform.position, Quaternion.identity * Quaternion.Euler(new Vector3(0, 0, 0)));
            }
            if (carDir == true && spawnedCar)
            {
                spawnedCar.GetComponent<CarScript>().isReverse = true;
            }
        
    }


}
