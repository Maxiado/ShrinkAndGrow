using _Scripts.Progress;
using UnityEngine;
using TMPro; // Para la UI

public class LevelScoreManager : MonoBehaviour
{
    public static LevelScoreManager Instance;

    [Header("Objetivos del Nivel")]
    public int parTransformations = 3;
    public int baseScrapsReward = 100;

    [Header("Temporizador y Multiplicador")]
    public float timeForMaxMultiplier = 30f;
    public float timeForZeroMultiplier = 120f;
    public float maxMultiplier = 3.0f;

    [Header("Elementos de UI (HUD en pantalla)")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI transformationCountText;
    
    // ¡NUEVA VARIABLE PARA LA CHATARRA!
    public TextMeshProUGUI inLevelScrapsText; 

    [HideInInspector] public int inLevelScrapsCollected = 0;
    
    private float timeElapsed = 0f;
    private int transformationsUsed = 0;
    private bool levelCompleted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Forzamos que el texto empiece en 0
        UpdateScrapsUI();
        UpdateUI();
    }

    void Update()
    {
        if (levelCompleted) return;

        timeElapsed += Time.deltaTime;
        UpdateUI();
    }

    // El jugador llama a esto cuando toca la tecla Q o E
    public void RegisterTransformation()
    {
        if (levelCompleted) return;
        transformationsUsed++;
        UpdateUI();
    }

    // El coleccionable (QuantumScrap) llama a esto al ser tocado
    public void AddInLevelScraps(int amount)
    {
        if (levelCompleted) return;
        
        inLevelScrapsCollected += amount;
        
        // ¡Llamamos a la actualización visual inmediatamente!
        UpdateScrapsUI(); 
    }

    public float GetCurrentMultiplier()
    {
        if (timeElapsed <= timeForMaxMultiplier) return maxMultiplier;
        if (timeElapsed >= timeForZeroMultiplier) return 0f;
        float t = (timeElapsed - timeForMaxMultiplier) / (timeForZeroMultiplier - timeForMaxMultiplier);
        return Mathf.Lerp(maxMultiplier, 0f, t);
    }

    private void UpdateScrapsUI()
    {
        if (inLevelScrapsText != null)
        {
            inLevelScrapsText.text = $"{inLevelScrapsCollected}";
            // Aquí podrías disparar una animación para que el número "salte"
        }
    }

    private void UpdateUI()
    {
        // (Aquí va el mismo código que ya tenías para actualizar el tiempo, el multiplicador y las transformaciones)
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeElapsed / 60F);
            int seconds = Mathf.FloorToInt(timeElapsed - minutes * 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (multiplierText != null)
        {
            multiplierText.text = $"Bono: x{GetCurrentMultiplier():F1}";
            if (GetCurrentMultiplier() <= 0) multiplierText.color = Color.gray;
            else multiplierText.color = Color.yellow;
        }

        if (transformationCountText != null)
        {
            transformationCountText.text = $"Cambios: {transformationsUsed} / {parTransformations}";
            if (transformationsUsed > parTransformations) transformationCountText.color = Color.red;
        }
    }

    public void FinishLevel()
    {
        levelCompleted = true;
        float finalMultiplier = GetCurrentMultiplier();
        int totalBaseScraps = baseScrapsReward + inLevelScrapsCollected;

        LevelResultScreen resultScreen = GetComponent<LevelResultScreen>();
        
        if (resultScreen != null)
        {
            resultScreen.ShowResults(timeElapsed, finalMultiplier, transformationsUsed, parTransformations, totalBaseScraps);
        }
    }
}