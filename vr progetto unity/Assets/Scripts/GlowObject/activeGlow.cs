using UnityEngine;

public class GlowOnUnlock : MonoBehaviour
{
    private Material mat;
    private Color baseColor;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float speed = 2f;

    // Cambiamo il riferimento per puntare allo script di raccolta
    private PickUpItem pickUpScript;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");

        // Salviamo il colore base (nero o spento)
        baseColor = mat.GetColor("_EmissionColor");

        // Cerchiamo il componente PickUpItem che ha la variabile canPickedUp
        pickUpScript = GetComponent<PickUpItem>();

        if (pickUpScript == null)
            Debug.LogWarning($"PickUpItem non trovato su {gameObject.name}.");
    }

    void Update()
    {
        // Controlliamo la variabile canPickedUp (settata in NotebookItems.cs)
        if (pickUpScript == null || !pickUpScript.canPickedUp)
        {
            // Se non è attivo, manteniamo il colore base (spento)
            mat.SetColor("_EmissionColor", Color.black);
            return;
        }

        // Se canPickedUp è true, l'oggetto inizia a lampeggiare
        float emission = Mathf.PingPong(Time.time * speed, glowIntensity);
        Color finalColor = Color.white * Mathf.LinearToGammaSpace(emission); // Usiamo bianco o baseColor per l'intensità
        mat.SetColor("_EmissionColor", finalColor);
    }
}