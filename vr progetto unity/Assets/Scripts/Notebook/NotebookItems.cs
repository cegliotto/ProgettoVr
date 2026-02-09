using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NotebookItems : MonoBehaviour {

    [SerializeField] private GameObject[] itemsIcon;
    [SerializeField] private GameObject[] checkIcons;
    [SerializeField] private float fadeDuration = 0.5f; // durata animazione apparizione oggetti in notebook

    [SerializeField] private AudioClip suspanceMusic;

    //private int itemsToPickUp = 6; // 6 oggetti da prendere
    public int itemsPickedUp = 0;

    public void AnnotateNewItem(ItemType item) {
        int index = (int)item;
        if ((int)item > itemsIcon.Length - 1) return;

        NotebookManager.Instance.OpenNotebook(0);

        GameObject icon = itemsIcon[index];
        CanvasGroup cg = icon.GetComponent<CanvasGroup>();

        icon.SetActive(true);

        if(item == ItemType.Cappello) {
            // devo sbloccare il cappello
            PickUpItem cappello = FindObjectsByType<PickUpItem>(FindObjectsSortMode.None).FirstOrDefault(p => p.GetItemType() == ItemType.Cappello);
            cappello.enabled = true;
            cappello.canPickedUp = true;


            var audioSource = NotebookManager.Instance.GetComponent<AudioSource>();

            MusicManager.Instance.PlayMusicImmediate(suspanceMusic, 1f);

        }

        // animazione per fade
        StartCoroutine(Utils.FadeCanvasGroup(cg, 0f, 1f, fadeDuration));
    }

    public void OnItemPickedUp(ItemType item) {
        int index = (int)item;
        if (index > itemsIcon.Length-1) return;

        // suono scrittura taccuino

        GameObject icon = itemsIcon[index];
        CanvasGroup cg = icon.GetComponent<CanvasGroup>();

        GameObject check_icon = checkIcons[index];
        CanvasGroup cg_check = check_icon.GetComponent<CanvasGroup>();


        if (!icon.activeSelf) { // se non era stato annotato, si annota
            AnnotateNewItem(item);
        }

        itemsPickedUp++;

        // lo aggiungo alla lista degli oggetti raccolti
        if(NotebookManager.Instance != null) {
            NotebookManager.Instance.AddItem(item);
            NotebookManager.Instance.PlayWriteSound();

            NotebookManager.Instance.SetBusyForSeconds(fadeDuration);
        }

        // animazione per fade
        StartCoroutine(Utils.FadeCanvasGroup(cg, cg.alpha, 0.6f, fadeDuration));
        StartCoroutine(Utils.FadeCanvasGroup(cg_check, 0.0f, 1f, fadeDuration));
    }
}
