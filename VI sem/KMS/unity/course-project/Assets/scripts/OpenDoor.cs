using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("OpenDoor", false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetBool("OpenDoor", true);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetBool("OpenDoor", false);
        }
    }
}