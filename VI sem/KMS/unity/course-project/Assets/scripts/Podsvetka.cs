using UnityEngine;

public class Podsvetka : MonoBehaviour
{
    public Material glowMaterial;
    private Material[] originalMaterials;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterials = rend.materials;
        }
    }

    public void Highlight()
    {
        if (rend != null && glowMaterial != null)
        {
            Material[] highlightMaterials = new Material[originalMaterials.Length];
            for (int i = 0; i < highlightMaterials.Length; i++)
            {
                highlightMaterials[i] = glowMaterial;
            }
            rend.materials = highlightMaterials;
        }
    }

    public void Unhighlight()
    {
        if (rend != null && originalMaterials != null)
        {
            rend.materials = originalMaterials;
        }
    }
}