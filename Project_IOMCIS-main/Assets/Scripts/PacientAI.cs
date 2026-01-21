using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Necesar pentru Coroutine (Așteptare)

public class PacientAI : MonoBehaviour
{
    [Header("Destinații")]
    public Transform destinatiePat;
    public Transform destinatieIesire;

    [Header("Componente")]
    public Animator animatorPacient;
    
    private NavMeshAgent agent;
    private bool ePePiciorDePlecare = false;
    private bool seMisca = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;

        if (animatorPacient)
        {
            animatorPacient.Play("Breathing Idle");
            animatorPacient.ResetTrigger("Sit"); 
            animatorPacient.ResetTrigger("StandUp"); // Resetăm și asta
            animatorPacient.SetBool("IsWalking", false);
        }
    }

    public void MergiLaPat()
    {
        if (destinatiePat != null)
        {
            agent.isStopped = false;
            agent.SetDestination(destinatiePat.position);
            
            if(animatorPacient) animatorPacient.SetBool("IsWalking", true);
            
            seMisca = true;
            ePePiciorDePlecare = false;
        }
    }

    // --- MODIFICARE AICI: Folosim o Coroutine pentru secvența de plecare ---
    public void PleacaAcasa()
    {
        StartCoroutine(SecventaRidicareSiPlecare());
    }

    IEnumerator SecventaRidicareSiPlecare()
    {
        // 1. Declasăm animația de ridicare
        if(animatorPacient)
        {
            animatorPacient.ResetTrigger("Sit"); // Siguranță
            animatorPacient.SetTrigger("StandUp"); // RIDICĂ-TE!
        }

        // 2. Așteptăm să se termine animația (aprox 2.5 secunde, depinde de animația ta)
        // Poți ajusta numărul acesta dacă se mișcă prea repede sau prea târziu
        yield return new WaitForSeconds(2.5f); 

        // 3. Abia ACUM începem să ne mișcăm spre ușă
        if (destinatieIesire != null)
        {
            agent.isStopped = false;
            agent.SetDestination(destinatieIesire.position);
            
            if(animatorPacient) animatorPacient.SetBool("IsWalking", true);

            ePePiciorDePlecare = true;
            seMisca = true;
        }
    }
    // -----------------------------------------------------------------------

    void Update()
    {
        if (seMisca == false) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                seMisca = false;

                if (!ePePiciorDePlecare) 
                {
                    // A ajuns la PAT
                    agent.isStopped = true;
                    if(animatorPacient)
                    {
                        animatorPacient.SetBool("IsWalking", false);
                        animatorPacient.SetTrigger("Sit");
                    }
                    transform.rotation = destinatiePat.rotation;
                }
                else 
                {
                    // A ajuns la UȘĂ
                    Destroy(gameObject);
                }
            }
        }
    }
}