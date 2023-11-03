using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyguidAi : MonoBehaviour
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

    public State state = State.STOP;
    public NavMeshAgent nav;
    public Transform tr;
    public Rigidbody rb;
   

    [Header("Player Setting")]
    [SerializeField]
    float defaultSpeed = 2.0f;//�⺻ �ӵ�
    float maxSpeed = 5.0f;//�ִ� �ӵ�
    float hp = 100.0f;//ä��



    [SerializeField]
    Vector3 startPos;     //���������� ��ġ
    float yPos = 0.0f;

    [SerializeField]
    Vector3 Pos;          //Ÿ���� ��ġ
    Vector3 playerTr; //Ÿ���� ������ ��ġ

    public float area_in = 0;//�����ġ�� �ִ�Ÿ�
    public float attackDist = 10.0f;//���� �����Ÿ�
    public float traceDist = 20.0f;//���� �����Ÿ�
    public float Pdist;  //�÷��̾���� �Ÿ�
    int sw = 1;
    float angle = 0;
    public int attackCountMax;  //공격횟수 유닛별로 지정
    int attackCount=0;

    [Header("check Setting")]
    public bool isDie = false;
    public bool isFind = false;
    public bool isReturn = false;
    public bool isLook = false;


    [SerializeField]
    public EnemyView enemyView;
    public enemyRegDoll enemyRegdoll;

    [Header("Animation")]
    [SerializeField]
    public float walkAndRunCount = 0;//

    public Animator animator;
    private readonly int hashwalk = Animator.StringToHash("walk");
    private readonly int hashChWalkAndRun = Animator.StringToHash("chWalkAndRun");
    Fire f;

    void Start()
    {
        tr = GetComponent<Transform>();
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        enemyView = GetComponent<EnemyView>();
        enemyRegdoll = GetComponentInParent<enemyRegDoll>();
        animator =GetComponent<Animator>();
        nav.speed = defaultSpeed;
        startPos = this.tr.position;  //ó�� ��ġ�� ������ �������� ����
        isLook = enemyView.look;
        Pos = startPos;
        f=GetComponent<Fire>();
        if (!nav.pathPending)
        {
            StartCoroutine("CheckState");
            StartCoroutine("Go");
        }

    }
    private void Update()
    {
        //rb.velocity = Vector3.zero; //�浹 �� �̲������°� ����
        isLook = enemyView.look;
        
        patrolling();
    }

    public IEnumerator CheckState()
    {
        while (!isDie)
        {

            if (state == State.DIE) {

                yield break;
            }
                
            else if (state == State.SIT)
            {

            }

            //�� ĳ���Ϳ� �������� ���� �Ÿ��� ���
            float dist = Vector3.Distance(tr.position, startPos);
            playerTr = enemyView.TelePos;

            if (isLook == true)
            {

                //�� ĳ���Ϳ� �÷��̾� ���� �Ÿ��� ���
                Pdist = Vector3.Distance(tr.position, playerTr);

                //������ �����ϰ� ���� �����Ÿ� �̳��� ���
                if (Pdist <= attackDist)
                {
                    state = State.ATTACK;
                }
                //���� �����Ÿ� �̳��� ���
                else if (Pdist <= traceDist)
                {
                    state = State.TRACE;
                }

            }
            else if (isLook == false)
            {

                if (isFind ==  true && Pdist <= traceDist)
                {
                    state = State.FIND;

                }

                ///���������� ��ġ�� �ڽŰ� ������ ���� ��
                else if (isFind==false&&isReturn==true)
                {
                    state = State.RETURN;
                   
                }

                else if (isReturn ==false&&isFind==false) //������ ���� ���� ���Ͱ� ������ ��
                {
                    state = State.PATROL;
                }

                else
                {
                    state = State.STOP;
                }

                
            }
            yield return new WaitForSeconds(0.3f);

        }
    }
    public IEnumerator Go()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            //���¿� ���� �б� ó��
            switch (state)
            {

                case State.DIE:
                    isDie = true;
                    Stop();
                    GetComponent<CapsuleCollider>().enabled = false;
                    enemyRegdoll.chTr = tr;
                    enemyRegdoll.changeRegdoll();
                    break;

                //��� �ִϸ��̼�(�߰� ����)
                case State.ATTACK:
                    Stop();
                   if(attackCount<=attackCountMax){
                        f.fire();
                        attackCount++;
                    }else{
                        //재장전 모션 (GetCurrentAnimatorStateInfo)?
                        attackCount=0;
                    }
                    break;

                case State.TRACE:
                    nav.isStopped = false;
                    nav.speed = maxSpeed;
                    isFind = true;
                    //���ΰ��� ��ġ�� �Ѱ� �������� ����
                    Pos = playerTr;
                    move(Pos);
                    Walk();
                    break;

                case State.FIND:
                    nav.isStopped = false;
                    nav.speed = maxSpeed;
                    //���ΰ��� ��ġ�� �Ѱ� �������� ����
                    Pos = playerTr;
                    move(Pos);
                    Walk();
                    if (nav.remainingDistance <= nav.stoppingDistance)
                    {
                        isFind = false;
                        isReturn = true;
                    }
                        break;

                case State.RETURN:
                    nav.isStopped = false;
                    nav.speed = defaultSpeed;
                    Pos = startPos;//Ÿ���� ��ġ�� ������������
                    move(Pos);
                    Walk();
                    if (tr.position.x==startPos.x&&tr.position.z==startPos.z)
                    {
                        isReturn = false;
                    }
                    break;

                case State.PATROL:
                    nav.isStopped = false;
                    nav.speed = defaultSpeed;
                    Stop();
                    //patrolling();

                    break;

                case State.STOP:
                    Stop();
                    break;

            }
        }

    }
    void Stop()
    {
        nav.isStopped = true;
        animator.SetBool(hashwalk, false);

        // nav.velocity = Vector3.zero;
    }

    void Walk()
    {
        animator.SetBool(hashwalk, true);

        if (state == State.TRACE&&nav.speed==maxSpeed)
        {
            if (walkAndRunCount < 1)
            {
                walkAndRunCount += Time.deltaTime;
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 2));
            }
        }
        else
        {
            if (walkAndRunCount >= 0)
            {
                walkAndRunCount -= Time.deltaTime;
            }

            animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 2));
        }

    }

    void move(Vector3 pos)
    {
        this.Pos = pos;
        nav.SetDestination(Pos);

    }

    void patrolling()
    {

        if (state == State.PATROL)
        {
            if (tr.eulerAngles.y >= 280 && tr.eulerAngles.y <= 300)
            {
                sw = 1;
                UnityEngine.Debug.Log("a");

            }
            else if (tr.eulerAngles.y < 80 && tr.eulerAngles.y >= 60)
            {
                sw = -1;
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


            tr.eulerAngles = new Vector3(0, yPos, 0);

            // float zPos = startPos.z + Random.Range(1, 5);

        }
    }


}
