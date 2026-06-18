using System;
using System.Collections;
using UnityEngine;

public class Pickable : MonoBehaviour, IPickable
{
    [SerializeField] private PickableType pickableType;
    [SerializeField] private float transitionDuration;
    private Rigidbody rb;
    private Collider col; 
    public Collider colChild;

    //Variables para el Shader
    public string pickupVariableName = "_IsPickedUp";
    private Material artifactMaterial;
    private Coroutine transitionCoroutine;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>(); 
        var rend = GetComponent<Renderer>();
        if (!rend) return;
        artifactMaterial = rend.material;
        artifactMaterial.SetFloat(pickupVariableName, 0f);
    }
    public PickableType GetPickableType()
    {
        return pickableType;
    }

    public void PickUp()
    {
        rb.isKinematic = true;
        
        if (col) col.enabled = false; 
        if (colChild) colChild.enabled = false; 
        transform.SetParent(GameManager.Instance.playerHoldPos);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        ActivateArtifact();
    }

    public void ReleaseObject()
    {
        rb.isKinematic = false;
        if (col) col.enabled = true; 
        if (colChild) colChild.enabled = true; 
        transform.SetParent(null);
        
        DeactivateArtifact();
    }
    
    //Activacion y desactivacion del shader progresivamente
    private void ActivateArtifact()
    {
        if (!artifactMaterial) return;
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(SmoothTransition(0f, 1f));
    }
    private void DeactivateArtifact()
    {
        if (!artifactMaterial) return;
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(SmoothTransition(1f, 0f));
    }
    
    private IEnumerator SmoothTransition(float startValue, float endValue)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / transitionDuration);
            artifactMaterial.SetFloat(pickupVariableName, currentValue);
            
            yield return null;
        }
        artifactMaterial.SetFloat(pickupVariableName, endValue);
    }
}