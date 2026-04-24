using System.Collections;
using UnityEngine;

public class MartianShaderController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí los 5 MeshRenderers del marciano")]
    public SkinnedMeshRenderer[] meshRenderers;

    [Header("Configuración del Shader")]
    [Tooltip("Nombre exacto de la variable en el Shader Graph o código")]
    public string propertyName = "_ScannerPositionY";
    
    [Tooltip("Valor inicial (Suelo)")]
    public float startValue = -1f;
    
    [Tooltip("Valor final (Cabeza)")]
    public float endValue = 6f;
    
    [Tooltip("Duración en segundos de CADA TRAYECTO (debe ser igual a durationPerMove del escáner)")]
    public float durationPerSweep = 2f;
    
    private int propertyID;
    private MaterialPropertyBlock propBlock;
    private Coroutine currentRoutine;

    private void Awake()
    {
        // Cacheamos el ID de la propiedad para mayor rendimiento
        propertyID = Shader.PropertyToID(propertyName);
        propBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        // Aseguramos que el efecto empiece en el suelo
        SetShaderValue(startValue);
    }

    /// <summary>
    /// Función para iniciar el barrido de ida y vuelta del shader.
    /// </summary>
    public void PlayShaderEffect()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(ShaderSequenceRoutine());
    }

    /// <summary>
    /// Corutina principal que maneja la subida y la bajada.
    /// </summary>
    private IEnumerator ShaderSequenceRoutine()
    {
        // 1. El efecto sube del suelo (-1) a la cabeza (6)
        yield return StartCoroutine(SweepShaderOverTime(startValue, endValue, durationPerSweep));

        // 2. El efecto baja de la cabeza (6) al suelo (-1)
        yield return StartCoroutine(SweepShaderOverTime(endValue, startValue, durationPerSweep));

        // 3. Finaliza
        currentRoutine = null;
    }

    /// <summary>
    /// Corutina de apoyo que interpola el valor del shader en el tiempo.
    /// </summary>
    private IEnumerator SweepShaderOverTime(float start, float end, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float currentValue = Mathf.Lerp(start, end, elapsedTime / duration);
            SetShaderValue(currentValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Aseguramos que llegue exactamente al valor de destino
        SetShaderValue(end);
    }

    /// <summary>
    /// Aplica el valor del shader a todos los MeshRenderers usando MaterialPropertyBlock.
    /// </summary>
    private void SetShaderValue(float value)
    {
        foreach (SkinnedMeshRenderer renderer in meshRenderers)
        {
            if (renderer != null)
            {
                renderer.GetPropertyBlock(propBlock);
                propBlock.SetFloat(propertyID, value);
                renderer.SetPropertyBlock(propBlock);
            }
        }
    }
}