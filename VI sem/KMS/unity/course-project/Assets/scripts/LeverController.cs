using UnityEngine;

public class LeverController : MonoBehaviour
{
    [Header("Рычаг")]
    public Animator leverAnimator;

    [Header("Аппарели")]
    public Animator[] rampsAnimators;

    [Header("Настройки")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionRadius = 8f;
    public bool showPrompt = true;

    [Header("Задержки")]
    public float leverAnimationDelay = 0.5f;   
    public float rampsDelay = 0.5f;          

    [Header("Звуки")]
    public AudioSource audioSource;
    public AudioClip leverPullSound;  
    public AudioClip rampsMoveSound;  
    public float leverSoundVolume = 0.8f;
    public float rampsSoundVolume = 0.7f;
    public bool playSoundOnToggle = true;

    private bool isOpen = false;
    private bool isBusy = false;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (player == null)
            player = GameObject.Find("Player")?.transform;

        if (leverAnimator == null)
            leverAnimator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null && (leverPullSound != null || rampsMoveSound != null))
            audioSource = gameObject.AddComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.volume = 0.7f;
        }
    }

    void Update()
    {
        if (isBusy) return;

        bool isNear = Vector3.Distance(transform.position, player.position) < interactionRadius;

        if (isNear && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(OperateSequence());
        }
    }

    System.Collections.IEnumerator OperateSequence()
    {
        isBusy = true;

        isOpen = !isOpen;

        PlayLeverSound();

        yield return new WaitForSeconds(leverAnimationDelay);

        if (leverAnimator != null)
        {
            leverAnimator.SetBool("Open", isOpen);
        }

        yield return new WaitForSeconds(rampsDelay);

        PlayRampsSound();

        if (rampsAnimators != null)
        {
            foreach (Animator anim in rampsAnimators)
            {
                if (anim != null)
                {
                    anim.SetBool("Open", isOpen);
                }
            }
        }

        isBusy = false;
    }

    void PlayLeverSound()
    {
        if (!playSoundOnToggle) return;
        if (audioSource == null) return;
        if (leverPullSound == null)
        {
            return;
        }

        audioSource.PlayOneShot(leverPullSound, leverSoundVolume);
    }

    void PlayRampsSound()
    {
        if (!playSoundOnToggle) return;
        if (audioSource == null) return;
        if (rampsMoveSound == null) return;

        audioSource.PlayOneShot(rampsMoveSound, rampsSoundVolume);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    public bool AreRampsOpen()
    {
        return isOpen;
    }
}