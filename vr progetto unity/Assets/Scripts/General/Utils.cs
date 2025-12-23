using System.Collections;
using UnityEngine;

public static class Utils {

    // Script usati per abilitare e disabilitare le collisioni tra due collider
    // Viene utile per disabilitare le collisioni tra un oggetto e il player durante il grabbing
    public static void DisableCollision(Collider cl1, Collider cl2) {
        Physics.IgnoreCollision(cl1, cl2, true);
    }

    public static void EnableCollision(Collider cl1, Collider cl2) {
        Physics.IgnoreCollision(cl1, cl2, false);
    }

    public static IEnumerator FadeCanvasGroup(
        CanvasGroup canvasGroup,
        float from,
        float to,
        float duration
    ) {
        float elapsed = 0f;
        canvasGroup.alpha = from;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
