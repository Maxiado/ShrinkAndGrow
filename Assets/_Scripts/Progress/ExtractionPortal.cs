using _Scripts.Progress;
using UnityEngine;

public class ExtractionPortal : MonoBehaviour
{
    [Header("Efectos de Victoria")]
    public GameObject victoryParticles; // Partículas brillantes al tocarlo
    public AudioClip victorySound;      // Un sonido de triunfo épico

    private bool isTriggered = false; // Para evitar que se active dos veces

    void OnTriggerEnter(Collider other)
    {
        // Si ya se activó, ignoramos cualquier otra colisión
        if (isTriggered) return;

        // Comprobamos si el que entró al portal es el jugador
        if (other.CompareTag("Player"))
        {
            isTriggered = true;

            // 1. Reproducir efectos visuales y sonoros
            if (victoryParticles != null)
            {
                Instantiate(victoryParticles, transform.position, Quaternion.identity);
            }
            if (victorySound != null)
            {
                AudioSource.PlayClipAtPoint(victorySound, transform.position);
            }

            // 2. Opcional: Desactivar los controles o visuales del jugador para que "desaparezca"
            // other.gameObject.SetActive(false); 

            // 3. ¡Llamar al Árbitro para calcular los puntos y mostrar la pantalla final!
            if (LevelScoreManager.Instance != null)
            {
                LevelScoreManager.Instance.FinishLevel();
            }
            else
            {
                Debug.LogError("No hay un LevelScoreManager en la escena. ¡Añade uno!");
            }
        }
    }
}