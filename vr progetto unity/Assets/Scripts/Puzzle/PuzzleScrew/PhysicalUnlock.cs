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

        // Calculates the real length based on the collider or mesh
        float length = 1.0f;
        if (TryGetComponent<Collider>(out Collider col))
        {
            // adaptable to various sizes
            length = col.bounds.size.z;
        }

        // Moves by 1.5 times the calculated length
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