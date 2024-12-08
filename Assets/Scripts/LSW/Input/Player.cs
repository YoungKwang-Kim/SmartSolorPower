using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotSpeed = 2000f;
    float mx;
    float my;
    bool isRotatable = true;

    private void Start()
    {
        // ī�޶��� �ʱ� ���� ȸ���� ��������
        mx = transform.eulerAngles.y;
        my = -transform.eulerAngles.x;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // q�� ������ y�� �ϰ�, e�� ������ y�� ���
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        Vector3 movement = new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;
        transform.Translate(movement);

        // �����̽��ٸ� ������ �ִ� ������ ���콺�� ȸ������ ����
        if (!Input.GetKey(KeyCode.Space))
        {
            float rot_horizontal = Input.GetAxis("Mouse X");
            float ver_horizontal = Input.GetAxis("Mouse Y");

            mx += rot_horizontal * rotSpeed * Time.deltaTime;
            my += ver_horizontal * rotSpeed * Time.deltaTime;

            // ���Ʒ� �þ� y�� ����
            my = Mathf.Clamp(my, -80f, 80f);
            // x, y �� ȸ�� Vector3�� �����ؼ� �ۼ�
            transform.eulerAngles = new Vector3(-my, mx, 0);
        }
    }
}
