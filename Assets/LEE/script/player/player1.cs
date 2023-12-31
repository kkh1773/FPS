using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class player1 : MonoBehaviour
{
    public enum State{
        WALK,
        RUN,
        SLOW,
        JUMP,
        SIT,
        IDLE


    }

    private float h = 0.0f;//가로  x축
    private float v = 0.0f;//세로 y축
    private Rigidbody rb;//rigidbody 저장변수
    private Transform tr;//transform 저장변수

    [SerializeField] private playerMouseRotate mouseRotate;//마우스 회전에 관한것들 저장하는 변수(데이터 타입 playerMouseRotate)
    [SerializeField] private Camera _camera;//카메라 에 관한 것들 저장하는 변수(데이터 타입 Camera)

    [SerializeField]
    public bool isIdle = true; //媛�留뚰엳 ?덇린 ?뺤씤
    public bool startJump = false;//?먰봽 ?뺤씤
    public bool downToFloar = true;//???우쓬 ?뺤씤 


   
    public Animator animator;
    private readonly int hashChWalkAndRun = Animator.StringToHash("chWalkAndRun");//嫄룰린 ?щ━湲?媛?議곗젙
    private readonly int hashFmove = Animator.StringToHash("Fmove");//?욎쑝濡?
    private readonly int hashBmove = Animator.StringToHash("Bmove");//?ㅻ줈
    private readonly int hashRmove = Animator.StringToHash("Rmove");//?ㅻⅨ履쎌쑝濡?
    private readonly int hashLmove = Animator.StringToHash("Lmove");//?쇱そ?쇰줈
    private readonly int hashChJump = Animator.StringToHash("chJump");//?먰봽媛?議곗젙
    private readonly int hashJump = Animator.StringToHash("Jump");//?먰봽


   
    [Header("Player Setting")]
    public float moveSpeed;//?띾룄
    public float maxSpeed = 5.0f;//理쒕??띾룄
    public float runSpeed = 4.0f;//?щ━湲곗냽??
    public float defaultSpeed = 3.0f;//湲곕낯?띾룄
    public float sitSpeed = 2.0f;//湲곕낯?띾룄
    public float minSpeed = 1.0f;//理쒖냼?띾룄
    public float jumpHeight = 5.0f;//?먰봽 ?믪씠
    public float hp = 100.0f;//泥대젰
    public float sp = 50.0f;//諛⑹뼱??
    public float stemina = 100.0f;//?ㅽ뀒誘몃굹e

   
    [Header("Animator Blend Count")]
    public float walkAndRunCount = 0;
    public float aimCount = 0;
    public float jumpCount = 0;
    public float turnCount = 0;
    public float rurnChangeCount = 0;

    //湲고? ?ㅽ뿕??
    public float y = 0;
    public float rt = 0.0f;

    
    public State _state;


    // Start is called before the first frame update
    void Start()
    {
        //媛곴컖 而댄룷?뚰듃 ?�??
        tr = GetComponent<Transform>();//transform ?�??
        rb = GetComponent<Rigidbody>();//rigidbody ?�??

        _camera = GetComponentInChildren<Camera>();//?섏씠?ы궎 ?섏쐞??ぉ??Camera ?�??

        mouseRotate.Init(tr, _camera.transform);//playerMouseRotate?대옒?ㅼ쓽 硫붿냼??lnit???뚮젅?댁뼱 ?꾩튂?� 移대찓???뺣낫媛??꾨떖(珥덇린媛??ㅼ젙)

        moveSpeed = defaultSpeed;//?됱떆 ?띾룄 吏�??

        _state = State.IDLE;//湲곕낯 ?닿굅?ㅼ젙

    }

    // ?꾨젅?꾨쭏???몄텧(遺덇퇋移? 臾쇰━?④낵 ?녾굅???�?대㉧?먯꽌 ?ъ슜??
    void Update()
    {
        rt = tr.transform.rotation.y;
        playerRotate();//?뚯쟾 ?⑥닔 ?몄텧
        playerState();//?뚮젅?댁뼱 ?곹깭泥댄겕
    }

    //?쇱젙 諛섎났??洹쒖튃??臾쇰━?④낵?덈뒗 ?ㅻ툕?앺듃???ъ슜)
    private void FixedUpdate()
    {
        mouseRotate.UpdateCursorLock();//mouseRotate??UpdateCursorLock()?ㅽ뻾
        playerAim();//?쇱そ?ㅻ? ?꾨Ⅴ怨??덉쓣??
        playerIdle();//硫덉떠?덉쓣??
        playerMove();//?�吏곸엫
        playerWalk();//嫄룰린
        playerRun();//?щ━湲?
        playerJump();//?먰봽 ?몄텧
                     //playerSit();//?됯린

    }

    void OnEnable()
    {
        //StartCoroutine(playerState());


    }

    //?곹깭?뺤씤 硫붿냼??
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


    //?대룞硫붿냼??
    private void playerMove()
    {

        //bool isMove=moveDir.magnitude>0;
        //?닿굅???꾩튂媛믪쓣 怨꾩궛?댁꽌 0蹂대떎 ?щ㈃ true, ?꾨땲硫?false???ｋ뒗 湲곕뒫 ?섏?留????ㅽ겕由쏀듃???щ윭媛�吏� ?�吏곸엫?댁엳湲곗뿉 ?곹빀?섏? ?딆쓬 李멸퀬!
        h = Input.GetAxisRaw("Horizontal");//?좊땲?곗냽 input manager??Horizontal??吏�?뺣맂 ??a,d瑜??꾨???洹멸컪??h???�??媛�濡?
        v = Input.GetAxisRaw("Vertical");//?좊땲?곗냽 input manager??Vertical??吏�?뺣맂 ??w,s瑜??꾨???洹멸컪??v???�???몃줈)

        // 媛�濡? ?몃줈???�吏곸엫??怨꾩궛 ??媛믪쓣 ?�??
        //Vector3.forward-z異뺤쓣 媛�由ы궡==(0,0,1) / Vector3.right-x異뺤쓣 媛�由ы궡==(1,0,0)
        Vector3 MoveDir = (Vector3.forward * v) + (Vector3.right * h);//(0,0,v)+(h,0,0)=(h,0,v)

        tr.Translate(MoveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
        //?뚮옒?댁뼱瑜?怨꾩궛???꾩튂濡??대룞
        //normalized???뺢퇋?붾줈 ?뚮옒?댁뼱??濡쒖뺄醫뚰몴瑜?(1,1,1)濡?珥덇린?붿떆?????쒗궗???�吏곸엫??臾몄젣諛쒖깮)(?ш린?쒕뒗 媛믪씠 ?덉쑝硫?1.0.1)濡?珥덇린??
        // ?꾨젅?꾩닔 ?듭씪???꾪빐 deltaTime???ъ슜??
        //Space.Self??濡쒖뺄醫뚰몴瑜??삵븳?? 諛섎?濡?Space.world???붾뱶醫뚰몴瑜??삵븳??(?붾뱶濡??좎떆 ?ъ옄由?嫄몄쓬???좉쾬??

    }

    //?먯엫 ?ㅼ젙硫붿냼??
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

    //嫄룰린 ?ㅼ젙硫붿냼??
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

    //?щ━湲??ㅼ젙硫붿냼??
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



    //?뚯쟾 ?ㅼ젙 硫붿냼??
    private void playerRotate()
    {
        mouseRotate.LookRotation(tr, _camera.transform);
        //mouseRotated??LookRotation?⑥닔???뚮젅?댁뼱??tr(transform)?뺣낫?� 移대찓?쇱쓽 transform???뺣낫瑜?以�??

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


    }

    //?먰봽 ?ㅼ젙硫붿냼??
    private void playerJump()
    {
        if (_state == State.JUMP && startJump == true)//?좊땲?곗뿉 input manager???�?λ맂 jump??space諛붿씠??)瑜??꾨Ⅴ怨?isJumping??false?대㈃ 
        {

            animator.SetTrigger(hashJump);
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            //AddForce()???ㅻ툕?앺듃???섏쓣 媛�?좊븣 ?곕뒗 ?⑥닔
            //?꾩そ y異뺤쑝濡?jumpHight留뚰겮 ?섏쓣 二쇨린+ ForceMode.Impulse??吏㏃? ?쒓컙???섏쓣二쇰뒗 紐⑤뱶?쇨퀬 ??
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


    //?됯린 ?ㅼ젙硫붿냼??
    private void playerSit()
    {


    }
    //

    //異⑸룎媛먯?
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            downToFloar = true;
        }
    }



}
