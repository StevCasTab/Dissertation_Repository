using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SinglePlayerManager : MonoBehaviour
{
    public bool timerbegin = false;

    public bool Objective1Completed = false;
    public bool Objective2Completed = false;
    public bool Objective3Completed = false;
    public bool Objective4Completed = false;

    public GameObject GameTimerText;

    public List<float> ObjectiveTimers = new List<float>();
    public List<string> ObjectiveNames = new List<string>();
    public List<int> IncorrectAttempts = new List<int>();

    public List<string> SkippedObjectiveNames = new List<string>();
    public List<float> SkippedObjectiveTimers = new List<float>();
    public List<int> IncorrectAttemptsForSkipped = new List<int>();
    public float GameTimer = 0;

    private bool EndGame = false;

    public GameObject Objective2Check;
    public GameObject Objective3Check;
    public GameObject Objective4Check;

    public int BadgeNumber = 9999;

    public List<GameObject> BlockAccessTables = new List<GameObject>();

    public void Start()
    {

        if (File.Exists("SingleBadge.data"))
        {
            FileStream stream = new FileStream("SingleBadge.data", FileMode.Open);
            if (stream != null)
            {
                print("World Found");
            }
            try
            {
                BinaryFormatter fm = new BinaryFormatter();
                string f = fm.Deserialize(stream) as string;
                BadgeNumber = Convert.ToInt32(f);
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
        }
    }
    public void Update()
    {
        if (timerbegin)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                float m = Mathf.Round(timer);
                GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().text = m.ToString();
                GameObject.Find("Canvas").transform.Find("TimerText").GetChild(0).GetComponent<Text>().text = m.ToString();
            }
            else
            {
                timerbegin = false;
                GameObject.Find("player").GetComponent<PlayerController>().controlsEnabled = true;
                StartCoroutine(BeginGame());
            }
        }

        if(!timerbegin && timer <= 0 && !EndGame)
        {
            GameTimer += Time.deltaTime;
        }
    }

    private float timer = 3f;
    public void PreSingle()
    {
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("TitleText").GetComponent<Text>().text = "Singleplayer";
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("ContentText").GetComponent<Text>().text = "When you click the 'OK' button below, a timer will start. When the timer hits 0, the game begins.\n\n How long it takes you to complete the mode, how long it takes to complete code objectives and how many incorrect attempts you make will be saved.\n\nHappy Coding!";
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(BeginSingle);
    }

    public void BeginSingle()
    {
        GameObject.Find("Canvas").transform.Find("CodeImage").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("TimerText").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("TimerImage").gameObject.SetActive(true);
        timerbegin = true;
    }

    IEnumerator BeginGame()
    {
        GameTimerText.SetActive(true);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("TimerImage").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().text = "GO!";
        GameObject.Find("Canvas").transform.Find("TimerText").GetChild(0).GetComponent<Text>().text = "GO!";
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().fontSize = 53;
        GameObject.Find("Canvas").transform.Find("TimerText").GetChild(0).GetComponent<Text>().fontSize = 50;
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().fontSize = 51;
        GameObject.Find("Canvas").transform.Find("TimerText").GetChild(0).GetComponent<Text>().fontSize = 48;
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().color = new Color32(231, 255, 0, 150);
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().fontSize = 48;
        GameObject.Find("Canvas").transform.Find("TimerText").GetChild(0).GetComponent<Text>().fontSize = 45;
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().color = new Color32(231, 255, 0, 125);
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().fontSize = 45;
        GameObject.Find("Canvas").transform.Find("TimerText").GetChild(0).GetComponent<Text>().fontSize = 42;
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().color = new Color32(231, 255, 0, 100);
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().fontSize = 43;
        GameObject.Find("Canvas").transform.Find("TimerText").GetChild(0).GetComponent<Text>().fontSize = 40;
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().color = new Color32(231, 255, 0, 75);
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().fontSize = 41;
        GameObject.Find("Canvas").transform.Find("TimerText").GetChild(0).GetComponent<Text>().fontSize = 38;
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().color = new Color32(231, 255, 0, 50);
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().fontSize = 38;
        GameObject.Find("Canvas").transform.Find("TimerText").GetChild(0).GetComponent<Text>().fontSize = 35;
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().color = new Color32(231, 255, 0, 25);
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().fontSize = 35;
        GameObject.Find("Canvas").transform.Find("TimerText").GetChild(0).GetComponent<Text>().fontSize = 32;
        GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>().color = new Color32(231, 255, 0, 0);
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("Canvas").transform.Find("TimerText").gameObject.SetActive(false);
    }

    public void GoToObjective2()
    {
        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[0].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(false);
        Objective1Completed = true;
        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[1].SetActive(true);
        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[1].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(true);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").gameObject.GetComponent<RectTransform>().localPosition = new Vector3(-368.89f, 315f, 0);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(737.7f, 200f);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(0, -2.5f, 0);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(737.79f, 194.948f);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").GetChild(0).GetComponent<TMP_Text>().text = "<s>Objective 1: Cross the Ravine!</s>\nObjective 2: Build up to the tower!";
        GameObject t = Instantiate(Objective2Check, new Vector3(158.57f, 48.69f, 87f), Quaternion.identity);
    }

    public void GoToObjective3()
    {
        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[1].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(false);
        Objective2Completed = true;
        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[2].SetActive(true);
        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[2].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(true);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").gameObject.GetComponent<RectTransform>().localPosition = new Vector3(-368.89f, 315f, 0);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(737.7f, 200f);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").GetChild(0).GetComponent<TMP_Text>().text = "<s>Objective 1: Cross the Ravine!</s>\n<s>Objective 2: Build up to the tower!</s>\nObjective 3: Build a bridge to the floating island!";
        GameObject t = Instantiate(Objective3Check, new Vector3(22.75f, 44.55f, 158.57f), Quaternion.identity);
    }

    public void GoToObjective4()
    {
        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[2].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(false);
        Objective3Completed = true;
        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[3].SetActive(true);
        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[3].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(true);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").gameObject.GetComponent<RectTransform>().localPosition = new Vector3(-368.89f, 315f, 0);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(737.7f, 200f);
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").GetChild(0).GetComponent<TMP_Text>().text = "<s>Objective 1: Cross the Ravine!</s>\n<s>Objective 2: Build up to the tower!</s>\n<s>Objective 3: Build a bridge to the floating island!</s>\nObjective 4: Help the Villagers reach their children over the wall!";
        GameObject t = Instantiate(Objective4Check, new Vector3(8.54f, 44.53f, 1.3f), Quaternion.identity);
    }

    public void EndMode()
    {
        EndGame = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject.Find("player").GetComponent<PlayerController>().controlsEnabled = false;
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("CodeImage").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("CodeImage").Find("OkButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("CodeImage").Find("OkButton2").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("CodeImage").Find("ContentText").gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        GameObject.Find("Canvas").transform.Find("CodeImage").Find("TitleText").gameObject.GetComponent<Text>().text = "Singleplayer Completed!";

        StartCoroutine(FinalEnd());
    }

    IEnumerator FinalEnd()
    {
        float GTimer = Mathf.Round(GameTimer * 10.0f) * 0.1f;
        GameObject GM = GameObject.Find("GameManager");

        int totalIncorrect = 0;

        foreach(int t in IncorrectAttempts)
        {
            totalIncorrect += t;
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").Find("ContentText").gameObject.GetComponent<Text>().text = "Player: " + GM.GetComponent<DataToHold>().PlayerName + "\n Total Time taken: " + GTimer.ToString() + "\nTotal Incorrect Attempts: " + totalIncorrect;

        GameObject.Find("GameManager").GetComponent<DataToHold>().SaveInfo(true);
        yield return new WaitForSeconds(5f);
        Destroy(GameObject.Find("GameManager").gameObject);
        SceneManager.LoadScene(0);
    }

    public void SelectBlocks(string blockrow)
    {
        GameObject.Find("GameManager").GetComponent<DataToHold>().SelectedBlock(blockrow);
    }

    public void SkipQuestion()
    {
        GameObject.Find("GameManager").GetComponent<DataToHold>().InteractingWithBAT.GetComponent<BlockAccessTable>().CloseQuestion();
    }
}
