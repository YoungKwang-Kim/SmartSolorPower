using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 태양광 발전소의 전력 데이터를 관리하고 업데이트하는 클래스
/// </summary>
public class PowerManager : MonoBehaviour
{
    #region UI References
    [Header("UI Elements")]
    [SerializeField] private Text dateText;    // 현재 날짜 표시 Text
    [SerializeField] private Text timeText;    // 현재 시간 표시 Text
    #endregion

    #region Private Fields
    private JsonParsing powerData;             // 발전소 데이터 파싱 객체
    private string lastUpdateTime;             // 마지막 업데이트 시간
    private const string TIME_SUFFIX = "_50";  // 시간 폴더 접미사
    private const string DATE_SUFFIX = "_REMS"; // 날짜 폴더 접미사
    #endregion

    #region Unity Lifecycle Methods
    /// <summary>
    /// 컴포넌트 초기화 시 호출되는 메서드
    /// </summary>
    private void Start()
    {
        InitializePowerData();
    }

    /// <summary>
    /// 매 프레임마다 호출되는 메서드
    /// 시간이 변경되었을 때만 데이터를 업데이트
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
    /// 전력 데이터 초기 설정을 수행하는 메서드
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
    /// 시간 변경 여부를 확인하는 메서드
    /// </summary>
    /// <returns>시간이 변경되었으면 true, 아니면 false</returns>
    private bool HasTimeChanged()
    {
        return lastUpdateTime != timeText.text;
    }

    /// <summary>
    /// 전력 데이터를 업데이트하는 메서드
    /// </summary>
    private void UpdatePowerData()
    {
        UpdateFolderPaths();
        StartCoroutine(FetchPowerData());
        lastUpdateTime = timeText.text;
    }

    /// <summary>
    /// 날짜와 시간에 따른 폴더 경로를 업데이트하는 메서드
    /// </summary>
    private void UpdateFolderPaths()
    {
        string currentDate = dateText.text;
        string currentTime = ParseTimeFromText();

        powerData.dateFileName = $"{currentDate}{DATE_SUFFIX}";
        powerData.timeFileName = $"{currentTime}{TIME_SUFFIX}";
    }

    /// <summary>
    /// 시간 텍스트에서 시간 값을 파싱하는 메서드
    /// </summary>
    /// <returns>파싱된 시간 문자열</returns>
    private string ParseTimeFromText()
    {
        string[] timeComponents = timeText.text.Split(':');
        return timeComponents[0];
    }

    /// <summary>
    /// 전력 데이터를 비동기적으로 가져오는 코루틴
    /// </summary>
    private IEnumerator FetchPowerData()
    {
        yield return StartCoroutine(powerData.GetChargeInfo());
    }
    #endregion
}