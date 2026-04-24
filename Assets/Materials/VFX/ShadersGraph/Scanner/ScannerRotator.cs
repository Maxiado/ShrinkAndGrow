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

    // Variable pública para leer, privada para modificar. Indica si el escáner está activo.
    public bool isScanning { get; private set; } = false;

    private Coroutine currentRoutine;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(angleA, 0f, 0f);
    }

    /// <summary>
    /// Función pública accesible desde cualquier otra clase para iniciar la secuencia.
    /// </summary>
    public void StartRotationSequence()
    {
        // VALIDACIÓN: Si ya está escaneando, ignoramos la orden y salimos de la función
        if (isScanning) 
        {
            return; 
        }
        
        // Bloqueamos el estado para que no se pueda volver a llamar
        isScanning = true;
        currentRoutine = StartCoroutine(RotationSequenceRoutine());
    }

    /// <summary>
    /// Corutina principal que maneja el orden de los movimientos.
    /// </summary>
    private IEnumerator RotationSequenceRoutine()
    {
        // 1. Empieza del Punto A (Suelo) al Punto B (Cabeza)
        yield return StartCoroutine(RotateXOverTime(angleA, angleB, durationPerMove));

        // 2. Del Punto B (Cabeza) al Punto A (Suelo)
        yield return StartCoroutine(RotateXOverTime(angleB, angleA, durationPerMove));

        // 3. Finaliza y liberamos el escáner para que pueda usarse de nuevo
        currentRoutine = null;
        isScanning = false; 
        Debug.Log("Secuencia de escaneo en X finalizada.");
    }

    /// <summary>
    /// Corutina de apoyo que rota el objeto suavemente entre dos ángulos en el eje X.
    /// </summary>
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