using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Worm : Bug {

    Transform head;

    bool killing = false;
    bool canAttach = true;
    public float killDelay = 0.2f;
    public AudioClip reattachSound;

    public List<WormPart> bodyParts;
    List<Vector3> partPositions = new List<Vector3>();
    Vector2 partAnchor;
    Vector2 partConnectedAnchor;

    override protected void Start() {
        base.Start();
        head = transform.Find("Head");
        partAnchor = transform.Find("Body 1").GetComponent<HingeJoint2D>().anchor;
        partConnectedAnchor = transform.Find("Body 1").GetComponent<HingeJoint2D>().connectedAnchor;
        foreach (Transform part in transform) {
            partPositions.Add(part.localPosition);
        }
    }

    protected override void Update() {
        base.Update();
        //Debug.Log(holdingBerry);
    }

    public void Kill(WormPart part) {
        StartCoroutine(KillRoutine(part));
        //KillRoutine(part);
    }

    IEnumerator KillRoutine(WormPart part) {
        if (!killing) {
            killing = true;
            canAttach = false;
            if (!part.isHead && !holdingBerry) {
                for (int i = bodyParts.IndexOf(part); i < bodyParts.Count; i++) {
                    bodyParts[i].Kill();
                    bodyParts[i-1].GetComponent<HingeJoint2D>().enabled = false;
                }
            }
            else {
                if (holdingBerry) {
                    moveSpeed = origMoveSpeed;
                    maxSpeed = origMaxSpeed;
                    berry.GetComponent<SpriteRenderer>().sprite = squishedBerries[Random.Range(0, squishedBerries.Length - 1)];
                    berry.transform.parent = null;
                    holdingBerry = false;
                    GetComponent<AudioSource>().clip = berrySquishSound;
                    GetComponent<AudioSource>().Play();
                    int berryCount = 0;
                    foreach (GameObject b in GameObject.FindGameObjectsWithTag("berry")) {
                        if (b.transform.parent != null && b.transform.parent.tag == "berry tree") {
                            berryCount++;
                        }
                    }
                    if (berryCount + gameManager.bugScore < 3) {
                        gameManager.EndGame(WinnerType.Human);
                    }
                }
                else {
					Instantiate(ghost, bodyParts[0].transform.position + bodyParts[0].GetComponent<SpriteRenderer>().bounds.extents, Quaternion.identity);
                    bodyParts[0].Kill(WormPartStates.Dead);
                    moveSpeed = 0;
                    maxSpeed = 0;
                    GetComponent<AudioSource>().PlayOneShot(deathScream);
                    for (int i = 0; i < bodyParts.Count; i++) {
                        bodyParts[i].Kill(WormPartStates.Dead);
                        bodyParts[i].GetComponent<HingeJoint2D>().enabled = false;
                    }
                    if (gameManager.inGame) {
                        gameManager.EndGame(WinnerType.Human);
                    }
                }
            }
            yield return new WaitForSeconds(killDelay);
            canAttach = true;
            killing = false;
        }
    }

    public void Collide(Collision2D c) {
        base.OnCollisionEnter2D(c);
        Reattach(c);
        //foreach (WormPart part in bodyParts) {
        //    Physics2D.IgnoreCollision(c.gameObject.GetComponent<Collider2D>(), part.GetComponent<Collider2D>());
        //}
    }
    
    void Reattach(Collision2D c) {
        if (c.gameObject.tag == "worm" && !c.gameObject.GetComponent<WormPart>().isHead && c.gameObject.GetComponent<WormPart>().state == WormPartStates.Detatched && canAttach) {
            GetComponent<AudioSource>().PlayOneShot(reattachSound);
            canAttach = false;
            //c.gameObject.GetComponent<WormPart>().canKill = false;
            int insertIndex = 1;
            //int insertIndex = bodyParts.IndexOf(bodyParts.Last(w => w.state == WormPartStates.Alive)) + 1;
            bodyParts.Remove(c.gameObject.GetComponent<WormPart>());
            bodyParts.Insert(insertIndex, c.gameObject.GetComponent<WormPart>());
            c.gameObject.GetComponent<WormPart>().Kill(WormPartStates.Alive);
            c.gameObject.GetComponent<HingeJoint2D>().enabled = false;
            c.transform.localPosition = head.localPosition + partPositions[insertIndex];//partPositions[insertIndex];
            c.transform.localRotation = Quaternion.identity;
            for (int i = 0; i < bodyParts.Count; i++) {
                if (i > 0 && bodyParts[i].GetComponent<WormPart>().state == WormPartStates.Alive) {
                    bodyParts[i].transform.localPosition = head.transform.localPosition + partPositions[i];
                    bodyParts[i].transform.localRotation = Quaternion.identity;
                }
                bodyParts[i].GetComponent<HingeJoint2D>().anchor = partAnchor;
                bodyParts[i].GetComponent<HingeJoint2D>().connectedAnchor = partConnectedAnchor;
                if (i < bodyParts.Count - 1) {
                    bodyParts[i].GetComponent<HingeJoint2D>().connectedBody = bodyParts[i + 1].GetComponent<Rigidbody2D>();
                    if (bodyParts[i + 1].state == WormPartStates.Alive) {
                        bodyParts[i].GetComponent<HingeJoint2D>().enabled = true;
                    }
                }
                bodyParts[bodyParts.Count - 1].GetComponent<HingeJoint2D>().enabled = false;
            }
            //head.transform.localPosition = partPositions[insertIndex];
        }
        canAttach = true;
		//c.gameObject.GetComponent<WormPart>().canKill = true;
    }

    public new void OnTriggerEnter2D(Collider2D c) {
         if (c.tag == "berry" && !holdingBerry) {
             moveSpeed = origMoveSpeed * berrySpeedMultiplier;
             maxSpeed = origMaxSpeed * berrySpeedMultiplier;
             berry = GameObject.Instantiate(c.gameObject, transform.position, Quaternion.identity) as GameObject;
             berry.GetComponent<Collider2D>().enabled = false;
             berry.transform.parent = head.transform;
             berry.transform.localPosition = new Vector3(0, 0, 0);
             holdingBerry = true;
             c.gameObject.SetActive(false);
             GetComponent<AudioSource>().clip = berrySnatchSound;
             GetComponent<AudioSource>().Play();
         }
     }

    override protected void Move() {
        Vector2 velocity = head.GetComponent<Rigidbody2D>().velocity;
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0) {
            if (velocity.x < maxSpeed)
                velocity += Vector2.right * Input.GetAxisRaw("Horizontal");
            else
                velocity = new Vector2(Mathf.Sign(velocity.x), velocity.y);
        }
        else {
            velocity = new Vector2(0f, velocity.y);
        }
        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0) {
            if (Mathf.Abs(velocity.y) < maxSpeed)
                velocity += Vector2.up * Input.GetAxisRaw("Vertical");
            else
                velocity = new Vector2(velocity.x, Mathf.Sign(velocity.y));
        }
        else {
            velocity = new Vector2(velocity.x, 0f);
        }
        velocity = velocity.normalized * moveSpeed;
        head.GetComponent<Rigidbody2D>().velocity = velocity;
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        if (h > 0 && v == 0) {
            head.eulerAngles = new Vector3(0, 0, -90);
        }
        else if (h < 0 && v == 0) {
            head.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (h == 0 && v > 0) {
            head.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (h == 0 && v < 0) {
            head.eulerAngles = new Vector3(0, 0, 180);
        }
        else if (h > 0 && v > 0) {
            head.eulerAngles = new Vector3(0, 0, -45);
        }
        else if (h > 0 && v < 0) {
            head.eulerAngles = new Vector3(0, 0, 225);
        }
        else if (h < 0 && v > 0) {
            head.eulerAngles = new Vector3(0, 0, 45);
        }
        else if (h < 0 && v < 0) {
            head.eulerAngles = new Vector3(0, 0, 135);
        }
    }

}
