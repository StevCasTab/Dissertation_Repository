using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public void SelectBlocks(string blockrow)
    {
        GameObject.Find("GameManager").GetComponent<DataToHold>().SelectedBlock(blockrow);
    }

    public void RestartTutorial()
    {
        Application.wantsToQuit -= GameObject.Find("GameManager").GetComponent<DataToHold>().StopQuit;
        SceneManager.LoadScene(1);
    }

    public void SkipQuestion()
    {
        GameObject.Find("GameManager").GetComponent<DataToHold>().InteractingWithBAT.GetComponent<BlockAccessTable>().CloseQuestion();
    }
}
