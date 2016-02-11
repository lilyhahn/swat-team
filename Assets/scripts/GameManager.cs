using UnityEngine;
using System.Collections;

public enum WinnerType {
    Human,
    Bug
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
    public float restartDelay = 0.5f;
    public bool showControls = true;
    public int controlPromptTime = 604800;
    float endTime;
    bool bugReady = false;
    bool swatterReady = false;
    int swatter = 0;
    int character = 0;
	int mode = 0;
    GameObject hand;
    bool swatterScrolling = false;
    bool bugScrolling = false;
    bool selectingCharacterActive = false;
    public bool paused { get; private set; }
    enum StateType {
        PreMenu,
        MainMenu,
		SelectingMode,
        SelectingCharacter,
        SelectingStage,
        InGame,
        GameOver,
        Credits
    }
    StateType state = StateType.MainMenu;
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
        paused = false;
        Cursor.visible = false;
        foreach (Transform l in GameObject.Find("levels").transform) {
            l.gameObject.SetActive(false);
        }
        GameObject.Find("levels").transform.Find("level" + (int)(Random.Range(1f, 4f))).gameObject.SetActive(true);
        //StartCoroutine(PreMenuTransistion());
    }
    void Update() {
        switch (state) {
			case StateType.SelectingMode:
				if(Mathf.Abs(Input.GetAxisRaw("Vertical (Menu)")) > 0){
					int oldMode = mode;
					mode += (int)Input.GetAxisRaw("Vertical (Menu)");
					if(mode > 1){
						mode = 1;
					}
					if(mode < 0){
						mode = 0;
					}
					if(mode == 0 && mode != oldMode){
						modeSelectText.GetComponent<Animator>().SetTrigger("classic");
					}
					if(mode == 1 && mode != oldMode){
						modeSelectText.GetComponent<Animator>().SetTrigger("new");
					}
				}
			break;
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
        if (Input.GetButtonDown("Submit")) {
            switch (state) {
            	case StateType.PreMenu:
					Camera.main.cullingMask = LayerMask.NameToLayer("Everything");
					preMenuText.SetActive(false);
					state = StateType.MainMenu;
					break;
                case StateType.MainMenu:
					state = StateType.SelectingMode;
					LeanTween.move(Camera.main.gameObject, modeSelectCamera, 0.5f);
					LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, modeSelectZoom, 0.5f);
					modeSelectText.SetActive(true);
                  	/*state = StateType.SelectingCharacter;
                    LeanTween.move(Camera.main.gameObject, characterSelectCamera, 0.5f);
                    LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, characterSelectCameraZoom, 0.5f);
                    characterSelectText.SetActive(true);*/
                    break;
                 case StateType.SelectingMode:
					state = StateType.SelectingCharacter;
					LeanTween.move(Camera.main.gameObject, characterSelectCamera, 0.5f);
					LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, characterSelectCameraZoom, 0.5f);
					switch(mode){
						case 0:
							voiceAudio.PlayOneShot(gnatVoices[Random.Range(0, gnatVoices.Length - 1)]);
							break;
						case 1:
							voiceAudio.PlayOneShot(berryVoices[Random.Range(0, gnatVoices.Length - 1)]);
							break;
					}
					characterSelectText.SetActive(true);
				break;
                /*case StateType.SelectingCharacter:
                    state = StateType.SelectingStage;
                    characterSelectText.SetActive(false);
                    stageSelectText.SetActive(true);
                    LeanTween.move(Camera.main.gameObject, stageSelectCamera, 0.5f);
                    LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, stageSelectCameraZoom, 0.5f);
                    break;*/
                case StateType.SelectingStage:
                    stageSelectText.SetActive(false);
                    state = StateType.InGame;
                    characters[character].SetActive(true);
                    swatters[swatter].SetActive(true);
                    LeanTween.move(Camera.main.gameObject, inGameCamera, 0.5f);
                    LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, inGameCameraZoom, 0.5f);
                    break;
            }
        }
		if(Input.GetButtonDown ("Cancel")){
			switch(state){
                case StateType.InGame:
                    if (!paused) {
                        Time.timeScale = 0;
                        paused = true;
                    }
                    else {
                        Time.timeScale = 1;
                        paused = false;
                    }
                    break;
                case StateType.SelectingCharacter:
                    if (selectingCharacterActive) {
                        selectingCharacterActive = false;
                        break;
                    }
                    berryMode.SetActive(false);
                    LeanTween.move(Camera.main.gameObject, modeSelectCamera, 0.5f);
					LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, modeSelectZoom, 0.5f);
                    state = StateType.SelectingMode;
                    break;
                case StateType.SelectingMode:
                    LeanTween.move(Camera.main.gameObject, mainMenuCamera, 0.5f);
					LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, mainMenuZoom, 0.5f);
                    state = StateType.MainMenu;
                    break;
			    case StateType.PreMenu:
				    Application.Quit();
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
		GetComponent<AudioSource>().Stop ();
		inGame = true;
		state = StateType.InGame;
		LeanTween.move(Camera.main.gameObject, inGameCamera, 0.5f);
		LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, inGameCameraZoom, 0.5f);
		if(mode == 1){
			berryMode.SetActive(true);
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
		Instantiate(characters[character], Random.insideUnitCircle * randomSpawnRadius, Quaternion.identity);
		hand = Instantiate(swatters[swatter]) as GameObject;
		if(mode == 0){
			gnatSpawner.SetActive(true);
		}
	}
    public void EndGame(WinnerType winner) {
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
    	if(bugScore == winningScores[mode]){
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
}
