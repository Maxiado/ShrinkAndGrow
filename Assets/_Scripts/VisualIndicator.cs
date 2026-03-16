using UnityEngine;

public class VisualIndicator : MonoBehaviour
{
    public Material activeMaterial;   // El verde
    public Material inactiveMaterial; // El rojo
    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        SetState(false); // Empezamos en rojo
    }

    public void SetState(bool isActive)
    {
        meshRenderer.material = isActive ? activeMaterial : inactiveMaterial;
    }
}