using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyguidAi : EnemyAi
{
    //EnemyAl�� �� �������� ���� �ְ� ��ӽ�Ŵ

    private readonly int hashatt = Animator.StringToHash("att");//att hash�� ���� ���� ����
    private readonly int hashreload = Animator.StringToHash("reload");//reload hash�� ���� ���� ����

    [Header("another C#script")]
    public Fire f;

    public int attackCountMax;  //����Ƚ�� ���ֺ��� ����
    int attackCount = 0;

    public override void Start()
    {
        
        f = GetComponent<Fire>();
        if (!nav.pathPending)//����������� ���� �غ� ���� �ʴ� ���(path)�� ��Ÿ���ϴٶ�� ��(�б�����) false�� �Ի��� �Ϸ� �Ǿ��ٴ� ���̴�.
        {
            StartCoroutine("CheckState");//����Ȯ�� �ڷ�ƾ ����
            StartCoroutine("Go");//�ൿ �ڷ�ƾ ����
        }

    }

    

    //���� üũ �ڷ�ƾ �Լ�
    public override IEnumerator CheckState()
    {
        Debug.Log(1);
        //���� ���¶��
        if (isDie)
        {
            yield break;//�ڷ�ƾ ����
        }

        //���� �ʾҴٸ� 
        while (!isDie)
        {
            //hp�� ���ٸ� (���� �ϼ� �ȵ�)
            if (hp<=0) {

                state = State.DIE;//�ֱ�
             
            }
            //������
            else if (state == State.SIT)
            {

            }

            //float dist = Vector3.Distance(tr.position, startPos);//�ʿ����(Ȥ�ó� �ؼ� ����)
            
            //�ĺ��� ���¶��
            if (isLook == true)
            {
                Pdist = Vector3.Distance(tr.position, playerTr);//�� ĳ���Ϳ� �÷��̾� ���� �Ÿ��� ���
                playerTr = enemyView.TelePos;//�ĺ��� Ÿ���� ��ġ�� ����

                //���� �����Ÿ� �̳��� ���
                if (Pdist <= attackDist)
                {
                    state = State.ATTACK;//���ݻ��·� ��Ⱥ
                }
                //���� �����Ÿ� �̳��� ���
                else if (Pdist <= traceDist)
                {
                    state = State.TRACE;//�������·� ��ȯ
                }

            }
            //�ĺ��� �ȵ� ���¶��
            else if (isLook == false)
            {
                //���� �������� �������� ������ �ȉ�ٸ� �׸��� �� ���������� �߰��� ������ ���� �����Ÿ� ���̶��
                if (isFind ==  true && Pdist <= traceDist)
                {
                    state = State.FIND;//���� ���·� ��ȯ

                }
                //���� ���°� �ƴϰ� �ٽ� ���� �����ġ�� ���ߵ� ���¶��
                else if (isFind==false&&isReturn==true)
                {
                    state = State.RETURN;//���� ���·� ��ȯ
                   
                }
                //���͵� �Ϸ��߰�, �������µ� �ƴ϶�� 
                else if (isReturn ==false&&isFind==false) 
                {
                    state = State.PATROL;//�������·� ��ȯ
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
    public override IEnumerator Go()
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

               //���ݻ���
                case State.ATTACK:
                    playerTr = enemyView.TelePos;//�ĺ��� Ÿ���� ��ġ�� ����
                    Pos = playerTr;//���� ��ġ�� ����

                    Stop();
                    //���� ����(�߰� ����)
                    if (attackCount <= attackCountMax)
                    {
                        animator.SetBool(hashatt, true);
                        f.fire();
                        attackCount++;
                    }
                    else
                    {
                        animator.SetBool(hashatt, false);
                        animator.SetBool(hashreload, true);
                        
                        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            attackCount = 0;
                            animator.SetBool(hashreload, false);
                        }
                        
                    }
                    break;

                //��������
                case State.TRACE:
                    animator.SetBool(hashatt, false);
                    nav.isStopped = false;//nav �������� ����
                    nav.speed = maxSpeed;//�ִ�ӵ��� ����
                    isFind = true;//�������µ� ���� tre�� ��ȯ
                    Pos = playerTr;//���� ��ġ�� ����
                    move(Pos);//�̵��Լ��� ����������
                    WalkAnimation();//�̵��ִϸ��̼� �Լ��� ����
                    break;

                //��������
                case State.FIND:
                    nav.isStopped = false;//nav �������� ����
                    nav.speed = maxSpeed;//�ִ�ӵ��� ����
                    Pos = playerTr;//���ΰ��� ��ġ�� �Ѱ� �������� ����
                    move(Pos);//�̵��Լ��� ����������
                    WalkAnimation();//�̵��ִϸ��̼� �Լ��� ����
                    if (nav.pathPending) break;
                        if (nav.remainingDistance <= 0.5f)//���������� ���� �Ÿ�<=0.5f
                    {
                        isFind = false;//��������
                        isReturn = true;//���ͽ���
                    }
                        break;

                //���� ����
                case State.RETURN:
                    nav.isStopped = false;//nav �������� ����
                    nav.speed = defaultSpeed;//�⺻�ӵ��� ����
                    Pos = startPos;//Ÿ���� ��ġ�� ������������
                    move(Pos);//�̵��Լ��� ����������
                    WalkAnimation();//�̵��ִϸ��̼� �Լ��� ����
                    if (nav.pathPending) break;
                    if (nav.remainingDistance <= 0.5f)//���������� ���� �Ÿ�<=0.5f
                    {
                        isReturn = false;//���� ��
                    }
                    break;

                //������
                case State.PATROL:
                    nav.isStopped = false;//nav �������� ����
                    nav.speed = defaultSpeed;//�⺻�ӵ��� ����
                    Stop();//���� ����
                    //patrolling();

                    break;

                //�������
                case State.STOP:
                    Stop();//���� ����
                    break;

            }
        }

    }

    


}
