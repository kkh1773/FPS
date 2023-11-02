using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down*Time.deltaTime*10,Space.Self);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.CompareTag("Player"))
            {

            }
        }else if (gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {

            }
        }
        
    }


}
