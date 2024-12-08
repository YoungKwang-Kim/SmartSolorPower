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
/// 지역별 태양광 발전량 데이터를 관리하고 UI에 표시하는 클래스
/// </summary>
public class PowerDataManager : MonoBehaviour
{
    #region UI References
    [Header("UI References")]
    [SerializeField] private Text dateText;                           // 현재 날짜를 표시하는 Text
    [SerializeField] private Text timeText;                          // 현재 시간을 표시하는 Text
    [SerializeField] private TextMeshProUGUI[] topCityNameTexts;     // 상위 도시 이름을 표시하는 TextMeshPro 배열
    [SerializeField] private TextMeshProUGUI[] topCityValueTexts;    // 상위 도시 발전량을 표시하는 TextMeshPro 배열
    [SerializeField] private PieChart pieChart;                      // 도별 발전량을 표시하는 파이 차트
    [SerializeField] private TextMeshProUGUI[] topProvincePercentageTexts;  // 상위 도 발전량 비율을 표시하는 TextMeshPro 배열
    [SerializeField] private TextMeshProUGUI[] regionPowerTexts;     // 전체 지역 발전량을 표시하는 TextMeshPro 배열
    [SerializeField] private TextMeshProUGUI[] powerIndicators;      // 발전량 지표를 표시하는 TextMeshPro 배열
    #endregion

    #region Configuration Constants
    [Header("Configuration")]
    // Firebase 데이터베이스 기본 URL
    private const string BASE_URL = "https://solarpowerdata-default-rtdb.firebaseio.com";
    private const string TIME_SUFFIX = "_50";          // 시간 폴더 접미사
    private const int TOP_CITIES_COUNT = 3;           // 표시할 상위 도시 수
    private const int TOP_PROVINCES_COUNT = 4;        // 표시할 상위 도 수

    // 지역 코드 배열 (Firebase 데이터베이스의 키값)
    private static readonly string[] REGION_CODES = {
        "data_11", "data_26", "data_27", "data_28", "data_29", "data_30", "data_31",
        "data_36", "data_41", "data_42", "data_43", "data_44", "data_45", "data_46",
        "data_47", "data_48", "data_50"
    };

    // 지역 이름 배열 (UI에 표시될 이름)
    private static readonly string[] REGION_NAMES = {
        "서울특별시", "부산광역시", "대구광역시", "인천광역시", "광주광역시", "대전광역시", "울산광역시",
        "세종특별시", "경기도", "강원도", "충청북도", "충청남도", "전라북도", "전라남도",
        "경상북도", "경상남도", "제주특별자치도"
    };
    #endregion

    #region Private Fields
    private double totalPower;           // 전체 발전량 합계
    private string currentDateFolder;    // 현재 날짜 폴더명
    private string currentTimeFolder;    // 현재 시간 폴더명
    #endregion

    /// <summary>
    /// 컴포넌트 초기화 시 호출되는 메서드
    /// </summary>
    private void Start()
    {
        InitializeData();
    }

    /// <summary>
    /// 데이터 초기 설정을 수행하는 메서드
    /// </summary>
    private void InitializeData()
    {
        totalPower = 0;
        UpdateDataDisplay();
    }

    /// <summary>
    /// UI 데이터를 업데이트하고 새로운 데이터를 가져오는 메서드
    /// </summary>
    public void UpdateDataDisplay()
    {
        SetUIVisibility(false);
        UpdateFolderPaths();
        StartCoroutine(FetchPowerData());
    }

    /// <summary>
    /// 현재 날짜와 시간에 따른 폴더 경로를 업데이트하는 메서드
    /// </summary>
    private void UpdateFolderPaths()
    {
        currentDateFolder = $"{dateText.text}_REMS";
        currentTimeFolder = GetAdjustedTimeFolder();
    }

    /// <summary>
    /// 1시간 전 데이터를 가져오기 위해 시간을 조정하는 메서드
    /// </summary>
    private string GetAdjustedTimeFolder()
    {
        string[] timeParts = timeText.text.Split(":");
        int adjustedHour = int.Parse(timeParts[0]) - 1;

        if (adjustedHour < 0) return "00" + TIME_SUFFIX;
        return (adjustedHour < 10 ? "0" : "") + adjustedHour + TIME_SUFFIX;
    }

    /// <summary>
    /// 모든 지역의 발전량 데이터를 가져오는 코루틴
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
    /// 특정 지역의 발전량 데이터를 가져오는 코루틴
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
    /// 가져온 지역 데이터를 처리하고 저장하는 메서드
    /// </summary>
    private void ProcessRegionData(int index, string jsonData, Dictionary<string, double> cityData, Dictionary<string, double> provinceData)
    {
        var powerData = JsonUtility.FromJson<PowerDataInfoArray>("{\"powerDataInfo\":" + jsonData + "}");
        double powerValue = powerData.powerDataInfo[0].dayGelec;

        totalPower += powerValue;
        UpdateRegionUI(index, powerValue);

        // 도시(0-6)와 도(8-15) 데이터를 각각 다른 딕셔너리에 저장
        if (index < 7) cityData[REGION_NAMES[index]] = powerValue;
        else if (index >= 8 && index < 16) provinceData[REGION_NAMES[index]] = powerValue;
    }

    /// <summary>
    /// 지역별 UI 텍스트를 업데이트하는 메서드
    /// </summary>
    private void UpdateRegionUI(int index, double powerValue)
    {
        regionPowerTexts[index].text = powerValue.ToString();
        powerIndicators[index].text = $"{powerValue:F2}";
    }

    /// <summary>
    /// 상위 도시들의 발전량 정보를 UI에 업데이트하는 메서드
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
    /// 상위 도들의 발전량 정보를 파이차트와 UI에 업데이트하는 메서드
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
    /// 파이차트 데이터를 업데이트하는 메서드
    /// </summary>
    private void UpdatePieChartData(int index, string name, double value)
    {
        pieChart.UpdateData(index, 0, value);
        pieChart.UpdateDataName(index, 0, name);
        pieChart.UpdateDataName(index, 1, name);
    }

    /// <summary>
    /// UI 요소들의 가시성을 설정하는 메서드
    /// </summary>
    private void SetUIVisibility(bool visible)
    {
        foreach (var text in regionPowerTexts)
        {
            text.enabled = visible;
        }
    }
}