using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
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
    Cappello,
    None
}

public class NotebookManager : MonoBehaviour {
    public static NotebookManager Instance;

    [SerializeField] private PagePair[] pagePairs; // Coppie di pagine
    // Andando avanti col notebook si passa alla coppia di pagine successiva
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;

    [SerializeField] private float pageFadeDuration = 0.4f;
    private bool isTransitioning = false;

    public int currentPageIndex;

    public NotebookItems notebookItemsManager;
    public NotebookNotes notebookNotesManager;

    [SerializeField] private Button test_item;
    [SerializeField] private Button test_notes;

    [SerializeField] private Transform notebookHolder;
    [SerializeField] private Transform notebookCamera;
    [SerializeField] private Transform notebookTransform;

    private Animator anim;

    int i = 0;

    private List<ItemType> itemsPickedUp = new List<ItemType>();

    public bool IsContentAnimating = false;
    private AudioSource audioSource;
    [SerializeField] private AudioClip paperWriteClip;
    [SerializeField] private AudioClip longPaperWriteClip;
    [SerializeField] private AudioClip openingClosingNotebookClip;
    [SerializeField] private AudioClip pageScrollClip;

    [SerializeField] private Animator notebookAnimator;

    [SerializeField] private float animationDuration = 0.5f; // animazione notebook camera
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Quaternion targetLocalRotation;
    private Coroutine animationCoroutine;
    private bool isNotebookClosing = false;

    [SerializeField] private float notebookCameraFOV = 40f; // fov per la camera del notebook
    [SerializeField] private float defaultFOV; // fov main camera

    private Vector3 notebookClosedPosition;
    [SerializeField] private Vector3 notebookClosedPositionOffset;
    private Vector3 notebookOpenPosition;

    void Awake() {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentPageIndex = 0;

        if (notebookCamera != null) {
            targetLocalRotation = notebookCamera.transform.localRotation;
        }

        test_item.onClick.AddListener(() => {
            notebookItemsManager.AnnotateNewItem((ItemType)i);
            notebookItemsManager.OnItemPickedUp((ItemType)i++);
        });

        test_notes.onClick.AddListener(() => {
            notebookNotesManager.UnlockNewProgress(NotebookNotes.NotesProgress.Ticket);
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

    private void Start() {
        notebookOpenPosition = notebookTransform.transform.localPosition;
        notebookClosedPosition = notebookOpenPosition + notebookClosedPositionOffset;
    }

    private void Update() {
        if(PuzzleManager.Instance != null) {
            if (PuzzleManager.Instance.isPlayingPuzzle) {
                return;
            }
        }

        if (Player.Instance != null) {
            this.notebookHolder.transform.position = Player.Instance.notebookHolder.transform.position;
            this.notebookHolder.transform.rotation = Player.Instance.notebookHolder.transform.rotation;

            if (Keyboard.current.iKey.wasPressedThisFrame) {
                if (notebookHolder.gameObject.activeSelf) {

                    if (!IsContentAnimating)
                        CloseNotebook();
                }
                else if (!Player.Instance.playerInteract.IsGrabbing()) {
                    OpenNotebook();
                }
            }
            // esc per chiudere il notebook
            if (Keyboard.current.escapeKey.wasPressedThisFrame) {
                if (notebookHolder.gameObject.activeSelf) {
                    if (!IsContentAnimating)
                        CloseNotebook();
                }
            }
        }

        ArrowVisibilityManagement();
    }

    private void ArrowVisibilityManagement() { // Visibilita' frecce in base a pagina corrente del notebook
        prevPageButton.gameObject.SetActive(currentPageIndex != 0);
        nextPageButton.gameObject.SetActive(currentPageIndex != pagePairs.Length - 1);
    }

    public void SetBusyForSeconds(float duration) {
        StartCoroutine(BusyRoutine(duration));
    }

    private IEnumerator BusyRoutine(float duration) {
        IsContentAnimating = true;
        // WaitForSecondsRealtime assicura che funzioni anche se metti il gioco in pausa
        yield return new WaitForSecondsRealtime(duration);
        IsContentAnimating = false;
    }

    public void OpenNotebook(int specificPageIndex = -1) {
        if (notebookHolder.gameObject.activeSelf || isNotebookClosing) return;

        isNotebookClosing = false;
        PlayOpenCloseNotebookSound();

        if (specificPageIndex >= 0 && specificPageIndex < pagePairs.Length) {
            currentPageIndex = specificPageIndex;
        }

        // si resettano visivamente le pagine
        UpdatePagesVisualsImmediate();

        notebookHolder.gameObject.SetActive(true);
        // animazione camera
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimateCameraOpen());

        // animazione notebook
        if (notebookAnimator != null) {
            notebookAnimator.SetTrigger("Open");
            notebookAnimator.ResetTrigger("Close");
        }

        Player.Instance.SetState(Player.PlayerState.Pause);

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetContext(CursorContext.UI);
    }
    // Animazione camera per apertura
    private IEnumerator AnimateCameraOpen() {
        Transform mainCamTransform = Camera.main.transform;
        Transform holderTransform = notebookCamera.transform;

        holderTransform.rotation = mainCamTransform.rotation;
        Quaternion startLocalRotation = holderTransform.localRotation;

        float timeElapsed = 0f;
        float startFOV = defaultFOV;

        Vector3 startPos = notebookClosedPosition;

        while (timeElapsed < animationDuration) {
            float t = timeElapsed / animationDuration;
            float curveValue = animationCurve.Evaluate(t);

            holderTransform.localRotation = Quaternion.Slerp(startLocalRotation, targetLocalRotation, curveValue);
            notebookCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(startFOV, notebookCameraFOV, curveValue);
            notebookTransform.localPosition = Vector3.Lerp(notebookClosedPosition, notebookOpenPosition, curveValue);

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        holderTransform.localRotation = targetLocalRotation;
        animationCoroutine = null;
    }

    public void CloseNotebook() {
        if (!notebookHolder.gameObject.activeSelf || isNotebookClosing) return;

        PlayOpenCloseNotebookSound();
        isNotebookClosing = true;
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimateCameraClose());
    }

    // Animazione camera per chiusura
    private IEnumerator AnimateCameraClose() {
        if (notebookAnimator != null) {
            notebookAnimator.SetTrigger("Close");
        }

        Transform mainCamTransform = Camera.main.transform;
        Transform holderTransform = notebookCamera.transform;

        Quaternion startRotation = holderTransform.rotation;

        float timeElapsed = 0f;

        float startFOV = notebookCamera.GetComponent<Camera>().fieldOfView;
        Vector3 startPos = notebookOpenPosition;

        while (timeElapsed < animationDuration) {
            if (!isNotebookClosing) yield break;

            float t = timeElapsed / animationDuration;
            float curveValue = animationCurve.Evaluate(t);

            holderTransform.rotation = Quaternion.Slerp(startRotation, mainCamTransform.rotation, curveValue);
            notebookCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(startFOV, defaultFOV, curveValue);
            notebookTransform.localPosition = Vector3.Lerp(notebookOpenPosition, notebookClosedPosition, curveValue);

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        isNotebookClosing = false;

        notebookHolder.gameObject.SetActive(false);

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetContext(CursorContext.Gameplay);

        Player.Instance.SetState(Player.PlayerState.Idle);

        animationCoroutine = null;
    }

    private void UpdatePagesVisualsImmediate() {
        for (int i = 0; i < pagePairs.Length; i++) {
            PagePair pair = pagePairs[i];

            if (i == currentPageIndex) {
                pair.ShowPair();
                SetPairAlpha(pair, 1f);
            }
            else {
                pair.HidePair();
                SetPairAlpha(pair, 0f);
            }
        }
    }



    // Helper per settare l'alpha di entrambi i lati velocemente
    private void SetPairAlpha(PagePair pair, float alpha) {
        if (pair.pageLeft != null) {
            CanvasGroup cg = pair.pageLeft.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = alpha;
        }
        if (pair.pageRIght != null) {
            CanvasGroup cg = pair.pageRIght.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = alpha;
        }
    }


    private IEnumerator ResumeGameplayNextFrame() {
        yield return null; // frame successivo

        Player.Instance.SetState(Player.PlayerState.Idle);

        var pc = Player.Instance.GetComponent<PlayerController>();
        pc.SetCameraRotation(
            pc.GetCameraXRotation(),
            pc.GetCameraYRotation()
        );
    }

    private void NextPagePair() {
        if (isTransitioning || IsContentAnimating) return;
        if (currentPageIndex >= pagePairs.Length - 1) return;

        if (anim != null) {
            anim.SetTrigger("NextPage");
        }

        ArrowVisibilityManagement();

        StartCoroutine(ChangePagePair(currentPageIndex + 1));
    }

    public void PreviousPagePair() {
        if (isTransitioning || IsContentAnimating) return;
        if (currentPageIndex <= 0) return;

        if (anim != null) {
            anim.SetTrigger("PrevPage");
        }

        ArrowVisibilityManagement();

        StartCoroutine(ChangePagePair(currentPageIndex - 1));
    }

    public void PlayWriteSound() {
        if (audioSource == null || paperWriteClip == null) return;

        audioSource.PlayOneShot(paperWriteClip);
    }

    public void PlayLongWriteSound() {
        if (audioSource == null || longPaperWriteClip == null) return;

        audioSource.PlayOneShot(longPaperWriteClip);
    }

    private void PlayOpenCloseNotebookSound() {
        if (audioSource == null || openingClosingNotebookClip == null) return;

        audioSource.PlayOneShot(openingClosingNotebookClip);
    }

    private void PlayPageScrollSound() {
        if (audioSource == null || pageScrollClip == null) return;

        audioSource.PlayOneShot(pageScrollClip);
    }

    private IEnumerator ChangePagePair(int newIndex) {
        isTransitioning = true;

        PagePair current = pagePairs[currentPageIndex];
        PagePair next = pagePairs[newIndex];
        PlayPageScrollSound(); // Suono di cambio pagina

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
