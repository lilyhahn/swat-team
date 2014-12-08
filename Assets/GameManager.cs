using UnityEngine;
using System.Collections;

public enum WinnerType{
	Human,
	Bug
}

public class GameManager : MonoBehaviour {
	public Vector3 characterSelectCamera;
	public float characterSelectCameraZoom;
	public GameObject characterSelectText;
	public Vector3 stageSelectCamera;
	public float stageSelectCameraZoom;
	public GameObject stageSelectText;
	public Vector3 inGameCamera;
	public float inGameCameraZoom;
	public GameObject[] characters;
	public GameObject[] characterProfiles;
	public Sprite[] finalCharacterProfiles;
	public GameObject[] swatters;
	public GameObject[] swatterProfiles;
	public Sprite[] finalSwatterProfiles;
	public AudioClip humanWinJingle;
	public AudioClip bugWinJingle;
	bool bugReady = false;
	bool swatterReady = false;
	int swatter = 0;
	int character = 0;
	Bug bug;
	Hand hand;
	enum StateType{
		MainMenu,
		SelectingCharacter,
		SelectingStage,
		InGame,
		GameOver,
		Credits
	}
	StateType state = StateType.MainMenu;
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
					characterProfiles[character].GetComponent<SpriteRenderer>().sprite = finalCharacterProfiles[character];
					bugReady = true;
				}
				if(Input.GetButtonDown("Fire1")){
					swatterProfiles[swatter].GetComponent<SpriteRenderer>().sprite = finalSwatterProfiles[swatter];
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
				if(Input.anyKey && !Input.GetKey(KeyCode.Escape)){
					Destroy(bug);
					Destroy (hand);
					StartGame();
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
		state = StateType.InGame;
		bug = Instantiate(characters[character]) as Bug;
		hand = Instantiate(swatters[swatter]) as Hand;
		LeanTween.move(Camera.main.gameObject, inGameCamera, 0.5f);
		LeanTween.value(gameObject, UpdateZoom, Camera.main.orthographicSize, inGameCameraZoom, 0.5f);
	}
	public void EndGame(WinnerType winner){
		state = StateType.GameOver;
		if(winner == WinnerType.Human){
			audio.clip = humanWinJingle;
		}
		else if(winner == WinnerType.Bug){
			audio.clip = bugWinJingle;
		}
		audio.Play ();
	}
}
