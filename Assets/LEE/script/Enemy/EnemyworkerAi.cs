using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class EnemyWorkerAi : MonoBehaviour
{
    //�� ���¿� ���� ������ ����
    public enum State
    {
        DIE,
        SIT,
        FIND,
        WORK,
        RUN,
        PANIC,
        STOP
    }

    //�⺻������ �ʿ��� ���� ���� ����
    public State state = State.STOP;// ������ ���� ���� �ʱ�ȭ 
    public NavMeshAgent nav;// �׺���̼� �߰�
    public Transform tr;//�ش� ������Ʈ transform ��������
    public Rigidbody rb;//�ش� ������Ʈ rigidbody ���� ����

    //��ġ, ȸ��, ��Ÿ ���� ����
    Vector3 Pos;        //�̵��� ��ġ
    Vector3 playerTr;   //Ÿ���� ������ ��ġ
    Vector3 hidePos;


    [Header("Player Setting")]
    [SerializeField] float defaultSpeed = 2.0f;//�⺻ �ӵ�
    [SerializeField] float maxSpeed = 5.0f;//�ִ� �ӵ�
    [SerializeField] float hp = 100.0f;//ä��


    [Header("check Setting")]
    public bool isDie = false;// �׾���?
    public bool isFind = false;//ã���ֳ�?
    public bool isLook = false;// �߰��ߴ°�?
    public bool isWork = false;//���ϴ°�?
    public bool isPanic = false;//�д��� �Դ°�?

    [Header("Animation")]
    public Animator animator;//���ϸ��̼� ���� ����
    public float walkAndRunCount = 0; //move blend tree���� �� ����
    private readonly int hashwalk = Animator.StringToHash("walk");//walk hash�� ���� ���� ����
    private readonly int hashChWalkAndRun = Animator.StringToHash("chWalkAndRun");//chWalkAndRun hash�� ���� ���� ����
    private readonly int hashPanic = Animator.StringToHash("panic");//walk hash�� ���� ���� ����
    private readonly int hashWork = Animator.StringToHash("work");//walk hash�� ���� ���� ����

    [Header("another C#script")]
    public EnemyView enemyView;//�þ߰� ���� ��ũ��Ʈ ����
    public enemyRegDoll enemyRegdoll;//���׵� ���� ��ũ��Ʈ ����



    void Start()
    {
        //�ʱⰪ ����
        tr = GetComponent<Transform>();//transform �� ����
        nav = GetComponent<NavMeshAgent>();//nav�� ����
        rb = GetComponent<Rigidbody>();// rigidbode�� ����
        animator = GetComponent<Animator>();//animatior�� ����
        enemyView = GetComponent<EnemyView>();// EnemyView��ũ��Ʈ ����
        enemyRegdoll = GetComponentInParent<enemyRegDoll>();//enemyRegDoll��ũ��Ʈ ����


        //�ʱⰪ ����
        nav.speed = defaultSpeed;//�⺻�ӵ��� ����
        isLook = enemyView.look;//���� Ÿ��ĺ����°�����
       // Pos = startPos;//�� ��ġ�� ����
        hidePos = GameObject.FindGameObjectWithTag("hideSpot").transform.position;

        if (!nav.pathPending)//����������� ���� �غ� ���� �ʴ� ���(path)�� ��Ÿ���ϴٶ�� ��(�б�����) false�� �Ի��� �Ϸ� �Ǿ��ٴ� ���̴�.
        {
            StartCoroutine("CheckState");//����Ȯ�� �ڷ�ƾ ����
            StartCoroutine("Go");//�ൿ �ڷ�ƾ ����
        }

    }

    private void Update()
    {
        //rb.velocity = Vector3.zero; //�浹 �� �̲������°� ����
        isLook = enemyView.look;//���� Ÿ��ĺ����°�����

    }

    //���� üũ �ڷ�ƾ �Լ�
    public IEnumerator CheckState()
    {
        //���� ���¶��
        if (isDie)
        {
            yield break;//�ڷ�ƾ ����
        }

        //���� �ʾҴٸ� 
        while (!isDie)
        {
            //hp�� ���ٸ� (���� �ϼ� �ȵ�)
            if (hp <= 0)
            {

                state = State.DIE;//�ֱ�

            }
            //������
            else if (state == State.SIT)
            {

            }

            //float dist = Vector3.Distance(tr.position, startPos);//�ʿ����(Ȥ�ó� �ؼ� ����)

            //�ĺ��� ���¶��
            if (isLook == true && isPanic == false)
            {
                state = State.FIND;//���� ���·� ��ȯ
            }

            else if (isLook == false || isPanic==true)
            {

                if (isFind == true)
                {
                    state = State.RUN;
                }
                else if (isWork == true)
                {
                    state = State.WORK;
                }
                else if (isPanic == true)
                {
                    state = State.PANIC;
                }

                else
                {
                    state = State.STOP;//����!
                }
            }


            
            yield return new WaitForSeconds(0.3f);//0.3�� ������

        }
    }

    //���¿� ���� �ൿ �ڷ�ƾ �Լ�
    public IEnumerator Go()
    {

        if (isDie)
        {
            yield break;//�ڷ�ƾ ����
        }
        //���׾��ٸ� 
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);//0.3�� ������



            //���¿� ���� �б� ó��
            switch (state)
            {
                //�׾�����
                case State.DIE:
                    isDie = true;//���� Ȱ��ȭ
                    Stop();//�ӽù���
                    GetComponent<CapsuleCollider>().enabled = false;//���� �ݶ��̴� ����
                    enemyRegdoll.chTr = tr;//���׵���ũ��Ʈ�� ���� ��ġ ����
                    enemyRegdoll.changeRegdoll();//���׵��� �ٲٴ� �Լ� ȣ��
                    break;//while�� ������

                //��������
                case State.FIND:
                    isFind = true;
                    isPanic = false;
                    isWork = false;
                    nav.isStopped = false;//nav �������� ����
                    nav.speed = maxSpeed;//�ִ�ӵ��� ����
                    playerTr = enemyView.TelePos;//�ĺ��� Ÿ���� ��ġ�� ����
                    Pos = hidePos;//���ΰ��� ��ġ�� �Ѱ� �������� ����
                    nav.destination = Pos;
                   
                        move(Pos);//�̵��Լ��� ����������
                        WalkAnimation();//�̵��ִϸ��̼� �Լ��� ����
                    
                    if (nav.pathPending) break;

                    if (nav.remainingDistance <= 1.0f)//���������� ���� �Ÿ�<=��ǥ�� �����ϴٰ� ��ǥ ��ġ�� ����������� ������ �����ϴ� ���� �Ÿ�
                      {
                            UnityEngine.Debug.Log("isb");
                            isFind = false;
                            isPanic = true;

                        }
                    break;

                case State.RUN:
                    isFind = true;
                    isPanic = false;
                    isWork = false;
                    nav.isStopped = false;//nav �������� ����
                    nav.speed = maxSpeed;//�ִ�ӵ��� ����
                    playerTr = enemyView.TelePos;//�ĺ��� Ÿ���� ��ġ�� ����
                    Pos = hidePos;
                   
                    move(Pos);//�̵��Լ��� ����������
                    WalkAnimation();//�̵��ִϸ��̼� �Լ��� ����
                    nav.destination = Pos;

                    if (nav.pathPending) break;

                    if (nav.remainingDistance <= 1.0f)//���������� ���� �Ÿ�<=��ǥ�� �����ϴٰ� ��ǥ ��ġ�� ����������� ������ �����ϴ� ���� �Ÿ�
                    {
                        UnityEngine.Debug.Log("isa");
                        isFind = false;
                        isPanic = true;

                    }
                    break;

                case State.WORK:
                    nav.isStopped = true;
                    nav.speed = defaultSpeed;
                    Stop();
                    animator.SetBool(hashWork, true);
                    break;

                case State.PANIC:
                    nav.isStopped = true;
                    nav.speed = defaultSpeed;
                    Stop();
                    animator.SetBool(hashPanic, true);//������ Ȯ�� �ؽ� ture�� ��ȯ
                    break;

                //�������
                case State.STOP:
                    Stop();//���� ����
                    break;

            }
        }

    }

    //���� �Լ�
    void Stop()
    {
        nav.isStopped = true;////nav �������� 
        animator.SetBool(hashWork, false);
        animator.SetBool(hashPanic, false);
        animator.SetBool(hashwalk, false);//������ Ȯ�� �ؽ� false�� ��ȯ

        // nav.velocity = Vector3.zero;
    }

    //�̵� ���ϸ��̼� �Լ�
    void WalkAnimation()
    {
        animator.SetBool(hashWork, false);
        animator.SetBool(hashwalk, true);//������ Ȯ�� �ؽ� ture�� ��ȯ

        if ((state == State.FIND || state == State.RUN) && nav.speed == maxSpeed)//���������̰� �ӵ��� �ִ�ӵ����
        {

            animator.SetFloat(hashChWalkAndRun, 1);
            //������ 1�� �ø���(1�� �޸���ִϸ��̼� ���
            /* if (walkAndRunCount < 1)
             {

                walkAndRunCount += Time.deltaTime;
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 30.0f));
             }*/
        }
        else//�� �ܿ� ��Ȳ
        {

            animator.SetFloat(hashChWalkAndRun, 0);
            //������ 0�� �ø���(0�� �ȴ� �ִϸ��̼� ���
            /*if (walkAndRunCount >= 0)
            {
                walkAndRunCount -= Time.deltaTime;
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 30.0f));
            }*/


        }

    }

    //�̵��Լ�
    void move(Vector3 pos)
    {
        nav.SetDestination(pos);//���� �������� �̵�����

    }

  
}
