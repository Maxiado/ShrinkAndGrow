using System;
using TMPro;
using UnityEngine;

public class PlayerPickUpController : MonoBehaviour
{
    private bool isContactPickable;
    private IPickable pickable;
    private bool isHoldingObj;
    private bool isOnScanner;
    
    
    public TextMeshProUGUI fPrompt;
    public Transform platform;
    public Transform startPoint;
    public float distanceThreshold; 
    public bool IsHoldingObject => isHoldingObj; 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isHoldingObj && isContactPickable && pickable != null)
            {
               
                if (CanSeePickable())
                {
                    pickable.PickUp();
                    isHoldingObj = true;
                    fPrompt.text = "F to pick up";
                    fPrompt.gameObject.SetActive(false);
                }
                else
                {
                    fPrompt.text = "Cant pick Up, shield blocking";
                }
            }
           
            else if (isHoldingObj)
            {
                if (pickable != null && !CalculateDistance())
                {
                    ForceDrop(); 
                }
            }
        }
    }

    public void ForceDrop(Vector3 pushForce = default)
    {
        if (!isHoldingObj || pickable == null) return;

        pickable.ReleaseObject();
        isHoldingObj = false;

        if (pushForce != Vector3.zero)
        {
            Rigidbody rb = (pickable as MonoBehaviour)?.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(pushForce, ForceMode.Impulse);
            }
        }

        if (!isContactPickable)
        {
            pickable = null;
        }
    }
    
    private bool CanSeePickable()
    {
        MonoBehaviour pickableObj = pickable as MonoBehaviour;
        if (pickableObj == null) return true;

        Vector3 playerChest = transform.position + Vector3.up * 1f; 
        Vector3 objectPos = pickableObj.transform.position;         

        Vector3 directionToPlayer = playerChest - objectPos;
        float distance = directionToPlayer.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(objectPos, directionToPlayer.normalized, distance);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Escudo"))
            {
                return false; 
            }
        }

        return true; 
    }
    private bool CalculateDistance()
    {
        return Vector3.Distance(transform.position, platform.position) < distanceThreshold;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            if (!isHoldingObj)
            {
                pickable = other.GetComponentInParent<IPickable>();
            }
            isContactPickable = true;
            fPrompt.gameObject.SetActive(true);
        }

        if (other.CompareTag("Scanner"))
        {
            isOnScanner = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            isContactPickable = false;
            fPrompt.gameObject.SetActive(false);
            fPrompt.text = "F to pick up";
        }
    }
}