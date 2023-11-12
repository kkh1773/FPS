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
        RERODING,
        ATTACK,
        TRACE,
        FIND,
        RETURN,
        PATROL,
        STOP
    }

    //기본적으로 필요한 값들 변수 선언
    public State state = State.STOP;// 열거형 변수 선언 초기화 
    public NavMeshAgent nav;// 네비게이션 추가
    public Transform tr;//해당 오브젝트 transform 변수선언
    public Rigidbody rb;//해당 오브젝트 rigidbody 변수 선언

    //위치, 회전, 기타 관련 설정
    protected Vector3 Pos;        //이동할 위치
    protected Vector3 playerTr;   //타겟의 마지막 위치
    protected Vector3 startPos;   //순찰구역의 위치
    protected float Pdist;  //플레이어와의 거리
    protected float yPos = 0.0f;  //Y축 기준 회전 값   
    protected int sw = 1;// 회전 관련 스위치 변수(patrolling에 사용)

    [Header("Animation")]
    public Animator animator;//에니메이션 변수 선언
    public float walkAndRunCount = 0; //move blend tree관련 값 설정
    protected readonly int hashwalk = Animator.StringToHash("walk");//walk hash값 저장 변수 선언
    protected readonly int hashChWalkAndRun = Animator.StringToHash("chWalkAndRun");//chWalkAndRun hash값 저장 변수 선언
    protected readonly int hashdamage = Animator.StringToHash("damage");//damage hash값 저장 변수 선언 (공격받을 때)

    [Header("Player Setting")]
    [SerializeField] protected float defaultSpeed = 2.0f;//기본 속도
    [SerializeField] protected float maxSpeed = 5.0f;//최대 속도
    [SerializeField] public float hp = 100.0f;//채력
    //public float area_in = 0;//경계위치의 최대거리(필요없지만 혹시나 해서 남김)
    [SerializeField] protected float attackDist = 10.0f;//공격 사정거리
    [SerializeField] protected float traceDist = 20.0f;//추적 사정거리

    [SerializeField]
    protected bool patroler = true; //순찰을 하는 타입의 병사인지 확인

    protected float area_in=10;

    [Header("check Setting")]
    public bool isDie = false;// 죽었나?
    public bool isFind = false;//찾고있나?
    public bool isReturn = false;//다시 돌아가나
    public bool isLook = false;// 발견했는가?


    [Header("another C#script")]
    public EnemyView enemyView;//시야각 조정 스크립트 변수
    public enemyRegDoll enemyRegdoll;//레그돌 설정 스크립트 변수

    private void Awake()
    {
        tr = GetComponent<Transform>();
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        enemyView = GetComponent<EnemyView>();

        //초기값 설정
        nav.speed = defaultSpeed;//기본속도로 설정
        startPos = this.tr.position;  //처음 위치한 구역을 기준으로 순찰
        isLook = enemyView.look;//현재 타깃식별상태값저장
        Pos = startPos;//현 위치값 전달

    }


    public virtual void Start()
    {
        //초기값 설정
        nav.speed = defaultSpeed;//기본속도로 설정
        startPos = this.tr.position;  //처음 위치한 구역을 기준으로 순찰
        isLook = enemyView.look;//현재 타깃식별상태값저장
        Pos = startPos;//현 위치값 전달

        if (!nav.pathPending)//계산중이지만 아직 준비가 되지 않는 경로(path)를 나타냅니다라는 뜻(읽기전용) false면 게산이 완료 되었다는 뜻이다.
        {
            StartCoroutine("CheckState");//상태확인 코루틴 실행
            StartCoroutine("Go");//행동 코루틴 실행
        }
    }
    void Update()
    {
        //rb.velocity = Vector3.zero; //충돌 시 미끄러지는거 방지
        isLook = enemyView.look;//현재 타깃식별상태값저장
        patrolling();//경계근무 함수호출(제자리 회전)
    }

    public virtual IEnumerator CheckState()
    {
        Debug.Log(5);
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
    public virtual IEnumerator Go(){
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
    //멈춤 함수
    protected void Stop()
    {
        nav.isStopped = true;////nav 정지상태 
        animator.SetBool(hashwalk, false);//움직임 확인 해쉬 false로 전환

        // nav.velocity = Vector3.zero;
    }

    //이동 에니메이션 함수
    protected void WalkAnimation()
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
    protected void move(Vector3 pos)
    {
        nav.SetDestination(pos);//받은 목적지로 이동시작

    }

    //경계근무 함수(회전)
    protected void patrolling()
    {

        if (state == State.PATROL)//경계상태라면 
        {
            if (tr.eulerAngles.y >= 280 && tr.eulerAngles.y <= 300)
            {
                sw = 1;//1일때는 오른쪽으로 이동
                UnityEngine.Debug.Log("a");

            }
            else if (tr.eulerAngles.y < 80 && tr.eulerAngles.y >= 60)
            {
                sw = -1;//-1일때는 왼쪽으로 이동
                UnityEngine.Debug.Log("b");

            }

            if (sw == 1)
            {

                yPos = tr.eulerAngles.y + 0.1f + Time.deltaTime;

            }
            else if (sw == -1)
            {

                yPos = tr.eulerAngles.y - (0.1f + Time.deltaTime);

            }

            //transform에 계산한 값 대입
            tr.eulerAngles = new Vector3(0, yPos, 0);

            // float zPos = startPos.z + Random.Range(1, 5);

        }
    }
    public void takeDamge()
    {
        Debug.Log(2);
        animator.SetTrigger(hashdamage);

    }
}
