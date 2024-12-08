//using UnityEngine;
//using System.IO.Ports;


//public class ArduinoControl : MonoBehaviour
//{
//    SerialPort stream = new SerialPort("COM3", 9600); // ��Ʈ�� ���巹��Ʈ ����
//    public GameObject panel; // �����̼��� ������ ������Ʈ

//    private Quaternion prevRotation; // ���� �����̼��� �����ϴ� ����

//    void Start()
//    {
//        stream.Open(); // �ø��� ��Ʈ ����
//        prevRotation = panel.transform.rotation; // �ʱ� �����̼� �� ����
//    }

//    void Update()
//    {
//        if (stream.IsOpen)
//        {
//            try
//            {
//                // ���� ���� ������ �޾ƿ���
//                string value = stream.ReadLine();
//                float angle = float.Parse(value);

//                // 800 ������ ���� ���ؼ��� 0���� ���ƿ����� ����
//                if (angle <= 800)
//                {
//                    panel.transform.rotation = Quaternion.Euler(0, 0, 0);
//                }
//                // 800 �ʰ��� ���� ���ؼ��� 60���� ����
//                else if (angle > 800 && angle != 60)
//                {
//                    panel.transform.rotation = Quaternion.Euler(0, 60, 0);
//                }

//                // �����̼� ���� ����� ��쿡�� Debug.Log�� ���
//                if (panel.transform.rotation != prevRotation)
//                {
//                    Debug.Log("���� ȸ�� ����: " + panel.transform.rotation.eulerAngles.y);
//                    prevRotation = panel.transform.rotation;
//                }
//            }
//            catch (System.Exception)
//            {
//                // ���� ó�� - �߸��� ������ �����͸� ���� ���
//            }
//        }
//    }

//    void OnDestroy()
//    {
//        stream.Close(); // ���α׷� ���� �� ��Ʈ �ݱ�
//    }
//}