using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// OpenWeatherMap API를 통해 날씨 정보를 가져와 표시하는 클래스
/// </summary>
public class WeatherAPI : MonoBehaviour
{
    #region Constants
    private const string API_KEY = "9de2d70f99200d51e41cdc48f150976e";
    private const string BASE_URL = "https://api.openweathermap.org/data/2.5/weather";
    private const string ICONS_PATH = "Icons/";
    private const float KELVIN_TO_CELSIUS = 273.15f;
    #endregion

    #region Inspector Fields
    [Header("UI References")]
    [SerializeField] private Image weatherIcon;
    [SerializeField] private Text temperatureDisplay;

    [Header("Location Settings")]
    [SerializeField] private float latitude = 37.5833f;
    [SerializeField] private float longitude = 127f;
    [SerializeField] private string cityName = "Seoul";

    [Header("Update Settings")]
    [SerializeField] private float updateInterval = 300f; // 5 minutes
    #endregion

    #region Weather Conditions
    private static readonly WeatherCondition[] CONDITIONS = {
        new WeatherCondition(200, 299, "thunder", "Thunderstorm"),
        new WeatherCondition(300, 399, "drizzle", "Drizzle"),
        new WeatherCondition(500, 599, "Rain", "Rain"),
        new WeatherCondition(600, 699, "snow", "Snow"),
        new WeatherCondition(700, 799, "fog", "Fog"),
        new WeatherCondition(800, 800, "sun", "Clear"),
        new WeatherCondition(801, 899, "cloud", "Clouds")
    };
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        ValidateComponents();
        StartCoroutine(UpdateWeatherRoutine());
    }
    #endregion

    #region Weather Update Methods
    /// <summary>
    /// 주기적으로 날씨 정보를 업데이트하는 코루틴
    /// </summary>
    private IEnumerator UpdateWeatherRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(FetchWeatherData());
            yield return new WaitForSeconds(updateInterval);
        }
    }

    /// <summary>
    /// OpenWeatherMap API에서 날씨 데이터를 가져오는 코루틴
    /// </summary>
    private IEnumerator FetchWeatherData()
    {
        string url = $"{BASE_URL}?lat={latitude}&lon={longitude}&appid={API_KEY}";

        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Weather API Error: {request.error}");
            yield break;
        }

        ProcessWeatherResponse(request.downloadHandler.text);
    }

    /// <summary>
    /// API 응답을 처리하고 UI를 업데이트
    /// </summary>
    private void ProcessWeatherResponse(string jsonResponse)
    {
        try
        {
            var weatherData = JsonUtility.FromJson<WeatherData>(jsonResponse);
            UpdateWeatherUI(weatherData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to process weather data: {e.Message}");
        }
    }

    /// <summary>
    /// 날씨 데이터로 UI 업데이트
    /// </summary>
    private void UpdateWeatherUI(WeatherData data)
    {
        if (data == null) return;

        // 온도 업데이트
        float celsius = data.main.temp - KELVIN_TO_CELSIUS;
        temperatureDisplay.text = $"{cityName}: {celsius:F0}°C";

        // 날씨 아이콘 업데이트
        UpdateWeatherIcon(data.weather[0].id);
    }

    /// <summary>
    /// 날씨 ID에 따른 아이콘 업데이트
    /// </summary>
    private void UpdateWeatherIcon(int weatherId)
    {
        string iconName = GetWeatherIconName(weatherId);
        Sprite weatherSprite = Resources.Load<Sprite>($"{ICONS_PATH}{iconName}");

        if (weatherSprite != null)
        {
            weatherIcon.sprite = weatherSprite;
        }
        else
        {
            Debug.LogWarning($"Weather icon not found: {iconName}");
        }
    }

    /// <summary>
    /// 날씨 ID에 해당하는 아이콘 이름 반환
    /// </summary>
    private string GetWeatherIconName(int weatherId)
    {
        foreach (var condition in CONDITIONS)
        {
            if (condition.IsMatch(weatherId))
            {
                return condition.IconName;
            }
        }
        return "sun"; // 기본 아이콘
    }

    /// <summary>
    /// 필수 컴포넌트 유효성 검사
    /// </summary>
    private void ValidateComponents()
    {
        if (weatherIcon == null || temperatureDisplay == null)
        {
            Debug.LogError("Required UI components are missing!");
            enabled = false;
        }
    }
    #endregion

    #region Data Classes
    [Serializable]
    private class WeatherData
    {
        public Main main;
        public Weather[] weather;
    }

    [Serializable]
    private class Main
    {
        public float temp;
    }

    [Serializable]
    private class Weather
    {
        public int id;
    }

    private class WeatherCondition
    {
        public int MinId { get; }
        public int MaxId { get; }
        public string IconName { get; }
        public string Description { get; }

        public WeatherCondition(int minId, int maxId, string iconName, string description)
        {
            MinId = minId;
            MaxId = maxId;
            IconName = iconName;
            Description = description;
        }

        public bool IsMatch(int weatherId)
        {
            return weatherId >= MinId && weatherId <= MaxId;
        }
    }
    #endregion
}