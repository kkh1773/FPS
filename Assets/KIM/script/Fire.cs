using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    GameObject firepos;
    Transform firetr;
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        firepos=GameObject.Find("FirePos");
        firetr=firepos.transform;
    }

    public void fire(){
        Instantiate(bullet,firetr.position,firetr.rotation);
    }
}
