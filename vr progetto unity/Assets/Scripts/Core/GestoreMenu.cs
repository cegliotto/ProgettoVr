using UnityEngine;

public class GestoreMenu : MonoBehaviour
{
    private void Start() {
        if (CursorManager.Instance == null) return;

        CursorManager.Instance.SetContext(CursorContext.UI); // Setto contesto UI
    }
    public void EsciDalGioco()
    {
        Debug.Log("Il giocatore ha premuto Esci");

        // Chiude l'applicazione (funziona nel gioco esportato)
        Application.Quit();

        // Se si è nell'Editor di Unity, questo serve per fermare il PlayMode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}