using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;// �Ƹ� view�� ��Ÿ���� �޼ҵ� ����� ���� ��
using System.Diagnostics;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class playerRotate : MonoBehaviour
{
    [SerializeField] private Transform tr;//transform ����

    [SerializeField] private playerMouseRotate mouseRotate;
    [SerializeField] private Camera _camera;


    // Start is called before the first frame update
    void Start()
    {
        
       //tr = GetComponent<Transform>();//transform

        //_camera = GetComponent<Camera>();//�����׸� Camera
        mouseRotate.Init(tr, _camera.transform);//playerMouseRotateŬ������ �޼ҵ� lnit�� �÷��̾� ��ġ�� ī�޶� ������ ����(�ʱⰪ ����)
    }

    // Update is called once per frame
    void Update()
    {
        player_Rotate();//ȸ�� �Լ� ȣ��
    }

    private void FixedUpdate()
    {
       mouseRotate.UpdateCursorLock();//mouseRotate�� UpdateCursorLock()����
      
                  

    }

    private void player_Rotate()
    {
        mouseRotate.LookRotation(tr, _camera.transform);
        
        //mouseRotated�� LookRotation�Լ��� �÷��̾��� tr(transform)������ ī�޶��� transform�� ������ �ش�.


    }
}
