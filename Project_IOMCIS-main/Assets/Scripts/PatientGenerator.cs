using UnityEngine;
using System.Collections.Generic; // Pentru Liste

public class PatientGenerator : MonoBehaviour
{
    // Liste de nume pentru diversitate
    private string[] numeBarbati = { "Adrian Popa", "Ion Stoica", "Vasile Munteanu", "Gheorghe Marin", "Mihai Radu", "Alexandru Stan" };
    // Poți adăuga și nume de femei dacă ai modele 3D feminine

    // Funcția care creează un ScriptableObject nou în memorie
    public PatientDataSO GenereazaDosar()
    {
        // 1. Creăm instanța în memorie (Magic!)
        PatientDataSO dosarNou = ScriptableObject.CreateInstance<PatientDataSO>();

        // 2. Generăm datele Generale
        dosarNou.patientName = numeBarbati[Random.Range(0, numeBarbati.Length)];
        dosarNou.age = Random.Range(20, 85);
        
        // Generăm istoricul medical (text simplu)
        string[] boli = { "Hipertensiune", "Fumător", "Sedentar", "Istoric cardiac", "Colesterol mărit" };
        dosarNou.medicalHistorySummary = "Pacient cunoscut cu: " + boli[Random.Range(0, boli.Length)];

        // 3. Generăm datele Medicale (Diabet)
        // Glicemie mare (între 150 și 400)
        dosarNou.fastingGlucose = Random.Range(150, 400); 
        
        // HbA1c (între 7.0 și 12.0)
        dosarNou.currentHbA1c = Random.Range(7.0f, 12.0f);
        
        // Simptome fizice (Random true/false)
        dosarNou.hasLipodystrophy = (Random.value > 0.8f); // 20% șanse
        dosarNou.hasFootSensitivityLoss = (Random.value > 0.7f); // 30% șanse

        // 4. Calculăm Tratamentul CORECT (Logica jocului)
        // Formula simplă: (Glicemie - 100) / 10. Ex: 300 glicemie -> (200)/10 = 20 unități
        float necesar = (dosarNou.fastingGlucose - 100) / 10.0f;
        dosarNou.targetInsulinBasal = Mathf.Round(necesar); 
        if (dosarNou.targetInsulinBasal < 0) dosarNou.targetInsulinBasal = 0;

        // 5. Generăm Dialogul (Opțional - momentan lăsăm gol sau standard)
        dosarNou.listaIntrebari = new List<DialogPereche>();
        // Aici am putea adăuga întrebări standard, dar e mai complex. 
        // Putem refolosi o listă de întrebări "template" dacă vrei.
        dosarNou.listaIntrebari = new List<DialogPereche>();

        // Întrebarea 1
        DialogPereche q1 = new DialogPereche();
        q1.intrebareJucator = "Ați mâncat dulciuri?";
        // Răspuns dependent de glicemie (logică simplă)
        if (dosarNou.fastingGlucose > 200) 
        q1.raspunsPacient = "Recunosc, am mâncat o prăjitură aseară...";
        else 
        q1.raspunsPacient = "Nu, domnule doctor, țin regim strict.";
        dosarNou.listaIntrebari.Add(q1);

        // Întrebarea 2
        DialogPereche q2 = new DialogPereche();
        q2.intrebareJucator = "Vă simțiți rău?";
        q2.raspunsPacient = "Uneori amețesc puțin.";
        dosarNou.listaIntrebari.Add(q2);
            
        // Întrebarea 3 - Comanda de așezare (OBLIGATORIE pentru butonul de așezare)
        DialogPereche q3 = new DialogPereche();
        q3.intrebareJucator = "Vă rog să vă așezați pe pat";
        q3.raspunsPacient = "Sigur, imediat.";
        dosarNou.listaIntrebari.Add(q3);
        return dosarNou;
    }
}