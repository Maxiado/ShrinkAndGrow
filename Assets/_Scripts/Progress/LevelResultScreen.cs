using System.Collections;
using TMPro;
using UnityEngine;

namespace _Scripts.Progress
{
    public class LevelResultScreen : MonoBehaviour
    {
        [Header("Referencias de Datos")]
        public PlayerProgress playerProgress; // Tu ScriptableObject
    
        [Header("Elementos de UI (Textos)")]
        public GameObject resultPanel; // El panel general que se activa al ganar
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI multiplierText;
        public TextMeshProUGUI transformationsText;
        public TextMeshProUGUI baseScrapsText;
        public TextMeshProUGUI bonusText;
        public TextMeshProUGUI finalTotalText;

        [Header("Elementos de UI (Botones)")]
        public GameObject buttonsContainer; // Contenedor de "Reintentar" / "Siguiente"

        [Header("Ajustes de Animación")]
        public float timeBetweenSteps = 0.5f; // Pausa entre cada estadística
        public float countAnimationSpeed = 0.05f; // Velocidad a la que suben los números

        private int finalScrapsCalculated = 0;

        void Start()
        {
            // Asegurarnos de que la pantalla esté oculta al empezar a jugar
            resultPanel.SetActive(false);
            buttonsContainer.SetActive(false);
        }

        // Esta función la llamará el LevelScoreManager cuando el jugador toque la meta
        public void ShowResults(float finalTime, float finalMultiplier, int transformationsUsed, int parTransformations, int baseScraps)
        {
            // 1. Pausamos el juego (opcional, pero recomendado)
            Time.timeScale = 0f; 
        
            // 2. Activamos el panel
            resultPanel.SetActive(true);
        
            // 3. Iniciamos la coreografía de números
            StartCoroutine(AnimateResultsRoutine(finalTime, finalMultiplier, transformationsUsed, parTransformations, baseScraps));
        }

        private IEnumerator AnimateResultsRoutine(float time, float multiplier, int transformations, int par, int baseScraps)
        {
            // Limpiamos los textos antes de empezar
            timeText.text = "";
            multiplierText.text = "";
            transformationsText.text = "";
            baseScrapsText.text = "";
            bonusText.text = "";
            finalTotalText.text = "Total: 0";

            // Paso 1: Mostrar el Tiempo y el Multiplicador
            yield return new WaitForSecondsRealtime(timeBetweenSteps);
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time - minutes * 60);
            timeText.text = $"Tiempo: {minutes:00}:{seconds:00}";
            multiplierText.text = $"Multiplicador: x{multiplier:F1}";
            // TIP: Aquí podrías reproducir un sonido de "Aparición" (Swoosh)

            // Paso 2: Mostrar la Chatarra Base con el multiplicador aplicado
            yield return new WaitForSecondsRealtime(timeBetweenSteps);
            int scrapsFromTime = Mathf.RoundToInt(baseScraps * multiplier);
            baseScrapsText.text = $"Chatarra por Tiempo: {scrapsFromTime}";
        
            // Paso 3: Mostrar las Transformaciones y calcular el Bono
            yield return new WaitForSecondsRealtime(timeBetweenSteps);
            transformationsText.text = $"Transformaciones: {transformations} / {par}";
        
            int transformationBonus = 0;
            if (transformations <= par)
            {
                transformationBonus = 50; // ¡Bono por eficiencia!
                bonusText.text = $"Bono Eficiencia: +{transformationBonus}";
                bonusText.color = Color.green;
                // TIP: Sonido de "Éxito / Campana"
            }
            else
            {
                bonusText.text = "Bono Eficiencia: Fallido";
                bonusText.color = Color.gray;
                // TIP: Sonido de "Error leve"
            }

            // Paso 4: Animar el Total Final subiendo rápidamente
            yield return new WaitForSecondsRealtime(timeBetweenSteps);
            finalScrapsCalculated = scrapsFromTime + transformationBonus;
        
            int currentCount = 0;
            while (currentCount < finalScrapsCalculated)
            {
                currentCount += 5; // Salta de 5 en 5 para que sea rápido
                if (currentCount > finalScrapsCalculated) currentCount = finalScrapsCalculated;
            
                finalTotalText.text = $"Total Ganado: {currentCount}";
                // TIP: Reproducir un sonido de "Moneda" (tic-tic-tic) muy cortito aquí
            
                yield return new WaitForSecondsRealtime(countAnimationSpeed);
            }

            // Paso 5: Guardar en la base de datos (ScriptableObject)
            if (playerProgress != null)
            {
                playerProgress.quantumScraps += finalScrapsCalculated;
            }

            // Paso 6: Mostrar los botones para continuar
            yield return new WaitForSecondsRealtime(timeBetweenSteps);
            buttonsContainer.SetActive(true);
            // TIP: Sonido de "Sello gigante / Impacto"
        }
    }
}