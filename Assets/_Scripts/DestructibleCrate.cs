using UnityEngine;

namespace _Scripts
{
    public class DestructibleCrate : MonoBehaviour
    {
        [Header("Recompensas")]
        public GameObject scrapPrefab; // Arrastra tu prefab de QuantumScrap aquí
        public int minScraps = 2;
        public int maxScraps = 5;

        [Header("Efectos")]
        public GameObject destructionParticles; // Prefab de humo/chispas
        public AudioClip breakSound;            // Sonido de madera o metal rompiéndose

        [Header("Física del 'Pop'")]
        public float popForce = 5f; // Fuerza con la que sale volando la chatarra

        private bool isDestroyed = false;

        // Usamos OnCollisionEnter porque la caja es un objeto sólido
        void OnCollisionEnter(Collision collision)
        {
            if (isDestroyed) return;

            if (collision.gameObject.CompareTag("Player"))
            {
                // Obtenemos el controlador del jugador para saber su tamaño
                PlayerScaleController playerScale = collision.gameObject.GetComponent<PlayerScaleController>();

                // ¡La magia! Solo se rompe si el jugador es el Titán
                if (playerScale != null && playerScale.IsTitan)
                {
                    BreakCrate();
                }
            }
        }

        // Opcional: Si quieres que el Titán la rompa al caerle encima
        void OnTriggerEnter(Collider other)
        {
            if (isDestroyed) return;

            if (other.CompareTag("Player"))
            {
                PlayerScaleController playerScale = other.GetComponent<PlayerScaleController>();
                // Si tiene un trigger en los pies (para aplastar)
                if (playerScale != null && playerScale.IsTitan)
                {
                    BreakCrate();
                }
            }
        }

        void BreakCrate()
        {
            isDestroyed = true;

            // 1. Efectos visuales y sonoros
            if (destructionParticles != null)
            {
                Instantiate(destructionParticles, transform.position, Quaternion.identity);
            }
            if (breakSound != null)
            {
                AudioSource.PlayClipAtPoint(breakSound, transform.position);
            }

            // 2. Generar la chatarra
            int scrapsToDrop = Random.Range(minScraps, maxScraps + 1);

            for (int i = 0; i < scrapsToDrop; i++)
            {
                // Instanciamos la chatarra un poco por encima del centro de la caja
                GameObject scrap = Instantiate(scrapPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);

                // Intentamos darle un empuje físico para que salgan volando como en una piñata
                Rigidbody scrapRb = scrap.GetComponent<Rigidbody>();
                if (scrapRb != null)
                {
                    // Dirección aleatoria hacia arriba y a los lados
                    Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f)).normalized;
                    scrapRb.AddForce(randomDir * popForce, ForceMode.Impulse);
                
                    // Añadimos un poco de torque para que salgan girando locamente
                    scrapRb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * popForce);
                }
                else
                {
                    // Si tu prefab no tiene Rigidbody, simplemente los esparcimos un poco en el espacio
                    scrap.transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
                }
            }

            // 3. Destruir la caja
            Destroy(gameObject);
        }
    }
}