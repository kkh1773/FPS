using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class EnemyViewTest : MonoBehaviour
{
    static public EnemyView instance;
    /*private void Awake()
    {
        #region 싱글톤
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        #endregion
    }*/

    [SerializeField] bool DebugMode = false;
    [Range(0f, 360f)] [SerializeField] float ViewAngle = 0f;
    [SerializeField] float ViewRadius = 1f;
    [SerializeField] LayerMask TargetMask;
    [SerializeField] LayerMask ObstacleMask;
    [SerializeField] List<Collider> hitTargetList = new List<Collider>();
    [SerializeField] int i = 0;
    [SerializeField] Collider[] Targets;
    [SerializeField] Vector3 myPos;
    [SerializeField] Vector3 x;
    [SerializeField] Vector3 y;
    [SerializeField] Vector3 z;
    [SerializeField] float Vs;

    // Start is called before the first frame update
    void Start()
    {
        hitTargetList.Clear();
    }

    // Update is called once per frame
    void Update()
    {

    }

 
    private void OnDrawGizmos() {
        if (!DebugMode) return;
         myPos = transform.position + Vector3.up * 2f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);
        float lookingAngle = transform.eulerAngles.y;  //캐릭터가 바라보는 방향의 각도
         Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
         Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
         Vector3 lookDir = AngleToDir(lookingAngle);
            x = rightDir;
            y = leftDir;
            z = lookDir;




        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.cyan);

        int index = 0;

        Targets = Physics.OverlapSphere(myPos, ViewRadius, TargetMask);

        if (Targets.Length == 0) return;
        foreach (Collider EnemyColli in Targets)
        {
            
            Vector3 targetPos = EnemyColli.transform.position;
            Vector3 targetDir = (targetPos - myPos).normalized;//방향 벡터
            UnityEngine.Debug.Log(targetDir);
            float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;// 각도구하는 식이구나
            float targetdis=Vector3.Distance(myPos, targetPos);

            Vs = targetAngle;
            if (targetAngle <= ViewAngle * 0.5f && targetdis < (ViewRadius - 0.3) && !Physics.Raycast(myPos, targetDir, targetdis, ObstacleMask))
            {
                if (DebugMode) Debug.DrawLine(myPos, targetPos, Color.red);

                if (hitTargetList.Contains(EnemyColli) != true)
                {
                    hitTargetList.Add(EnemyColli);
                    UnityEngine.Debug.Log("start");
                }

            }
            else if (((ViewAngle * 0.5f) <= targetAngle)|| (targetdis >= ViewRadius) || Physics.Raycast(myPos, targetDir, targetdis, ObstacleMask))
            {
                hitTargetList.Remove(EnemyColli);
            }
          
            
        }
    }
  

    Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }
    

}
