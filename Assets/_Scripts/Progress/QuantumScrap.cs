using UnityEngine;

namespace _Scripts.Progress
{
    public class QuantumScrap : MonoBehaviour
    {
        [Header("Configuración")]
        public int scrapValue = 5; 
        public float spinSpeed = 120f; 

        [Header("Efectos")]
         public GameObject collectParticles; 
       

        private bool isCollected = false;

      

        void OnTriggerEnter(Collider other)
        {
            
            if (isCollected) return;

            if (other.CompareTag("Player"))
            {
                isCollected = true;

              
                if (LevelScoreManager.Instance)
                {
                    LevelScoreManager.Instance.AddInLevelScraps(scrapValue);
                }
                
                if (collectParticles)
                {
                    Instantiate(collectParticles, transform.position, Quaternion.identity);
                }
                
                Destroy(gameObject);
            }
        }
    }
}