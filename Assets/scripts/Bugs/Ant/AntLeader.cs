using UnityEngine;
using System.Collections;

public class AntLeader : Bug {
    public int followerCount = 5;
    public float radius = 5;
    public AntFollower follower;

    protected override void Start() {
        base.Start();
        for (int i = 0; i < followerCount; i++) {
            Vector3 pos = transform.up * radius;
            pos = Quaternion.Euler(0, 0, 360 / radius * i) * pos;
            Debug.Log(pos);
            Instantiate(follower, pos, Quaternion.identity);
        }
    }
}
