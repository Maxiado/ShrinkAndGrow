using UnityEngine;

public class ShieldHoleController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Renderer shieldRenderer;
    public PlayerPickUpController playerPickUp;
    
    private Material shieldMat;
    private Collider shieldCollider;

    [Header("Configuración del Agujero")]
    [Tooltip("El tamaño máximo del agujero cuando el jugador lo atraviesa")]
    public float maxHoleRadius = 1.2f;
    [Tooltip("A qué distancia en metros empieza a abrirse el agujero")]
    public float activationDistance = 3.0f; 
    
    [Header("Configuración de Bloqueo")]
    [Tooltip("Distancia a la que el escudo te obliga a soltar el objeto")]
    public float dropDistance = 1.0f;

    void Start()
    {
        // ESTA ES LA LÍNEA QUE FALTABA: Asignar el material
        if (shieldRenderer)
        {
            shieldMat = shieldRenderer.material;
        }
        
        shieldCollider = GetComponent<Collider>(); 
    }

    void Update()
    {
        // Nos aseguramos de tener todas las referencias antes de hacer cálculos
        if (player != null && shieldCollider != null && playerPickUp != null)
        {
            // 1. Cálculos de Posición y Distancia
            Vector3 playerTarget = player.position;
            Vector3 pointOnShield = shieldCollider.ClosestPoint(playerTarget);
            float distanceToShield = Vector3.Distance(playerTarget, pointOnShield);

            // 2. Leemos el estado del jugador
            bool isHoldingObject = playerPickUp.IsHoldingObject;
            

            // 4. Lógica de Forzar Soltar (El escudo bloquea objetos)
            if (isHoldingObject && distanceToShield <= dropDistance)
            {
                // Calculamos el empuje: Desde el punto del escudo hacia la posición del jugador
                Vector3 pushBackDirection = (player.position - pointOnShield).normalized;
                pushBackDirection.y = 1.0f; // Pequeño salto hacia arriba para que el rebote se vea natural
            
                // Obligamos al jugador a soltar y le pasamos la fuerza de empuje
                playerPickUp.ForceDrop(pushBackDirection * 4f);
            }
            
            // 5. Lógica de Apertura del Agujero
            float currentRadius = 0f;
            if (distanceToShield < activationDistance)
            {
                // Calculamos porcentaje: 0 (está lejos) a 1 (está pegado al escudo)
                float percentage = 1f - (distanceToShield / activationDistance);
                currentRadius = Mathf.Lerp(0f, maxHoleRadius, percentage);
            }

            // 6. Enviamos los datos finales al Shader
            shieldMat.SetVector("_PlayerPosition", pointOnShield);
            shieldMat.SetFloat("_HoleRadius", currentRadius); 
        }
    }
}