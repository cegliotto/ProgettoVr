using UnityEngine;

public class GlowUntilSolved : MonoBehaviour
{
    private Material[] mats;
    private Color[] baseColors;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float speed = 2f;

    [SerializeField] private string puzzleID;
    [SerializeField] private PuzzleInteraction puzzleInteraction;

    private static System.Collections.Generic.HashSet<string> completedPuzzles = new System.Collections.Generic.HashSet<string>();
    private bool isStopped = false; // Flag per evitare sovrascritture dopo lo stop

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        mats = renderer.materials;
        baseColors = new Color[mats.Length];

        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].EnableKeyword("_EMISSION");
            // si salva il colore di emissione iniziale (quello spento o di default)
            baseColors[i] = mats[i].GetColor("_EmissionColor");
        }

        if (puzzleInteraction == null)
        {
            puzzleInteraction = GetComponentInParent<PuzzleInteraction>();
        }

        if (completedPuzzles.Contains(puzzleID))
        {
            StopGlow();
        }
    }

    void Update()
    {
        // Se è già stato risolto o fermato, non si fa niente
        if (isStopped || completedPuzzles.Contains(puzzleID)) return;

        if (puzzleInteraction != null && puzzleInteraction.solved)
        {
            completedPuzzles.Add(puzzleID);
            StopGlow();
            return;
        }

        // si somma l'emissione al colore base invece di moltiplicare solo
        float emission = Mathf.PingPong(Time.time * speed, glowIntensity);

        for (int i = 0; i < mats.Length; i++)
        {
            //  ColoreBase + (Bianco * intensità)
            // quando l'intensità è 0 si torna al baseColor
            Color finalColor = baseColors[i] + (Color.white * Mathf.LinearToGammaSpace(emission));
            mats[i].SetColor("_EmissionColor", finalColor);
        }
    }

    private void StopGlow()
    {
        isStopped = true;
        if (mats == null) return;

        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i] != null)
            {
                // Forziamo il ritorno al colore salvato in Start
                mats[i].SetColor("_EmissionColor", baseColors[i]);
            }
        }
    }
}