using UnityEngine;

public class DoubleLockDoor : MonoBehaviour
{
    [Header("Estado de las Placas")]
    public bool plate1Active = false;
    public bool plate2Active = false;

    [Header("Referencia a la Puerta")]
    public GameObject door; 
    public Vector3 openPositionOffset = new Vector3(0, 4, 0); // Se mueve 4 metros arriba
    public float speed = 3f;

    private Vector3 closedPos;
    private Vector3 openPos;

    void Start()
    {
        closedPos = door.transform.localPosition;
        openPos = closedPos + openPositionOffset;
    }

    void Update()
    {
        // Solo se abre si AMBAS son true
        Vector3 target = (plate1Active && plate2Active) ? openPos : closedPos;
        door.transform.localPosition = Vector3.Lerp(door.transform.localPosition, target, Time.deltaTime * speed);
    }

    // Funciones que llamarán las placas desde el Inspector
    public void SetPlate1(bool state) => plate1Active = state;
    public void SetPlate2(bool state) => plate2Active = state;
}