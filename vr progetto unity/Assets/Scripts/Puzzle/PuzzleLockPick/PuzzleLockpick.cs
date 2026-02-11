using UnityEngine;

public class PuzzleLockpick : PuzzleBase
{
    [SerializeField] private Transform rake;
    [SerializeField] private Transform torsionWrench;

    [Header("Rake Movement")]
    [Tooltip("Quanto si muove il rake per pixel di mouse (base). Più basso = più preciso.")]
    [SerializeField] private float rakeMoveSpeed = 0.00012f;


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

    [Header("Anti Spike / Stabilization")]
    [Tooltip("Clamp massimo delta pixel/frame (anti scatto quando cambiano focus/flash/animazioni).")]
    [SerializeField] private float maxDeltaPixelsPerFrame = 80f;

    [Tooltip("Frame da ignorare dopo aver settato un pin (sicurezza).")]
    [SerializeField] private int ignoreMouseFramesAfterPinSet = 1;

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
    [SerializeField] private AudioSource finalOneShotSource;

    [SerializeField] private AudioClip scrapeLoop;
    [SerializeField] private AudioClip pinClick;
    [SerializeField] private AudioClip successFinal;

    [Header("Cubi")]
    [SerializeField] private Material pinLockedColor;
    [SerializeField] private Material pinUnlockedColor;
    [SerializeField] private Renderer[] pinVisuals;

    private float baseRakeY;
    private int currentPinIndex = 1; // 1..4
    private bool[] pinSet;
    private bool isDragging;

    private LockPinZone currentZone;
    private float stuckTimer = 0f;
    private float lastRakeMoveTime = -999f;

    private float lastMouseY;
    private int ignoreMouseFrames = 0;

    protected void Start()
    {
        pinSet = new bool[4];

        for (int i = 0; i < pinVisuals.Length; i++)
            pinVisuals[i].material = pinLockedColor;

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
        // Tensione SEMPRE gestita (anche in stuck, ma senza aumento se serve)
        bool allowIncrease = stuckTimer <= 0f;

        if (stuckTimer > 0f)
            stuckTimer -= Time.deltaTime;

        HandleTensionInput(allowIncrease);
        HandleRakeDrag();          // ✅ il rake NON si “blocca” mai
        UpdateTorsionVisual();

        // Se sei in stuck, blocca solo il set pin (non il movimento)
        if (stuckTimer > 0f)
            return;

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
    }

    private void HandleRakeDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMouseY = Input.mousePosition.y; // ancora pulita
            ignoreMouseFrames = 0;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            StopScrape();
            return;
        }

        if (!isDragging) return;

        if (ignoreMouseFrames > 0)
        {
            ignoreMouseFrames--;
            lastMouseY = Input.mousePosition.y; // evita accumulo delta
            return;
        }

        float mouseY = Input.mousePosition.y;
        float deltaPixels = mouseY - lastMouseY;
        lastMouseY = mouseY;

        // ✅ Anti-spike: se Unity “salta” il cursore o cambia focus, qui lo smorzi
        deltaPixels = Mathf.Clamp(deltaPixels, -maxDeltaPixelsPerFrame, maxDeltaPixelsPerFrame);

        float speed01 = Mathf.Clamp01(Mathf.Abs(deltaPixels) / Mathf.Max(1f, accelPixelsForMax));
        float curve01 = Mathf.Clamp01(accelerationCurve.Evaluate(speed01));
        float multiplier = Mathf.Lerp(1f, maxAccelerationMultiplier, curve01);

        float desiredDeltaY = deltaPixels * rakeMoveSpeed * multiplier;

        float targetY = Mathf.Clamp(rake.localPosition.y + desiredDeltaY, rakeMinY, rakeMaxY);
        float movement = Mathf.Abs(targetY - rake.localPosition.y);

        Vector3 localPos = rake.localPosition;
        localPos.y = targetY;
        rake.localPosition = localPos;

        if (movement > 0.00005f)
        {
            lastRakeMoveTime = Time.time;
            StartScrape();
        }

        if (scrapeSource != null && scrapeSource.isPlaying && (Time.time - lastRakeMoveTime) > 0.08f)
            StopScrape();
    }

    private void TrySetCurrentPin()
    {
        if (currentPinIndex < 1 || currentPinIndex > 4) return;
        if (pinSet[currentPinIndex - 1]) return;

        if (currentZone == null) return;
        if (currentZone.pinIndex != currentPinIndex) return;

        // Rispetta davvero setWhileDragging:
        // - true: set mentre trascini (mouse tenuto) oppure al click
        // - false: SOLO al click (mouse down)
        if (!setWhileDragging)
        {
            if (!Input.GetMouseButtonDown(0)) return;
        }
        else
        {
            // se vuoi che richieda almeno “tenere premuto”, lascia così:
            if (!Input.GetMouseButton(0)) return;
        }

        if (currentZone.IsTensionInRange(tension))
        {
            pinSet[currentPinIndex - 1] = true;
            pinVisuals[currentPinIndex - 1].material = pinUnlockedColor;

            PlayOneShot(pinClick);
            currentZone.Flash();

            currentPinIndex++;
            currentZone = null;

            // ✅ Previene scatti immediati dopo il set
            ignoreMouseFrames = Mathf.Max(ignoreMouseFramesAfterPinSet, 0);
            lastMouseY = Input.mousePosition.y;

            if (currentPinIndex == 5)
            {
                PlayFinalOneShot(successFinal);
                PuzzleCompleted();
            }
        }
        else if (tension > currentZone.maxTension)
        {
            stuckTimer = stuckCooldown;

            ignoreMouseFrames = Mathf.Max(ignoreMouseFramesAfterPinSet, 0);
            lastMouseY = Input.mousePosition.y;
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

    private void PlayFinalOneShot(AudioClip clip)
    {
        if (finalOneShotSource == null || clip == null) return;
        finalOneShotSource.PlayOneShot(clip);
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

    // Chiamalo da OnTriggerEnter
    public void SetZone(Collider other)
    {
        var zone = other.GetComponent<LockPinZone>();
        if (zone == null) return;

        if (zone.pinIndex == currentPinIndex)
            currentZone = zone;
    }

    // ✅ IMPORTANTE: chiamalo da OnTriggerStay per aggiornare zona anche senza re-enter
    public void StayZone(Collider other)
    {
        var zone = other.GetComponent<LockPinZone>();
        if (zone == null) return;

        if (zone.pinIndex == currentPinIndex)
            currentZone = zone;
    }

    // Chiamalo da OnTriggerExit
    public void ClearZone(Collider other)
    {
        var zone = other.GetComponent<LockPinZone>();
        if (zone != null && currentZone == zone)
            currentZone = null;
    }
}
