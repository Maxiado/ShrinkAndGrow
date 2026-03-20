using UnityEngine;

namespace _Scripts.Progress
{
    public class QuantumScrap : MonoBehaviour
    {
        [Header("Configuración")]
        public int scrapValue = 5; // Cuánta chatarra suma al recogerlo
        public float spinSpeed = 120f; // Velocidad de rotación para llamar la atención

        [Header("Efectos (Opcional)")]
        // public GameObject collectParticles; // Arrastra un prefab de partículas aquí
        // public AudioClip collectSound;      // Arrastra un sonido de "clink" o "moneda"

        private bool isCollected = false;

        void Update()
        {
            // Hace que el objeto rote constantemente en el eje Y
            transform.Rotate(Vector3.up * (spinSpeed * Time.deltaTime), Space.World);
        }

        void OnTriggerEnter(Collider other)
        {
            // Evitar que se recoja dos veces en el mismo frame
            if (isCollected) return;

            if (other.CompareTag("Player"))
            {
                isCollected = true;

                // 1. Sumar los puntos al Manager del nivel
                if (LevelScoreManager.Instance != null)
                {
                    LevelScoreManager.Instance.AddInLevelScraps(scrapValue);
                }

                // 2. Reproducir efectos visuales y sonoros
                // if (collectParticles != null)
                // {
                //     Instantiate(collectParticles, transform.position, Quaternion.identity);
                // }
                //
                // if (collectSound != null)
                // {
                //     // Usamos PlayClipAtPoint para que el sonido suene aunque el objeto se destruya
                //     AudioSource.PlayClipAtPoint(collectSound, transform.position);
                // }

                // 3. Destruir el objeto
                Destroy(gameObject);
            }
        }
    }
}