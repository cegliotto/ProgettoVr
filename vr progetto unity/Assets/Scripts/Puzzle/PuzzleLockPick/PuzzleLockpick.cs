using UnityEngine;

public class PuzzleLockpick : PuzzleBase
{
    [SerializeField] private Transform rake;
    [SerializeField] private Transform torsionWrench;

    [Header("Rake Movement")]
    [Tooltip("Quanto si muove il rake per pixel di mouse (base). Più basso = più preciso.")]
    [SerializeField] private float rakeMoveSpeed = 0.00012f;

    [Tooltip("Range verticale (in locale) attorno alla posizione iniziale del rake.")]
    [SerializeField] private float rakeRange = 0.03f; // ↑ aumenta se sotto al 2° pin non arrivi

    [Tooltip("Moltiplicatore massimo quando muovi il mouse velocemente.")]
    [SerializeField] private float maxAccelerationMultiplier = 8f;

    [Tooltip("Quanti pixel/frame servono per arrivare al massimo della curva (più alto = meno accelera).")]
    [SerializeField] private float accelPixelsForMax = 35f;

    [Tooltip("Curva di accelerazione: input 0..1 (velocità mouse), output 0..1 (moltiplicatore).")]
    [SerializeField]
    private AnimationCurve accelerationCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.25f, 0.08f),
        new Keyframe(0.6f, 0.35f),
        new Keyframe(1f, 1f)
    );

    [SerializeField] private float rakeMinY = 0.2355f;
    [SerializeField] private float rakeMaxY = 0.28f;

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

    protected void Start()
    {
        pinSet = new bool[4];

        for (int i = 0; i < pinVisuals.Length; i++)
            pinVisuals[i].material.color = pinLockedColor;

        baseRakeY = rake.localPosition.y;

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
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMouseY = Input.mousePosition.y;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            StopScrape();
            return;
        }

        if (!isDragging) return;

        float mouseY = Input.mousePosition.y;
        float deltaPixels = mouseY - lastMouseY;
        lastMouseY = mouseY;

        // 0..1 in base a "quanto veloce" è il movimento (pixel/frame)
        float speed01 = Mathf.Clamp01(Mathf.Abs(deltaPixels) / Mathf.Max(1f, accelPixelsForMax));

        // curva (precisione sui piccoli movimenti, accelera sui grandi)
        float curve01 = Mathf.Clamp01(accelerationCurve.Evaluate(speed01));

        // moltiplicatore: 1..maxAccelerationMultiplier
        float multiplier = Mathf.Lerp(1f, maxAccelerationMultiplier, curve01);

        float desiredDeltaY = deltaPixels * rakeMoveSpeed * multiplier;

        float targetY = Mathf.Clamp(rake.localPosition.y + desiredDeltaY, rakeMinY, rakeMaxY);

        float movement = Mathf.Abs(targetY - rake.localPosition.y);

        Vector3 localPos = rake.localPosition;
        localPos.y = targetY;
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
