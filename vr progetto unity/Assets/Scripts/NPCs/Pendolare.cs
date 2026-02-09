using System;
using UnityEngine;

public class Pendolare : MonoBehaviour
{
    public static Pendolare Instance;
    [SerializeField] private Transform sittingTransform;
    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        if(Instance != null) { // Se c'e' gia' istanza 
            Destroy(gameObject);
            return;
        }
        Instance = this;

        anim = this.GetComponent<Animator>();
        if (PuzzleManager.Instance != null &&
            PuzzleManager.Instance.HasAnyPuzzleBeenSolved()) { // Alla risoluzione di qualunque puzzle va a sedersi
            MoveAndSit();
        }
    }
    public void MoveAndSit() {
        if(sittingTransform != null) {
            transform.position = sittingTransform.position;
            transform.rotation = sittingTransform.rotation;

            if (anim != null) {
                anim.SetTrigger("Sit");
            }
        }
    }
}
