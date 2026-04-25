using UnityEngine;
using System.Collections;

public class Flasher : MonoBehaviour
{
    [Header("3 огонька на мигалке")]
    public Light[] lights;

    [Header("Настройки мигания")]
    public float flashSpeed = 0.25f;
    public float lightIntensity = 2f;
    public Color flashColor = new Color(1f, 0.5f, 0f);
    public FlashingMode currentMode = FlashingMode.Strobe;

    public enum FlashingMode
    {
        Strobe,
        RunningLight
    }

    [Header("Управление")]
    public KeyCode nextModeKey = KeyCode.N;
    public bool showModeInGUI = true;

    private bool isFlashing = false;
    private Coroutine flashCoroutine;

    void Start()
    {
        if (lights == null || lights.Length == 0)
        {
            lights = GetComponentsInChildren<Light>();
        }

        if (lights == null || lights.Length == 0)
        {
            return;
        }

        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.color = flashColor;
                light.intensity = 0;
                light.range = 3f;
            }
        }
    }

    void Update()
    {
        if (isFlashing && Input.GetKeyDown(nextModeKey))
        {
            NextMode();
        }
    }

    void NextMode()
    {
        if (currentMode == FlashingMode.Strobe)
            currentMode = FlashingMode.RunningLight;
        else
            currentMode = FlashingMode.Strobe;

        isFlashing = false;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        foreach (Light light in lights)
        {
            if (light != null) light.intensity = 0;
        }

        isFlashing = true;
        flashCoroutine = StartCoroutine(FlashRoutine());

        Debug.Log($"Режим: {(currentMode == FlashingMode.Strobe ? "Стробоскоп" : "Бегущий огонёк")}");
    }

    IEnumerator StrobeMode()
    {
        while (isFlashing)
        {
            foreach (Light light in lights)
            {
                if (light != null) light.intensity = lightIntensity;
            }
            yield return new WaitForSeconds(flashSpeed);

            foreach (Light light in lights)
            {
                if (light != null) light.intensity = 0;
            }
            yield return new WaitForSeconds(flashSpeed);
        }
    }

    IEnumerator RunningLightMode()
    {
        while (isFlashing)
        {
            if (lights.Length > 0 && lights[0] != null)
                lights[0].intensity = lightIntensity;
            yield return new WaitForSeconds(flashSpeed);
            if (lights.Length > 0 && lights[0] != null)
                lights[0].intensity = 0;

            yield return new WaitForSeconds(0.05f);

            if (lights.Length > 1 && lights[1] != null)
                lights[1].intensity = lightIntensity;
            yield return new WaitForSeconds(flashSpeed);
            if (lights.Length > 1 && lights[1] != null)
                lights[1].intensity = 0;

            yield return new WaitForSeconds(0.05f);

            if (lights.Length > 2 && lights[2] != null)
                lights[2].intensity = lightIntensity;
            yield return new WaitForSeconds(flashSpeed);
            if (lights.Length > 2 && lights[2] != null)
                lights[2].intensity = 0;

            yield return new WaitForSeconds(flashSpeed);
        }
    }

    IEnumerator FlashRoutine()
    {
        FlashingMode mode = currentMode;

        while (isFlashing)
        {
            if (mode == FlashingMode.Strobe)
                yield return StartCoroutine(StrobeMode());
            else
                yield return StartCoroutine(RunningLightMode());
        }
    }

    public void EnableFlasher()
    {
        if (isFlashing) return;

        isFlashing = true;
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    public void DisableFlasher()
    {
        if (!isFlashing) return;

        isFlashing = false;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.intensity = 0;
                light.color = flashColor;
            }
        }
    }

    public void ToggleFlasher()
    {
        if (isFlashing)
            DisableFlasher();
        else
            EnableFlasher();
    }
}