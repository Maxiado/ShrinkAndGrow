using UnityEngine;

namespace Materials.VFX.ShadersGraph.Floor
{
    public class GroundSensor : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public LayerMask groundMask;

        [HideInInspector] public Collider currentGroundedCollider;

        void OnTriggerStay(Collider other)
        {
            if ((groundMask.value & (1 << other.gameObject.layer)) > 0)
            {
                playerMovement.SetGroundedState(true);
            
                currentGroundedCollider = other; 
            }
        }

        void OnTriggerExit(Collider other)
        {
            if ((groundMask.value & (1 << other.gameObject.layer)) > 0)
            {
                playerMovement.SetGroundedState(false);
            
                if (currentGroundedCollider == other)
                {
                    currentGroundedCollider = null;
                }
            }
        }
    }
}