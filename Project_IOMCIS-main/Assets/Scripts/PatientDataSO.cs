using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPatient", menuName = "Clinic/Patient Data")]
public class PatientDataSO : ScriptableObject
{
    [Header("Informații Generale")]
    public string patientName;
    public int age;

    [TextArea(3, 5)]
    public string medicalHistorySummary;

    [Header("Date de Laborator (Initial)")]
    public float currentHbA1c;
    public int fastingGlucose;
    public bool hasLipodystrophy;
    public bool hasFootSensitivityLoss;

    [Header("Soluția Corectă (Pentru verificare)")]
    public float targetInsulinBasal;
    public string correctDiagnosisNote;

    [Header("Dialog Anamneza (Vechi - 1 răspuns / întrebare)")]
    public List<DialogPereche> listaIntrebari;

    [Header("Dialog Anamneza (Nou - RANDOM)")]
    [Tooltip("Dacă lista asta are elemente, se folosește ea pentru întrebări + răspunsuri random.")]
    public List<DialogIntrebareRandom> dialogRandom;

    [Tooltip("Câte întrebări apar pe butoane la un consult.")]
    public int numarIntrebariPeEcran = 3;
}

[System.Serializable]
public class DialogPereche
{
    public string intrebareJucator;

    [TextArea]
    public string raspunsPacient;
}

[System.Serializable]
public class DialogIntrebareRandom
{
    public string intrebareJucator;

    [TextArea]
    public List<string> raspunsuriPosibile;
}
