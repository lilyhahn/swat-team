using UnityEngine;
using System.Collections;

public class AntFollower : Bug {
    protected override IEnumerator KillSelf() {
        dying = true;
        OnKill();
        if (holdingBerry) {
            maxSpeed = origMaxSpeed;
            moveSpeed = origMoveSpeed;
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
            GetComponent<Rigidbody2D>().isKinematic = true;
            dead = true;
            GetComponent<AudioSource>().clip = deathScream;
            GetComponent<AudioSource>().Play();
            GetComponent<Animator>().SetTrigger("dead");
        }
        yield return new WaitForSeconds(0.1f);
        dying = false;
    }
}
