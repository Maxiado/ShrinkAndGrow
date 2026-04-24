using System;
using UnityEngine;

public class Pickable : MonoBehaviour, IPickable
{
    [SerializeField] private PickableType pickableType;
    private Rigidbody rb;
    private Collider col; // Agregamos una referencia al Collider
    public Collider colChild; // Agregamos una referencia al Collider

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>(); // Buscamos el Collider al inicio
    }

    public PickableType GetPickableType()
    {
        return pickableType;
    }

    public void PickUp()
    {
        rb.isKinematic = true;
        
        // APAGAMOS LA COLISIÓN: Así no empuja al jugador
        if (col) col.enabled = false; 
        if (colChild) colChild.enabled = false; 
        
        transform.SetParent(GameManager.Instance.playerHoldPos);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void ReleaseObject()
    {
        rb.isKinematic = false;
        
        // ENCENDEMOS LA COLISIÓN: Para que choque con el piso al caer
        if (col) col.enabled = true; 
        if (colChild) colChild.enabled = true; 

        transform.SetParent(null);
    }
}