using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemySoldierAi : MonoBehaviour
{
    //�� ���¿� ���� ������ ����
    public enum State
    {
        DIE,
        SIT,
        RERODING,
        ATTACK,
        TRACE,
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
    Vector3 startPos;   //���������� ��ġ

    [Header("Player Setting")]
    [SerializeField] float defaultSpeed = 2.0f;//�⺻ �ӵ�
    [SerializeField] float maxSpeed = 5.0f;//�ִ� �ӵ�
    [SerializeField] float hp = 100.0f;//ä��

    [Header("check Setting")]
    public bool isDie = false;// �׾���?
    public bool isLook = false;// �߰��ߴ°�?

    [Header("Animation")]
    public Animator animator;//���ϸ��̼� ���� ����
    public float walkAndRunCount = 0; //move blend tree���� �� ����
    private readonly int hashwalk = Animator.StringToHash("walk");//walk hash�� ���� ���� ����
    private readonly int hashChWalkAndRun = Animator.StringToHash("chWalkAndRun");//chWalkAndRun hash�� ���� ���� ����

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
        playerTr = GameObject.FindGameObjectWithTag("Player").transform.position;//player �±׸� ���� ������Ʈ ������ �Ѱ���

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

            //���� ������ ���Դٸ�
            if (isLook == true)
            {
                state = State.ATTACK;//���ݻ��·� ��Ⱥ
            }

            //���� ������ ���� ���� �ʴٸ�
            else if (isLook == false)
            {
                state = State.TRACE;//�������·� ��ȯ
            }

            else
            {
                state = State.STOP;//����!
            }

            yield return new WaitForSeconds(0.3f);

        }
    }
    public IEnumerator Go()
    {

        if (isDie)
        {
            yield break;//�ڷ�ƾ ����
        }

        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

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

                //���ݻ���
                case State.ATTACK:
                    playerTr = enemyView.TelePos;//�ĺ��� Ÿ���� ��ġ�� ����
                    Pos = playerTr;//���� ��ġ�� ����
                    Stop();
                    //���� ����(�߰� ����)
                    break;

                //��������
                case State.TRACE:
                    nav.isStopped = false;//nav �������� ����
                    nav.speed = maxSpeed;//�ִ�ӵ��� ����
                    playerTr = GameObject.FindGameObjectWithTag("Player").transform.position;//player �±׸� ���� ������Ʈ ������ �Ѱ���
                    Pos = playerTr;//���� ��ġ�� ����
                    move(Pos);//�̵��Լ��� ����������
                    WalkAnimation();//�̵��ִϸ��̼� �Լ��� ����
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
            animator.SetBool(hashwalk, false);//������ Ȯ�� �ؽ� false�� ��ȯ

            // nav.velocity = Vector3.zero;
        }

    void WalkAnimation()
    {
        animator.SetBool(hashwalk, true);//������ Ȯ�� �ؽ� ture�� ��ȯ

        if (state == State.TRACE && nav.speed == maxSpeed)//���������̰� �ӵ��� �ִ�ӵ����
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
