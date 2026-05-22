using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Animator anim;
    private bool isOpen = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetBool("OpenDoor", false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void OpenDoorFunction()
    {
        if (anim != null && !isOpen)
        {
            anim.SetBool("OpenDoor", true);
            isOpen = true;
            PlaySound(doorOpenSound);
        }
    }

    public void CloseDoorFunction()
    {
        if (anim != null && isOpen)
        {
            anim.SetBool("OpenDoor", false);
            isOpen = false;
            PlaySound(doorCloseSound);
        }
    }

    public void ToggleDoor()
    {
        if (isOpen)
            CloseDoorFunction();
        else
            OpenDoorFunction();
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}