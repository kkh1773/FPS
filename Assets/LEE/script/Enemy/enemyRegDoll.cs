using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyRegDoll : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Transform tr;
    public  Transform chTr;
    public float destructionDelay = 5.0f;

    public GameObject charObj;
    public GameObject regdollObj;
    

    public void changeRegdoll()
    {
        tr.SetPositionAndRotation(chTr.position, chTr.rotation);
        charObj.SetActive(false);
         regdollObj.SetActive(true);
        UnityEngine.Debug.Log("cas");

      rb.AddForce(new Vector3(100f, 100f, -100f), ForceMode.Impulse);

       Destroy(gameObject, destructionDelay);


    }

    
}
