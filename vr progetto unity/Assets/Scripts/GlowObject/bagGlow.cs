using UnityEngine;

public class bagGlow : MonoBehaviour
{
    private Material mat;
    private Color baseColor;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float speed = 2f;

    // Specifichiamo quale oggetto deve far smettere il lampeggio
    [SerializeField] private ItemType stopItem = ItemType.Orologio;

    void Start()
    {
        // Recupera il materiale e abilita l'emissione
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");
        baseColor = mat.GetColor("_EmissionColor");
    }

    void Update()
    {
        // Verifica se l'oggetto è già stato raccolto tramite il NotebookManager
        if (NotebookManager.Instance != null && NotebookManager.Instance.AlreadyPickedUp(stopItem))
        {
            StopGlowing();
            return;
        }

        // Calcolo della pulsazione sinusoidale
        float emission = Mathf.PingPong(Time.time * speed, glowIntensity);
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        mat.SetColor("_EmissionColor", finalColor);
    }

    private void StopGlowing()
    {
        // Spegne l'emissione e disabilita lo script per ottimizzare le prestazioni
        mat.SetColor("_EmissionColor", Color.black);
        this.enabled = false;
    }
}