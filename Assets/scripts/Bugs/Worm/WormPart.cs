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
    public float attatchDelay = 0.5f;

    public WormPartStates state { get; private set; }

    bool killing = false;

    public void Awake() {
        state = WormPartStates.Alive;
    }

    public void Kill() {
        if (!killing)
            StartCoroutine(KillRoutine());
    }
    public void Kill(WormPartStates killState) {
        if (!isHead && !transform.parent.GetComponent<Worm>().holdingBerry) {
            switch (killState) {
                case WormPartStates.Alive:
                    state = WormPartStates.Alive;
                    break;
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
    }
    IEnumerator KillRoutine() {
        killing = true;
        if (!isHead && !transform.parent.GetComponent<Worm>().holdingBerry) {
            switch (state) {
                case WormPartStates.Alive:
                    transform.parent.GetComponent<Worm>().Kill(this);
                    yield return new WaitForSeconds(attatchDelay);
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
        else if(isHead){
            transform.parent.GetComponent<Worm>().Kill(this);
        }
        yield return new WaitForSeconds(0.2f);
        killing = false;
    }
    //void OnTriggerExit2D(Collider2D c) {
    //    if(c.tag == "swatter")
    //        killing = false;
    //}

    void OnCollisionEnter2D(Collision2D c) {
        transform.parent.GetComponent<Worm>().Collide(c);
    }

    void OnTriggerEnter2D(Collider2D c) {
        transform.parent.GetComponent<Worm>().OnTriggerEnter2D(c);
    }
}
