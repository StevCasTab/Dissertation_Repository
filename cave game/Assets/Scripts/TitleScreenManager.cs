using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Languages;

public class TitleScreenManager : MonoBehaviour
{
	public GameObject SinglePlayerPanel;
	public LanguagesAvailable Language;
	public GameObject DataToHoldPrefab;
	public GameObject bg;
    public GameObject ConnectingScreen;
    public GameObject DisConnectingScreen;
    public GameObject BadgesScreen;
    public GameObject ClearBadgesScreen;

    public bool delprefs = false;
    private bool invalid = false;


    public void Update()
    {
        if (delprefs)
        {
            delprefs = false;
            PlayerPrefs.DeleteAll();
        }
    }
    public void OpenDelScreen()
    {
        ClearBadgesScreen.SetActive(true);
    }
    public void CloseDelScreen()
    {
        ClearBadgesScreen.SetActive(false);
    }

    public void ClearBadges()
    {
        if (File.Exists("MultiBadge.data"))
        {
            File.Delete("MultiBadge.data");
        }
        if (File.Exists("SingleBadge.data"))
        {
            File.Delete("SingleBadge.data");
        }
        if (File.Exists("TutorialBadge.data"))
        {
            File.Delete("TutorialBadge.data");
        }
        BadgesScreen.transform.Find("S5").GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        BadgesScreen.transform.Find("S5Text").GetComponent<TMP_Text>().color = Color.grey;
        BadgesScreen.transform.Find("S3").GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        BadgesScreen.transform.Find("S3Text").GetComponent<TMP_Text>().color = Color.grey;
        BadgesScreen.transform.Find("S2").GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        BadgesScreen.transform.Find("S2Text").GetComponent<TMP_Text>().color = Color.grey;
        BadgesScreen.transform.Find("T5").GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        BadgesScreen.transform.Find("T5Text").GetComponent<TMP_Text>().color = Color.grey;
        BadgesScreen.transform.Find("T3").GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        BadgesScreen.transform.Find("T3Text").GetComponent<TMP_Text>().color = Color.grey;
        BadgesScreen.transform.Find("T1").GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        BadgesScreen.transform.Find("T1Text").GetComponent<TMP_Text>().color = Color.grey;
        BadgesScreen.transform.Find("M5").GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        BadgesScreen.transform.Find("M5Text").GetComponent<TMP_Text>().color = Color.grey;
        BadgesScreen.transform.Find("M3").GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        BadgesScreen.transform.Find("M3Text").GetComponent<TMP_Text>().color = Color.grey;
        BadgesScreen.transform.Find("M2").GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        BadgesScreen.transform.Find("M2Text").GetComponent<TMP_Text>().color = Color.grey;
        GameObject.Find("Canvas").transform.Find("DeleteBadges").gameObject.SetActive(false);
        ClearBadgesScreen.SetActive(false);
    }
    public void Start()
    {
        if (File.Exists("SingleBadge.data"))
        {
            int t = 999;
            FileStream stream = new FileStream("SingleBadge.data", FileMode.Open);
            if (stream != null)
            {
                print("World Found");
            }
            try
            {
                BinaryFormatter fm = new BinaryFormatter();
                string f = fm.Deserialize(stream) as string;
                t = Convert.ToInt32(f);
                fm = null;
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                UnityEngine.Debug.Log("Failed to deserialize. Reason: " + e.Message);
                return;
            }
            finally
            {
                stream.Close();
            }

            if(t == 1)
            {
                BadgesScreen.transform.Find("S5").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("S5Text").GetComponent<TMP_Text>().color = Color.white;
            }
            else if(t == 2)
            {
                BadgesScreen.transform.Find("S5").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("S3").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("S5Text").GetComponent<TMP_Text>().color = Color.white;
                BadgesScreen.transform.Find("S3Text").GetComponent<TMP_Text>().color = Color.white;
            }
            else if(t == 3)
            {
                BadgesScreen.transform.Find("S5").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("S3").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("S2").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("S5Text").GetComponent<TMP_Text>().color = Color.white;
                BadgesScreen.transform.Find("S3Text").GetComponent<TMP_Text>().color = Color.white;
                BadgesScreen.transform.Find("S2Text").GetComponent<TMP_Text>().color = Color.white;
            }
        }
        if (File.Exists("TutorialBadge.data"))
        {
            int t2 = 999;
            FileStream stream = new FileStream("TutorialBadge.data", FileMode.Open);
            if (stream != null)
            {
                print("World Found");
            }
            try
            {
                BinaryFormatter fm = new BinaryFormatter();
                string f = fm.Deserialize(stream) as string;
                t2 = Convert.ToInt32(f);
                fm = null;
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                UnityEngine.Debug.Log("Failed to deserialize. Reason: " + e.Message);
                return;
            }
            finally
            {
                stream.Close();
            }

            if (t2 == 1)
            {
                BadgesScreen.transform.Find("T5").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("T5Text").GetComponent<TMP_Text>().color = Color.white;
            }
            else if (t2 == 2)
            {
                BadgesScreen.transform.Find("T5").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("T3").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("T5Text").GetComponent<TMP_Text>().color = Color.white;
                BadgesScreen.transform.Find("T3Text").GetComponent<TMP_Text>().color = Color.white;
            }
            else if (t2 == 3)
            {
                BadgesScreen.transform.Find("T5").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("T3").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("T1").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("T5Text").GetComponent<TMP_Text>().color = Color.white;
                BadgesScreen.transform.Find("T3Text").GetComponent<TMP_Text>().color = Color.white;
                BadgesScreen.transform.Find("T1Text").GetComponent<TMP_Text>().color = Color.white;
            }
        }
        if (File.Exists("MultiBadge.data"))
        {
            int t2 = 999;
            FileStream stream = new FileStream("MultiBadge.data", FileMode.Open);
            if (stream != null)
            {
                print("World Found");
            }
            try
            {
                BinaryFormatter fm = new BinaryFormatter();
                string f = fm.Deserialize(stream) as string;
                t2 = Convert.ToInt32(f);
                fm = null;
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                UnityEngine.Debug.Log("Failed to deserialize. Reason: " + e.Message);
                return;
            }
            finally
            {
                stream.Close();
            }

            if (t2 == 1)
            {
                BadgesScreen.transform.Find("M5").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("M5Text").GetComponent<TMP_Text>().color = Color.white;
            }
            else if (t2 == 2)
            {
                BadgesScreen.transform.Find("M5").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("M3").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("M5Text").GetComponent<TMP_Text>().color = Color.white;
                BadgesScreen.transform.Find("M3Text").GetComponent<TMP_Text>().color = Color.white;
            }
            else if (t2 == 3)
            {
                BadgesScreen.transform.Find("M5").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("M3").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("M2").GetComponent<UnityEngine.UI.Image>().color = Color.white;
                BadgesScreen.transform.Find("M5Text").GetComponent<TMP_Text>().color = Color.white;
                BadgesScreen.transform.Find("M3Text").GetComponent<TMP_Text>().color = Color.white;
                BadgesScreen.transform.Find("M2Text").GetComponent<TMP_Text>().color = Color.white;
            }
        }

        if(!File.Exists("MultiBadge.data") && !File.Exists("SingleBadge.data") && !File.Exists("TutorialBadge.data"))
        {
            GameObject.Find("Canvas").transform.Find("DeleteBadges").gameObject.SetActive(false);
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("DeleteBadges").gameObject.SetActive(true);
        }
    }
    public void VisitURL(string url)
	{
		Application.OpenURL(url);
	}

	public void LoadSingle()
	{
        SinglePlayerPanel.SetActive(true);
        transform.Find("SingleNameInput").GetChild(0).Find("StartButton").gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        transform.Find("SingleNameInput").GetChild(0).Find("StartButton").gameObject.GetComponent<Button>().onClick.AddListener(StartSingle);
    }

	public void LoadTutorial()
	{
        SinglePlayerPanel.SetActive(true);
		transform.Find("SingleNameInput").GetChild(0).Find("StartButton").gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        transform.Find("SingleNameInput").GetChild(0).Find("StartButton").gameObject.GetComponent<Button>().onClick.AddListener(StartTutorial);
    }
    public void OpenBadgesScreen()
    {
        BadgesScreen.SetActive(true);
    }
    public void CloseBadgesScreen()
    {
        BadgesScreen.SetActive(false);
    }

	void StartSingle()
	{
        if (!invalid)
        {
            invalid = true;
            string text1 = SinglePlayerPanel.transform.GetChild(0).Find("NameInput").gameObject.GetComponent<TMP_InputField>().text;

            if (text1 != "" && text1 != " " && Language != LanguagesAvailable.None)
            {
                StartCoroutine(StartSingleRoutine());
            }
            else if (text1 == "" || text1 == " ")
            {
                StartCoroutine(emptyNameEntered());
            }
        }
    }

	public void QuitGame()
	{
		Application.Quit();
	}

	public void StartTutorial()
	{
		if (!invalid)
		{
			invalid = true;
			string text1 = SinglePlayerPanel.transform.GetChild(0).Find("NameInput").gameObject.GetComponent<TMP_InputField>().text;

			if (text1 != "" && text1 != " " && Language != LanguagesAvailable.None)
			{
				StartCoroutine(StartTutorialRoutine());
			}
            else if (text1 == "" || text1 == " ")
            {
                StartCoroutine(emptyNameEntered());
            }
        }
	}

	IEnumerator StartTutorialRoutine()
	{
        bg.SetActive(true);
		yield return new WaitForSeconds(0.1f);
        GameObject TutorialQuestionHolder = GameObject.Find("TitleQuestionHolder");
        GameObject DToHold = Instantiate(DataToHoldPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        DToHold.name = "GameManager";
        DToHold.GetComponent<DataToHold>().PlayerName = SinglePlayerPanel.transform.GetChild(0).Find("NameInput").gameObject.GetComponent<TMP_InputField>().text;
        if (Language == LanguagesAvailable.CSharp)
        {
            DToHold.GetComponent<DataToHold>().SelectedLanguage = global::Language.CSharp;
            foreach (GameObject t in TutorialQuestionHolder.GetComponent<TutorialQuestionHolder>().CSharpQuestions)
            {
                DToHold.GetComponent<DataToHold>().QuizQuestions.Add(t);
            }
        }
        else if (Language == LanguagesAvailable.Python)
        {
            DToHold.GetComponent<DataToHold>().SelectedLanguage = global::Language.Python;
            foreach (GameObject t in TutorialQuestionHolder.GetComponent<TutorialQuestionHolder>().PythonQuestions)
            {
                DToHold.GetComponent<DataToHold>().QuizQuestions.Add(t);
            }

        }
        CloseSingleName();
        DontDestroyOnLoad(DToHold);
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(1);
    }
    IEnumerator StartSingleRoutine()
    {
        bg.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameObject TutorialQuestionHolder = GameObject.Find("TitleQuestionHolder");
        GameObject DToHold = Instantiate(DataToHoldPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        DToHold.name = "GameManager";
        DToHold.GetComponent<DataToHold>().PlayerName = SinglePlayerPanel.transform.GetChild(0).Find("NameInput").gameObject.GetComponent<TMP_InputField>().text;
        if (Language == LanguagesAvailable.CSharp)
        {
            DToHold.GetComponent<DataToHold>().SelectedLanguage = global::Language.CSharp;
            foreach (GameObject t in TutorialQuestionHolder.GetComponent<TutorialQuestionHolder>().CSharpQuestions)
            {
                DToHold.GetComponent<DataToHold>().QuizQuestions.Add(t);
            }
        }
        else if (Language == LanguagesAvailable.Python)
        {
            DToHold.GetComponent<DataToHold>().SelectedLanguage = global::Language.Python;
            foreach (GameObject t in TutorialQuestionHolder.GetComponent<TutorialQuestionHolder>().PythonQuestions)
            {
                DToHold.GetComponent<DataToHold>().QuizQuestions.Add(t);
            }

        }
        CloseSingleName();
        DontDestroyOnLoad(DToHold);
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(2);
    }

    public void StartGame()
	{
		if (!invalid)
		{
			invalid = true;
			string text1 = SinglePlayerPanel.transform.GetChild(0).Find("NameInput").gameObject.GetComponent<TMP_InputField>().text;

			if (text1 != "" && text1 != " " && Language != LanguagesAvailable.None)
			{
				StartCoroutine(StartGameRoutine());
			}
			else if (text1 == "" || text1 == " ")
			{
				StartCoroutine(emptyNameEntered());
			}
		}
    }

    IEnumerator StartGameRoutine()
    {
        bg.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameObject DToHold = Instantiate(DataToHoldPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        DToHold.name = "GameManager";
        GameObject TutorialQuestionHolder = GameObject.Find("TutorialQuestionHolder");
        if (Language == LanguagesAvailable.CSharp)
        {
            DToHold.GetComponent<DataToHold>().SelectedLanguage = global::Language.CSharp;

            foreach (GameObject t in TutorialQuestionHolder.GetComponent<TutorialQuestionHolder>().CSharpQuestions)
            {
                DToHold.GetComponent<DataToHold>().QuizQuestions.Add(t);
            }
        }
        else if (Language == LanguagesAvailable.Python)
        {
            DToHold.GetComponent<DataToHold>().SelectedLanguage = global::Language.Python;

            foreach (GameObject t in TutorialQuestionHolder.GetComponent<TutorialQuestionHolder>().PythonQuestions)
            {
                DToHold.GetComponent<DataToHold>().QuizQuestions.Add(t);
            }
        }
        CloseSingleName();
        DontDestroyOnLoad(DToHold);
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(2);
    }


    public void OpenSingleName()
	{
		SinglePlayerPanel.SetActive(true);
        transform.Find("SingleNameInput").GetChild(0).Find("StartButton").gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        transform.Find("SingleNameInput").GetChild(0).Find("StartButton").gameObject.GetComponent<Button>().onClick.AddListener(StartGame);
    }
	public void CloseSingleName()
	{
		SinglePlayerPanel.SetActive(false);
		Language = LanguagesAvailable.None;
        SinglePlayerPanel.transform.GetChild(0).Find("NameInput").gameObject.GetComponent<TMP_InputField>().text = "";
        transform.Find("SingleNameInput").GetChild(0).Find("PythonButton").GetComponent<Image>().color = Color.white;
		transform.Find("SingleNameInput").GetChild(0).Find("PythonButton").GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 115f);
		transform.Find("SingleNameInput").GetChild(0).Find("C#Button").GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 115f);
		transform.Find("SingleNameInput").GetChild(0).Find("C#Button").GetComponent<Image>().color = Color.white;
	}

    public void SelectLanguage(int i)
    {
        if (GameObject.Find("Canvas").transform.Find("RoomScreen").gameObject.activeSelf == false)
        {
            if (i == 0)
            {
                transform.Find("SingleNameInput").GetChild(0).Find("C#Button").GetComponent<Image>().color = Color.white;
                transform.Find("SingleNameInput").GetChild(0).Find("PythonButton").GetComponent<Image>().color = Color.grey;
                Language = LanguagesAvailable.CSharp;
                transform.Find("SingleNameInput").GetChild(0).Find("C#Button").GetComponent<RectTransform>().sizeDelta = new Vector2(150f, 125f);
                transform.Find("SingleNameInput").GetChild(0).Find("PythonButton").GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 115f);
            }
            else if (i == 1)
            {
                transform.Find("SingleNameInput").GetChild(0).Find("PythonButton").GetComponent<Image>().color = Color.white;
                transform.Find("SingleNameInput").GetChild(0).Find("C#Button").GetComponent<Image>().color = Color.grey;
                Language = LanguagesAvailable.Python;
                transform.Find("SingleNameInput").GetChild(0).Find("C#Button").GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 115f);
                transform.Find("SingleNameInput").GetChild(0).Find("PythonButton").GetComponent<RectTransform>().sizeDelta = new Vector2(150f, 125f);
            }
        }
        else
        {
            if(i == 0)
            {
                GameObject.Find("Canvas").transform.Find("RoomScreen").GetChild(0).Find("PythonButton").GetComponent<Image>().color = Color.grey;
                GameObject.Find("Canvas").transform.Find("RoomScreen").GetChild(0).Find("C#Button").GetComponent<Image>().color = Color.white;
                Language = Languages.LanguagesAvailable.CSharp;
                GameObject.Find("Canvas").transform.Find("RoomScreen").GetChild(0).Find("PythonButton").GetComponent<RectTransform>().sizeDelta = new Vector2(160f, 160f);
                GameObject.Find("Canvas").transform.Find("RoomScreen").GetChild(0).Find("C#Button").GetComponent<RectTransform>().sizeDelta = new Vector2(193f, 192f);
            }
            else if(i == 1)
            {
                GameObject.Find("Canvas").transform.Find("RoomScreen").GetChild(0).Find("C#Button").GetComponent<Image>().color = Color.grey;
                GameObject.Find("Canvas").transform.Find("RoomScreen").GetChild(0).Find("PythonButton").GetComponent<Image>().color = Color.white;
                Language = Languages.LanguagesAvailable.Python;
                GameObject.Find("Canvas").transform.Find("RoomScreen").GetChild(0).Find("C#Button").GetComponent<RectTransform>().sizeDelta = new Vector2(160f, 160f);
                GameObject.Find("Canvas").transform.Find("RoomScreen").GetChild(0).Find("PythonButton").GetComponent<RectTransform>().sizeDelta = new Vector2(193f, 192f);
            }
        }
	}

	/*void loadscene()
	{
		System.Random rnd = new System.Random();
		int random = rnd.Next(0, 50);

		if (random >= 0 && random <= 16)
		{
			GameObject.Find("GameManager").GetComponent<DataToHold>().GameLevel = 0;
			SceneManager.LoadScene(2);
		}
		else if (random > 16 && random <= 33)
		{
			GameObject.Find("GameManager").GetComponent<DataToHold>().GameLevel = 1;
			SceneManager.LoadScene(3);
		}
		else if (random > 33 && random <= 50)
		{
			GameObject.Find("GameManager").GetComponent<DataToHold>().GameLevel = 2;
			SceneManager.LoadScene(4);
		}
	}*/

	IEnumerator emptyNameEntered()
	{
		SinglePlayerPanel.transform.GetChild(0).Find("NameInput").GetComponent<Image>().color = Color.red;
		yield return new WaitForSeconds(1f);
		SinglePlayerPanel.transform.GetChild(0).Find("NameInput").GetComponent<Image>().color = Color.white;
		//emptynameentered = false;
		invalid = false;
    }

    public void OpenMultiplayerPanel()
    {
        ConnectingScreen.SetActive(true);
        GameObject.Find("Lobby").GetComponent<Lobby>().ConnectToServer();
    }
    public void CloseMultiplayerPanel()
    {
        GameObject.Find("Canvas").transform.Find("MultiplayerScreen").gameObject.SetActive(false);
        DisConnectingScreen.SetActive(false);
        GameObject.Find("Canvas").transform.Find("MultiplayerScreen").Find("PlayerNameInput").GetComponent<TMP_InputField>().text = "";
    }
}
