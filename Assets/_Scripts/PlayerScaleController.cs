using _Scripts.Progress;
using UnityEngine;
using Cinemachine;

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

    [Header("Validación de Espacio")] public CapsuleCollider playerCollider; // El collider de tu jugador
    public LayerMask obstacleMask; // Selecciona "Ground", "Obstacles", "Walls", etc.
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

        // Aplicar transformaciones suaves
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        if (framingTransposer)
        {
            framingTransposer.m_CameraDistance = Mathf.Lerp(framingTransposer.m_CameraDistance, targetCamDistance,
                Time.deltaTime * scaleSpeed);
        }

        // Sistema de seguridad por caída
        if (transform.position.y < -10)
        {
            // Ojo: Asegúrate de tener tu GameManager configurado, sino esto dará error
            // GameManager.Instance.RestartLevel(); 
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
        // En C#, los Enums tienen valor numérico (Micro=0, Normal=1, Titan=2).
        // Si newSize > currentSize, significa que estamos intentando crecer.
        if (newSize > currentSize)
        {
            if (!CanFitInSpace(newSize))
            {
                Debug.LogWarning("¡Transformación cancelada! Chocarías con una pared o techo.");
                // TIP: Aquí puedes reproducir un sonido de "Error" o hacer que el jugador parpadee en rojo
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

        // --- ¡EL CAMBIO ESTÁ AQUÍ! ---
        // Solo registramos la transformación si NO es instantánea (al iniciar) 
        // Y si el nuevo tamaño NO es el Normal.
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

    // --- LA NUEVA MAGIA ANTI-EXPLOSIONES ---
    bool CanFitInSpace(PlayerSize targetSize)
    {
        if (!playerCollider) return true;

        // 1. Averiguamos qué escala vamos a tener (igual que antes)
        Vector3 futureScale = targetSize switch
        {
            PlayerSize.Micro => microScale,
            PlayerSize.Normal => normalScale,
            PlayerSize.Titan => titanScale,
            _ => normalScale
        };

        // 2. Calculamos las dimensiones mundiales (igual que antes)
        float futureRadius = playerCollider.radius * futureScale.x;
        float futureHeight = playerCollider.height * futureScale.y;
        float futureCenterY = playerCollider.center.y * futureScale.y;

        // 3. Calculamos los puntos superior e inferior de la cápsula (igual que antes)
        Vector3 centerWorld = transform.position + (Vector3.up * futureCenterY);
        float halfHeightMinusRadius = (futureHeight / 2f) - futureRadius;

        Vector3 pointBottom = centerWorld - (Vector3.up * halfHeightMinusRadius);
        Vector3 pointTop = centerWorld + (Vector3.up * halfHeightMinusRadius);

        // ¡TRUCO CLAVE! Ya no necesitamos subir el pointBottom (el +0.1f)
        // porque ahora vamos a ignorar el suelo programáticamente.

        // 4. NUEVA LÓGICA PROFESIONAL:
        // En lugar de CheckCapsule, usamos OverlapCapsule para obtener TODOS los choques.
        
        // Creamos una "lista" (un array) para que Unity ponga los resultados.
        // Un tamaño de 10 es más que suficiente para una detección.
        Collider[] hits = new Collider[10];
        
        // Usamos NonAlloc por rendimiento: detecta objetos en la cápsula y los pone en la lista.
        // Retorna el número de objetos encontrados.
        int numberOfHits = Physics.OverlapCapsuleNonAlloc(pointBottom, pointTop, futureRadius, hits, obstacleMask);

        // 5. ¡EL FILTRADO! Revisamos todos los objetos que Unity encontró.
        // SinumberOfHits es 0, significa que no chocamos con nada, por lo que cabe.
        if (numberOfHits == 0) return true;

        // Si encontramos objetos, vamos uno por uno:
        for (int i = 0; i < numberOfHits; i++)
        {
            Collider currentHit = hits[i];

            // --- ESTE ES EL CORAZÓN DE LA SOLUCIÓN ---
            // Le preguntamos al groundSensor: "¿currentHit es el mismo objeto que estamos pisando?"
            if (groundSensor != null && groundSensor.currentGroundedCollider == currentHit)
            {
                // ¡SÍ! Estamos chocando con el suelo bajo nuestros pies.
                // Como es el suelo que pisamos, NO es un obstáculo para la transformación.
                // Simplemente ignoramos este choque y seguimos revisando el resto de la lista.
                continue; 
            }

            // Si llegamos aquí, significa que currentHit NO es el suelo que pisamos.
            // Por lo tanto, es una pared, un techo u otro objeto que SÍ es un obstáculo.
            // Retornamos FALSE inmediatamente: ¡No hay espacio!
            return false; 
        }

        // Si revisamos toda la lista y no encontramos ningún obstáculo real
        // (solo ignoramos el suelo que pisamos), significa que hay espacio libre.
        return true;
    }
}