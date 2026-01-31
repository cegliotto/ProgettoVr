using UnityEngine;

public class SimpleGlow : MonoBehaviour
{
    private Material mat;
    private Color baseColor;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float speed = 2f;

    void Start()
    {
        // Prende il materiale dell'oggetto
        mat = GetComponent<Renderer>().material;
        // Abilita l'emissione se non è attiva
        mat.EnableKeyword("_EMISSION");
        baseColor = mat.GetColor("_EmissionColor");
    }

    void Update()
    {
        // Calcola una pulsazione sinusoidale
        float emission = Mathf.PingPong(Time.time * speed, glowIntensity);
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        mat.SetColor("_EmissionColor", finalColor);
    }
}