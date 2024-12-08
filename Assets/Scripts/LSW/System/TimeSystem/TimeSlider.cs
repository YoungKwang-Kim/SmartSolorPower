using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TimeSlider : MonoBehaviour
{
    public Slider timeSlider; 
    public Text timeText;
    private DateTime currentTime = DateTime.Now;

    private void OnEnable()
    {
        timeText.text = currentTime.ToString("HH") + ":00";
        // �����̴� ���� ����� ������
        timeSlider.onValueChanged.AddListener(UpdateTime);
        timeSlider.value = int.Parse(currentTime.ToString("HH"));
    }

    // �����̴� ���� ����� �� 
    void UpdateTime(float value)
    {
        // �����̴��� ���� �ð����� ��ȯ -> �ؽ�Ʈ
        timeText.text = Mathf.RoundToInt(value).ToString() + ":00";
    }
}
