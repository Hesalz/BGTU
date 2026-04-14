using UnityEngine;
using System.Collections;

public class Bot : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotSpeedTank = 90f;
    public float rotSpeedTurret = 120f;

    public Transform bash;
    public Transform stvol;
    public GameObject core;
    public AudioClip shootSound;

    public bool canshoot = true;
    public float life = 3f;

    private AudioSource audioSource;
    RaycastHit hit;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player") return;

        Vector3 dir = other.transform.position - transform.position;
        float distance = dir.magnitude;

        float turretInput = Vector3.SignedAngle(bash.up, dir, bash.forward);
        bash.Rotate(Vector3.forward * Mathf.Clamp(turretInput, -rotSpeedTurret * Time.deltaTime, rotSpeedTurret * Time.deltaTime), Space.Self);

        if (Physics.Raycast(bash.position, bash.up, out hit))
        {
            if (hit.transform.tag == "Player" && canshoot)
            {
                StartCoroutine(botshoot());
            }
        }

        if (distance < 150f)
        {
            float bodyInput = Vector3.SignedAngle(transform.up, dir, transform.forward);
            transform.Rotate(Vector3.forward * Mathf.Clamp(bodyInput, -rotSpeedTank * Time.deltaTime, rotSpeedTank * Time.deltaTime), Space.Self);
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        }
    }

    IEnumerator botshoot()
    {
        canshoot = false;

        Vector3 spawnPos = stvol.position + stvol.TransformDirection(Vector3.up * 4f);
        Instantiate(core, spawnPos, stvol.rotation);

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        yield return new WaitForSeconds(3f);
        canshoot = true;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "core")
        {
            if (--life < 1)
                Destroy(gameObject);
        }
    }
}
