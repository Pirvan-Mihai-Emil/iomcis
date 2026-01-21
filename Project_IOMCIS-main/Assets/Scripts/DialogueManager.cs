using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elemente")]
    public GameObject panouDialog;
    public TextMeshProUGUI textRaspuns;
    public Button[] butoaneIntrebari;

    [Header("Date")]
    public PatientDataSO datePacient;

    [Header("Setare FIXA pentru butonul 3")]
    [Tooltip("Textul întrebării care este MEREU pe butonul 3 și îl trimite la pat.")]
    public string intrebareTrimiteLaPat = "Vă rog să vă așezați pe pat";

    [Tooltip("Replica pacientului când apeși butonul 3.")]
    public string raspunsTrimiteLaPat = "Sigur, imediat.";

    // mapare: buton -> index întrebare selectată (pentru butoanele random/vechi)
    private int[] mapareButonLaIntrebare;
    private bool folosesteRandom = false;

    private const int INDEX_BUTON_PAT = 2; // butonul 3 => index 2

    void Start()
    {
        if (panouDialog != null)
            panouDialog.SetActive(false);

        if (butoaneIntrebari == null)
            butoaneIntrebari = new Button[0];

        mapareButonLaIntrebare = new int[butoaneIntrebari.Length];

        // Inițializare sigură pentru toate butoanele
        for (int i = 0; i < butoaneIntrebari.Length; i++)
        {
            if (butoaneIntrebari[i] == null) continue;

            butoaneIntrebari[i].onClick.RemoveAllListeners();
            int captured = i;
            butoaneIntrebari[i].onClick.AddListener(() => AlegeIntrebarea(captured));
        }
    }

    public void PornesteDiscutia()
    {
        if (panouDialog == null || textRaspuns == null || butoaneIntrebari == null || butoaneIntrebari.Length == 0)
        {
            Debug.LogError("DialogueManager: lipsește referință în Inspector (panouDialog / textRaspuns / butoaneIntrebari).");
            return;
        }

        if (datePacient == null)
        {
            Debug.LogError("DialogueManager: datePacient (PatientDataSO) NU este setat.");
            return;
        }

        panouDialog.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        textRaspuns.text = "Bună ziua, domnule doctor. Cu ce vă pot ajuta?";

        // decidem sursa de dialog
        folosesteRandom = (datePacient.dialogRandom != null && datePacient.dialogRandom.Count > 0);

        if (folosesteRandom)
            ConfigureazaButoaneRandom();
        else
            ConfigureazaButoaneVechi();

        // IMPORTANT: suprascriem mereu BUTONUL 3
        SeteazaButonul3Fix();
    }

    private void SeteazaButonul3Fix()
    {
        if (butoaneIntrebari.Length <= INDEX_BUTON_PAT || butoaneIntrebari[INDEX_BUTON_PAT] == null)
            return;

        Button b3 = butoaneIntrebari[INDEX_BUTON_PAT];
        b3.gameObject.SetActive(true);

        var tmp = b3.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = intrebareTrimiteLaPat;

        // maparea nu contează aici, fiind acțiune specială
        mapareButonLaIntrebare[INDEX_BUTON_PAT] = -999;
    }

    private bool EIntrebareaDePat(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        return s.Trim().ToLower() == intrebareTrimiteLaPat.Trim().ToLower();
    }

    private void ConfigureazaButoaneVechi()
    {
        // Vechi = datePacient.listaIntrebari
        // BUTONUL 3 îl setăm separat, deci aici configurăm doar celelalte.

        if (datePacient.listaIntrebari == null || datePacient.listaIntrebari.Count == 0)
        {
            Debug.LogWarning("DialogueManager: listaIntrebari este goală.");
            for (int i = 0; i < butoaneIntrebari.Length; i++)
            {
                if (butoaneIntrebari[i] == null) continue;
                if (i == INDEX_BUTON_PAT) continue; // buton 3 rămâne
                butoaneIntrebari[i].gameObject.SetActive(false);
            }
            return;
        }

        int idxLista = 0;

        for (int b = 0; b < butoaneIntrebari.Length; b++)
        {
            if (butoaneIntrebari[b] == null) continue;

            // buton 3 îl lăsăm pentru fix
            if (b == INDEX_BUTON_PAT) continue;

            // căutăm următoarea întrebare care NU e cea de pat
            while (idxLista < datePacient.listaIntrebari.Count &&
                   EIntrebareaDePat(datePacient.listaIntrebari[idxLista].intrebareJucator))
            {
                idxLista++;
            }

            if (idxLista < datePacient.listaIntrebari.Count)
            {
                butoaneIntrebari[b].gameObject.SetActive(true);

                var tmp = butoaneIntrebari[b].GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null) tmp.text = datePacient.listaIntrebari[idxLista].intrebareJucator;

                mapareButonLaIntrebare[b] = idxLista;
                idxLista++;
            }
            else
            {
                butoaneIntrebari[b].gameObject.SetActive(false);
            }
        }
    }

    private void ConfigureazaButoaneRandom()
    {
        int total = datePacient.dialogRandom.Count;

        // câte întrebări random vrem pe ecran (dar lăsăm butonul 3 pentru pat)
        int cateDorim = Mathf.Clamp(datePacient.numarIntrebariPeEcran, 1, butoaneIntrebari.Length);

        // listă de indici valizi (fără întrebarea de pat)
        List<int> indici = new List<int>(total);
        for (int i = 0; i < total; i++)
        {
            var q = datePacient.dialogRandom[i].intrebareJucator;
            if (!EIntrebareaDePat(q))
                indici.Add(i);
        }

        // shuffle simplu
        for (int i = 0; i < indici.Count; i++)
        {
            int j = Random.Range(i, indici.Count);
            int temp = indici[i];
            indici[i] = indici[j];
            indici[j] = temp;
        }

        int pick = 0;

        for (int b = 0; b < butoaneIntrebari.Length; b++)
        {
            if (butoaneIntrebari[b] == null) continue;

            // buton 3 e rezervat
            if (b == INDEX_BUTON_PAT) continue;

            if (pick < cateDorim && pick < indici.Count)
            {
                int indexIntrebare = indici[pick];
                mapareButonLaIntrebare[b] = indexIntrebare;

                var intrebare = datePacient.dialogRandom[indexIntrebare].intrebareJucator;

                butoaneIntrebari[b].gameObject.SetActive(true);
                var tmp = butoaneIntrebari[b].GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null) tmp.text = intrebare;

                pick++;
            }
            else
            {
                butoaneIntrebari[b].gameObject.SetActive(false);
            }
        }
    }

    // index primit = index BUTON
    public void AlegeIntrebarea(int buttonIndex)
    {
        if (datePacient == null) return;
        if (buttonIndex < 0 || buttonIndex >= butoaneIntrebari.Length) return;

        // ✅ BUTONUL 3: trimite la pat (singurul care mută pacientul)
        if (buttonIndex == INDEX_BUTON_PAT)
        {
            textRaspuns.text = raspunsTrimiteLaPat;

            // găsim pacientul curent și îl trimitem la pat
            PacientAI pacient = FindObjectOfType<PacientAI>();
            if (pacient == null)
            {
                Debug.LogError("DialogueManager: NU găsesc PacientAI în scenă. Nu pot trimite pacientul la pat.");
                return;
            }

            if (pacient.destinatiePat == null)
            {
                Debug.LogError("PacientAI: destinatiePat NU este setată în Inspector (pe pacient sau via SpitalManager).");
                return;
            }

            pacient.MergiLaPat();
            return;
        }

        // ✅ Restul butoanelor: doar răspunsuri
        if (mapareButonLaIntrebare == null || buttonIndex >= mapareButonLaIntrebare.Length) return;

        int indexIntrebare = mapareButonLaIntrebare[buttonIndex];
        if (indexIntrebare < 0) return;

        if (folosesteRandom)
        {
            if (datePacient.dialogRandom == null || indexIntrebare >= datePacient.dialogRandom.Count) return;

            var item = datePacient.dialogRandom[indexIntrebare];
            if (item.raspunsuriPosibile == null || item.raspunsuriPosibile.Count == 0)
            {
                textRaspuns.text = "(Pacientul nu are răspunsuri setate pentru această întrebare.)";
                return;
            }

            int r = Random.Range(0, item.raspunsuriPosibile.Count);
            textRaspuns.text = item.raspunsuriPosibile[r];
        }
        else
        {
            if (datePacient.listaIntrebari == null || indexIntrebare >= datePacient.listaIntrebari.Count) return;
            textRaspuns.text = datePacient.listaIntrebari[indexIntrebare].raspunsPacient;
        }
    }

    public void InchideDialog()
    {
        if (panouDialog != null)
            panouDialog.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
