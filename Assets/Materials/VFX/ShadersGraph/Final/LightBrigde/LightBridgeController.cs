using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering; 

public class LightBridgeController : MonoBehaviour
{
    [Header("Shader Settings")]
    public string extensionProperty = "_Extension";
    public float animationDuration = 1.5f;

    [Header("Complex System References")]
    public ParticleSystem sparkParticles;
    public Transform startPoint; // Donde nace la luz
    public Transform endPoint;   // Donde termina la luz

    private Material bridgeMaterial;
    private Collider bridgeCollider;
    private bool isBridgeActive = false;
    private Coroutine bridgeCoroutine;

    void Start()
    {
        bridgeCollider = GetComponent<Collider>();
        Renderer rend = GetComponent<Renderer>();
        
        if (rend != null)
        {
            bridgeMaterial = rend.material;
            bridgeMaterial.SetFloat(extensionProperty, 0f); // Empieza apagado
        }

        if (bridgeCollider != null) bridgeCollider.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleBridge();
        }
    }

    public void ToggleBridge()
    {
        if (bridgeCoroutine != null) StopCoroutine(bridgeCoroutine);

        if (isBridgeActive)
        {
            // Apagar (Desintegrar)
            bridgeCoroutine = StartCoroutine(AnimateBridge(1f, 0f));
            isBridgeActive = false;
        }
        else
        {
            // Encender (Crear)
            bridgeCoroutine = StartCoroutine(AnimateBridge(0f, 1f));
            isBridgeActive = true;
        }
    }

    private IEnumerator AnimateBridge(float startVal, float endVal)
    {
        // Activar colisión rápido si se está encendiendo
        if (endVal > 0.5f && bridgeCollider != null) bridgeCollider.enabled = true;

        // Encender el emisor de partículas
        if (sparkParticles != null) sparkParticles.Play();

        float timeElapsed = 0f;
        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            
            // currentValue va de 0 a 1 (o de 1 a 0)
            float currentValue = Mathf.Lerp(startVal, endVal, timeElapsed / animationDuration);
            
            // 1. Actualiza el Shader
            if (bridgeMaterial != null) bridgeMaterial.SetFloat(extensionProperty, currentValue);
            
            // 2. Mueve las partículas EXACTAMENTE al borde de la luz
            if (sparkParticles != null && startPoint != null && endPoint != null)
            {
                sparkParticles.transform.position = Vector3.Lerp(startPoint.position, endPoint.position, currentValue);
            }

            yield return null;
        }

        // Asegurar valores finales
        if (bridgeMaterial != null) bridgeMaterial.SetFloat(extensionProperty, endVal);
        
        // Apagar el emisor de partículas cuando termina de crecer/achicarse
        if (sparkParticles != null) sparkParticles.Stop();

        // Quitar colisión si se apagó
        if (endVal < 0.5f && bridgeCollider != null) bridgeCollider.enabled = false;
    }
}