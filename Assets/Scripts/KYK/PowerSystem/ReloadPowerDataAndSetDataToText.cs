using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using XCharts.Runtime;

/// <summary>
/// ������ �¾籤 ������ �����͸� �����ϰ� UI�� ǥ���ϴ� Ŭ����
/// </summary>
public class PowerDataManager : MonoBehaviour
{
    #region UI References
    [Header("UI References")]
    [SerializeField] private Text dateText;                           // ���� ��¥�� ǥ���ϴ� Text
    [SerializeField] private Text timeText;                          // ���� �ð��� ǥ���ϴ� Text
    [SerializeField] private TextMeshProUGUI[] topCityNameTexts;     // ���� ���� �̸��� ǥ���ϴ� TextMeshPro �迭
    [SerializeField] private TextMeshProUGUI[] topCityValueTexts;    // ���� ���� �������� ǥ���ϴ� TextMeshPro �迭
    [SerializeField] private PieChart pieChart;                      // ���� �������� ǥ���ϴ� ���� ��Ʈ
    [SerializeField] private TextMeshProUGUI[] topProvincePercentageTexts;  // ���� �� ������ ������ ǥ���ϴ� TextMeshPro �迭
    [SerializeField] private TextMeshProUGUI[] regionPowerTexts;     // ��ü ���� �������� ǥ���ϴ� TextMeshPro �迭
    [SerializeField] private TextMeshProUGUI[] powerIndicators;      // ������ ��ǥ�� ǥ���ϴ� TextMeshPro �迭
    #endregion

    #region Configuration Constants
    [Header("Configuration")]
    // Firebase �����ͺ��̽� �⺻ URL
    private const string BASE_URL = "https://solarpowerdata-default-rtdb.firebaseio.com";
    private const string TIME_SUFFIX = "_50";          // �ð� ���� ���̻�
    private const int TOP_CITIES_COUNT = 3;           // ǥ���� ���� ���� ��
    private const int TOP_PROVINCES_COUNT = 4;        // ǥ���� ���� �� ��

    // ���� �ڵ� �迭 (Firebase �����ͺ��̽��� Ű��)
    private static readonly string[] REGION_CODES = {
        "data_11", "data_26", "data_27", "data_28", "data_29", "data_30", "data_31",
        "data_36", "data_41", "data_42", "data_43", "data_44", "data_45", "data_46",
        "data_47", "data_48", "data_50"
    };

    // ���� �̸� �迭 (UI�� ǥ�õ� �̸�)
    private static readonly string[] REGION_NAMES = {
        "����Ư����", "�λ걤����", "�뱸������", "��õ������", "���ֱ�����", "����������", "��걤����",
        "����Ư����", "��⵵", "������", "��û�ϵ�", "��û����", "����ϵ�", "���󳲵�",
        "���ϵ�", "��󳲵�", "����Ư����ġ��"
    };
    #endregion

    #region Private Fields
    private double totalPower;           // ��ü ������ �հ�
    private string currentDateFolder;    // ���� ��¥ ������
    private string currentTimeFolder;    // ���� �ð� ������
    #endregion

    /// <summary>
    /// ������Ʈ �ʱ�ȭ �� ȣ��Ǵ� �޼���
    /// </summary>
    private void Start()
    {
        InitializeData();
    }

    /// <summary>
    /// ������ �ʱ� ������ �����ϴ� �޼���
    /// </summary>
    private void InitializeData()
    {
        totalPower = 0;
        UpdateDataDisplay();
    }

    /// <summary>
    /// UI �����͸� ������Ʈ�ϰ� ���ο� �����͸� �������� �޼���
    /// </summary>
    public void UpdateDataDisplay()
    {
        SetUIVisibility(false);
        UpdateFolderPaths();
        StartCoroutine(FetchPowerData());
    }

    /// <summary>
    /// ���� ��¥�� �ð��� ���� ���� ��θ� ������Ʈ�ϴ� �޼���
    /// </summary>
    private void UpdateFolderPaths()
    {
        currentDateFolder = $"{dateText.text}_REMS";
        currentTimeFolder = GetAdjustedTimeFolder();
    }

    /// <summary>
    /// 1�ð� �� �����͸� �������� ���� �ð��� �����ϴ� �޼���
    /// </summary>
    private string GetAdjustedTimeFolder()
    {
        string[] timeParts = timeText.text.Split(":");
        int adjustedHour = int.Parse(timeParts[0]) - 1;

        if (adjustedHour < 0) return "00" + TIME_SUFFIX;
        return (adjustedHour < 10 ? "0" : "") + adjustedHour + TIME_SUFFIX;
    }

    /// <summary>
    /// ��� ������ ������ �����͸� �������� �ڷ�ƾ
    /// </summary>
    private IEnumerator FetchPowerData()
    {
        totalPower = 0;
        Dictionary<string, double> cityData = new Dictionary<string, double>();
        Dictionary<string, double> provinceData = new Dictionary<string, double>();

        for (int i = 0; i < REGION_CODES.Length; i++)
        {
            yield return StartCoroutine(FetchRegionData(i, cityData, provinceData));
        }

        UpdateTopCities(cityData);
        UpdateTopProvinces(provinceData);
        SetUIVisibility(true);
    }

    /// <summary>
    /// Ư�� ������ ������ �����͸� �������� �ڷ�ƾ
    /// </summary>
    private IEnumerator FetchRegionData(int index, Dictionary<string, double> cityData, Dictionary<string, double> provinceData)
    {
        string url = $"{BASE_URL}/{currentDateFolder}/{currentTimeFolder}/{REGION_CODES[index]}.json";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ProcessRegionData(index, request.downloadHandler.text, cityData, provinceData);
            }
            else
            {
                Debug.LogError($"Failed to fetch data for {REGION_NAMES[index]}: {request.error}");
            }
        }
    }

    /// <summary>
    /// ������ ���� �����͸� ó���ϰ� �����ϴ� �޼���
    /// </summary>
    private void ProcessRegionData(int index, string jsonData, Dictionary<string, double> cityData, Dictionary<string, double> provinceData)
    {
        var powerData = JsonUtility.FromJson<PowerDataInfoArray>("{\"powerDataInfo\":" + jsonData + "}");
        double powerValue = powerData.powerDataInfo[0].dayGelec;

        totalPower += powerValue;
        UpdateRegionUI(index, powerValue);

        // ����(0-6)�� ��(8-15) �����͸� ���� �ٸ� ��ųʸ��� ����
        if (index < 7) cityData[REGION_NAMES[index]] = powerValue;
        else if (index >= 8 && index < 16) provinceData[REGION_NAMES[index]] = powerValue;
    }

    /// <summary>
    /// ������ UI �ؽ�Ʈ�� ������Ʈ�ϴ� �޼���
    /// </summary>
    private void UpdateRegionUI(int index, double powerValue)
    {
        regionPowerTexts[index].text = powerValue.ToString();
        powerIndicators[index].text = $"{powerValue:F2}";
    }

    /// <summary>
    /// ���� ���õ��� ������ ������ UI�� ������Ʈ�ϴ� �޼���
    /// </summary>
    private void UpdateTopCities(Dictionary<string, double> cityData)
    {
        var topCities = cityData.OrderByDescending(x => x.Value).Take(TOP_CITIES_COUNT);
        int index = 0;

        foreach (var city in topCities)
        {
            topCityNameTexts[index].text = city.Key;
            topCityValueTexts[index].text = $"{city.Value:F2}MWh";
            index++;
        }
    }

    /// <summary>
    /// ���� ������ ������ ������ ������Ʈ�� UI�� ������Ʈ�ϴ� �޼���
    /// </summary>
    private void UpdateTopProvinces(Dictionary<string, double> provinceData)
    {
        var topProvinces = provinceData.OrderByDescending(x => x.Value).Take(TOP_PROVINCES_COUNT);
        int index = 0;

        foreach (var province in topProvinces)
        {
            double percentage = 360 * province.Value / totalPower;
            UpdatePieChartData(index, province.Key, percentage);
            topProvincePercentageTexts[index].text = $"{(percentage * 100 / 360):F1}%";
            index++;
        }
    }

    /// <summary>
    /// ������Ʈ �����͸� ������Ʈ�ϴ� �޼���
    /// </summary>
    private void UpdatePieChartData(int index, string name, double value)
    {
        pieChart.UpdateData(index, 0, value);
        pieChart.UpdateDataName(index, 0, name);
        pieChart.UpdateDataName(index, 1, name);
    }

    /// <summary>
    /// UI ��ҵ��� ���ü��� �����ϴ� �޼���
    /// </summary>
    private void SetUIVisibility(bool visible)
    {
        foreach (var text in regionPowerTexts)
        {
            text.enabled = visible;
        }
    }
}