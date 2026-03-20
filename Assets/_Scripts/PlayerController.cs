using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidades por Escala")]
    public float microSpeed = 3f;
    public float normalSpeed = 6f;
    public float titanSpeed = 4f;

    [Header("Fuerza de Salto por Escala")]
    public float microJump = 5f;
    public float normalJump = 7f;
    public float titanJump = 10f; // El Titán pesa más, necesita más impulso

    [Header("Ajustes de Rotación")]
    public float turnSmoothTime = 0.1f;

    // Referencias
    private Rigidbody rb;
    private PlayerScaleController scaleController;
    private float turnSmoothVelocity;
    private Transform cam; 
    
    // Variables públicas para el Animator
    public float CurrentSpeed { get; private set; }
    public bool IsGrounded { get; private set; }

    // Evento/Delegado para avisarle al Animator que saltamos
    public System.Action OnJumpTriggered;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        scaleController = GetComponent<PlayerScaleController>();
        
        if (Camera.main != null) cam = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f && cam != null)
        {
            // Calculamos rotación basada en la cámara
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Movemos usando la VELOCIDAD en X y Z, pero RESPETAMOS la velocidad en Y (gravedad/salto)
            float activeSpeed = GetSpeedForCurrentSize();
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            rb.linearVelocity = new Vector3(moveDir.x * activeSpeed, rb.linearVelocity.y, moveDir.z * activeSpeed);
            
            CurrentSpeed = activeSpeed; 
        }
        else
        {
            // Si no hay input, frenamos en X y Z, pero mantenemos la Y (para caer bien)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            CurrentSpeed = 0f; 
        }
    }

    private void Jump()
    {
        float activeJumpForce = GetJumpForceForCurrentSize();
        
        // En lugar de usar AddForce, establecemos la velocidad directamente.
        // Esto IGNORA completamente la masa del Rigidbody. ¡Un salto perfecto y predecible!
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, activeJumpForce, rb.linearVelocity.z);

        // Avisamos al script de animaciones
        OnJumpTriggered?.Invoke();
    }

    float GetSpeedForCurrentSize()
    {
        if (!scaleController) return normalSpeed;
        return scaleController.currentSize switch
        {
            PlayerScaleController.PlayerSize.Micro => microSpeed,
            PlayerScaleController.PlayerSize.Normal => normalSpeed,
            PlayerScaleController.PlayerSize.Titan => titanSpeed,
            _ => normalSpeed
        };
    }

    float GetJumpForceForCurrentSize()
    {
        if (!scaleController) return normalJump;
        return scaleController.currentSize switch
        {
            PlayerScaleController.PlayerSize.Micro => microJump,
            PlayerScaleController.PlayerSize.Normal => normalJump,
            PlayerScaleController.PlayerSize.Titan => titanJump,
            _ => normalJump
        };
    }
   
    public void SetGroundedState(bool state)
    {
        IsGrounded = state;
    }
}