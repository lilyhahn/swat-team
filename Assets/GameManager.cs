using UnityEngine;
using System.Collections;

public enum WinnerType{
	Human,
	Bug
}

public class GameManager : MonoBehaviour {
	public float preMenuTime = 3f;
	public GameObject preMenuText;
	public Vector3 characterSelectCamera;
	public float characterSelectCameraZoom;
	public GameObject characterSelectText;
	public Vector3 stageSelectCamera;
	public float stageSelectCameraZoom;
	public GameObject stageSelectText;
	public Vector3 inGameCamera;
	public float inGameCameraZoom;
	public AudioClip inGameMusic;
	public GameObject gameOverText;
	public GameObject[] characters;
	public GameObject[] characterProfiles;
	public Sprite[] finalCharacterProfiles;
	public GameObject[] swatters;
	public GameObject[] swatterProfiles;
	public Sprite[] finalSwatterProfiles;
	public AudioClip humanWinJingle;
	public AudioClip bugWinJingle;
	public GameObject gnatSpawner;
	bool bugReady = false;
	bool swatterReady = false;
	int swatter = 0;
	int character = 0;
	GameObject bug;
	GameObject hand;
	enum StateType{
		PreMenu,
		MainMenu,
		SelectingCharacter,
		SelectingStage,
		InGame,
		GameOver,
		Credits
	}
	StateType state = StateType.PreMenu;
	void Start(){
		StartCoroutine(PreMenuTransistion());
	}
	void Update(){
		switch(state){
			case StateType.SelectingCharacter:
				characterProfiles[character].SetActive(false);
				if(Input.GetKeyDown (KeyCode.W) && !bugReady){
					character++;
				}
				if(Input.GetKeyDown (KeyCode.S) && !bugReady){
					character--;
				}
				if(character >= characters.Length)
					character = 0;
				if(character < 0)
					character = characters.Length - 1;
				characterProfiles[character].SetActive(true);
				
				swatterProfiles[swatter].SetActive(false);
				if(Input.GetKeyDown (KeyCode.UpArrow) && !swatterReady){
					swatter++;
				}
				if(Input.GetKeyDown(KeyCode.DownArrow) && !swatterReady){
					swatter--;
				}
				if(swatter >= swatters.Length)
					swatter = 0;
				if(swatter < 0)
					swatter = swatters.Length - 1;
				swatterProfiles[swatter].SetActive(true);
				if(Input.GetKeyDown(KeyCode.Space)){
					characterProfiles[character].transform.Find("egg").GetComponent<SpriteRenderer>().sprite = finalCharacterProfiles[character];
					bugReady = true;
				}
				if(Input.GetButtonDown("Fire1")){
					swatterProfiles[swatter].transform.Find ("box").GetComponent<SpriteRenderer>().sprite = finalSwatterProfiles[swatter];
					swatterReady = true;
				}
				if(swatterReady && bugReady){
					state = StateType.InGame;
					characterSelectText.SetActive(false);
					//stageSelectText.SetActive(true);
					StartGame();
				}
			break;
			case StateType.GameOver:
				if(Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape)){
					gameOverText.SetActive(false);
					Destroy (bug);
					Destroy (hand);
					gameOverText.transform.Find("human").gameObject.SetActive(false);
					gameOverText.transform.Find("bug").gameObject.SetActive(false);
					StartGame();
				}
				if(Input.GetKeyDown (KeyCode.Escape)){
					Application.LoadLevel(0);
				}
			break;
		}
		if(Input.GetButtonDown("Submit")){
			switch(state){
			case StateType.MainMenu:
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
	}
	void UpdateZoom(float val){
		Camera.main.orthographicSize = val;
	}
	void StartGame(){
		audio.clip = inGameMusic;
		audio.Play();
		state = StateType.InGame;
		gnatSpawner.SetActive(true);
		bug = Instantiate(characters[character]) as GameObject;
		hand = Instantiate(swatters[swatter]) as GameObject;
		LeanTween.move(Camera.main.gameObject, inGameCamera, 0.5f);
		LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, inGameCameraZoom, 0.5f);
	}
	public void EndGame(WinnerType winner){
		gameOverText.SetActive(true);
		state = StateType.GameOver;
		if(winner == WinnerType.Human){
			gameOverText.transform.Find("human").gameObject.SetActive(true);
			audio.clip = humanWinJingle;
		}
		else if(winner == WinnerType.Bug){
			gameOverText.transform.Find("bug").gameObject.SetActive(true);
			audio.clip = bugWinJingle;
		}
		audio.Play ();
	}
	IEnumerator PreMenuTransistion(){
		yield return new WaitForSeconds(preMenuTime);
		Camera.main.cullingMask = LayerMask.NameToLayer("Everything");
		preMenuText.SetActive(false);
		state = StateType.MainMenu;
	}
}
