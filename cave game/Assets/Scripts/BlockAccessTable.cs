using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockAccessTable : MonoBehaviour
{
    public BlockAccessType ThisTableType;
    public GameObject CodeImage;

    public Text TitleText;
    public Text ContentText;
    private bool OpenedImage = false;
    public Language SelectedLanguage;

    public bool norestart = false;

    public float objectivetimer = 0;
    public string objectivename;
    public int IncorrectAttempts = 0;

    private bool InvalidQuestion = false;
    public bool CompletedTutorial = false;
    public GameObject SelectionButtons1;
    public GameObject SelectionButtons2;

    public List<GameObject> QuizQuestions;
    public List<int> RecentQuestionsAnswered = new List<int>();

    public GameObject InstantiatedQuestion;
    private bool AttemptingQuestion = false;
    private int LastQuestionEntered = 99;
    private bool beginningQuiz = false;

    public int completedconcepts = 0;

    public int tutorialbadge = 9999;
    public GameObject thisTableMarker;

    public List<Button> selButtons1 = new List<Button>();
    public List<Button> selButtons2 = new List<Button>();
    public GameObject CloseQuestionButton;

    public bool ifclosedquestion = false;

    private Vector2 DefaultCodeImageSize;
    public bool TutorialMode = true;

    private byte Selectedblock = 99;
    public bool UnlockBlocks1 = false;
    public bool UnlockBlocks2 = false;
    public bool UnlockBlocks3 = false;

    public bool CompletedDataTypes = false;
    public bool CompletedLoops = false;
    public bool CompletedConditions = false;

    public int invalidDTypeAttempts = 0;
    public int invalidLoopAttempts = 0;
    public int invalidCondAttempts = 0;

    public bool SavedSkipped = false;

    public int DataTypeTries = 0;
    public int LoopsTries = 0;
    public int ConditionsTries = 0;

    public bool Saving = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("World") != null)
        {
            if (File.Exists("TutorialBadge.data") /*&& GameObject.Find("World").GetComponent<World>().WorldName == "Tutorial"*/)
            {
                CloseQuestionButton = CodeImage.transform.Find("CloseQuestionButton").gameObject;
                FileStream stream = new FileStream("TutorialBadge.data", FileMode.Open);
                if (stream != null)
                {
                    print("World Found");
                }
                try
                {
                    BinaryFormatter fm = new BinaryFormatter();
                    string f = fm.Deserialize(stream) as string;
                    tutorialbadge = Convert.ToInt32(f);
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
            DefaultCodeImageSize = CodeImage.GetComponent<RectTransform>().sizeDelta;
            TitleText = CodeImage.transform.Find("TitleText").GetComponent<Text>();
            ContentText = CodeImage.transform.Find("ContentText").GetComponent<Text>();
            SelectedLanguage = GameObject.Find("GameManager").GetComponent<DataToHold>().SelectedLanguage;


            SelectionButtons1 = CodeImage.transform.Find("SelectionButtons1").gameObject;
            SelectionButtons2 = CodeImage.transform.Find("SelectionButtons2").gameObject;

            foreach (Transform t in SelectionButtons1.transform)
            {
                selButtons1.Add(t.GetComponent<Button>());
            }
            foreach (Transform t in SelectionButtons2.transform)
            {
                selButtons2.Add(t.GetComponent<Button>());
            }

            if (SelectionButtons1.activeSelf)
            {
                SelectionButtons1.SetActive(false);
            }
            if (SelectionButtons2.activeSelf)
            {
                SelectionButtons2.SetActive(false);
            }
        }

        if (File.Exists("TutorialBadge.data") /*&& GameObject.Find("World").GetComponent<World>().WorldName == "Tutorial"*/)
        {
            //CloseQuestionButton = CodeImage.transform.Find("CloseQuestionButton").gameObject;
            FileStream stream = new FileStream("TutorialBadge.data", FileMode.Open);
            if (stream != null)
            {
                print("World Found");
            }
            try
            {
                BinaryFormatter fm = new BinaryFormatter();
                string f = fm.Deserialize(stream) as string;
                tutorialbadge = Convert.ToInt32(f);
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
        CloseQuestionButton = GameObject.Find("Canvas").transform.Find("CodeImage").Find("CloseQuestionButton").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("World") != null)
        {
            if (GameObject.Find("World").GetComponent<World>().WorldName != "Tutorial")
            {
                if (InstantiatedQuestion != null && !ifclosedquestion)
                {
                    AttemptingQuestion = true;
                    objectivetimer += Time.deltaTime;

                    if (objectivetimer >= 30 && IncorrectAttempts >= 2 && CloseQuestionButton.activeSelf == false)
                    {
                        CloseQuestionButton.SetActive(true);
                    }
                }
                else if (InstantiatedQuestion == null && AttemptingQuestion && !ifclosedquestion)
                {
                    AttemptingQuestion = false;
                    string objname = objectivename.Replace("(Clone)", "");
                    if (GameObject.Find("SinglePlayerManager") != null)
                    {
                        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().ObjectiveTimers.Add(objectivetimer);
                        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().ObjectiveNames.Add(objname);
                        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().IncorrectAttempts.Add(IncorrectAttempts);
                    }
                    else if (GameObject.Find("MultiplayerManager") != null)
                    {
                        GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().ObjectiveTimers.Add(objectivetimer);
                        GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().ObjectiveNames.Add(objname);
                        GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().IncorrectAttempts.Add(IncorrectAttempts);
                    }
                    objectivetimer = 0;
                    objectivename = null;
                    IncorrectAttempts = 0;
                }
                else if (ifclosedquestion && !SavedSkipped)
                {
                    SavedSkipped = true;
                    AttemptingQuestion = false;
                    string objname = objectivename.Replace("(Clone)", "");
                    if (GameObject.Find("SinglePlayerManager") != null)
                    {
                        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().SkippedObjectiveTimers.Add(objectivetimer);
                        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().SkippedObjectiveNames.Add(objname);
                        GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().IncorrectAttemptsForSkipped.Add(IncorrectAttempts);
                    }
                    else if (GameObject.Find("MultiplayerManager") != null)
                    {
                        GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().SkippedObjectiveTimers.Add(objectivetimer);
                        GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().SkippedObjectiveNames.Add(objname);
                        GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().IncorrectAttemptsForSkipped.Add(IncorrectAttempts);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isgrounded = GameObject.Find("player").GetComponent<PlayerController>().m_Grounded;
        if (!CodeImage.activeSelf && isgrounded && !OpenedImage)
        {
            InteractedWithTable();
            GameObject.Find("GameManager").GetComponent<DataToHold>().InteractingWithBAT = this.gameObject;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        bool isgrounded = GameObject.Find("player").GetComponent<PlayerController>().m_Grounded;
        if (!CodeImage.activeSelf && isgrounded && !OpenedImage)
        {
            InteractedWithTable();
            GameObject.Find("GameManager").GetComponent<DataToHold>().InteractingWithBAT = this.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OpenedImage = false;
        GameObject.Find("GameManager").GetComponent<DataToHold>().InteractingWithBAT = null;
    }


    public void CloseCodeImage()
    {
        GameObject.Find("Canvas").transform.Find("crosshair").gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameObject.Find("player").GetComponent<PlayerController>().controlsEnabled = true;
        CodeImage.SetActive(false);
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("GameManager").GetComponent<DataToHold>().InteractingWithBAT = null;
    }

    public void Close()
    {
        CloseCodeImage();
    }


    public void InteractedWithTable()
    {
        if (CodeImage.transform.Find("OkButton").gameObject.activeSelf == false)
        {
            CodeImage.transform.Find("OkButton").gameObject.SetActive(true);
        }
        GameObject.Find("Canvas").transform.Find("crosshair").gameObject.SetActive(false);
        CodeImage.SetActive(true);
        GameObject.Find("player").GetComponent<PlayerController>().controlsEnabled = false;
        GameObject.Find("player").GetComponent<PlayerController>().m_MoveDirection.x = 0;
        GameObject.Find("player").GetComponent<PlayerController>().m_MoveDirection.z = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (TutorialMode)
        {
            CodeImage.transform.Find("OkButton3").gameObject.SetActive(false);
            CodeImage.transform.Find("OkButton2").gameObject.SetActive(false);
            CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(0f, -376f, 0);
            if (ThisTableType == BlockAccessType.None)
            {
                ContentText.fontSize = 30;
                TitleText.GetComponent<Text>().text = "Block Access Table";
                ContentText.text = "No Concepts were explored yet. Explore the world and discover new programming concepts to unlock new blocks";
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(Close);
                CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Close";
            }
            else if (ThisTableType == BlockAccessType.DataTypes)
            {
                ContentText.fontSize = 30;
                TitleText.text = "Instructions: DataTypes";
                if (SelectedLanguage == Language.CSharp)
                {
                    ContentText.text = "Welcome!\n\nThis table explores the concept of DataTypes for the <color=cyan>C#</color> language!.\n\n Click the button below to start the tutorial!";
                }
                else
                {
                    ContentText.text = "Welcome!\n\nThis table explores the concept of DataTypes for the <color=cyan>Python</color> language!.\n\n Click the button below to start the tutorial!";
                }
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(DataTypesGuide);
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ok!";
            }
            else if (ThisTableType == BlockAccessType.Loops)
            {
                ContentText.fontSize = 30;
                TitleText.text = "Instructions: Loops";
                if (SelectedLanguage == Language.CSharp)
                {
                    ContentText.text = "Welcome!\n\nThis table explores the concept of Loops for the <color=cyan>C#</color> language!.\n\n Click the button below to start the tutorial!";
                }
                else
                {
                    ContentText.text = "Welcome!\n\nThis table explores the concept of Loops for the <color=cyan>Python</color> language!.\n\n Click the button below to start the tutorial!";
                }
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep1);
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ok!";
            }
            else if (ThisTableType == BlockAccessType.Conditions)
            {
                ContentText.fontSize = 30;
                TitleText.text = "Instructions: Conditions";
                if (SelectedLanguage == Language.CSharp)
                {
                    ContentText.text = "Welcome!\n\nThis table explores the concept of conditions for the <color=cyan>C#</color> language!.\n\n Click the button below to start the tutorial!";
                }
                else
                {
                    ContentText.text = "Welcome!\n\nThis table explores the concept of conditions for the <color=cyan>Python</color> language!.\n\n Click the button below to start the tutorial!";
                }
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(ConditionsStep1);
                CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ok!";
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            }
        }
        else
        {
            //bool check = GameObject.Find("World").GetComponent<World>().Singleplayer;
            //if (!check)
            //{
            TitleText.text = "Block Quiz";
            if (norestart)
            {
                ContentText.text = "Ready to answer a code quiz?\n\nClick on the 'Ready' button below!\n\nOr click the 'Exit' button to exit the code quiz";
            }
            else
            {
                if (ThisTableType == BlockAccessType.DataTypes && invalidDTypeAttempts >= 3)
                {
                    ContentText.text = "Ready to answer a code quiz?\n\nClick on the 'Ready' button below!\n\nOr click the 'Exit' button to exit the code quiz\n\nRecent data shows you struggled during the DataTypes tutorial.\n\nClick the 'Restart' Button to replay the tutorial";
                }
                else if (ThisTableType == BlockAccessType.Loops && invalidLoopAttempts >= 3)
                {
                    ContentText.text = "Ready to answer a code quiz?\n\nClick on the 'Ready' button below!\n\nOr click the 'Exit' button to exit the code quiz\n\nRecent data shows you struggled during the Loops tutorial.\n\nClick the 'Restart' Button to replay the tutorial";
                }
                else if (ThisTableType == BlockAccessType.Conditions && invalidCondAttempts >= 3)
                {
                    ContentText.text = "Ready to answer a code quiz?\n\nClick on the 'Ready' button below!\n\nOr click the 'Exit' button to exit the code quiz\n\nRecent data shows you struggled during the Conditions tutorial.\n\nClick the 'Restart' Button to replay the tutorial";
                }
                else
                {
                    ContentText.text = "Ready to answer a code quiz?\n\nClick on the 'Ready' button below!\n\nOr click the 'Exit' button to exit the code quiz\n\nClick the 'Restart' Button if you want to retry the tutorial";
                }
            }
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(InitializeBlockAchiever);
            GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ready";
            CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
            CodeImage.transform.Find("OkButton2").gameObject.SetActive(true);
            CodeImage.transform.Find("OkButton2").GetComponent<RectTransform>().localPosition = new Vector3(254f, -376f, 0);
            CodeImage.transform.Find("OkButton2").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton2").GetComponent<Button>().onClick.AddListener(Close);
            GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton2").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            CodeImage.transform.Find("OkButton2").GetChild(0).GetComponent<Text>().text = "Exit";
            if (GameObject.Find("World").GetComponent<World>().WorldName == "Tutorial" && !norestart)
            {
                CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                if (ThisTableType == BlockAccessType.DataTypes)
                {
                    CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(DataTypesGuide);
                }
                else if (ThisTableType == BlockAccessType.Loops)
                {
                    CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(LoopsStep1);
                }
                else if (ThisTableType == BlockAccessType.Conditions)
                {
                    CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(ConditionsStep1);
                }
                CodeImage.transform.Find("OkButton3").GetChild(0).GetComponent<Text>().text = "Restart";
            }
            else
            {
                if (CodeImage.transform.Find("OkButton3") != null)
                {
                    CodeImage.transform.Find("OkButton3").gameObject.SetActive(false);
                }
            }

            /*}
            else
            {
                if(GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().Objective1Completed == false)
                {
                    TitleText.text = "Objective 1";
                    ContentText.text = "Reach the other side of the ravine!";
                    ContentText.alignment = TextAnchor.MiddleCenter;
                }
                else if(GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().Objective2Completed == false)
                {
                    TitleText.text = "Objective 2";
                    ContentText.text = "Build a staircase to the top of the tower!";
                }
                else if(GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().Objective3Completed == false)
                {
                    TitleText.text = "Objective 3";
                    ContentText.text = "Build a bridge to the floating island!";
                }


                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(InitializeBlockAchiever);
                CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ready";
                CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-118f, -376f, 0);
                CodeImage.transform.Find("OkButton2").gameObject.SetActive(true);
                CodeImage.transform.Find("OkButton2").GetComponent<Button>().onClick.RemoveAllListeners();
                CodeImage.transform.Find("OkButton2").GetComponent<Button>().onClick.AddListener(Close);
                CodeImage.transform.Find("OkButton2").GetChild(0).GetComponent<Text>().text = "Exit";
            }*/
        }
        OpenedImage = true;
    }

    public void ClearAllSelListeners()
    {
        foreach (Button b in selButtons1)
        {
            b.onClick.RemoveAllListeners();
        }
        foreach(Button b in selButtons2)
        {
            b.onClick.RemoveAllListeners();
        }
    }

    void DataTypesGuide()
    {
        DataTypeTries++;
        if(CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text == "Ready")
        {
            CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ok!";
        }
        invalidDTypeAttempts = 0;
        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(0f, -376f, 0);
        CodeImage.transform.Find("OkButton3").gameObject.SetActive(false);
        CodeImage.transform.Find("OkButton2").gameObject.SetActive(false);
        if (SelectedLanguage == Language.CSharp)
        {
            ContentText.text = "Common DataTypes known in <color=cyan>C#</color> can be seen below:";
            ContentText.text += "\n\n<color=cyan>int</color> - stores integers (whole numbers) without decimals\n\n<color=cyan>double</color> - stores decimal numbers up to 15 digits\n\n<color=cyan>float</color> - stores decimal numbers up to 7 digits designated by the letter 'f'\n\n<color=cyan>string</color> - stores text, such as \"Hello World\"\n\n<color=cyan>bool</color> - stores a value with two states: <color=cyan>True</color>/<color=cyan>False</color>\n\n<color=cyan>Char</color> - Stores single characters such as 'A' or 'B'";
            ContentText.fontSize = 22;
        }
        else if(SelectedLanguage == Language.Python)
        {
            ContentText.text = "Common DataTypes known in <color=cyan>Python</color> can be seen below:";
            ContentText.text += "\n\n<color=cyan>int</color> - stores integers (whole numbers) without decimals\n\n<color=cyan>float</color> - stores decimal numbers\n\n<color=cyan>complex</color> - stores two values, one real value (first value) and one imaginary value (second value) (3+7j) \n\n<color=cyan>str</color> - stores text, such as \"Hello World\"\n\n<color=cyan>bool</color> - stores a value with two states: <color=cyan>True</color>/<color=cyan>False</color>\n\n<color=cyan>Bytearray</color> - Stores a sequence of values which range from 0-255";
            ContentText.fontSize = 22;
        }

        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(DataTypesStep1);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void DataTypesStep1()
    {
        if (SelectedLanguage == Language.CSharp)
        {
            ContentText.fontSize = 28;
            ContentText.text = "===============================\n\n---- temp = -----;\n\n<color=cyan>var</color> average = temp * water / 2f;\n\n===============================\n\n In the example above, we can tell the variable 'temp' is a float because it is eventually divided by '2f' which is already a <color=cyan>float</color> value\n\nHowever, if it didn't have the letter 'f' and was a decimal number (Example: 2.34), its datatype would be '<color=cyan>double</color>'.";
        }
        else
        {
            ContentText.fontSize = 28;
            ContentText.text = "===============================\n\n temp = -----;\n\naverage = temp * water / 2.5;\n\n===============================\n\n In the example above, we can tell the variable 'temp' is a float because it is eventually divided by a decimal number '2.5' which is already a <color=cyan>float</color> value in <color=cyan>*PYTHON*</color>.";
        }
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(DataTypesStep2);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void DataTypesStep2()
    {
        ContentText.fontSize = 30;
        CodeImage.transform.Find("OkButton").gameObject.SetActive(false);
        TitleText.text = "Quiz: DataTypes";
        if (SelectedLanguage == Language.CSharp)
        {
            ContentText.text = "<b>Guess the variable's datatype below:</b>\n===============================\n\n---- a = -----;\n\n<color=cyan>var</color> e = \"Example\" + a;\n\n Console.Writeline(e);\n\n===============================\n\n";
        }
        else if(SelectedLanguage == Language.Python)
        {
            ContentText.text = "<b>Guess the variable's datatype below:</b>\n===============================\n\na = -----;\n\ne = 2 + a;\n\n print(e);\n\n===============================\n\n";
        }
        SelectionButtons1.SetActive(true);
        SelectionButtons2.SetActive(true);

        if (SelectedLanguage == Language.CSharp)
        {
            selButtons1[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "String";
            selButtons1[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "Char";
            selButtons2[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "Bool";
            selButtons2[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "Double";

            selButtons1[0].onClick.AddListener(DataTypesStep3);
            selButtons1[1].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(1)); });
            selButtons2[0].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(2)); });
            selButtons2[1].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(3)); });
        }
        else if(SelectedLanguage == Language.Python)
        {
            selButtons1[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "complex";
            selButtons1[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "float";
            selButtons2[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "int";
            selButtons2[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "bytearray";

            selButtons1[0].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(0)); });
            selButtons1[1].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(1)); });
            selButtons2[0].onClick.AddListener(DataTypesStep3);
            selButtons2[1].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(3)); });
        }
        selButtons1[0].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
        selButtons1[1].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
        selButtons2[0].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
        selButtons2[1].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });

    }


    IEnumerator InvalidDataButton(int i)
    {
        invalidDTypeAttempts++;
        if(i == 0)
        {
            selButtons1[0].gameObject.GetComponent<Image>().color = Color.red;
        }
        else if (i == 1)
        {
            selButtons1[1].gameObject.GetComponent<Image>().color = Color.red;
        }
        else if (i == 2)
        {
            selButtons2[0].gameObject.GetComponent<Image>().color = Color.red;
        }
        else if (i == 3)
        {
            selButtons2[1].gameObject.GetComponent<Image>().color = Color.red;
        }
        yield return null;
    }

    void DataTypesStep3()
    {
        selButtons1[0].gameObject.GetComponent<Image>().color = Color.green;
        ClearAllSelListeners();
        selButtons1[0].gameObject.GetComponent<Image>().color = Color.white;
        selButtons1[1].gameObject.GetComponent<Image>().color = Color.white;
        selButtons2[0].gameObject.GetComponent<Image>().color = Color.white;
        selButtons2[1].gameObject.GetComponent<Image>().color = Color.white;

        if (SelectedLanguage == Language.CSharp)
        {
            ContentText.text = "<b>Guess the variable's datatype below:</b>\n===============================\n\n---- b = -----;\n\n<color=cyan>var</color> c = 4.55 * b;\n\nConsole.Writeline(e);\n\n===============================\n\n";
            selButtons1[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "Int";
            selButtons1[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "Double";
            selButtons2[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "Bool";
            selButtons2[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "Float";

            selButtons1[0].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(0)); });
            selButtons1[1].onClick.AddListener(DataTypesStep4);
            selButtons2[0].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(2)); });
            selButtons2[1].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(3)); });
        }
        else if (SelectedLanguage == Language.Python)
        {
            ContentText.text = "<b>Guess the variable's datatype below:</b>\n===============================\n\nb = complex(5-7j)\n\nprint(b)\n\n===============================\n\n";
            selButtons1[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "int";
            selButtons1[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "str";
            selButtons2[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "bool";
            selButtons2[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "complex";

            selButtons1[0].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(0)); });
            selButtons1[1].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(1)); });
            selButtons2[0].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(2)); });
            selButtons2[1].onClick.AddListener(delegate { DataTypesStep4(); });
        }

        selButtons1[0].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
        selButtons1[1].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
        selButtons2[0].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
        selButtons2[1].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void DataTypesStep4()
    {
        selButtons1[1].gameObject.GetComponent<Image>().color = Color.green;
        ClearAllSelListeners();
        selButtons1[0].gameObject.GetComponent<Image>().color = Color.white;
        selButtons1[1].gameObject.GetComponent<Image>().color = Color.white;
        selButtons2[0].gameObject.GetComponent<Image>().color = Color.white;
        selButtons2[1].gameObject.GetComponent<Image>().color = Color.white;

        if (SelectedLanguage == Language.CSharp)
        {
            ContentText.text = "<b>Guess the datatype of variable '<color=yellow>c</color>' below:</b>\n===============================\n\n<color=cyan>string</color> temp = \"Hello\";\n <color=cyan>foreach</color>(---- c <color=cyan>in</color> temp){\nConsole.WriteLine(c);\n}\n===============================\n\n";
            selButtons1[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "Double";
            selButtons1[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "Bool";
            selButtons2[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "Char";
            selButtons2[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "String";

            selButtons1[0].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(0)); });
            selButtons1[1].onClick.AddListener((delegate { StartCoroutine(InvalidDataButton(1)); }));
            selButtons2[0].onClick.AddListener(DataTypeStep5);
            selButtons2[1].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(3)); });
        }
        else if (SelectedLanguage == Language.Python)
        {
            ContentText.text = "<b>Guess the datatype of variable '<color=yellow>c</color>' below:</b>\n===============================\n\n<color=cyan>string</color> temp = \"Hello\";\n c = bytearray(temp,\"UTF-8\") \n===============================\n\n";
            selButtons1[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "complex";
            selButtons1[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "bytearray";
            selButtons2[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "int";
            selButtons2[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "str";

            selButtons1[0].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(0)); });
            selButtons1[1].onClick.AddListener(DataTypeStep5);
            selButtons2[0].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(2)); });
            selButtons2[1].onClick.AddListener(delegate { StartCoroutine(InvalidDataButton(3)); });
        }

        selButtons1[0].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
        selButtons1[1].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
        selButtons2[0].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
        selButtons2[1].onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });

    }

    void DataTypeStep5()
    {
        selButtons2[0].gameObject.GetComponent<Image>().color = Color.green;
        ClearAllSelListeners();
        selButtons1[0].gameObject.GetComponent<Image>().color = Color.white;
        selButtons1[1].gameObject.GetComponent<Image>().color = Color.white;
        selButtons2[0].gameObject.GetComponent<Image>().color = Color.white;
        selButtons2[1].gameObject.GetComponent<Image>().color = Color.white;

        SelectionButtons1.SetActive(false);
        SelectionButtons2.SetActive(false);
        if (CodeImage.transform.Find("OkButton").gameObject.activeSelf == false)
        {
            CodeImage.transform.Find("OkButton").gameObject.SetActive(true);
        }
        if (TutorialMode)
        {
            ContentText.fontSize = 27;
            if (completedconcepts < 2)
            {
                if (SelectedLanguage == Language.CSharp)
                {
                    if (invalidDTypeAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the DataType tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing DataType Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the DataType tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing DataType Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn datatypes better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(DataTypesGuide);
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
                else
                {
                    if (invalidDTypeAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the DataType tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing DataType Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the DataType tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing DataType Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn datatypes better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(DataTypesGuide);
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
            }
            else
            {
                if (SelectedLanguage == Language.CSharp)
                {
                    if (invalidDTypeAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the DataType tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing DataType Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the DataType tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing DataType Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn datatypes better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(DataTypesGuide);
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
                else
                {
                    if (invalidDTypeAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the DataType tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing DataType Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the DataType tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing DataType Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn datatypes better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(DataTypesGuide);
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
            }
            CodeImage.transform.Find("OkButton").gameObject.SetActive(true);
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(UnlockBlocks);
        }
        else
        {
            InteractedWithTable();
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void LoopsStep1()
    {
        LoopsTries++;
        if (CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text == "Ready")
        {
            CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ok!";
        }
        invalidLoopAttempts = 0;
        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(0f, -376f, 0);
        CodeImage.transform.Find("OkButton3").gameObject.SetActive(false);
        CodeImage.transform.Find("OkButton2").gameObject.SetActive(false);
        if (SelectedLanguage == Language.CSharp)
        {
            ContentText.text = "Loops in <color=cyan>C#</color> can be implemented in 3 different ways:\n\n";
            ContentText.text += "1. <color=cyan>For</color> Loops\n\n2.<color=cyan>Foreach</color> Loops\n\n3.<color=cyan>While</color> Loops";
            ContentText.fontSize = 30;
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep2);
        }
        else
        {
            ContentText.text = "Loops in <color=cyan>Python</color> can be implemented in 2 different ways:\n\n";
            ContentText.text += "1. <color=cyan>For</color> Loops\n\n2.<color=cyan>While</color> Loops";
            ContentText.fontSize = 30;
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep2);
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void LoopsStep2()
    {
        if(SelectedLanguage == Language.CSharp)
        {
            TitleText.text = "Instructions: Mathematical Conditions";
            if (ThisTableType == BlockAccessType.Loops)
            {
                ContentText.text = "Before learning about loops, you must learn about mathematical conditions.\n\nThis is important concept to learn as they are required to implement loops which utilize integers";
            }
            else
            {
                CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";
                CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
                ContentText.fontSize = 30;
                ContentText.text = "To correctly implement a condition, you must learn about mathematical conditions.\n\nThis is important concept to learn as they are required to implement conditions which utilize integers ";
            }
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();

            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep3);
        }
        else
        {
            if (ThisTableType == BlockAccessType.Loops)
            {
                TitleText.text = "Instructions: For Loops";
                ContentText.text = "In Python, For Loops repeat a block of code for each item in a list\n================================\n\n\n\n\n================================\nAs can be seen in the example above, the temporary variable 'x' is declared to represent each item in the numbers array.";
                ContentText.fontSize = 26;
                CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
                CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "numbers = [1,2,3,4,5]\n<color=cyan>for</color> x <color=cyan>in</color> numbers:\n\n        <color=cyan>print</color>(x)";
                CodeImage.transform.Find("CodeText").GetComponent<Text>().fontSize = 33;
                CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(8f, -44f, 0);
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep3);
            }
            else
            {
                CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";
                CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
                ContentText.fontSize = 30;
                ContentText.text = "To correctly implement a condition, you must learn about mathematical conditions.\n\nThis is important concept to learn as they are required to implement conditions which utilize integers ";
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep3);
            }
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void LoopsStep3()
    {
        if(SelectedLanguage == Language.CSharp)
        {
            ContentText.fontSize = 25;
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            if (ThisTableType == BlockAccessType.Loops)
            {
                ContentText.text = "Logical conditions from mathematics are the basis of creating mathematical statements for loops to follow.\n\nThe six mathematical conditions can be seen below:\n\n> - Greater than\n\n< - Smaller than\n\n>= - Greater than/Equal to\n\n<= - Smaller than/Equal to\n\n== - Equal to\n\n!= - Not Equal to";
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep31);
            }
            else
            {
                ContentText.text = "Logical conditions from mathematics are the basis of creating mathematical statements for if statements to follow.\n\nThe six mathematical conditions can be seen below:\n\n> - Greater than\n\n< - Smaller than\n\n>= - Greater than/Equal to\n\n<= - Smaller than/Equal to\n\n== - Equal to\n\n!= - Not Equal to";
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(ConditionsStep31);
            }
        }
        else
        {
            if (ThisTableType == BlockAccessType.Loops)
            {
                TitleText.text = "Quiz: For Loops";
                ContentText.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 7.9f, 0);
                ContentText.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(877f, 622f);
                CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
                ContentText.text = "Complete the following for loop statements";
                CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(0, -368f, 0);
                TitleText.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 375f, 0);
                CodeImage.transform.Find("Line").GetComponent<RectTransform>().localPosition = new Vector3(0f, 344f, 0f);
                CodeImage.transform.Find("Line").GetComponent<RectTransform>().sizeDelta = new Vector2(1035f, 9.6f);
                InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[1], new Vector3(0, 0, 0), Quaternion.identity);
                InstantiatedQuestion.transform.SetParent(CodeImage.transform);
                InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1f);
                InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckPythonLoops);
                CodeImage.GetComponent<RectTransform>().localPosition = new Vector3(19.2f, 0, 0);
            }
            else
            {
                ContentText.fontSize = 25;
                ContentText.text = "Logical conditions from mathematics are the basis of creating mathematical statements for if statements to follow.\n\nThe six mathematical conditions can be seen below:\n\n> - Greater than\n\n< - Smaller than\n\n>= - Greater than/Equal to\n\n<= - Smaller than/Equal to\n\n== - Equal to\n\n!= - Not Equal to";
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(ConditionsStep2);
            }
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }
    void CheckPythonLoops()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check4 = false;
            bool check5 = false;

            string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "");
            string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "");
            string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "");
            string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "");
            string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text;

            if (t1.ToLower() == "numbers")
            {
                check1 = true;
            }
            if (t2.ToLower() == "animals")
            {
                check2 = true;
            }
            if (t3.ToLower() != "" && t3.ToLower() != " ")
            {
                check3 = true;
            }
            if (t4.ToLower() == "keys")
            {
                check4 = true;
            }
            if (t5.ToLower() != "" && t5.ToLower() != "")
            {
                List<string> temp = new List<string>();
                string[] tholder = t5.Split(" ");

                foreach (string th in tholder)
                {
                    if (th != " " && th != "")
                    {
                        temp.Add(th);
                    }
                }

                if (temp.Count > 0)
                {
                    if (temp[0] != "" && temp[0] != " " && temp[1] == "in" && temp[2] == "cars")
                    {
                        check5 = true;
                    }
                }
            }

            if (check1 && check2 && check3 && check4 && check5)
            {
                LoopsStep4();
                InvalidQuestion = false;
            }
            else
            {
                invalidLoopAttempts++;
                StartCoroutine(InvalidPythonLoops(check1, check2, check3, check4, check5));
            }
        }
    }

    IEnumerator InvalidPythonLoops(bool c1, bool c2, bool c3, bool c4, bool c5)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        if (!c5)
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
        }
        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }

    void LoopsStep31()
    {
        TitleText.text = "Instructions: For Loops";
        ContentText.text = "In C#,  For Loops are utilized to repeat a block of code for a specific amount of times\n\n\n\n<b>Section 1</b> executes once at the start of the loop\n\n<b>Section 2</b> is the condition to repeat the loop\n\n<b>Section 3</b> executes every time the Loop ends\n\n\n\n\n\n<b>Section 1</b> creates a temporary variable \n(int i = 0)\n\n<b>Section 2</b> defines the condition (i < 5).\nIf condition is true, loop continues.\nIf it's false, the loop will end.\n\n<b>Section 3</b> adds to the integer in Section 1(i++) each time the loop repeats";
        ContentText.fontSize = 19;
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "\n<color=cyan>for</color>(*Section1*; *Section2*; *Section3*){...\n\n\n\n\n\n\n<color=cyan>for</color>(<color=cyan>int</color> i = 0; i < 5; i++){...";
        CodeImage.transform.Find("CodeText").GetComponent<Text>().fontSize = 30;
        CodeImage.transform.Find("CodeText").GetComponent<Text>().alignment = TextAnchor.UpperCenter;
        CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(0f, 100f, 0);
        CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(623f,389f);
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep32);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void LoopsStep32()
    {
        TitleText.text = "Quiz: For Loops";
        CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
        ContentText.text = "Complete the following for loop statements";
        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(0, -368f, 0);
        TitleText.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 375f, 0);
        InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[0], new Vector3(0, 0, 0), Quaternion.identity);
        InstantiatedQuestion.transform.SetParent(CodeImage.transform);
        InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1f);
        InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckCSharpLoops);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }
    void CheckCSharpLoops()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            print("Checking");
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check31 = false;
            bool check4 = false;
            bool check41 = false;

            string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text;
            string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text;
            string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text;
            string t31 = InstantiatedQuestion.transform.Find("Input31").GetComponent<TMP_InputField>().text;
            string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text;
            string t41 = InstantiatedQuestion.transform.Find("Input41").GetComponent<TMP_InputField>().text;


            string t1f = t1.Replace(" ", "").ToLower();
            string t2f = t2.Replace(" ", "").ToLower();
            string t3f = t3.Replace(" ", "").ToLower();
            string t31f = t31.Replace(" ", "").ToLower();
            string t4f = t4.Replace(" ", "").ToLower();
            string t41f = t41.Replace(" ", "").ToLower();
            if (t1f == "x<10")
            {
                check1 = true;
            }
            if (t2f == "y>20")
            {
                check2 = true;
            }
            if (t3f == "intk=4" || t3f == "intk=4;")
            {
                check3 = true;
            }
            if (t31f == "k<40" || t31f == "k<40;")
            {
                check31 = true;
            }
            if (t4f == "intl=50" || t4f == "intl=50;")
            {
                check4 = true;
            }
            if(t41f == "l>10" || t41f == "l>10;")
            {
                check41 = true;
            }

            if (check1 && check2 && check3 && check31 && check4 && check41)
            {
                LoopsStep4();
                InvalidQuestion = false;
            }
            else
            {
                invalidLoopAttempts++;
                StartCoroutine(InvalidCSharpLoops1(check1, check2, check3, check31, check4, check41));
            }
        }
    }

    IEnumerator InvalidCSharpLoops1(bool c1, bool c2, bool c3, bool c31, bool c4, bool c41)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c31)
        {
            InstantiatedQuestion.transform.Find("Input31").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input31").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        if (!c41)
        {
            InstantiatedQuestion.transform.Find("Input41").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input41").GetComponent<Image>().color = Color.green;
        }
        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input31").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input41").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }
    void LoopsStep4()
    {
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep5);
        if (InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        /*if (SelectedLanguage == Language.CSharp) {
            ContentText.fontSize = 25;
            TitleText.text = "Instructions: While Loops";
            ContentText.text = "Now we will move onto While Loops.\n\n While loops repeat a block of code as long as their specified conditions result to 'True'\n\n\n\n\n\n\n In the example below, the code will run over and over as long as the variable (j) is greater than 0";
            CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
            CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "\n\n\n\n\n\n\n\n\n\n<color=cyan>while</color>(condition){\n    ... \n}\n\n\n\n\n\n<color=cyan>int</color> j = 10;\n<color=cyan>while</color>( j > 0 ){\n    Console.WriteLine(j);\n    j--;\n}";
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(623f, 824f);
            CodeImage.transform.Find("CodeText").GetComponent<Text>().alignment = TextAnchor.UpperLeft;
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(223f, 100f, 0);
        }
        else
        {

        }*/
        

        TitleText.text = "Instructions: Add/Deduct Integers";
        ContentText.text = "An Integral part of";
        ContentText.fontSize = 23;
        if(SelectedLanguage == Language.Python)
        {
            ContentText.text += " While Loops";
        }
        else
        {
            ContentText.text += " loops that utilize numerical conditions";
        }

        ContentText.text += " is adding/deducting an integer mentioned in the loop. Below are several ways to add/deduct from an integer\n\n\n\n\n\ni++ automatically adds 1 to the integer\ni+=1 and i=i+1 adds 1 to the integer but can be modified to add as much as you want\n\n\n\n\ni-- automatically deducts 1 from the integer\ni-=1 and i=i-1 deducts 1 from the integer but can be modified to deduct as much as you wantw";

        CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "\ni++\ni+=1\ni=i+1\n\n\n\n\ni--\ni-=1\ni=i-1";
        CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(76.4f, 539f);
        CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(0, -33.5f, 0);
        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(0, -376f, 0);
        CodeImage.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });

    }

    void LoopsStep5()
    {
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";
        CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        if (SelectedLanguage == Language.CSharp)
        {
            ContentText.fontSize = 30;
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckLoopsStep5);
            ContentText.text = "Complete the following for loops:";
            InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[2], new Vector3(0, 0, 0), Quaternion.identity);
            InstantiatedQuestion.transform.SetParent(CodeImage.transform);
            InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1.4f, 1.4f, 1f);
        }
        else
        {
            CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(316.9f,483.6f);
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(27.5f, -61f, 0);
            TitleText.text = "Instructions: While Loops";
            ContentText.text = "Now we will move onto While Loops.\n\nWhile loops repeat a block of code as long as their specified conditions result to 'True'\n\n\n\n\n\n In the example below, the code will run over and over again as long as the variable (i) is smaller than 6";
            CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "<color=cyan>While</color> *condition*:\n   //Code To Execute\n\n\n\n\n\n  i = 1\n\n  <color=cyan>While</color> i < 6:\n      print(i)\n      i += 1";
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep7);
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CheckLoopsStep5()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            bool check1 = false;
            bool check12 = false;
            bool check2 = false;
            bool check3 = false;
            bool check31 = false;
            bool check32 = false;
            bool check4 = false;
            bool check41 = false;
            bool check42 = false;

            string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t12 = InstantiatedQuestion.transform.Find("Input12").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t31 = InstantiatedQuestion.transform.Find("Input31").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t32 = InstantiatedQuestion.transform.Find("Input32").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t41 = InstantiatedQuestion.transform.Find("Input41").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t42 = InstantiatedQuestion.transform.Find("Input42").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

            if (t1 == "c<20")
            {
                check1 = true;
            }
            if (t12 == "c++" || t12 == "c+=1" || t12 == "c=c+1")
            {
                check12 = true;
            }
            if (t2 == "y>=40")
            {
                check2 = true;
            }
            if (t3 == "intf=0")
            {
                check3 = true;
            }
            if (t31 == "f>10")
            {
                check31 = true;
            }
            if (t32 == "f+=2" || t32 == "f=f+2")
            {
                check32 = true;
            }
            if (t4 == "intg=8")
            {
                check4 = true;
            }
            if (t41 == "g!=0")
            {
                check41 = true;
            }
            if (t42 == "g-=2" || t42 == "g=g-2")
            {
                check42 = true;
            }

            if (check1 && check12 && check2 && check3 && check31 && check32 && check4 && check41 && check42)
            {
                InvalidQuestion = false;
                LoopsStep6();
            }
            else
            {
                invalidLoopAttempts++;
                StartCoroutine(InvalidCSharpLoops2(check1, check12, check2, check3, check31, check32, check4, check41, check42));
            }
        }
    }

    IEnumerator InvalidCSharpLoops2(bool c1, bool c12, bool c2, bool c3, bool c31, bool c32, bool c4, bool c41, bool c42)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c12)
        {
            InstantiatedQuestion.transform.Find("Input12").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input12").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c31)
        {
            InstantiatedQuestion.transform.Find("Input31").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input31").GetComponent<Image>().color = Color.green;
        }
        if (!c32)
        {
            InstantiatedQuestion.transform.Find("Input32").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input32").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        if (!c41)
        {
            InstantiatedQuestion.transform.Find("Input41").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input41").GetComponent<Image>().color = Color.green;
        }
        if (!c42)
        {
            InstantiatedQuestion.transform.Find("Input42").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input42").GetComponent<Image>().color = Color.green;
        }

        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input12").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input31").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input32").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input41").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input42").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }

    void LoopsStep6()
    {
        if (InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        TitleText.text = "Instructions: While Loops";
        ContentText.text = "Now we will move onto While Loops\n\n While loops repeat a block of code as long as their specified conditions result to 'True'\n\n\n\n\n\n In the example below, the code will run over and over as long as the variable (j) is greater than 0";
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "\n\n<color=cyan>while</color>(condition){\n    ... \n}\n\n\n\n\n\n<color=cyan>int</color> j = 10;\n<color=cyan>while</color>( j > 0 ){\n    Console.WriteLine(j);\n    j--;\n}";
        CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
        CodeImage.transform.Find("CodeText").GetComponent<Text>().alignment = TextAnchor.UpperLeft;
        CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(324f, 539f);
        ContentText.fontSize = 25;
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep7);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void LoopsStep7()
    {
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";
        CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
        TitleText.text = "Quiz: While Loops";
        ContentText.text = "Complete the following while loops";
        if (SelectedLanguage == Language.CSharp)
        {
            InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[3], new Vector3(0, 0, 0), Quaternion.identity);
            InstantiatedQuestion.transform.SetParent(CodeImage.transform);
            InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1.4f, 1.4f, 1f);
        }
        else
        {
            InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[4], new Vector3(0, 0, 0), Quaternion.identity);
            InstantiatedQuestion.transform.SetParent(CodeImage.transform);
            InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1f);
        }
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckLoopsStep7);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CheckLoopsStep7()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check31 = false;
            bool check4 = false;
            bool check41 = false;

            if(SelectedLanguage == Language.CSharp)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ","").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t31 = InstantiatedQuestion.transform.Find("Input31").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t41 = InstantiatedQuestion.transform.Find("Input41").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();


                if (t1 == "x<10")
                {
                    check1 = true;
                }
                if (t2 == "y>10")
                {
                    check2 = true;
                }
                if (t3 == "z>=20")
                {
                    check3 = true;
                }
                if (t31 == "z-=5" || t31 == "z=z-5" || t31 == "z-=5;" || t31 == "z=z-5;")
                {
                    check31 = true;
                }
                if (t4 == "g<=30")
                {
                    check4 = true;
                }
                if (t41 == "g+=2" || t41 == "g=g+2" || t41 == "g+=2;" || t41 == "g=g+2;")
                {
                    check41 = true;
                }

                if (check1 && check2 && check3 && check31 && check4 && check41)
                {
                    InvalidQuestion = false;
                    LoopsStep8();
                }
                else
                {
                    invalidLoopAttempts++;
                    StartCoroutine(InvalidLoopsStep7(check1, check2, check3, check31, check4, check41));
                }
            }
            else
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t31 = InstantiatedQuestion.transform.Find("Input31").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t41 = InstantiatedQuestion.transform.Find("Input41").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 == "r>20")
                {
                    check1 = true;
                }
                if (t2 == "t<25")
                {
                    check2 = true;
                }
                if (t3 == "s!=50")
                {
                    check3 = true;
                }
                if (t31 == "s-=5" || t31 == "s=s-5" || t31 == "s-=5;" || t31 == "s=s-5;")
                {
                    check31 = true;
                }
                if (t4 == "j<=12")
                {
                    check4 = true;
                }
                if (t41 == "j+=3" || t41 == "j=j+3" || t41 == "j+=3;" || t41 == "j=j+3;")
                {
                    check41 = true;
                }

                if (check1 && check2 && check3 && check31 && check4 && check41)
                {
                    InvalidQuestion = false;
                    LoopFinal();
                }
                else
                {
                    invalidLoopAttempts++;
                    StartCoroutine(InvalidLoopsStep7(check1, check2, check3, check31, check4, check41));
                }
            }
        }
    }

    IEnumerator InvalidLoopsStep7(bool c1, bool c2, bool c3, bool c31, bool c4, bool c41)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c31)
        {
            InstantiatedQuestion.transform.Find("Input31").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input31").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        if (!c41)
        {
            InstantiatedQuestion.transform.Find("Input41").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input41").GetComponent<Image>().color = Color.green;
        }

        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input31").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input41").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }

    void LoopsStep8()
    {
        if (InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        TitleText.text = "Instructions: Foreach Loops";
        ContentText.text = "Well done!. The final loop we will cover is a foreach loop\nForeach loops repeat a block of code for every single item found in a list\n\n\n\n\nThe below example outputs all elements in the <b>cars</b> array\n\n\n\n\n\nThe foreach loop works by naming a temporary variable (<color=cyan>string</color> i) to represent each element in the list and then stating which list it is from. (<color=cyan>in</color> cars)";
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "\n\n<color=cyan>foreach</color>(<color=lightcoral>type</color> variablename in <color=cyan>array/list</color>){\n   //Code to execute\n}\n\n\n\n<color=cyan>string</color>[] cars = {\"Volvo\",\"BMW\",\"Ford\",\"Mazda\"};\n<color=cyan>foreach</color> (<color=cyan>string</color> i <color=cyan>in</color> cars){\n     Console.WriteLine(i);\n}";
        CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
        CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(648f, 539f);
        CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(90.9f, -22f, 0);
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep9);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void LoopsStep9()
    {
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";
        CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
        TitleText.text = "Quiz: Foreach Loops";
        ContentText.text = "Complete the following foreach loops";
        InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[5], new Vector3(0, 0, 0), Quaternion.identity);
        InstantiatedQuestion.transform.SetParent(CodeImage.transform);
        InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1.4f, 1.4f, 1f);
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckLoopsStep9);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    public void CheckLoopsStep9()
    {
        InvalidQuestion = true;
        bool check1 = false;
        bool check2 = false;
        bool check3 = false;
        bool check4 = false;
        bool check5 = false;

        string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
        string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
        string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.ToLower();
        string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
        string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.ToLower();

        if (t1.ToLower() == "nums")
        {
            check1 = true;
        }
        if (t2.ToLower() == "animals")
        {
            check2 = true;
        }
        if (t3.ToLower() != "" && t3.ToLower() != " ")
        {
            List<string> temp = new List<string>();
            string[] tholder = t3.Split(" ");

            foreach (string th in tholder)
            {
                if (th != " " && th != "")
                {
                    temp.Add(th);
                }
            }

            if (temp[0] == "int" && temp[1] != "" && temp[1] != " ")
            {
                check3 = true;
            }
            else
            {
                print("bad sep" + temp[0]);
                print("bad sep2" + temp[1]);
            }
        }
        else
        {
            print("bad" + t3);
        }
        if (t4.ToLower() == "keys")
        {
            check4 = true;
        }
        else
        {
            print("bad" + t4);
        }
        if (t5.ToLower() != "" && t5.ToLower() != "")
        {
            List<string> temp = new List<string>();
            string[] tholder = t5.Split(" ");

            foreach (string th in tholder)
            {
                if (th != " " && th != "")
                {
                    temp.Add(th);
                }
            }

            if (temp.Count > 0)
            {
                if (temp[0] == "bool" && temp[1] != "" && temp[1] != " " && temp[2] == "in" && temp[3] == "premchecks")
                {
                    check5 = true;
                }
            }
        }

        if (check1 && check2 && check3 && check4 && check5)
        {
            LoopFinal();
            InvalidQuestion = false;
        }
        else
        {
            invalidLoopAttempts++;
            StartCoroutine(InvalidPythonLoops(check1, check2, check3, check4, check5));
        }
    }

    void LoopFinal()
    {
        if (InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        if (CodeImage.transform.Find("OkButton").gameObject.activeSelf == false)
        {
            CodeImage.transform.Find("OkButton").gameObject.SetActive(true);
        }
        TitleText.text = "Loops Completed";
        if (TutorialMode)
        {
            if (completedconcepts < 2)
            {
                if (SelectedLanguage == Language.CSharp)
                {
                    if (invalidLoopAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Loops tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing Loop Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Loops tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing Loop Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn loops better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(LoopsStep1);
                        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
                else
                {
                    if (invalidLoopAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Loops tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing Loop Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Loops tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing Loop Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn loops better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(LoopsStep1);
                        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
            }
            else
            {
                if (SelectedLanguage == Language.CSharp)
                {
                    if (invalidLoopAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Loops tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing Loop Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Loops tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing Loop Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn loops better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(LoopsStep1);
                        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
                else
                {
                    if (invalidLoopAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Loops tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing Loop Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Loops tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing Loop Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn loops better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(LoopsStep1);
                        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
            }

            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(UnlockBlocks);
        }
        else
        {
            InteractedWithTable();
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void ConditionsStep1()
    {
        ConditionsTries++;
        if (CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text == "Ready")
        {
            CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ok!";
        }
        invalidCondAttempts = 0;
        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(0f, -376f, 0);
        CodeImage.transform.Find("OkButton3").gameObject.SetActive(false);
        CodeImage.transform.Find("OkButton2").gameObject.SetActive(false);
        TitleText.text = "Instructions: Conditions";
        if (SelectedLanguage == Language.CSharp)
        {
            ContentText.text = "Conditions in <color=cyan>C#</color> can be implemented in 2 different ways:\n\n";
            ContentText.text += "1. <color=cyan>If</color> statements\n\n2. <color=cyan>Switch</color> statements";
            ContentText.fontSize = 30;
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(ConditionsStep2);
        }
        else
        {
            ContentText.text = "Conditions in <color=cyan>Python</color> can only be implemented using If Statements\n\n\n\n\n\n\n\n\nUse '<color=cyan>If</color>' to specify a condition to execute a block of code\n\nUse '<color=cyan>Elif</color>' to specify a new condition if the previous conditions are false.\n\nUse '<color=cyan>Else</color>' to specify a block of code which executes if all other conditions are false\n\nNote that '<color=cyan>Else</color>' does not need a condition as it takes any other input that doesn't match the other conditions.";
            ContentText.fontSize = 23;
            CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
            CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "<color=cyan>if</color> *condition* :\n  #Code to implement\n\n<color=cyan>elif</color> *condition 2*:\n  #Code to implement\n\n<color=cyan>else</color>:\n  #Code to implement";
            CodeImage.transform.Find("CodeText").GetComponent<Text>().fontSize = 26;
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(74f, 148f, 0);
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(379f, 244f);
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(ConditionsStep12);
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void ConditionsStep12()
    {
        CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(74f, 210f, 0);
        ContentText.text = "\n\n\n\n\n\n\n\n\nNote that these statements have an order.\n\nAny if statement starts with the keyword '<color=cyan>if</color>' to signify the start of a condition\n\nAny other specific conditions after the first one would use the '<color=cyan>elif</color>' statement\n\nIf all other conditions are false and you would still like some code to execute, the '<color=cyan>else</color>' statement would be used at the end of the entire if statement.";
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep2);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }
    void ConditionsStep2()
    {
        if (SelectedLanguage == Language.CSharp)
        {
            ContentText.text = "\n\n\n\n\n\n\n\n\n\nUse '<color=cyan>If</color>' to specify a condition to execute a block of code\n\nUse '<color=cyan>Else if</color>' to specify a new condition if the previous conditions are false.\n\nUse '<color=cyan>Else</color>' to specify a block of code which executes if all other conditions are false\n\nNote that '<color=cyan>Else</color>' does not need a condition as it takes any other input that doesn't match the other conditions.";
            ContentText.fontSize = 23;
            CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
            CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "<color=cyan>if</color>(*condition*){\n  #Code to implement\n}\n\n<color=cyan>else if</color>(*condition 2*){\n  #Code to implement\n}\n\n<color=cyan>else</color>{\n  #Code to implement\n}";
            CodeImage.transform.Find("CodeText").GetComponent<Text>().fontSize = 25;
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(74f, 148f, 0);
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(379f, 337f);
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(ConditionsStep3);
        }
        else
        {
            CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";
            CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
            TitleText.text = "Quiz: If Statements";
            ContentText.text = "Complete the following if statements";
            InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[6], new Vector3(0, 0, 0), Quaternion.identity);
            InstantiatedQuestion.transform.SetParent(CodeImage.transform);
            InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckConditionsStep2);
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CheckConditionsStep2()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check4 = false;

            string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

            if (t1 == "if")
            {
                check1 = true;
            }
            if (t2 == "elif")
            {
                check2 = true;
            }
            if (t3 == "animal==\"cat\"")
            {
                check3 = true;
            }
            else
            {
                print(t3);
            }
            if (t4 == "else")
            {
                check4 = true;
            }

            if (check1 && check2 && check3 && check4)
            {
                InvalidQuestion = false;
                ConditionsStep3();
            }
            else
            {
                invalidCondAttempts++;
                StartCoroutine(InvalidConditionsStep2(check1, check2, check3, check4));
            }
        }
    }

    IEnumerator InvalidConditionsStep2(bool c1, bool c2, bool c3, bool c4)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }

        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }
    void ConditionsStep3()
    {
        if(InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        if (SelectedLanguage == Language.CSharp)
        {
            ContentText.text = "\n\n\n\n\n\n\n\n\n\nNote that these statements have an order.\n\nAny if statement starts with the keyword '<color=cyan>if</color>' to signify the start of a condition\n\nAny other specific conditions after the first one would use the '<color=cyan>else if</color>' statement\n\nIf all other conditions are false and you would still like some code to execute, the '<color=cyan>else</color>' statement would be used at the end of the entire if statement.";
            ContentText.fontSize = 22;
            CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
            CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "<color=cyan>int</color> day = 3;\n\n<color=cyan>if</color>(day > 5){\n   Console.WriteLine(\"Weekend is here\");\n}\n<color=cyan>else if</color>(day <= 5 && day > 0){\n    Console.WriteLine(\"Still a weekday\");\n}\n<color=cyan>else</color>{\n    Console.WriteLine(\"Number is not valid\");\n}";
            CodeImage.transform.Find("CodeText").GetComponent<Text>().fontSize = 26;
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(24f, 174f, 0);
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(509f, 341f);
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(LoopsStep2);
        }
        else
        {
            InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[7], new Vector3(0, 0, 0), Quaternion.identity);
            InstantiatedQuestion.transform.SetParent(CodeImage.transform);
            InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckConditionsStep3);
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CheckConditionsStep3()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check4 = false;
            bool check5 = false;

            string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

            if (t1 == "if")
            {
                check1 = true;
            }
            if (t2 == "i>0")
            {
                check2 = true;
            }
            else
            {
                print(t2);
            }
            if (t3 == "elif")
            {
                check3 = true;
            }
            else
            {
                print(t3);
            }
            if (t4 == "i>5")
            {
                check4 = true;
            }
            print(t4);
            if (t5 == "else")
            {
                check5 = true;
            }
            else
            {
                print(t5);
            }

            if (check1 && check2 && check3 && check4 && check5)
            {
                InvalidQuestion = false;
                ConditionsStep4();
            }
            else
            {
                invalidCondAttempts++;
                StartCoroutine(InvalidConditionsStep3(check1, check2, check3, check4, check5));
            }
        }
    }

    IEnumerator InvalidConditionsStep3(bool c1, bool c2, bool c3, bool c4, bool c5)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        if (!c5)
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
        }
        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }

    void ConditionsStep31()
    {
        CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";
        TitleText.text = "Quiz: If Statements";
        ContentText.text = "Complete the following if statements\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n*NOTE: when creating conditions to compare text, the condition is always \"==\" to check if the text matches.";
        InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[9], new Vector3(0, 0, 0), Quaternion.identity);
        InstantiatedQuestion.transform.SetParent(CodeImage.transform);
        InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckConditionsStep31);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CheckConditionsStep31()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check4 = false;

            string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();


            if (t1 == "if")
            {
                check1 = true;
            }
            if (t2 == "elseif")
            {
                check2 = true;
            }
            else
            {
                print(t2);
            }
            if (t3 == "animal==\"cat\"")
            {
                check3 = true;
            }
            else
            {
                print(t3);
            }
            if (t4 == "else")
            {
                check4 = true;
            }
            

            if (check1 && check2 && check3 && check4)
            {
                InvalidQuestion = false;
                ConditionsStep4();
            }
            else
            {
                invalidCondAttempts++;
                StartCoroutine(InvalidConditionsStep31(check1, check2, check3, check4));
            }
        }
    }

    IEnumerator InvalidConditionsStep31(bool c1, bool c2, bool c3, bool c4)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        
        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }

    void ConditionsStep4()
    {
        if(InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        if (SelectedLanguage == Language.CSharp)
        {
            TitleText.text = "Quiz: If Statements 2";
            CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";
            CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
            InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[10], new Vector3(0, 0, 0), Quaternion.identity);
            InstantiatedQuestion.transform.SetParent(CodeImage.transform);
            InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 46f, 0);
            InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1f);
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckConditionsStep4);
        }
        else
        {
            TitleText.text = "Instructions: If Statements";
            ContentText.text = "Another keyword used in situations where no code is to be executed in an if statement, the keyword '<color=cyan>pass</color>' is used to skip the condition entirely\n\n\n\n\n\n\nOnly use the '<color=cyan>pass</color>' keyword when you know for sure no code will be executed in a condition. If you don't use it and keep a condition's code empty, it will cause an error.";
            CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
            CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "<color=cyan>if</color> *condition*:\n    *code to execute*\n<color=cyan>elif</color> *condition*:\n    *code to execute*\n<color=cyan>else</color>:\n    <color=cyan>pass</color>\n\n\n\n\n\n\n\n\ni=5\n\n<color=cyan>if</color> i == 1:\n    <color=cyan>print</color>(\"1\")\n<color=cyan>elif</color> i == 2:\n    <color=cyan>print</color>(\"2\")\n<color=cyan>else</color>:\n    <color=cyan>pass</color>";
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(379f, 577.9f);
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(128f, -76f, 0);
            CodeImage.transform.Find("CodeText").GetComponent<Text>().fontSize = 23;
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(ConditionsStep5);
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CheckConditionsStep4()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check4 = false;
            bool check5 = false;
            bool check6 = false;
            bool check7 = false;
            

            string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t6 = InstantiatedQuestion.transform.Find("Input6").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t7 = InstantiatedQuestion.transform.Find("Input7").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();


            if (t1 == "if")
            {
                check1 = true;
            }
            if (t2 == "i>0")
            {
                check2 = true;
            }
            else
            {
                print(t2);
            }
            if (t3 == "elseif")
            {
                check3 = true;
            }
            else
            {
                print(t3);
            }
            if (t4 == "i>5")
            {
                check4 = true;
            }
            if(t5 == "elseif")
            {
                check5 = true;
            }
            if(t6 == "i==10")
            {
                check6 = true;
            }
            if(t7 == "else")
            {
                check7 = true;
            }


            if (check1 && check2 && check3 && check4 && check5 && check6 && check7)
            {
                InvalidQuestion = false;
                ConditionsStep5();
            }
            else
            {
                invalidCondAttempts++;
                StartCoroutine(InvalidConditionsStep4(check1, check2, check3, check4, check5, check6, check7));
            }
        }
    }
    IEnumerator InvalidConditionsStep4(bool c1, bool c2, bool c3, bool c4, bool c5, bool c6, bool c7)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        if (!c5)
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
        }
        if (!c6)
        {
            InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.green;
        }
        if (!c7)
        {
            InstantiatedQuestion.transform.Find("Input7").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input7").GetComponent<Image>().color = Color.green;
        }

        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input7").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }
    void ConditionsStep5()
    {
        if (SelectedLanguage == Language.CSharp)
        {
            if (InstantiatedQuestion != null)
            {
                Destroy(InstantiatedQuestion);
            }
            ContentText.fontSize = 22;
            TitleText.text = "Instructions: Switch Statements";
            ContentText.text = "Well done!\n\nNow we will move onto <color=cyan>Switch</color> statements.\n\n\n\n\n\n\n\n\nUse the <color=cyan>Switch</color> statement to select one of many code blocks to be executed\n\n<color=cyan>Switch</color> expression is evaluated once.\n\nConditions are examined through '<color=cyan>cases</color>'\n\nThe value in the expression (key) is compared with the values of each case\n}. ";
            CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "<color=cyan>switch</color>(key){\n   <color=cyan>case</color> <color=cyan>*condition*</color>:\n    *Code To Execute*\n    <color=cyan>break;</color>\n   <color=cyan>case</color> <color=cyan>*condition*</color>:\n    *Code To Execute*\n    <color=cyan>break;</color>\n}";
            CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(150f, 79f, 0);
            CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(ConditionsStep6);
        }
        else
        {
            TitleText.text = "Quiz: If Statements";
            ContentText.text = "Complete this If statement";
            CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";
            CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
            InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[8], new Vector3(0, 0, 0), Quaternion.identity);
            InstantiatedQuestion.transform.SetParent(CodeImage.transform);
            InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckConditionsStep5);
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CheckConditionsStep5()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check4 = false;
            bool check5 = false;
            bool check6 = false;

            string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t6 = InstantiatedQuestion.transform.Find("Input6").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

            if (t1 == "if")
            {
                check1 = true;
            }
            if (t2 == "i==1")
            {
                check2 = true;
            }
            else
            {
                print(t2);
            }
            if (t3 == "elif")
            {
                check3 = true;
            }
            else
            {
                print(t3);
            }
            if (t4 == "i==2")
            {
                check4 = true;
            }
            if (t5 == "else")
            {
                check5 = true;
            }
            else
            {
                print(t5);
            }
            if (t6 == "pass")
            {
                check6 = true;
            }
            else
            {
                print(t6);
            }

            if (check1 && check2 && check3 && check4 && check5 && check6)
            {
                InvalidQuestion = false;
                ConditionsFinal();
            }
            else
            {
                invalidCondAttempts++;
                StartCoroutine(InvalidConditionsStep5(check1, check2, check3, check4, check5, check6));
            }

        }
    }

    IEnumerator InvalidConditionsStep5(bool c1, bool c2, bool c3, bool c4, bool c5, bool c6)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        if (!c5)
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
        }
        if (!c6)
        {
            InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.green;
        }
        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }

    void ConditionsStep6()
    {
        TitleText.text = "Quiz: Switch Statements";
        ContentText.text = "Complete the following switch statement";
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";
        CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
        InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[11], new Vector3(0, 0, 0), Quaternion.identity);
        InstantiatedQuestion.transform.SetParent(CodeImage.transform);
        InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckConditionsStep6);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CheckConditionsStep6()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check4 = false;
            bool check5 = false;

            string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

            if (t1 == "key")
            {
                check1 = true;
            }
            if (t2 == "case")
            {
                check2 = true;
            }
            else
            {
                print(t2);
            }
            if (t3 == "break" || t3 == "break;")
            {
                check3 = true;
            }
            else
            {
                print(t3);
            }
            if (t4 == "case")
            {
                check4 = true;
            }
            print(t4);
            if (t5 == "break" || t5 == "break;")
            {
                check5 = true;
            }
            else
            {
                print(t5);
            }
           
            if (check1 && check2 && check3 && check4 && check5)
            {
                InvalidQuestion = false;
                ConditionsStep7();
            }
            else
            {
                invalidCondAttempts++;
                StartCoroutine(InvalidConditionsStep6(check1, check2, check3, check4, check5));
            }

        }
    }

    IEnumerator InvalidConditionsStep6(bool c1, bool c2, bool c3, bool c4, bool c5)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        if (!c5)
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
        }
       
        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }

    void ConditionsStep7()
    {
        if(InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[12], new Vector3(0, 0, 0), Quaternion.identity);
        InstantiatedQuestion.transform.SetParent(CodeImage.transform);
        InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckConditionsStep7);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CheckConditionsStep7()
    {
        if (!InvalidQuestion)
        {
            InvalidQuestion = true;
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check4 = false;
            bool check5 = false;

            string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
            string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

            if (t1 == "number")
            {
                check1 = true;
            }
            if (t2 == "case")
            {
                check2 = true;
            }
            else
            {
                print(t2);
            }
            if (t3 == "break" || t3 == "break;")
            {
                check3 = true;
            }
            else
            {
                print(t3);
            }
            if (t4 == "case")
            {
                check4 = true;
            }
            print(t4);
            if (t5 == "break" || t5 == "break;")
            {
                check5 = true;
            }
            else
            {
                print(t5);
            }

            if (check1 && check2 && check3 && check4 && check5)
            {
                InvalidQuestion = false;
                ConditionsStep8();
            }
            else
            {
                invalidCondAttempts++;
                StartCoroutine(InvalidConditionsStep7(check1, check2, check3, check4, check5));
            }

        }
    }

    void ConditionsStep8()
    {
        if (InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        TitleText.text = "Instructions: Switch Statements";
        ContentText.text = "Well done!\n\nOne final keyword to take note of when utilizing switch statements is '<color=cyan>default</color>'.\n\n It is used to specify code which will execute if the values of all other cases do not match the value within the switch statement";
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(TestConditionsStep8);

        CodeImage.transform.Find("CodeText").gameObject.SetActive(true);
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "<color=cyan>string</color> vehicle = \"Car\";\n\n<color=cyan>switch</color>(vehicle){\n\n   <color=cyan>case</color> \"Motorbike\":\n              *Code To Execute*\n              break;\n   <color=cyan>case</color> \"Plane\":\n              *Code To Execute*\n              break;\n   <color=cyan>default</color>:\n              Console.WriteLine(\"Unknown\");\n}";
        CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().localPosition = new Vector3(150f, -139f,0);
        CodeImage.transform.Find("CodeText").GetComponent<RectTransform>().sizeDelta = new Vector2(509f, 359f);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void TestConditionsStep8()
    {
        CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
        CodeImage.transform.Find("CodeText").GetComponent<Text>().text = "";

        TitleText.text = "Quiz: Switch Statements";
        ContentText.text = "Complete the following switch statement";
        InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().TutorialQuestions[13], new Vector3(0, 0, 0), Quaternion.identity);
        InstantiatedQuestion.transform.SetParent(CodeImage.transform);
        InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CheckConditionsStep8);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CheckConditionsStep8()
    {
        bool check1 = false;
        bool check2 = false;
        bool check3 = false;
        bool check4 = false;

        string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
        string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
        string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
        string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
      

        if (t1 == "animal")
        {
            check1 = true;
        }
        if (t2 == "case")
        {
            check2 = true;
        }
        else
        {
            print(t2);
        }
        if (t3 == "break" || t3 == "break;")
        {
            check3 = true;
        }
        else
        {
            print(t3);
        }
        if (t4 == "default")
        {
            check4 = true;
        }
        
        if(check1 && check2 && check3 && check4)
        {
            InvalidQuestion = false;
            ConditionsFinal();
        }
        else
        {
            invalidCondAttempts++;
            StartCoroutine(InvalidConditionsStep8(check1, check2, check3, check4));
        }
    }

    IEnumerator InvalidConditionsStep8(bool c1, bool c2, bool c3, bool c4)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }

    IEnumerator InvalidConditionsStep7(bool c1, bool c2, bool c3, bool c4, bool c5)
    {
        if (!c1)
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
        }
        if (!c2)
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
        }
        if (!c3)
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
        }
        if (!c4)
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
        }
        if (!c5)
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
        }
        else
        {
            InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
        }

        yield return new WaitForSeconds(1f);
        InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
        InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
        InvalidQuestion = false;
    }

    void ConditionsFinal()
    {
        if (InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        if (CodeImage.transform.Find("OkButton").gameObject.activeSelf == false)
        {
            CodeImage.transform.Find("OkButton").gameObject.SetActive(true);
        }
        TitleText.text = "Completed Conditions";
        if (TutorialMode)
        {
            if (completedconcepts < 2)
            {
                if (SelectedLanguage == Language.CSharp)
                {
                    if (invalidCondAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Conditions tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing Conditions Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Conditions tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing Conditions Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn conditions better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(ConditionsStep1);
                        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
                else
                {
                    if (invalidCondAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Conditions tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing Conditions Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Conditions tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 4 new blocks you can retrieve by completing Conditions Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn conditions better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(ConditionsStep1);
                        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
            }
            else
            {
                if (SelectedLanguage == Language.CSharp)
                {
                    if (invalidCondAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Conditions tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing Conditions Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Conditions tutorial for the <color=cyan>C#</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing Conditions Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn conditions better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(ConditionsStep1);
                        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
                else
                {
                    if (invalidCondAttempts < 3)
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Conditions tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing Conditions Objectives!";
                    }
                    else
                    {
                        ContentText.text = "Well done!\n\nYou've completed the Conditions tutorial for the <color=cyan>Python</color> Language!\n\nYou have now <color=yellow>unlocked</color> 8 new blocks you can retrieve by completing Conditions Objectives!\n\nHowever, recent results show you struggled to answer this tutorial's questions correctly. It's highly suggested to retry the tutorial to learn conditions better.";
                        CodeImage.transform.Find("OkButton3").gameObject.SetActive(true);
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.RemoveAllListeners();
                        CodeImage.transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(ConditionsStep1);
                        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton3").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(-255f, -376f, 0);
                    }
                }
            }

            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(UnlockBlocks);
        }
        else
        {
            if(ThisTableType == BlockAccessType.DataTypes)
            {
                foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
                {
                    if (temp != this.gameObject)
                    {
                        temp.GetComponent<BlockAccessTable>().CompletedDataTypes = true;
                        temp.GetComponent<BlockAccessTable>().DataTypeTries = DataTypeTries;
                    }

                }
            }
            else if (ThisTableType == BlockAccessType.Loops)
            {
                foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
                {
                    if (temp != this.gameObject)
                    {
                        temp.GetComponent<BlockAccessTable>().CompletedDataTypes = true;
                        temp.GetComponent<BlockAccessTable>().LoopsTries = LoopsTries;
                    }

                }
            }
            else if (ThisTableType == BlockAccessType.Conditions)
            {
                foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
                {
                    if (temp != this.gameObject)
                    {
                        temp.GetComponent<BlockAccessTable>().CompletedDataTypes = true;
                        temp.GetComponent<BlockAccessTable>().ConditionsTries = ConditionsTries;
                    }

                }
            }
            InteractedWithTable();
        }
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }
    void UnlockBlocks()
    {
        if (thisTableMarker != null) {
            Destroy(thisTableMarker);
        }
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        
        TutorialMode = false;
        Close();

        if (completedconcepts == 0)
        {
            UnlockBlocks1 = true;
            StartCoroutine(BlocksUnlocked(1));

            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
            {
                if (temp != this.gameObject)
                {
                    temp.GetComponent<BlockAccessTable>().UnlockBlocks1 = true;
                }
            }
        }
        else if(completedconcepts == 1)
        {
            UnlockBlocks2 = true;
            StartCoroutine(BlocksUnlocked(2));

            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
            {
                if (temp != this.gameObject)
                {
                    temp.GetComponent<BlockAccessTable>().UnlockBlocks2 = true;
                }
            }
        }
        else if(completedconcepts == 2)
        {
            UnlockBlocks3 = true;
            StartCoroutine(BlocksUnlocked(3));

            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
            {
                if (temp != this.gameObject)
                {
                    temp.GetComponent<BlockAccessTable>().UnlockBlocks3 = true;
                }
            }
        }

        if(ThisTableType == BlockAccessType.DataTypes)
        {
            CompletedDataTypes = true;
            if (completedconcepts == 0)
            {
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.DataTypes;
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().CompletedDataTypes = true;
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().TutorialMode = false;
            }
            else
            {
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().CompletedDataTypes = true;
            }

            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
            {
                if (temp != this.gameObject)
                {
                    temp.GetComponent<BlockAccessTable>().CompletedDataTypes = true;
                    temp.GetComponent<BlockAccessTable>().DataTypeTries = DataTypeTries;
                }

            }
        }
        else if(ThisTableType == BlockAccessType.Loops)
        {
            CompletedLoops = true;
            if (completedconcepts == 0)
            {
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.Loops;
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().CompletedLoops = true;
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().TutorialMode = false;
            }
            else
            {
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().CompletedLoops = true;
            }
            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
            {
                if (temp != this.gameObject)
                {
                    temp.GetComponent<BlockAccessTable>().CompletedLoops = true;
                    temp.GetComponent<BlockAccessTable>().LoopsTries = LoopsTries;
                }
            }
        }
        else if(ThisTableType == BlockAccessType.Conditions)
        {
            CompletedConditions = true;
            if (completedconcepts == 0)
            {
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.Conditions;
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().CompletedConditions = true;
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().TutorialMode = false;
            }
            else
            {
                GameObject.Find("SpawnBat").GetComponent<BlockAccessTable>().CompletedConditions = true;
            }
            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
            {
                if (temp != this.gameObject)
                {
                    temp.GetComponent<BlockAccessTable>().CompletedConditions = true;
                    temp.GetComponent<BlockAccessTable>().ConditionsTries = ConditionsTries;
                }
            }
        }
        completedconcepts++;

        foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
        {
            if (temp != this.gameObject)
            {
                temp.GetComponent<BlockAccessTable>().completedconcepts = completedconcepts;
            }
        }
    }

    IEnumerator BlocksUnlocked(int Step)
    {

        UpdateAchTextColor(255);
        GameObject AchievementImage = GameObject.Find("Canvas").transform.Find("AchievementImage").gameObject;
        GameObject Images = AchievementImage.transform.Find("Images").gameObject;
        Text TitleText = AchievementImage.transform.Find("AchText (1)").GetComponent<Text>();
        PlayerIO pi = GameObject.Find("player").GetComponent<PlayerIO>();

        if (Step == 1) {

            byte[] image = new byte[] { 1, 2, 3, 5 };
            int byteindex = 0;
            foreach (Transform t in Images.transform)
            {
                t.gameObject.GetComponent<Image>().sprite = pi.spritesByBlockID[image[byteindex] - 1];
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                byteindex++;
            }
        }
        else if(Step == 2)
        {
            TitleText.text = "<color=yellow>Achievement Unlocked</color>: Getting there!";
            byte[] image = new byte[] { 6, 7, 28, 23 };
            int byteindex = 0;
            foreach (Transform t in Images.transform)
            {
                t.gameObject.GetComponent<Image>().sprite = pi.spritesByBlockID[image[byteindex] - 1];
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                byteindex++;
            }
        }
        else if(Step == 3 && !CompletedTutorial)
        {
            TitleText.text = "<color=yellow>Achievement Unlocked</color>: Concepts completed!";

            AchievementImage.GetComponent<RectTransform>().sizeDelta = new Vector2(675f, 383f);
            AchievementImage.transform.Find("AchText").GetComponent<RectTransform>().localPosition = new Vector3(0, -45f, 0);
            Images.GetComponent<RectTransform>().localPosition = new Vector3(0, 29f, 0);
            AchievementImage.transform.Find("Images2").gameObject.SetActive(true);
            AchievementImage.transform.Find("Images2").GetComponent<RectTransform>().localPosition = new Vector3(0, -71f, 0);

            byte[] image = new byte[] {24,26,20,21,19,17,13,15,34};
            int byteindex = 0;
            foreach (Transform t in Images.transform)
            {
                t.gameObject.GetComponent<Image>().sprite = pi.spritesByBlockID[image[byteindex] - 1];
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                byteindex++;
            }
            foreach(Transform t in AchievementImage.transform.Find("Images2"))
            {
                t.gameObject.GetComponent<Image>().sprite = pi.spritesByBlockID[image[byteindex] - 1];
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                byteindex++;
            }
        }
        else if(Step == 3 && CompletedTutorial)
        {
            AchievementImage.GetComponent<RectTransform>().sizeDelta = new Vector2(1083f, 684f);
            AchievementImage.transform.Find("AchText (1)").GetComponent<RectTransform>().localPosition = new Vector3(0, 312f, 0);
            AchievementImage.transform.Find("AchText").GetComponent<RectTransform>().localPosition = new Vector3(0, -212f, 0);
            Images.GetComponent<RectTransform>().localPosition = new Vector3(0,140f, 0);
            AchievementImage.transform.Find("Images2").gameObject.SetActive(true);
            AchievementImage.transform.Find("Images2").GetComponent<RectTransform>().localPosition = new Vector3(0, 40f, 0);
            AchievementImage.transform.Find("Images3").gameObject.SetActive(true);
            AchievementImage.transform.Find("Images3").GetComponent<RectTransform>().localPosition = new Vector3(0, -60f, 0);
            AchievementImage.transform.Find("Images4").gameObject.SetActive(true);
            AchievementImage.transform.Find("Images4").GetComponent<RectTransform>().localPosition = new Vector3(0, -160f, 0);

            byte[] image = new byte[] { 1, 2, 3, 5, 6, 7, 28, 23, 24, 26, 20, 21, 19, 17, 13, 15};
            int byteindex = 0;
            foreach (Transform t in Images.transform)
            {
                t.gameObject.GetComponent<Image>().sprite = pi.spritesByBlockID[image[byteindex] - 1];
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                byteindex++;
            }
            foreach (Transform t in AchievementImage.transform.Find("Images2"))
            {
                t.gameObject.GetComponent<Image>().sprite = pi.spritesByBlockID[image[byteindex] - 1];
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                byteindex++;
            }
            foreach (Transform t in AchievementImage.transform.Find("Images3"))
            {
                t.gameObject.GetComponent<Image>().sprite = pi.spritesByBlockID[image[byteindex] - 1];
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                byteindex++;
            }
            foreach (Transform t in AchievementImage.transform.Find("Images4"))
            {
                t.gameObject.GetComponent<Image>().sprite = pi.spritesByBlockID[image[byteindex] - 1];
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                byteindex++;
            }
        }

        GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("explode1");
        AchievementImage.SetActive(true);
        AchievementImage.GetComponent<Image>().color = new Color32(15, 15, 15, 0);
        yield return new WaitForSeconds(0.1f);
        AchievementImage.GetComponent<Image>().color = new Color32(15, 15, 15, 50);
        yield return new WaitForSeconds(0.1f);
        AchievementImage.GetComponent<Image>().color = new Color32(15, 15, 15, 100);
        yield return new WaitForSeconds(0.1f);
        AchievementImage.GetComponent<Image>().color = new Color32(15, 15, 15, 150);
        yield return new WaitForSeconds(0.1f);
        AchievementImage.GetComponent<Image>().color = new Color32(15, 15, 15, 188);
        yield return new WaitForSeconds(1f);
        UpdateImagesColor(15);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(30);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(45);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(60);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(75);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(90);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(105);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(120);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(135);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(150);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(165);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(180);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(195);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(210);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(225);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(240);
        yield return new WaitForSeconds(0.1f);
        UpdateImagesColor(255);
        yield return new WaitForSeconds(4f);

        AchievementImage.GetComponent<Image>().color = new Color32(15, 15, 15, 188);
        UpdateImagesColor(188);
        UpdateAchTextColor(188);
        yield return new WaitForSeconds(0.1f);
        AchievementImage.GetComponent<Image>().color = new Color32(15, 15, 15, 150);
        UpdateImagesColor(150);
        UpdateAchTextColor(150);
        yield return new WaitForSeconds(0.1f);
        AchievementImage.GetComponent<Image>().color = new Color32(15, 15, 15, 100);
        UpdateImagesColor(100);
        UpdateAchTextColor(100);
        yield return new WaitForSeconds(0.1f);
        AchievementImage.GetComponent<Image>().color = new Color32(15, 15, 15, 50);
        UpdateImagesColor(50);
        UpdateAchTextColor(50);
        yield return new WaitForSeconds(0.1f);
        AchievementImage.GetComponent<Image>().color = new Color32(15, 15, 15, 0);
        UpdateImagesColor(0);
        UpdateAchTextColor(0);
        AchievementImage.SetActive(false);

        if(Step == 3 && !CompletedTutorial)
        {
            CompletedAllConcepts();
        }
    }

    void UpdateImagesColor(byte a)
    {
        GameObject AchievementImage = GameObject.Find("Canvas").transform.Find("AchievementImage").gameObject;
        GameObject Images = AchievementImage.transform.Find("Images").gameObject;
        foreach (Transform t in Images.transform)
        {
            t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, a);
        }

        if (completedconcepts == 3 && !CompletedTutorial)
        {
            GameObject img2 = AchievementImage.transform.Find("Images2").gameObject;

            foreach (Transform t in img2.transform)
            {
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, a);
            }
        }
        else if(completedconcepts == 3 && CompletedTutorial)
        {
            GameObject img2 = AchievementImage.transform.Find("Images2").gameObject;

            foreach (Transform t in img2.transform)
            {
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, a);
            }
            GameObject img3 = AchievementImage.transform.Find("Images3").gameObject;

            foreach (Transform t in img3.transform)
            {
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, a);
            }
            GameObject img4 = AchievementImage.transform.Find("Images4").gameObject;

            foreach (Transform t in img4.transform)
            {
                t.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, a);
            }
        }
    }

    /*private void OnApplicationQuit()
    {
        if(GameObject.Find("World").GetComponent<World>().WorldName == "Tutorial")
        {
            GameObject.Find("GameManager").GetComponent<DataToHold>().SaveUnFinTutorial();
        }
    }*/

    void UpdateAchTextColor(byte a)
    {
        GameObject AchievementImage = GameObject.Find("Canvas").transform.Find("AchievementImage").gameObject;
        Text ach1 = AchievementImage.transform.Find("AchText (1)").GetComponent<Text>();
        TMP_Text ach2 = AchievementImage.transform.Find("AchText").GetComponent<TMP_Text>();

        ach1.color = new Color32(255, 255, 255, a);
        ach2.color = new Color32(255, 255, 255, a);
    }

    public void CompletedAllConcepts()
    {
        if (!Saving)
        {
            Saving = true;
            GameObject.Find("Canvas").transform.Find("crosshair").gameObject.SetActive(false);
            CodeImage.SetActive(true);
            GameObject.Find("player").GetComponent<PlayerController>().controlsEnabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;


            TitleText.text = "Tutorial Completed";
            ContentText.text = "Well done!\n\nYou completed all the concepts in the tutorial!\n\nYou can either continue building in the tutorial and practise the objectives or exit and try the <color=cyan>Singleplayer</color>/<color=cyan>Multiplayer</color> modes!\n\n<color=cyan>Singleplayer</color> will be similar the tutorial but with a timer.\n<color=cyan>Multiplayer</color> will also be similar to the tutorial, but you will compete with another player for the shortest time and least incorrect attempts.";

            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(Completion);
            GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            CompletedTutorial = true;
            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("B.A.T"))
            {
                if (temp != this.gameObject)
                {
                    temp.GetComponent<BlockAccessTable>().CompletedTutorial = true;
                }
            }

            GameObject.Find("TutorialTimer").GetComponent<TutorialTimer>().StartTimer = false;

            GameObject.Find("GameManager").GetComponent<DataToHold>().SaveTutorial( DataTypeTries, LoopsTries, ConditionsTries);
        }
    }

    void Completion()
    {
        Close();
        StartCoroutine(BlocksUnlocked(3));
        Saving = false;
    }

    void InitializeBlockAchiever()
    {
        if (CodeImage.transform.Find("CodeText").gameObject.activeSelf)
        {
            CodeImage.transform.Find("CodeText").gameObject.SetActive(false);
        }
        if(GameObject.Find("World").GetComponent<World>().WorldName == "Tutorial" && CodeImage.transform.Find("OkButton3").gameObject.activeSelf == true)
        {
            CodeImage.transform.Find("OkButton3").gameObject.SetActive(false);
        }
        TitleText.text = "Code Quiz";
        ContentText.text = "";
        CodeImage.transform.Find("OkButton2").gameObject.SetActive(false);
        CodeImage.transform.Find("OkButton").GetComponent<RectTransform>().localPosition = new Vector3(0, -376f, 0);
        CodeImage.transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Submit";
        UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;

        if (SelectedLanguage == Language.CSharp)
        {
            int rand = 999;
            if (CompletedDataTypes && CompletedLoops && CompletedConditions)
            {
                print("All Concepts Available");
                rand = UnityEngine.Random.Range(0, GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions.Count);
                /*if(rand == LastQuestionEntered)
                {
                    rand = UnityEngine.Random.Range(0, GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions.Count);
                }*/
                bool found = false;
                foreach(int i in RecentQuestionsAnswered)
                {
                    if(rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(0, GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions.Count);

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if(rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (CompletedDataTypes && CompletedLoops && !CompletedConditions)
            {
                print("DataTypes and Loops Available");
                rand = UnityEngine.Random.Range(0, 10);

                if (rand > 7)
                {
                    rand += 5;
                }

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(0, 10);
                        if (rand > 7)
                        {
                            rand += 5;
                        }
                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }

            }
            else if (CompletedDataTypes && CompletedConditions && !CompletedLoops)
            {
                print("DataTypes and Conditions Available");
                rand = UnityEngine.Random.Range(0, 10);
                if (rand > 4)
                {
                    rand += 3;
                }

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(0, 10);

                        if (rand > 4)
                        {
                            rand += 3;
                        }
                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (CompletedConditions && CompletedLoops && !CompletedDataTypes)
            {
                print("Conditions and Loops Available");
                rand = UnityEngine.Random.Range(5, GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions.Count);

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(5, GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions.Count);

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (CompletedDataTypes && !CompletedLoops && !CompletedConditions)
            {
                print("DataTypes available");
                rand = UnityEngine.Random.Range(0, 5);

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(0, 5);

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (!CompletedDataTypes && CompletedLoops && !CompletedConditions)
            {
                print("Loops available");
                rand = UnityEngine.Random.Range(5, 10);
                if (rand > 7)
                {
                    rand += 5;
                }

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(5, 10);
                        if (rand > 7)
                        {
                            rand += 5;
                        }

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (!CompletedDataTypes && !CompletedLoops && CompletedConditions)
            {
                print("Conditions available");
                rand = UnityEngine.Random.Range(8, 13);

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(8, 13);

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }

            if (rand != 999)
            {
                if (RecentQuestionsAnswered.Count == 3)
                {
                    int t1 = RecentQuestionsAnswered[0];
                    int t2 = RecentQuestionsAnswered[1];

                    RecentQuestionsAnswered[0] = rand;
                    RecentQuestionsAnswered[1] = t1;
                    RecentQuestionsAnswered[2] = t2;
                }
                else if(RecentQuestionsAnswered.Count == 2)
                {
                    RecentQuestionsAnswered.Add(rand);
                    int t1 = RecentQuestionsAnswered[0];
                    int t2 = RecentQuestionsAnswered[1];
                    int t3 = RecentQuestionsAnswered[2];
                    RecentQuestionsAnswered[0] = t3;
                    RecentQuestionsAnswered[1] = t1;
                    RecentQuestionsAnswered[2] = t2;
                }
                else if (RecentQuestionsAnswered.Count == 1)
                {
                    RecentQuestionsAnswered.Add(rand);
                    int t1 = RecentQuestionsAnswered[0];
                    int t2 = RecentQuestionsAnswered[1];
                    RecentQuestionsAnswered[0] = t2;
                    RecentQuestionsAnswered[1] = t1;
                }
                else
                {
                    RecentQuestionsAnswered.Add(rand);
                }
                List<GameObject> tables = GameObject.Find("World").GetComponent<World>().TablesSpawned;
                print("Found tables " + tables.Count);
                foreach (GameObject temp in tables)
                {
                    if (temp != this.gameObject)
                    {
                        temp.GetComponent<BlockAccessTable>().RecentQuestionsAnswered.Clear();

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            temp.GetComponent<BlockAccessTable>().RecentQuestionsAnswered.Add(i);
                        }
                    }

                }

                CodeImage.transform.Find("Line").gameObject.SetActive(false);
                InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions[rand], new Vector3(0, 0, 0), Quaternion.identity);
                InstantiatedQuestion.transform.SetParent(CodeImage.transform);
                InstantiatedQuestion.transform.SetAsFirstSibling();
                ContentText.gameObject.transform.SetAsFirstSibling();
                TitleText.gameObject.transform.SetAsFirstSibling();
                InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.1f, 1f);
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
                objectivename = InstantiatedQuestion.name;
                if (rand != 2 && rand != 3 && rand != 4)
                {
                    CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { CheckBlockQuestion(InstantiatedQuestion.GetComponent<TutorialQuizQuestion>().QuizQuestionNum); });
                    GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                }
                else
                {
                    CheckBlockQuestionButtons(InstantiatedQuestion.GetComponent<TutorialQuizQuestion>().QuizQuestionNum);
                }
            }
        }
        else if(SelectedLanguage == Language.Python)
        {
            int rand = 999;
            if (CompletedDataTypes && CompletedLoops && CompletedConditions)
            {
                rand = UnityEngine.Random.Range(0, GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions.Count);

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(0, GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions.Count);

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (CompletedDataTypes && CompletedLoops && !CompletedConditions)
            {
                rand = UnityEngine.Random.Range(0, 9);

                if (rand > 6)
                {
                    rand += 3;
                }

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(0, 9);

                        if (rand > 6)
                        {
                            rand += 3;
                        }

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (CompletedDataTypes && CompletedConditions && !CompletedLoops)
            {
                rand = UnityEngine.Random.Range(0, 8);
                if (rand > 4)
                {
                    rand += 2;
                }

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(0, 8);
                        if (rand > 4)
                        {
                            rand += 2;
                        }

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (CompletedConditions && CompletedLoops && !CompletedDataTypes)
            {
                rand = UnityEngine.Random.Range(5, GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions.Count);

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(5, GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions.Count);

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (CompletedDataTypes && !CompletedLoops && !CompletedConditions)
            {
                rand = UnityEngine.Random.Range(0, 5);

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(0, 5);

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (!CompletedDataTypes && CompletedLoops && !CompletedConditions)
            {
                rand = UnityEngine.Random.Range(5, 9);
                if (rand > 6)
                {
                    rand += 3;
                }

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(5, 9);
                        if (rand > 6)
                        {
                            rand += 3;
                        }

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }
            else if (!CompletedDataTypes && !CompletedLoops && CompletedConditions)
            {
                rand = UnityEngine.Random.Range(7, 10);

                bool found = false;
                foreach (int i in RecentQuestionsAnswered)
                {
                    if (rand == i)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    bool unfound = false;
                    while (!unfound)
                    {
                        bool checkforsamequestion = false;
                        rand = UnityEngine.Random.Range(7, 10);

                        foreach (int i in RecentQuestionsAnswered)
                        {
                            if (rand == i)
                            {
                                checkforsamequestion = true;
                            }
                        }

                        if (!checkforsamequestion)
                        {
                            unfound = true;
                        }
                    }
                }
            }

            if (rand != 999)
            {
                if (RecentQuestionsAnswered.Count == 3)
                {
                    int t1 = RecentQuestionsAnswered[0];
                    int t2 = RecentQuestionsAnswered[1];

                    RecentQuestionsAnswered[0] = rand;
                    RecentQuestionsAnswered[1] = t1;
                    RecentQuestionsAnswered[2] = t2;
                }
                else if (RecentQuestionsAnswered.Count == 2)
                {
                    RecentQuestionsAnswered.Add(rand);
                    int t1 = RecentQuestionsAnswered[0];
                    int t2 = RecentQuestionsAnswered[1];
                    int t3 = RecentQuestionsAnswered[2];
                    RecentQuestionsAnswered[0] = t3;
                    RecentQuestionsAnswered[1] = t1;
                    RecentQuestionsAnswered[2] = t2;
                }
                else if (RecentQuestionsAnswered.Count == 1)
                {
                    RecentQuestionsAnswered.Add(rand);
                    int t1 = RecentQuestionsAnswered[0];
                    int t2 = RecentQuestionsAnswered[1];
                    RecentQuestionsAnswered[0] = t2;
                    RecentQuestionsAnswered[1] = t1;
                }
                else
                {
                    RecentQuestionsAnswered.Add(rand);
                }
                List<GameObject> tables = GameObject.Find("World").GetComponent<World>().TablesSpawned;
                print("Found tables " + tables.Count);
                foreach (GameObject temp in tables)
                {
                    if (temp != this.gameObject)
                    {
                        temp.GetComponent<BlockAccessTable>().RecentQuestionsAnswered.Clear();

                        foreach(int i in RecentQuestionsAnswered)
                        {
                            temp.GetComponent<BlockAccessTable>().RecentQuestionsAnswered.Add(i);
                        }
                    }

                }
                print(rand);
                CodeImage.transform.Find("Line").gameObject.SetActive(false);
                InstantiatedQuestion = Instantiate(GameObject.Find("GameManager").GetComponent<DataToHold>().QuizQuestions[rand], new Vector3(0, 0, 0), Quaternion.identity);
                InstantiatedQuestion.transform.SetParent(CodeImage.transform);
                InstantiatedQuestion.transform.SetAsFirstSibling();
                ContentText.gameObject.transform.SetAsFirstSibling();
                TitleText.gameObject.transform.SetAsFirstSibling();
                InstantiatedQuestion.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                InstantiatedQuestion.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.1f, 1f);
                CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
                objectivename = InstantiatedQuestion.name;
                if (rand != 2 && rand != 3 && rand != 4)
                {
                    CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { CheckBlockQuestion(InstantiatedQuestion.GetComponent<TutorialQuizQuestion>().QuizQuestionNum); });
                }
                else
                {
                    CheckBlockQuestionButtons(InstantiatedQuestion.GetComponent<TutorialQuizQuestion>().QuizQuestionNum);
                }
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            }
        }
    }

    void CheckBlockQuestionButtons(int question)
    {
        if(SelectedLanguage == Language.CSharp)
        {
            if(question == 2)
            {
                Button btn1 = InstantiatedQuestion.transform.Find("Button1").GetComponent<Button>();
                Button btn2 = InstantiatedQuestion.transform.Find("Button2").GetComponent<Button>();
                Button btn3 = InstantiatedQuestion.transform.Find("Button3").GetComponent<Button>();
                Button btn4 = InstantiatedQuestion.transform.Find("Button4").GetComponent<Button>();

                btn1.onClick.AddListener(delegate { CorrectButtonClick(); });
                btn2.onClick.AddListener(delegate { InvalidButtonClick(1); });
                btn3.onClick.AddListener(delegate { InvalidButtonClick(2); });
                btn4.onClick.AddListener(delegate { InvalidButtonClick(3); });

                btn1.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn2.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn3.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn4.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            }
            else if(question == 3)
            {
                Button btn1 = InstantiatedQuestion.transform.Find("Button1").GetComponent<Button>();
                Button btn2 = InstantiatedQuestion.transform.Find("Button2").GetComponent<Button>();
                Button btn3 = InstantiatedQuestion.transform.Find("Button3").GetComponent<Button>();
                Button btn4 = InstantiatedQuestion.transform.Find("Button4").GetComponent<Button>();

                btn1.onClick.AddListener(delegate { InvalidButtonClick(0); });
                btn2.onClick.AddListener(delegate { CorrectButtonClick(); });
                btn3.onClick.AddListener(delegate { InvalidButtonClick(2); });
                btn4.onClick.AddListener(delegate { InvalidButtonClick(3); });

                btn1.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn2.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn3.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn4.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            }
            else if(question == 4)
            {
                Button btn1 = InstantiatedQuestion.transform.Find("Button1").GetComponent<Button>();
                Button btn2 = InstantiatedQuestion.transform.Find("Button2").GetComponent<Button>();
                Button btn3 = InstantiatedQuestion.transform.Find("Button3").GetComponent<Button>();
                Button btn4 = InstantiatedQuestion.transform.Find("Button4").GetComponent<Button>();

                btn1.onClick.AddListener(delegate { CorrectButtonClick(); ; });
                btn2.onClick.AddListener(delegate { InvalidButtonClick(1); });
                btn3.onClick.AddListener(delegate { InvalidButtonClick(2); });
                btn4.onClick.AddListener(delegate { InvalidButtonClick(3); });

                btn1.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn2.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn3.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn4.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            }
        }
        else if(SelectedLanguage == Language.Python)
        {
            if (question == 2)
            {
                Button btn1 = InstantiatedQuestion.transform.Find("Button1").GetComponent<Button>();
                Button btn2 = InstantiatedQuestion.transform.Find("Button2").GetComponent<Button>();
                Button btn3 = InstantiatedQuestion.transform.Find("Button3").GetComponent<Button>();
                Button btn4 = InstantiatedQuestion.transform.Find("Button4").GetComponent<Button>();

                btn1.onClick.AddListener(delegate { InvalidButtonClick(0); });
                btn2.onClick.AddListener(delegate { CorrectButtonClick(); });
                btn3.onClick.AddListener(delegate { InvalidButtonClick(2); });
                btn4.onClick.AddListener(delegate { InvalidButtonClick(3); });

                btn1.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn2.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn3.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn4.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            }
            else if (question == 3)
            {
                Button btn1 = InstantiatedQuestion.transform.Find("Button1").GetComponent<Button>();
                Button btn2 = InstantiatedQuestion.transform.Find("Button2").GetComponent<Button>();
                Button btn3 = InstantiatedQuestion.transform.Find("Button3").GetComponent<Button>();
                Button btn4 = InstantiatedQuestion.transform.Find("Button4").GetComponent<Button>();

                btn1.onClick.AddListener(delegate { InvalidButtonClick(0); });
                btn2.onClick.AddListener(delegate { InvalidButtonClick(1); });
                btn3.onClick.AddListener(delegate { CorrectButtonClick();  });
                btn4.onClick.AddListener(delegate { InvalidButtonClick(3); });

                btn1.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn2.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn3.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn4.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            }
            else if (question == 4)
            {
                Button btn1 = InstantiatedQuestion.transform.Find("Button1").GetComponent<Button>();
                Button btn2 = InstantiatedQuestion.transform.Find("Button2").GetComponent<Button>();
                Button btn3 = InstantiatedQuestion.transform.Find("Button3").GetComponent<Button>();
                Button btn4 = InstantiatedQuestion.transform.Find("Button4").GetComponent<Button>();

                btn1.onClick.AddListener(delegate { InvalidButtonClick(0); });
                btn2.onClick.AddListener(delegate { InvalidButtonClick(1); });
                btn3.onClick.AddListener(delegate { InvalidButtonClick(2); });
                btn4.onClick.AddListener(delegate { CorrectButtonClick(); });

                btn1.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn2.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn3.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
                btn4.onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
            }
        }
    }

    void CorrectButtonClick()
    {
        Destroy(InstantiatedQuestion);
        CompletedQuiz();
    }
    void InvalidButtonClick(int buttonnum)
    {
        IncorrectAttempts += 1;
        if(buttonnum == 0)
        {
            InstantiatedQuestion.transform.Find("Button1").GetComponent<Image>().color = Color.red;
        }
        else if (buttonnum == 1)
        {
            InstantiatedQuestion.transform.Find("Button2").GetComponent<Image>().color = Color.red;
        }
        else if (buttonnum == 2)
        {
            InstantiatedQuestion.transform.Find("Button3").GetComponent<Image>().color = Color.red;
        }
        else if (buttonnum == 3)
        {
            InstantiatedQuestion.transform.Find("Button4").GetComponent<Image>().color = Color.red;
        }
    }
    public void CheckBlockQuestion(int question)
    {
        if(SelectedLanguage == Language.CSharp)
        {
            if (question == 0)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;
                bool check6 = false;

                TMP_Dropdown t1 = InstantiatedQuestion.transform.Find("DropDown1").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t2 = InstantiatedQuestion.transform.Find("DropDown2").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t3 = InstantiatedQuestion.transform.Find("DropDown3").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t4 = InstantiatedQuestion.transform.Find("DropDown4").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t5 = InstantiatedQuestion.transform.Find("DropDown5").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t6 = InstantiatedQuestion.transform.Find("DropDown6").GetComponent<TMP_Dropdown>();

                if(t1.value == 4)
                {
                    check1 = true;
                }
                if(t2.value == 2)
                {
                    check2 = true;
                }
                if(t3.value == 3)
                {
                    check3 = true;
                }
                if(t4.value == 5)
                {
                    check4 = true;
                }
                if(t5.value == 3)
                {
                    check5 = true;
                }
                if(t6.value == 5)
                {
                    check6 = true;
                }

                if(check1 && check2 && check3 && check4 && check5 && check6)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 1)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;
                bool check6 = false;

                TMP_Dropdown t1 = InstantiatedQuestion.transform.Find("DropDown1").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t2 = InstantiatedQuestion.transform.Find("DropDown2").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t3 = InstantiatedQuestion.transform.Find("DropDown3").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t4 = InstantiatedQuestion.transform.Find("DropDown4").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t5 = InstantiatedQuestion.transform.Find("DropDown5").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t6 = InstantiatedQuestion.transform.Find("DropDown6").GetComponent<TMP_Dropdown>();

                if (t1.value == 5)
                {
                    check1 = true;
                }
                if (t2.value == 2)
                {
                    check2 = true;
                }
                if (t3.value == 4)
                {
                    check3 = true;
                }
                if (t4.value == 3)
                {
                    check4 = true;
                }
                if (t5.value == 4)
                {
                    check5 = true;
                }
                if (t6.value == 5)
                {
                    check6 = true;
                }

                if (check1 && check2 && check3 && check4 && check5 && check6)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 5)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;
                bool check6 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ","").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t6 = InstantiatedQuestion.transform.Find("Input6").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if(t1 == "i<50" || t1 == "i<50;")
                {
                    check1 = true;
                }
                if(t2 == "j>=10" || t2 == "j>=10;")
                {
                    check2 = true;
                }
                if(t3 == "j--" || t3=="j-=1" || t3 == "j=j-1" || t3 == "j--;" || t3 == "j-=1;" || t3 == "j=j-1;")
                {
                    check3 = true;
                }
                if(t4 == "intk=0" || t4 == "intk=0;")
                {
                    check4 = true;
                }
                if(t5 == "k<=30" || t5 == "k<=30;")
                {
                    check5 = true;
                }
                if(t6 == "k++" || t6 == "k+=1" || t6 == "k=k+1" || t6 == "k++;" || t6 == "k+=1;" || t6 == "k=k+1;")
                {
                    check6 = true;
                }

                if(check1 && check2 && check3 && check4 && check5 && check6)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 6)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;
                bool check6 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t6 = InstantiatedQuestion.transform.Find("Input6").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 == "a>30" || t1 == "a>30;")
                {
                    check1 = true;
                }
                if (t2 == "b<=15" || t2 == "b<=15;")
                {
                    check2 = true;
                }
                if (t3 == "b+=2" || t3 == "b=b+2" || t3 == "b+=2;" || t3 == "b=b+2;")
                {
                    check3 = true;
                }
                if (t4 == "intc=24" || t4 == "intc=24;")
                {
                    check4 = true;
                }
                if (t5 == "c!=12" || t5 == "c!=12;")
                {
                    check5 = true;
                }
                if (t6 == "c-=3" || t6 == "c=c-3" || t6 == "c-=3;" || t6 == "c=c-3;")
                {
                    check6 = true;
                }

                if (check1 && check2 && check3 && check4 && check5 && check6)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 7)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.ToLower();

                if(t1 == "numbers")
                {
                    check1 = true;
                }
                if(t2 != "" && t2 != " ")
                {
                    List<string> temp = new List<string>();
                    string[] tholder = t2.Split(" ");

                    foreach (string t in tholder)
                    {
                        print(t);
                        if (t != "" && t != " ")
                        {
                            temp.Add(t);
                        }
                    }

                    if (temp.Count == 2)
                    {
                        if (temp[0] == "string" && temp[1] != " " && temp[1] != "")
                        {
                            check2 = true;
                        }
                        else
                        {
                            foreach (string t in temp)
                            {
                                print(t);
                            }
                        }
                    }
                    else
                    {
                        print(temp.Count);
                    }
                }
                if(t3 == "fruitstocks")
                {
                    check3 = true;
                }
                if(t4 != "" && t4 != " ")
                {
                    List<string> temp = new List<string>();
                    string[] tholder = t4.Split(" ");

                    foreach(string t in tholder)
                    {
                        if(t != "" && t != " ")
                        {
                            temp.Add(t);
                        }
                    }

                    if (temp.Count == 4)
                    {
                        if (temp[0] == "bool" && temp[1] != " " && temp[1] != "" && temp[2] == "in" && temp[3] == "premchecks")
                        {
                            check4 = true;
                        }
                    }
                }

                if(check1 && check2 && check3 && check4)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 8)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                

                if(t1 == "if")
                {
                    check1 = true;
                }
                if(t2 == "elseif")
                {
                    check2 = true;
                }
                if(t3 == "else")
                {
                    check3 = true;
                }

                if(check1 && check2 && check3)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 9)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if(t1 == "if")
                {
                    check1 = true;
                }
                if(t2 == ">10")
                {
                    check2 = true;
                }
                if(t3 == "elseif")
                {
                    check3 = true;
                }
                if(t4 == "<=10")
                {
                    check4 = true;
                }
                if(t5 == "else")
                {
                    check5 = true;
                }

                if(check1 && check2 && check3 && check4 && check5)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 10)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 == "if")
                {
                    check1 = true;
                }
                if (t2 == ">")
                {
                    check2 = true;
                }
                if (t3 == "elseif")
                {
                    check3 = true;
                }
                if (t4 == "index>=50")
                {
                    check4 = true;
                }

                if(check1 && check2 && check3 && check4)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 11)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 == "keycode")
                {
                    check1 = true;
                }
                else
                {
                    print("Invalid: " + t1);
                }
                if(t2 == "break" || t2 == "break;")
                {
                    check2 = true;
                }
                else
                {
                    print("Invalid: " + t2);
                }
                if(t3 == "case")
                {
                    check3 = true;
                }
                else
                {
                    print("Invalid: " + t3);
                }
                if(t4 == "break" || t4 == "break;")
                {
                    check4 = true;
                }
                else
                {
                    print("Invalid: " + t4);
                }
                if(t5 == "default" || t5 == "default:")
                {
                    check5 = true;
                }
                else
                {
                    print("Invalid: " + t5);
                }

                if(check1 && check2 && check3 && check4 && check5)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 12)
            {

                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 == "index")
                {
                    check1 = true;
                }
                if (t2 == "break" || t2 == "break;")
                {
                    check2 = true;
                }
                if (t3 == "case")
                {
                    check3 = true;
                }
                if (t4 == "break" || t4 == "break;")
                {
                    check4 = true;
                }
                if (t5 == "case")
                {
                    check5 = true;
                }

                if(check1 && check2 && check3 && check4 && check5)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 13)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if(t1 == "x<20")
                {
                    check1 = true;
                }
                if(t2 == "y<30")
                {
                    check2 = true;
                }
                if(t3 == "y+=5" || t3 == "y=y+5" || t3 == "y+=5;" || t3 == "y=y+5;")
                {
                    check3 = true;
                }

                if(check1 && check2 && check3)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if (question == 14)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 == "v>=50")
                {
                    check1 = true;
                }
                if (t2 == "z>4")
                {
                    check2 = true;
                }
                if (t3 == "z-=4" || t3 == "z=z-4" || t3 == "z-=4;" || t3 == "z=z-4;")
                {
                    check3 = true;
                }

                if(check1 && check2 && check3)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
        }
        else if(SelectedLanguage == Language.Python)
        {
            if (question == 0)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;
                bool check6 = false;

                TMP_Dropdown t1 = InstantiatedQuestion.transform.Find("DropDown1").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t2 = InstantiatedQuestion.transform.Find("DropDown2").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t3 = InstantiatedQuestion.transform.Find("DropDown3").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t4 = InstantiatedQuestion.transform.Find("DropDown4").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t5 = InstantiatedQuestion.transform.Find("DropDown5").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t6 = InstantiatedQuestion.transform.Find("DropDown6").GetComponent<TMP_Dropdown>();

                if (t1.value == 5)
                {
                    check1 = true;
                }
                if (t2.value == 3)
                {
                    check2 = true;
                }
                if (t3.value == 4)
                {
                    check3 = true;
                }
                if (t4.value == 2)
                {
                    check4 = true;
                }
                if (t5.value == 1)
                {
                    check5 = true;
                }
                if (t6.value == 3)
                {
                    check6 = true;
                }

                if (check1 && check2 && check3 && check4 && check5 && check6)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if (question == 1)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;
                bool check6 = false;

                TMP_Dropdown t1 = InstantiatedQuestion.transform.Find("DropDown1").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t2 = InstantiatedQuestion.transform.Find("DropDown2").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t3 = InstantiatedQuestion.transform.Find("DropDown3").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t4 = InstantiatedQuestion.transform.Find("DropDown4").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t5 = InstantiatedQuestion.transform.Find("DropDown5").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t6 = InstantiatedQuestion.transform.Find("DropDown6").GetComponent<TMP_Dropdown>();

                if (t1.value == 4)
                {
                    check1 = true;
                }
                if (t2.value == 3)
                {
                    check2 = true;
                }
                if (t3.value == 1)
                {
                    check3 = true;
                }
                if (t4.value == 2)
                {
                    check4 = true;
                }
                if (t5.value == 3)
                {
                    check5 = true;
                }
                if (t6.value == 4)
                {
                    check6 = true;
                }

                if (check1 && check2 & check3 && check4 && check5 && check6)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if (question == 5)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.ToLower();


                if (t1 == "integers")
                {
                    check1 = true;
                }
                if (t2 != "" && t2 != " ")
                {
                    check2 = true;
                }
                if (t3 == "strings")
                {
                    check3 = true;
                }
                if (t4 != "" && t4 != " ")
                {
                    List<string> temp = new List<string>();
                    string[] tholder = t4.Split(" ");

                    foreach(string t in tholder)
                    {
                        temp.Add(t);
                    }

                    if (temp.Count == 3)
                    {
                        if (temp[0] != "" && temp[0] != " " && temp[1] == "in" && temp[2] == "bools")
                        {
                            check4 = true;
                        }
                    }
                }

                if (check1 && check2 && check3 && check4)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if (question == 6)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.ToLower();


                if (t1 == "keys")
                {
                    check1 = true;
                }
                if (t2 != "" && t2 != " ")
                {
                    check2 = true;
                }
                if (t3 == "cars")
                {
                    check3 = true;
                }
                if (t4 != "" && t4 != " ")
                {
                    List<string> temp = new List<string>();
                    string[] tholder = t4.Split(" ");

                    foreach (string t in tholder)
                    {
                        temp.Add(t);
                    }

                    if (temp.Count == 3)
                    {
                        if (temp[0] != "" && temp[0] != " " && temp[1] == "in" && temp[2] == "drinks")
                        {
                            check4 = true;
                        }
                    }
                }

                if (check1 && check2 && check3 && check4)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 7)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if(t1 == "if")
                {
                    check1 = true;
                }
                if (t2 == "elif")
                {
                    check2 = true;
                }
                if (t3 == "elif")
                {
                    check3 = true;
                }
                if (t4 == "else")
                {
                    check4 = true;
                }

                if(check1 && check2 && check3 && check4)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if (question == 8)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 == "if")
                {
                    check1 = true;
                }
                if (t2 == ">10")
                {
                    check2 = true;
                }
                if (t3 == "elif")
                {
                    check3 = true;
                }
                if (t4 == "<=10")
                {
                    check4 = true;
                }
                if(t5 == "else")
                {
                    check5 = true;
                }

                if (check1 && check2 && check3 && check4 && check5)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 9)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;
                bool check4 = false;
                bool check5 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if(t1 == "if")
                {
                    check1 = true;
                }
                if(t2 == ">")
                {
                    check2 = true;
                }
                if(t3 == "elif")
                {
                    check3 = true;
                }
                if(t4 == ">=")
                {
                    check4 = true;
                }
                if(t5 == "pass")
                {
                    check5 = true;
                }

                if(check1 && check2 && check3 && check4 && check5)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 10)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if(t1 == "x<20")
                {
                    check1 = true;
                }
                if(t2 == "y>30")
                {
                    check2 = true;
                }
                if(t3 == "y-=2" || t3 == "y=y-2")
                {
                    check3 = true;
                }

                if(check1 && check2 && check3)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
            else if(question == 11)
            {
                bool check1 = false;
                bool check2 = false;
                bool check3 = false;

                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 == "v>=30")
                {
                    check1 = true;
                }
                if (t2 == "z<40")
                {
                    check2 = true;
                }
                if (t3 == "z+=4" || t3 == "z=z+4")
                {
                    check3 = true;
                }

                if (check1 && check2 && check3)
                {
                    Destroy(InstantiatedQuestion);
                    CompletedQuiz();
                }
                else
                {
                    StartCoroutine(InvalidQuestionAttempt(question));
                }
            }
        }
    }


    IEnumerator InvalidQuestionAttempt(int questionnumber)
    {
        IncorrectAttempts += 1;
        if (SelectedLanguage == Language.CSharp)
        {
            if (questionnumber == 0)
            {
                TMP_Dropdown t1 = InstantiatedQuestion.transform.Find("DropDown1").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t2 = InstantiatedQuestion.transform.Find("DropDown2").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t3 = InstantiatedQuestion.transform.Find("DropDown3").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t4 = InstantiatedQuestion.transform.Find("DropDown4").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t5 = InstantiatedQuestion.transform.Find("DropDown5").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t6 = InstantiatedQuestion.transform.Find("DropDown6").GetComponent<TMP_Dropdown>();

                if (t1.value != 4)
                {
                    InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.green;
                }
                if (t2.value != 2)
                {
                    InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.green;
                }
                if (t3.value != 3)
                {
                    InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.green;
                }
                if (t4.value != 5)
                {
                    InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.green;
                }
                if (t5.value != 3)
                {
                    InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.green;
                }
                if (t6.value != 5)
                {
                    InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.green;
                }
                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 1)
            {
                TMP_Dropdown t1 = InstantiatedQuestion.transform.Find("DropDown1").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t2 = InstantiatedQuestion.transform.Find("DropDown2").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t3 = InstantiatedQuestion.transform.Find("DropDown3").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t4 = InstantiatedQuestion.transform.Find("DropDown4").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t5 = InstantiatedQuestion.transform.Find("DropDown5").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t6 = InstantiatedQuestion.transform.Find("DropDown6").GetComponent<TMP_Dropdown>();

                if (t1.value != 5)
                {
                    InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.green;
                }
                if (t2.value != 2)
                {
                    InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.green;
                }
                if (t3.value != 4)
                {
                    InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.green;
                }
                if (t4.value != 3)
                {
                    InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.green;
                }
                if (t5.value != 4)
                {
                    InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.green;
                }
                if (t6.value != 5)
                {
                    InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.green;
                }
                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 5)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t6 = InstantiatedQuestion.transform.Find("Input6").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "i<50" && t1 != "i<50;")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "j>=10" && t2 != "j>=10;")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "j--" && t3 != "j-=1" && t3 != "j=j-1" && t3 != "j--;" && t3 != "j-=1;" && t3 != "j=j-1;")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != "intk=0" && t4 != "intk=0;")
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                }
                if (t5 != "k<=30" && t5 != "k<=30;")
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
                }
                if (t6 != "k++" && t6 != "k+=1" && t6 != "k=k+1" && t6 != "k++;" && t6 != "k+=1;" && t6 == "k=k+1;")
                {
                    InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.green;
                }
                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 6)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t6 = InstantiatedQuestion.transform.Find("Input6").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "a>30" && t1 != "a>30;")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "b<=15" && t2 != "b<=15;")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "b+=2" && t3 != "b=b+2" && t3 != "b+=2;" && t3 != "b=b+2;")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red; ;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != "intc=24" && t4 != "intc=24;")
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                }
                if (t5 != "c!=12" && t5 != "c!=12;")
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
                }
                if (t6 != "c-=3" && t6 != "c=c-3" && t6 != "c-=3;" && t6 != "c=c-3;")
                {
                    InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.green;
                }
                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input6").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 7)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.ToLower();

                if (t1 != "numbers")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "" || t2 != " ")
                {
                    List<string> temp = new List<string>();
                    string[] tholder = t2.Split(" ");

                    foreach (string t in tholder)
                    {
                        if (t != "" && t != " ")
                        {
                            temp.Add(t);
                        }
                    }

                    if (temp.Count == 2)
                    {
                        if (temp[0] != "string" || temp[1] == "" || temp[1] == " ")
                        {
                            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                        }
                        else
                        {
                            InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                        }
                    }
                    else
                    {
                        InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                    }
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "fruitstocks")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != "" && t4 != " ")
                {
                    List<string> temp = new List<string>();
                    string[] tholder = t4.Split(" ");

                    foreach (string t in tholder)
                    {
                        if (t != "" && t != " ")
                        {
                            temp.Add(t);
                        }
                    }

                    if (temp.Count == 4)
                    {
                        if (temp[0] != "bool" || temp[1] == "" || temp[1] == " " || temp[2] != "in" || temp[2] != "premchecks")
                        {
                            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                        }
                        else
                        {
                            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                        }
                    }
                    else
                    {
                        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                    }
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                }

                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 8)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();


                if (t1 != "if")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "elseif")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "else")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 9)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "if")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != ">10")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "elseif")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != "<=10")
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                }
                if (t5 != "else")
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
                }
                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 10)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "if")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != ">")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "elseif")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != ">=")
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 11)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "keycode")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "break" && t2 != "break;")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "case")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != "break" && t4 != "break;")
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                }
                if (t5 != "default" && t5 != "default:")
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
            }
            else if (questionnumber == 12)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "index")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "break" && t2 != "break;")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "case")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != "break" && t4 != "break;")
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                }
                if (t5 != "case")
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 13)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "x<20")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "y<30")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "y+=5" && t3 != "y=y+5" && t3 != "y+=5;" && t3 != "y=y+5;")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
            }
            else if (questionnumber == 14)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "v>=50")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "z>4")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "z-=4" && t3 != "z=z-4" && t3 != "z-=4;" && t3 != "z=z-4;")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
            }
        }
        else if(SelectedLanguage == Language.Python)
        {
            if (questionnumber == 0)
            {
                TMP_Dropdown t1 = InstantiatedQuestion.transform.Find("DropDown1").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t2 = InstantiatedQuestion.transform.Find("DropDown2").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t3 = InstantiatedQuestion.transform.Find("DropDown3").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t4 = InstantiatedQuestion.transform.Find("DropDown4").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t5 = InstantiatedQuestion.transform.Find("DropDown5").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t6 = InstantiatedQuestion.transform.Find("DropDown6").GetComponent<TMP_Dropdown>();

                if (t1.value != 5)
                {
                    InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.green;
                }
                if (t2.value != 3)
                {
                    InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.green;
                }
                if (t3.value != 4)
                {
                    InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.green;
                }
                if (t4.value != 2)
                {
                    InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.green;
                }
                if (t5.value != 1)
                {
                    InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.green;
                }
                if (t6.value != 3)
                {
                    InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 1)
            {
                TMP_Dropdown t1 = InstantiatedQuestion.transform.Find("DropDown1").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t2 = InstantiatedQuestion.transform.Find("DropDown2").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t3 = InstantiatedQuestion.transform.Find("DropDown3").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t4 = InstantiatedQuestion.transform.Find("DropDown4").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t5 = InstantiatedQuestion.transform.Find("DropDown5").GetComponent<TMP_Dropdown>();
                TMP_Dropdown t6 = InstantiatedQuestion.transform.Find("DropDown6").GetComponent<TMP_Dropdown>();

                if (t1.value != 4)
                {
                    InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.green;
                }
                if (t2.value != 3)
                {
                    InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.green;
                }
                if (t3.value != 1)
                {
                    InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.green;
                }
                if (t4.value != 2)
                {
                    InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.green;
                }
                if (t5.value != 3)
                {
                    InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.green;
                }
                if (t6.value != 4)
                {
                    InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("DropDown1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown5").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("DropDown6").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 5)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.ToLower();


                if (t1 != "integers")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 == "" || t2 == " ")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "strings")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != "" || t4 != " ")
                {
                    List<string> temp = new List<string>();
                    string[] tholder = t4.Split(" ");

                    foreach (string t in tholder)
                    {
                        temp.Add(t);
                    }

                    if (temp.Count == 3)
                    {
                        if (temp[0] == "" || temp[0] == " " || temp[1] != "in" || temp[2] != "bools")
                        {
                            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                        }
                        else
                        {
                            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                        }
                    }
                    else
                    {
                        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                    }

                }
                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
            }
            else if (questionnumber == 6)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.ToLower();


                if (t1 != "keys")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 == "" || t2 == " ")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "cars")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != "" || t4 != " ")
                {
                    List<string> temp = new List<string>();
                    string[] tholder = t4.Split(" ");

                    foreach (string t in tholder)
                    {
                        temp.Add(t);
                    }

                    if (temp.Count == 3)
                    {
                        if (temp[0] == "" || temp[0] == " " || temp[1] != "in" || temp[2] != "drinks")
                        {
                            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                        }
                        else
                        {
                            InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                        }
                    }
                    else
                    {
                        InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                    }

                }
                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 7)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "if")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "elif")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "elif")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != "else")
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);
                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 8)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "if")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != ">10")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "elif")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != "<=10")
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                }
                if (t5 != "else")
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);

                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 9)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t4 = InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t5 = InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "if")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != ">")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "elif")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }
                if (t4 != ">=")
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.green;
                }
                if (t5 != "pass")
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);

                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 10)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "x<20")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "y>30")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "y-=2" && t3 != "y=y-2")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);

                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
            }
            else if(questionnumber == 11)
            {
                string t1 = InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t2 = InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();
                string t3 = InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text.Replace(" ", "").ToLower();

                if (t1 != "v>=30")
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.green;
                }
                if (t2 != "z<40")
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.green;
                }
                if (t3 != "z+=4" && t3 == "z=z+2")
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.green;
                }

                yield return new WaitForSeconds(1f);

                InstantiatedQuestion.transform.Find("Input1").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<Image>().color = Color.white;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<Image>().color = Color.white;
            }
        }
    }

    void CompletedQuiz()
    {
        if(InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        CodeImage.transform.Find("Line").gameObject.SetActive(true);
        CodeImage.transform.Find("SelectBlocks").gameObject.SetActive(true);
        TitleText.text = "Block Selection";
        ContentText.text = "Select a block!";
        if(UnlockBlocks1 == true)
        {
            CodeImage.transform.Find("SelectBlocks").transform.Find("Images1").gameObject.SetActive(true);
        }
        else
        {
            CodeImage.transform.Find("SelectBlocks").transform.Find("Images1").gameObject.SetActive(false);
        }
        if(UnlockBlocks2 == true)
        {
            CodeImage.transform.Find("SelectBlocks").transform.Find("Images2").gameObject.SetActive(true);
        }
        else
        {
            CodeImage.transform.Find("SelectBlocks").transform.Find("Images2").gameObject.SetActive(false);
        }
        if (UnlockBlocks3 == true)
        {
            CodeImage.transform.Find("SelectBlocks").transform.Find("Images3").gameObject.SetActive(true);
            CodeImage.transform.Find("SelectBlocks").transform.Find("Images4").gameObject.SetActive(true);
        }
        else
        {
            CodeImage.transform.Find("SelectBlocks").transform.Find("Images3").gameObject.SetActive(false);
            CodeImage.transform.Find("SelectBlocks").transform.Find("Images4").gameObject.SetActive(false);
        }
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        CodeImage.transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(AddBlockToPlayer);
        CodeImage.transform.Find("OkButton").transform.GetChild(0).GetComponent<Text>().text = "Select";
    }

    public void SelectBlock(string blockrow)
    {
        string[] temp = blockrow.Split(",");
        int row = Convert.ToInt32(temp[0]);
        int blocknum = Convert.ToInt32(temp[1]);

        print("Row: " + row);
        print("Index: " + blocknum);
        GameObject selectblocks = CodeImage.transform.Find("SelectBlocks").gameObject;

        if (row == 1)
        {
            int index = 0;
            foreach(Transform t in selectblocks.transform.Find("Images1"))
            {
                if(index == blocknum)
                {
                    t.gameObject.GetComponent<Image>().color = Color.white;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 110f);
                }
                else
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
                index++;
            }

            if (selectblocks.transform.Find("Images2").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images2"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }
            if (selectblocks.transform.Find("Images3").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images3"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }
            if (selectblocks.transform.Find("Images4").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images4"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }
        }
        else if(row == 2)
        {
            if (selectblocks.transform.Find("Images1").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images1"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }

            int index = 0;
            foreach (Transform t in selectblocks.transform.Find("Images2"))
            {
                if (index == blocknum)
                {
                    t.gameObject.GetComponent<Image>().color = Color.white;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 110f);
                }
                else
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
                index++;
            }

            if (selectblocks.transform.Find("Images3").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images3"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }
            if (selectblocks.transform.Find("Images4").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images4"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }

        }
        else if(row == 3)
        {
            if (selectblocks.transform.Find("Images1").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images1"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }

            if (selectblocks.transform.Find("Images2").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images2"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }
            int index = 0;
            foreach (Transform t in selectblocks.transform.Find("Images3"))
            {
                if (index == blocknum)
                {
                    t.gameObject.GetComponent<Image>().color = Color.white;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 110f);
                }
                else
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
                index++;
            }
            if (selectblocks.transform.Find("Images4").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images4"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }
        }
        else if(row == 4)
        {
            if (selectblocks.transform.Find("Images1").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images1"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }

            if (selectblocks.transform.Find("Images2").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images2"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }
            if (selectblocks.transform.Find("Images3").gameObject.activeSelf)
            {
                foreach (Transform t in selectblocks.transform.Find("Images3"))
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
            }
            int index = 0;
            foreach (Transform t in selectblocks.transform.Find("Images4"))
            {
                if (index == blocknum)
                {
                    t.gameObject.GetComponent<Image>().color = Color.white;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 110f);
                }
                else
                {
                    t.gameObject.GetComponent<Image>().color = Color.grey;
                    t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                }
                index++;
            }
        }

        if(row == 1)
        {
            if (blocknum == 0)
            {
                Selectedblock = 1;
            }
            else if(blocknum == 1)
            {
                Selectedblock = 2;
            }
            else if (blocknum == 2)
            {
                Selectedblock = 3;
            }
            else if (blocknum == 3)
            {
                Selectedblock = 5;
            }
        }
        else if (row == 2)
        {
            if (blocknum == 0)
            {
                Selectedblock = 6;
            }
            else if (blocknum == 1)
            {
                Selectedblock = 7;
            }
            else if (blocknum == 2)
            {
                Selectedblock = 28;
            }
            else if (blocknum == 3)
            {
                Selectedblock = 23;
            }
        }
        else if (row == 3)
        {
            if (blocknum == 0)
            {
                Selectedblock = 24;
            }
            else if (blocknum == 1)
            {
                Selectedblock = 26;
            }
            else if (blocknum == 2)
            {
                Selectedblock = 20;
            }
            else if (blocknum == 3)
            {
                Selectedblock = 21;
            }
        }
        else if (row == 4)
        {
            if (blocknum == 0)
            {
                Selectedblock = 19;
            }
            else if (blocknum == 1)
            {
                Selectedblock = 17;
            }
            else if (blocknum == 2)
            {
                Selectedblock = 13;
            }
            else if (blocknum == 3)
            {
                Selectedblock = 15;
            }
        }

        //byte[] image = new byte[] { 1, 2, 3, 5, 6, 7, 28, 23, 24, 26, 20, 21, 19, 17, 13, 15};
    }

    void AddBlockToPlayer()
    {
        if(Selectedblock != 99)
        {
            bool didchange = false;
            PlayerIO pi = GameObject.Find("player").GetComponent<PlayerIO>();

            for(int i = 0; i < pi.hotbarBlocksAmount.Length; i++)
            {
                if(pi.hotbarBlocks[i] == 34 && pi.hotbarBlocksAmount[i] == 0)
                {
                    pi.hotbarBlocksAmount[i] = 8;
                    pi.hotbarBlocks[i] = Selectedblock;
                    didchange = true;
                    break;
                }
                else if(pi.hotbarBlocks[i] == Selectedblock && pi.hotbarBlocksAmount[i] != 0)
                {
                        pi.hotbarBlocksAmount[i] += 8;
                        didchange = true;
                        break;
                }
            }

            if (!didchange)
            {
                bool completed = false;
                int row = 0;
                int index = 0;
                for (int i = 0; i < pi.InvRow1.Length; i++)
                {
                    if(pi.InvRow1[i] == 34)
                    {
                        pi.InvRow1[i] = Selectedblock;
                        index = i;
                        completed = true;
                        break;
                    }
                    else
                    {
                        if (pi.InvRow1[i] == Selectedblock)
                        {
                            index = i;
                            completed = true;
                            break;
                        }
                    }
                }

                if (!completed)
                {
                    row = 1;
                    for (int i = 0; i < pi.InvRow2.Length; i++)
                    {
                        if (pi.InvRow2[i] == 34)
                        {
                            pi.InvRow1[i] = Selectedblock;
                            index = i;
                            completed = true;
                            break;
                        }
                        else
                        {
                            if (pi.InvRow2[i] == Selectedblock)
                            {
                                index = i;
                                completed = true;
                                break;
                            }
                        }
                    }

                    if (!completed)
                    {
                        row = 2;
                        for (int i = 0; i < pi.InvRow3.Length; i++)
                        {
                            if (pi.InvRow3[i] == 34)
                            {
                                pi.InvRow3[i] = Selectedblock;
                                index = i;
                                completed = true;
                                break;
                            }
                            else
                            {
                                if(pi.InvRow3[i] == Selectedblock)
                                {
                                    index = i;
                                    completed = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (pi.InventoryRows[row].transform.GetChild(index).gameObject.GetComponent<Image>().sprite != null)
                {
                    int currentamount = Convert.ToInt32(pi.InventoryRows[row].transform.GetChild(index).GetChild(0).GetComponent<TMP_Text>().text);
                    currentamount += 8;
                    pi.InventoryRows[row].transform.GetChild(index).GetChild(0).GetComponent<TMP_Text>().text = currentamount.ToString();
                }
                else
                {
                    pi.InventoryRows[row].transform.GetChild(index).gameObject.GetComponent<Image>().sprite = pi.spritesByBlockID[Selectedblock - 1];
                    pi.InventoryRows[row].transform.GetChild(index).GetChild(0).GetComponent<TMP_Text>().text = "8";
                }

            }

            CodeImage.transform.Find("SelectBlocks").gameObject.SetActive(false);
            Selectedblock = 99;

            GameObject selectblocks = CodeImage.transform.Find("SelectBlocks").gameObject;
            foreach (Transform t in selectblocks.transform.Find("Images1"))
            {
                t.gameObject.GetComponent<Image>().color = Color.white;
                t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 110f);
            }
            foreach (Transform t in selectblocks.transform.Find("Images2"))
            {
                t.gameObject.GetComponent<Image>().color = Color.white;
                t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 110f);
            }
            foreach (Transform t in selectblocks.transform.Find("Images3"))
            {
                t.gameObject.GetComponent<Image>().color = Color.white;
                t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 110f);
            }
            foreach (Transform t in selectblocks.transform.Find("Images4"))
            {
                t.gameObject.GetComponent<Image>().color = Color.white;
                t.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 110f);
            }
            Close();

        }
    }

    public void CloseQuestion()
    {
        ifclosedquestion = true;
        int t = RecentQuestionsAnswered[0];

        if (SelectedLanguage == Language.CSharp)
        {
            if (t == 5)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input6").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "i<50";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "j>=10";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "j--";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "int k = 0";
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text = "k<=30";
                InstantiatedQuestion.transform.Find("Input6").GetComponent<TMP_InputField>().text = "k++";
            }
            else if (t == 6)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input6").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "a>30";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "b<=15";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "b+=2";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "int c = 24";
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text = "c!=12";
                InstantiatedQuestion.transform.Find("Input6").GetComponent<TMP_InputField>().text = "c-=3";
            }
            else if (t == 7)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "numbers";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "random name";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "fruitstocks";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "bool prem in premchecks";
            }
            else if (t == 8)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "if";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "else if";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "else";
            }
            else if (t == 9)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "if";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = ">10";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "else if";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "<=10";
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text = "else";
            }
            else if (t == 10)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "if";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = ">";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "else if";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "index >= 50";
            }
            else if (t == 11)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "keycode";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "break";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "case";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "break";
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text = "default";
            }
            else if (t == 12)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "index";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "break";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "case";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "break";
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text = "case";
            }
            else if(t == 13)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "x<20";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "y<30";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "y+=5";
            }
            else if(t == 14)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "v>=50";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "z>4";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "z-=4";
            }
        }
        else if(SelectedLanguage == Language.Python)
        {
            if(t == 5)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "integers";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "random name";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "strings";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "x in bools";
            }
            else if(t == 6)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "keys";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "random name";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "cars";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "x in drinks";
            }
            else if(t == 7)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "if";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "elif";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "elif";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "else";
            }
            else if(t == 8)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "if";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = ">10";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "elif";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = "<=10";
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text = "else";
            }
            else if(t == 9)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "if";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = ">";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "elif";
                InstantiatedQuestion.transform.Find("Input4").GetComponent<TMP_InputField>().text = ">=";
                InstantiatedQuestion.transform.Find("Input5").GetComponent<TMP_InputField>().text = "pass";
            }
            else if(t == 10)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "x<20";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "y>30";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "y-=2";
            }
            else if(t == 11)
            {
                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().interactable = false;
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().interactable = false;

                InstantiatedQuestion.transform.Find("Input1").GetComponent<TMP_InputField>().text = "v>=30";
                InstantiatedQuestion.transform.Find("Input2").GetComponent<TMP_InputField>().text = "z<40";
                InstantiatedQuestion.transform.Find("Input3").GetComponent<TMP_InputField>().text = "z+=4";
            }
        }

        StartCoroutine(Skipped());
    }

    IEnumerator Skipped()
    {
        yield return new WaitForSeconds(5.5f);
        objectivetimer = 0;
        objectivename = null;
        IncorrectAttempts = 0;
        if(InstantiatedQuestion != null)
        {
            Destroy(InstantiatedQuestion);
        }
        if(CloseQuestionButton.activeSelf == true)
        {
            CloseQuestionButton.SetActive(false);
        }
        ifclosedquestion = false;
        SavedSkipped = false;
        Close();
    }
}



public enum BlockAccessType
{
    Generic,
    DataTypes,
    Loops,
    Conditions,
    None
}