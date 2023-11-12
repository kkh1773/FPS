using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//y�� x�� �ü� ������

[System.Serializable]
public class playerMouseRotate
{
    public float xSensitivity = 5f;//x�� �ΰ���
    public float ySensitivity = 5f;//y�� �ΰ���
    public bool clampVR = true;//vertical ȸ�� ���� ����
    public float minRotateX = -70f;//x�� �ּ� �ޱ�
    public float maxRotateY = 70f;//y�� �ּ� �ޱ�
    public bool smooth = true;//�ε巯�� ����
    public float smoothTime = 5f;//��巯�� ��������
    public bool lockCursor = true;//���ü� ����//���콺 Ŀ���� ���ϲ��� �Ⱥ��ϲ����� ����


    private Quaternion playerTargetRotate;//�÷��̾� ȸ�� ������� ����
    private Quaternion cameraTargetRotate;//ī�޶� ȸ�� ��� ���� ����
    private bool m_cursorIsLocked = true;//Ŀ���� ��ݵǾ��ִ��� Ȯ��

    //ȸ���� ����
    public void Init(Transform player, Transform camera)//�÷��̾�, ī�޶��� ȸ���� �ޱ�
    {
        playerTargetRotate = player.localRotation;//�Ѱܹ���  �÷��̾��� ����(���)��ǥ ȸ���� ����
        cameraTargetRotate = camera.localRotation;//�Ѱܹ���  ī�޶��� ����(���)��ǥ ȸ���� ����
    }

    //ȸ���� ���
    public void LookRotation(Transform player, Transform camera)
    {
        float yRotate = Input.GetAxis("Mouse X") * xSensitivity;//�Էµ� ũ���� 2������
        float xRotate = Input.GetAxis("Mouse Y") * ySensitivity;//�Էµ� ũ���� 2������


        //ī�޶�, �÷��̾��� ȸ���� ����
        playerTargetRotate *= Quaternion.Euler(0f, yRotate, 0f);//y�� �������� �÷��̾��� ȸ���� ���
        cameraTargetRotate *= Quaternion.Euler(-xRotate, 0f, 0f);//x�� �������� ī�޶��� ȸ���� ���
        //x�࿡ -�� �ٴ� ������ ���� ���� -������ �����̱� �����̴�.

        if (clampVR)//vertical ȸ�� ������ �ɸ���(����)
            cameraTargetRotate = ClampRotationX(cameraTargetRotate); //ClampRotationX�޼ҵ�� ī�޶��� ��꘳�� �����ش�. ���� ��

        if (smooth)//�ε巯���� �۵� �Ѵٸ�
        {
            player.localRotation = Quaternion.Slerp(player.localRotation, playerTargetRotate, smoothTime * Time.deltaTime);//�÷��̾��� ���� ȸ������ ����� �÷��̾��� ȸ���� ���̸� 5�������� ���� �����մϴ�.
            camera.localRotation = Quaternion.Slerp(camera.localRotation, cameraTargetRotate, smoothTime * Time.deltaTime);//ī�޶��� ���� ȸ������ ����� ī�޶��� ȸ���� ���̸� 5�������� ���� �����մϴ�.
            //���� �Լ��� �������� ȸ���� �ϴ°� ���Ѵ�.
        }
        else
        {
            player.localRotation = playerTargetRotate;//�÷��̾��� ������ǥ�� ����� ��ǥ�̴�.
            camera.localRotation = cameraTargetRotate;//ī�޶��� ������ǥ�� ����� ��ǥ�̴�.
        }

        }

    public void SetCursorLock(bool value)//Ŀ�� Ǯ�� Ȯ�� �޼ҵ�
    {
        lockCursor = value;//bool�� ����
        if (!lockCursor)//false�� ���
        {
            Cursor.lockState = CursorLockMode.None;//��� Ǯ��
            Cursor.visible = true;//Ŀ������
        }
    }

    public void UpdateCursorLock()//Ŀ�� ��� Ȯ�� �޼ҵ�
    {
        if (lockCursor)//true�� ���
        {
            InternalLockUpdate(); //InternalLockUpdate�޼ҵ� ȣ��
        }
    }

    private void InternalLockUpdate()//Ŀ�� ���� ��ȯ �޼ҵ�
    {
        /*if (Input.GetKeyUp(KeyCode.M))//M�� ������ ���콺 Ȱ��ȭ�� ���� ȸ���ϰ� ��
        {
            m_cursorIsLocked = true;//�� �ɸ�
        }*/

        if (m_cursorIsLocked)//true�� ���
        {
            Cursor.lockState = CursorLockMode.Locked;//���콺 ����� ���ɸ�
            Cursor.visible = false;//Ŀ�� �����
        }
        /*else if (!m_cursorIsLocked)//false�� ���
        {
            Cursor.lockState = CursorLockMode.None;//�� Ǯ��
            Cursor.visible = true;//Ŀ������
        }*/
    }


    //���ʹϾ��� ���߿� �ڼ��� �������� ��� ������ �׳� �̷��� ���ٰ� �˾Ƶξ��.
    private Quaternion ClampRotationX(Quaternion quat)//ȸ�� �ִ�ġ�� ������ ��� �۵�
    {
        quat.x /= quat.w; //x���� 
        quat.y /= quat.w;
        quat.z /= quat.w;
        quat.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(quat.x);
        angleX = Mathf.Clamp(angleX, minRotateX, maxRotateY);
        quat.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return quat;
    }

}


