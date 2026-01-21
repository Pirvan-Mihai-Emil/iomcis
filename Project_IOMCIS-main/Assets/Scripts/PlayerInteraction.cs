using UnityEngine;
using UnityEngine.UI; // Avem nevoie de asta pentru a controla Ținta (Image)

public class PlayerInteraction : MonoBehaviour
{
    public float distanta = 50f; // Cât de departe poți ajunge cu mâna
    public Image tinta;
    //AAAA
    public ComputerMonitor scriptMonitor;
    public DialogueManager scriptDialog;
    public TreatmentManager scriptTratament;
    void Update()
    {
        Ray raza = new Ray(transform.position, transform.forward);
        RaycastHit obiectLovis;
        if (Physics.Raycast(raza, out obiectLovis, distanta))
        {
            if (obiectLovis.collider.CompareTag("Computer"))
            {
                tinta.color = Color.green;

                if (Input.GetMouseButtonDown(0))
                {
                    scriptMonitor.DeschideMonitor();
                }
            }
            else if (obiectLovis.collider.CompareTag("Patient")) 
            {
                tinta.color = Color.blue; 
                if (Input.GetMouseButtonDown(0))
                {
                    scriptDialog.PornesteDiscutia();
                }
            }
            else if (obiectLovis.collider.CompareTag("Picior"))
            {
                tinta.color = Color.yellow; // Se face galbenă când țintești piciorul

                if (Input.GetMouseButtonDown(0))
                {
                    // Luăm scriptul de pe sfera pe care am dat click
                    FootZone zona = obiectLovis.collider.GetComponent<FootZone>();
                    if (zona != null)
                    {
                        zona.TesteazaZona();
                    }
                }
            }
            else if (obiectLovis.collider.CompareTag("Reteta")) 
            {
                tinta.color = Color.magenta;

                if (Input.GetMouseButtonDown(0))
                {
                    scriptTratament.DeschideReteta();
                }
            }
            else
            {
                tinta.color = Color.red;
            }
        }
        else
        {
            tinta.color = Color.red;
        }
    }
}