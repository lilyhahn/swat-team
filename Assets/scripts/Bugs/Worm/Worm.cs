﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Worm : Bug {

    Transform head;

    bool killing = false;

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
        Debug.Log(holdingBerry);
    }

    public void Kill(WormPart part) {
        StartCoroutine(KillRoutine(part));
    }

    IEnumerator KillRoutine(WormPart part) {
        if (!killing) {
            killing = true;
            if (!part.isHead && !holdingBerry) {
                for (int i = bodyParts.IndexOf(part) - 1; i < bodyParts.Count; i++) {
                    bodyParts[i].GetComponent<HingeJoint2D>().enabled = false;
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
                    bodyParts[0].Kill(WormPartStates.Dead);
                    for (int i = 0; i < bodyParts.Count; i++) {
                        bodyParts[i].Kill(WormPartStates.Dead);
                        bodyParts[i].GetComponent<HingeJoint2D>().enabled = false;
                    }
                    if (gameManager.inGame) {
                        gameManager.EndGame(WinnerType.Human);
                    }
                }
            }
            yield return new WaitForSeconds(1.5f);
            killing = false;
        }
    }

    public void Collide(Collision2D c) {
        base.OnCollisionEnter2D(c);
        if (c.gameObject.tag == "worm" && !c.gameObject.GetComponent<WormPart>().isHead && c.gameObject.GetComponent<WormPart>().state == WormPartStates.Detatched) {
            int insertIndex = bodyParts.IndexOf(bodyParts.Last(w => w.state == WormPartStates.Alive)) + 1;
            bodyParts.Remove(c.gameObject.GetComponent<WormPart>());
            try {
                bodyParts.Insert(insertIndex, c.gameObject.GetComponent<WormPart>());
            }
            catch (System.InvalidOperationException) {
                insertIndex = 1;
                bodyParts.Insert(insertIndex, c.gameObject.GetComponent<WormPart>());
            }
            c.gameObject.GetComponent<WormPart>().Kill(WormPartStates.Alive);
            for (int i = 0; i < insertIndex; i++) {
                bodyParts[i].transform.localPosition = partPositions[i];
                bodyParts[i].transform.localRotation = Quaternion.identity;
                bodyParts[i].GetComponent<HingeJoint2D>().anchor = partAnchor;
                bodyParts[i].GetComponent<HingeJoint2D>().connectedAnchor = partConnectedAnchor;
                if (i < bodyParts.Count - 1) {
                    bodyParts[i].GetComponent<HingeJoint2D>().connectedBody = bodyParts[i + 1].GetComponent<Rigidbody2D>();
                    if (bodyParts[i + 1].state == WormPartStates.Alive) {
                        bodyParts[i].GetComponent<HingeJoint2D>().enabled = true;
                    }
                }
            }
        }
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