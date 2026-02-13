using UnityEngine;

public enum CursorContext {
    Gameplay,
    UI
}

public class CursorManager : MonoBehaviour {

    public static CursorManager Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetContext(CursorContext context) {
        switch (context) {
            case CursorContext.Gameplay:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;

            case CursorContext.UI:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
                break;
        }
    }

    private void Update() {
        GameObject cursorImage = GameObject.FindGameObjectWithTag("Cursor");
        cursorImage.transform.position = Input.mousePosition;
    }
}
