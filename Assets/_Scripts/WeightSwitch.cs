using UnityEngine;
using UnityEngine.Events;

public class WeightSwitch : MonoBehaviour
{
    public enum PlateType { HeavyOnly, MicroOnly, AnyWeight }

    [Header("Configuración de Lógica")]
    public PlateType plateType = PlateType.HeavyOnly;
    
    [Header("Ajustes Visuales")]
    public Transform buttonVisual;
    public float sinkAmount = 0.1f;
    public float speed = 5f;

    [Header("Eventos")]
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    private Vector3 upPosition;
    private Vector3 downPosition;
    private Vector3 targetPosition;
    
    private bool isActive = false;
    private bool isLocked = false; 
    private int objectsOnTop = 0; 
    private bool playerIsCounting = false;

    void Start()
    {
        upPosition = buttonVisual.localPosition;
        downPosition = upPosition + (Vector3.down * sinkAmount);
        targetPosition = upPosition;
    }

    public void LockPressed()
    {
        isLocked = true;
        if (!isActive) Activate();
    }

    void Update()
    {
        buttonVisual.localPosition = Vector3.Lerp(buttonVisual.localPosition, targetPosition, Time.deltaTime * speed);
        
        if (isLocked) return;

        if (objectsOnTop > 0 && !isActive) Activate();
        else if (objectsOnTop <= 0 && isActive) Deactivate();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isLocked) return;

        // Los cubos siempre cuentan como "Heavy" o "AnyWeight"
        if (other.CompareTag("Cubo") && plateType != PlateType.MicroOnly)
        {
            objectsOnTop++;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isLocked) return;

        if (other.CompareTag("Player"))
        {
            var pc = other.GetComponent<PlayerScaleController>();
            if (pc != null)
            {
                bool fulfillsCondition = CheckCondition(pc);

                if (fulfillsCondition && !playerIsCounting) {
                    objectsOnTop++;
                    playerIsCounting = true;
                } 
                else if (!fulfillsCondition && playerIsCounting) {
                    objectsOnTop--;
                    playerIsCounting = false;
                }
            }
        }
    }

    bool CheckCondition(PlayerScaleController pc)
    {
        return plateType switch
        {
            PlateType.HeavyOnly => pc.IsTitan,
            PlateType.MicroOnly => pc.IsMicro,
            PlateType.AnyWeight => true,
            _ => false
        };
    }

    void OnTriggerExit(Collider other)
    {
        if (isLocked) return;

        if (other.CompareTag("Cubo") && plateType != PlateType.MicroOnly)
        {
            objectsOnTop--;
        }
        else if (other.CompareTag("Player") && playerIsCounting)
        {
            objectsOnTop--;
            playerIsCounting = false;
        }
    }

    void Activate()
    {
        isActive = true;
        targetPosition = downPosition;
        onActivate.Invoke();
    }

    void Deactivate()
    {
        isActive = false;
        targetPosition = upPosition;
        onDeactivate.Invoke();
    }
}