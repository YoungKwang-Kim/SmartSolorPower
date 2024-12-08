using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// �¾籤 �������� ���� �����͸� �����ϰ� ������Ʈ�ϴ� Ŭ����
/// </summary>
public class PowerManager : MonoBehaviour
{
    #region UI References
    [Header("UI Elements")]
    [SerializeField] private Text dateText;    // ���� ��¥ ǥ�� Text
    [SerializeField] private Text timeText;    // ���� �ð� ǥ�� Text
    #endregion

    #region Private Fields
    private JsonParsing powerData;             // ������ ������ �Ľ� ��ü
    private string lastUpdateTime;             // ������ ������Ʈ �ð�
    private const string TIME_SUFFIX = "_50";  // �ð� ���� ���̻�
    private const string DATE_SUFFIX = "_REMS"; // ��¥ ���� ���̻�
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// ������Ʈ �ʱ�ȭ �� ȣ��Ǵ� �޼���
    /// </summary>
    private void Start()
    {
        InitializePowerData();
    }

    /// <summary>
    /// �� �����Ӹ��� ȣ��Ǵ� �޼���
    /// �ð��� ����Ǿ��� ���� �����͸� ������Ʈ
    /// </summary>
    private void Update()
    {
        if (HasTimeChanged())
        {
            UpdatePowerData();
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// ���� ������ �ʱ� ������ �����ϴ� �޼���
    /// </summary>
    private void InitializePowerData()
    {
        powerData = new JsonParsing();
        UpdateFolderPaths();
        lastUpdateTime = timeText.text;
    }
    #endregion

    #region Data Update Methods
    /// <summary>
    /// �ð� ���� ���θ� Ȯ���ϴ� �޼���
    /// </summary>
    /// <returns>�ð��� ����Ǿ����� true, �ƴϸ� false</returns>
    private bool HasTimeChanged()
    {
        return lastUpdateTime != timeText.text;
    }

    /// <summary>
    /// ���� �����͸� ������Ʈ�ϴ� �޼���
    /// </summary>
    private void UpdatePowerData()
    {
        UpdateFolderPaths();
        StartCoroutine(FetchPowerData());
        lastUpdateTime = timeText.text;
    }

    /// <summary>
    /// ��¥�� �ð��� ���� ���� ��θ� ������Ʈ�ϴ� �޼���
    /// </summary>
    private void UpdateFolderPaths()
    {
        string currentDate = dateText.text;
        string currentTime = ParseTimeFromText();

        powerData.dateFileName = $"{currentDate}{DATE_SUFFIX}";
        powerData.timeFileName = $"{currentTime}{TIME_SUFFIX}";
    }

    /// <summary>
    /// �ð� �ؽ�Ʈ���� �ð� ���� �Ľ��ϴ� �޼���
    /// </summary>
    /// <returns>�Ľ̵� �ð� ���ڿ�</returns>
    private string ParseTimeFromText()
    {
        string[] timeComponents = timeText.text.Split(':');
        return timeComponents[0];
    }

    /// <summary>
    /// ���� �����͸� �񵿱������� �������� �ڷ�ƾ
    /// </summary>
    private IEnumerator FetchPowerData()
    {
        yield return StartCoroutine(powerData.GetChargeInfo());
    }
    #endregion
}