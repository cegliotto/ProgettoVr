using UnityEngine;
using System.Collections;

public class PhysicalUnlock : MonoBehaviour
{
    [SerializeField] private float moveDistance = 0.5f;
    [SerializeField] private float moveDuration = 1.0f;

    public void Unlock()
    {  
        StartCoroutine(AnimateMove());
    }

    private IEnumerator AnimateMove()
    {
        Vector3 startPos = transform.position;

        // si calcola la lunghezza reale basandoci sul collider o mesh
        float length = 1.0f;
        if (TryGetComponent<Collider>(out Collider col))
        {
            length = col.bounds.size.z;
        }

        // Sposta di 1.5 volte la lunghezza calcolata
        float finalDistance = length * moveDistance;
        Vector3 endPos = transform.position + (transform.forward * finalDistance);

        float elapsed = 0;
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}