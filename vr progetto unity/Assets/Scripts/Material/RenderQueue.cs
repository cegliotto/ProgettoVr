using UnityEngine;

public class RenderQueue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Renderer>().material.renderQueue = 3300;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
