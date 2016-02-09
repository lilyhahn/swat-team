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
    public float killDelay = 0.5f;
	public float explodeCoefficient = 3f;

    //public WormPartStates state { get; private set; }
    public WormPartStates state;
    
    bool killing = false;
    public bool canKill = true;

    public void Awake() {
        state = WormPartStates.Alive;
    }

    public void Kill() {
        if (!killing)
            StartCoroutine(KillRoutine());
            //KillRoutine();
    }
    public void Kill(WormPartStates killState) {
        //if (!isHead) {
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
        //}
    }
    IEnumerator KillRoutine() {
        killing = true;
        if (!isHead && canKill) {
            canKill = false;
            switch (state) {
                case WormPartStates.Alive:
                    transform.parent.GetComponent<Worm>().Kill(this);
                    GetComponent<HingeJoint2D>().enabled = false;
					GetComponent<Rigidbody2D>().AddForce(-transform.up * explodeCoefficient);
					Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bug"), LayerMask.NameToLayer("Bug"));
                    state = WormPartStates.Detatched;
                    yield return new WaitForSeconds(killDelay);
                    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bug"), LayerMask.NameToLayer("Bug"), false);
                    canKill = true;
                    break;
                case WormPartStates.Detatched:
                    GetComponent<SpriteRenderer>().sprite = dead;
                    GetComponent<Rigidbody2D>().isKinematic = true;
                    state = WormPartStates.Dead;
                    yield return new WaitForSeconds(killDelay);
                    canKill = true;
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
        //yield return new WaitForSeconds(1f);
        killing = false;
    }

    void OnCollisionEnter2D(Collision2D c) {
        if(state == WormPartStates.Alive)
            transform.parent.GetComponent<Worm>().Collide(c);
    }

    void OnTriggerEnter2D(Collider2D c) {
        transform.parent.GetComponent<Worm>().OnTriggerEnter2D(c);
    }

    void Update() {
        //Debug.Log(gameObject.name + " " + state);
        if(GetComponent<HingeJoint2D>().connectedBody.GetComponent<WormPart>().state != WormPartStates.Alive){
        	GetComponent<HingeJoint2D>().enabled = false;
        }
        if (state != WormPartStates.Alive) {
            GetComponent<HingeJoint2D>().enabled = false;
        }
    }
}
