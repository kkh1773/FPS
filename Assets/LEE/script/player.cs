using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Device;

public class player : MonoBehaviour
{/*
         W  p  ?  \   l?   ?   ?   ?   ?     3  N  m  ?  ?  ?  ?       @  \  �  ?  ?  ?  com.unity.editor.ui com.unity.editor.headless com.unity.editor.dark-skin com.unity.editor.disable-splash-screen com.unity.editor.legacy.embedded com.unity.editor.legacy.pro com.unity.editor.platforms.android com.unity.editor.platforms.android.pro com.unity.editor.platforms.ios com.unity.editor.platforms.ios.pro com.unity.editor.platforms.nintendo3ds com.unity.editor.platforms.ps4 com.unity.editor.platforms.uwp com.unity.editor.platforms.uwp.pro com.unity.editor.platforms.vita com.unity.editor.platforms.xboxone com.unity.editor.watermarks.disable com.unity.editor.watermarks.edu com.unity.editor.watermarks.prototyping com.unity.editor.watermarks.trial com.unity.editor.time-limited-license com.unity.editor.internal-developer com.unity.tiny d5GUNIyY0X1+wF4iVBW+nd5CgZ8= e087ce29e18279a5a95d0650d6e33f96fcf0ec9f e087ce29e18279a5a95d0650d6e33f96fcf0ec9ff52d1b174faffe8bedc3783363ada59e7cd6ad2f rializeField] private Camera _camera;

    //?�태 ?�인
    public bool isIdle = true; //가만히 ?�기 ?�인
    public bool startJump = false;//?�프 ?�인
    public bool downToFloar = true;//???�음 ?�인 


    //?�니메이???�?��??? ?�라미터
    public Animator animator;
    private readonly int hashChWalkAndRun = Animator.StringToHash("chWalkAndRun");//걷기 ?�리�?�?조정
    private readonly int hashFmove = Animator.StringToHash("Fmove");//?�으�?
    private readonly int hashBmove = Animator.StringToHash("Bmove");//?�로
    private readonly int hashRmove = Animator.StringToHash("Rmove");//?�른쪽으�?
    private readonly int hashLmove = Animator.StringToHash("Lmove");//?�쪽?�로
    private readonly int hashChJump = Animator.StringToHash("chJump");//?�프�?조정
    private readonly int hashJump = Animator.StringToHash("Jump");//?�프


    //?�정�?
    [Header("Player Setting")]
    public float moveSpeed;//?�도
    public float maxSpeed = 5.0f;//최�??�도
    public float runSpeed = 4.0f;//?�리기속??
    public float defaultSpeed = 3.0f;//기본?�도
    public float sitSpeed = 2.0f;//기본?�도
    public float minSpeed = 1.0f;//최소?�도
    public float jumpHeight = 5.0f;//?�프 ?�이
    public float hp = 100.0f;//체력
    public float sp = 50.0f;//방어??
    public float stemina = 100.0f;//?�테미나e

    //animator blend?�치 조절??
    [Header("Animator Blend Count")]
    public float walkAndRunCount = 0;
    public float aimCount = 0;
    public float jumpCount = 0;
    public float turnCount = 0;
    public float rurnChangeCount = 0;

    //기�? ?�험??
    public float y = 0;
    public float rt = 0.0f;

    //?�거??변???�언
    public State _state;


    // Start is called before the first frame update
    void Start()
    {
        //각각 컴포?�트 ?�??
        tr = GetComponent<Transform>();//transform ?�??
        rb = GetComponent<Rigidbody>();//rigidbody ?�??

        _camera = GetComponentInChildren<Camera>();//?�이?�키 ?�위??��??Camera ?�??

        mouseRotate.Init(tr, _camera.transform);//playerMouseRotate?�래?�의 메소??lnit???�레?�어 ?�치?� 카메???�보�??�달(초기�??�정)

        moveSpeed = defaultSpeed;//?�시 ?�도 지??

        _state = State.IDLE;//기본 ?�거?�정

    }

    // ?�레?�마???�출(불규�? 물리?�과 ?�거???�?�머?�서 ?�용??
    void Update()
    {
        rt = tr.transform.rotation.y;
        playerRotate();//?�전 ?�수 ?�출
        playerState();//?�레?�어 ?�태체크
    }

    //?�정 반복??규칙??물리?�과?�는 ?�브?�트???�용)
    private void FixedUpdate()
    {
        mouseRotate.UpdateCursorLock();//mouseRotate??UpdateCursorLock()?�행
        playerAim();//?�쪽?��? ?�르�??�을??
        playerIdle();//멈춰?�을??
        playerMove();//?�직임
        playerWalk();//걷기
        playerRun();//?�리�?
        playerJump();//?�프 ?�출
                     //playerSit();//?�기

    }

    void OnEnable()
    {
        //StartCoroutine(playerState());


    }

    //?�태?�인 메소??
    private void playerState()
    {

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) && Input.GetKey(KeyCode.LeftShift) == false && Input.GetKey(KeyCode.Space) == false && _state != State.JUMP && downToFloar == true)
        {

            _state = State.WALK;
            isIdle = false;

            UnityEngine.Debug.Log("walk");
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) && Input.GetKey(KeyCode.LeftShift) == true && Input.GetKey(KeyCode.Space) == false && _state != State.JUMP && downToFloar == true)
        {
            if (stemina > 0)
            {

                _state = State.RUN;
                isIdle = false;

                UnityEngine.Debug.Log("run");
            }
            else
            {
                _state = State.WALK;
            }


        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) && Input.GetKey(KeyCode.Tab) == true)
        {

            _state = State.SLOW;
            isIdle = false;

            UnityEngine.Debug.Log("slow");

        }
        else if ((_state == State.WALK || _state == State.RUN || _state == State.IDLE) && Input.GetKey(KeyCode.Space))
        {
            _state = State.JUMP;
            startJump = true;
            isIdle = false;

            UnityEngine.Debug.Log("jump");

        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _state = State.SIT;
            isIdle = false;

            UnityEngine.Debug.Log("sit");
        }
        else if (isIdle == true)
        {
            _state = State.IDLE;

            UnityEngine.Debug.Log("idle");

        }
    }

    private void playerIdle()
    {
        if (_state == State.IDLE)
        {
            animator.SetBool(hashFmove, false);
            animator.SetBool(hashBmove, false);
            animator.SetBool(hashRmove, false);
            animator.SetBool(hashLmove, false);
        }
    }


    //?�동메소??
    private void playerMove()
    {

        //bool isMove=moveDir.magnitude>0;
        //?�거???�치값을 계산?�서 0보다 ?�면 true, ?�니�?false???�는 기능 ?��?�????�크립트???�러가지 ?�직임?�있기에 ?�합?��? ?�음 참고!
        h = Input.GetAxisRaw("Horizontal");//?�니?�속 input manager??Horizontal??지?�된 ??a,d�??��???그값??h???�??가�?
        v = Input.GetAxisRaw("Vertical");//?�니?�속 input manager??Vertical??지?�된 ??w,s�??��???그값??v???�???�로)

        // 가�? ?�로???�직임??계산 ??값을 ?�??
        //Vector3.forward-z축을 가리킴==(0,0,1) / Vector3.right-x축을 가리킴==(1,0,0)
        Vector3 MoveDir = (Vector3.forward * v) + (Vector3.right * h);//(0,0,v)+(h,0,0)=(h,0,v)

        tr.Translate(MoveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
        //?�래?�어�?계산???�치�??�동
        //normalized???�규?�로 ?�래?�어??로컬좌표�?(1,1,1)�?초기?�시?????�킬???�직임??문제발생)(?�기?�는 값이 ?�으�?1.0.1)�?초기??
        // ?�레?�수 ?�일???�해 deltaTime???�용??
        //Space.Self??로컬좌표�??�한?? 반�?�?Space.world???�드좌표�??�한??(?�드�??�시 ?�자�?걸음???�것??

    }

    //?�임 ?�정메소??
    private void playerAim()
    {
        if (Input.GetMouseButton(1))
        {
            if (aimCount >= 0)
            {
                aimCount -= Time.deltaTime * 2;
            }
            else
            {
                aimCount = 0;
            }

            animator.SetLayerWeight(1, Mathf.Lerp(0, 1, aimCount * 2));
        }
        else
        {
            if (aimCount < 1)
            {
                aimCount += Time.deltaTime * 2;

            }
            animator.SetLayerWeight(1, Mathf.Lerp(0, 1, aimCount * 2));

        }
    }

    //걷기 ?�정메소??
    private void playerWalk()
    {
        if (_state == State.WALK && isIdle == false)
        {
            moveSpeed = defaultSpeed;



            if (walkAndRunCount >= 0)
            {
                walkAndRunCount -= Time.deltaTime;
            }


            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool(hashFmove, true);
                animator.SetBool(hashBmove, false);
                animator.SetBool(hashRmove, false);
                animator.SetBool(hashLmove, false);
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 2));
                UnityEngine.Debug.Log("fwalk");
            }
            else if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool(hashBmove, true);
                animator.SetBool(hashFmove, false);
                animator.SetBool(hashRmove, false);
                animator.SetBool(hashLmove, false);
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 2));

            }
            else if (Input.GetKey(KeyCode.D))
            {
                animator.SetBool(hashBmove, false);
                animator.SetBool(hashFmove, false);
                animator.SetBool(hashRmove, true);
                animator.SetBool(hashLmove, false);
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 2));
            }
            else if (Input.GetKey(KeyCode.A))
            {
                animator.SetBool(hashBmove, false);
                animator.SetBool(hashFmove, false);
                animator.SetBool(hashRmove, false);
                animator.SetBool(hashLmove, true);
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 2));
            }
            else
            {
                animator.SetBool(hashFmove, false);
                animator.SetBool(hashBmove, false);
                animator.SetBool(hashRmove, false);
                animator.SetBool(hashLmove, false);
                isIdle = true;
            }

            if (stemina < 100)
            {
                stemina += (Time.deltaTime * 2);
            }

        }
    }

    //?�리�??�정메소??
    private void playerRun()
    {
        if (_state == State.RUN && isIdle == false)
        {
            moveSpeed = maxSpeed;

            if (walkAndRunCount < 1)
            {
                walkAndRunCount += Time.deltaTime;

            }

            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool(hashFmove, true);
                animator.SetBool(hashBmove, false);
                animator.SetBool(hashRmove, false);
                animator.SetBool(hashLmove, false);
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 2));
            }
            else if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool(hashBmove, true);
                animator.SetBool(hashFmove, false);
                animator.SetBool(hashRmove, false);
                animator.SetBool(hashLmove, false);
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 2));
            }
            else if (Input.GetKey(KeyCode.D))
            {
                animator.SetBool(hashBmove, false);
                animator.SetBool(hashFmove, false);
                animator.SetBool(hashRmove, true);
                animator.SetBool(hashLmove, false);
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 2));
            }
            else if (Input.GetKey(KeyCode.A))
            {
                animator.SetBool(hashBmove, false);
                animator.SetBool(hashFmove, false);
                animator.SetBool(hashRmove, false);
                animator.SetBool(hashLmove, true);
                animator.SetFloat(hashChWalkAndRun, Mathf.Lerp(0, 1, walkAndRunCount * 2));
            }
            else
            {
                animator.SetBool(hashFmove, false);
                animator.SetBool(hashBmove, false);
                animator.SetBool(hashRmove, false);
                animator.SetBool(hashLmove, false);
                isIdle = true;
            }

            stemina -= (Time.deltaTime * 10);

        }
    }



    //?�전 ?�정 메소??
    private void playerRotate()
    {
        mouseRotate.LookRotation(tr, _camera.transform);
        //mouseRotated??LookRotation?�수???�레?�어??tr(transform)?�보?� 카메?�의 transform???�보�?준??

        // if ((tr.transform.rotation.y>rt) && _state == State.IDLE)
        // {
        /*if (turnCount >= 0)
        {
            aimCount -= Time.deltaTime * 2;
        }
        else
        {
            turnCount = 0;
        }*/



        // animator.SetLayerWeight(3, 1);
        //  }
        // else if ((tr.transform.rotation.y < rt)&&_state==State.IDLE)
        // {
        /*(if (turnCount < 1)
        {
            turnCount += Time.deltaTime * 2;

        }*/
        //  animator.SetLayerWeight(3, 1);

        // }
        // else
        // {
        //     animator.SetLayerWeight(3, 0);
        // }

    /*
    private void playerJump()
    {
        if (_state == State.JUMP && startJump == true)//?�니?�에 input manager???�?�된 jump??space바이??)�??�르�?isJumping??false?�면 
        {

            animator.SetTrigger(hashJump);
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            //AddForce()???�브?�트???�을 가?�때 ?�는 ?�수
            //?�쪽 y축으�?jumpHight만큼 ?�을 주기+ ForceMode.Impulse??짧�? ?�간???�을주는 모드?�고 ??
            jumpCount = 0;
            downToFloar = false;
            startJump = false;
            animator.SetFloat(hashChJump, 0);

        }
        else if (_state == State.JUMP && startJump == false && downToFloar == false)
        {
            if (jumpCount <= 1)
            {
                jumpCount += Time.deltaTime;
            }
            animator.SetFloat(hashChJump, Mathf.Lerp(0, 1, jumpCount * 2));
        }
        else if (downToFloar == true)
        {
            animator.SetFloat(hashChJump, 2);

            isIdle = true;
        }
        else
        {
            jumpCount = 0;

            isIdle = true;
        }
    }


    //?�기 ?�정메소??
    private void playerSit()
    {


    }
    //

    //충돌감�?
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            downToFloar = true;
        }
    }

}
    */
    }

