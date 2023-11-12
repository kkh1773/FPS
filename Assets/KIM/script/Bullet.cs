using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 0;
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
                //플레이어 hp있는 컴포넌트 불러깍음
            }
        }else if (gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                //적의 hp 깍음
                EnemyAi e = collision.gameObject.GetComponent<EnemyAi>();
                e.hp -= damage;
                e.takeDamge();
            }
        }
        Destroy(gameObject);
    }


}
