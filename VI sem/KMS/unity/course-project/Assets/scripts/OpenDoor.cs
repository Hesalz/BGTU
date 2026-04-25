using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Animator anim;
    private bool isOpen = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetBool("OpenDoor", false);
    }

    public void OpenDoorFunction()
    {
        if (anim != null && !isOpen)
        {
            anim.SetBool("OpenDoor", true);
            isOpen = true;
        }
    }

    public void CloseDoorFunction()
    {
        if (anim != null && isOpen)
        {
            anim.SetBool("OpenDoor", false);
            isOpen = false;
        }
    }

    public void ToggleDoor()
    {
        if (isOpen)
            CloseDoorFunction();
        else
            OpenDoorFunction();
    }
}