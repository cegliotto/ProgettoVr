using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotebookItems : MonoBehaviour {

    [SerializeField] private GameObject[] itemsIcon;
    [SerializeField] private float fadeDuration = 0.5f; // durata animazione apparizione oggetti in notebook

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

        // lo aggiungo alla lista degli oggetti raccolti
        if(NotebookManager.Instance != null) {
            NotebookManager.Instance.AddItem(item);
        }

        // animazione per fade
        StartCoroutine(Utils.FadeCanvasGroup(cg, cg.alpha, 1f, fadeDuration));
    }
}
