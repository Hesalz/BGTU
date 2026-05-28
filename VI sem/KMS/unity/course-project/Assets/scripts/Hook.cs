using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header("References")]
    public WinchSystem winchSystem;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip attachSound;
    public float soundVolume = 0.8f;
    public bool playSoundOnAttach = true;

    private bool hasAttached = false;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null && attachSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.volume = soundVolume;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.transform.root.GetComponent<Rigidbody>();

        if (rb == null) return;
        if (!rb.CompareTag("Towable")) return;

        TowPoint point = other.transform.root.GetComponentInChildren<TowPoint>();

        if (point == null)
        {
            return;
        }

        if (winchSystem.IsHookAttached()) return;

        winchSystem.AttachToVehicle(rb, point.transform);

        if (playSoundOnAttach && !hasAttached)
        {
            PlayAttachSound();
            hasAttached = true;
        }
    }

    private void PlayAttachSound()
    {
        if (audioSource == null) return;
        if (attachSound == null)
        {
            Debug.LogWarning("Attach sound not assigned!");
            return;
        }

        audioSource.PlayOneShot(attachSound, soundVolume);
    }

    public void ResetAttachState()
    {
        hasAttached = false;
    }
}