using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        RETURN,
        STOP,
        TRACE,
        ATTACK,
        PATROL,
        DIE
    }
    
    public State state = State.STOP;
    NavMeshAgent nav;
    [SerializeField]
    float speed = 2.0f;      //속도

    [SerializeField]
    bool patroler = true; //순찰을 하는 타입의 병사인지 확인
    Vector3 startPos;        //순찰구역의 위치

    Vector3 playerTr;
    [SerializeField]
    Vector3 Pos;          //타겟의 위치
    public bool isDie = false;
    float area_in=10;
    public float attackDist = 15.0f;//공격 사정거리
    public float traceDist = 30.0f;//추적 사정거리
    float Pdist;  //플레이어와의 거리


    [SerializeField]
    EnemyView view;

    [SerializeField]
    Rigidbody rb;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        rb=GetComponent<Rigidbody>();
        view =GameObject.Find("ray").GetComponent<EnemyView>(); //자식 오브젝트에 있는 EnemyView 가져옴
        nav.speed = speed;
        startPos = transform.position;  //처음 위치한 구역을 기준으로 순찰        
        if(!nav.pathPending)     
        {
            StartCoroutine("CheckState");
            StartCoroutine("Go");
        }

    }
    private void Update()
    {
        rb.velocity = Vector3.zero; //충돌 시 미끄러지는거 방지
    }

    public IEnumerator CheckState()
    {
        while (!isDie)
        {
            if (state == State.DIE)
                yield break;

            //적 캐릭터와 순찰구역 간의 거리를 계산
            float dist = Vector3.Distance(transform.position,startPos);
            if(view.look/*raycast.look*/){
                playerTr=view.TelePos;
                //적 캐릭터와 플레이어 간의 거리를 계산
                Pdist = Vector3.Distance(transform.position,playerTr);
            }
                //공격이 가능하고 공격 사정거리 이내인 경우
                if (view.att&&Pdist <= attackDist)
                {
                    state = State.ATTACK;
                }
                //추적 사정거리 이내인 경우
                else if (view.look&&Pdist <= traceDist)
                {
                    state = State.TRACE;
                //Debug.Log(Pdist);
                }
                ///순찰구역의 위치가 자신과 떨어져 있을 때
                else if (dist >= area_in)
                {
                    state = State.RETURN;
                }
                else if (state!=State.RETURN) //주위에 적이 없고 복귀가 끝났을 때
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
    public IEnumerator Go(){
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);
            
            //상태에 따라 분기 처리
            switch(state)
            {
                case State.RETURN:
                    nav.isStopped = false;
                    Pos =startPos;    //타겟의 위치를 순찰구역으로
                    //nav.ResetPath();
                    view.look=false;
                    move(Pos);
                    break;
                case State.STOP:
                    Stop();
                    break;
                case State.TRACE:
                    nav.isStopped=false;
                    //주인공의 위치를 넘겨 추적모드로 변경
                    Pos = playerTr;
                    move(Pos);
                    break;
                case State.ATTACK:
                    Stop();
                    //공격 시작(추가 예정)

                    break;
                case State.PATROL:
                    nav.isStopped = false;
                    if (Vector3.Distance(Pos, this.transform.position) <= 1f)
                    {
                        Pos = patroling();
                        Debug.Log(1);
                    }
                    move(Pos);
                    break;
                case State.DIE:
                    isDie = true;
                    Stop();
                    //사망 애니메이션(추가 예정)

                    GetComponent<CapsuleCollider>().enabled = false;
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

    Vector3 patroling() //순찰 목적지 정하기
    {
        float xPos = startPos.x + Random.Range(1, 5);
        float zPos = startPos.z + Random.Range(1, 5);
        return new Vector3(xPos, startPos.y, zPos);
    }
}
