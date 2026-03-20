using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator; // Arrastra el modelo 3D que tiene el Animator aquí
    
    private PlayerMovement movement;
    private PlayerScaleController scaleController;

    void Start()
    {
        // Buscamos los componentes en el objeto principal (la cápsula)
        movement = GetComponentInParent<PlayerMovement>();
        scaleController = GetComponentInParent<PlayerScaleController>();
    }

    void Update()
    {
        if (animator == null || movement == null || scaleController == null) return;

        // 1. Enviar la velocidad actual (Para pasar de Idle a Walk)
        animator.SetFloat("Speed", movement.CurrentSpeed);

        // 2. Enviar el Modo de Tamaño
        // Casteamos el Enum (Micro=0, Normal=1, Titan=2) a un número entero (Int)
        int sizeModeInt = (int)scaleController.currentSize;
        animator.SetInteger("SizeMode", sizeModeInt);
    }
}