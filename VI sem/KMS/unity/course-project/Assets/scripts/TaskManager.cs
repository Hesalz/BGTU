using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI infoText;
    public GameObject taskPanel;
    public float taskDelaySeconds = 1f;

    [Header("References")]
    public WinchSystem winchSystem;
    public VehicleEntryController vehicleEntryController;
    public VehicleController vehicleController;
    public Flasher flasher;
    public LeverController leverController;

    [Header("Settings")]
    public float raycastDistance = 5f;
    public LayerMask vehicleLayer;

    private Dictionary<int, bool> tasksCompleted = new Dictionary<int, bool>();
    private int currentTask = 1;
    private bool isPracticeMode = false;
    private bool leverUsedForTask4 = false;
    private bool leverUsedForTask7 = false;

    void Start()
    {
        for (int i = 1; i <= 7; i++)
            tasksCompleted[i] = false;

        if (taskPanel != null)
            taskPanel.SetActive(false);
    }


    void Update()
    {
        if (!isPracticeMode) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursor();
        }

        switch (currentTask)
        {
            case 1: CheckTask1(); break;
            case 2: CheckTask2(); break;
            case 3: CheckTask3(); break;
            case 4: CheckTask4(); break;
            case 5: CheckTask5(); break;
            case 6: CheckTask6(); break;
            case 7: CheckTask7(); break;
        }
    }

    private bool isCursorLocked = true;

    void ToggleCursor()
    {
        isCursorLocked = !isCursorLocked;

        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void ShowInfo(string message)
    {
        if (infoText != null)
            infoText.text = message;
    }

    void CompleteTask(int task)
    {
        if (tasksCompleted[task]) return;

        tasksCompleted[task] = true;
        ShowInfo($"Задание {task} выполнено!");

        if (task == currentTask && currentTask < 7)
        {
            StartCoroutine(ShowNextTaskWithDelay());
        }

        if (AllTasksCompleted())
            ShowInfo("Все задания успешно выполнены!");
    }

    System.Collections.IEnumerator ShowNextTaskWithDelay()
    {
        yield return new WaitForSeconds(taskDelaySeconds);

        currentTask++;
        ShowCurrentTaskHint();
    }

    void ShowCurrentTaskHint()
    {
        switch (currentTask)
        {
            case 1:
                ShowInfo("Задание 1: Откройте дверь (E) → Сядьте в машину → Закройте дверь (E)");
                break;
            case 2:
                ShowInfo("Задание 2: Включите мигалку (F) → При необходимости смените режим (N)");
                break;
            case 3:
                ShowInfo("Задание 3: Найдите неправильно припаркованный автомобиль и подойдите к нему");
                break;
            case 4:
                ShowInfo("Задание 4: Опустите аппарели с помощью рычага с левой стороны платформы (E)");
                break;
            case 5:
                ShowInfo("Задание 5: Возьмите крюк (ЛКМ) → Прицепите к бамперу машины");
                break;
            case 6:
                ShowInfo("Задание 6: Управляйте лебёдкой (R), чтобы затянуть машину на платформу");
                break;
            case 7:
                ShowInfo("Задание 7: Поднимите аппарели с помощью рычага с левой стороны платформы (E) → Сядьте в машину (E)");
                break;
        }
    }

    bool AllTasksCompleted()
    {
        for (int i = 1; i <= 7; i++)
            if (!tasksCompleted[i]) return false;
        return true;
    }

    void CheckTask1()
    {
        if (vehicleEntryController != null &&
            vehicleEntryController.IsInsideVehicle() &&
            vehicleEntryController.IsDoorClosed())
        {
            CompleteTask(1);
        }
    }

    void CheckTask2()
    {
        if (flasher != null && flasher.IsFlashing() && flasher.currentMode != Flasher.FlashingMode.Strobe)
        {
            CompleteTask(2);
        }
    }

    void CheckTask3()
    {
        Camera currentCamera = vehicleEntryController.playerCamera;

        Ray ray = currentCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            Debug.Log($"Hit: {hit.collider.gameObject.name}, Tag: {hit.collider.tag}");

            if (hit.collider.CompareTag("Towable"))
            {
                CompleteTask(3);
            }
        }
    }

    void CheckTask4()
    {
        if (vehicleEntryController != null && !vehicleEntryController.IsInsideVehicle())
        {
            if (!leverUsedForTask4 && leverController != null && leverController.AreRampsOpen())
            {
                leverUsedForTask4 = true;
                CompleteTask(4);
            }
        }
    }

    void CheckTask5()
    {
        if (winchSystem != null && winchSystem.IsHookAttached())
        {
            CompleteTask(5);
        }
    }

    void CheckTask6()
    {
        if (winchSystem != null && winchSystem.IsVehicleOnPlatform())
        {
            CompleteTask(6);
        }
    }

    void CheckTask7()
    {
        if (winchSystem != null && winchSystem.IsVehicleLocked())
        {
            if (!leverUsedForTask7 && leverController != null && !leverController.AreRampsOpen())
            {
                leverUsedForTask7 = true;
            }

            if (leverUsedForTask7 && vehicleEntryController != null && vehicleEntryController.IsInsideVehicle())
            {
                CompleteTask(7);
            }
        }
    }

    public void StartPractice()
    {
        isPracticeMode = true;

        if (taskPanel != null)
            taskPanel.SetActive(true);

        ShowCurrentTaskHint();
    }

    public void OnTaskClick(int taskNumber)
    {
        string[] descriptions = {
            "",
            "Задание 1: Откройте дверь (E) → Сядьте в машину → Закройте дверь (E)",
            "Задание 2: Включите мигалку (F) → Смените режим (N)",
            "Задание 3: Найдите и идентифицируйте неправильно припаркованный автомобиль (E)",
            "Задание 4: Выйдите из машины → Опустите аппарели (рычаг, E)",
            "Задание 5: Возьмите крюк (ЛКМ) → Прицепите к бамперу машины (E)",
            "Задание 6: Управляйте лебёдкой (R), чтобы затянуть машину на платформу",
            "Задание 7: Поднимите аппарели (рычаг, E) → Сядьте в машину (E)"
        };

        if (taskNumber >= 1 && taskNumber <= 7)
            ShowInfo(descriptions[taskNumber]);
    }

    public void ExitPractice()
    {
        isPracticeMode = false;
        currentTask = 1;
        leverUsedForTask4 = false;
        leverUsedForTask7 = false;

        for (int i = 1; i <= 7; i++)
            tasksCompleted[i] = false;

        if (taskPanel != null)
            taskPanel.SetActive(false);

        if (infoText != null)
            infoText.text = "";
    }
}