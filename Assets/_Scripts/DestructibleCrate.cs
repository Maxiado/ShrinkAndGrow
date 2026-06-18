using UnityEngine;

namespace _Scripts
{
    public class DestructibleCrate : MonoBehaviour
    {
        [Header("Recompensas")]
        public GameObject scrapPrefab; 
        public GameObject vfxExplosionPrefab; 
        public int minScraps = 2;
        public int maxScraps = 5;

        [Header("Efectos")]
        public AudioClip breakSound;            

        [Header("Física del 'Pop'")]
        public float popForce = 5f; 

        private bool isDestroyed = false;

        public void BreakCrate()
        {
            if(isDestroyed) return;
            isDestroyed = true;
        
            int scrapsToDrop = Random.Range(minScraps, maxScraps + 1);

            for (int i = 0; i < scrapsToDrop; i++)
            {
              
                GameObject scrap = Instantiate(scrapPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                Instantiate(vfxExplosionPrefab, transform.position, Quaternion.identity);
                Rigidbody scrapRb = scrap.GetComponent<Rigidbody>();
                if (scrapRb != null)
                {
                    Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f)).normalized;
                    scrapRb.AddForce(randomDir * popForce, ForceMode.Impulse);
                    scrapRb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * popForce);
                }
                else
                {
                    scrap.transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
                }
            }
            Destroy(gameObject);
        }
    }
}