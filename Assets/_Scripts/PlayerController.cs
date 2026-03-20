using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidades por Escala")]
    public float microSpeed = 3f;
    public float normalSpeed = 6f;
    public float titanSpeed = 4f; // El Titán suele ser más lento pero imparable

    [Header("Ajustes de Rotación")]
    public float turnSmoothTime = 0.1f;
    
    // Referencias
    private Rigidbody rb;
    private PlayerScaleController scaleController;
    private float turnSmoothVelocity;
    
    // Variable pública para que el Animator sepa a qué velocidad vamos
    public float CurrentSpeed { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        scaleController = GetComponent<PlayerScaleController>();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // 1. Leer los inputs de WASD o Flechas
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // 2. Calcular hacia dónde debe mirar el modelo
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // 3. Obtener la velocidad correcta según la escala actual
            float activeSpeed = GetSpeedForCurrentSize();

            // 4. Mover el Rigidbody
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.MovePosition(rb.position + moveDir.normalized * activeSpeed * Time.fixedDeltaTime);
            
            CurrentSpeed = activeSpeed; // Le pasamos la velocidad real al Animator
        }
        else
        {
            CurrentSpeed = 0f; // Estamos quietos
        }
    }

    float GetSpeedForCurrentSize()
    {
        // Consultamos al PlayerScaleController en qué estado estamos
        if (scaleController == null) return normalSpeed;

        return scaleController.currentSize switch
        {
            PlayerScaleController.PlayerSize.Micro => microSpeed,
            PlayerScaleController.PlayerSize.Normal => normalSpeed,
            PlayerScaleController.PlayerSize.Titan => titanSpeed,
            _ => normalSpeed
        };
    }
}