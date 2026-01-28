using UnityEngine;

public class PuzzleLockpick : PuzzleBase
{
    [SerializeField] private Transform rake;


    [SerializeField] private Transform torsionWrench;

    [Header("Rake Movement")]
    [SerializeField] private float rakeMoveSpeed = 0.0025f; // scala del delta mouse
    [SerializeField] private float rakeMinY = -0.02f;       // limiti local Y
    [SerializeField] private float rakeMaxY = 0.02f;

    [Header("Tension")]
    [Range(0f, 1f)]
    [SerializeField] private float tension = 0f;

    [SerializeField] private float tensionChangeSpeed = 1.2f; // quanto aumenta per secondo
    [SerializeField] private float torsionMaxAngle = 20f;      // rotazione visiva max (gradi)

    [Header("Pin Interaction")]
    [Tooltip("Se true, prova a settare il pin mentre il rake è in zona (più facile). Se false, solo al click.")]
    [SerializeField] private bool setWhileDragging = true;

    [SerializeField] private float stuckCooldown = 0.3f; // blocco breve se tensione troppo alta

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

    private LockPinZone currentZone;     // zona in cui si trova il rake (se è quella del pin corrente)
    private float stuckTimer = 0f;
    private float lastRakeMoveTime = -999f;
    protected void Start()
    {
        // Array che tiene traccia dei pin già settati (4 pin totali)
        pinSet = new bool[4];

        for (int i = 0; i < pinVisuals.Length; i++)
        {
            pinVisuals[i].material.color = pinLockedColor;
        }

        // Salvo la Y iniziale del rake e imposto un range attorno a quella posizione
        baseRakeY = rake.localPosition.y;
        rakeMinY = baseRakeY - 0.02f;
        rakeMaxY = baseRakeY + 0.02f;

        // Aggiorna subito la rotazione visiva del torsion
        // in base al valore iniziale della tensione
        UpdateTorsionVisual();

        // Prepara l'AudioSource per il suono di "scrape" continuo
        SetupScrapeLoop();
    }

    private void Update()
    {
        // Logica principale del puzzle (chiamata ogni frame)
        PuzzleBehaviour();

        // Uscita manuale dal puzzle (non completato)
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitPuzzle();
    }

    protected override void PuzzleBehaviour()
    {
        // Se siamo in stato di "stuck" (tensione troppo alta)
        if (stuckTimer > 0f)
        {
            // Scala il timer
            stuckTimer -= Time.deltaTime;

            // Durante lo stuck permettiamo SOLO di diminuire la tensione
            // così l'utente capisce che deve mollare
            HandleTensionInput(allowIncrease: false);

            // Aggiorna comunque il feedback visivo del torsion
            UpdateTorsionVisual();
            return;
        }

        // Gestione normale della tensione (A / D)
        HandleTensionInput(allowIncrease: true);

        // Gestione del movimento del rake tramite drag del mouse
        HandleRakeDrag();

        // Aggiorna la rotazione del torsion in base alla tensione corrente
        UpdateTorsionVisual();

        TrySetCurrentPin();


    }

    private void HandleTensionInput(bool allowIncrease)
    {
        float delta = 0f;

        // A = diminuisce tensione
        if (Input.GetKey(KeyCode.A)) delta -= 1f;

        // D = aumenta tensione
        if (Input.GetKey(KeyCode.D)) delta += 1f;

        // Se siamo in stuck, non permettiamo di aumentare la tensione
        if (!allowIncrease && delta > 0f)
            delta = 0f;

        // Aggiorna il valore di tensione nel tempo
        tension += delta * tensionChangeSpeed * Time.deltaTime;

        // Clamp tra 0 e 1 (valore normalizzato)
        tension = Mathf.Clamp01(tension);
        Debug.Log("Tensione attuale: " + tension);

    }
    private float dragStartMouseY;
    private float dragStartRakeY;

    private void HandleRakeDrag()
    {
        // Inizio drag
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartMouseY = Input.mousePosition.y;
            dragStartRakeY = rake.localPosition.y;
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

        // Movimento "assoluto": evita salti e drift
        float mouseY = Input.mousePosition.y;
        float offsetY = (mouseY - dragStartMouseY) * rakeMoveSpeed;
        float targetY = Mathf.Clamp(dragStartRakeY + offsetY, rakeMinY, rakeMaxY);

        float movement = Mathf.Abs(targetY - rake.localPosition.y);

        // applica posizione
        Vector3 localPos = rake.localPosition;
        localPos.y = targetY;
        rake.localPosition = localPos;

        // --- AUDIO SCRAPE STABILE ---
        if (movement > 0.00005f)
        {
            lastRakeMoveTime = Time.time;
            StartScrape();
        }

        // ferma solo se è passato un po’ di tempo dall’ultimo movimento vero
        if (scrapeSource != null && scrapeSource.isPlaying && (Time.time - lastRakeMoveTime) > 0.08f)
        {
            StopScrape();
        }

        
    }

    private void TrySetCurrentPin()
    {
        Debug.Log($"pinCorrente={currentPinIndex} | tensione={tension} | currentZone={(currentZone ? currentZone.name : "NULL")}");


        // Controlli di sicurezza sull'indice del pin
        if (currentPinIndex < 1 || currentPinIndex > 4) return;

        // Se il pin corrente è già stato settato, esci
        if (pinSet[currentPinIndex - 1]) return;

        // Se il rake non si trova nella zona del pin corrente, esci
        if (currentZone == null) return;
        if (currentZone.pinIndex != currentPinIndex) return;

        // Caso corretto: tensione nel range giusto
        if (currentZone.IsTensionInRange(tension))
        {
            // Segna il pin come settato
            pinSet[currentPinIndex - 1] = true;
            pinVisuals[currentPinIndex - 1].material.color = pinUnlockedColor;

            // Feedback audio
            PlayOneShot(pinClick);

            // Feedback visivo opzionale sul pin
            currentZone.Flash();

            // Passa al pin successivo (ordine fisso)
            currentPinIndex++;
            currentZone = null;

            // Se abbiamo appena settato l'ultimo pin
            if (currentPinIndex == 5)
            {
                PlayOneShot(successFinal);
                PuzzleCompleted();
            }
        }
        // Caso: tensione troppo alta
        else if (tension > currentZone.maxTension)
        {
            stuckTimer = stuckCooldown;
        }
        // Caso: tensione troppo bassa → nessun evento
    }


    private void UpdateTorsionVisual()
    {
        // Se il modello del torsion non è assegnato, esci
        if (torsionWrench == null) return;

        // Interpolazione dell'angolo in base alla tensione
        float angle = Mathf.Lerp(0f, torsionMaxAngle, tension);

        // Rotazione locale del torsion (asse da adattare al modello)
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
        {
            PuzzleManager.Instance.CompletePuzzle();
        }
    }

    protected override void ExitPuzzle()
    {
        PuzzleManager.Instance.ExitFromPuzzle();
    }

    public void SetZone(Collider other)
    {
        var zone = other.GetComponent<LockPinZone>();
        if (zone == null) return;

        // Ordine fisso: accetta solo la zona del pin corrente
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
