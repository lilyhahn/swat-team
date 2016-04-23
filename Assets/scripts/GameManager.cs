using UnityEngine;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public enum WinnerType {
    Human,
    Bug
}

public enum SelectionArgument {
    Gnat,
    Berry,
    Anvil,
    Defibrilator,
    Old,
    Petit,
    Ant,
    Lady,
    Spider,
    Wasp,
    Worm
}

public enum SelectionType{
    Mode,
    Bug,
    Swatter
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
    public MenuButton[] SwatterButtons;
    public MenuButton[] BugButtons;
}

[System.Serializable]
public class MenuButton {
    public GameObject ButtonObject;
    public MenuAction Action;
}

[System.Serializable]
public class MenuAction {
    public DirectionType Direction;
    public SelectionArgument ModeArg = 0;
    public SelectionType Type;
    public string MethodName;
    public void Activate(GameManager gameManager){
        MethodInfo buttonMethod = gameManager.GetType().GetMethod(this.MethodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (buttonMethod != null) {
            buttonMethod.Invoke(gameManager, new object[] { this.Direction, this.ModeArg, this.Type });
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
    
    public Sprite[] unselectedSprites;
    public Sprite[] selectedSprites;

    float endTime;
    bool bugReady = false;
    bool swatterReady = false;
    SelectionArgument selectedSwatter = SelectionArgument.Old;
    SelectionArgument selectedBug = SelectionArgument.Ant;
	SelectionArgument mode = 0;
    GameObject hand;
    bool swatterScrolling = false;
    bool bugScrolling = false;
    bool selectingCharacterActive = false;
    public bool paused { get; private set; }
    bool resuming = false;
    enum StateType {
        PreMenu,
        MainMenu,
		SelectingMode,
        SelectingCharacter,
        InGame,
        GameOver,
        Credits
    }
    StateType state = StateType.PreMenu;
    MenuButton currentBugButton;
    MenuButton currentSwatterButton;
    int bugButtonIndex = 0;
    int swatterButtonIndex = 0;
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
        bugBorders.DrawBorders(menu[(int)state].BugButtons[0].ButtonObject.GetComponent<BoxCollider2D>());
        currentBugButton = menu[(int)state].BugButtons[bugButtonIndex];
    }
    void Update() {
        bugBorders.DrawBorders(currentBugButton.ButtonObject.GetComponent<BoxCollider2D>());
        if((Mathf.Abs(Input.GetAxisRaw("Horizontal (Bug Menu)")) > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical (Bug Menu)")) > 0) && !bugAxisDown && !bugReady && state != StateType.InGame){
            bugAxisDown = true;
            bugButtonIndex += (int) Input.GetAxisRaw("Horizontal (Bug Menu)");
            bugButtonIndex += (int) Input.GetAxisRaw("Vertical (Bug Menu)");
            if(bugButtonIndex < 0){
                bugButtonIndex = menu[(int)state].BugButtons.Length - 1;
            }
            if(bugButtonIndex >= menu[(int)state].BugButtons.Length){
                bugButtonIndex = 0;
            }
            currentBugButton = menu[(int)state].BugButtons[bugButtonIndex];
            bugBorders.DrawBorders(currentBugButton.ButtonObject.GetComponent<BoxCollider2D>());
        }
        if(Mathf.Abs(Input.GetAxisRaw("Horizontal (Bug Menu)")) == 0 && Mathf.Abs(Input.GetAxisRaw("Vertical (Bug Menu)")) == 0){
            bugAxisDown = false;
        }
        if(Input.GetButtonDown("Submit (Bug)")){
            currentBugButton.Action.Activate(this);
        }
        if((Mathf.Abs(Input.GetAxisRaw("Horizontal (Bug Menu)")) > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical (Bug Menu)")) > 0) && !bugAxisDown && bugReady){
            CharacterSelectTransition(DirectionType.Backward, currentBugButton.Action.ModeArg, SelectionType.Bug);
        }
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << LayerMask.NameToLayer(state.ToString()));
        MenuButton buttonHit = null;
        if(hit.collider != null && !swatterReady){
            try {
                buttonHit = menu.First(m => m.MenuObject == hit.transform.parent.gameObject).SwatterButtons.First(b => b.ButtonObject == hit.transform.gameObject);
            }
            catch (System.InvalidOperationException) {
                buttonHit = null;
            }
            if (buttonHit != null) {
                swatterBorders.DrawBorders(buttonHit.ButtonObject.GetComponent<BoxCollider2D>());
                currentSwatterButton = buttonHit;
                swatterButtonIndex = System.Array.FindIndex(menu[(int)state].SwatterButtons, delegate(MenuButton b){return b.ButtonObject == hit.transform.gameObject;});
            }
        }
        if(Input.GetButtonDown("Submit (Swatter Mouse)") && buttonHit != null && !swatterReady){
            buttonHit.Action.Activate(this);
        }
        if(Input.GetButtonDown("Cancel (Swatter)") && swatterReady){
            CharacterSelectTransition(DirectionType.Backward, currentSwatterButton.Action.ModeArg, SelectionType.Swatter);
        }
        if(Input.anyKeyDown && state == StateType.PreMenu){
            Camera.main.cullingMask = LayerMask.NameToLayer("Everything");
            PerformTransition(menu[(int)state], DirectionType.Forward);
            state = StateType.MainMenu;
        }
        switch (state) {
            case StateType.GameOver:
                if (Input.anyKeyDown && !Input.GetButtonDown("Cancel") && Time.time > endTime + restartDelay) {
                    Cleanup();
                    StartCoroutine(StartGame());
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
                case StateType.GameOver:
                    PerformTransition(menu[(int)StateType.InGame], DirectionType.Backward);
                    Cleanup();
                    bugBorders.gameObject.SetActive(true);
                    swatterBorders.gameObject.SetActive(true);
                    state = StateType.SelectingCharacter;
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
        Instantiate(characters[(int)selectedBug], Random.insideUnitCircle * randomSpawnRadius, Quaternion.identity);
        hand = Instantiate(characters[(int)selectedSwatter]) as GameObject;
		inGame = true;
		state = StateType.InGame;
		if(mode == SelectionArgument.Berry){
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
		if(mode == SelectionArgument.Gnat){
			gnatSpawner.SetActive(true);
		}
        paused = false;
	}
    public void EndGame(WinnerType winner) {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
        bugReady = false;
        swatterReady = false;
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
        bool hasBugButtons = false;
        bool hasSwatterButtons = false;
        switch (direction) {
            case DirectionType.Forward:
                LeanTween.moveLocal(screen.MenuObject, screen.FinalPosition[positionIndex], menuTransitionTime);
                LeanTween.scale(screen.MenuObject, screen.FinalScale[positionIndex], menuTransitionTime);
                foreach (GameObject a in screen.AlphaObjects) {
                    LeanTween.alpha(a, 0f, menuTransitionTime);
                }
                hasBugButtons = menu[(int)state + 1].BugButtons.Length > 0;
                hasSwatterButtons = menu[(int)state + 1].SwatterButtons.Length > 0;
                if(hasBugButtons){
                    bugButtonIndex = 0;
                    currentBugButton = menu[(int)state + 1].BugButtons[bugButtonIndex];
                }
                if(hasSwatterButtons){
                    swatterButtonIndex = 0;
                    currentSwatterButton = menu[(int)state + 1].SwatterButtons[swatterButtonIndex];
                }
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
                hasBugButtons = menu[(int)state - 1].BugButtons.Length > 0;
                hasSwatterButtons = menu[(int)state - 1].SwatterButtons.Length > 0;
                if(hasBugButtons){
                    bugButtonIndex = 0;
                    currentBugButton = menu[(int)state - 1].BugButtons[bugButtonIndex];
                }
                if(hasSwatterButtons){
                    swatterButtonIndex = 0;
                    currentSwatterButton = menu[(int)state - 1].SwatterButtons[swatterButtonIndex];
                }
                break;
        }
        if(hasBugButtons)
            bugBorders.DrawBorders(currentBugButton.ButtonObject.GetComponent<BoxCollider2D>());
        if(hasSwatterButtons)
            swatterBorders.DrawBorders(currentSwatterButton.ButtonObject.GetComponent<BoxCollider2D>());
    }
    void MainMenuTransition(DirectionType direction, SelectionArgument mode = 0, SelectionType type = 0) {
        StartCoroutine(MainMenuTransitionRoutine(direction));
    }
    IEnumerator MainMenuTransitionRoutine(DirectionType direction) {
        switch (direction) {
            case DirectionType.Forward:
               Animator doorknob = menu[(int)state].MenuObject.transform.Find("Doorknob").GetComponent<Animator>();
               doorknob.SetTrigger("open");
                //menu[(int)state].MenuObject.transform.Find("Doorknob").GetComponent<Animator>().enabled = true;
                while(doorknob.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.doorknob")){
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
    void ModeSelectTransition(DirectionType direction, SelectionArgument selectedMode, SelectionType type = 0) {
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
    void CharacterSelectTransition(DirectionType direction, SelectionArgument selectedCharacter, SelectionType type){
        switch(direction){
            case DirectionType.Forward:
                switch(type){
                    case SelectionType.Bug:
                        bugReady = true;
                        selectedBug = selectedCharacter;
                        currentBugButton.ButtonObject.GetComponent<SpriteRenderer>().sprite = selectedSprites[(int)selectedCharacter];
                        currentBugButton.ButtonObject.GetComponent<BoxCollider2D>().size = currentBugButton.ButtonObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
                        break;
                    case SelectionType.Swatter:
                        swatterReady = true;
                        selectedSwatter = selectedCharacter;
                        currentSwatterButton.ButtonObject.GetComponent<SpriteRenderer>().sprite = selectedSprites[(int)selectedCharacter];
                        currentBugButton.ButtonObject.GetComponent<BoxCollider2D>().size = currentBugButton.ButtonObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
                        break;
                }
                if(bugReady && swatterReady){
                    PerformTransition(menu[(int)state], DirectionType.Forward);
                    bugBorders.gameObject.SetActive(false);
                    swatterBorders.gameObject.SetActive(false);
                    state = StateType.InGame;
                    StartCoroutine(StartGame());
                }
                break;
            case DirectionType.Backward:
                if(!bugReady && !swatterReady){
                    PerformTransition(menu[(int)state], DirectionType.Backward);
                    state = StateType.SelectingMode;
                }
                switch(type){
                    case SelectionType.Bug:
                        bugReady = false;
                        currentBugButton.ButtonObject.GetComponent<SpriteRenderer>().sprite = unselectedSprites[(int)selectedCharacter];
                        currentBugButton.ButtonObject.GetComponent<BoxCollider2D>().size = currentBugButton.ButtonObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
                        break;
                    case SelectionType.Swatter:
                        swatterReady = false;
                        currentSwatterButton.ButtonObject.GetComponent<SpriteRenderer>().sprite = unselectedSprites[(int)selectedCharacter];
                        currentBugButton.ButtonObject.GetComponent<BoxCollider2D>().size = currentBugButton.ButtonObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
                        break;
                }
                break;
        }
    }
}
