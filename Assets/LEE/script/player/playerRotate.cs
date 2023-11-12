using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;// 아마 view에 나타나는 메소드 사용을 위한 것
using System.Diagnostics;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class playerRotate : MonoBehaviour
{
    [SerializeField] private Transform tr;//transform 저장

    [SerializeField] private playerMouseRotate mouseRotate;
    [SerializeField] private Camera _camera;


    // Start is called before the first frame update
    void Start()
    {
        
       //tr = GetComponent<Transform>();//transform

        //_camera = GetComponent<Camera>();//하위항목에 Camera
        mouseRotate.Init(tr, _camera.transform);//playerMouseRotate클래스의 메소드 lnit에 플레이어 위치와 카메라 정보값 전달(초기값 설정)
    }

    // Update is called once per frame
    void Update()
    {
        player_Rotate();//회전 함수 호출
    }

    private void FixedUpdate()
    {
       mouseRotate.UpdateCursorLock();//mouseRotate안 UpdateCursorLock()실행
      
                  

    }

    private void player_Rotate()
    {
        mouseRotate.LookRotation(tr, _camera.transform);
        
        //mouseRotated의 LookRotation함수에 플레이어의 tr(transform)정보와 카메라의 transform의 정보를 준다.


    }
}
