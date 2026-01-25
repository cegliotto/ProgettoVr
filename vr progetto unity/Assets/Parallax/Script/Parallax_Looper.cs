using UnityEngine;

public class ParallaxLooper : MonoBehaviour
{
    public float speed = 6f;            // scorre in +Z globale
    public float segmentLengthZ = 60f;  // distanza tra A e B in Z globale

    Transform a, b;
    float startZ;

    void Start()
    {
        a = transform.GetChild(0);
        b = transform.GetChild(1);

        // memorizzo lo Z iniziale del gruppo come riferimento
        startZ = transform.position.z;
    }

    void Update()
    {
        float dz = speed * Time.deltaTime;
        Vector3 step = new Vector3(0f, 0f, dz);

        a.position += step;
        b.position += step;

        float limitZ = startZ + segmentLengthZ;

        if (a.position.z >= limitZ)
            a.position -= new Vector3(0f, 0f, segmentLengthZ * 2f);

        if (b.position.z >= limitZ)
            b.position -= new Vector3(0f, 0f, segmentLengthZ * 2f);
    }
}
