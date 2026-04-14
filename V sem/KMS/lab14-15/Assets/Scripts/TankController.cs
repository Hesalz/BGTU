using UnityEngine;

public class TankControl : MonoBehaviour
{
    public Transform turret;
    public Transform barrel;

    public float moveSpeed = 40f;
    public float turnSpeed = 90f;

    public float turretSpeed = 120f;
    public float barrelSpeed = 60f;
    public float minBarrelAngle = -25f;
    public float maxBarrelAngle = 10f;

    private float barrelAngle;

    private bool showControlPanel = true;
    private float controlPanelWidth = 700f;
    private float controlPanelHeight = 400f;

    public AudioClip shootSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
    }

    void Update()
    {
        MoveHull();
        RotateTurret();
        RotateBarrel();
        Shoot();
    }

    void MoveHull()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        transform.Translate(Vector3.up * v * moveSpeed * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.forward * h * turnSpeed * Time.deltaTime, Space.Self);
    }

    void RotateTurret()
    {
        if (!turret) return;

        float mouseX = Input.GetAxis("Mouse X");
        turret.Rotate(Vector3.forward * mouseX * turretSpeed * Time.deltaTime, Space.Self);
    }

    void RotateBarrel()
    {
        if (!barrel) return;

        float mouseY = Input.GetAxis("Mouse Y");
        barrelAngle -= mouseY * barrelSpeed * Time.deltaTime;
        barrelAngle = Mathf.Clamp(barrelAngle, minBarrelAngle, maxBarrelAngle);

        barrel.localRotation = Quaternion.Euler(0f, barrelAngle, 0f);
    }
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }
    }


    void OnGUI()
    {
        if (showControlPanel)
        {
            GUIStyle bigLabelStyle = new GUIStyle(GUI.skin.label);
            bigLabelStyle.fontSize = 40;
            bigLabelStyle.fontStyle = FontStyle.Bold;

            GUIStyle bigSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
            bigSliderStyle.fixedHeight = 60f;

            GUIStyle bigSliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
            bigSliderThumbStyle.fixedHeight = 60f;
            bigSliderThumbStyle.fixedWidth = 40f;

            GUIStyle bigButtonStyle = new GUIStyle(GUI.skin.button);
            bigButtonStyle.fontSize = 50;
            bigButtonStyle.fontStyle = FontStyle.Bold;

            GUI.Box(new Rect(10, 10, controlPanelWidth, controlPanelHeight), "");
            GUI.Label(new Rect(50, 50, 600, 80), "СКОРОСТЬ ТАНКА: " + moveSpeed.ToString("F0"), bigLabelStyle);

            moveSpeed = GUI.HorizontalSlider(
                new Rect(50, 150, 600, 60),
                moveSpeed,
                10f,
                100f,
                bigSliderStyle,
                bigSliderThumbStyle
            );

            if (GUI.Button(new Rect(50, 250, 600, 120), "СКРЫТЬ", bigButtonStyle))
            {
                showControlPanel = false;
            }
        }
        else
        {
            GUIStyle bigButtonStyle = new GUIStyle(GUI.skin.button);
            bigButtonStyle.fontSize = 60;
            bigButtonStyle.fontStyle = FontStyle.Bold;

            if (GUI.Button(new Rect(10, 10, 400, 200), "ПОКАЗАТЬ", bigButtonStyle))
            {
                showControlPanel = true;
            }
        }
    }
}
