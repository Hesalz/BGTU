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

        if (leverAnimator != null)
        {
            leverAnimator.SetBool("Open", isOpen);
        }

        yield return new WaitForSeconds(0.5f);

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}