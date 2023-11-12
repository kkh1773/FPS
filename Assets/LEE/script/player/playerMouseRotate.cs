using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//y축 x축 시선 고정식

[System.Serializable]
public class playerMouseRotate
{
    public float xSensitivity = 5f;//x축 민감도
    public float ySensitivity = 5f;//y축 민감도
    public bool clampVR = true;//vertical 회전 제한 여부
    public float minRotateX = -70f;//x축 최소 앵글
    public float maxRotateY = 70f;//y축 최소 앵글
    public bool smooth = true;//부드러움 여부
    public float smoothTime = 5f;//브드러움 강도설정
    public bool lockCursor = true;//가시성 여부//마우스 커서를 보일꺼냐 안보일꺼냐의 여부


    private Quaternion playerTargetRotate;//플래이어 회전 계산저장 변수
    private Quaternion cameraTargetRotate;//카메라 회전 계산 저장 변수
    private bool m_cursorIsLocked = true;//커서가 잠금되어있는지 확인

    //회전값 삽입
    public void Init(Transform player, Transform camera)//플레이어, 카메라의 회전값 받기
    {
        playerTargetRotate = player.localRotation;//넘겨받은  플레이어의 로컬(상대)좌표 회전값 저장
        cameraTargetRotate = camera.localRotation;//넘겨받은  카메라의 로컬(상대)좌표 회전값 저장
    }

    //회전값 계산
    public void LookRotation(Transform player, Transform camera)
    {
        float yRotate = Input.GetAxis("Mouse X") * xSensitivity;//입력된 크기의 2배해줌
        float xRotate = Input.GetAxis("Mouse Y") * ySensitivity;//입력된 크기의 2배해줌


        //카메라, 플레이어의 회전값 계산식
        playerTargetRotate *= Quaternion.Euler(0f, yRotate, 0f);//y축 기준으로 플레이어의 회전값 계산
        cameraTargetRotate *= Quaternion.Euler(-xRotate, 0f, 0f);//x축 기준으로 카메라의 회전값 계산
        //x축에 -가 붙는 이유는 위를 보면 -축으로 움직이기 때문이다.

        if (clampVR)//vertical 회전 제한이 걸리면(세로)
            cameraTargetRotate = ClampRotationX(cameraTargetRotate); //ClampRotationX메소드로 카메라의 계산삾을 보내준다. 제한 검

        if (smooth)//부드러움이 작동 한다면
        {
            player.localRotation = Quaternion.Slerp(player.localRotation, playerTargetRotate, smoothTime * Time.deltaTime);//플레이어의 현재 회전값과 계산한 플레이어의 회전값 사이를 5초정도로 구형 보간합니다.
            camera.localRotation = Quaternion.Slerp(camera.localRotation, cameraTargetRotate, smoothTime * Time.deltaTime);//카메라의 현재 회전값과 계산한 카메라의 회전값 사이를 5초정도로 구형 보간합니다.
            //보간 함수는 몇초정도 회전을 하는걸 말한다.
        }
        else
        {
            player.localRotation = playerTargetRotate;//플레이어의 로컬좌표는 계산한 좌표이다.
            camera.localRotation = cameraTargetRotate;//카메라의 로컬좌표는 계산한 좌표이다.
        }

        }

    public void SetCursorLock(bool value)//커서 풀림 확인 메소드
    {
        lockCursor = value;//bool값 대입
        if (!lockCursor)//false일 경우
        {
            Cursor.lockState = CursorLockMode.None;//잠김 풀림
            Cursor.visible = true;//커서보임
        }
    }

    public void UpdateCursorLock()//커서 잠김 확인 메소드
    {
        if (lockCursor)//true일 경우
        {
            InternalLockUpdate(); //InternalLockUpdate메소드 호출
        }
    }

    private void InternalLockUpdate()//커서 상태 변환 메소드
    {
        /*if (Input.GetKeyUp(KeyCode.M))//M를 누르면 마우스 활성화를 시켜 회전하게 됨
        {
            m_cursorIsLocked = true;//락 걸림
        }*/

        if (m_cursorIsLocked)//true일 경우
        {
            Cursor.lockState = CursorLockMode.Locked;//마우스 가운데로 락걸림
            Cursor.visible = false;//커서 사라짐
        }
        /*else if (!m_cursorIsLocked)//false일 경우
        {
            Cursor.lockState = CursorLockMode.None;//락 풀림
            Cursor.visible = true;//커서보임
        }*/
    }


    //쿼터니언은 나중에 자세히 공부한후 써라 지근은 그냥 이렇게 쓴다고만 알아두어라.
    private Quaternion ClampRotationX(Quaternion quat)//회정 최대치에 도달할 경우 작동
    {
        quat.x /= quat.w; //x값을 
        quat.y /= quat.w;
        quat.z /= quat.w;
        quat.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(quat.x);
        angleX = Mathf.Clamp(angleX, minRotateX, maxRotateY);
        quat.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return quat;
    }

}


