using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PagePair {
    public GameObject pageLeft;
    public GameObject pageRIght;

    public void ShowPair() {
        pageLeft.SetActive(true);
        pageRIght.SetActive(true);
    }

    public void HidePair() {
        pageLeft.SetActive(false);
        pageRIght.SetActive(false);
    }
}

public enum ItemType {
    Bastone,
    Borsa,
    Occhiali,
    Collana,
    Orologio,
    Cappello
}

public class NotebookManager : MonoBehaviour {
    public static NotebookManager Instance;

    [SerializeField] private PagePair[] pagePairs; // Coppie di pagine
    // Andando avanti col notebook si passa alla coppia di pagine successiva
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;

    [SerializeField] private float pageFadeDuration = 0.4f;
    private bool isTransitioning = false;

    private int currentPageIndex;

    public NotebookItems notebookItemsManager;
    public NotebookNotes notebookNotesManager;

    [SerializeField] private Button test_item;
    [SerializeField] private Button test_notes;

    [SerializeField] private Transform notebookHolder;

    private Animator anim;

    int i = 0;

    private List<ItemType> itemsPickedUp = new List<ItemType>();

    void Awake() {
        anim = GetComponent<Animator>();
        currentPageIndex = 0;

        test_item.onClick.AddListener(() => {
            notebookItemsManager.AnnotateNewItem((ItemType)i);
            notebookItemsManager.OnItemPickedUp((ItemType)i++);
        });

        test_notes.onClick.AddListener(() => {
            notebookNotesManager.UnlockNewProgress();
        });

        nextPageButton.onClick.AddListener(() => {
            NextPagePair();
        });

        prevPageButton.onClick.AddListener(() => {
            PreviousPagePair();
        });

        if (Instance != null) { // Se c'e' gia' istanza 
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // per non distruggerlo al cambio di scena
    }

    private void Update() {
        if (Player.Instance != null) {
            this.notebookHolder.transform.position = Player.Instance.notebookHolder.transform.position;
            this.notebookHolder.transform.rotation = Player.Instance.notebookHolder.transform.rotation;
        }
    }

    private void NextPagePair() {
        if (isTransitioning) return;
        if (currentPageIndex >= pagePairs.Length - 1) return;

        if(anim != null) {
            anim.SetTrigger("NextPage");
        }

        StartCoroutine(ChangePagePair(currentPageIndex + 1));
    }

    private void PreviousPagePair() {
        if (isTransitioning) return;
        if (currentPageIndex <= 0) return;

        if (anim != null) {
            anim.SetTrigger("PrevPage");
        }

        StartCoroutine(ChangePagePair(currentPageIndex - 1));
    }

    private IEnumerator ChangePagePair(int newIndex) {
        isTransitioning = true;

        PagePair current = pagePairs[currentPageIndex];
        PagePair next = pagePairs[newIndex];

        yield return StartCoroutine(TransitionPagePair(current, next));

        currentPageIndex = newIndex;
        isTransitioning = false;
    }

    private IEnumerator TransitionPagePair(PagePair fromPair, PagePair toPair) {
        // Assicuro che la nuova coppia sia attiva
        toPair.pageLeft.SetActive(true);
        toPair.pageRIght.SetActive(true);

        CanvasGroup fromLeft = fromPair.pageLeft.GetComponent<CanvasGroup>();
        CanvasGroup fromRight = fromPair.pageRIght.GetComponent<CanvasGroup>();
        CanvasGroup toLeft = toPair.pageLeft.GetComponent<CanvasGroup>();
        CanvasGroup toRight = toPair.pageRIght.GetComponent<CanvasGroup>();

        // Imposto alpha iniziali
        toLeft.alpha = 0f;
        toRight.alpha = 0f;

        // Fade OUT corrente + Fade IN successiva (in parallelo)
        StartCoroutine(Utils.FadeCanvasGroup(fromLeft, 1f, 0f, pageFadeDuration));
        StartCoroutine(Utils.FadeCanvasGroup(fromRight, 1f, 0f, pageFadeDuration));
        StartCoroutine(Utils.FadeCanvasGroup(toLeft, 0f, 1f, pageFadeDuration));
        StartCoroutine(Utils.FadeCanvasGroup(toRight, 0f, 1f, pageFadeDuration));

        yield return new WaitForSeconds(pageFadeDuration);

        // Spengo definitivamente la vecchia coppia
        fromPair.HidePair();
    }

    public void AddItem(ItemType new_item) {
        itemsPickedUp.Add(new_item);
    }

    public bool AlreadyPickedUp(ItemType item) {
        return itemsPickedUp.Contains(item);
    }
}
