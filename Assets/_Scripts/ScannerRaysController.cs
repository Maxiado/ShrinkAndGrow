using System.Collections;
using UnityEngine;

public class ScannerRaysController : MonoBehaviour
{
    private static readonly int Color1 = Shader.PropertyToID("_Color");

    [Header("Referencias")]
    public MeshRenderer raysRenderer;

    [Header("Configuración de Color")]
    [Tooltip("El nombre 'Reference' del color en tu Shader Graph")]
    public string colorPropertyName = "_Color";
    
    public Color normalRed;
    public Color successGreen = Color.green;
    public Color successYellow = Color.yellow;

    private int propID;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        propID = Shader.PropertyToID(colorPropertyName);
        propBlock = new MaterialPropertyBlock();
    }
    public void FlashSuccess()
    {
        StartCoroutine(FlashRoutine(successGreen));
    }

    public void FlashFailed()
    {
        StartCoroutine(FlashRoutine(successYellow));
    }
    private IEnumerator FlashRoutine(Color color)
    {
        
        for (int i = 0; i < 3; i++)
        {
            SetColor(color * 5f); // Verde intenso HDR
            yield return new WaitForSeconds(0.2f);
            SetColor(Color.black); // Opcional: un parpadeo apagado
            yield return new WaitForSeconds(0.1f);
        }
        if(color == Color.green) SetColor(successGreen * 5);
        else SetColor(normalRed * 5); 
    }

    private void SetColor(Color col)
    {
        if (!raysRenderer) return;
        raysRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(propID, col);
        raysRenderer.SetPropertyBlock(propBlock);
    }
}