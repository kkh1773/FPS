using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Device;

public class player : MonoBehaviour
{/*
         W  p  ?  \   l?   ?   ?   ?   ?     3  N  m  ?  ?  ?  ?       @  \  €  ?  ?  ?  com.unity.editor.ui com.unity.editor.headless com.unity.editor.dark-skin com.unity.editor.disable-splash-screen com.unity.editor.legacy.embedded com.unity.editor.legacy.pro com.unity.editor.platforms.android com.unity.editor.platforms.android.pro com.unity.editor.platforms.ios com.unity.editor.platforms.ios.pro com.unity.editor.platforms.nintendo3ds com.unity.editor.platforms.ps4 com.unity.editor.platforms.uwp com.unity.editor.platforms.uwp.pro com.unity.editor.platforms.vita com.unity.editor.platforms.xboxone com.unity.editor.watermarks.disable com.unity.editor.watermarks.edu com.unity.editor.watermarks.prototyping com.unity.editor.watermarks.trial com.unity.editor.time-limited-license com.unity.editor.internal-developer com.unity.tiny d5GUNIyY0X1+wF4iVBW+nd5CgZ8= e087ce29e18279a5a95d0650d6e33f96fcf0ec9f e087ce29e18279a5a95d0650d6e33f96fcf0ec9ff52d1b174faffe8bedc3783363ada59e7cd6ad2f rializeField] private Camera _camera;

    //?íƒœ ?•ì¸
    public bool isIdle = true; //ê°€ë§Œíˆ ?ˆê¸° ?•ì¸
    public bool startJump = false;//?í”„ ?•ì¸
    public bool downToFloar = true;//???¿ìŒ ?•ì¸ 


    //?ë‹ˆë©”ì´???€?¥ë??? ?Œë¼ë¯¸í„°
    public Animator animator;
    private readonly int hashChWalkAndRun = Animator.StringToHash("chWalkAndRun");//ê±·ê¸° ?¬ë¦¬ê¸?ê°?ì¡°ì •
    private readonly int hashFmove = Animator.StringToHash("Fmove");//?ìœ¼ë¡?
    private readonly int hashBmove = Animator.StringToHash("Bmove");//?¤ë¡œ
    private readonly int hashRmove = Animator.StringToHash("Rmove");//?¤ë¥¸ìª½ìœ¼ë¡?
    private readonly int hashLmove = Animator.StringToHash("Lmove");//?¼ìª½?¼ë¡œ
    private readonly int hashChJump = Animator.StringToHash("chJump");//?í”„ê°?ì¡°ì •
    private readonly int hashJump = Animator.StringToHash("Jump");//?í”„


    //?¤ì •ê°?
    [Header("Player Setting")]
    public float moveSpeed;//?ë„
    public float maxSpeed = 5.0f;//ìµœë??ë„
    public float runSpeed = 4.0f;//?¬ë¦¬ê¸°ì†??
    public float defaultSpeed = 3.0f;//ê¸°ë³¸?ë„
    public float sitSpeed = 2.0f;//ê¸°ë³¸?ë„
    public float minSpeed = 1.0f;//ìµœì†Œ?ë„
    public float jumpHeight = 5.0f;//?í”„ ?’ì´
    public float hp = 100.0f;//ì²´ë ¥
    public float sp = 50.0f;//ë°©ì–´??
    public float stemina = 100.0f;//?¤í…Œë¯¸ë‚˜e

    //animator blend?˜ì¹˜ ì¡°ì ˆ??
    [Header("Animator Blend Count")]
    public float walkAndRunCount = 0;
    public float aimCount = 0;
    public float jumpCount = 0;
    public float turnCount = 0;
    public float rurnChangeCount = 0;

    //ê¸°í? ?¤í—˜??
    public float y = 0;
    public float rt = 0.0f;

    //?´ê±°??ë³€??? ì–¸
    public State _state;


    // Start is called before the first frame update
    void Start()
    {
        //ê°ê° ì»´í¬?ŒíŠ¸ ?€??
        tr = GetComponent<Transform>();//transform ?€??
        rb = GetComponent<Rigidbody>();//rigidbody ?€??

        _camera = GetComponentInChildren<Camera>();//?˜ì´?¬í‚¤ ?˜ìœ„??ª©??Camera ?€??

        mouseRotate.Init(tr, _camera.transform);//playerMouseRotate?´ë˜?¤ì˜ ë©”ì†Œ??lnit???Œë ˆ?´ì–´ ?„ì¹˜?€ ì¹´ë©”???•ë³´ê°??„ë‹¬(ì´ˆê¸°ê°??¤ì •)

        moveSpeed = defaultSpeed;//?‰ì‹œ ?ë„ ì§€??

        _state = State.IDLE;//ê¸°ë³¸ ?´ê±°?¤ì •

    }

    // ?„ë ˆ?„ë§ˆ???¸ì¶œ(ë¶ˆê·œì¹? ë¬¼ë¦¬?¨ê³¼ ?†ê±°???€?´ë¨¸?ì„œ ?¬ìš©??
    void Update()
    {
        rt = tr.transform.rotation.y;
        playerRotate();//?Œì „ ?¨ìˆ˜ ?¸ì¶œ
        playerState();//?Œë ˆ?´ì–´ ?íƒœì²´í¬
    }

    //?¼ì • ë°˜ë³µ??ê·œì¹™??ë¬¼ë¦¬?¨ê³¼?ˆëŠ” ?¤ë¸Œ?íŠ¸???¬ìš©)
    private void FixedUpdate()
    {
        mouseRotate.UpdateCursorLock();//mouseRotate??UpdateCursorLock()?¤í–‰
        playerAim();//?¼ìª½?¤ë? ?„ë¥´ê³??ˆì„??
        playerIdle();//ë©ˆì¶°?ˆì„??
        playerMove();//?€ì§ì„
        playerWalk();//ê±·ê¸°
        playerRun();//?¬ë¦¬ê¸?
        playerJump();//?í”„ ?¸ì¶œ
                     //playerSit();//?‰ê¸°

    }

    void OnEnable()
    {
        //StartCoroutine(playerState());


    }

    //?íƒœ?•ì¸ ë©”ì†Œ??
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


    //?´ë™ë©”ì†Œ??
    private void playerMove()
    {

        //bool isMove=moveDir.magnitude>0;
        //?´ê±°???„ì¹˜ê°’ì„ ê³„ì‚°?´ì„œ 0ë³´ë‹¤ ?¬ë©´ true, ?„ë‹ˆë©?false???£ëŠ” ê¸°ëŠ¥ ?˜ì?ë§????¤í¬ë¦½íŠ¸???¬ëŸ¬ê°€ì§€ ?€ì§ì„?´ìˆê¸°ì— ?í•©?˜ì? ?ŠìŒ ì°¸ê³ !
        h = Input.GetAxisRaw("Horizontal");//? ë‹ˆ?°ì† input manager??Horizontal??ì§€?•ëœ ??a,dë¥??„ë???ê·¸ê°’??h???€??ê°€ë¡?
        v = Input.GetAxisRaw("Vertical");//? ë‹ˆ?°ì† input manager??Vertical??ì§€?•ëœ ??w,së¥??„ë???ê·¸ê°’??v???€???¸ë¡œ)

        // ê°€ë¡? ?¸ë¡œ???€ì§ì„??ê³„ì‚° ??ê°’ì„ ?€??
        //Vector3.forward-zì¶•ì„ ê°€ë¦¬í‚´==(0,0,1) / Vector3.right-xì¶•ì„ ê°€ë¦¬í‚´==(1,0,0)
        Vector3 MoveDir = (Vector3.forward * v) + (Vector3.right * h);//(0,0,v)+(h,0,0)=(h,0,v)

        tr.Translate(MoveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
        //?Œë˜?´ì–´ë¥?ê³„ì‚°???„ì¹˜ë¡??´ë™
        //normalized???•ê·œ?”ë¡œ ?Œë˜?´ì–´??ë¡œì»¬ì¢Œí‘œë¥?(1,1,1)ë¡?ì´ˆê¸°?”ì‹œ?????œí‚¬???€ì§ì„??ë¬¸ì œë°œìƒ)(?¬ê¸°?œëŠ” ê°’ì´ ?ˆìœ¼ë©?1.0.1)ë¡?ì´ˆê¸°??
        // ?„ë ˆ?„ìˆ˜ ?µì¼???„í•´ deltaTime???¬ìš©??
        //Space.Self??ë¡œì»¬ì¢Œí‘œë¥??»í•œ?? ë°˜ë?ë¡?Space.world???”ë“œì¢Œí‘œë¥??»í•œ??(?”ë“œë¡?? ì‹œ ?¬ìë¦?ê±¸ìŒ??? ê²ƒ??

    }

    //?ì„ ?¤ì •ë©”ì†Œ??
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

    //ê±·ê¸° ?¤ì •ë©”ì†Œ??
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

    //?¬ë¦¬ê¸??¤ì •ë©”ì†Œ??
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



    //?Œì „ ?¤ì • ë©”ì†Œ??
    private void playerRotate()
    {
        mouseRotate.LookRotation(tr, _camera.transform);
        //mouseRotated??LookRotation?¨ìˆ˜???Œë ˆ?´ì–´??tr(transform)?•ë³´?€ ì¹´ë©”?¼ì˜ transform???•ë³´ë¥?ì¤€??

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
        if (_state == State.JUMP && startJump == true)//? ë‹ˆ?°ì— input manager???€?¥ëœ jump??spaceë°”ì´??)ë¥??„ë¥´ê³?isJumping??false?´ë©´ 
        {

            animator.SetTrigger(hashJump);
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            //AddForce()???¤ë¸Œ?íŠ¸???˜ì„ ê°€? ë•Œ ?°ëŠ” ?¨ìˆ˜
            //?„ìª½ yì¶•ìœ¼ë¡?jumpHightë§Œí¼ ?˜ì„ ì£¼ê¸°+ ForceMode.Impulse??ì§§ì? ?œê°„???˜ì„ì£¼ëŠ” ëª¨ë“œ?¼ê³  ??
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


    //?‰ê¸° ?¤ì •ë©”ì†Œ??
    private void playerSit()
    {


    }
    //

    //ì¶©ëŒê°ì?
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

