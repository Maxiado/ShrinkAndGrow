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
        propertyID = Shader.PropertyToID(propertyName);
        propBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
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
        yield return StartCoroutine(SweepShaderOverTime(startValue, endValue, durationPerSweep));

        yield return StartCoroutine(SweepShaderOverTime(endValue, startValue, durationPerSweep));

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
        SetShaderValue(end);
    }

    /// <summary>
    /// Aplica el valor del shader usando MaterialPropertyBlock.
    /// </summary>
    private void SetShaderValue(float value)
    {
        foreach (SkinnedMeshRenderer renderer in meshRenderers)
        {
            if (renderer)
            {
                renderer.GetPropertyBlock(propBlock);
                propBlock.SetFloat(propertyID, value);
                renderer.SetPropertyBlock(propBlock);
            }
        }
    }
}