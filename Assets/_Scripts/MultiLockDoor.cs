using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class MultiLockDoor : MonoBehaviour
{
    [Header("Configuración de Cerraduras")]
    public bool[] lockStates;
    public bool isSticky = true;

    [Header("Evento de Victoria")]
    [Tooltip("Aquí arrastra los botones (LockPressed) y luces (SetState true)")]
    public UnityEvent onPuzzleSolved;

    [Header("Referencias de Movimiento")]
    public Transform doorTransform; 
    public Vector3 openOffset = new Vector3(0, 4, 0);
    public float speed = 3f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool wasFullyUnlocked = false;

    void Start()
    {
        if (doorTransform == null) doorTransform = transform;
        closedPos = doorTransform.localPosition;
        openPos = closedPos + openOffset;
    }

    void Update()
    {
        // Verificamos si todos los estados en el array son true
        bool allActive = lockStates.All(state => state == true);

        // Si se activa todo por primera vez, disparamos la victoria
        if (allActive && !wasFullyUnlocked)
        {
            wasFullyUnlocked = true;
            onPuzzleSolved?.Invoke(); // Avisa a botones y luces que se bloqueen
            Debug.Log("<color=green>Puzle Resuelto: Bloqueando mecanismos.</color>");
        }

        // Determinar posición de la puerta
        bool shouldBeOpen = isSticky ? wasFullyUnlocked : allActive;
        Vector3 target = shouldBeOpen ? openPos : closedPos;

        doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, target, Time.deltaTime * speed);
    }

    // Métodos para el Inspector (UnityEvents de las placas)
    public void ActivateLock(int index) => SetLockValue(index, true);
    public void DeactivateLock(int index) => SetLockValue(index, false);

    private void SetLockValue(int index, bool state)
    {
        if (wasFullyUnlocked && isSticky) return; // Si ya ganamos, no aceptamos cambios

        if (index >= 0 && index < lockStates.Length)
        {
            lockStates[index] = state;
        }
    }
}