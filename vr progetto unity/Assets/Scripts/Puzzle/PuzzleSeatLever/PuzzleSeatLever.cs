using UnityEngine;

public class PuzzleSeatLever : PuzzleBase
{

    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private SeatLever lever;
    [SerializeField] private SeatDrawer drawer;

    private SeatLever activeLever;
    private bool isClicking;


    [SerializeField] private float idleTimeToClose = 0.75f;

    private float lastPullTime;

    void Update()
    {
        PuzzleBehaviour();
    }


    protected override void PuzzleBehaviour()
    {
        SelectLever();
        HandleInput();
        HandleDrawerAutoClose();
        CheckCompletion();
        ExitPuzzle();
    }


    private void SelectLever()
    {
        if (isClicking) return;


        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~0))
        {
            SeatLever lever = hit.collider.GetComponentInParent<SeatLever>();
            if (lever != null && lever != activeLever)
            {
                activeLever = lever;
                Debug.Log("Leva selezionata: " + activeLever.name);
            }
        }
        else
        {
            activeLever = null;
        }
    }



    private void HandleInput()
    {
        // click inizia solo se il mouse è sopra la leva
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast fatto AL MOMENTO DEL CLICK per evitare che prenda click su altri oggetti della scena
            Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~0))
            {
                SeatLever clickedLever = hit.collider.GetComponentInParent<SeatLever>();
                if (clickedLever == null) return;

                activeLever = clickedLever;
                isClicking = true;

                activeLever.Pull();
                //drawer.OpenStep(); TOLTO PERCHè IL CASSETTO VIENE GESTITO DALLA LEVA E NON PIù DALL'INPUT DIRETTAMENTE
                lastPullTime = Time.time;
                return;
            }
        }

        // rilascio click
        if (Input.GetMouseButtonUp(0))
        {
            isClicking = false;
            return;
        }
    }


    private void HandleDrawerAutoClose()
    {
        if (Time.time - lastPullTime > idleTimeToClose)
        {
            drawer.CloseStep(Time.deltaTime);
        }
    }

    private void CheckCompletion()
    {
        if (drawer.IsFullyOpen())
        {
            PuzzleCompleted();
        }
    }

    protected override void PuzzleCompleted()
    {
        Debug.Log("Cassetto aperto!");

        if (PuzzleManager.Instance != null)
        {
            PuzzleManager.Instance.CompletePuzzle();
        }
    }

    protected override void ExitPuzzle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PuzzleManager.Instance.ExitFromPuzzle();
        }
    }
}
