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
        
        Collider[] Targets = Physics.OverlapSphere(myPos, ViewRadius,TargetMask);

        if (Targets.Length == 0) return;
        foreach(Collider EnemyColli in Targets)
        {
            Vector3 targetPos = EnemyColli.transform.position;
            Vector3 targetDir = (targetPos - myPos).normalized;
            float targetDis=Vector3.Distance(myPos,targetPos);
            float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;
            if(targetAngle <= ViewAngle * 0.5f && !Physics.Raycast(myPos, targetDir, targetDis, ObstacleMask))
            {
                look=true;
                hitTargetList.Add(EnemyColli);
                TelePos=EnemyColli.transform;
                if (DebugMode) Debug.DrawLine(myPos, targetPos, Color.red);
            }
            
        }
        hitTargetList.Clear();
        
    }

    Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

}
