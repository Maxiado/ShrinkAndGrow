using System;
using UnityEngine;

public class PlayerPickUpController : MonoBehaviour
{
    private bool isContactPickable;
    private IPickable pickable;
    private bool isHoldingObj;
    private bool isOnScanner;
    public GameObject fPrompt;
    public Transform platform;
    public float distanceThreshold;

    private void Update()
    {
        // Solo revisamos el input una vez por frame
        if (Input.GetKeyDown(KeyCode.F))
        {
            // CASO 1: No tenemos nada en las manos y podemos recoger algo
            if (!isHoldingObj && isContactPickable && pickable != null)
            {
                pickable.PickUp();
                isHoldingObj = true;

                // FORZAMOS QUE SE APAGUE LA INTERFAZ AQUÍ
                fPrompt.SetActive(false);
            }
            // CASO 2: Ya tenemos algo en las manos y NO estamos sobre el escáner
            else if (isHoldingObj)
            {
                if (pickable != null)
                {
                    if (!CalculateDistance())
                    {
                        pickable.ReleaseObject();
                        isHoldingObj = false;

                        // Si al soltarlo ya no estamos chocando con él, limpiamos la variable
                        if (!isContactPickable)
                        {
                            pickable = null;
                        }
                    }
                }
            }
        }
    }

    private bool CalculateDistance()
    {
        Debug.Log(Vector3.Distance(transform.position, platform.position));
        return Vector3.Distance(transform.position, platform.position) < distanceThreshold;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            // Solo actualizamos si no tenemos nada en las manos
            if (!isHoldingObj)
            {
                pickable = other.GetComponentInParent<IPickable>();
            }

            isContactPickable = true;
            fPrompt.SetActive(true);
        }

        if (other.CompareTag("Scanner"))
        {
            Debug.Log("ENTRÓ A: " + other.gameObject.name);
            isOnScanner = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            isContactPickable = false;
            fPrompt.SetActive(false);
        }
    }
}