using UnityEngine;
using TMPro;

public class RightPanelButtons : MonoBehaviour
{
    public for_camera cameraController;
    public TextMeshProUGUI infoText;
    public VehicleEntryController vehicleEntryController;
    public TaskManager taskManager;

    [Header("3D Объекты для подсветки")]
    public GameObject lebedkaObject;
    public GameObject hookObject;
    public GameObject shassiObject;
    public GameObject migalkaObject;
    public GameObject platformaObject;
    public GameObject appareliObject;

    [Header("Позиции камеры для элементов")]
    public Vector3 lebedkaPos = new Vector3(591f, 11.48f, 485.54f);
    public Vector3 lebedkaRot = new Vector3(0f, 71.598f, 0f);
    public Vector3 shassiPos = new Vector3(585.3f, 7.7f, 498.1f);
    public Vector3 shassiRot = new Vector3(11.404f, -180f, 1.306f);
    public Vector3 migalkaPos = new Vector3(592.9f, 13.8f, 488.8f);
    public Vector3 migalkaRot = new Vector3(34.554f, 125.91f, 1.306f);
    public Vector3 platformaPos = new Vector3(572.4f, 14.6f, 486.9f);
    public Vector3 platformaRot = new Vector3(33.664f, 90f, 1.306f);
    public Vector3 appareliPos = new Vector3(574.35f, 6.07f, 485.36f);
    public Vector3 appareliRot = new Vector3(-35.418f, 71.781f, 3.026f);

    [Header("Тексты для информации")]
    public string lebedkaInfo = "Лебёдка - гидравлическое тяговое устройство с тросом и крюком для подтягивания автомобиля на платформу эвакуатора.";
    public string shassiInfo = "Шасси - колёсная база и рама эвакуатора.";
    public string migalkaInfo = "Мигалка - световой сигнальный маячок оранжевого цвета.";
    public string platformaInfo = "Платформа - грузовая площадка для перевозки эвакуируемого автомобиля.";
    public string appareliInfo = "Аппарели - выдвижные трапы для заезда автомобиля на платформу.";

    private GameObject currentlyHighlighted;

    private void ShowInfo(string info)
    {
        if (infoText != null)
            infoText.text = info;
    }

    private void HighlightObject(GameObject obj, string info)
    {
        if (currentlyHighlighted != null)
        {
            Podsvetka prev = currentlyHighlighted.GetComponent<Podsvetka>();
            if (prev != null) prev.Unhighlight();
        }

        ShowInfo(info);

        if (obj != null)
        {
            Podsvetka highlighter = obj.GetComponent<Podsvetka>();
            if (highlighter != null)
            {
                highlighter.Highlight();
                currentlyHighlighted = obj;
            }
        }
    }

    public void OnLebedkaClick()
    {
        HighlightObject(lebedkaObject != null ? lebedkaObject : hookObject, lebedkaInfo);
        cameraController.MoveToElement(lebedkaPos, Quaternion.Euler(lebedkaRot));
    }

    public void OnShassiClick()
    {
        HighlightObject(shassiObject, shassiInfo);
        cameraController.MoveToElement(shassiPos, Quaternion.Euler(shassiRot));
    }

    public void OnMigalkaClick()
    {
        HighlightObject(migalkaObject, migalkaInfo);
        cameraController.MoveToElement(migalkaPos, Quaternion.Euler(migalkaRot));
    }

    public void OnPlatformaClick()
    {
        HighlightObject(platformaObject, platformaInfo);
        cameraController.MoveToElement(platformaPos, Quaternion.Euler(platformaRot));
    }

    public void OnAppareliClick()
    {
        HighlightObject(appareliObject, appareliInfo);
        cameraController.MoveToElement(appareliPos, Quaternion.Euler(appareliRot));
    }

    public void OnPracticeClick()
    {
        if (currentlyHighlighted != null)
        {
            Podsvetka prev = currentlyHighlighted.GetComponent<Podsvetka>();
            if (prev != null) prev.Unhighlight();
            currentlyHighlighted = null;
        }

        if (vehicleEntryController != null)
            vehicleEntryController.StartPractice();

        if (taskManager != null)
            taskManager.StartPractice();

        ShowInfo("Режим практики. Управление персонажем: WASD. Подойдите к двери и нажмите E.");
        cameraController.StartPracticeMode();
    }
}