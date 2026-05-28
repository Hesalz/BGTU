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

    [Header("Звук")]
    public AudioSource audioSource;
    public AudioClip flashSound;
    public float soundVolume = 0.7f;

    private bool isFlashing = false;
    private Coroutine flashCoroutine;

    void Start()
    {
        if (lights == null || lights.Length == 0)
        {
            lights = GetComponentsInChildren<Light>();
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null && flashSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.volume = soundVolume;
            audioSource.loop = true;
            audioSource.clip = flashSound;
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

        if (audioSource != null && flashSound != null)
        {
            audioSource.Play();
        }
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
            }
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void ToggleFlasher()
    {
        if (isFlashing)
            DisableFlasher();
        else
            EnableFlasher();
    }

    public bool IsFlashing()
    {
        return isFlashing;
    }
}