using UnityEngine;
using TMPro; 

public class FootZone : MonoBehaviour
{
    [Header("Setări Zonă")]
    public string numeZona = "Degete"; 
    public bool esteAmortita = false;  
    [Header("Referințe")]
    public PatientDataSO datePacient; 
    public TextMeshProUGUI textReactie; 

    private void Start()
    {
        if (datePacient.hasFootSensitivityLoss && numeZona == "Degete")
        {
            esteAmortita = true;
        }
        else
        {
            esteAmortita = false;
        }
    }

    public void TesteazaZona()
    {
        if (esteAmortita)
        {
            textReactie.text = "Pacient: (Nu reacționează)";
            textReactie.color = Color.red;
        }
        else
        {
            textReactie.text = "Pacient: Da, simt!";
            textReactie.color = Color.green;
        }
        Invoke("StergeText", 2f);
    }
    void StergeText()
    {
        textReactie.text = "";
    }
}