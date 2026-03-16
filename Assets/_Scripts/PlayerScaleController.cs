using UnityEngine;
using Cinemachine;

public class PlayerScaleController : MonoBehaviour
{
    public enum PlayerSize { Micro, Normal, Titan }

    [Header("Estado Actual")]
    public PlayerSize currentSize = PlayerSize.Normal;

    [Header("Teclas de Control")]
    public KeyCode microKey = KeyCode.Q;
    public KeyCode titanKey = KeyCode.E;

    [Header("Configuración de Escalas")]
    public Vector3 microScale = new Vector3(0.2f, 0.2f, 0.2f);
    public Vector3 normalScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 titanScale = new Vector3(2.5f, 2.5f, 2.5f);
    
    [Header("Física y Peso")]
    public float microMass = 0.1f;
    public float normalMass = 1.0f;
    public float titanMass = 20.0f;

    [Header("Cámara (Cinemachine)")]
    public CinemachineVirtualCamera vCam;
    public float microCamDistance = 3f;
    public float normalCamDistance = 6f;
    public float titanCamDistance = 15f;

    [Header("Ajustes Globales")]
    public float scaleSpeed = 5f;
    public LayerMask obstacleLayer;
    public float checkRadius = 0.8f;

    private Vector3 targetScale;
    private float targetCamDistance;
    private CinemachineFramingTransposer framingTransposer;
    private Rigidbody rb;

    // Propiedades para comunicación con otros scripts
    public bool IsTitan => currentSize == PlayerSize.Titan;
    public bool IsMicro => currentSize == PlayerSize.Micro;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        framingTransposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        SetSize(PlayerSize.Normal, true); 
    }

    void Update()
    {
        HandleInput();

        // Aplicar transformaciones suaves
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        if (framingTransposer)
        {
            framingTransposer.m_CameraDistance = Mathf.Lerp(framingTransposer.m_CameraDistance, targetCamDistance, Time.deltaTime * scaleSpeed);
        }

        // Sistema de seguridad por caída
        if (transform.position.y < -10) GameManager.Instance.RestartLevel();
    }

    void HandleInput()
    {
        // Lógica para tecla TITÁN (E)
        if (Input.GetKeyDown(titanKey))
        {
            if (currentSize == PlayerSize.Titan) 
                AttemptChangeSize(PlayerSize.Normal); // Toggle a Normal
            else 
                AttemptChangeSize(PlayerSize.Titan);  // De Micro o Normal a Titan
        }

        // Lógica para tecla MICRO (Q)
        if (Input.GetKeyDown(microKey))
        {
            if (currentSize == PlayerSize.Micro) 
                AttemptChangeSize(PlayerSize.Normal); // Toggle a Normal
            else 
                AttemptChangeSize(PlayerSize.Micro);  // De Titan o Normal a Micro
        }
    }

    void AttemptChangeSize(PlayerSize newSize)
    {
        // Si vamos a una escala mayor (Micro -> Normal o Normal/Micro -> Titan), checkeamos espacio
        float targetHeight = GetHeightForSize(newSize);
        
        if (newSize > currentSize) // Solo checkeamos si crecemos
        {
            if (!CanGrow(targetHeight)) 
            {
                Debug.LogWarning("No hay espacio para crecer");
                return;
            }
        }

        SetSize(newSize);
    }

    float GetHeightForSize(PlayerSize size)
    {
        return size switch
        {
            PlayerSize.Micro => microScale.y,
            PlayerSize.Normal => normalScale.y,
            PlayerSize.Titan => titanScale.y,
            _ => 1f
        };
    }

    void SetSize(PlayerSize newSize, bool instant = false)
    {
        currentSize = newSize;

        switch (currentSize)
        {
            case PlayerSize.Micro:
                targetScale = microScale;
                targetCamDistance = microCamDistance;
                rb.mass = microMass;
                break;
            case PlayerSize.Normal:
                targetScale = normalScale;
                targetCamDistance = normalCamDistance;
                rb.mass = normalMass;
                break;
            case PlayerSize.Titan:
                targetScale = titanScale;
                targetCamDistance = titanCamDistance;
                rb.mass = titanMass;
                break;
        }

        if (instant)
        {
            transform.localScale = targetScale;
            if (framingTransposer) framingTransposer.m_CameraDistance = targetCamDistance;
        }
    }

    bool CanGrow(float targetHeight)
    {
        float rayLength = targetHeight;
        // Checkeamos una esfera en la cabeza del tamaño objetivo
        return !Physics.CheckSphere(transform.position + Vector3.up * rayLength, checkRadius, obstacleLayer);
    }
}