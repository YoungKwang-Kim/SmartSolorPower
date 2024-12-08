using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class solCalendar : MonoBehaviour
{
    // �޷��� ǥ���� UI ��ҵ�
    public GameObject calendarPanel;
    public Text yearNumText;  // �⵵�� ǥ���� �ؽ�Ʈ UI
    public Text monthNumText; // ���� ǥ���� �ؽ�Ʈ UI

    // ��¥ ������ ������
    public GameObject dateitem;

   
    public List<GameObject> dateprefabs = new List<GameObject>();
    const int totalDateNum = 42; // �� ȭ�鿡 ǥ���� �� ��¥ ������ ��

    private DateTime dateTime;
    public static solCalendar solcalendarInstance;

    void Start()
    {
        solcalendarInstance = this;

        // ��¥ ������ �ʱ�ȭ
        Vector3 startPos = dateitem.transform.localPosition;
        dateprefabs.Clear();
        dateprefabs.Add(dateitem);

        //��¥ �����۵��� �����ϰ� ��ġ ����
        for (int i = 1; i < totalDateNum; i++)
        {
            GameObject item = GameObject.Instantiate(dateitem) as GameObject;
            //��¥ ������ �����Ͽ� ���ο� GameObject�� ����

            item.name = "Item" + (i + 1).ToString();
           

            item.transform.SetParent(dateitem.transform.parent);
            //������ GameObject�� dateitem�� �θ�� �����Ͽ� ȭ�鿡 ǥ�õ� ��ġ�� ����

            item.transform.localScale = Vector3.one;
            //������ GameObject�� ũ�⸦ ���� ũ��� ����

            item.transform.localRotation = Quaternion.identity;
            //������ GameObject�� ȸ���� �ʱ�ȭ
            item.transform.localPosition = new Vector3((i % 7) * 36 + startPos.x, startPos.y - (i / 7) * 30, startPos.z);
            //������ GameObject�� ��ġ�� ����Ͽ� ����. �� ��¥ �������� 7��, �� �� ������ 30, �� ������ 36
            dateprefabs.Add(item);
            //������ GameObject�� dateprefabs ����Ʈ�� �߰��մϴ�. �� ����Ʈ�� �Ŀ� ������ ��¥ �����ۿ� �����ϱ� ���� ���
        }

        dateTime = DateTime.Now;

        // ���� ���� �޷� ����
        CreateCalendar();

        calendarPanel.SetActive(false); // ó������ ���̵��� �����߽��ϴ�.
    }

    // ���� ���� �޷��� �����ϴ� �Լ�
    void CreateCalendar()
    {
        
        DateTime firstDay = dateTime.AddDays(-(dateTime.Day - 1));
        int index = GetDays(firstDay.DayOfWeek);
        //ù ���� ���Ͽ� ���� �ش� ���Ͽ� �´� �ε��� �� ������.
        //GetDays �ռ� --> ������ ���ڷ� ��ȯ

        int date = 0;
        for (int i = 0; i < totalDateNum; i++)
        {
            Text label = dateprefabs[i].GetComponentInChildren<Text>();
            dateprefabs[i].SetActive(false); //dateItem��Ȱ��ȭ -> ��� ��¥ �ʱ�ȭ

            if (i >= index)
            {
                // ���� ���� �ش��ϴ� ��¥�� Ȱ��ȭ
                DateTime thatDay = firstDay.AddDays(date);
                if (thatDay.Month == firstDay.Month)
                {
                    dateprefabs[i].SetActive(true); //dateItem Ȱ��ȭ

                    label.text = (date + 1).ToString(); //dateItem�� text�ȿ� ��¥ ǥ��
                    date++; //���� ��¥��
                }
            }
        }
        yearNumText.text = dateTime.Year.ToString();
        monthNumText.text = dateTime.Month.ToString("D2");  // �� �ڸ� �� ���� �� �ڸ��� ǥ�� (01, 02, ..., 12)
    }

    // ���Ͽ� ���� �ε��� ��ȯ
    int GetDays(DayOfWeek day) //GetDays �Լ� -> ������ ���ڷ� ��ȯ�ϴ� �Լ�
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 1;
            case DayOfWeek.Tuesday: return 2;
            case DayOfWeek.Wednesday: return 3;
            case DayOfWeek.Thursday: return 4;
            case DayOfWeek.Friday: return 5;
            case DayOfWeek.Saturday: return 6;
            case DayOfWeek.Sunday: return 0;
        }

        return 0;
    }


    public void YearPrev()
    {
        dateTime = dateTime.AddYears(-1);
        CreateCalendar();
    }

    public void YearNext()
    {
        dateTime = dateTime.AddYears(1);
        CreateCalendar();
    }

    public void MonthPrev()
    {
        dateTime = dateTime.AddMonths(-1);
        CreateCalendar();
    }

    public void MonthNext()
    {
        dateTime = dateTime.AddMonths(1);
        CreateCalendar();
    }


    // ó������ �޷��� ���̵��� �����߽��ϴ�.
    // �޷��� ���̰� �ϴ� �Լ�
    public void ShowCalendar(Text target)
    {
        calendarPanel.SetActive(true);
        _target = target;
        //calendarPanel.transform.position = new Vector3(965, 475, 0);//Input.mousePosition-new Vector3(0,120,0);
    }

    
    Text _target;
    //public Text text_Select_Date; //�Ű����� ���� ��� �ؽ�Ʈ ����� ����



    // ������ Ŭ���� Text�� ��¥ ǥ���ϴ� �Լ�
    public void OnDateItemClick(string day)
    {
        _target.text = yearNumText.text + "-" + monthNumText.text + "-" + int.Parse(day).ToString("D2");
        calendarPanel.SetActive(true);
    }
}
