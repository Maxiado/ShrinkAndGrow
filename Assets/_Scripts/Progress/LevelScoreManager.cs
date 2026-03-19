using TMPro;
using UnityEngine;

namespace _Scripts.Progress
{
    public class LevelScoreManager : MonoBehaviour
    {
        public static LevelScoreManager Instance; // Singleton para fácil acceso

        [Header("Objetivos del Nivel")]
        public int parTransformations = 3;
        public int baseScrapsReward = 100; // Cuánta chatarra da el nivel al pasarlo

        [Header("Temporizador y Multiplicador")]
        public float timeForMaxMultiplier = 30f; // Si lo pasas en menos de 30s, tienes x3
        public float timeForZeroMultiplier = 120f; // Si tardas más de 2 mins, tienes x0
        public float maxMultiplier = 3.0f;

        [Header("Elementos de UI (Arrastrar desde el Canvas)")]
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI multiplierText;
        public TextMeshProUGUI transformationCountText;

        // Variables internas
        private float timeElapsed = 0f;
        private int transformationsUsed = 0;
        private bool levelCompleted = false;

        void Awake()
        {
            // Configuramos el Singleton
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            UpdateUI();
        }

        void Update()
        {
            if (levelCompleted) return;

            timeElapsed += Time.deltaTime;
            UpdateUI();
        }

        // El PlayerScaleController llamará a esta función cada vez que use Q o E
        public void RegisterTransformation()
        {
            if (levelCompleted) return;
        
            transformationsUsed++;
            UpdateUI();
        }

        // Calcula el multiplicador actual basado en el tiempo
        public float GetCurrentMultiplier()
        {
            if (timeElapsed <= timeForMaxMultiplier) return maxMultiplier;
            if (timeElapsed >= timeForZeroMultiplier) return 0f;

            // Calcula cuánto ha bajado entre el tiempo mínimo y el máximo
            float t = (timeElapsed - timeForMaxMultiplier) / (timeForZeroMultiplier - timeForMaxMultiplier);
            return Mathf.Lerp(maxMultiplier, 0f, t);
        }

        private void UpdateUI()
        {
            if (timerText != null)
            {
                // Formatear tiempo en Minutos:Segundos
                int minutes = Mathf.FloorToInt(timeElapsed / 60F);
                int seconds = Mathf.FloorToInt(timeElapsed - minutes * 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            if (multiplierText != null)
            {
                multiplierText.text = $"Bono: x{GetCurrentMultiplier():F1}";
            
                // Cambiar color visualmente si se acaba el bono
                if (GetCurrentMultiplier() <= 0) multiplierText.color = Color.gray;
                else multiplierText.color = Color.yellow;
            }

            if (transformationCountText != null)
            {
                transformationCountText.text = $"Cambios: {transformationsUsed} / {parTransformations}";
            
                // Si nos pasamos del Par, lo ponemos en rojo para avisar al jugador
                if (transformationsUsed > parTransformations)
                    transformationCountText.color = Color.red;
            }
        }

        // Esta función se llama al cruzar la puerta de victoria
        public void FinishLevel()
        {
            levelCompleted = true;
        
            float finalMultiplier = GetCurrentMultiplier();
            int earnedScraps = Mathf.RoundToInt(baseScrapsReward * finalMultiplier);
        
            bool metTransformationGoal = (transformationsUsed <= parTransformations);
            if (metTransformationGoal)
            {
                earnedScraps += 50; // Bono extra por ser eficiente con las formas
            }

            Debug.Log($"¡Nivel Completado! Multiplicador: x{finalMultiplier:F1} | Cambios: {transformationsUsed}");
            Debug.Log($"Chatarra ganada: {earnedScraps} (Bono objetivo formas: {metTransformationGoal})");
        
            // Aquí le sumaríamos earnedScraps al PlayerProgress.cs
        }
    }
}