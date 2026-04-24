using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform playerHoldPos;
    public MonoBehaviour[] playerScripts;
    public Rigidbody playerRb;
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void LockPlayerInput(bool lockInput)
    {
        // Recorremos la lista de scripts y los encendemos o apagamos
        foreach (MonoBehaviour script in playerScripts)
        {
            if (script != null)
            {
                // Si lockInput es true, queremos que enabled sea false (por eso la negación !)
                script.enabled = !lockInput; 
            }
        }

        // Si lo estamos bloqueando, le quitamos la inercia para que no siga resbalando
        if (lockInput && playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
        }
    }
}
