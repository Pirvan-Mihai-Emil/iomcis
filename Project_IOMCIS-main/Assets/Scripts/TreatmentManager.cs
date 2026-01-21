using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TreatmentManager : MonoBehaviour
{
    [Header("UI Elemente")]
    public GameObject panouTratament;
    public GameObject grupControale; 
    
    public Slider sliderInsulina;
    public TextMeshProUGUI textDoza;
    public TextMeshProUGUI textRezultat;
    public Button butonConfirmare;

    [Header("Date")]
    public PatientDataSO datePacient;

    void Start()
    {
        panouTratament.SetActive(false);
        textRezultat.gameObject.SetActive(false); 
        sliderInsulina.minValue = 0;
        sliderInsulina.maxValue = 60;
        sliderInsulina.onValueChanged.AddListener(ActualizeazaTextDoza);
    }

    void ActualizeazaTextDoza(float valoare)
    {
        textDoza.text = "Doza: " + valoare.ToString("0") + " Unități";
    }

    public void DeschideReteta()
    {
        panouTratament.SetActive(true);
        grupControale.SetActive(true); 
        textRezultat.gameObject.SetActive(false); 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void VerificaRezultatul()
    {
        StartCoroutine(AfiseazaRezultatCuIntarziere());
    }

    IEnumerator AfiseazaRezultatCuIntarziere()
    {
        grupControale.SetActive(false); // Blocăm butoanele
        
        float dozaAleasa = sliderInsulina.value;
        float dozaCorecta = (datePacient != null) ? datePacient.targetInsulinBasal : 0;
        float diferenta = Mathf.Abs(dozaAleasa - dozaCorecta);

        textRezultat.gameObject.SetActive(true);

        if (diferenta <= 2)
        {
            textRezultat.text = "EXCELENT! Doza este corectă.";
            textRezultat.color = Color.green;
        }
        else if (dozaAleasa < dozaCorecta)
        {
            textRezultat.text = "PREA PUȚIN! Risc de hiperglicemie.";
            textRezultat.color = Color.red;
        }
        else
        {
            textRezultat.text = "PREA MULT! Risc de hipoglicemie.";
            textRezultat.color = Color.red;
        }

        yield return new WaitForSeconds(3f);

        textRezultat.gameObject.SetActive(false); 
        grupControale.SetActive(true); // Deblocăm butoanele
    }

    public void InchidePanou()
    {
        panouTratament.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- FUNCTIA NOUA CARE LIPSEA ---
    public bool ETratamentCorect()
    {
        if (datePacient == null) return false;
        
        float dozaAleasa = sliderInsulina.value;
        return Mathf.Abs(dozaAleasa - datePacient.targetInsulinBasal) <= 2;
    }
}