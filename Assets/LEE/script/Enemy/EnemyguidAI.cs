using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyguidAi : EnemyAi
{
    //EnemyAl에 적 공통으로 쓸거 넣고 상속시킴

    private readonly int hashatt = Animator.StringToHash("att");//att hash값 저장 변수 선언
    private readonly int hashreload = Animator.StringToHash("reload");//reload hash값 저장 변수 선언

    [Header("another C#script")]
    public Fire f;

    public int attackCountMax;  //공격횟수 유닛별로 지정
    int attackCount = 0;

    public override void Start()
    {
        
        f = GetComponent<Fire>();
        if (!nav.pathPending)//계산중이지만 아직 준비가 되지 않는 경로(path)를 나타냅니다라는 뜻(읽기전용) false면 게산이 완료 되었다는 뜻이다.
        {
            StartCoroutine("CheckState");//상태확인 코루틴 실행
            StartCoroutine("Go");//행동 코루틴 실행
        }

    }

    

    //상태 체크 코루틴 함수
    public override IEnumerator CheckState()
    {
        Debug.Log(1);
        //죽은 상태라면
        if (isDie)
        {
            yield break;//코루틴 종료
        }

        //죽지 않았다면 
        while (!isDie)
        {
            //hp가 없다면 (아직 완성 안됨)
            if (hp<=0) {

                state = State.DIE;//주금
             
            }
            //제작중
            else if (state == State.SIT)
            {

            }

            //float dist = Vector3.Distance(tr.position, startPos);//필요없음(혹시나 해서 남김)
            
            //식별된 상태라면
            if (isLook == true)
            {
                Pdist = Vector3.Distance(tr.position, playerTr);//적 캐릭터와 플레이어 간의 거리를 계산
                playerTr = enemyView.TelePos;//식별된 타겟의 위치값 전달

                //공격 사정거리 이내인 경우
                if (Pdist <= attackDist)
                {
                    state = State.ATTACK;//공격상태로 전횐
                }
                //추적 사정거리 이내인 경우
                else if (Pdist <= traceDist)
                {
                    state = State.TRACE;//추적상태로 전환
                }

            }
            //식별이 안된 상태라면
            else if (isLook == false)
            {
                //아직 마ㅓ지막 지역까지 정찰이 안됬다면 그리고 그 마지막으로 발견한 지역이 추적 사정거리 안이라면
                if (isFind ==  true && Pdist <= traceDist)
                {
                    state = State.FIND;//수색 상태로 전환

                }
                //수색 상태가 아니고 다시 본인 경계위치로 가야될 상태라면
                else if (isFind==false&&isReturn==true)
                {
                    state = State.RETURN;//복귀 상태로 전환
                   
                }
                //복귀도 완료했고, 수색상태도 아니라면 
                else if (isReturn ==false&&isFind==false) 
                {
                    state = State.PATROL;//정찰상태로 전환
                }
                else
                {
                    state = State.STOP;//멈춰!
                }

                
            }
            yield return new WaitForSeconds(0.3f);//0.3초 딜레이

        }
    }

    //상태에 따른 행동 코루틴 함수
    public override IEnumerator Go()
    {

        if (isDie)
        {
            yield break;//코루틴 종료
        }
        //안죽었다면 
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);//0.3초 딜레이



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

                //추적상태
                case State.TRACE:
                    animator.SetBool(hashatt, false);
                    nav.isStopped = false;//nav 정지상태 해제
                    nav.speed = maxSpeed;//최대속도로 변경
                    isFind = true;//수색상태도 같이 tre로 전환
                    Pos = playerTr;//적의 위치값 전달
                    move(Pos);//이동함수로 목적지전달
                    WalkAnimation();//이동애니메이션 함수로 전달
                    break;

                //수색상태
                case State.FIND:
                    nav.isStopped = false;//nav 정지상태 해제
                    nav.speed = maxSpeed;//최대속도로 변경
                    Pos = playerTr;//주인공의 위치를 넘겨 추적모드로 변경
                    move(Pos);//이동함수로 목적지전달
                    WalkAnimation();//이동애니메이션 함수로 전달
                    if (nav.pathPending) break;
                        if (nav.remainingDistance <= 0.5f)//목적지까지 남은 거리<=0.5f
                    {
                        isFind = false;//수색종료
                        isReturn = true;//복귀실행
                    }
                        break;

                //복귀 상태
                case State.RETURN:
                    nav.isStopped = false;//nav 정지상태 해제
                    nav.speed = defaultSpeed;//기본속도로 변경
                    Pos = startPos;//타겟의 위치를 순찰구역으로
                    move(Pos);//이동함수로 목적지전달
                    WalkAnimation();//이동애니메이션 함수로 전달
                    if (nav.pathPending) break;
                    if (nav.remainingDistance <= 0.5f)//목적지까지 남은 거리<=0.5f
                    {
                        isReturn = false;//복귀 끝
                    }
                    break;

                //경계상태
                case State.PATROL:
                    nav.isStopped = false;//nav 정지상태 해제
                    nav.speed = defaultSpeed;//기본속도로 변경
                    Stop();//멈춤 실행
                    //patrolling();

                    break;

                //멈춤상태
                case State.STOP:
                    Stop();//멈춤 실행
                    break;

            }
        }

    }

    


}
