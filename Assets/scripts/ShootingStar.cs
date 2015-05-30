using UnityEngine;
using System.Collections;

public class ShootingStar : MonoBehaviour {
    public Vector2 bounds;
    public float minDelay = 0f;
    public float maxDelay = 30f;
    Vector3 center;

    void Start() {
        center = transform.position;
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot() {
        while (true) {
            transform.position = new Vector3(Random.Range(center.x - bounds.x, center.x + bounds.x), Random.Range(center.y - bounds.y, center.y + bounds.y));
            GetComponent<Animator>().SetTrigger("shoot");
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }

}
