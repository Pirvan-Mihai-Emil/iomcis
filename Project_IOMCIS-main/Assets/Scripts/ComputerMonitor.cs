using UnityEngine;
using TMPro;

public class ComputerMonitor : MonoBehaviour
{
    [Header("Elemente UI")]
    public GameObject panouCalculator;
    public TextMeshProUGUI textNume; 
    public TextMeshProUGUI textIstoric;
    public TextMeshProUGUI textHbA1c;

    [Header("Datele Pacientului")]
    public PatientDataSO datePacient;
    public void DeschideMonitor()
    {
        textNume.text = "Pacient: " + datePacient.patientName + " (" + datePacient.age + " ani)";
        textIstoric.text = "Istoric: " + datePacient.medicalHistorySummary;
        panouCalculator.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void InchideMonitor()
    {
        panouCalculator.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}