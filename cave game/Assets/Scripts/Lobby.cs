using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using static Languages;

public class Lobby : MonoBehaviourPunCallbacks, IPunObservable
{

    public bool Updating = false;

    public GameObject ConnectingScreen;
    public GameObject DisConnectingScreen;

    private List<RoomInfo> Rooms;

    public GameObject CreateRoomPanel;
    private bool JoinedLobby = false;
    public List<RoomInfo> testlist = new List<RoomInfo>();
    public List<GameObject> ListOfRooms;
    public GameObject RowRoom;
    public GameObject ScrollViewContent;
    public GameObject MultiplayerScreen;
    public GameObject bg;

    private bool roomerror = false;


    public string thisplayer;

    private Player ThisPlayerInRoom = Player.None;

    public List<Photon.Realtime.Player> playersInRoom = new List<Photon.Realtime.Player>();

    public GameObject RoomScreen;

    void Start()
    {
        /*PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "1.0";

            PhotonNetwork.ConnectUsingSettings();
        }*/

    }

    private void Update()
    {
        thisplayer = ThisPlayerInRoom.ToString();
        if (PhotonNetwork.IsConnected && PhotonNetwork.NetworkClientState.ToString() == "JoinedLobby")
        {
            JoinedLobby = true;
        }

        if(Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ConnectToServer()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "1.0";

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /*void LoadScene()
    {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                print("Loading Scene..");
                PhotonNetwork.LoadLevel("Game");
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
            }
    }*/

    public void DisconnectFromServer()
    {
        DisConnectingScreen.SetActive(true);
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected To Master");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        JoinedLobby = true;
        ConnectingScreen.SetActive(false);
        GameObject.Find("Canvas").transform.Find("MultiplayerScreen").gameObject.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause.ToString() != "DisconnectByClientLogic")
        {
            print("Disconnected: " + cause.ToString() + " Server Address: " + PhotonNetwork.ServerAddress);
        }
        else
        {
            GameObject.Find("Canvas").GetComponent<TitleScreenManager>().CloseMultiplayerPanel();
        }
    }

    public void CreateRoom()
    {
        if (!roomerror)
        {
            roomerror = true;
            bool checkifnametaken = false;
            string pname = CreateRoomPanel.transform.Find("RoomInput").GetComponent<TMP_InputField>().text;
            foreach (RoomInfo room in Rooms)
            {
                if (room.Name == pname)
                {
                    checkifnametaken = true;
                }
            }

            if (!checkifnametaken)
            {
                if (CreateRoomPanel.transform.Find("NameInput").GetComponent<TMP_InputField>().text != "" && CreateRoomPanel.transform.Find("NameInput").GetComponent<TMP_InputField>().text != " ")
                {
                    RoomOptions roomOptions = new RoomOptions();
                    roomOptions.IsOpen = true;
                    roomOptions.IsVisible = true;
                    roomOptions.MaxPlayers = 2;
                    roomOptions.EmptyRoomTtl = 0;

                    string roomname = CreateRoomPanel.transform.Find("RoomInput").GetComponent<TMP_InputField>().text;
                    PhotonNetwork.JoinOrCreateRoom(roomname, roomOptions, TypedLobby.Default);
                    roomerror = false;
                }
                else
                {
                    StartCoroutine(Emptynamegiven());
                }
            }
            else
            {
                StartCoroutine(RoomNameTaken());
            }
        }
    }

    IEnumerator Emptynamegiven()
    {
        CreateRoomPanel.transform.Find("NameInput").GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(1f);
        CreateRoomPanel.transform.Find("NameInput").GetComponent<Image>().color = Color.white;
        roomerror = false;
    }
    IEnumerator RoomNameTaken()
    {
        CreateRoomPanel.transform.Find("RoomInput").GetComponent<Image>().color = Color.red;
        CreateRoomPanel.transform.Find("ErrorRoom").gameObject.SetActive(true);
        if (CreateRoomPanel.transform.Find("NameInput").GetComponent<TMP_InputField>().text == "" || CreateRoomPanel.transform.Find("NameInput").GetComponent<TMP_InputField>().text == " ")
        {
            CreateRoomPanel.transform.Find("NameInput").GetComponent<Image>().color = Color.red;
        }
        yield return new WaitForSeconds(1f);
        CreateRoomPanel.transform.Find("RoomInput").GetComponent<Image>().color = Color.white;
        CreateRoomPanel.transform.Find("ErrorRoom").gameObject.SetActive(false);
        CreateRoomPanel.transform.Find("NameInput").GetComponent<Image>().color = Color.white;
        roomerror = false;
    }

    public override void OnCreatedRoom()
    {
        ThisPlayerInRoom = Player.Player1;
        print("Room Created");
        string name = CreateRoomPanel.transform.Find("NameInput").GetComponent<TMP_InputField>().text;
        CloseCreateRoomPanel();
        MultiplayerScreen.SetActive(false);
        MultiplayerScreen.transform.Find("PlayerNameInput").GetComponent<TMP_InputField>().text = "";
        PhotonNetwork.NickName = name;
        RoomScreen.SetActive(true);
        RoomScreen.transform.GetChild(0).Find("RoomName").GetComponent<TMP_Text>().text = PhotonNetwork.CurrentRoom.Name;
        UpdateRoomPlayerList();
    }

    public void OpenCreateRoomPanel()
    {
        CreateRoomPanel.SetActive(true);
    }
    public void CloseCreateRoomPanel()
    {
        CreateRoomPanel.SetActive(false);
        CreateRoomPanel.transform.Find("NameInput").GetComponent<TMP_InputField>().text = "";
        CreateRoomPanel.transform.Find("RoomInput").GetComponent<TMP_InputField>().text = "";
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (!Updating)
        {
            Updating = true;
            print("Rooms Cache updated");

            /*foreach (GameObject t in ListOfRooms)
            {
                if (t == null)
                {
                    ListOfRooms.Remove(t);
                }
            }*/
            if(Rooms == null)
            {
                Rooms = new List<RoomInfo>();
            }
            int roomcount = 0;
            foreach (RoomInfo room in roomList)
            {
                Rooms.Add(room);
                if (room.RemovedFromList)
                {
                    int index = ListOfRooms.FindIndex(x => x.transform.GetChild(0).GetComponent<TMP_Text>().text == room.Name);


                    print("Deleting room at index: " + index);
                    if (index != -1)
                    {
                        GameObject t = ListOfRooms[index].gameObject;
                        ListOfRooms.Remove(t);
                        Destroy(t);
                        //testlist.Remove(room);
                    }
                    /*if (ListOfRooms.Count > 0)
                    {
                        for (int i = 0; i < ListOfRooms.Count; i++)
                        {
                            if (ListOfRooms[i].transform.GetChild(0).GetComponent<TMP_Text>().text == room.Name)
                            {
                                GameObject r = ListOfRooms[i].gameObject;
                                ListOfRooms.Remove(r);
                                Destroy(r);
                                testlist.Remove(room);


                                if (ListOfRooms[i] == null)
                                {
                                    ListOfRooms.Remove(ListOfRooms[i]);
                                }


                                break;
                            }
                        }
                    }*/
                }
                else
                {
                    roomcount += 1;
                    bool found = false;
                    foreach (GameObject t in ListOfRooms)
                    {
                        if (t.transform.GetChild(0).GetComponent<TMP_Text>().text == room.Name)
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        Debug.Log("Created Room Prefab");
                        GameObject temp = Instantiate(RowRoom);

                        temp.transform.parent = ScrollViewContent.transform;

                        temp.transform.GetChild(0).GetComponent<TMP_Text>().text = room.Name;

                        temp.transform.localScale = Vector3.one;

                        temp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(room.Name); });
                        ListOfRooms.Add(temp);

                        break;
                    }

                }
            }

            
            print("Room List: " + roomcount);
            if (ListOfRooms.Count > 0)
            {
                GameObject.Find("Canvas").transform.Find("MultiplayerScreen").Find("EmptyListText").gameObject.SetActive(false);
            }
            else
            {
                GameObject.Find("Canvas").transform.Find("MultiplayerScreen").Find("EmptyListText").gameObject.SetActive(true);
            }

            //testlist = roomList;

            //ListOfRooms.RemoveAll(s => s == null);


            Updating = false;
        }
    }

    public void UpdateRoomPlayerList()
    {
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            print("One Player found");
            RoomScreen.transform.GetChild(0).Find("Player1Input").GetComponent<TMP_InputField>().text = PhotonNetwork.PlayerList[0].NickName;
            RoomScreen.transform.GetChild(0).Find("Player2Input").GetComponent<TMP_InputField>().text = "Waiting...";
        }
        else if (PhotonNetwork.PlayerList.Length == 2)
        {
            print("Two Players found");
            print("Player 1: " + PhotonNetwork.PlayerList[0].NickName);
            print("Player 2: " + PhotonNetwork.PlayerList[1].NickName);
            RoomScreen.transform.GetChild(0).Find("Player1Input").GetComponent<TMP_InputField>().text = PhotonNetwork.PlayerList[0].NickName;
            RoomScreen.transform.GetChild(0).Find("Player2Input").GetComponent<TMP_InputField>().text = PhotonNetwork.PlayerList[1].NickName;
        }
        else
        {
            print("No players in room found");
        }
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            ThisPlayerInRoom = Player.Player2;
            RoomScreen.transform.GetChild(0).Find("StartButton").GetComponent<Button>().interactable = false;
        }
        MultiplayerScreen.SetActive(false);
        MultiplayerScreen.transform.Find("PlayerNameInput").GetComponent<TMP_InputField>().text = "";
        RoomScreen.SetActive(true);
        RoomScreen.transform.GetChild(0).Find("RoomName").GetComponent<TMP_Text>().text = PhotonNetwork.CurrentRoom.Name;
        UpdateRoomPlayerList();
    }
    public void LeaveCurrentRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void JoinRoom(string roomname)
    {
        string pname = MultiplayerScreen.transform.Find("PlayerNameInput").GetComponent<TMP_InputField>().text;
        if (PhotonNetwork.NetworkClientState.ToString() != "Joined" && pname != null && pname.Replace(" ","") != "" && pname.Replace(" ", "") != " ")
        {
            RoomScreen.transform.GetChild(0).Find("C#Button").GetComponent<Button>().interactable = false;
            RoomScreen.transform.GetChild(0).Find("PythonButton").GetComponent<Button>().interactable = false;
            PhotonNetwork.NickName = pname;
            PhotonNetwork.JoinRoom(roomname);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        print("Detected new player");
        if(newPlayer != PhotonNetwork.PlayerList[0])
        {
            RoomScreen.transform.GetChild(0).Find("Player2Input").GetComponent<TMP_InputField>().text = PhotonNetwork.PlayerList[1].NickName;
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        print("Detected player left");
        /*
        if(otherPlayer == PhotonNetwork.PlayerList[1] && otherPlayer != PhotonNetwork.LocalPlayer)
        {
            UpdateRoomPlayerList();
        }
        else if(otherPlayer == PhotonNetwork.PlayerList[1] && otherPlayer == PhotonNetwork.LocalPlayer)
        {
            RoomScreen.SetActive(false);
            RoomScreen.transform.GetChild(0).Find("RoomName").GetComponent<TMP_Text>().text = "";
            RoomScreen.transform.GetChild(0).Find("Player1Input").GetComponent<TMP_InputField>().text = "";
            RoomScreen.transform.GetChild(0).Find("Player2Input").GetComponent<TMP_InputField>().text = "Waiting...";
            MultiplayerScreen.SetActive(true);
        }
        if(otherPlayer == PhotonNetwork.PlayerList[0] && otherPlayer != PhotonNetwork.LocalPlayer)
        {
            LeaveCurrentRoom();
            RoomScreen.SetActive(false);
            RoomScreen.transform.GetChild(0).Find("RoomName").GetComponent<TMP_Text>().text = "";
            RoomScreen.transform.GetChild(0).Find("Player1Input").GetComponent<TMP_InputField>().text = "";
            RoomScreen.transform.GetChild(0).Find("Player2Input").GetComponent<TMP_InputField>().text = "Waiting...";
            MultiplayerScreen.SetActive(true);
        }
        else if(otherPlayer == PhotonNetwork.PlayerList[0] && otherPlayer == PhotonNetwork.LocalPlayer)
        {
            RoomScreen.SetActive(false);
            RoomScreen.transform.GetChild(0).Find("RoomName").GetComponent<TMP_Text>().text = "";
            RoomScreen.transform.GetChild(0).Find("Player1Input").GetComponent<TMP_InputField>().text = "";
            RoomScreen.transform.GetChild(0).Find("Player2Input").GetComponent<TMP_InputField>().text = "Waiting...";
        }*/

        if(ThisPlayerInRoom == Player.Player2)
        {
            int index = ListOfRooms.FindIndex(x => x.transform.GetChild(0).GetComponent<TMP_Text>().text == PhotonNetwork.CurrentRoom.Name);


            print("Deleting room at index: " + index);
            if (index != -1)
            {
                GameObject t = ListOfRooms[index].gameObject;
                ListOfRooms.Remove(t);
                Destroy(t);
                //testlist.Remove(room);
            }

            if(ListOfRooms.Count == 0)
            {
                GameObject.Find("Canvas").transform.Find("MultiplayerScreen").Find("EmptyListText").gameObject.SetActive(true);
            }
            LeaveCurrentRoom();

        }
        else if(ThisPlayerInRoom == Player.Player1)
        {
            UpdateRoomPlayerList();
        }
    }

    IEnumerator EmptyRoom()
    {
        yield return new WaitForSeconds(1f);
        LeaveCurrentRoom();
    }


    public void StartGame()
    {
        TitleScreenManager tm = GameObject.Find("Canvas").GetComponent<TitleScreenManager>();

        if (PhotonNetwork.PlayerList.Length == 2 && PhotonNetwork.IsMasterClient && tm.Language != LanguagesAvailable.None)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonView pv = PhotonView.Get(this);
            pv.RPC("StartG", RpcTarget.All);
        }
    }

    [PunRPC]
    public void StartG(){
        StartCoroutine(start());
    }

    IEnumerator start()
    {
        GameObject.Find("Canvas").transform.Find("bg (1)").gameObject.SetActive(true);
        RoomScreen.SetActive(false);
        TitleScreenManager tm = GameObject.Find("Canvas").GetComponent<TitleScreenManager>();
        TutorialQuestionHolder tqm = GameObject.Find("TitleQuestionHolder").GetComponent<TutorialQuestionHolder>();
        GameObject DToHold = Instantiate(tm.DataToHoldPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        DToHold.name = "GameManager";
        DToHold.GetComponent<DataToHold>().PlayerName = PhotonNetwork.LocalPlayer.NickName;
        if (tm.Language == LanguagesAvailable.CSharp)
        {
            DToHold.GetComponent<DataToHold>().SelectedLanguage = global::Language.CSharp;
            foreach (GameObject t in tqm.CSharpQuestions)
            {
                DToHold.GetComponent<DataToHold>().QuizQuestions.Add(t);
            }
        }
        else if (tm.Language == LanguagesAvailable.Python)
        {
            DToHold.GetComponent<DataToHold>().SelectedLanguage = global::Language.Python;
            foreach (GameObject t in tqm.PythonQuestions)
            {
                DToHold.GetComponent<DataToHold>().QuizQuestions.Add(t);
            }

        }

        if (PhotonNetwork.IsMasterClient)
        {
            DToHold.GetComponent<DataToHold>().thisPlayer = Player.Player1;
        }
        else
        {
            DToHold.GetComponent<DataToHold>().thisPlayer = Player.Player2;
        }

        DontDestroyOnLoad(DToHold);
        yield return new WaitForSeconds(0.2f);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MultiMAIN");
        }
    }
    public override void OnLeftRoom()
    {
        ThisPlayerInRoom = Player.None;
        RoomScreen.SetActive(false);
        if (ThisPlayerInRoom == Player.Player2)
        {
            RoomScreen.transform.GetChild(0).Find("StartButton").GetComponent<Button>().interactable = true;
        }
        RoomScreen.transform.GetChild(0).Find("RoomName").GetComponent<TMP_Text>().text = "";
        RoomScreen.transform.GetChild(0).Find("Player1Input").GetComponent<TMP_InputField>().text = "";
        RoomScreen.transform.GetChild(0).Find("Player2Input").GetComponent<TMP_InputField>().text = "Waiting...";
        RoomScreen.transform.GetChild(0).Find("C#Button").GetComponent<Button>().interactable = true;
        RoomScreen.transform.GetChild(0).Find("PythonButton").GetComponent<Button>().interactable = true;
        MultiplayerScreen.SetActive(true);
    }

    void OnApplicationQuit()
    {
        if (PhotonNetwork.InRoom)
        {
            LeaveCurrentRoom();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            string t = GameObject.Find("Canvas").GetComponent<TitleScreenManager>().Language.ToString();
            stream.SendNext(t);
        }
        else if(PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            string l = stream.ReceiveNext().ToString();

            if (l != null && l != "" && l != " ")
            {
                if (l == "Python" && GameObject.Find("Canvas").GetComponent<TitleScreenManager>().Language != Languages.LanguagesAvailable.Python)
                {
                    RoomScreen.transform.GetChild(0).Find("C#Button").GetComponent<Image>().color = Color.grey;
                    RoomScreen.transform.GetChild(0).Find("PythonButton").GetComponent<Image>().color = Color.white;
                    GameObject.Find("Canvas").GetComponent<TitleScreenManager>().Language = Languages.LanguagesAvailable.Python;
                    RoomScreen.transform.GetChild(0).Find("C#Button").GetComponent<RectTransform>().sizeDelta = new Vector2(160f, 160f);
                    RoomScreen.transform.GetChild(0).Find("PythonButton").GetComponent<RectTransform>().sizeDelta = new Vector2(193f, 192f);
                }
                else if (l == "CSharp" && GameObject.Find("Canvas").GetComponent<TitleScreenManager>().Language != Languages.LanguagesAvailable.CSharp)
                {
                    RoomScreen.transform.GetChild(0).Find("PythonButton").GetComponent<Image>().color = Color.grey;
                    RoomScreen.transform.GetChild(0).Find("C#Button").GetComponent<Image>().color = Color.white;
                    GameObject.Find("Canvas").GetComponent<TitleScreenManager>().Language = Languages.LanguagesAvailable.CSharp;
                    RoomScreen.transform.GetChild(0).Find("PythonButton").GetComponent<RectTransform>().sizeDelta = new Vector2(160f, 160f);
                    RoomScreen.transform.GetChild(0).Find("C#Button").GetComponent<RectTransform>().sizeDelta = new Vector2(193f, 192f);
                }
            }
        }
    }
}

