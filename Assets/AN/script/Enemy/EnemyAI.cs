using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public enum State
    {
        DIE,
        SIT,
        ATTACK,
        TRACE,
        FIND,
        RETURN,
        PATROL,
        STOP  
    }
    
    public State state = State.STOP;
    public NavMeshAgent nav;
    public Transform tr;
    public Rigidbody rb;

    [Header("Player Setting")]
    [SerializeField]
    float defaultSpeed = 2.0f;//기본 속도
    float maxSpeed = 5.0f;//최대 속도
    float hp = 100.0f;//채력

    [SerializeField]
    bool patroler = true; //순찰을 하는 타입의 병사인지 확인
    Vector3 startPos;     //순찰구역의 위치

    
    [SerializeField]
    Vector3 Pos;          //타겟의 위치
    Vector3 playerTr; //타겟의 마지막 위치

    float area_in=10;
    public float attackDist = 15.0f;//공격 사정거리
    public float traceDist = 30.0f;//추적 사정거리
    float Pdist;  //플레이어와의 거리

    [Header("check Setting")]
    public bool isDie = false;
    public bool isFind = false;
    public bool isLook=false;


    [SerializeField]
    EnemyView enemyView;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        enemyView = GetComponent<EnemyView>();



    }


    void Start()
    {
        
        nav.speed = defaultSpeed;
        startPos = tr.position;  //처음 위치한 구역을 기준으로 순찰
        isLook = enemyView.look;                                

        if(!nav.pathPending)     
        {
            StartCoroutine("CheckState");
            StartCoroutine("Go");
        }

    }
    private void Update()
    {
       //rb.velocity = Vector3.zero; //충돌 시 미끄러지는거 방지
    }

    public IEnumerator CheckState()
    {
        while (!isDie)
        {

            if (state == State.DIE)
                yield break;
            else if (state == State.SIT)
            {

            }

            //적 캐릭터와 순찰구역 간의 거리를 계산
            float dist = Vector3.Distance(tr.position, startPos);
            playerTr = enemyView.TelePos;
           
            //적 캐릭터와 플레이어 간의 거리를 계산
            Pdist = Vector3.Distance(tr.position, playerTr);


            if (isLook == true)
            {
 
                //공격이 가능하고 공격 사정거리 이내인 경우
                if (Pdist <= attackDist)
                {
                    state = State.ATTACK;
                }
                //추적 사정거리 이내인 경우
                else if (Pdist <= traceDist)
                {
                    state = State.TRACE;
                    
                }

            }
            else if (isLook == false) {
                ///순찰구역의 위치가 자신과 떨어져 있을 때
                if (Pdist <= traceDist)
                {
                    state = State.FIND;
                }

                else if (dist >= area_in)
                {
                    state = State.RETURN;
                }

                else if (state != State.RETURN) //주위에 적이 없고 복귀가 끝났을 때
                {
                    state = State.PATROL;
                }

                else
                {
                    state = State.STOP;
                }

                yield return new WaitForSeconds(0.3f);
            }

        }
    }
    public IEnumerator Go(){
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);
            
            //상태에 따라 분기 처리
            switch(state)
            {

                case State.DIE:
                    isDie = true;
                    Stop();
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
                //사망 애니메이션(추가 예정)
                case State.ATTACK:
                    Stop();
                    //공격 시작(추가 예정)
                    break;
                case State.TRACE:
                    nav.isStopped=false;
                    nav.speed = maxSpeed;
                    //주인공의 위치를 넘겨 추적모드로 변경
                    Pos = playerTr;
                    move(Pos);
                    break;

                case State.FIND:
                    nav.isStopped = false;
                    nav.speed = maxSpeed;
                    //주인공의 위치를 넘겨 추적모드로 변경
                    Pos = playerTr;
                    move(Pos);
                    break;

                case State.RETURN:
                    nav.isStopped = false;
                    nav.speed = defaultSpeed;
                    Pos = startPos;//타겟의 위치를 순찰구역으로
                    move(Pos);
                    break;
                case State.PATROL:
                    nav.isStopped = false;
                    nav.speed = defaultSpeed;
                    if (Vector3.Distance(Pos, tr.position) <= 1f)
                    {
                        //Pos = patroling();
                        
                    }
                    move(startPos);
                    break;

                case State.STOP:
                    Stop();
                    break;

            }
        }
        
    }
    void Stop(){
        nav.isStopped=true;
        
        nav.velocity=Vector3.zero;
    }

    void move(Vector3 pos){
        this.Pos=pos;
        nav.SetDestination(Pos);
    }

    //Vector3 patroling() //순찰 목적지 정하기
    //{
       // float xPos = startPos.x + Random.Range(1, 5);
       // float zPos = startPos.z + Random.Range(1, 5);
       // return new Vector3(xPos, startPos.y, zPos);
   // }
}
