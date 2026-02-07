using UnityEngine;

public class GlowUntilSolved : MonoBehaviour
{
    private Material mat;
    private Color baseColor;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float speed = 2f;

    [SerializeField] private string puzzleID; // ID unico per la persistenza
    private PuzzleInteraction puzzleInteraction;

    // Registro statico per ricordare i puzzle risolti tra le scene
    private static System.Collections.Generic.HashSet<string> completedPuzzles = new System.Collections.Generic.HashSet<string>();

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");
        baseColor = mat.GetColor("_EmissionColor");

        puzzleInteraction = GetComponent<PuzzleInteraction>();

        // Se il puzzle risulta già risolto nel registro statico, spegni subito
        if (completedPuzzles.Contains(puzzleID))
        {
            StopGlow();
        }
    }

    void Update()
    {
        if (completedPuzzles.Contains(puzzleID)) return;

        // Se il componente PuzzleInteraction rileva che il puzzle è stato risolto
        if (puzzleInteraction != null && puzzleInteraction.solved)
        {
            completedPuzzles.Add(puzzleID); // Salva lo stato risolto
            StopGlow();
            return;
        }

        // Lampeggio standard finché non è risolto
        float emission = Mathf.PingPong(Time.time * speed, glowIntensity);
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        mat.SetColor("_EmissionColor", finalColor);
    }

    private void StopGlow()
    {
        if (mat != null) mat.SetColor("_EmissionColor", baseColor);
    }
}