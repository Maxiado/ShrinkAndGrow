using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator; 
    
    private PlayerMovement movement;
    private PlayerScaleController scaleController;

    void Start()
    {
        movement = GetComponentInParent<PlayerMovement>();
        scaleController = GetComponentInParent<PlayerScaleController>();

        // Nos suscribimos al evento de salto del script de físicas
        if (movement)
        {
            movement.OnJumpTriggered += TriggerJumpAnimation;
        }
    }

    void Update()
    {
        if (!animator || !movement || !scaleController) return;

        animator.SetFloat("Speed", movement.CurrentSpeed);
        animator.SetInteger("SizeMode", (int)scaleController.currentSize);
        
        // ¡NUEVO! Le decimos al Animator si estamos tocando el suelo o cayendo
        animator.SetBool("IsGrounded", movement.IsGrounded);
    }

    private void TriggerJumpAnimation()
    {
        // Disparamos el Trigger de salto en el Animator
        if (animator) animator.SetTrigger("Jump");
    }

    private void OnDestroy()
    {
        // Buena práctica: desuscribirse del evento al destruir el objeto
        if (movement) movement.OnJumpTriggered -= TriggerJumpAnimation;
    }
}