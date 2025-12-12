using UnityEngine;

//nota
// FIX JITTER:
// Prima la gerarchia era la seguente: Player > CameraHolder > camera
// per risolvere problemi di Jitter il cameraHolder e' stato posto "fuori dal player"
// e qui si controlla la posizione del cameraHolder, mentre la camera segue in modo fisso il cameraHolder
// dallo script CameraMove

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;

    private void Update() {
        transform.position = cameraPosition.position;
    }
}
