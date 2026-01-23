using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotebookItems : MonoBehaviour {

    [SerializeField] private GameObject[] itemsIcon;
    [SerializeField] private float fadeDuration = 0.5f; // durata animazione apparizione oggetti in notebook

    //private int itemsToPickUp = 6; // 6 oggetti da prendere
    public int itemsPickedUp = 0;

    public void AnnotateNewItem(ItemType item) {
        int index = (int)item;
        if ((int)item > itemsIcon.Length - 1) return;

        // suono scrittura taccuino

        GameObject icon = itemsIcon[index];
        CanvasGroup cg = icon.GetComponent<CanvasGroup>();

        icon.SetActive(true);

        // animazione per fade
        StartCoroutine(Utils.FadeCanvasGroup(cg, 0f, 0.4f, fadeDuration));
    }

    public void OnItemPickedUp(ItemType item) {
        int index = (int)item;
        if (index > itemsIcon.Length-1) return;

        // suono scrittura taccuino

        GameObject icon = itemsIcon[index];
        CanvasGroup cg = icon.GetComponent<CanvasGroup>();

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
        StartCoroutine(Utils.FadeCanvasGroup(cg, cg.alpha, 1f, fadeDuration));
    }
}
