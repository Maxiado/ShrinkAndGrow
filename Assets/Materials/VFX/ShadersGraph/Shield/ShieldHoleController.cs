using UnityEngine;

public class ShieldHoleController : MonoBehaviour
{
    public Transform player;
    public Renderer shieldRenderer;
    private Material shieldMat;
    private Collider shieldCollider;

    [Header("Configuración del Agujero")]
    [Tooltip("El tamaño máximo del agujero cuando el jugador lo atraviesa")]
    public float maxHoleRadius = 1.2f;
    
    [Tooltip("A qué distancia en metros empieza a abrirse el agujero")]
    public float activationDistance = 3.0f; 
    void Start()
    {
        shieldMat = shieldRenderer.material;
        
        // Necesitamos el Collider del escudo para saber dónde está su superficie
        shieldCollider = GetComponent<Collider>(); 
    }

    void Update()
    {
        if (player != null && shieldMat != null && shieldCollider != null)
        {
            // La posición central del jugador
            Vector3 playerTarget = player.position;

            // 1. Encontramos el punto del escudo que está más cerca del jugador
            Vector3 pointOnShield = shieldCollider.ClosestPoint(playerTarget);

            // Le enviamos este punto al shader para que dibuje el agujero ahí
            shieldMat.SetVector("_PlayerPosition", pointOnShield);

            // 2. Calculamos la distancia real entre el jugador y ese punto del escudo
            float distanceToShield = Vector3.Distance(playerTarget, pointOnShield);

            // 3. Lógica de Apertura (Proximidad)
            float currentRadius = 0f;
            
            if (distanceToShield < activationDistance)
            {
                // Calculamos un porcentaje de 0 (lejos) a 1 (tocando el escudo)
                float percentage = 1f - (distanceToShield / activationDistance);
                
                // Interpolamos suavemente desde 0 hasta el tamaño máximo
                currentRadius = Mathf.Lerp(0f, maxHoleRadius, percentage);
            }

            // 4. Le enviamos el nuevo tamaño animado al Shader
            // IMPORTANTE: Asegúrate de que el nombre Reference de tu variable Radius sea "_HoleRadius"
            shieldMat.SetFloat("_HoleRadius", currentRadius); 
        }
    }
}