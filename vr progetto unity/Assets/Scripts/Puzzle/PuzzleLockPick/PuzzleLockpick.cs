using UnityEngine;

public class PuzzleLockpick : PuzzleBase
{
    [SerializeField] private Transform rake;
    [SerializeField] private Transform torsionWrench;

    [Header("Rake Movement")]
    [Tooltip("Quanto il rake segue lo spostamento del mouse (in unità locali per pixel). Più basso = più preciso.")]
    [SerializeField] private float rakeMoveSpeed = 0.00015f; // MOLTO più basso

    [Tooltip("Velocità massima del rake (unità locali al secondo). Evita che un micro-movimento/step del mouse faccia saltare.")]
    [SerializeField] private float maxRakeSpeed = 0.03f;

    [SerializeField] private float rakeMinY = -0.02f;
    [SerializeField] private float rakeMaxY = 0.02f;

    [Header("Tension")]
    [Range(0f, 1f)]
    [SerializeField] private float tension = 0f;

    [SerializeField] private float tensionChangeSpeed = 1.2f;
    [SerializeField] private float torsionMaxAngle = 20f;

    [Header("Pin Interaction")]
    [Tooltip("Se true, prova a settare il pin mentre il rake è in zona (più facile). Se false, solo al click.")]
    [SerializeField] private bool setWhileDragging = true;

    [SerializeField] private float stuckCooldown = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioSource scrapeSource;
    [SerializeField] private AudioSource oneShotSource;
    [SerializeField] private AudioClip scrapeLoop;
    [SerializeField] private AudioClip pinClick;
    [SerializeField] private AudioClip successFinal;

    [Header("Cubi")]
    [SerializeField] private Color pinLockedColor;
    [SerializeField] private Color pinUnlockedColor;
    [SerializeField] private Renderer[] pinVisuals;

    private float baseRakeY;
    private int currentPinIndex = 1; // 1..4
    private bool[] pinSet;
    private bool isDragging;

    private LockPinZone currentZone;
    private float stuckTimer = 0f;
    private float lastRakeMoveTime = -999f;

    private float lastMouseY;
    private float rakeY; // stato "continuo" del rake (più stabile del target diretto)

    protected void Start()
    {
        pinSet = new bool[4];

        for (int i = 0; i < pinVisuals.Length; i++)
            pinVisuals[i].material.color = pinLockedColor;

        baseRakeY = rake.localPosition.y;
        rakeMinY = baseRakeY - 0.02f;
        rakeMaxY = baseRakeY + 0.02f;

        rakeY = baseRakeY;

        UpdateTorsionVisual();
        SetupScrapeLoop();
    }

    private void Update()
    {
        PuzzleBehaviour();

        if (Input.GetKeyDown(KeyCode.Escape))
            ExitPuzzle();
    }

    protected override void PuzzleBehaviour()
    {
        if (stuckTimer > 0f)
        {
            stuckTimer -= Time.deltaTime;

            HandleTensionInput(allowIncrease: false);
            UpdateTorsionVisual();
            return;
        }

        HandleTensionInput(allowIncrease: true);
        HandleRakeDrag();
        UpdateTorsionVisual();

        TrySetCurrentPin();
    }

    private void HandleTensionInput(bool allowIncrease)
    {
        float delta = 0f;

        if (Input.GetKey(KeyCode.A)) delta -= 1f;
        if (Input.GetKey(KeyCode.D)) delta += 1f;

        if (!allowIncrease && delta > 0f)
            delta = 0f;

        tension += delta * tensionChangeSpeed * Time.deltaTime;
        tension = Mathf.Clamp01(tension);

        Debug.Log("Tensione attuale: " + tension);
    }

    private void HandleRakeDrag()
    {
        // Inizio drag
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMouseY = Input.mousePosition.y;

            // riallinea lo stato interno alla posizione attuale
            rakeY = rake.localPosition.y;
            return;
        }

        // Fine drag
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            StopScrape();
            return;
        }

        if (!isDragging) return;

        // Movimento incrementale (più "fine"): usa delta pixel frame-to-frame
        float mouseY = Input.mousePosition.y;
        float deltaPixels = mouseY - lastMouseY;
        lastMouseY = mouseY;

        // Converti pixel -> unità locali (molto piccolo)
        float desiredDeltaY = deltaPixels * rakeMoveSpeed;

        // Limita la velocità (anti-salto) indipendentemente dal DPI / polling
        float maxStep = maxRakeSpeed * Time.deltaTime;
        desiredDeltaY = Mathf.Clamp(desiredDeltaY, -maxStep, maxStep);

        // Applica e clamp ai limiti
        rakeY = Mathf.Clamp(rakeY + desiredDeltaY, rakeMinY, rakeMaxY);

        float movement = Mathf.Abs(rakeY - rake.localPosition.y);

        Vector3 localPos = rake.localPosition;
        localPos.y = rakeY;
        rake.localPosition = localPos;

        // --- AUDIO SCRAPE STABILE ---
        if (movement > 0.00005f)
        {
            lastRakeMoveTime = Time.time;
            StartScrape();
        }

        if (scrapeSource != null && scrapeSource.isPlaying && (Time.time - lastRakeMoveTime) > 0.08f)
        {
            StopScrape();
        }
    }

    private void TrySetCurrentPin()
    {
        Debug.Log($"pinCorrente={currentPinIndex} | tensione={tension} | currentZone={(currentZone ? currentZone.name : "NULL")}");

        if (currentPinIndex < 1 || currentPinIndex > 4) return;
        if (pinSet[currentPinIndex - 1]) return;

        if (currentZone == null) return;
        if (currentZone.pinIndex != currentPinIndex) return;

        if (currentZone.IsTensionInRange(tension))
        {
            pinSet[currentPinIndex - 1] = true;
            pinVisuals[currentPinIndex - 1].material.color = pinUnlockedColor;

            PlayOneShot(pinClick);
            currentZone.Flash();

            currentPinIndex++;
            currentZone = null;

            if (currentPinIndex == 5)
            {
                PlayOneShot(successFinal);
                PuzzleCompleted();
            }
        }
        else if (tension > currentZone.maxTension)
        {
            stuckTimer = stuckCooldown;
        }
    }

    private void UpdateTorsionVisual()
    {
        if (torsionWrench == null) return;

        float angle = Mathf.Lerp(0f, torsionMaxAngle, tension);
        torsionWrench.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    private void SetupScrapeLoop()
    {
        if (scrapeSource == null || scrapeLoop == null) return;

        scrapeSource.loop = true;
        scrapeSource.clip = scrapeLoop;
        scrapeSource.playOnAwake = false;
    }

    private void StartScrape()
    {
        if (scrapeSource == null || scrapeLoop == null) return;
        if (!scrapeSource.isPlaying)
            scrapeSource.Play();
    }

    private void StopScrape()
    {
        if (scrapeSource == null) return;
        if (scrapeSource.isPlaying)
            scrapeSource.Stop();
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (oneShotSource == null || clip == null) return;
        oneShotSource.PlayOneShot(clip);
    }

    protected override void PuzzleCompleted()
    {
        if (PuzzleManager.Instance != null)
            PuzzleManager.Instance.CompletePuzzle();
    }

    protected override void ExitPuzzle()
    {
        PuzzleManager.Instance.ExitFromPuzzle();
    }

    public void SetZone(Collider other)
    {
        var zone = other.GetComponent<LockPinZone>();
        if (zone == null) return;

        if (zone.pinIndex == currentPinIndex)
            currentZone = zone;
    }

    public void ClearZone(Collider other)
    {
        var zone = other.GetComponent<LockPinZone>();
        if (zone != null && currentZone == zone)
            currentZone = null;
    }
}
