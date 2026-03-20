using UnityEngine;
using Cinemachine;

public class TitanFootsteps : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;
    private PlayerScaleController scaleController;

    void Start()
    {
        // Buscamos el Impulse Source en el padre (la cápsula)
        impulseSource = GetComponentInParent<CinemachineImpulseSource>();
        scaleController = GetComponentInParent<PlayerScaleController>();
    }

    // Esta función la llamaremos desde un Evento de Animación en Unity
    public void OnFootstep()
    {
        if (scaleController != null && scaleController.currentSize == PlayerScaleController.PlayerSize.Titan)
        {
            if (impulseSource != null)
            {
                // Dispara el temblor de cámara
                impulseSource.GenerateImpulseWithForce(0.5f); // Ajusta la fuerza aquí
                
                // Opcional: Aquí puedes poner un sonido de "PUM" de paso pesado
                // AudioSource.PlayClipAtPoint(sonidoPasoTitan, transform.position);
            }
        }
    }
}