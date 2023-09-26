using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State//구조체
    {
        RETURN,//복귀
        STOP, //정지
        TRACE,//추적
        ATTACK,//공격
        PATROL,
        DIE//뒤짐
    }
    
    public State state = State.RETURN;
    // [SerializeField]
    // EnemyRaycast raycast;
    NavMeshAgent nav;
    // public Transform playerTr;
    [SerializeField]
    float speed = 2.0f;      //속도
    [SerializeField]
    GameObject target;        //점령구역의 위치
    GameObject pl;
    Transform playerTr;
    [SerializeField]
    Vector3 Pos;          //타겟의 위치
    public bool isDie = false;
    float area_in=10;
    public float attackDist = 5.0f;
    //추적 사정거리
    public float traceDist = 10.0f;
    float Pdist;
    [SerializeField]
    EnemyView view;

    [SerializeField]
    Rigidbody rb;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        rb=GetComponent<Rigidbody>();
       // animator = GetComponent<Animator>();
       // animator.SetTrigger(hashRun);
        //target = GameObject.Find("area").GetComponent<Transform>();
        nav.speed = speed;
        
        Pos=target.transform.position;
        
        if(!nav.pathPending)     
        {
            nav.SetDestination(Pos);
            StartCoroutine("CheckState");
            StartCoroutine("Go");
        }

    }

    void Update() {
        //rb.velocity =Vector3.zero;
    }

    public IEnumerator CheckState()
    {
        //적 캐릭터가 사망하기 전까지 도는 무한루프
        while (!isDie)
        {
            //상태가 사망이면 코루틴 함수를 종료시킴
            if (state == State.DIE)
                yield break;

            //적 캐릭터와 순찰구역 간의 거리를 계산
            float dist = Vector3.Distance(transform.position,target.transform.position);
            if(view.look/*raycast.look*/){
                playerTr=view.TelePos;
                //적 캐릭터와 플레이어 간의 거리를 계산
                Pdist = Vector3.Distance(transform.position,playerTr.position);
            }
                //공격 사정거리 이내인 경울
                if (view.look&&Pdist <= attackDist)
                {
                    state = State.ATTACK;
                }
                //추적 사정거리 이내인 경우
                else if (view.look&&Pdist <= traceDist)//이것이 공격 사정거리에 없을 시(공격거리가 아닐시 더 확장해서(아마 범위 일것이다) 실행 되고 공격과 뒤바뀌면 traceDist와 충돌이 발생할 수 있다.
                {
                    state = State.TRACE;
                }
                ////순찰구역의 위치가 자신과 떨어져 있을 때
                else if (dist >= area_in)
                {
                    state = State.RETURN;
                }
                else
                {
                    state = State.STOP;
                }
            
                
            
            //0.3초 대기하는 동안 제어건을 양보
            yield return new WaitForSeconds(0.3f);//yield문을 넣으면 이 구간 0.3초 동안 대기 즉 이 구문이 실행되는 동안 다른 루틴으로 빠져서 다른건 안가리 킨다. 그후 0.3초가 다 됬는지 코루틴이 확인후 다 되면 다시 이 구문으로 돌아와 실행된다. 
        }
    }
    public IEnumerator Go(){
        //적 캐릭터가 사망할때 까지 무한루프
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);//위에 설명되어 있다.
            
            //상태에 따라 분기 처리
            switch(state)
            {
                case State.RETURN:
                    Pos=target.transform.position;    //타겟의 위치를 순찰구역으로
                    //nav.ResetPath();
                    view.look=false;
                    move(Pos);
                    break;
                case State.STOP:
                    Stop();
                    break;
                case State.TRACE:
                    nav.isStopped=false;
                    //총알 발사 정지
                    //enemyFire.isFire = false;
                    //주인공의 위치를 넘겨 추적모드로 변경
                    Pos = playerTr.position;
                    move(Pos);
                    //animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    //순찰 및 추적을 정지
                    Stop();
                    //animator.SetBool(hashMove, false);

                    /*//총알 발사 시작
                    if (enemyFire.isFire == false)
                        enemyFire.isFire = true;*/
                    break;
                case State.DIE:
                    isDie = true;
                    //enemyFire.isFire = false;
                    //순찰 및 추적을 정지
                    Stop();
                    //사망 애니메이션 의 종류를 지정
                    //animator.SetInteger(hashDieIdx, Random.Range(0, 3));
                    //사망 에니메이션 실행
                    //animator.SetTrigger(hashDie);
                    //Capsule Collider 컴포넌트를 비활성화
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
}
