﻿using UnityEngine;
using System.Collections;

public enum WinnerType {
    Human,
    Bug
}

public class GameManager : MonoBehaviour {
    public float preMenuTime = 3f;
    public GameObject preMenuText;
    public Vector3 mainMenuCamera;
    public float mainMenuZoom;
	public Vector3 modeSelectCamera;
	public float modeSelectZoom;
	public GameObject modeSelectText;
	public GameObject[] modeNames;
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
    float endTime;
    bool bugReady = false;
    bool swatterReady = false;
    int swatter = 0;
    int character = 0;
	int mode = 0;
    GameObject bug;
    GameObject hand;
    bool swatterScrolling = false;
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
    StateType state = StateType.PreMenu;
    void Start() {
        Cursor.visible = false;
        foreach (Transform l in GameObject.Find("levels").transform) {
            l.gameObject.SetActive(false);
        }
        GameObject.Find("levels").transform.Find("level" + (int)(Random.Range(1f, 4f))).gameObject.SetActive(true);
        StartCoroutine(PreMenuTransistion());
    }
    void Update() {
        switch (state) {
			case StateType.SelectingMode:
				if(Mathf.Abs(Input.GetAxisRaw("Horizontal (Menu)")) > 0){
					modeNames[mode].GetComponent<TextMesh>().color = Color.white;
					mode += (int)Input.GetAxisRaw("Horizontal (Menu)");
					if(mode > 1){
						mode = 1;
					}
					if(mode < 0){
						mode = 0;
					}
					modeNames[mode].GetComponent<TextMesh>().color = Color.blue;
				}
			break;
            case StateType.SelectingCharacter:
                characterProfiles[character].SetActive(false);
                if (Input.GetAxisRaw("Vertical (Bug Menu)") > 0 && !bugReady) {
                    character++;
                }
                if (Input.GetAxisRaw("Vertical (Bug Menu)") < 0 && !bugReady) {
                    character--;
                }
                if (character >= characters.Length)
                    character = characters.Length-1;
                if (character < 0)
                    character = 0;
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
                    swatter = swatters.Length - 1;
                if (swatter < 0)
                    swatter = 0;
                swatterProfiles[swatter].SetActive(true);
                if (Input.GetButtonDown("Submit (Bug)")) {
                    characterProfiles[character].transform.Find("egg").GetComponent<SpriteRenderer>().sprite = finalCharacterProfiles[character];
                    bugReady = true;
                }
                if (Input.GetButtonDown("Submit (Swatter)")) {
                    swatterProfiles[swatter].transform.Find("box").GetComponent<SpriteRenderer>().sprite = finalSwatterProfiles[swatter];
                    swatterReady = true;
                }
                if (swatterReady && bugReady) {
                    state = StateType.InGame;
                    characterSelectText.SetActive(false);
                    //stageSelectText.SetActive(true);
                    StartGame();
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
                    bugScore = 0;
                    gameOverText.SetActive(false);
                    Destroy(bug);
                    Destroy(hand);
                    Destroy(GameObject.FindGameObjectWithTag("web"));
                    foreach (GameObject dead in GameObject.FindGameObjectsWithTag("dead")) {
                        Destroy(dead);
                    }
                    foreach (GameObject berry in GameObject.FindGameObjectsWithTag("berry")) {
                        if(berry.transform.parent != null && berry.transform.parent.tag != "berry tree")
                            Destroy(berry);
                        if(berry.transform.parent == null)
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
                    StartGame();
                }
                if (Input.GetButtonDown("Cancel")) {
                    Application.LoadLevel(0);
                }
                break;
        }
        if (Input.GetButtonDown("Submit")) {
            switch (state) {
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
                case StateType.SelectingCharacter:
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
    void StartGame(){
		GetComponent<AudioSource>().clip = inGameMusic;
		GetComponent<AudioSource>().time = inGameMusicTime;
		GetComponent<AudioSource>().Play();
		state = StateType.InGame;
		bug = Instantiate(characters[character]) as GameObject;
		hand = Instantiate(swatters[swatter]) as GameObject;
		LeanTween.move(Camera.main.gameObject, inGameCamera, 0.5f);
		LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, inGameCameraZoom, 0.5f);
		switch(mode){
			case 0: //classic gnat-eating
				gnatSpawner.SetActive(true);
			break;
			case 1:
                berryMode.SetActive(true);
			break;
		}
	}
    public void EndGame(WinnerType winner) {
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
}
