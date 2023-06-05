using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.PlayerLoop;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public bool timerbegin = false;

    public bool player1ready = false;
    public bool player2ready = false;

    private bool Spectating = false;

    public bool Updatecheat = false;
    public string player = "p1";
    public int objectivecheat = 1;

    public bool Objective1Completed = false;
    public bool Objective2Completed = false;
    public bool Objective3Completed = false;
    public bool Objective4Completed = false;
    private float timer = 3f;
    public GameObject GameTimerText;
    public GameObject WaitingScreen;

    public List<float> ObjectiveTimers = new List<float>();
    public List<string> ObjectiveNames = new List<string>();
    public List<int> IncorrectAttempts = new List<int>();

    public List<string> SkippedObjectiveNames = new List<string>();
    public List<float> SkippedObjectiveTimers = new List<float>();
    public List<int> IncorrectAttemptsForSkipped = new List<int>();

    public Camera SpectatorCamera;

    public bool StartedGame = false;

    public float GameTimer = 0;

    public bool EndGame = false;

    public GameObject Objective2Check;
    public GameObject Objective3Check;
    public GameObject Objective4Check;
    Vector2 turn;
    public List<GameObject> BlockAccessTables = new List<GameObject>();

    public GameObject EndScreen;

    public int BadgeNumber = 999;

    public void Awake()
    {
        if (SpectatorCamera.gameObject.activeSelf)
        {
            SpectatorCamera.gameObject.SetActive(false);
        }

        if (File.Exists("MultiBadge.data"))
        {
            FileStream stream = new FileStream("MultiBadge.data", FileMode.Open);
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
        if (Updatecheat)
        {
            Updatecheat = false;
            GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("CompletedObjective", RpcTarget.All, player, objectivecheat);
        }
        if (timerbegin)
        {
            GameObject.Find("Canvas").transform.Find("TimerText").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("TimerImage").gameObject.SetActive(true);
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

        if (!timerbegin && timer <= 0 && !EndGame)
        {
            GameTimer += Time.deltaTime;
        }

        if(WaitingScreen.activeSelf && !timerbegin)
        {
            if(player1ready && player2ready)
            {
                WaitingScreen.SetActive(false);
                timerbegin = true;
            }
        }

        if (Spectating)
        {
            turn.x += Input.GetAxis("Mouse X");
            turn.y += Input.GetAxis("Mouse Y");
            SpectatorCamera.gameObject.transform.parent.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
        }
    }

    public void PreMulti()
    {

        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("TitleText").GetComponent<Text>().text = "Multiplayer";
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("ContentText").GetComponent<Text>().text = "When you click the 'OK' button below, a timer will start. When the timer hits 0, the game begins.\n\n How long it takes you to complete the mode, how long it takes to complete code objectives and how many incorrect attempts you make will be saved.\n\nAnd Remember, you're competing with another player so answer questions and build as quickly as you can.\n\n Happy Coding!";
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(BeginMulti);
    }

    public void BeginMulti()
    {
        GameObject.Find("Canvas").transform.Find("CodeImage").gameObject.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("PlayerReady", RpcTarget.All, "p1");
        }
        else
        {
            GameObject.Find("PlayerConnector2").GetComponent<PhotonView>().RPC("PlayerReady", RpcTarget.All, "p2");
        }
        WaitingScreen.SetActive(true);
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
        if (GameObject.Find("SinglePlayerManager") != null)
        {
            GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[0].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(false);
            Objective1Completed = true;
            GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[1].SetActive(true);
            GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[1].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(true);
        }
        else if(GameObject.Find("MultiplayerManager") != null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("CompletedObjective", RpcTarget.All, "p1", 1);
            }
            else
            {
                GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("CompletedObjective", RpcTarget.All, "p2", 1);
            }
            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables[0].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(false);
            Objective1Completed = true;
            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables[1].SetActive(true);
            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables[1].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(true);
        }
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").GetChild(0).GetComponent<TMP_Text>().text = "<s>Objective 1: Cross the Ravine!</s>\nObjective 2: Build up to the tower!\nObjective 3: Build a bridge to the floating island!\nObjective 4: Help the Villagers reach their children over the wall!";
        GameObject t = Instantiate(Objective2Check, new Vector3(158.57f, 48.69f, 87f), Quaternion.identity);
    }

    public void GoToObjective3()
    {
        if (GameObject.Find("SinglePlayerManager") != null)
        {
            GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[1].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(false);
            Objective2Completed = true;
            GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[2].SetActive(true);
            GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[2].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(true);
        }
        else if (GameObject.Find("MultiplayerManager") != null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("CompletedObjective", RpcTarget.All, "p1", 2);
            }
            else
            {
                GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("CompletedObjective", RpcTarget.All, "p2", 2);
            }
            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables[1].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(false);
            Objective2Completed = true;
            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables[2].SetActive(true);
            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables[2].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(true);
        }
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").GetChild(0).GetComponent<TMP_Text>().text = "<s>Objective 1: Cross the Ravine!</s>\n<s>Objective 2: Build up to the tower!</s>\nObjective 3: Build a bridge to the floating island!\nObjective 4: Help the Villagers reach their children over the wall!";
        GameObject t = Instantiate(Objective3Check, new Vector3(22.75f, 44.55f, 158.57f), Quaternion.identity);
    }

    public void GoToObjective4()
    {
        if (GameObject.Find("SinglePlayerManager") != null)
        {
            GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[2].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(false);
            Objective3Completed = true;
            GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[3].SetActive(true);
            GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables[3].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(true);
        }
        else if (GameObject.Find("MultiplayerManager") != null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("CompletedObjective", RpcTarget.All, "p1", 3);
            }
            else
            {
                GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("CompletedObjective", RpcTarget.All, "p2", 3);
            }
            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables[2].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(false);
            Objective3Completed = true;
            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables[3].SetActive(true);
            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables[3].GetComponent<BlockAccessTable>().thisTableMarker.SetActive(true);
        }
        GameObject.Find("Canvas").transform.Find("ObjectiveImage").GetChild(0).GetComponent<TMP_Text>().text = "<s>Objective 1: Cross the Ravine!</s>\n<s>Objective 2: Build up to the tower!</s>\n<s>Objective 3: Build a bridge to the floating island!</s>\nObjective 4: Help the Villagers reach their children over the wall!";
        GameObject t = Instantiate(Objective4Check, new Vector3(8.54f, 44.53f, 1.3f), Quaternion.identity);
    }


    public void EndMode()
    {
        EndGame = true;
        if ((PhotonNetwork.IsMasterClient && GameObject.Find("PlayerConnector").GetComponent<PlayerConnector>().p2Obj4 != true) || (!PhotonNetwork.IsMasterClient && GameObject.Find("PlayerConnector").GetComponent<PlayerConnector>().p1Obj4 != true))
        {
            Spectating = true;
            SpectatorCamera.gameObject.SetActive(true);
            GameObject.Find("player").transform.GetChild(0).gameObject.SetActive(false);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("CompletedObjective", RpcTarget.All, "p1", 4);
        }
        else
        {
            GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("CompletedObjective", RpcTarget.All, "p2", 4);
        }
    }

    public void SL()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("PlayerConnector").GetComponent<PlayerConnector>().calcinc();
        }
        else
        {
            GameObject.Find("PlayerConnector2").GetComponent<PlayerConnector>().calcinc();
        }
        EndScreen.transform.GetChild(1).Find("P1Name").GetComponent<TMP_Text>().text = PhotonNetwork.PlayerList[0].NickName;
        EndScreen.transform.GetChild(1).Find("P2Name").GetComponent<TMP_Text>().text = PhotonNetwork.PlayerList[1].NickName;

        StartCoroutine(SaveAndLeave());
    }

    IEnumerator SaveAndLeave()
    {
        GameObject.Find("GameManager").GetComponent<DataToHold>().SaveMulti();
        yield return new WaitForSeconds(7f);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (GameObject.Find("GameManager") != null)
        {
            Destroy(GameObject.Find("GameManager").gameObject);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);

    }
    /*
    public void EndMode()
    {
        EndGame = true;
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
        GameObject MultiConnector = GameObject.Find("MultiplayerConnector");

        int totalIncorrect = 0;

        foreach (int t in MultiConnector.GetComponent<MultiplayerConnector>().IncorrectAttempts)
        {
            totalIncorrect += t;
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").Find("ContentText").gameObject.GetComponent<Text>().text = "Player: " + GM.GetComponent<DataToHold>().PlayerName + "\n Total Time taken: " + GTimer.ToString() + "\nTotal Incorrect Attempts: " + totalIncorrect;

        GameObject.Find("GameManager").GetComponent<DataToHold>().SaveInfo(true);
        yield return new WaitForSeconds(5f);
        Destroy(GameObject.Find("GameManager").gameObject);
        SceneManager.LoadScene(0);
    }
    */

    public void SelectBlocks(string blockrow)
    {
        GameObject.Find("GameManager").GetComponent<DataToHold>().SelectedBlock(blockrow);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if(GameObject.Find("PlayerConnector").GetComponent<PlayerConnector>().p1Obj4 != true && otherPlayer != PhotonNetwork.LocalPlayer && GameObject.Find("GameManager").GetComponent<DataToHold>().thisPlayer == Player.Player2)
        {
            StartCoroutine(InterruptGame(otherPlayer.NickName));
        }
        else if(GameObject.Find("PlayerConnector").GetComponent<PlayerConnector>().p2Obj4 != true && otherPlayer != PhotonNetwork.LocalPlayer && GameObject.Find("GameManager").GetComponent<DataToHold>().thisPlayer == Player.Player1) 
        {
            StartCoroutine(InterruptGame(otherPlayer.NickName));
        }
    }


    IEnumerator InterruptGame(string pname)
    {
            GameObject.Find("player").GetComponent<PlayerController>().controlsEnabled = false;
            GameObject.Find("Canvas").transform.Find("CodeImage").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("CodeImage").Find("TitleText").GetComponent<Text>().text = "GAME OVER";
            GameObject.Find("Canvas").transform.Find("CodeImage").Find("ContentText").GetComponent<Text>().text = pname + " Left the game!\n\nReturning to Lobby...";
            GameObject.Find("GameManager").GetComponent<DataToHold>().SaveInterruptedMulti(pname);
            yield return new WaitForSeconds(5f);
            Destroy(GameObject.Find("GameManager").gameObject);
            PhotonNetwork.LeaveRoom();
            yield return new WaitForSeconds(1f);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(0);
    }

    public void SkipQuestion()
    {
        GameObject.Find("GameManager").GetComponent<DataToHold>().InteractingWithBAT.GetComponent<BlockAccessTable>().CloseQuestion();
    }
}
