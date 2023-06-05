using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerConnector : MonoBehaviour, IPunObservable
{

    public Vector3 CurrentPlayerPos;

    public Vector3 ReceivedPos;

    public GameObject ObjectToMove;
    public bool Ready = false;
    private bool otherready = false;
    public MultiplayerManager mp;

    public int p1IncAttempts = 0;
    public int p2IncAttempts = 0;

    public float P1GameTimer = 0;
    public float P2GameTimer = 0;

    public bool p1Obj1 = false;
    public bool p1Obj2 = false;
    public bool p1Obj3 = false;
    public bool p1Obj4 = false;

    public bool p2Obj1 = false;
    public bool p2Obj2 = false;
    public bool p2Obj3 = false;
    public bool p2Obj4 = false;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(CurrentPlayerPos);
            calcinc();
            stream.SendNext(GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().GameTimer);
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(p1IncAttempts);
            }
            else
            {
                stream.SendNext(p2IncAttempts);
            }
        }

        if (stream.IsReading)
        {
            ReceivedPos = (Vector3) stream.ReceiveNext();
                if (PhotonNetwork.IsMasterClient)
                {
                    P2GameTimer = (float) stream.ReceiveNext();
                    p2IncAttempts = (int)stream.ReceiveNext();
                }
                else
                {
                    P1GameTimer = (float) stream.ReceiveNext();
                    p1IncAttempts = (int)stream.ReceiveNext();

                }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CurrentPlayerPos = GameObject.Find("player").transform.position;

        if (ObjectToMove != null)
        {
            var step = 20f * Time.deltaTime;
            ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, ReceivedPos, step);
            if (ObjectToMove.transform.Find("Camera").gameObject.activeSelf == false)
            {
                ObjectToMove.transform.GetChild(0).LookAt(GameObject.Find("player").transform);
            }
            else
            {
                ObjectToMove.transform.GetChild(0).LookAt(ObjectToMove.transform.Find("Camera"));
            }
        }
    }

    public void calcinc()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            p1IncAttempts = 0;

            foreach(int i in mp.IncorrectAttempts)
            {
                p1IncAttempts += i;
            }
        }
        else
        {
            p2IncAttempts = 0;

            foreach (int i in mp.IncorrectAttempts)
            {
                p2IncAttempts += i;
            }
        }
    }
    [PunRPC]
    public void PlayerReady(string specificplayer)
    {
        if(specificplayer == "p1")
        {
            mp.player1ready = true;
        }
        else if(specificplayer == "p2")
        {
            mp.player2ready = true;
        }
    }

    [PunRPC]
    public void CompletedObjective(string player, int obj)
    {
        if(player == "p1")
        {
            if (obj == 1 && !p1Obj1)
            {
                GameObject.Find("Canvas").transform.Find("ObjectiveImage").Find("P1Name").GetComponent<RectTransform>().localPosition = new Vector3(259.82f, 33f, 0);
                p1Obj1 = true;
            }
            else if(obj == 2 && !p1Obj2)
            {
                GameObject.Find("Canvas").transform.Find("ObjectiveImage").Find("P1Name").GetComponent<RectTransform>().localPosition = new Vector3(259.82f, 0.78217f, 0);
                p1Obj2 = true;
            }
            else if(obj == 3 && !p1Obj3)
            {
                GameObject.Find("Canvas").transform.Find("ObjectiveImage").Find("P1Name").GetComponent<RectTransform>().localPosition = new Vector3(259.82f, -39f, 0);
                p1Obj3 = true;
            }
            else if(obj == 4 && !p1Obj4)
            {
                p1Obj4 = true;
                Destroy(GameObject.Find("Canvas").transform.Find("ObjectiveImage").Find("P1Name").gameObject);
            }
        }
        else if(player == "p2")
        {
            if (obj == 1 && !p2Obj1)
            {
                p2Obj1 = true;
                GameObject.Find("Canvas").transform.Find("ObjectiveImage").Find("P2Name").GetComponent<RectTransform>().localPosition = new Vector3(396.1f, 33f, 0);
            }
            else if(obj == 2 && !p2Obj2)
            {
                p2Obj2 = true;
                GameObject.Find("Canvas").transform.Find("ObjectiveImage").Find("P2Name").GetComponent<RectTransform>().localPosition = new Vector3(396.1f, 0.78217f, 0);
            }
            else if(obj == 3 && !p2Obj3)
            {
                p2Obj3 = true;
                GameObject.Find("Canvas").transform.Find("ObjectiveImage").Find("P2Name").GetComponent<RectTransform>().localPosition = new Vector3(396.1f, -39f, 0);
            }
            else if(obj == 4 && !p2Obj4)
            {
                p2Obj4 = true;
                Destroy(GameObject.Find("Canvas").transform.Find("ObjectiveImage").Find("P2Name").gameObject);
            }
        }

        if(obj == 4)
        {
            if(player == "p2" && !PhotonNetwork.IsMasterClient)
            {
                GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().EndGame = true;
            }
            if(player == "p1" && PhotonNetwork.IsMasterClient)
            {
                GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().EndGame = true;
            }
        }
        if(p1Obj1 && p2Obj1 && p1Obj2 && p2Obj2 && p1Obj3 && p2Obj3 && p1Obj4 && p2Obj4 && PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("SL", RpcTarget.All);
        }
    }

    [PunRPC]
    public void SL()
    {
        GameObject.Find("player").GetComponent<PlayerController>().controlsEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().SL();
    }
}
