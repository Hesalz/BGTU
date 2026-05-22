using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ResultTable : MonoBehaviour
{
    [Header("Значения таблицы (TextMeshPro)")]
    public TextMeshProUGUI[] flashModeValues;
    public TextMeshProUGUI[] avgSpeedValues;
    public TextMeshProUGUI[] loadingTimeValues;

    [Header("Кнопки и панель")]
    public GameObject tablePanel;
    public Button closeButton;

    [Header("Ссылки на системы")]
    public Flasher flasher;
    public WinchSystem winchSystem;
    public VehicleController vehicleController;

    [Header("Настройки сохранения")]
    public int maxMeasures = 3;
    public string saveFileName = "measures.json";

    private List<MeasureData> measures = new List<MeasureData>();
    private bool isRecording = false;
    private float attachTime = 0f;
    private float lockedTime = 0f;
    private float speedSum = 0f;
    private int speedSamples = 0;

    [System.Serializable]
    public class MeasureData
    {
        public string flashMode;
        public float avgSpeed;
        public float loadingTime;
        public string timestamp;
    }

    [System.Serializable]
    public class MeasuresList
    {
        public List<MeasureData> measures = new List<MeasureData>();
    }

    void Start()
    {
        LoadMeasures();

        if (winchSystem != null)
        {
            winchSystem.OnHookAttached += OnHookAttached;
            winchSystem.OnVehicleLocked += OnVehicleLocked;
        }

        UpdateTableDisplay();
    }

    void Update()
    {
        if (isRecording && vehicleController != null)
        {
            float speedKPH = Mathf.Abs(vehicleController.GetCurrentSpeed()) * 3.6f;
            speedSum += speedKPH;
            speedSamples++;
        }
    }

    void OnHookAttached()
    {
        if (measures.Count < maxMeasures && !isRecording)
        {
            StartNewMeasure();
        }

        if (isRecording && attachTime == 0f)
        {
            attachTime = Time.time;
            Debug.Log($"[Таблица] Захват крюка: {attachTime:F2} сек");
        }
    }

    void OnVehicleLocked()
    {
        if (isRecording && attachTime > 0f && lockedTime == 0f)
        {
            lockedTime = Time.time;
            RecordCurrentMeasure();
        }
    }

    public void StartNewMeasure()
    {
        if (measures.Count >= maxMeasures)
        {
            Debug.Log($"[Таблица] Достигнуто максимум {maxMeasures} замеров. Нажмите Очистка.");
            return;
        }

        isRecording = true;
        attachTime = 0f;
        lockedTime = 0f;
        speedSum = 0f;
        speedSamples = 0;

        Debug.Log($"[Таблица] Начат замер №{measures.Count + 1}");
    }

    void RecordCurrentMeasure()
    {
        if (!isRecording) return;

        MeasureData data = new MeasureData();
        data.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        if (flasher != null)
        {
            data.flashMode = flasher.currentMode == Flasher.FlashingMode.Strobe ? "Strobe" : "Running";
        }
        else
        {
            data.flashMode = "N/A";
        }

        if (speedSamples > 0)
        {
            data.avgSpeed = speedSum / speedSamples;
        }
        else
        {
            data.avgSpeed = 0f;
        }

        if (attachTime > 0f && lockedTime > 0f)
        {
            data.loadingTime = lockedTime - attachTime;
        }
        else
        {
            data.loadingTime = 0f;
        }

        measures.Add(data);
        SaveMeasures();
        UpdateTableDisplay();
        isRecording = false;

        Debug.Log($"[Таблица] Замер {measures.Count}: {data.flashMode}, {data.avgSpeed:F2} км/ч, {data.loadingTime:F2} сек");
    }

    void UpdateTableDisplay()
    {
        for (int i = 0; i < maxMeasures; i++)
        {
            if (i < measures.Count)
            {
                if (flashModeValues != null && flashModeValues.Length > i && flashModeValues[i] != null)
                    flashModeValues[i].text = measures[i].flashMode;

                if (avgSpeedValues != null && avgSpeedValues.Length > i && avgSpeedValues[i] != null)
                    avgSpeedValues[i].text = measures[i].avgSpeed.ToString("F2") + " км/ч";

                if (loadingTimeValues != null && loadingTimeValues.Length > i && loadingTimeValues[i] != null)
                    loadingTimeValues[i].text = measures[i].loadingTime.ToString("F2") + " сек";
            }
            else
            {
                if (flashModeValues != null && flashModeValues.Length > i && flashModeValues[i] != null)
                    flashModeValues[i].text = "---";
                if (avgSpeedValues != null && avgSpeedValues.Length > i && avgSpeedValues[i] != null)
                    avgSpeedValues[i].text = "---";
                if (loadingTimeValues != null && loadingTimeValues.Length > i && loadingTimeValues[i] != null)
                    loadingTimeValues[i].text = "---";
            }
        }
    }

    public void ShowTable()
    {
        if (tablePanel != null)
        {
            bool isActive = tablePanel.activeSelf;
            tablePanel.SetActive(!isActive);
        }
    }

    public void ClearTable()
    {
        measures.Clear();
        isRecording = false;
        attachTime = 0f;
        lockedTime = 0f;
        speedSum = 0f;
        speedSamples = 0;
        UpdateTableDisplay();

        try
        {
            if (File.Exists(GetSavePath()))
            {
                File.Delete(GetSavePath());
                Debug.Log("[Таблица] Файл сохранения удалён");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Таблица] Ошибка удаления файла: {e.Message}");
        }

        Debug.Log("[Таблица] Очищено");
    }


    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, saveFileName);
    }

    public void SaveMeasures()
    {
        try
        {
            MeasuresList wrapper = new MeasuresList();
            wrapper.measures = measures;

            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(GetSavePath(), json);

            Debug.Log($"[Таблица] Данные сохранены: {GetSavePath()}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Таблица] Ошибка сохранения: {e.Message}");
        }
    }

    public void LoadMeasures()
    {
        string path = GetSavePath();
        if (!File.Exists(path))
        {
            Debug.Log($"[Таблица] Файл сохранения не найден, создаём пустой список.");
            measures = new List<MeasureData>();
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            MeasuresList wrapper = JsonUtility.FromJson<MeasuresList>(json);

            if (wrapper != null && wrapper.measures != null)
            {
                measures = wrapper.measures;
                Debug.Log($"[Таблица] Загружено {measures.Count} замеров из {path}");
            }
            else
            {
                measures = new List<MeasureData>();
                Debug.LogWarning("[Таблица] Файл повреждён, создан пустой список.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Таблица] Ошибка загрузки: {e.Message}");
            measures = new List<MeasureData>();
        }
    }

    void OnDestroy()
    {
        if (winchSystem != null)
        {
            winchSystem.OnHookAttached -= OnHookAttached;
            winchSystem.OnVehicleLocked -= OnVehicleLocked;
        }
    }
}