using System;
using System.Collections;
using UnityEngine;

public class XAxisRotator : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [Tooltip("Ángulo en el eje X para el Punto A (Suelo)")]
    public float angleA = -20f;
    
    [Tooltip("Ángulo en el eje X para el Punto B (Cabeza)")]
    public float angleB = 75f;
    
    [Tooltip("Duración en segundos de cada trayecto")]
    public float durationPerMove = 2f;

    public bool isScanning { get; private set; } = false;

    private Coroutine currentRoutine;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(angleA, 0f, 0f);
    }

    public void StartRotationSequence()
    {
        if (isScanning) 
        {
            return; 
        }
        
        isScanning = true;
        currentRoutine = StartCoroutine(RotationSequenceRoutine());
    }

    private IEnumerator RotationSequenceRoutine()
    {
        yield return StartCoroutine(RotateXOverTime(angleA, angleB, durationPerMove));

        yield return StartCoroutine(RotateXOverTime(angleB, angleA, durationPerMove));

        currentRoutine = null;
        isScanning = false; 
    }

    private IEnumerator RotateXOverTime(float startAngle, float endAngle, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float currentX = Mathf.LerpAngle(startAngle, endAngle, elapsedTime / duration);
            Vector3 currentEuler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(currentX, currentEuler.y, currentEuler.z);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        Vector3 finalEuler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(endAngle, finalEuler.y, finalEuler.z);
    }
}