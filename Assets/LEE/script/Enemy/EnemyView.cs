using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyView : MonoBehaviour
{
    //미끄럼 방지를 위해 velocity 초기화시켰더니 레이캐스트 발사가 안됨 그래서 일단 자식 오브젝트 만들어서 거기에 넣었음



    [SerializeField] bool DebugMode = false;
    [Range(0f, 360f)] [SerializeField] float ViewAngle = 0f;
    [SerializeField] float ViewRadius = 1f;
    [SerializeField] LayerMask TargetMask;
    [SerializeField] LayerMask ObstacleMask;
    
    [SerializeField]List<Collider> hitTargetList = new List<Collider>();

    public Transform tr;
    
    public Vector3 TelePos;
    public bool look=false;  //발견 시
    [SerializeField]
    Collider[] Targets;

    public Vector3 r;
    // Start is called before the first frame update


    void Start()
    {
        tr = GetComponent<Transform>();
        DebugMode =true;
        
       
    }


    //정규화 시켜야 될것같다.

    private void OnDrawGizmos() { //기즈모 안에 들어왔을 때
        if (!DebugMode)
        {
            return;
        }

        Vector3 myPos = tr.position + Vector3.up* 2.0f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);
        float lookingAngle = tr.eulerAngles.y;  //캐릭터가 바라보는 방향의 각도
        Vector3 rightDir = AngleToDir(tr.eulerAngles.y + ViewAngle * 0.5f); //오른쪽 시야각 끝 지정
        Vector3 leftDir = AngleToDir(tr.eulerAngles.y - ViewAngle * 0.5f);  //왼쪽 시야각 끝 지정
        Vector3 lookDir = AngleToDir(lookingAngle);                                 // 중앙선
        UnityEngine.Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        UnityEngine.Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        UnityEngine.Debug.DrawRay(myPos, lookDir * ViewRadius, Color.cyan);

       
        Targets = Physics.OverlapSphere(myPos, ViewRadius,TargetMask);  //범위에 있는 오브젝트 중 레이어가 TargetMask에 지정되있는 것들 전부 가져옴


        if (Targets.Length == 0)
        {

            if (Vector3.Distance(myPos, TelePos) <= 0.5f)//마지막으로 본 곳에 도착했는데 못찾을 경우
            {
                UnityEngine.Debug.Log("a");
                look = false;  //놓침    
            }

            return;
        }
       else if (Vector3.Distance(myPos, TelePos) <= 0.5f)//마지막으로 본 곳에 도착했는데 못찾을 경우
        {
         UnityEngine.Debug.Log("b");
           look = false;  //놓침    
         }

        foreach (Collider EnemyColli in Targets)
        {

            Vector3 targetPos = EnemyColli.transform.position+Vector3.up*2.0f;//플레이어 위치
            Vector3 targetDir = (targetPos - myPos).normalized; //플레이어와 적 사이의 방향벡터
            float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg; //플레이어와 적 사이의 각도
            float targetdis = Vector3.Distance(tr.position, targetPos); //거리

            r = targetPos;
            if (targetAngle <= ViewAngle * 0.5f/*플레이어가 시야각 안에 있을 때 */ && targetdis<(ViewRadius-0.6)/*시야 거리 안에 있을 때*/&&!Physics.Raycast(myPos, targetDir, targetdis, ObstacleMask)/*사이에 벽이 없을 때*/)
            {
                if (DebugMode)
                {
                    UnityEngine.Debug.DrawLine(myPos, targetPos, Color.red);
                }


                TelePos = targetPos;//위치 기록 AI스크립트에 넘길 것
                if (hitTargetList.Contains(EnemyColli) != true) //기존에 해당 오브젝트가 hitTargetList에 없을 때 추가
                {
                    hitTargetList.Add(EnemyColli);
                    look = true; //발견
                   

                    UnityEngine.Debug.Log("start");
                }

            }
            else if (((ViewAngle * 0.5f) <= targetAngle) || Physics.Raycast(myPos, targetDir, targetdis, ObstacleMask)||ViewRadius-targetdis<=0.6)//위에 조건 중 하나라도 걸리면 실행
            {
                hitTargetList.Remove(EnemyColli); //hitTargetList에서 제거
                look = false; //발견

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
