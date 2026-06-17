using System.Collections;
using UnityEngine;

public class ScannerInteractable : MonoBehaviour
{
   [Header("Referencias de Sistemas")]
    public XAxisRotator scanner;
    public MartianShaderController shaderEffect;
    public GameObject doorToUnlock;

    [Header("Configuración")]
    public PickableType requiredKey = PickableType.RedKey;
    public ScannerRaysController raysEffect;
    private bool isPlayerInRange = false;
    private GameObject playerRef;
    public GameObject fPrompt;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (scanner.isScanning) return;

            StartCoroutine(FullSecuritySequence());
        }
    }

    private IEnumerator FullSecuritySequence()
    {
        GameManager.Instance.LockPlayerInput(true);
        scanner.StartRotationSequence();
        shaderEffect.PlayShaderEffect();

        float totalWaitTime = scanner.durationPerMove * 2;
        yield return new WaitForSeconds(totalWaitTime);

        bool hasValidKey = false;

        if (playerRef) 
        {
            IPickable item = playerRef.GetComponentInChildren<IPickable>();

            if (item != null && item.GetPickableType() == requiredKey)
            {
                hasValidKey = true;
            }
        }

        if (hasValidKey)
        {
            if (raysEffect) raysEffect.FlashSuccess();
            if (doorToUnlock)
            {
                doorToUnlock.SetActive(false); 
            }
        }
        else
        {
            if (raysEffect) raysEffect.FlashFailed();
        }
        GameManager.Instance.LockPlayerInput(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerRef = other.gameObject;
            fPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerRef = null;
            fPrompt.SetActive(false);
        }
    }
}