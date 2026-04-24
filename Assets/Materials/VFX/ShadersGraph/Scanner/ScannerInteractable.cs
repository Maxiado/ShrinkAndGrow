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
        // Solo verificamos si está en rango y presiona la tecla
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // Validar si ya está escaneando para no solapar animaciones
            if (scanner.isScanning) return;

            // Iniciar el escaneo SIEMPRE, sin importar lo que tenga en las manos
            StartCoroutine(FullSecuritySequence());
        }
    }

    private IEnumerator FullSecuritySequence()
    {
        GameManager.Instance.LockPlayerInput(true);
        // 1. Iniciamos movimiento físico y efecto visual
        scanner.StartRotationSequence();
        shaderEffect.PlayShaderEffect();

        // 2. ESPERAR a que el escáner termine su ciclo (ida y vuelta)
        float totalWaitTime = scanner.durationPerMove * 2;
        yield return new WaitForSeconds(totalWaitTime);

        // 3. VERIFICACIÓN AL FINALIZAR EL ESCANEO
        bool hasValidKey = false;

        // Validamos que el jugador siga dentro de la zona al terminar el escaneo
        if (playerRef) 
        {
            IPickable item = playerRef.GetComponentInChildren<IPickable>();

            if (item != null && item.GetPickableType() == requiredKey)
            {
                hasValidKey = true;
            }
        }

        // 4. RESULTADO DEL ESCANEO
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
            playerRef = null; // Limpiamos la referencia si se va
            fPrompt.SetActive(false);
        }
    }
}