using UnityEngine;


// Rappresenta una "zona pin" astratta della serratura.
// Quando il rake entra in questa zona, il puzzle controller
// verifica se la tensione è corretta per settare il pin.
public class LockPinZone : MonoBehaviour
{
    [Header("Pin Settings")]
    public int pinIndex = 1;    // Indice del pin (1..4).

    [Range(0f, 1f)]              // Valore minimo di tensione (normalizzato 0..1)
    public float minTension = 0.25f; 

    [Range(0f, 1f)]              // Valore massimo di tensione (normalizzato 0..1)
    public float maxTension = 0.45f;

    [Header("Optional Visual Feedback")]

    // Renderer opzionale da usare per dare un feedback visivo
    // quando il pin viene correttamente settato.
    // Può essere una mesh nascosta, un dettaglio emissivo, ecc.
    public Renderer feedbackRenderer;

    // Materiale temporaneo usato per il flash visivo del pin.
    // Se non assegnato, il feedback visivo viene semplicemente ignorato.
    public Material flashMaterial;

    // Materiale originale del renderer, salvato per ripristinarlo dopo il flash
    private Material _originalMat;

    
    // Ritorna true se la tensione corrente rientra
    // nell'intervallo valido per questo pin.
    public bool IsTensionInRange(float tension)
    {
        return tension >= minTension && tension <= maxTension;
    }

    private void Awake()
    {
        // Salva il materiale originale per poterlo ripristinare
        // dopo il feedback visivo
        if (feedbackRenderer != null)
            _originalMat = feedbackRenderer.sharedMaterial;
    }

    
    // Esegue un breve flash visivo sul pin per indicare
    // che è stato settato correttamente.
    public void Flash(float duration = 0.15f)
    {
        // Se non sono stati assegnati renderer o materiale,
        // il feedback visivo viene semplicemente saltato
        if (feedbackRenderer == null || flashMaterial == null)
            return;

        StopAllCoroutines();
        StartCoroutine(FlashRoutine(duration));
    }

    
    // Coroutine che applica temporaneamente il materiale di flash
    // e poi ripristina quello originale.
    private System.Collections.IEnumerator FlashRoutine(float duration)
    {
        feedbackRenderer.material = flashMaterial;

        yield return new WaitForSeconds(duration);

        if (_originalMat != null)
            feedbackRenderer.material = _originalMat;
    }


}
