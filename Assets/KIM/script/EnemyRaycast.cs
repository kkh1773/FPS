using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRaycast : MonoBehaviour
{
    public float lineSize = 16f;
    public bool look=false;
    public RaycastHit hit;
    public GameObject pl;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckPlayer());
    }

    
    public IEnumerator CheckPlayer(){
        
        while(!look){
            Debug.DrawRay(transform.position, transform.forward * lineSize, Color.yellow);
            //RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, lineSize))
            {
                if(hit.collider.gameObject.tag=="Player"){
                    look=true;
                    pl=hit.collider.gameObject;
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}


