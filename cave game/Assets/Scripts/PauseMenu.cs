using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{   
	public GameObject menuObj;
	public GameObject mainObj;
	public GameObject optionsObj;
	public GameObject newLvlObj;

	public AudioSource _music;

	public Text[] musicTexts;
	public Text[] graphicsTexts;
	public Text[] invMouseTexts;

	int mode = 0;

	[HideInInspector]
	public bool paused = false;

	public bool music = true;
	public bool invertMouse = false;

	public static PauseMenu pauseMenu;

	void Awake()
	{
		pauseMenu = this;

		if (!music) _music.Stop();
	}

	void Update()
	{
		if (!World.currentWorld.worldInitialized) return;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			paused = !paused;
			mode = 0;

			if (paused)
			{
				menuObj.SetActive(true);
				mainObj.SetActive(true);
			}
			else
			{
				menuObj.SetActive(false);
				mainObj.SetActive(false);
				optionsObj.SetActive(false);
				newLvlObj.SetActive(false);
			}
		}

		if(mode == 1) SetOptionsText();
	}

	void SetTexts(Text[] texts, string msg)
	{
		foreach(Text t in texts)
		{
			t.text = msg;
		}
	}

	public void UnPauseGame()
	{
		paused = false;
		mode = 0;

		menuObj.SetActive(false);
		mainObj.SetActive(false);
		optionsObj.SetActive(false);
	}

	public void ChangeGraphicsMode()
	{
		World.currentWorld.ChangeGraphicsMode();
	}

	public void ToggleMusic()
	{
		music = !music;

		if (!music && _music.isPlaying) _music.Pause();

		if (music) _music.Play();
	}

	public void ToggleInvertMouse()
	{
		invertMouse = !invertMouse;
	}

	public void ViewOptions()
	{
		mode = 1;
		optionsObj.SetActive(true);
		mainObj.SetActive(false);
	}

	public void CloseOptions()
	{
		mode = 0;
		optionsObj.SetActive(false);
		mainObj.SetActive(true);
	}

	public void QuitToTitle()
	{
		if (GameObject.Find("MultiplayerManager") == null)
		{
			StartCoroutine(Quitzies());
		}
		else
		{
			PhotonNetwork.LeaveRoom();
			if (GameObject.Find("GameManager") != null)
			{
				Destroy(GameObject.Find("GameManager"));
			}
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			SceneManager.LoadScene(0);
		}
	}
	
	IEnumerator Quitzies()
	{
        string worldn = GameObject.Find("World").GetComponent<World>().WorldName;
		if (worldn == "Singleplayer")
		{
			GameObject.Find("GameManager").GetComponent<DataToHold>().SaveInfo(false);
		}
		else if (worldn == "Tutorial"){
			GameObject[] t = GameObject.FindGameObjectsWithTag("B.A.T");
			if (t[0].GetComponent<BlockAccessTable>().CompletedTutorial == false)
			{
				GameObject.Find("GameManager").GetComponent<DataToHold>().SaveUnFinTutorial();
			}
		}
		yield return new WaitForSeconds(2f);
		QuitziesNow();
	}
	void QuitziesNow()
	{
		Destroy(GameObject.Find("GameManager").gameObject);
		if (GameObject.Find("World").GetComponent<World>().WorldName == "Tutorial")
		{
			Application.wantsToQuit += GameObject.Find("GameManager").GetComponent<DataToHold>().StopQuit;
		}
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
	}

	void SetOptionsText()
	{
		if(World.currentWorld.gMode == GraphicsMode.Fast) SetTexts(graphicsTexts, "Graphics: Fast");
		else if (World.currentWorld.gMode == GraphicsMode.Fancy) SetTexts(graphicsTexts, "Graphics: Fancy");
		else if(World.currentWorld.gMode == GraphicsMode.Insane) SetTexts(graphicsTexts, "Graphics: Insane");

		if (music) SetTexts(musicTexts, "Music: ON");
		else SetTexts(musicTexts, "Music: OFF");

		if (invertMouse) SetTexts(invMouseTexts, "Invert Mouse Y: ON");
		else SetTexts(invMouseTexts, "Invert Mouse Y: OFF");
	}
}