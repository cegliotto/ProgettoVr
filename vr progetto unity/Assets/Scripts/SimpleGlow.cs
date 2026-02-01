using UnityEngine;

public class SimpleGlow : MonoBehaviour
{
    private Material mat;
    private Color baseColor;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float speed = 2f;

    // Identificativo unico per oggetto 
    [SerializeField] private string puzzleID;

    //  variabile statica che ricorda quali puzzle sono stati completati
    private static System.Collections.Generic.HashSet<string> completedPuzzles = new System.Collections.Generic.HashSet<string>();

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");
        baseColor = mat.GetColor("_EmissionColor");

        // Al riavvio della scena, controlla se questo ID era già stato completato
        if (completedPuzzles.Contains(puzzleID))
        {
            StopGlowingPermanently();
        }
    }

    void Update()
    {
        // 1. Controllo: se è completato, esci subito.
        if (completedPuzzles.Contains(puzzleID))
        {
            return; // BLOCCA il lampeggio qui.
        }

        // 2. Esecuzione: questo codice gira SOLO se non è completato.
        float emission = Mathf.PingPong(Time.time * speed, glowIntensity);
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        mat.SetColor("_EmissionColor", finalColor);
    }

    public void SelectObject()
    {
        if (!completedPuzzles.Contains(puzzleID))
        {
            completedPuzzles.Add(puzzleID);
            // IMPORTANTE: Spegni l'emissione un'ultima volta 
            // così non rimane "bloccato" sulla luce forte.
            mat.SetColor("_EmissionColor", baseColor);
        }
    }

    private void StopGlowingPermanently()
    {
        if (mat != null) mat.SetColor("_EmissionColor", baseColor);
    }
}