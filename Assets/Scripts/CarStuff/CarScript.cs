using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
public bool isReverse;
[SerializeField] int speed;
[SerializeField] float destroySpeed;

[SerializeField] Transform Wheel_1;
[SerializeField] Transform Wheel_2;
[SerializeField] Transform Wheel_3;
[SerializeField] Transform Wheel_4;




    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestoryActor());
        if (isReverse == false)
        {
            transform.Rotate(0, 180, 0);
        }
    }


    // Update is called once per frame
    void Update()
    {
            transform.position += transform.forward * Time.deltaTime * speed;


            Wheel_1.Rotate(180, 0, 0 * Time.deltaTime);
            Wheel_2.Rotate(180, 0, 0 * Time.deltaTime);
            Wheel_3.Rotate(180, 0, 0 * Time.deltaTime);
            Wheel_4.Rotate(180, 0, 0 * Time.deltaTime);
    }


    IEnumerator DestoryActor()
    {
        yield return new WaitForSeconds(destroySpeed);
        Destroy(this.gameObject);
    }
}
