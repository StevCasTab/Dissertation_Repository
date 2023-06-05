using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCheck : MonoBehaviour
{
    public int ObjectiveNumber = 99;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "player")
        {
            if (GameObject.Find("SinglePlayerManager") != null)
            {
                if (ObjectiveNumber == 1)
                {
                    GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().GoToObjective2();
                }
                else if (ObjectiveNumber == 2)
                {
                    GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().GoToObjective3();
                }
                else if (ObjectiveNumber == 3)
                {
                    GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().GoToObjective4();
                }
                else if (ObjectiveNumber == 4)
                {
                    GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().EndMode();
                }
            }
            if(GameObject.Find("MultiplayerManager") != null)
            {
                if (ObjectiveNumber == 1)
                {
                    GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().GoToObjective2();
                }
                else if (ObjectiveNumber == 2)
                {
                    GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().GoToObjective3();
                }
                else if (ObjectiveNumber == 3)
                {
                    GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().GoToObjective4();
                }
                else if (ObjectiveNumber == 4)
                {
                    GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().EndMode();
                }
            }
            Destroy(this.gameObject);
        }
    }
}
