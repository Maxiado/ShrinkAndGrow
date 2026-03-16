using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float baseSpeed = 7f;
    public float rotationSpeed = 10f;
    public float jumpForce = 6f;

    [Header("Multiplicadores de Escala")]
    public float smallSpeedMultiplier = 1.2f;
    public float bigSpeedMultiplier = 0.6f;

    private Rigidbody rb;
    private Transform mainCam; // Referencia a la cámara
    private Vector3 moveInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Buscamos la cámara principal automáticamente
        if (Camera.main != null) mainCam = Camera.main.transform;
    }

    void Update()
    {
        // 1. Obtener Input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, 0, moveZ).normalized;

        // 2. Salto (Corregido: Solo Espacio)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (moveInput.magnitude >= 0.1f && mainCam != null)
        {
            // --- CÁLCULO RELATIVO A LA CÁMARA ---
            // Obtenemos la dirección de la cámara pero ignoramos su inclinación arriba/abajo (Y)
            Vector3 camForward = mainCam.forward;
            Vector3 camRight = mainCam.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // Creamos la dirección de movimiento final
            Vector3 relativeMove = (camForward * moveInput.z) + (camRight * moveInput.x);

            // Ajustar velocidad según escala
            float currentSpeed = baseSpeed;
            if (transform.localScale.x > 1f) currentSpeed *= bigSpeedMultiplier;
            else currentSpeed *= smallSpeedMultiplier;

            // Aplicar velocidad
            Vector3 targetVelocity = relativeMove * currentSpeed;
            targetVelocity.y = rb.linearVelocity.y; // Respetamos la gravedad
            rb.linearVelocity = targetVelocity;

            // Rotar al personaje hacia la dirección del movimiento
            Quaternion targetRotation = Quaternion.LookRotation(relativeMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Frenado suave si no hay input
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void Jump()
    {
        // ForceMode.VelocityChange para que el salto sea consistente sin importar la masa
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    // Detector de suelo
    void OnCollisionStay(Collision collision)
    {
        // Verificamos que el choque sea por debajo
        if (collision.contacts[0].normal.y > 0.5f) isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}