using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// OpenWeatherMap API�� ���� ���� ������ ������ ǥ���ϴ� Ŭ����
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
    /// �ֱ������� ���� ������ ������Ʈ�ϴ� �ڷ�ƾ
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
    /// OpenWeatherMap API���� ���� �����͸� �������� �ڷ�ƾ
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
    /// API ������ ó���ϰ� UI�� ������Ʈ
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
    /// ���� �����ͷ� UI ������Ʈ
    /// </summary>
    private void UpdateWeatherUI(WeatherData data)
    {
        if (data == null) return;

        // �µ� ������Ʈ
        float celsius = data.main.temp - KELVIN_TO_CELSIUS;
        temperatureDisplay.text = $"{cityName}: {celsius:F0}��C";

        // ���� ������ ������Ʈ
        UpdateWeatherIcon(data.weather[0].id);
    }

    /// <summary>
    /// ���� ID�� ���� ������ ������Ʈ
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
    /// ���� ID�� �ش��ϴ� ������ �̸� ��ȯ
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
        return "sun"; // �⺻ ������
    }

    /// <summary>
    /// �ʼ� ������Ʈ ��ȿ�� �˻�
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