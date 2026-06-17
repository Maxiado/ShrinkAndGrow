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
        if (shieldRenderer)
        {
            shieldMat = shieldRenderer.material;
        }
        
        shieldCollider = GetComponent<Collider>(); 
    }

    void Update()
    {
        if (player && shieldCollider && playerPickUp)
        {
            Vector3 playerTarget = player.position;
            Vector3 pointOnShield = shieldCollider.ClosestPoint(playerTarget);
            float distanceToShield = Vector3.Distance(playerTarget, pointOnShield);

            bool isHoldingObject = playerPickUp.IsHoldingObject;
            
            if (isHoldingObject && distanceToShield <= dropDistance)
            {
                Vector3 pushBackDirection = (player.position - pointOnShield).normalized;
                pushBackDirection.y = 1.0f;
            
                playerPickUp.ForceDrop(pushBackDirection * 4f);
            }
            
            float currentRadius = 0f;
            if (distanceToShield < activationDistance)
            {
                float percentage = 1f - (distanceToShield / activationDistance);
                currentRadius = Mathf.Lerp(0f, maxHoleRadius, percentage);
            }

            shieldMat.SetVector("_PlayerPosition", pointOnShield);
            shieldMat.SetFloat("_HoleRadius", currentRadius); 
        }
    }
}