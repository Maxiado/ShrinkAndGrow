using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public LayerMask groundMask;

    // ¡NUEVA VARIABLE! Para guardar la información del suelo que pisamos
    [HideInInspector] public Collider currentGroundedCollider;

    void OnTriggerStay(Collider other)
    {
        if ((groundMask.value & (1 << other.gameObject.layer)) > 0)
        {
            playerMovement.SetGroundedState(true);
            
            // ¡NUEVO! Guardamos el collider del objeto que estamos pisando
            currentGroundedCollider = other; 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((groundMask.value & (1 << other.gameObject.layer)) > 0)
        {
            playerMovement.SetGroundedState(false);
            
            // ¡NUEVO! Al salir, reseteamos la referencia
            if (currentGroundedCollider == other)
            {
                currentGroundedCollider = null;
            }
        }
    }
}