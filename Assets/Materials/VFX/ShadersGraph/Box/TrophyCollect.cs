using UnityEngine;

public class TrophyCollect : MonoBehaviour
{
    private bool isCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (!other.CompareTag("Player")) return;
        isCollected = true;

        if (LevelScoreManager.Instance)
        {
            LevelScoreManager.Instance.AddInLevelTrophy(1);
        }

        Destroy(gameObject);
    }
}