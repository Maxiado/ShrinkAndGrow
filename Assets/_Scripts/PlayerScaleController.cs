using _Scripts.Progress;
using UnityEngine;
using Cinemachine;
using Materials.VFX.ShadersGraph.Floor;

public class PlayerScaleController : MonoBehaviour
{
    public enum PlayerSize
    {
        Micro,
        Normal,
        Titan
    }

    [Header("Estado Actual")] public PlayerSize currentSize = PlayerSize.Normal;

    [Header("Teclas de Control")] public KeyCode microKey = KeyCode.Q;
    public KeyCode titanKey = KeyCode.E;

    [Header("Configuración de Escalas")] public Vector3 microScale = new Vector3(0.2f, 0.2f, 0.2f);
    public Vector3 normalScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 titanScale = new Vector3(2.5f, 2.5f, 2.5f);

    [Header("Física y Peso")] public float microMass = 0.1f;
    public float normalMass = 1.0f;
    public float titanMass = 20.0f;

    [Header("Cámara (Cinemachine)")] public CinemachineVirtualCamera vCam;
    public float microCamDistance = 3f;
    public float normalCamDistance = 6f;
    public float titanCamDistance = 15f;

    [Header("Ajustes Globales")] public float scaleSpeed = 5f;

    [Header("Validación de Espacio")] public CapsuleCollider playerCollider; 
    public LayerMask obstacleMask; 
    public GroundSensor groundSensor;
    
    private Vector3 targetScale;
    private float targetCamDistance;
    private CinemachineFramingTransposer framingTransposer;
    private Rigidbody rb;

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
        
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        if (framingTransposer)
        {
            framingTransposer.m_CameraDistance = Mathf.Lerp(framingTransposer.m_CameraDistance, targetCamDistance,
                Time.deltaTime * scaleSpeed);
        }
        
        if (transform.position.y < -10)
        {
            GameManager.Instance.RestartLevel(); 
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(titanKey))
        {
            if (currentSize == PlayerSize.Titan)
                AttemptChangeSize(PlayerSize.Normal);
            else
                AttemptChangeSize(PlayerSize.Titan);
        }

        if (Input.GetKeyDown(microKey))
        {
            if (currentSize == PlayerSize.Micro)
                AttemptChangeSize(PlayerSize.Normal);
            else
                AttemptChangeSize(PlayerSize.Micro);
        }
    }

    void AttemptChangeSize(PlayerSize newSize)
    {
        if (newSize > currentSize)
        {
            if (!CanFitInSpace(newSize))
            {
                Debug.LogWarning("¡Transformación cancelada! Chocarías con una pared o techo.");
                return;
            }
        }

        SetSize(newSize);
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
        
        if (!instant && LevelScoreManager.Instance != null && currentSize != PlayerSize.Normal)
        {
            LevelScoreManager.Instance.RegisterTransformation();
        }

        if (instant)
        {
            transform.localScale = targetScale;
            if (framingTransposer) framingTransposer.m_CameraDistance = targetCamDistance;
        }
    }
    
    bool CanFitInSpace(PlayerSize targetSize)
    {
        if (!playerCollider) return true;

        
        Vector3 futureScale = targetSize switch
        {
            PlayerSize.Micro => microScale,
            PlayerSize.Normal => normalScale,
            PlayerSize.Titan => titanScale,
            _ => normalScale
        };

       
        float futureRadius = playerCollider.radius * futureScale.x;
        float futureHeight = playerCollider.height * futureScale.y;
        float futureCenterY = playerCollider.center.y * futureScale.y;

     
        Vector3 centerWorld = transform.position + (Vector3.up * futureCenterY);
        float halfHeightMinusRadius = (futureHeight / 2f) - futureRadius;

        Vector3 pointBottom = centerWorld - (Vector3.up * halfHeightMinusRadius);
        Vector3 pointTop = centerWorld + (Vector3.up * halfHeightMinusRadius);
        
        Collider[] hits = new Collider[10];
        
      
        int numberOfHits = Physics.OverlapCapsuleNonAlloc(pointBottom, pointTop, futureRadius, hits, obstacleMask);

     
        if (numberOfHits == 0) return true;

   
        for (int i = 0; i < numberOfHits; i++)
        {
            Collider currentHit = hits[i];

           
            if (groundSensor != null && groundSensor.currentGroundedCollider == currentHit)
            {
                continue; 
            }

          
            return false; 
        }

        return true;
    }
}