using UnityEngine;

public class CoreOverload : MonoBehaviour
{
    public enum CoreState { Idle, Overheating, Stabilized }
    
    [Header("Estado del Núcleo")]
    public CoreState currentState = CoreState.Idle;

    [Header("Componentes")]
    public Renderer coreRenderer;
    public ParticleSystem orbitalParticles;

    [Header("Dificultad (Cuenta Regresiva)")]
    [Tooltip("Tiempo en segundos que tiene el jugador para resolver el puzzle")]
    public float timeToExplode = 30f;
    private float timer;

    private Material coreMat;
    private Color baseColor2;

    void Start()
    {
        coreMat = coreRenderer.material;
        baseColor2 = coreMat.GetColor("_Color2");
        
        // Iniciamos el reloj con el tiempo máximo
        timer = timeToExplode; 
    }

    void Update()
    {
        if (currentState == CoreState.Overheating)
        {
            // 1. El reloj cuenta hacia atrás
            timer -= Time.deltaTime;

            // 2. Calculamos la inestabilidad 't' (0 = estable, 1 = a punto de explotar)
            // Ejemplo: Si quedan 30s, t = 0. Si quedan 0s, t = 1.
            float t = 1f - (timer / timeToExplode);
            
            // Actualizamos los VFX en tiempo real
            UpdateVFX(t);

            // 3. ¿Qué pasa si el tiempo llega a cero?
            if (timer <= 0f)
            {
                timer = 0f;
                Explode();
            }
        }
    }

    // Método que actualiza el Shader y las Partículas según el valor 't'
    private void UpdateVFX(float t)
    {
        // Shader
        coreMat.SetFloat("_FloatingSpeed", Mathf.Lerp(2f, 10f, t));
        coreMat.SetFloat("_FloatingHeight", Mathf.Lerp(0.5f, 2f, t));
        coreMat.SetFloat("_PulseSpeed", Mathf.Lerp(3f, 20f, t));
        coreMat.SetFloat("_PulseAmount", Mathf.Lerp(0.1f, 0.5f, t));
        coreMat.SetFloat("_EnergySpeed", Mathf.Lerp(1f, 8f, t));

        float colorIntensity = Mathf.Lerp(1f, 6f, t);
        coreMat.SetColor("_Color2", baseColor2 * colorIntensity);

        // Partículas
        var velocityModule = orbitalParticles.velocityOverLifetime;
        velocityModule.speedModifierMultiplier = Mathf.Lerp(1f, 5f, t);

        var emissionModule = orbitalParticles.emission;
        emissionModule.rateOverTimeMultiplier = Mathf.Lerp(20f, 150f, t);
    }

    // --- EVENTOS DEL JUEGO ---

    // Este evento se dispara cuando el jugador entra al área invisible
    private void OnTriggerEnter(Collider other)
    {
        // Asegúrate de que tu jugador tenga el Tag "Player" en el inspector
        if (other.CompareTag("Player") && currentState == CoreState.Idle)
        {
            currentState = CoreState.Overheating;
            Debug.Log("¡El núcleo detectó presencia! Iniciando sobrecarga...");
        }
    }

    // Función PÚBLICA que debes llamar desde el script de tu Puzzle cuando el jugador gane
    public void PuzzleSolved()
    {
        if (currentState == CoreState.Overheating)
        {
            currentState = CoreState.Stabilized;
            UpdateVFX(0f); // Devolvemos todos los valores a la normalidad de golpe
            Debug.Log("¡Puzzle resuelto! Núcleo estabilizado.");
        }
    }

    private void Explode()
    {
        currentState = CoreState.Idle; // Evita que se llame varias veces
        Debug.Log("¡BOOM! El jugador no logró resolver el puzzle a tiempo.");
        // Aquí puedes agregar código para reiniciar el nivel, restar vida, etc.
    }
}