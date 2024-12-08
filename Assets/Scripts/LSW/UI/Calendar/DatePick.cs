using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatePick : MonoBehaviour
{
    public Slider slider;
    public void OnDateItemClick()
    {
        CalendarController._calendarInstance.OnDateItemClick(gameObject.GetComponentInChildren<Text>().text);
        //���� ��ũ��Ʈ�� ������ ��ü(��¥ ������)�� �ڽ� �߿��� Text ������Ʈ�� �ؽ�Ʈ ��������
        slider.value = 12f;
    }
}
