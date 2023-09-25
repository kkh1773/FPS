using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadGun : MonoBehaviour
{
    [SerializeField]
    private GameObject myGun;
    private Animation myAnim;
    private GameObject gunmaster;
    private GunMaster a;
    int rand;
    void Start()
    {
        gunmaster=GameObject.Find("GunMaster");
        a=gunmaster.GetComponent<GunMaster>();
        rand=Random.Range(0,a.guns.Length);
        Debug.Log(a.guns.Length);
        myGun=Instantiate(a.guns[rand],transform.position,Quaternion.identity);
        myGun.transform.parent=this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
