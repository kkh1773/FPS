using System.Collections;
using System.Collections.Generic;
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
    bool ishit=false;
    [SerializeField]List<Collider> hitTargetList = new List<Collider>();
    
    public Vector3 TelePos;
    public bool look=false;  //발견 시
    public bool att = false; //사이에 벽이 없어 공격 가능 상황 거리는 AI스크립트에서 고려
    [SerializeField]
    Collider[] Targets;
    // Start is called before the first frame update
    void Start()
    {
        DebugMode=true;
    }

    

    private void OnDrawGizmos() { //기즈모 안에 들어왔을 때
        if (!DebugMode) return;
        // this.transform.rotation = transform.parent.rotation; //계속 날아가길래 위치 맞춰줌
        // this.transform.position = transform.parent.position;
        Vector3 myPos = transform.position;//+ Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);
        float lookingAngle = transform.eulerAngles.y;  //캐릭터가 바라보는 방향의 각도
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f); //오른쪽 시야각 끝 지정
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);  //왼쪽 시야각 끝 지정
        Vector3 lookDir = AngleToDir(lookingAngle);                                 // 중앙선
        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.cyan);

       
        Targets = Physics.OverlapSphere(myPos, ViewRadius,TargetMask);  //범위에 있는 오브젝트 중 레이어가 TargetMask에 지정되있는 것들 전부 가져옴

        if (Targets.Length == 0)
        {
            if (Vector3.Distance(transform.position, TelePos) <= .5f) //플레이어가 범위 내에 있다가 나갔을 때 추적하다가 마지막으로 본 위치까지 오면 돌아가게 함
            {
                look = false;
            }
            return; //아무것도 못가져오면 재실행
        }
        
        foreach (Collider EnemyColli in Targets)
        {

            Vector3 targetPos = EnemyColli.transform.position;  //플레이어 위치
            Vector3 targetDir = (targetPos - myPos).normalized; //플레이어와 적 사이의 방향벡터
            float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg; //플레이어와 적 사이의 각도
            float targetdis = Vector3.Distance(myPos, targetPos); //거리

            if (targetAngle <= ViewAngle * 0.5f/*플레이어가 시야각 안에 있을 때 */ && targetdis<(ViewRadius-0.3)/*시야 거리 안에 있을 때*/&&!Physics.Raycast(myPos, targetDir, targetdis, ObstacleMask)/*사이에 벽이 없을 때*/)
            {
                if (DebugMode) Debug.DrawLine(myPos, targetPos, Color.red);
                look = true; //발견
                att = true;  //공격 가능
                //hitTargetList.Add(EnemyColli);
                TelePos = EnemyColli.transform.position;//위치 기록 AI스크립트에 넘길 것
                if (hitTargetList.Contains(EnemyColli) != true) //기존에 해당 오브젝트가 hitTargetList에 없을 때 추가
                {
                    hitTargetList.Add(EnemyColli);
                    UnityEngine.Debug.Log("start");
                }

            }
            else if (((ViewAngle * 0.5) <= targetAngle) || Physics.Raycast(myPos, targetDir, targetdis, ObstacleMask)||ViewRadius-targetdis<=0.6)//위에 조건 중 하나라도 걸리면 실행
            {
                hitTargetList.Remove(EnemyColli); //hitTargetList에서 제거
                att = false;                      //못때림
                if (Vector3.Distance(transform.position, TelePos) <= .5f)//마지막으로 본 곳에 도착했는데 못찾을 경우
                {
                    look = false;  //놓침
                }
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
