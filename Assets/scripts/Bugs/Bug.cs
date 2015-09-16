using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]

public class Bug : MonoBehaviour {

    public float moveSpeed = 2f;
    public float maxSpeed = 5f;
    public float berrySpeedMultiplier = 0.7f;
    public AudioClip gnatEatSound;
    public AudioClip deathScream;
    public AudioClip specialSound;
    public AudioClip berrySnatchSound;
    public AudioClip berryDepositSound;
    public AudioClip berrySquishSound;
    protected bool dead;
    protected bool dying = false;
    public bool holdingBerry { get; protected set; }
    protected GameObject berry = null;
    protected GameManager gameManager;
    public float winningScore = 15;
    public float cooldown = 1f;
    public float finalCooldown = 0.5f;
    public float nextFire = 0.0f;
    public float scoreScale = 2f;
    public GameObject ghost;

    public Sprite[] squishedBerries;

    protected float origMaxSpeed;
    protected float origMoveSpeed;

    virtual protected void Start() {
        holdingBerry = false;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        origMaxSpeed = maxSpeed;
        origMoveSpeed = moveSpeed;
    }

    virtual protected void Move() {
        Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0) {
            if (Mathf.Abs(velocity.x) < maxSpeed)
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
        GetComponent<Rigidbody2D>().velocity = velocity;
        if (Input.GetAxis("Vertical") != 0) {
            GetComponent<Animator>().SetInteger("direction", (int)Mathf.Sign(Input.GetAxis("Vertical")));
            GetComponent<Animator>().SetTrigger("move");
        }
        if (Input.GetAxis("Horizontal") != 0) {
            //GetComponent<Animator>().SetInteger("direction", 1);
            GetComponent<Animator>().SetTrigger("move");
        }
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        if (h == 0 && v == 0) {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - (transform.eulerAngles.z % 45));
        }
        if (h > 0 && v == 0) {
            if (GetComponent<Animator>().GetInteger("direction") > 0)
                transform.eulerAngles = new Vector3(0, 0, -90 * Mathf.Sign(Input.GetAxis("Horizontal")));
            else if (GetComponent<Animator>().GetInteger("direction") < 0)
                transform.eulerAngles = new Vector3(0, 0, 90 * Mathf.Sign(Input.GetAxis("Horizontal")));
        }
        else if (h < 0 && v == 0) {
            if (GetComponent<Animator>().GetInteger("direction") > 0)
                transform.eulerAngles = new Vector3(0, 0, -90 * Mathf.Sign(Input.GetAxis("Horizontal")));
            else if (GetComponent<Animator>().GetInteger("direction") < 0)
                transform.eulerAngles = new Vector3(0, 0, 90 * Mathf.Sign(Input.GetAxis("Horizontal")));
        }
        if (h == 0 && v > 0) {
            transform.rotation = Quaternion.identity;
        }
        else if (h == 0 && v < 0) {
            transform.rotation = Quaternion.identity;
        }
        else if (h > 0 && v > 0) {
            transform.eulerAngles = new Vector3(0, 0, -45);
        }
        else if (h > 0 && v < 0) {
            transform.eulerAngles = new Vector3(0, 0, 45);
        }
        else if (h < 0 && v > 0) {
            transform.eulerAngles = new Vector3(0, 0, 45);
        }
        else if (h < 0 && v < 0) {
            transform.eulerAngles = new Vector3(0, 0, -45);
        }
    }

    // Update is called once per frame
    virtual protected void Update() {
        /*if(score >= winningScore && inGame){
            inGame = false;
            GameObject.Find("GameManager").GetComponent<GameManager>().EndGame(WinnerType.Bug);
        }*/
        if (!dead) {
            Move();
            if (Input.GetButtonDown("Bug Special")) {
                Special();
            }
        }
    }
    public void Kill() {
        if (!dying)
            StartCoroutine(KillSelf());
    }
    protected virtual IEnumerator KillSelf() {
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
            if (gameManager.inGame) {
                GameObject.Find("GameManager").GetComponent<GameManager>().EndGame(WinnerType.Human);
            }
        }
        yield return new WaitForSeconds(0.1f);
        dying = false;
    }

    virtual protected void OnKill() {
        // override in children for events
        Instantiate(ghost, transform.position + GetComponent<SpriteRenderer>().bounds.extents, Quaternion.identity);
    }

    public void Reset() {
        dead = false;
        GetComponent<Animator>().SetTrigger("Reset");

    }
    virtual protected bool Special() {
        if (Time.time < nextFire) {
            return false;
        }
        nextFire = Time.time + cooldown;
        GetComponent<AudioSource>().clip = specialSound;
        GetComponent<AudioSource>().Play();
        Camera.main.GetComponent<CameraShake>().PlayShake(0.5f, 0.5f, 0.05f);
        GetComponent<Animator>().SetTrigger("special");
        return true;
    }
    protected virtual void OnTriggerEnter2D(Collider2D c) {
        if (c.tag == "berry" && !dead && !holdingBerry) {
            moveSpeed = origMoveSpeed * berrySpeedMultiplier;
            maxSpeed = origMaxSpeed * berrySpeedMultiplier;
            berry = GameObject.Instantiate(c.gameObject, transform.position, Quaternion.identity) as GameObject;
            berry.GetComponent<Collider2D>().enabled = false;
            berry.transform.parent = transform;
            holdingBerry = true;
            c.gameObject.SetActive(false);
            GetComponent<AudioSource>().clip = berrySnatchSound;
            GetComponent<AudioSource>().Play();
        }
    }
    protected virtual void OnCollisionEnter2D(Collision2D c) {
        if (c.gameObject.tag == "gnat" && !dead) {
            GetComponent<AudioSource>().clip = gnatEatSound;
            GetComponent<AudioSource>().Play();
            gameManager.ScoreBug(1);
            cooldown = Mathf.Lerp(cooldown, finalCooldown, gameManager.bugScore / winningScore);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(scoreScale, scoreScale), gameManager.bugScore / winningScore);
            Destroy(c.gameObject);
        }
        if (c.gameObject.tag == "web") {
            /*Debug.Log("collision");
            moveSpeed = moveSpeed / 2;
            maxSpeed = maxSpeed / 2;*/
            Physics2D.IgnoreCollision(c.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
        if (c.gameObject.tag == "house" && holdingBerry) {
            maxSpeed = origMaxSpeed;
            moveSpeed = origMoveSpeed;
            berry.transform.parent = c.transform;
            holdingBerry = false;
            gameManager.ScoreBug(1);
            GetComponent<AudioSource>().clip = berryDepositSound;
            GetComponent<AudioSource>().Play();
        }
    }
    void OnCollisionExit2D(Collision2D c) {
        if (c.gameObject.tag == "web") {
            moveSpeed = origMoveSpeed;
            maxSpeed = origMaxSpeed;
        }
    }
}
