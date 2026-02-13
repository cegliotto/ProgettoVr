using UnityEngine;
using UnityEngine.UI;

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
                HideCursor();
                break;

            case CursorContext.UI:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
                ShowCursor();
                break;
        }
    }

    private void Update() {
        GameObject cursorImage = GameObject.FindGameObjectWithTag("Cursor");
        cursorImage.transform.position = Input.mousePosition;
    }

    private void ShowCursor() {
        GameObject cursor = GameObject.FindGameObjectWithTag("Cursor");
        cursor.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }

    private void HideCursor() {
        GameObject cursor = GameObject.FindGameObjectWithTag("Cursor");
        cursor.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    }
}
