using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Firebase;
using Firebase.Database;
using TMPro;
using System;
using Photon.Pun;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;

public class DataToHold : MonoBehaviour
{
    public Language SelectedLanguage;
    public List<GameObject> TutorialQuestions;
    public List<GameObject> QuizQuestions;
    public GameObject InteractingWithBAT;
    public Player thisPlayer;
    public string PlayerName;
    public bool Saved = false;
    public bool Saving = false;
    public int curindex = -5;

    public MultiplayerManager mp;

    private DatabaseReference db;

    /*private void OnApplication()
    {
        if (GameObject.Find("World").GetComponent<World>().WorldName == "Tutorial")
        {
            SaveUnFinTutorial();
        }
    }*/

    // Start is called before the first frame update
    void Start()
    {
        /*if (GameObject.Find("World").GetComponent<World>().WorldName == "Tutorial")
        {
            Application.wantsToQuit += SaveUnFinTutorial;
        }*/
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    //For selecting block after answering quiz question (TUTORIAL)
    public void SelectedBlock(string blockrow)
    {
        InteractingWithBAT.GetComponent<BlockAccessTable>().SelectBlock(blockrow);
    }

    public void SaveInfo(bool completed)
    {
        StartCoroutine(Save(completed));
    }

    public void SaveTutorial(int DataTypeTries, int LoopTries, int ConditionsTries)
    {
        GameObject.Find("TutorialTimer").GetComponent<TutorialTimer>().StartTimer = false;
        StartCoroutine(SaveTut(DataTypeTries, LoopTries, ConditionsTries));
    }

    public void SaveUnFinTutorial()
    {
        StartCoroutine(SaveUnfinishedTutorial());
    }

    public bool StopQuit()
    {
        Saving = true;
        StartCoroutine(SaveUnfinishedTutorial());
        return false;
    }
    IEnumerator SaveUnfinishedTutorial()
    {
        int index = 0;
        db.Child("Tutorial").Child(SelectedLanguage.ToString()).Child(PlayerName).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                print("Successful Retrieval");
                DataSnapshot snap = task.Result;

                int i = Convert.ToInt32(snap.ChildrenCount);

                index = i;
                print("Current Children: " + index);
                curindex = index;
            }
        });
        yield return new WaitForSeconds(0.3f);
        index++;
        db.Child("Tutorial").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Completed").SetValueAsync("No");
        db.Child("Tutorial").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Time").SetValueAsync(GameObject.Find("TutorialTimer").GetComponent<TutorialTimer>().timer.ToString("F1"));
        print("Done Saving");
        if (Saving)
        {
            Application.Quit();
        }
    }

    IEnumerator SaveTut( int dTries, int ltries, int ctries)
    {
        int index = 0;
        db.Child("Tutorial").Child(SelectedLanguage.ToString()).Child(PlayerName).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                print("Successful Retrieval");
                DataSnapshot snap = task.Result;

                int i = Convert.ToInt32(snap.ChildrenCount);

                index = i;
                print("Current Children: " + index);
            }
        });
        yield return new WaitForSeconds(1f);
        index++;
        db.Child("Tutorial").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Completed").SetValueAsync("Yes");
        db.Child("Tutorial").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("DataTypeTries").SetValueAsync(dTries.ToString());
        db.Child("Tutorial").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("LoopsTries").SetValueAsync(ltries.ToString());
        db.Child("Tutorial").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("ConditionsTries").SetValueAsync(ctries.ToString());
        db.Child("Tutorial").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Time").SetValueAsync(GameObject.Find("TutorialTimer").GetComponent<TutorialTimer>().timer.ToString("F1"));
    }
    IEnumerator Save(bool completed)
    {
        string worldn = GameObject.Find("World").GetComponent<World>().WorldName;
        SinglePlayerManager sp = GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>();
        int index = 0;
        db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                print("Successful Retrieval");
                DataSnapshot snap = task.Result;

                int i = Convert.ToInt32(snap.ChildrenCount);

                index = i;
                print("Current Children: " + index);
            }
        });
        yield return new WaitForSeconds(1f);
        index++;
        print("Next database entry: " + index);
        if (completed)
        {
            float AvTime = 0;
            foreach (float time in sp.ObjectiveTimers)
            {
                AvTime += time;
            }

            if (AvTime != 0)
            {
                AvTime = AvTime / sp.ObjectiveTimers.Count;
            }

            int TotalInc = 0;

            foreach (int inc in sp.IncorrectAttempts)
            {
                TotalInc += inc;
            }
            db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Average Time").SetValueAsync(AvTime.ToString("F1"));
            db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Total Time Taken").SetValueAsync(sp.GameTimer.ToString("F1"));
            db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Total Incorrect Attempts").SetValueAsync(TotalInc);

            for (int i = 0; i < sp.ObjectiveNames.Count; i++)
            {
                db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Objectives").Child(i.ToString()).Child("Objective Name").SetValueAsync(sp.ObjectiveNames[i]);
                db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Objectives").Child(i.ToString()).Child("Time").SetValueAsync(sp.ObjectiveTimers[i].ToString("F1"));
                db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Objectives").Child(i.ToString()).Child("Incorrect Attempts").SetValueAsync(sp.IncorrectAttempts[i]);
            }

            if(sp.SkippedObjectiveNames.Count > 0)
            {
                for(int j = 0; j < sp.SkippedObjectiveNames.Count; j++)
                {
                    db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("SkippedObj").Child(j.ToString()).Child("Objective Name").SetValueAsync(sp.SkippedObjectiveNames[j]);
                    db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("SkippedObj").Child(j.ToString()).Child("Time").SetValueAsync(sp.SkippedObjectiveTimers[j].ToString("F1"));
                    db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("SkippedObj").Child(j.ToString()).Child("Incorrect Attempts").SetValueAsync(sp.IncorrectAttemptsForSkipped[j]);
                }
            }
        }
        else
        {
            db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Completed").SetValueAsync("No");
            db.Child("SinglePlayer").Child(SelectedLanguage.ToString()).Child(PlayerName).Child("Session " + index.ToString()).Child("Time").SetValueAsync(sp.GameTimer.ToString("F1"));
        }
    }


    public void SaveMultiplayerInfo()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("PlayerConnector").GetComponent<PhotonView>().RPC("SaveMulti", RpcTarget.All);
        }
    }

    public void SaveMulti()
    {
        StartCoroutine(SavingMulti());
    }

    IEnumerator SavingMulti()
    {

        int MultiIndex = 0;
        db.Child("Multiplayer").Child(SelectedLanguage.ToString()).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                print("Successful Retrieval");
                DataSnapshot snap = task.Result;

                print(snap.ChildrenCount.ToString());
                int i = Convert.ToInt32(snap.ChildrenCount);

                MultiIndex = i;
                print("Current Children: " + MultiIndex);
            }
        });
        yield return new WaitForSeconds(0.5f);
        MultiIndex++;

        print("Match Index: " + MultiIndex);

        if (PhotonNetwork.IsMasterClient) {
            float avTime = 0;
            foreach(float t in mp.ObjectiveTimers)
            {
                avTime += t;
            }
            avTime = avTime/ mp.ObjectiveTimers.Count;

            int totalinc = 0;

            foreach(int t in mp.IncorrectAttempts)
            {
                totalinc += t;
            }
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player1 Stats").Child("Name").SetValueAsync(PhotonNetwork.PlayerList[0].NickName);
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player1 Stats").Child("Game Time").SetValueAsync(mp.GameTimer.ToString("F1"));
            if (mp.ObjectiveNames.Count > 0)
            {
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player1 Stats").Child("Average Obj Time").SetValueAsync(avTime.ToString("F1"));
            }
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player1 Stats").Child("Total Incorrect").SetValueAsync(totalinc.ToString());
            for (int i = 0; i < mp.ObjectiveNames.Count; i++)
            {
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player1 Stats").Child("Objectives").Child(i.ToString()).Child("Objective Name").SetValueAsync(mp.ObjectiveNames[i]);
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player1 Stats").Child("Objectives").Child(i.ToString()).Child("Objective Time").SetValueAsync(mp.ObjectiveTimers[i].ToString("F1"));
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player1 Stats").Child("Objectives").Child(i.ToString()).Child("Incorrect Attempts").SetValueAsync(mp.IncorrectAttempts[i]);
            }

            if (mp.SkippedObjectiveNames.Count > 0)
            {
                for (int j = 0; j < mp.SkippedObjectiveNames.Count; j++)
                {
                    db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player1 Stats").Child("SkippedObj").Child(j.ToString()).Child("Objective Name").SetValueAsync(mp.SkippedObjectiveNames[j]);
                    db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player1 Stats").Child("SkippedObj").Child(j.ToString()).Child("Time").SetValueAsync(mp.SkippedObjectiveTimers[j].ToString("F1"));
                    db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player1 Stats").Child("SkippedObj").Child(j.ToString()).Child("Incorrect Attempts").SetValueAsync(mp.IncorrectAttemptsForSkipped[j]);
                }
            }
        }
        else
        {
            float avTime = 0;
            foreach (float t in mp.ObjectiveTimers)
            {
                avTime += t;
            }
            avTime = avTime / mp.ObjectiveTimers.Count;
            int totalinc = 0;

            foreach (int t in mp.IncorrectAttempts)
            {
                totalinc += t;
            }
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player2 Stats").Child("Name").SetValueAsync(PhotonNetwork.PlayerList[1].NickName);
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player2 Stats").Child("Game Time").SetValueAsync(mp.GameTimer.ToString("F1"));
            if (mp.ObjectiveNames.Count > 0)
            {
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player2 Stats").Child("Average Obj Time").SetValueAsync(avTime.ToString("F1"));
            }
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player2 Stats").Child("Total Incorrect").SetValueAsync(totalinc.ToString());
            for (int i = 0; i < mp.ObjectiveNames.Count; i++)
            {
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player2 Stats").Child("Objectives").Child(i.ToString()).Child("Objective Name").SetValueAsync(mp.ObjectiveNames[i]);
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player2 Stats").Child("Objectives").Child(i.ToString()).Child("Objective Time").SetValueAsync(mp.ObjectiveTimers[i]);
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player2 Stats").Child("Objectives").Child(i.ToString()).Child("Incorrect Attempts").SetValueAsync(mp.IncorrectAttempts[i]);
            }

            if (mp.SkippedObjectiveNames.Count > 0)
            {
                for (int j = 0; j < mp.SkippedObjectiveNames.Count; j++)
                {
                    db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player2 Stats").Child("SkippedObj").Child(j.ToString()).Child("Objective Name").SetValueAsync(mp.SkippedObjectiveNames[j]);
                    db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player2 Stats").Child("SkippedObj").Child(j.ToString()).Child("Time").SetValueAsync(mp.SkippedObjectiveTimers[j].ToString("F1"));
                    db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + MultiIndex).Child("Player2 Stats").Child("SkippedObj").Child(j.ToString()).Child("Incorrect Attempts").SetValueAsync(mp.IncorrectAttemptsForSkipped[j]);
                }
            }
        }
    }


    public void SaveInterruptedMulti(string otherplayer)
    {
        StartCoroutine(SaveIntMulti(otherplayer));
    }

    IEnumerator SaveIntMulti(string otherplayer)
    {
        int index = 0;

        if (PhotonNetwork.LocalPlayer.NickName != otherplayer && thisPlayer == Player.Player1)
        {
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    print("Successful Retrieval");
                    DataSnapshot snap = task.Result;

                    int i = Convert.ToInt32(snap.ChildrenCount);

                    index = i;
                    print("Current Children: " + index);
                }
            });
            yield return new WaitForSeconds(0.2f);
            index++;
            if (mp.ObjectiveNames.Count > 0)
            {
                float avTime = 0;
                foreach (float t in mp.ObjectiveTimers)
                {
                    avTime += t;
                }
                avTime = avTime / mp.ObjectiveTimers.Count;

                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player1 Stats").Child("Average Obj Time").SetValueAsync(avTime.ToString("F1"));
                int totalinc = 0;

                foreach (int t in mp.IncorrectAttempts)
                {
                    totalinc += t;
                }

                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player1 Stats").Child("Total Incorrect").SetValueAsync(totalinc.ToString());
            }

            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player1 Stats").Child("Name").SetValueAsync(PhotonNetwork.LocalPlayer.NickName);
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player1 Stats").Child("Game Time").SetValueAsync(mp.GameTimer.ToString("F1"));
            for (int i = 0; i < mp.ObjectiveNames.Count; i++)
            {
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player1 Stats").Child("Objectives").Child(i.ToString()).Child("Objective Name").SetValueAsync(mp.ObjectiveNames[i]);
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player1 Stats").Child("Objectives").Child(i.ToString()).Child("Objective Time").SetValueAsync(mp.ObjectiveTimers[i]);
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player1 Stats").Child("Objectives").Child(i.ToString()).Child("Incorrect Attempts").SetValueAsync(mp.IncorrectAttempts[i]);
            }

            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player2 Stats").Child("Name").SetValueAsync(otherplayer);
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player2 Stats").Child("FORFEITED").SetValueAsync("YES");

        }
        else if(PhotonNetwork.LocalPlayer.NickName != otherplayer && thisPlayer == Player.Player2)
        {
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    print("Successful Retrieval");
                    DataSnapshot snap = task.Result;

                    int i = Convert.ToInt32(snap.ChildrenCount);

                    index = i;
                    print("Current Children: " + index);
                }
            });
            yield return new WaitForSeconds(0.2f);
            index++;

            if (mp.ObjectiveNames.Count > 0)
            {
                float avTime = 0;
                foreach (float t in mp.ObjectiveTimers)
                {
                    avTime += t;
                }
                avTime = avTime / mp.ObjectiveTimers.Count;

                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player2 Stats").Child("Average Obj Time").SetValueAsync(avTime.ToString("F1"));
                int totalinc = 0;

                foreach (int t in mp.IncorrectAttempts)
                {
                    totalinc += t;
                }

                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player2 Stats").Child("Total Incorrect").SetValueAsync(totalinc.ToString());
            }

            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player2 Stats").Child("Name").SetValueAsync(PhotonNetwork.LocalPlayer.NickName);
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player2 Stats").Child("Game Time").SetValueAsync(mp.GameTimer.ToString("F1"));
            for (int i = 0; i < mp.ObjectiveNames.Count; i++)
            {
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player2 Stats").Child("Objectives").Child(i.ToString()).Child("Objective Name").SetValueAsync(mp.ObjectiveNames[i]);
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player2 Stats").Child("Objectives").Child(i.ToString()).Child("Objective Time").SetValueAsync(mp.ObjectiveTimers[i]);
                db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player2 Stats").Child("Objectives").Child(i.ToString()).Child("Incorrect Attempts").SetValueAsync(mp.IncorrectAttempts[i]);
            }

            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player1 Stats").Child("Name").SetValueAsync(otherplayer);
            db.Child("Multiplayer").Child(SelectedLanguage.ToString()).Child("Match " + index).Child("Player1 Stats").Child("FORFEITED").SetValueAsync("YES");
        }
    }
}

public enum Language
{
    Python,
    CSharp,
    None
}

public enum Player
{
    Player1,
    Player2,
    None
}
