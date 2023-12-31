using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Device;

public class player : MonoBehaviour
{/*
         W  p  ?  \   l?   ?   ?   ?   ?     3  N  m  ?  ?  ?  ?       @  \    ?  ?  ?  com.unity.editor.ui com.unity.editor.headless com.unity.editor.dark-skin com.unity.editor.disable-splash-screen com.unity.editor.legacy.embedded com.unity.editor.legacy.pro com.unity.editor.platforms.android com.unity.editor.platforms.android.pro com.unity.editor.platforms.ios com.unity.editor.platforms.ios.pro com.unity.editor.platforms.nintendo3ds com.unity.editor.platforms.ps4 com.unity.editor.platforms.uwp com.unity.editor.platforms.uwp.pro com.unity.editor.platforms.vita com.unity.editor.platforms.xboxone com.unity.editor.watermarks.disable com.unity.editor.watermarks.edu com.unity.editor.watermarks.prototyping com.unity.editor.watermarks.trial com.unity.editor.time-limited-license com.unity.editor.internal-developer com.unity.tiny d5GUNIyY0X1+wF4iVBW+nd5CgZ8= e087ce29e18279a5a95d0650d6e33f96fcf0ec9f e087ce29e18279a5a95d0650d6e33f96fcf0ec9ff52d1b174faffe8bedc3783363ada59e7cd6ad2f rializeField] private Camera _camera;

    //?ํ ?์ธ
    public bool isIdle = true; //๊ฐ๋งํ ?๊ธฐ ?์ธ
    public bool startJump = false;//?ํ ?์ธ
    public bool downToFloar = true;//???ฟ์ ?์ธ 


    //?๋๋ฉ์ด????ฅ๋??? ?๋ผ๋ฏธํฐ
    public Animator animator;
    private readonly int hashChWalkAndRun = Animator.StringToHash("chWalkAndRun");//๊ฑท๊ธฐ ?ฌ๋ฆฌ๊ธ?๊ฐ?์กฐ์ 
    private readonly int hashFmove = Animator.StringToHash("Fmove");//?์ผ๋ก?
    private readonly int hashBmove = Animator.StringToHash("Bmove");//?ค๋ก
    private readonly int hashRmove = Animator.StringToHash("Rmove");//?ค๋ฅธ์ชฝ์ผ๋ก?
    private readonly int hashLmove = Animator.StringToHash("Lmove");//?ผ์ชฝ?ผ๋ก
    private readonly int hashChJump = Animator.StringToHash("chJump");//?ํ๊ฐ?์กฐ์ 
    private readonly int hashJump = Animator.StringToHash("Jump");//?ํ


    //?ค์ ๊ฐ?
    [Header("Player Setting")]
    public float moveSpeed;//?๋
    public float maxSpeed = 5.0f;//์ต๋??๋
    public float runSpeed = 4.0f;//?ฌ๋ฆฌ๊ธฐ์??
    public float defaultSpeed = 3.0f;//๊ธฐ๋ณธ?๋
    public float sitSpeed = 2.0f;//๊ธฐ๋ณธ?๋
    public float minSpeed = 1.0f;//์ต์?๋
    public float jumpHeight = 5.0f;//?ํ ?์ด
    public float hp = 100.0f;//์ฒด๋ ฅ
    public float sp = 50.0f;//๋ฐฉ์ด??
    public float stemina = 100.0f;//?คํ๋ฏธ๋e

    //animator blend?์น ์กฐ์ ??
    [Header("Animator Blend Count")]
    public float walkAndRunCount = 0;
    public float aimCount = 0;
    public float jumpCount = 0;
    public float turnCount = 0;
    public float rurnChangeCount = 0;

    //๊ธฐํ? ?คํ??
    public float y = 0;
    public float rt = 0.0f;

    //?ด๊ฑฐ??๋ณ??? ์ธ
    public State _state;


    // Start is called before the first frame update
    void Start()
    {
        //๊ฐ๊ฐ ์ปดํฌ?ํธ ???
        tr = GetComponent<Transform>();//transform ???
        rb = GetComponent<Rigidbody>();//rigidbody ???

        _camera = GetComponentInChildren<Camera>();//?์ด?ฌํค ?์??ชฉ??Camera ???

        mouseRotate.Init(tr, _camera.transform);//playerMouseRotate?ด๋?ค์ ๋ฉ์??lnit???๋ ?ด์ด ?์น? ์นด๋ฉ???๋ณด๊ฐ??๋ฌ(์ด๊ธฐ๊ฐ??ค์ )

        moveSpeed = defaultSpeed;//?์ ?๋ ์ง??

        _state = State.IDLE;//๊ธฐ๋ณธ ?ด๊ฑฐ?ค์ 

    }

    // ?๋ ?๋ง???ธ์ถ(๋ถ๊ท์น? ๋ฌผ๋ฆฌ?จ๊ณผ ?๊ฑฐ????ด๋จธ?์ ?ฌ์ฉ??
    void Update()
    {
        rt = tr.transform.rotation.y;
        playerRotate();//?์  ?จ์ ?ธ์ถ
        playerState();//?๋ ?ด์ด ?ํ์ฒดํฌ
    }

    //?ผ์  ๋ฐ๋ณต??๊ท์น??๋ฌผ๋ฆฌ?จ๊ณผ?๋ ?ค๋ธ?ํธ???ฌ์ฉ)
    private void FixedUpdate()
    {
        mouseRotate.UpdateCursorLock();//mouseRotate??UpdateCursorLock()?คํ
        playerAim();//?ผ์ชฝ?ค๋? ?๋ฅด๊ณ??์??
        playerIdle();//๋ฉ์ถฐ?์??
        playerMove();//?์ง์
        playerWalk();//๊ฑท๊ธฐ
        playerRun();//?ฌ๋ฆฌ๊ธ?
        playerJump();//?ํ ?ธ์ถ
                     //playerSit();//?๊ธฐ

    }

    void OnEnable()
    {
        //StartCoroutine(playerState());


    }

    //?ํ?์ธ ๋ฉ์??
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


    //?ด๋๋ฉ์??
    private void playerMove()
    {

        //bool isMove=moveDir.magnitude>0;
        //?ด๊ฑฐ???์น๊ฐ์ ๊ณ์ฐ?ด์ 0๋ณด๋ค ?ฌ๋ฉด true, ?๋๋ฉ?false???ฃ๋ ๊ธฐ๋ฅ ?์?๋ง????คํฌ๋ฆฝํธ???ฌ๋ฌ๊ฐ์ง ?์ง์?ด์๊ธฐ์ ?ํฉ?์? ?์ ์ฐธ๊ณ !
        h = Input.GetAxisRaw("Horizontal");//? ๋?ฐ์ input manager??Horizontal??์ง?๋ ??a,d๋ฅ??๋???๊ทธ๊ฐ??h?????๊ฐ๋ก?
        v = Input.GetAxisRaw("Vertical");//? ๋?ฐ์ input manager??Vertical??์ง?๋ ??w,s๋ฅ??๋???๊ทธ๊ฐ??v??????ธ๋ก)

        // ๊ฐ๋ก? ?ธ๋ก???์ง์??๊ณ์ฐ ??๊ฐ์ ???
        //Vector3.forward-z์ถ์ ๊ฐ๋ฆฌํด==(0,0,1) / Vector3.right-x์ถ์ ๊ฐ๋ฆฌํด==(1,0,0)
        Vector3 MoveDir = (Vector3.forward * v) + (Vector3.right * h);//(0,0,v)+(h,0,0)=(h,0,v)

        tr.Translate(MoveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
        //?๋?ด์ด๋ฅ?๊ณ์ฐ???์น๋ก??ด๋
        //normalized???๊ท?๋ก ?๋?ด์ด??๋ก์ปฌ์ขํ๋ฅ?(1,1,1)๋ก?์ด๊ธฐ?์?????ํฌ???์ง์??๋ฌธ์ ๋ฐ์)(?ฌ๊ธฐ?๋ ๊ฐ์ด ?์ผ๋ฉ?1.0.1)๋ก?์ด๊ธฐ??
        // ?๋ ?์ ?ต์ผ???ํด deltaTime???ฌ์ฉ??
        //Space.Self??๋ก์ปฌ์ขํ๋ฅ??ปํ?? ๋ฐ๋?๋ก?Space.world???๋์ขํ๋ฅ??ปํ??(?๋๋ก?? ์ ?ฌ์๋ฆ?๊ฑธ์??? ๊ฒ??

    }

    //?์ ?ค์ ๋ฉ์??
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

    //๊ฑท๊ธฐ ?ค์ ๋ฉ์??
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

    //?ฌ๋ฆฌ๊ธ??ค์ ๋ฉ์??
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



    //?์  ?ค์  ๋ฉ์??
    private void playerRotate()
    {
        mouseRotate.LookRotation(tr, _camera.transform);
        //mouseRotated??LookRotation?จ์???๋ ?ด์ด??tr(transform)?๋ณด? ์นด๋ฉ?ผ์ transform???๋ณด๋ฅ?์ค??

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
        if (_state == State.JUMP && startJump == true)//? ๋?ฐ์ input manager????ฅ๋ jump??space๋ฐ์ด??)๋ฅ??๋ฅด๊ณ?isJumping??false?ด๋ฉด 
        {

            animator.SetTrigger(hashJump);
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            //AddForce()???ค๋ธ?ํธ???์ ๊ฐ? ๋ ?ฐ๋ ?จ์
            //?์ชฝ y์ถ์ผ๋ก?jumpHight๋งํผ ?์ ์ฃผ๊ธฐ+ ForceMode.Impulse??์งง์? ?๊ฐ???์์ฃผ๋ ๋ชจ๋?ผ๊ณ  ??
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


    //?๊ธฐ ?ค์ ๋ฉ์??
    private void playerSit()
    {


    }
    //

    //์ถฉ๋๊ฐ์?
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

