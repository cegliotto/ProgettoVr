using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 2f;  // velocità del cubo
    Vector3 direction = new Vector3(1,0,0); // direzione del movimento


public float counter = 2f;
    void Update()
    {
        counter -= Time.deltaTime;
        // muove il cubo nella direzione scelta
        if (counter > 0)
            transform.Translate(direction * speed * Time.deltaTime);
    }
}
