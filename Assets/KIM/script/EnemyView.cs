using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    [SerializeField] bool DebugMode = false;
    [Range(0f, 360f)] [SerializeField] float ViewAngle = 0f;
    [SerializeField] float ViewRadius = 1f;
    [SerializeField] LayerMask TargetMask;
    [SerializeField] LayerMask ObstacleMask;
    bool ishit=false;
    [SerializeField]List<Collider> hitTargetList = new List<Collider>();
    
    public Transform TelePos;
    public bool look=false;
    public bool att = false;
    [SerializeField]
    Collider[] Targets;
    // Start is called before the first frame update
    void Start()
    {
        DebugMode=true;
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnDrawGizmos() {
        if (!DebugMode) return;
        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);
        float lookingAngle = transform.eulerAngles.y;  //캐릭터가 바라보는 방향의 각도
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);
        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.cyan);

       
        Targets = Physics.OverlapSphere(myPos, ViewRadius,TargetMask);

        if (Targets.Length == 0) return;
        /*foreach(Collider EnemyColli in Targets)
        {
            Vector3 targetPos = EnemyColli.transform.position;
            Vector3 targetDir = (targetPos - myPos).normalized;
            float targetDis=Vector3.Distance(myPos,targetPos);
            float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;
            if(targetAngle <= ViewAngle * 0.5f && !Physics.Raycast(myPos, targetDir, targetDis, ObstacleMask))
            {
                look=true;
                att = true;
                hitTargetList.Add(EnemyColli);
                TelePos=EnemyColli.transform;
                if (DebugMode) Debug.DrawLine(myPos, targetPos, Color.red);
            }else if (targetAngle <= ViewAngle * 0.5f && Physics.Raycast(myPos, targetDir, targetDis, ObstacleMask))
            {
                att = false;
            }


        }*/
        foreach (Collider EnemyColli in Targets)
        {

            Vector3 targetPos = EnemyColli.transform.position;
            Vector3 targetDir = (targetPos - myPos).normalized;
            UnityEngine.Debug.Log(targetDir);
            float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;
            float targetdis = Vector3.Distance(myPos, targetPos);

            
            if (targetAngle <= ViewAngle * 0.5f && targetdis<(ViewRadius-0.3)&&!Physics.Raycast(myPos, targetDir, targetdis, ObstacleMask))
            {
                if (DebugMode) Debug.DrawLine(myPos, targetPos, Color.red);

                if (hitTargetList.Contains(EnemyColli) != true)
                {
                    hitTargetList.Add(EnemyColli);
                    UnityEngine.Debug.Log("start");
                }

            }
            else if (((ViewRadius * 0.01) <= targetAngle) || Physics.Raycast(myPos, targetDir, targetdis, ObstacleMask)||ViewRadius-targetdis<=0.6)
            {
                hitTargetList.Remove(EnemyColli);
            }
            else if (hitTargetList.Contains(EnemyColli) != false)
            {
                hitTargetList.Remove(EnemyColli);
            }

        }
        //hitTargetList.Clear();
        
    }

    Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

}
