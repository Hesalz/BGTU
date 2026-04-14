using UnityEngine;

public class RotateWheel : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Trigger");
        }
    }
}