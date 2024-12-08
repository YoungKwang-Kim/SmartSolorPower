using PowerPlant.Data;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using XCharts.Runtime;

/// <summary>
/// 발전소 데이터를 차트로 시각화하는 관리자 클래스
/// </summary>
public class ChartManager : MonoBehaviour
{
    #region Constants
    private const string BASE_URL = "https://solarpowerdata-default-rtdb.firebaseio.com";
    private const string TIME_FOLDER = "23_50";
    private const string DATE_SUFFIX = "_REMS";
    private const int DAYS_TO_DISPLAY = 5;
    private const float CHECK_INTERVAL = 60f;  // 날짜 변경 체크 간격 (초)
    #endregion

    #region Serialized Fields
    [Header("Chart Reference")]
    [SerializeField] private BarChart barChart;
    #endregion

    #region Private Fields
    private readonly string[] REGION_CODES = {
        "data_11", "data_26", "data_27", "data_28", "data_29", "data_30", "data_31",
        "data_36", "data_41", "data_42", "data_43", "data_44", "data_45", "data_46",
        "data_47", "data_48", "data_50"
    };

    private DateTime lastCheckedDate;
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// 컴포넌트 초기화 시 호출되는 메서드
    /// </summary>
    private void Start()
    {
        InitializeChart();
    }
    #endregion

    #region Initialization Methods
    /// <summary>
    /// 차트 초기 설정을 수행하는 메서드
    /// </summary>
    private void InitializeChart()
    {
        if (!ValidateComponents()) return;

        lastCheckedDate = DateTime.Now.Date;
        UpdateChart();
        StartCoroutine(MonitorDateChange());
    }

    /// <summary>
    /// 필수 컴포넌트들이 할당되었는지 확인하는 메서드
    /// </summary>
    private bool ValidateComponents()
    {
        if (barChart == null)
        {
            Debug.LogError("BarChart reference is missing!");
            return false;
        }
        return true;
    }
    #endregion

    #region Chart Update Methods
    /// <summary>
    /// 날짜 변경을 모니터링하는 코루틴
    /// </summary>
    private IEnumerator MonitorDateChange()
    {
        WaitForSeconds wait = new WaitForSeconds(CHECK_INTERVAL);

        while (true)
        {
            yield return wait;

            DateTime currentDate = DateTime.Now.Date;
            if (currentDate != lastCheckedDate)
            {
                lastCheckedDate = currentDate;
                UpdateChart();
            }
        }
    }

    /// <summary>
    /// 차트 데이터를 업데이트하는 메서드
    /// </summary>
    private void UpdateChart()
    {
        DateTime today = DateTime.Now;

        for (int i = 0; i < DAYS_TO_DISPLAY; i++)
        {
            DateTime targetDate = today.AddDays(i - DAYS_TO_DISPLAY);
            string formattedDate = targetDate.ToString("yyyy-MM-dd");

            barChart.UpdateXAxisData(i, $"{targetDate.Day}일");
            StartCoroutine(FetchDailyPowerData(formattedDate, i));
        }
    }

    /// <summary>
    /// 특정 날짜의 발전량 데이터를 가져오는 코루틴
    /// </summary>
    private IEnumerator FetchDailyPowerData(string date, int dataIndex)
    {
        double totalPower = 0;

        foreach (string regionCode in REGION_CODES)
        {
            string url = $"{BASE_URL}/{date}{DATE_SUFFIX}/{TIME_FOLDER}/{regionCode}.json";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    totalPower += ProcessPowerData(request.downloadHandler.text);
                }
                else
                {
                    Debug.LogWarning($"Failed to fetch data for {regionCode}: {request.error}");
                }
            }
        }

        UpdateChartData(dataIndex, totalPower);
    }

    /// <summary>
    /// 발전량 데이터를 처리하는 메서드
    /// </summary>
    private double ProcessPowerData(string jsonData)
    {
        try
        {
            var powerData = JsonUtility.FromJson<PowerDataInfoArray>("{\"powerDataInfo\":" + jsonData + "}");
            return powerData.powerDataInfo[0].dayGelec;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error processing power data: {e.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 차트 데이터를 업데이트하는 메서드
    /// </summary>
    private void UpdateChartData(int index, double value)
    {
        double roundedValue = Math.Round(value, 1);
        barChart.UpdateData(0, index, roundedValue);
    }
    #endregion
}