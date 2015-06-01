using UnityEngine;
using System.Collections;

public enum WormPartStates {
    Alive,
    Detatched,
    Dead,
    Deader
}

public class WormPart : MonoBehaviour {
    public Sprite dead;
    public Sprite deader;
    public bool isHead;

    WormPartStates state = WormPartStates.Alive;

    bool killing = false;

    public void Kill() {
        if (!killing)
            KillRoutine();
    }
    public void Kill(WormPartStates killState) {
        switch (killState) {
            case WormPartStates.Detatched:
                transform.parent.GetComponent<Worm>().Kill(this);
                state = WormPartStates.Detatched;
                break;
            case WormPartStates.Dead:
                GetComponent<SpriteRenderer>().sprite = dead;
                GetComponent<Rigidbody2D>().isKinematic = true;
                state = WormPartStates.Dead;
                break;
            case WormPartStates.Deader:
                GetComponent<SpriteRenderer>().sprite = deader;
                state = WormPartStates.Deader;
                break;
        }
    }
    void KillRoutine() {
        killing = true;
        switch (state) {
            case WormPartStates.Alive:
                transform.parent.GetComponent<Worm>().Kill(this);
                state = WormPartStates.Detatched;
                break;
            case WormPartStates.Detatched:
                GetComponent<SpriteRenderer>().sprite = dead;
                GetComponent<Rigidbody2D>().isKinematic = true;
                state = WormPartStates.Dead;
                break;
            case WormPartStates.Dead:
                GetComponent<SpriteRenderer>().sprite = deader;
                state = WormPartStates.Deader;
                break;
        }
    }
    void OnTriggerExit2D() {
        killing = false;
    }
}
