using System.Collections;
using System.Collections.Generic;
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
     
        [Header("Onda Expansiva (Shockwave)")]
        public float radioMaximoShockwave = 10f;      
        public float velocidadExpansionShockwave = 15f; 
        public LayerMask capaCajas;
        
        private bool habilidadEnUso = false;

        private void Start()
        {
            if (rendererSuelo != null)
            {
                materialSuelo = rendererSuelo.material;
                materialSuelo.SetFloat("_Step_Time", -1000f);
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void Update()
        {
            if (!Input.GetKeyDown(teclaActivacion)) return;
            {
                if (!habilidadEnUso) EjecutarPisoton();
            }
        }

        private void EjecutarPisoton()
        {
            habilidadEnUso = true;
            animator.SetBool("HaciendoPisoton", true);
            animator.SetTrigger("HacerPisoton");
        }


        public void Evento_FinAnimacion()
        {
            habilidadEnUso = false;
            animator.SetBool("HaciendoPisoton", false);
        }
        public void Evento_PieTocaElSuelo()
        {
            DispararPulsoEnShader();
            StartCoroutine(ExpandirShockwaveCoroutine());
        }
        private void DispararPulsoEnShader()
        {
            if (!materialSuelo) return;
            
            Vector3 posicionImpacto = puntoDeImpacto ? puntoDeImpacto.position : transform.position;

            materialSuelo.SetVector("_Step_Position", posicionImpacto);
            materialSuelo.SetFloat("_Step_Time", Time.time);
        }
        
        
        private IEnumerator ExpandirShockwaveCoroutine()
        {
            float radioActual = 0f;
            Vector3 posicionImpacto = puntoDeImpacto ? puntoDeImpacto.position : transform.position;
            
            HashSet<DestructibleCrate> cajasRompiblesHit = new HashSet<DestructibleCrate>();

            while (radioActual < radioMaximoShockwave)
            {
                radioActual += velocidadExpansionShockwave * Time.deltaTime;

                Collider[] colliders = Physics.OverlapSphere(posicionImpacto, radioActual, capaCajas);

                foreach (Collider col in colliders)
                {
                    DestructibleCrate caja = col.GetComponent<DestructibleCrate>();
                    
                    if (caja && !cajasRompiblesHit.Contains(caja))
                    {
                        cajasRompiblesHit.Add(caja); 
                        caja.BreakCrate();           
                    }
                }

                yield return null; 
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (puntoDeImpacto != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(puntoDeImpacto.position, radioMaximoShockwave);
            }
        }
    }
}