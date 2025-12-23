using UnityEngine;

public class PhysicalUnlock : MonoBehaviour
{
    public void Unlock()
    {
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false; // Disattiva Kinematic: ora il mobile risponde alla fisica/input
            rb.useGravity = true;
            Debug.Log("Viti rimosse: il comodino è ora mobile!");
        }
    }
}