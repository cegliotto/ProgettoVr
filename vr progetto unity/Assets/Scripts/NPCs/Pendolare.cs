using System;
using UnityEngine;

public class Pendolare : MonoBehaviour
{
    [SerializeField] private Transform sittingTransform;
    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = this.GetComponent<Animator>();
        if (PuzzleManager.Instance != null &&
            PuzzleManager.Instance.HasAnyPuzzleBeenSolved()) { // Alla risoluzione di qualunque puzzle va a sedersi
            MoveAndSit();
        }
    }
    private void MoveAndSit() {
        if(sittingTransform != null) {
            transform.position = sittingTransform.position;
            transform.rotation = sittingTransform.rotation;

            if (anim != null) {
                anim.SetTrigger("Sit");
            }
        }
    }
}
