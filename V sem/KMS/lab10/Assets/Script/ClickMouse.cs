using UnityEngine;
using UnityEngine.EventSystems;

public class ClickMouse : MonoBehaviour, IPointerClickHandler
{
    private int force = 100;

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeColor();
        ApplyForce(eventData);
    }

    private void ChangeColor()
    {
        float red = Random.Range(0f, 1f);
        float green = Random.Range(0f, 1f);
        float blue = Random.Range(0f, 1f);

        Color col1 = new Color(red, green, blue);
        gameObject.GetComponent<Renderer>().material.color = col1;
    }

    private void ApplyForce(PointerEventData eventData)
    {
        Vector3 target = eventData.pointerPressRaycast.worldPosition;
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 direction = target - cameraPos;
        Vector3 normalizedDirection = direction.normalized;
        Vector3 forceVector = normalizedDirection * force;

        gameObject.GetComponent<Rigidbody>().AddForceAtPosition(forceVector, target);
    }

    public void SetForce(int newForce)
    {
        force = newForce;
    }

    public void IncreaseForce(int increment = 50)
    {
        force += increment;
    }
}