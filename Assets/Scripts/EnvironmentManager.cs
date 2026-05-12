using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum Season { Winter, Spring, Summer, Autumn }
public enum TimeOfDay { Night, Morning, Day, Evening }
public enum WeatherCondition { Sunny, Cloudy, Precipitation }

public class EnvironmentManager : MonoBehaviour
{
    [Header("Current State")]
    public Season currentSeason = Season.Summer;
    public TimeOfDay currentTime = TimeOfDay.Day;
    public WeatherCondition currentWeather = WeatherCondition.Sunny;
    [Range(1, 5)] public int windForce = 1;
    public float windAngle = 0f;

    [Header("Visual References")]
    public Light directionalLight; 
    public Volume globalVolume;

    private ColorAdjustments colorAdjustments;
    private Bloom bloom;
    
    public static event Action<Season> OnSeasonChanged;
    public static event Action<TimeOfDay> OnTimeChanged;
    public static event Action<WeatherCondition> OnWeatherChanged;
    public static event Action<int, float> OnWindChanged;

    void Start()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out colorAdjustments);
            globalVolume.profile.TryGet(out bloom);
        }
        
        ApplySeasonVisuals(currentSeason);
        ApplyTimeVisuals(currentTime);
    }

    void Update()
    {
        HandleSeasonInput();
        HandleTimeInput();
        HandleWeatherInput();
        HandleWindInput();
    }

    void HandleSeasonInput() {
        if (Input.GetKey(KeyCode.R))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSeason(Season.Winter);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSeason(Season.Spring);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSeason(Season.Summer);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeSeason(Season.Autumn);
        }
    }
    void HandleTimeInput() {
        if (Input.GetKey(KeyCode.T))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeTime(TimeOfDay.Night);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeTime(TimeOfDay.Morning);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeTime(TimeOfDay.Day);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeTime(TimeOfDay.Evening);
        }
    }
    void HandleWeatherInput() {
        if (Input.GetKey(KeyCode.Y))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeather(WeatherCondition.Sunny);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeather(WeatherCondition.Cloudy);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeather(WeatherCondition.Precipitation);
        }
    }
    void HandleWindInput()
    {
        if (Input.GetKey(KeyCode.U))
        {
            for (int i = 1; i <= 5; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    windForce = i;
                    OnWindChanged?.Invoke(windForce, windAngle);
                    Debug.Log($"Ветер: Сила {windForce}");
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                windAngle = (windAngle - 45f) % 360f;
                OnWindChanged?.Invoke(windForce, windAngle);
                Debug.Log($"Ветер: Направление {windAngle}°");
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                windAngle = (windAngle + 45f) % 360f;
                OnWindChanged?.Invoke(windForce, windAngle);
                Debug.Log($"Ветер: Направление {windAngle}°");
            }
        }
    }

    void ChangeSeason(Season newSeason)
    {
        if (currentSeason == newSeason) return;
        currentSeason = newSeason;
        OnSeasonChanged?.Invoke(currentSeason);
        ApplySeasonVisuals(currentSeason);
        Debug.Log($"Сменили сезон на: {currentSeason}");
    }

    void ChangeTime(TimeOfDay newTime)
    {
        if (currentTime == newTime) return;
        currentTime = newTime;
        OnTimeChanged?.Invoke(currentTime);
        ApplyTimeVisuals(currentTime);
        Debug.Log($"Сменили время суток на: {currentTime}");
    }

    void ChangeWeather(WeatherCondition newWeather)
    {
        if (currentWeather == newWeather) return;
        currentWeather = newWeather;
        OnWeatherChanged?.Invoke(currentWeather);
    }

    void ApplySeasonVisuals(Season season)
    {
        if (colorAdjustments != null)
        {
            switch (season)
            {
                case Season.Summer:
                    colorAdjustments.colorFilter.value = Color.white;
                    break;
                case Season.Winter:
                    colorAdjustments.colorFilter.value = new Color(0.6f, 0.8f, 1f);
                    break;
                case Season.Autumn:
                    colorAdjustments.colorFilter.value = new Color(1f, 0.8f, 0.6f);
                    break;
                case Season.Spring:
                    colorAdjustments.colorFilter.value = new Color(0.8f, 1f, 0.8f);
                    break;
            }
        }
    }

    [Header("Time Colors")]
    public Color dayColor = Color.white;
    public Color morningColor = new Color(1f, 0.8f, 0.6f);
    public Color eveningColor = new Color(1f, 0.4f, 0.3f);
    public Color nightColor = new Color(0.1f, 0.15f, 0.3f);

    void ApplyTimeVisuals(TimeOfDay time)
    {
        float intensity = 1f;
        float exposure = 1f;
        Color targetColor = dayColor;
        float fogDensity = 0.01f;

        switch (time)
        {
            case TimeOfDay.Morning:
                intensity = 0.6f;
                exposure = 0.8f;
                targetColor = morningColor;
                fogDensity = 0.005f;
                break;
            case TimeOfDay.Day:
                intensity = 1.2f;
                exposure = 1.0f;
                targetColor = dayColor;
                fogDensity = 0.005f;
                break;
            case TimeOfDay.Evening:
                intensity = 0.5f;
                exposure = 0.7f;
                targetColor = eveningColor;
                fogDensity = 0.005f;
                break;
            case TimeOfDay.Night:
                intensity = 0.05f;
                exposure = 0.1f;
                targetColor = nightColor;
                fogDensity = 0.005f;
                break;
        }

        if (directionalLight != null)
        {
            directionalLight.intensity = intensity;
            directionalLight.color = targetColor;
        }

        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetFloat("_Exposure", exposure);
            RenderSettings.skybox.SetColor("_SkyTint", targetColor);
        }

        RenderSettings.fog = true;
        RenderSettings.fogColor = targetColor * 0.5f; // Туман чуть темнее неба
        RenderSettings.fogDensity = fogDensity;

        RenderSettings.ambientLight = targetColor * 0.2f;

        if (bloom != null)
        {
            bloom.intensity.value = (time == TimeOfDay.Night) ? 5f : 0.5f;
        }
    }
}