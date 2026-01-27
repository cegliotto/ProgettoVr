using UnityEngine;

public class ParallaxLooper : MonoBehaviour
{
    public float speed = 6f;            // scorre in +Z globale
    public float segmentLengthZ = 60f;  // distanza tra segmenti (L)

    Transform a, b, c;
    float startZ;

    void Start()
    {
        a = transform.GetChild(0);
        b = transform.GetChild(1);
        c = transform.GetChild(2);

        startZ = a.position.z;
    }

    void Update()
    {
        float dz = speed * Time.deltaTime;
        Vector3 step = new Vector3(0f, 0f, dz);

        a.position += step;
        b.position += step;
        c.position += step;

        float limitZ = startZ + segmentLengthZ;
        float wrap = segmentLengthZ * 3f; // 3 segmenti -> 3L

        if (a.position.z >= limitZ) a.position -= new Vector3(0f, 0f, wrap);
        if (b.position.z >= limitZ) b.position -= new Vector3(0f, 0f, wrap);
        if (c.position.z >= limitZ) c.position -= new Vector3(0f, 0f, wrap);
    }
}
