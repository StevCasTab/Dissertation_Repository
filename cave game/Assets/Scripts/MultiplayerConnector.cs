using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class MultiplayerConnector : MonoBehaviour, IPunObservable
{
    public string Player1Name;
    public string Player2Name;

    public string GameTimer;

    public List<string> player1ObjectiveNames = new List<string>();
    public List<float> player1ObjectiveTimes = new List<float>();
    public List<int> player1IncorrectAttempts = new List<int>();

    public List<string> player2ObjectiveNames = new List<string>();
    public List<float> player2ObjectiveTimes = new List<float>();
    public List<int> player2IncorrectAttempts = new List<int>();

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (GameObject.Find("GameManager").GetComponent<DataToHold>().thisPlayer == Player.Player1)
            {
                stream.SendNext(Player1Name);
            }
            else if(GameObject.Find("GameManager").GetComponent<DataToHold>().thisPlayer == Player.Player2){
                stream.SendNext(Player2Name);
            }
        }
        if (stream.IsReading)
        {
            if (GameObject.Find("GameManager").GetComponent<DataToHold>().thisPlayer == Player.Player1)
            {
                Player2Name = stream.ReceiveNext().ToString();
            }
            else if (GameObject.Find("GameManager").GetComponent<DataToHold>().thisPlayer == Player.Player2)
            {
                Player1Name = stream.ReceiveNext().ToString();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find("GameManager").GetComponent<DataToHold>().thisPlayer == Player.Player1)
        {
            Player1Name = GameObject.Find("GameManager").GetComponent<DataToHold>().PlayerName;
        }
        if (GameObject.Find("GameManager").GetComponent<DataToHold>().thisPlayer == Player.Player2)
        {
            Player2Name = GameObject.Find("GameManager").GetComponent<DataToHold>().PlayerName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
