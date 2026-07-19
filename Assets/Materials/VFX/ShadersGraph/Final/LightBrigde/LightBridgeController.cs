using System;
using System.Collections;
using UnityEngine;

public class LightBridgeController : MonoBehaviour
{
    [Header("Shader Settings")]
    public string extensionProperty = "_Extension";
    public float animationDuration = 1.5f;

    [Header("Complex System References")]
    public ParticleSystem sparkParticles;
    public Transform startPoint; 
    public Transform endPoint;   

    private Material bridgeMaterial;
    private Collider bridgeCollider;
    private bool isBridgeActive = false;
    private Coroutine bridgeCoroutine;

    void Start()
    {
        bridgeCollider = GetComponent<Collider>();
        var rend = GetComponent<Renderer>();
        
        if (rend)
        {
            bridgeMaterial = rend.material;
            bridgeMaterial.SetFloat(extensionProperty, 0f);
        }

        if (bridgeCollider) bridgeCollider.enabled = false;
    }
    
    public void ToggleBridge()
    {
        if (bridgeCoroutine != null) StopCoroutine(bridgeCoroutine);

        if (isBridgeActive)
        {
            bridgeCoroutine = StartCoroutine(AnimateBridge(1f, 0f));
            isBridgeActive = false;
        }
        else
        {
            bridgeCoroutine = StartCoroutine(AnimateBridge(0f, 1f));
            isBridgeActive = true;
        }
    }

    private IEnumerator AnimateBridge(float startVal, float endVal)
    {
        if (endVal > 0.5f && bridgeCollider) bridgeCollider.enabled = true;

        if (sparkParticles) sparkParticles.Play();

        var timeElapsed = 0f;
        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            var currentValue = Mathf.Lerp(startVal, endVal, timeElapsed / animationDuration);
            
            if (bridgeMaterial) bridgeMaterial.SetFloat(extensionProperty, currentValue);
            
            if (sparkParticles && startPoint && endPoint)
            {
                sparkParticles.transform.position = Vector3.Lerp(startPoint.position, endPoint.position, currentValue);
            }

            yield return null;
        }

        if (bridgeMaterial) bridgeMaterial.SetFloat(extensionProperty, endVal);
        
        if (sparkParticles) sparkParticles.Stop();

        if (endVal < 0.5f && bridgeCollider) bridgeCollider.enabled = false;
    }
}