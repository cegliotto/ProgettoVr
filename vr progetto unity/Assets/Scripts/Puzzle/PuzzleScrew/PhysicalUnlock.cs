using UnityEngine;
using System.Collections;

public class PhysicalUnlock : MonoBehaviour
{
    [SerializeField] private float moveDistance = 0.5f;
    [SerializeField] private float moveDuration = 1.0f;

    public void Unlock()
    {
        // Usiamo una coroutine per lo spostamento fluido
        StartCoroutine(AnimateMove());
    }

    private IEnumerator AnimateMove()
    {
        Vector3 startPos = transform.position;
        // transform.forward sposta l'oggetto "davanti" rispetto a dove guarda
        Vector3 endPos = transform.position + (transform.forward * moveDistance);
        float elapsed = 0;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;

        // Attiviamo la fisica solo alla fine del movimento
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}