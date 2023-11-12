using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemySoldierAi : MonoBehaviour
{
    //각 상태에 따른 열거형 선언
    public enum State
    {
        DIE,
        SIT,
        RERODING,
        ATTACK,
        TRACE,
        STOP
    }

    //기본적으로 필요한 값들 변수 선언
    public State state = State.STOP;// 열거형 변수 선언 초기화 
    public NavMeshAgent nav;// 네비게이션 추가
    public Transform tr;//해당 오브젝트 transform 변수선언
    public Rigidbody rb;//해당 오브젝트 rigidbody 변수 선언

    //위치, 회전, 기타 관련 설정
    Vector3 Pos;        //이동할 위치
    Vector3 playerTr;   //타겟의 마지막 위치
    Vector3 startPos;   //순찰구역의 위치

    [Header("Player Setting")]
    [SerializeField] float defaultSpeed = 2.0f;//기본 속도
    [SerializeField] float maxSpeed = 5.0f;//최대 속도
    [SerializeField] float hp = 100.0f;//채력

    [Header("check Setting")]
    public bool isDie = false;// 죽었나?
    public bool isLook = false;// 발견했는가?

    [Header("Animation")]
    public Animator animator;//에니메이션 변수 선언
    public float walkAndRunCount = 0; //move blend tree관련 값 설정
    private readonly int hashwalk = Animator.StringToHash("walk");//walk hash값 저장 변수 선언
    private readonly int hashChWalkAndRun = Animator.StringToHash("chWalkAndRun");//chWalkAndRun hash값 저장 변수 선언

    [Header("another C#script")]
    public EnemyView enemyView;//시야각 조정 스크립트 변수
    public enemyRegDoll enemyRegdoll;//레그돌 설정 스크립트 변수




    void Start()
    {
        //초기값 저장
        tr = GetComponent<Transform>();//transform 값 저장
        nav = GetComponent<NavMeshAgent>();//nav값 저장
        rb = GetComponent<Rigidbody>();// rigidbode값 저장
        animator = GetComponent<Animator>();//animatior값 저장
        enemyView = GetComponent<EnemyView>();// EnemyView스크립트 저장
        enemyRegdoll = GetComponentInParent<enemyRegDoll>();//enemyRegDoll스크립트 저장

        //초기값 설정
        nav.speed = defaultSpeed;//기본속도로 설정
        isLook = enemyView.look;//현재 타깃식별상태값저장
        playerTr = GameObject.FindGameObjectWithTag("Player").transform.position;//player 태그를 가진 오브젝트 위지값 넘겨줌

        if (!nav.pathPending)//계산중이지만 아직 준비가 되지 않는 경로(path)를 나타냅니다라는 뜻(읽기전용) false면 게산이 완료 되었다는 뜻이다.
        {
            StartCoroutine("CheckState");//상태확인 코루틴 실행
            StartCoroutine("Go");//행동 코루틴 실행
        }

    }
    private void Update()
    {
        //rb.velocity = Vector3.zero; //충돌 시 미끄러지는거 방지
        isLook = enemyView.look;//현재 타깃식별상태값저장

    }

    //상태 체크 코루틴 함수
    public IEnumerator CheckState()
    {
        //죽은 상태라면
        if (isDie)
        {
            yield break;//코루틴 종료
        }

        //죽지 않았다면 
        while (!isDie)
        {

            //hp가 없다면 (아직 완성 안됨)
            if (hp <= 0)
            {
                state = State.DIE;//주금

            }

            //제작중
            else if (state == State.SIT)
            {

            }

            //공격 범위에 들어왔다면
            if (isLook == true)
            {
                state = State.ATTACK;//공격상태로 전횐
            }

            //공격 범위에 들어와 있지 않다면
            else if (isLook == false)
            {
                state = State.TRACE;//추적상태로 전환
            }

            else
            {
                state = State.STOP;//멈춰!
            }

            yield return new WaitForSeconds(0.3f);

        }
    }
    public IEnumerator Go()
    {

        if (isDie)
        {
            yield break;//코루틴 종료
        }

        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            //상태에 따라 분기 처리
            switch (state)
            {

                //죽었을때
                case State.DIE:
                    isDie = true;//죽음 활성화
                    Stop();//임시방편
                    GetComponent<CapsuleCollider>().enabled = false;//기존 콜라이더 삭제
                    enemyRegdoll.chTr = tr;//레그돌스크립트로 현재 위치 전달
                    enemyRegdoll.changeRegdoll();//레그돌로 바꾸는 함수 호출
                    break;//while문 나가기

                //공격상태
                case State.ATTACK:
                    playerTr = enemyView.TelePos;//식별된 타겟의 위치값 전달
                    Pos = playerTr;//적의 위치값 전달
                    Stop();
                    //공격 시작(추가 예정)
                    break;

                //추적상태
                case State.TRACE:
                    nav.isStopped = false;//nav 정지상태 해제
                    nav.speed = maxSpeed;//최대속도로 변경
                    playerTr = GameObject.FindGameObjectWithTag("Player").transform.position;//player 태그를 가진 오브젝트 위지값 넘겨줌
                    Pos = playerTr;//적의 위치값 전달
                    move(Pos);//이동함수로 목적지전달
                    WalkAnimation();//이동애니메이션 함수로 전달
                    break;

                //멈춤상태
                case State.STOP:
                    Stop();//멈춤 실행
                    break;

            }
        }

    }

        //멈춤 함수
        void Stop()
        {
            nav.isStopped = true;////nav 정지상태 
            animator.SetBool(hashwalk, false);//움직임 확인 해쉬 false로 전환

            // nav.velocity = Vector3.zero;
        }

    void WalkAnimation()
    {
        animator.SetBool(hashwalk, true);//움직임 확인 해쉬 ture로 전환

        if (state == State.TRACE && nav.speed == maxSpeed)//추적상태이고 속도가 최대속도라면
        {

            animator.SetFloat(hashChWalkAndRun, 1);
            //서서히 1로 늘리기(1은 달리기애니메이션 재생
            /* if (walkAndRunCount < 1)
             {

                walkAndRunCount += Time.deltaTime;
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 30.0f));
             }*/
        }
        else//그 외외 상황
        {

            animator.SetFloat(hashChWalkAndRun, 0);
            //서서히 0로 늘리기(0은 걷는 애니메이션 재생
            /*if (walkAndRunCount >= 0)
            {
                walkAndRunCount -= Time.deltaTime;
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 30.0f));
            }*/


        }

    }

    //이동함수
    void move(Vector3 pos)
    {
        nav.SetDestination(pos);//받은 목적지로 이동시작

    }



}
