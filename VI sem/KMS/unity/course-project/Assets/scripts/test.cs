using UnityEngine;

public class FindColorProperty : MonoBehaviour
{
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null && rend.material != null)
        {
            Debug.Log($"=== Материал: {rend.material.name} ===");
            Debug.Log($"Shader: {rend.material.shader.name}");

            var shader = rend.material.shader;
            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                if (shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Color)
                {
                    Debug.Log($"Color property: {shader.GetPropertyName(i)}");
                }
            }
        }
    }
}