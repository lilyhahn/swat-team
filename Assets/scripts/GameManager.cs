using UnityEngine;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public enum WinnerType {
    Human,
    Bug
}

public enum ModeType {
    Gnat,
    Berry
}

public enum DirectionType {
    Forward,
    Backward
}

[System.Serializable]
public class MenuScreen{
    public Vector3 InitialPosition;
    public Vector3 InitialScale;
    public Vector3[] FinalPosition;
    public Vector3[] FinalScale;
    public GameObject MenuObject;
    public GameObject[] AlphaObjects;
    public MenuButton[] Buttons;
}

[System.Serializable]
public class MenuButton {
    public GameObject ButtonObject;
    public MenuAction Action;
}

[System.Serializable]
public class MenuAction {
    public DirectionType Direction;
    public ModeType ModeArg = 0;
    public string MethodName;
    public void Activate(GameManager gameManager){
        MethodInfo buttonMethod = gameManager.GetType().GetMethod(this.MethodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (buttonMethod != null) {
            buttonMethod.Invoke(gameManager, new object[] { this.Direction, this.ModeArg });
        }
    }
}

public class GameManager : MonoBehaviour {
	public TextMesh countdown;
    public float randomSpawnRadius = 1;
	public bool inGame = false;
    public float preMenuTime = 3f;
    public GameObject preMenuText;
    public Vector3 mainMenuCamera;
    public float mainMenuZoom;
	public Vector3 modeSelectCamera;
	public float modeSelectZoom;
	public GameObject modeSelectText;
	public AudioSource voiceAudio;
	public AudioClip[] gnatVoices;
	public AudioClip[] berryVoices;
	public AudioClip beep12;
	public AudioClip beep3;
    public AudioClip menuMusic;
	public int[] winningScores; // in order of mode number
	public int bugScore {get; private set;} // for bug
    public Vector3 characterSelectCamera;
    public float characterSelectCameraZoom;
    public GameObject characterSelectText;
    public Vector3 stageSelectCamera;
    public float stageSelectCameraZoom;
    public GameObject stageSelectText;
    public Vector3 inGameCamera;
    public float inGameCameraZoom;
    public AudioClip inGameMusic;
    float inGameMusicTime = 0;
    public GameObject gameOverText;
    public GameObject[] characters;
    public GameObject[] characterProfiles;
    public Sprite[] originalCharacterProfiles;
    public Sprite[] finalCharacterProfiles;
    public GameObject[] swatters;
    public GameObject[] swatterProfiles;
    public Sprite[] originalSwatterProfiles;
    public Sprite[] finalSwatterProfiles;
    public AudioClip humanWinJingle;
    public AudioClip bugWinJingle;
    public GameObject gnatSpawner;
    public GameObject berryMode;
    public GameObject[] berryOutlines;
    public float restartDelay = 0.5f;
    public bool showControls = true;
    public int controlPromptTime = 604800;
    public float menuTransitionTime = 0.5f;
    public List<MenuScreen> menu;
    public Borders bugBorders;
    public Borders swatterBorders;

    float endTime;
    bool bugReady = false;
    bool swatterReady = false;
    int swatter = 0;
    int character = 0;
	ModeType mode = 0;
    GameObject hand;
    bool swatterScrolling = false;
    bool bugScrolling = false;
    bool selectingCharacterActive = false;
    public bool paused { get; private set; }
    bool resuming = false;
    enum StateType {
        MainMenu,
		SelectingMode,
        SelectingCharacter,
        InGame,
        GameOver,
        Credits
    }
    StateType state = StateType.MainMenu;
    MenuButton currentBugButton;
    int buttonIndex = 0;
    bool bugAxisDown;
    bool swatterAxisDown;
    void Start() {
        if (PlayerPrefs.HasKey("LastPlayed") && System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds - PlayerPrefs.GetInt("LastPlayed") >= controlPromptTime) {
            showControls = true;
        }
        else if (!PlayerPrefs.HasKey("LastPlayed")) {
            showControls = true;
        }
        else {
            showControls = false;
        }
        PlayerPrefs.SetInt("LastPlayed", (int)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
        paused = true;
        //foreach (Transform l in GameObject.Find("levels").transform) {
        //    l.gameObject.SetActive(false);
        //}
        //GameObject.Find("levels").transform.Find("level" + (int)(Random.Range(1f, 4f))).gameObject.SetActive(true);
        
        //StartCoroutine(PreMenuTransistion());
        Camera.main.GetComponent<Borders>().DrawBorders(menu[(int)state].Buttons[0].ButtonObject.GetComponent<BoxCollider2D>());
        currentBugButton = menu[(int)state].Buttons[buttonIndex];
    }
    void Update() {
        Camera.main.GetComponent<Borders>().DrawBorders(currentBugButton.ButtonObject.GetComponent<BoxCollider2D>());
        if((Mathf.Abs(Input.GetAxisRaw("Horizontal (Bug Menu)")) > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical (Bug Menu)")) > 0) && !bugAxisDown){
            bugAxisDown = true;
            buttonIndex += (int) Input.GetAxisRaw("Horizontal (Bug Menu)");
            buttonIndex += (int) Input.GetAxisRaw("Vertical (Bug Menu)");
            if(buttonIndex < 0){
                buttonIndex = menu[(int)state].Buttons.Length - 1;
            }
            if(buttonIndex >= menu[(int)state].Buttons.Length){
                buttonIndex = 0;
            }
            currentBugButton = menu[(int)state].Buttons[buttonIndex];
            Camera.main.GetComponent<Borders>().DrawBorders(currentBugButton.ButtonObject.GetComponent<BoxCollider2D>());
        }
        if(Mathf.Abs(Input.GetAxisRaw("Horizontal (Bug Menu)")) == 0 && Mathf.Abs(Input.GetAxisRaw("Vertical (Bug Menu)")) == 0){
            bugAxisDown = false;
        }
        if(Input.GetButtonDown("Submit (Bug)")){
            currentBugButton.Action.Activate(this);
        }
        if(Input.GetButtonDown("Submit (Swatter Mouse)")){
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << LayerMask.NameToLayer(state.ToString()));
            if(hit.collider != null){
                MenuButton buttonHit = null;
                try {
                    buttonHit = menu.First(m => m.MenuObject == hit.transform.parent.gameObject).Buttons.First(b => b.ButtonObject == hit.transform.gameObject);
                }
                catch (System.InvalidOperationException) {
                    buttonHit = null;
                }
                if (buttonHit != null) {
                    buttonHit.Action.Activate(this);
                }
                
            }
        }
        switch (state) {
            case StateType.SelectingCharacter:
                characterProfiles[character].SetActive(false);
                if (Input.GetAxisRaw("Vertical (Bug Menu)") > 0 && !bugReady && !bugScrolling) {
                    character++;
                    bugScrolling = true;
                }
                if (Input.GetAxisRaw("Vertical (Bug Menu)") < 0 && !bugReady && !bugScrolling) {
                    character--;
                    bugScrolling = true;
                }
                if (Input.GetAxisRaw("Vertical (Bug Menu)") == 0) {
                    bugScrolling = false;
                }
                if (character >= characters.Length)
                    character = 0;
                if (character < 0)
                    character = characters.Length - 1;
                characterProfiles[character].SetActive(true);

                swatterProfiles[swatter].SetActive(false);
                if (Input.GetAxisRaw("Vertical (Swatter Menu)") > 0 && !swatterReady) {
                    if (!swatterScrolling) {
                        swatter++;
                        swatterScrolling = true;
                    }
                }
                if (Input.GetAxisRaw("Vertical (Swatter Menu)") < 0 && !swatterReady) {
                    if (!swatterScrolling) {
                        swatter--;
                        swatterScrolling = true;
                    }
                }
                if(Input.GetAxisRaw("Vertical (Swatter Menu)") == 0)
                        swatterScrolling = false;
                if (swatter >= swatters.Length)
                    swatter = 0;
                if (swatter < 0)
                    swatter = swatters.Length - 1;
                swatterProfiles[swatter].SetActive(true);
                if (Input.GetButtonDown("Submit (Bug)")) {
                    selectingCharacterActive = true;
                    characterProfiles[character].transform.Find("egg").GetComponent<SpriteRenderer>().sprite = finalCharacterProfiles[character];
                    bugReady = true;
                }
                if (Input.GetButtonDown("Submit (Swatter)")) {
                    selectingCharacterActive = true;
                    swatterProfiles[swatter].transform.Find("box").GetComponent<SpriteRenderer>().sprite = finalSwatterProfiles[swatter];
                    swatterReady = true;
                }
                if (swatterReady && bugReady) {
                    state = StateType.InGame;
                    characterSelectText.SetActive(false);
                    //stageSelectText.SetActive(true);
                    StartCoroutine(StartGame());
                }
                if (Input.GetButtonDown("Cancel (Bug)") && bugReady) {
                    characterProfiles[character].transform.Find("egg").GetComponent<SpriteRenderer>().sprite = originalCharacterProfiles[character];
                    bugReady = false;
                }
                if (Input.GetButtonDown("Cancel (Swatter)") && swatterReady) {
                    swatterProfiles[swatter].transform.Find("box").GetComponent<SpriteRenderer>().sprite = originalSwatterProfiles[swatter];
                    swatterReady = false;
                }
                break;
            case StateType.GameOver:
                if (Input.anyKeyDown && !Input.GetButtonDown("Cancel") && Time.time > endTime + restartDelay) {
                    Cleanup();
                    StartCoroutine(StartGame());
                }
                if (Input.GetButtonDown("Cancel")) {
                    Cleanup();
                    GetComponent<AudioSource>().clip = menuMusic;
                    GetComponent<AudioSource>().Play();
                    LeanTween.move(Camera.main.gameObject, characterSelectCamera, 0.5f);
                    LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, characterSelectCameraZoom, 0.5f);
                    characterProfiles[character].transform.Find("egg").GetComponent<SpriteRenderer>().sprite = originalCharacterProfiles[character];
                    bugReady = false;
                    swatterProfiles[swatter].transform.Find("box").GetComponent<SpriteRenderer>().sprite = originalSwatterProfiles[swatter];
                    swatterReady = false;
                    characterSelectText.SetActive(true);
                    state = StateType.SelectingCharacter;

                }
                break;
        }
		if(Input.GetButtonDown ("Cancel")){
			switch(state){
                case StateType.InGame:
                    if (!paused) {
                        Time.timeScale = 0;
                        paused = true;
                    }
                    else {
                        StartCoroutine(Resume());
                    }
                    break;
			    case StateType.MainMenu:
				    Application.Quit();
				    break;
			}
		}
    }
    void UpdateZoom(float val) {
        Camera.main.orthographicSize = val;
    }
    IEnumerator StartGame(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        paused = true;
		GetComponent<AudioSource>().Stop ();
        Instantiate(characters[character], Random.insideUnitCircle * randomSpawnRadius, Quaternion.identity);
        hand = Instantiate(swatters[swatter]) as GameObject;
		inGame = true;
		state = StateType.InGame;
		LeanTween.move(Camera.main.gameObject, inGameCamera, 0.5f);
		LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, inGameCameraZoom, 0.5f);
		if(mode == ModeType.Berry){
			berryMode.SetActive(true);
            foreach(GameObject outline in berryOutlines){
                outline.SetActive(true);
            }
		}
		countdown.gameObject.SetActive(true);
    	countdown.text = "3";
    	GetComponent<AudioSource>().PlayOneShot(beep12);
    	yield return new WaitForSeconds(1f);
    	countdown.text = "2";
		GetComponent<AudioSource>().PlayOneShot(beep12);
    	yield return new WaitForSeconds(1f);
    	countdown.text = "1";
		GetComponent<AudioSource>().PlayOneShot(beep3);
    	yield return new WaitForSeconds(1f);
    	countdown.gameObject.SetActive(false);
		GetComponent<AudioSource>().clip = inGameMusic;
		GetComponent<AudioSource>().time = inGameMusicTime;
		GetComponent<AudioSource>().Play();
		if(mode == 0){
			gnatSpawner.SetActive(true);
		}
        paused = false;
	}
    public void EndGame(WinnerType winner) {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    	inGame = false;
        inGameMusicTime = GetComponent<AudioSource>().time;
        endTime = Time.time;
        gameOverText.SetActive(true);
        state = StateType.GameOver;
        if (winner == WinnerType.Human) {
            gameOverText.transform.Find("human").gameObject.SetActive(true);
            GetComponent<AudioSource>().clip = humanWinJingle;
        }
        else if (winner == WinnerType.Bug) {
            gameOverText.transform.Find("bug").gameObject.SetActive(true);
            GetComponent<AudioSource>().clip = bugWinJingle;
        }
        GetComponent<AudioSource>().time = 0f;
        GetComponent<AudioSource>().Play();
    }
    public void ScoreBug(int amount){
    	bugScore += amount;
    	if(bugScore == winningScores[(int)mode]){
    		EndGame(WinnerType.Bug);
    	}
    }
    IEnumerator PreMenuTransistion() {
        yield return new WaitForSeconds(preMenuTime);
        Camera.main.cullingMask = LayerMask.NameToLayer("Everything");
        preMenuText.SetActive(false);
        state = StateType.MainMenu;
    }

    void Cleanup() {
        bugScore = 0;
        gameOverText.SetActive(false);
        //Destroy(bug);
        foreach (GameObject bug in GameObject.FindGameObjectsWithTag("bug")) {
            Destroy(bug);
        }
        Destroy(hand);
        Destroy(GameObject.FindGameObjectWithTag("web"));
        gnatSpawner.SetActive(false);
        foreach (GameObject dead in GameObject.FindGameObjectsWithTag("dead")) {
            Destroy(dead);
        }
        foreach (GameObject gnat in GameObject.FindGameObjectsWithTag("gnat")) {
            Destroy(gnat);
        }
        foreach (GameObject berry in GameObject.FindGameObjectsWithTag("berry")) {
            if (berry.transform.parent != null && berry.transform.parent.tag != "berry tree")
                Destroy(berry);
            if (berry.transform.parent == null)
                Destroy(berry);
        }
        foreach (GameObject hit in GameObject.FindGameObjectsWithTag("hit")) {
            Destroy(hit);
        }
        foreach (Transform berryTree in berryMode.transform.Find("berryTrees")) {
            foreach (Transform berry in berryTree) {
                if (berry.tag == "berry") {
                    berry.gameObject.SetActive(true);
                }
            }
        }
        gameOverText.transform.Find("human").gameObject.SetActive(false);
        gameOverText.transform.Find("bug").gameObject.SetActive(false);
    }
    IEnumerator Resume() {
        if (!resuming) {
            resuming = true;
            foreach (GameObject bug in GameObject.FindGameObjectsWithTag("bug")) {
                bug.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            foreach (GameObject gnat in GameObject.FindGameObjectsWithTag("gnat")) {
                gnat.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            Time.timeScale = 1f;
            countdown.gameObject.SetActive(true);
            countdown.text = "3";
            GetComponent<AudioSource>().PlayOneShot(beep12);
            yield return new WaitForSeconds(1f);
            countdown.text = "2";
            GetComponent<AudioSource>().PlayOneShot(beep12);
            yield return new WaitForSeconds(1f);
            countdown.text = "1";
            GetComponent<AudioSource>().PlayOneShot(beep3);
            yield return new WaitForSeconds(1f);
            countdown.gameObject.SetActive(false);
            paused = false;
            resuming = false;
        }
    }
    void PerformTransition(MenuScreen screen, DirectionType direction, int positionIndex = 0) {
        switch (direction) {
            case DirectionType.Forward:
                LeanTween.moveLocal(screen.MenuObject, screen.FinalPosition[positionIndex], menuTransitionTime);
                LeanTween.scale(screen.MenuObject, screen.FinalScale[positionIndex], menuTransitionTime);
                foreach (GameObject a in screen.AlphaObjects) {
                    LeanTween.alpha(a, 0f, menuTransitionTime);
                }
                buttonIndex = 0;
                currentBugButton = menu[(int)state + 1].Buttons[buttonIndex];
                break;
            case DirectionType.Backward:
                LeanTween.moveLocal(screen.MenuObject, screen.InitialPosition, menuTransitionTime);
                LeanTween.scale(screen.MenuObject, screen.InitialScale, menuTransitionTime);
                foreach (GameObject a in screen.AlphaObjects) {
                    LeanTween.alpha(a, 1f, menuTransitionTime);
                }
                int screenIndex = menu.FindIndex(delegate(MenuScreen s){return s.MenuObject == screen.MenuObject;});
                if(screenIndex > 0){
                    MenuScreen lastMenu = menu[screenIndex - 1];
                    LeanTween.moveLocal(lastMenu.MenuObject, lastMenu.InitialPosition, menuTransitionTime);
                    LeanTween.scale(lastMenu.MenuObject, lastMenu.InitialScale, menuTransitionTime);
                    foreach (GameObject a in lastMenu.AlphaObjects) {
                        LeanTween.alpha(a, 1f, menuTransitionTime);
                    }
                }
                buttonIndex = 0;
                currentBugButton = menu[(int)state - 1].Buttons[buttonIndex];
                break;
        }
        Camera.main.GetComponent<Borders>().DrawBorders(currentBugButton.ButtonObject.GetComponent<BoxCollider2D>());
    }
    void MainMenuTransition(DirectionType direction, ModeType mode = 0) {
        StartCoroutine(MainMenuTransitionRoutine(direction));
    }
    IEnumerator MainMenuTransitionRoutine(DirectionType direction) {
        switch (direction) {
            case DirectionType.Forward:
               Animator doorknob = menu[(int)state].MenuObject.transform.Find("Doorknob").GetComponent<Animator>();
               doorknob.SetTrigger("open");
                //menu[(int)state].MenuObject.transform.Find("Doorknob").GetComponent<Animator>().enabled = true;
                while(doorknob.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base.doorknob")){
                    Debug.Log("doorknob anim playing");
                    yield return null;
                }
                PerformTransition(menu[(int)state], DirectionType.Forward);
                state = StateType.SelectingMode;
                break;
            case DirectionType.Backward:
                Application.Quit();
                break;
        }
        
    }
    void ModeSelectTransition(DirectionType direction, ModeType selectedMode) {
        switch (direction) {
            case DirectionType.Forward:
                PerformTransition(menu[(int)state], DirectionType.Forward, (int)selectedMode);
                mode = selectedMode;
                state = StateType.SelectingCharacter;
                break;
            case DirectionType.Backward:
                PerformTransition(menu[(int)state], DirectionType.Backward);
                //menu[(int)state - 1].MenuObject.transform.Find("Doorknob").GetComponent<Animator>().Play("doorknob", -1, 0f);
                //menu[(int)state - 1].MenuObject.transform.Find("Doorknob").GetComponent<Animator>().enabled = false;
                state = StateType.MainMenu;
                break;
        }
    }
}
