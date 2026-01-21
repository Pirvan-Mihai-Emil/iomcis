using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SpitalManager : MonoBehaviour
{
    [Header("Configurare Spital")]
    public GameObject pacientPrefab; 
    public Transform punctSpawn;     
    public Transform punctPat;       
    public Transform punctIesire;    
    
    [Header("Componente Generate")]
    public PatientGenerator generatorPacienti;

    [Header("Sisteme de Actualizat")]
    public ComputerMonitor scriptMonitor;
    public TreatmentManager scriptTratament;
    public DialogueManager scriptDialog;
    
    public Button butonConfirmareTratament; 

    [Header("Interfata")]
    public Button butonAsezare; 

    private GameObject pacientCurent;
    private PacientAI scriptMiscare;
    private PatientDataSO dosarCurent;
    private bool seAsteaptaPacient = false;

    void Start()
    {
        if (butonConfirmareTratament != null)
        {
            butonConfirmareTratament.onClick.AddListener(TratamentFinalizat);
        }

        StartCoroutine(SpawnRoutine(1f));
    }

    void Update()
    {
        if (pacientCurent == null && !seAsteaptaPacient)
        {
            StartCoroutine(SpawnRoutine(5f));
        }
    }

    void TratamentFinalizat()
    {
        // Verificăm dacă tratamentul e corect înainte să îl trimitem acasă
        if (scriptTratament != null && scriptTratament.ETratamentCorect())
        {
            Debug.Log("Tratament corect! Pacientul pleacă.");
            StartCoroutine(SecventaPlecare());
        }
        else
        {
            Debug.Log("Tratament greșit. Pacientul mai rămâne.");
        }
    }

    IEnumerator SecventaPlecare()
    {
        yield return new WaitForSeconds(3.5f); 

        if (pacientCurent != null && scriptMiscare != null)
        {
            scriptMiscare.PleacaAcasa();
            
            if(scriptMonitor) scriptMonitor.InchideMonitor();
            if(scriptTratament) scriptTratament.InchidePanou();
        }
    }

    IEnumerator SpawnRoutine(float timpAsteptare)
    {
        seAsteaptaPacient = true;
        yield return new WaitForSeconds(timpAsteptare);

        // 1. Spawn
        pacientCurent = Instantiate(pacientPrefab, punctSpawn.position, punctSpawn.rotation);
         
        scriptMiscare = pacientCurent.GetComponent<PacientAI>();
        scriptMiscare.destinatiePat = punctPat;
        scriptMiscare.destinatieIesire = punctIesire;
        scriptMiscare.animatorPacient = pacientCurent.GetComponentInChildren<Animator>();

        // 2. Generare Date
        if (generatorPacienti != null)
        {
            dosarCurent = generatorPacienti.GenereazaDosar();
            
            if(scriptMonitor) scriptMonitor.datePacient = dosarCurent;
            if(scriptTratament) scriptTratament.datePacient = dosarCurent;
            if(scriptDialog) scriptDialog.datePacient = dosarCurent;

            FootZone[] zonePicioare = pacientCurent.GetComponentsInChildren<FootZone>();
            foreach(FootZone zona in zonePicioare) zona.datePacient = dosarCurent;
        }

        // 3. UI Buton Asezare
        if (butonAsezare != null)
        {
            butonAsezare.onClick.RemoveAllListeners();
            butonAsezare.onClick.AddListener(() => scriptMiscare.MergiLaPat());
        }
        
        seAsteaptaPacient = false;
    }
}