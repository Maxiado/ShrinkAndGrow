using UnityEngine;

namespace _Scripts
{
    public class HabilidadPisotonTitan : MonoBehaviour
    {
        [Header("Componentes")] public Animator animator;
        public MeshRenderer rendererSuelo;
        private Material materialSuelo;

        [Tooltip("El objeto vacío colocado donde el pie choca con el suelo")]
        public Transform puntoDeImpacto;

        [Header("Configuración")] public KeyCode teclaActivacion = KeyCode.R;

        // Candado para no repetir la animación antes de que termine
        private bool habilidadEnUso = false;

        private void Start()
        {
            if (rendererSuelo != null)
            {
                materialSuelo = rendererSuelo.material;
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void Update()
        {
            // Solo activamos si presionas R y no está ya pisando
            if (!Input.GetKeyDown(teclaActivacion)) return;
            {
                if (!habilidadEnUso) EjecutarPisoton();
            }
        }

        private void EjecutarPisoton()
        {
            habilidadEnUso = true;

            // Le decimos al Animator que bloquee el "Any State"
            animator.SetBool("HaciendoPisoton", true);

            // Disparamos la transición hacia la animación
            animator.SetTrigger("HacerPisoton");
        }


        public void Evento_FinAnimacion()
        {
            // Apagamos el candado de la habilidad
            habilidadEnUso = false;

            // Le decimos al Animator que "Any State" ya puede volver a funcionar
            animator.SetBool("HaciendoPisoton", false);
        }

        // --- EVENTOS DE ANIMACIÓN (Los llama Unity, no el Update) ---

        // 1. Pon este evento en la línea de tiempo EXACTAMENTE cuando el zapato choca con el piso
        public void Evento_PieTocaElSuelo()
        {
            DispararPulsoEnShader();
        }


        private void DispararPulsoEnShader()
        {
            if (!materialSuelo) return;
            
            Vector3 posicionImpacto = puntoDeImpacto ? puntoDeImpacto.position : transform.position;

            materialSuelo.SetVector("_Step_Position", posicionImpacto);
            materialSuelo.SetFloat("_Step_Time", Time.time);
        }
    }
}