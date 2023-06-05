using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class CraftingManager : MonoBehaviour
{
    //Blocks: 1(Dirt),2(Grass),3(Stone),4(BedrOCK),5(Cobblestone),6(Sand),7(Gravel),8/9/10/11(Stone),12(Wood),13(Leaves),14(Planks),15(Ice),16(Snow),17(MossyCobble),18(Obsidian),19(Sponge),20(Bookshelf),21(Stone),22(Brick),23(StoneBrick),24(BrokenCobb),25(StoneBrick),26(Pumpkin),27(Clay),28(Grass),29(Poppy),30(Dandelion),31(Red MUshroom),32(Brown Mushroom),33/34(Air),35(Crafting Bench)
    //Others: 36 (Stick), 53(IronIngot),54(GoldIngot),55(Diamond)

    //Tools: 37(WoodSword),38(WoodShovel),39(WoodPick),40(WoodAxe),41(StoneSword),42(StoneShovel),43(StonePick),44(StoneAxe),45(IronSword),46(IronShovel),47(IronPick),48(IronAxe),49(DiamondSword),50(DiamondShovel),51(DiamondPick),52(DiamondAxe)

    public GameObject Player;

    //1: Sword, 2:Shovel, 3:Pickaxe, 4:Axe
    public GameObject[] WoodToolBluePrints;
    public GameObject[] StoneToolBluePrints;
    public GameObject[] IronToolBluePrints;

    //1: Wood, 2: Stick, 3: Crafting Bench
    public GameObject[] OtherBluePrints;

    public GameObject Inventory;

    public GameObject CodeSpace;

    public byte[,] InvenBytes = new byte[3, 3];

    public GameObject CraftingGrid;

    public byte[,] CraftGridBytes = new byte[4, 4];
    public byte? ResSlotByte;

    [SerializeField]
    public GameObject CraftGrid;

    [SerializeField]
    public GameObject ResSlot;

    private byte oldByte = new byte();
    public GameObject InstantiatedBlueprint;

    [SerializeField]
    private int InstantiatedPrintIndex = 999;
    private bool InitializeCraftGrid = false;


    private List<GameObject> Buttons = new List<GameObject>();
    private List<bool> CorrectAnswers = new List<bool>();
    private BlueprintType CurrentBlueprintType = BlueprintType.NULL;
    public bool CorrectAnswerChosen = false;

    // Start is called before the first frame update
    void Start()
    {
        oldByte = 255;
    }

    // Update is called once per frame
    void Update()
    {
        if(CurrentBlueprintType != BlueprintType.NULL)
        {
            if(CurrentBlueprintType == BlueprintType.FillIn)
            {
                if(InstantiatedPrintIndex == 2)
                {
                    string text1 = InstantiatedBlueprint.transform.GetChild(1).Find("Q1").GetComponent<TMP_InputField>().text.Replace(" ", "");
                    string text2 = InstantiatedBlueprint.transform.GetChild(1).Find("Q2").GetComponent<TMP_InputField>().text.Replace(" ", "");
                    if (text1 == "i<4" || text1 == "i<=3")
                    {
                        CorrectAnswers[0] = true;

                        if(InstantiatedBlueprint.transform.GetChild(1).Find("Q1").GetComponent<Image>().color != Color.green)
                        {
                            InstantiatedBlueprint.transform.GetChild(1).Find("Q1").GetComponent<Image>().color = Color.green;
                        }
                    }
                    else
                    {
                        CorrectAnswers[0] = false;

                        if (InstantiatedBlueprint.transform.GetChild(1).Find("Q1").GetComponent<Image>().color != Color.white)
                        {
                            InstantiatedBlueprint.transform.GetChild(1).Find("Q1").GetComponent<Image>().color = Color.white;
                        }
                    }
                    if(text2 == "plank")
                    {
                        CorrectAnswers[1] = true;
                        if (InstantiatedBlueprint.transform.GetChild(1).Find("Q2").GetComponent<Image>().color != Color.green)
                        {
                            InstantiatedBlueprint.transform.GetChild(1).Find("Q2").GetComponent<Image>().color = Color.green;
                        }
                    }
                    else
                    {
                        CorrectAnswers[1] = false;
                        if (InstantiatedBlueprint.transform.GetChild(1).Find("Q2").GetComponent<Image>().color != Color.white)
                        {
                            InstantiatedBlueprint.transform.GetChild(1).Find("Q2").GetComponent<Image>().color = Color.white;
                        }
                    }

                    if(CorrectAnswers[0] == true && CorrectAnswers[1] == true)
                    {
                        CorrectAnswerChosen = true;
                        if (ResSlot.transform.GetChild(0).GetComponent<Image>().color != Color.white)
                        {
                            ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                        }
                    }
                    else
                    {
                        CorrectAnswerChosen = false;
                        if (ResSlot.transform.GetChild(0).GetComponent<Image>().color != Color.grey)
                        {
                            ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                        }
                    }
                }
            }
        }
        if (Inventory.activeSelf)
        {
            if (Inventory.transform.Find("CraftGrid").gameObject.activeSelf)
            {
                if (CraftGrid == null)
                {
                    CraftGrid = Inventory.transform.Find("CraftGrid").gameObject;
                }
                if (ResSlot == null)
                {
                    ResSlot = CraftGrid.transform.Find("ResSlot").gameObject;
                }
            }
            else if (Inventory.transform.Find("CraftTableGrid").gameObject.activeSelf)
            {
                if (CraftGrid == null)
                {
                    CraftGrid = Inventory.transform.Find("CraftTableGrid").gameObject;
                }
                if (ResSlot == null)
                {
                    ResSlot = CraftGrid.transform.Find("ResSlot").gameObject;
                }
            }


            //Make sure all crafting slots are empty
            if (!InitializeCraftGrid)
            {
                if(InstantiatedBlueprint != null)
                {
                    DestroyBlueprint();
                    InstantiatedPrintIndex = 999;
                }
                CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                if (Inventory.transform.Find("CraftTableGrid").gameObject.activeSelf)
                {
                    CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                    CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                    CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                    CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                    CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                }
                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                CodeSpace = Inventory.transform.Find("CodeSpace").gameObject;
                InitializeCraftGrid = true;

                if (Inventory.transform.Find("CraftTableGrid").gameObject.activeSelf)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            CraftGridBytes[i, j] = 34;
                            print(i + "," + j + ": " + CraftGridBytes[i,j]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            InvenBytes[i, j] = 34;
                        }
                    }
                }
            }

            if (Inventory.transform.Find("CraftGrid").gameObject.activeSelf)
            {
                //Stick
                if ((InvenBytes[0, 0] == 15 && InvenBytes[1, 0] == 15 && InvenBytes[0, 1] == 34 && InvenBytes[1, 1] == 34) || (InvenBytes[0, 1] == 15 && InvenBytes[1, 1] == 15 && InvenBytes[0, 0] == 34 && InvenBytes[1, 0] == 34))
                {

                    if (InstantiatedBlueprint == null)
                    {
                        CorrectAnswerChosen = false;
                        InstantiatedBlueprint = Instantiate(OtherBluePrints[1], new Vector3(0, 0, 0), Quaternion.identity);
                        InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                        InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                        InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                        InstantiatedPrintIndex = 1;
                        Buttons.Clear();
                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);
                        Buttons[0].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                        CurrentBlueprintType = BlueprintType.Buttons;
                    }
                    else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 1)
                    {
                        CorrectAnswerChosen = false;
                        print("Switching Blueprints");
                        Destroy(InstantiatedBlueprint);
                        InstantiatedBlueprint = Instantiate(OtherBluePrints[1], new Vector3(0, 0, 0), Quaternion.identity);
                        InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                        InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                        InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                        InstantiatedPrintIndex = 1;
                        Buttons.Clear();
                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);

                        Buttons[0].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                        CurrentBlueprintType = BlueprintType.Buttons;
                    }

                    if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[36])
                    {
                        ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[36];
                        ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "4";
                        ResSlotByte = 36;
                        ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                    }
                }
                //Crafting Bench
                else if (InvenBytes[0, 0] == 15 && InvenBytes[1, 0] == 15 && InvenBytes[0, 1] == 15 && InvenBytes[1, 1] == 15)
                {
                    if (InstantiatedBlueprint == null)
                    {
                        CorrectAnswerChosen = false;
                        InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                        InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                        InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                        InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                        InstantiatedPrintIndex = 2;
                        CorrectAnswers.Clear();
                        CorrectAnswers.Add(false);
                        CorrectAnswers.Add(false);
                        CurrentBlueprintType = BlueprintType.FillIn;
                    }
                    else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 2)
                    {
                        CorrectAnswerChosen = false;
                        Destroy(InstantiatedBlueprint);
                        InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                        InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                        InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                        InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                        InstantiatedPrintIndex = 2;
                        CorrectAnswers.Clear();
                        CorrectAnswers.Add(false);
                        CorrectAnswers.Add(false);
                        CurrentBlueprintType = BlueprintType.FillIn;
                    }

                    if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35])
                    {
                        ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35];
                        ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "1";
                        ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                        ResSlotByte = 35;
                    }
                }
                else
                {
                    if (InvenBytes[0, 0] == 15 || InvenBytes[0, 1] == 15 || InvenBytes[1, 0] == 15 || InvenBytes[1, 1] == 15)
                    {
                        if ((InvenBytes[0, 0] == 15 && InvenBytes[0, 1] == 34) || (InvenBytes[0, 1] == 15 && InvenBytes[0, 0] == 34))
                        {
                            CurrentBlueprintType = BlueprintType.NULL;
                            ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                            ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
                            ResSlotByte = null;
                            if (InstantiatedBlueprint != null)
                            {
                                Destroy(InstantiatedBlueprint);
                                CorrectAnswerChosen = false;
                            }
                        }
                        else if ((InvenBytes[1, 0] == 15 && InvenBytes[1, 1] == 34) || (InvenBytes[1, 1] == 15 && InvenBytes[1, 0] == 34))
                        {
                            CurrentBlueprintType = BlueprintType.NULL;
                            ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                            ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
                            ResSlotByte = null;
                            if (InstantiatedBlueprint != null)
                            {
                                Destroy(InstantiatedBlueprint);
                                CorrectAnswerChosen = false;
                            }
                        }
                    }
                    else
                    {
                        int ifound = 99;
                        int jfound = 99;
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (InvenBytes[i, j] == 13)
                                {
                                    ifound = i;
                                    jfound = j;
                                }
                            }
                        }

                        if (ifound != 99 && jfound != 99)
                        {
                            bool checkforemptyslots = true;
                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    if (i != ifound && j != jfound)
                                    {
                                        if (InvenBytes[i, j] != 34)
                                        {
                                            checkforemptyslots = false;
                                        }
                                    }
                                }
                            }



                            if (checkforemptyslots == false)
                            {
                                if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34])
                                {
                                    CurrentBlueprintType = BlueprintType.NULL;
                                    ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                                    ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
                                    ResSlotByte = null;

                                    if (InstantiatedBlueprint != null)
                                    {
                                        Destroy(InstantiatedBlueprint);
                                        CorrectAnswerChosen = false;
                                    }
                                }
                            }
                            else
                            {
                                //Planks
                                if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[14])
                                {
                                    if (InstantiatedBlueprint == null)
                                    {
                                        CorrectAnswerChosen = false;
                                        InstantiatedBlueprint = Instantiate(OtherBluePrints[0], new Vector3(0, 0, 0), Quaternion.identity);
                                        InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                        InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                        InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                        InstantiatedPrintIndex = 0;
                                        Buttons.Clear();
                                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);
                                        Buttons[1].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                                        ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                                    }
                                    else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 0)
                                    {
                                        CorrectAnswerChosen = false;
                                        Destroy(InstantiatedBlueprint);
                                        InstantiatedBlueprint = Instantiate(OtherBluePrints[0], new Vector3(0, 0, 0), Quaternion.identity);
                                        InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                        InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                        InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                        InstantiatedPrintIndex = 0;
                                        Buttons.Clear();
                                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                                        Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);
                                        Buttons[1].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                                        ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                                    }
                                    ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[14];
                                    ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "4";
                                    ResSlotByte = 15;
                                }
                            }
                        }
                    }
                }
            }
            else if (Inventory.transform.Find("CraftTableGrid").gameObject.activeSelf)
            {
                if (CraftGridBytes[0, 1] == 15)
                {
                 
                    //Stick
                    if ((CraftGridBytes[0, 0] == 15 || CraftGridBytes[0, 2] == 15) && CraftGridBytes[1, 0] == 34 && CraftGridBytes[1, 1] == 34 && CraftGridBytes[1, 2] == 34 && CraftGridBytes[2, 0] == 34 && CraftGridBytes[2, 1] == 34 && CraftGridBytes[2, 2] == 34)
                    {
                        if (InstantiatedBlueprint == null)
                        {
                            CorrectAnswerChosen = false;
                            InstantiatedBlueprint = Instantiate(OtherBluePrints[1], new Vector3(0, 0, 0), Quaternion.identity);
                            InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                            InstantiatedPrintIndex = 1;
                            Buttons.Clear();
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);
                            Buttons[0].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                            CurrentBlueprintType = BlueprintType.Buttons;
                        }
                        else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 1)
                        {
                            CorrectAnswerChosen = false;
                            print("Switching Blueprints");
                            Destroy(InstantiatedBlueprint);
                            InstantiatedBlueprint = Instantiate(OtherBluePrints[1], new Vector3(0, 0, 0), Quaternion.identity);
                            InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                            InstantiatedPrintIndex = 1;
                            Buttons.Clear();
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);

                            Buttons[0].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                            CurrentBlueprintType = BlueprintType.Buttons;
                        }

                        if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[36])
                        {
                            ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[36];
                            ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "4";
                            ResSlotByte = 36;
                            ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                        }
                    }
                    else
                    {
                        //Crafting Bench
                        //TopLeft
                        if (CraftGridBytes[0, 0] == 15 && CraftGridBytes[1, 0] == 15 && CraftGridBytes[1, 1] == 15 && CraftGridBytes[0, 2] == 34 && CraftGridBytes[1, 2] == 34 && CraftGridBytes[2, 0] == 34 && CraftGridBytes[2, 1] == 34 && CraftGridBytes[2, 2] == 34)
                        {
                            if (InstantiatedBlueprint == null)
                            {
                                CorrectAnswerChosen = false;
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }
                            else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 2)
                            {
                                CorrectAnswerChosen = false;
                                Destroy(InstantiatedBlueprint);
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }

                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35])
                            {
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "1";
                                ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                                ResSlotByte = 35;
                            }
                        }
                        //Bottom Left
                        else if (CraftGridBytes[0, 2] == 15 && CraftGridBytes[1, 1] == 15 && CraftGridBytes[1, 2] == 15 && CraftGridBytes[0, 0] == 34 && CraftGridBytes[1, 0] == 34 && CraftGridBytes[2, 0] == 34 && CraftGridBytes[2, 1] == 34 && CraftGridBytes[2, 2] == 34)
                        {
                            if (InstantiatedBlueprint == null)
                            {
                                CorrectAnswerChosen = false;
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }
                            else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 2)
                            {
                                CorrectAnswerChosen = false;
                                Destroy(InstantiatedBlueprint);
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }

                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35])
                            {
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "1";
                                ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                                ResSlotByte = 35;
                            }
                        }
                        else
                        {
                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34])
                            {
                                CurrentBlueprintType = BlueprintType.NULL;
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
                                ResSlotByte = null;

                                if (InstantiatedBlueprint != null)
                                {
                                    Destroy(InstantiatedBlueprint);
                                    CorrectAnswerChosen = false;
                                }
                            }
                        }
                    }
                }
                else if (CraftGridBytes[1, 1] == 15)
                {
                    //Stick
                    if ((CraftGridBytes[1, 0] == 15 || CraftGridBytes[1, 2] == 15) && CraftGridBytes[0, 0] == 34 && CraftGridBytes[0, 1] == 34 && CraftGridBytes[0, 2] == 34 && CraftGridBytes[2, 0] == 34 && CraftGridBytes[2, 1] == 34 && CraftGridBytes[2, 2] == 34)
                    {
                        if (InstantiatedBlueprint == null)
                        {
                            CorrectAnswerChosen = false;
                            InstantiatedBlueprint = Instantiate(OtherBluePrints[1], new Vector3(0, 0, 0), Quaternion.identity);
                            InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                            InstantiatedPrintIndex = 1;
                            Buttons.Clear();
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);
                            Buttons[0].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                            CurrentBlueprintType = BlueprintType.Buttons;
                        }
                        else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 1)
                        {
                            CorrectAnswerChosen = false;
                            print("Switching Blueprints");
                            Destroy(InstantiatedBlueprint);
                            InstantiatedBlueprint = Instantiate(OtherBluePrints[1], new Vector3(0, 0, 0), Quaternion.identity);
                            InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                            InstantiatedPrintIndex = 1;
                            Buttons.Clear();
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);

                            Buttons[0].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                            CurrentBlueprintType = BlueprintType.Buttons;
                        }

                        if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[36])
                        {
                            ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[36];
                            ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "4";
                            ResSlotByte = 36;
                            ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                        }
                    }
                    else
                    {
                        //Crafting Bench
                        //Top Left
                        if (CraftGridBytes[0, 0] == 15 && CraftGridBytes[1, 0] == 15 && CraftGridBytes[0, 1] == 15 && CraftGridBytes[0, 2] == 34 && CraftGridBytes[1, 2] == 34 && CraftGridBytes[2, 0] == 34 && CraftGridBytes[2, 1] == 34 && CraftGridBytes[2, 2] == 34)
                        {
                            if (InstantiatedBlueprint == null)
                            {
                                CorrectAnswerChosen = false;
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }
                            else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 2)
                            {
                                CorrectAnswerChosen = false;
                                Destroy(InstantiatedBlueprint);
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }

                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35])
                            {
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "1";
                                ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                                ResSlotByte = 35;
                            }
                        }
                        //Top Right
                        else if (CraftGridBytes[2, 0] == 15 && CraftGridBytes[2, 1] == 15 && CraftGridBytes[1, 0] == 15 && CraftGridBytes[0, 0] == 34 && CraftGridBytes[0, 1] == 34 && CraftGridBytes[0, 2] == 34 && CraftGridBytes[1, 2] == 34 && CraftGridBytes[2, 2] == 34)
                        {
                            if (InstantiatedBlueprint == null)
                            {
                                CorrectAnswerChosen = false;
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }
                            else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 2)
                            {
                                CorrectAnswerChosen = false;
                                Destroy(InstantiatedBlueprint);
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }

                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35])
                            {
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "1";
                                ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                                ResSlotByte = 35;
                            }
                        }
                        //Bottom Left
                        else if (CraftGridBytes[0, 2] == 15 && CraftGridBytes[0, 1] == 15 && CraftGridBytes[1, 2] == 15 && CraftGridBytes[0, 0] == 34 && CraftGridBytes[1, 0] == 34 && CraftGridBytes[2, 0] == 34 && CraftGridBytes[2, 1] == 34 && CraftGridBytes[2, 2] == 34)
                        {
                            if (InstantiatedBlueprint == null)
                            {
                                CorrectAnswerChosen = false;
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }
                            else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 2)
                            {
                                CorrectAnswerChosen = false;
                                Destroy(InstantiatedBlueprint);
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }

                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35])
                            {
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "1";
                                ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                                ResSlotByte = 35;
                            }
                        }
                        //Bottom Right
                        else if (CraftGridBytes[2, 2] == 15 && CraftGridBytes[2, 1] == 15 && CraftGridBytes[1, 2] == 15 && CraftGridBytes[0, 0] == 34 && CraftGridBytes[0, 1] == 34 && CraftGridBytes[0, 2] == 34 && CraftGridBytes[1, 0] == 34 && CraftGridBytes[2, 0] == 34)
                        {
                            if (InstantiatedBlueprint == null)
                            {
                                CorrectAnswerChosen = false;
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }
                            else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 2)
                            {
                                CorrectAnswerChosen = false;
                                Destroy(InstantiatedBlueprint);
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }

                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35])
                            {
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "1";
                                ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                                ResSlotByte = 35;
                            }
                        }
                        else
                        {
                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34])
                            {
                                CurrentBlueprintType = BlueprintType.NULL;
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
                                ResSlotByte = null;

                                if (InstantiatedBlueprint != null)
                                {
                                    Destroy(InstantiatedBlueprint);
                                    CorrectAnswerChosen = false;
                                }
                            }
                        }
                    }
                }
                else if (CraftGridBytes[2, 1] == 15)
                {
                    //Stick
                    if ((CraftGridBytes[2, 0] == 15 || CraftGridBytes[2, 2] == 15) && CraftGridBytes[0, 0] == 34 && CraftGridBytes[0, 1] == 34 && CraftGridBytes[0, 2] == 34 && CraftGridBytes[1, 0] == 34 && CraftGridBytes[1, 1] == 34 && CraftGridBytes[1, 2] == 34)
                    {
                        if (InstantiatedBlueprint == null)
                        {
                            CorrectAnswerChosen = false;
                            InstantiatedBlueprint = Instantiate(OtherBluePrints[1], new Vector3(0, 0, 0), Quaternion.identity);
                            InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                            InstantiatedPrintIndex = 1;
                            Buttons.Clear();
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);
                            Buttons[0].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                            CurrentBlueprintType = BlueprintType.Buttons;
                        }
                        else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 1)
                        {
                            CorrectAnswerChosen = false;
                            print("Switching Blueprints");
                            Destroy(InstantiatedBlueprint);
                            InstantiatedBlueprint = Instantiate(OtherBluePrints[1], new Vector3(0, 0, 0), Quaternion.identity);
                            InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                            InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                            InstantiatedPrintIndex = 1;
                            Buttons.Clear();
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                            Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);

                            Buttons[0].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                            CurrentBlueprintType = BlueprintType.Buttons;
                        }

                        if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[36])
                        {
                            ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[36];
                            ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "4";
                            ResSlotByte = 36;
                            ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                        }
                    }
                    else
                    {
                        //Crafting Bench
                        //Top Right
                        if (CraftGridBytes[2, 0] == 15 && CraftGridBytes[1, 0] == 15 && CraftGridBytes[1, 1] == 15 && CraftGridBytes[0, 0] == 34 && CraftGridBytes[0, 1] == 34 && CraftGridBytes[0, 2] == 34 && CraftGridBytes[1, 2] == 34 && CraftGridBytes[2, 2] == 34)
                        {
                            if (InstantiatedBlueprint == null)
                            {
                                CorrectAnswerChosen = false;
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }
                            else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 2)
                            {
                                CorrectAnswerChosen = false;
                                Destroy(InstantiatedBlueprint);
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }

                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35])
                            {
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "1";
                                ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                                ResSlotByte = 35;
                            }
                        }
                        //Bottom Right
                        else if (CraftGridBytes[2, 2] == 15 && CraftGridBytes[1, 1] == 15 && CraftGridBytes[1, 2] == 15 && CraftGridBytes[0, 0] == 34 && CraftGridBytes[0, 1] == 34 && CraftGridBytes[0, 2] == 34 && CraftGridBytes[1, 2] == 34 && CraftGridBytes[2, 2] == 34)
                        {
                            if (InstantiatedBlueprint == null)
                            {
                                CorrectAnswerChosen = false;
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }
                            else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 2)
                            {
                                CorrectAnswerChosen = false;
                                Destroy(InstantiatedBlueprint);
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 2;
                                CorrectAnswers.Clear();
                                CorrectAnswers.Add(false);
                                CorrectAnswers.Add(false);
                                CurrentBlueprintType = BlueprintType.FillIn;
                            }

                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35])
                            {
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[35];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "1";
                                ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                                ResSlotByte = 35;
                            }
                        }
                        else
                        {
                            if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34])
                            {
                                CurrentBlueprintType = BlueprintType.NULL;
                                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                                ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
                                ResSlotByte = null;

                                if (InstantiatedBlueprint != null)
                                {
                                    Destroy(InstantiatedBlueprint);
                                    CorrectAnswerChosen = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Plank
                    int ifound = 999;
                    int jfound = 999;
                    bool FoundWood = false;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (CraftGridBytes[i, j] == 13)
                            {
                                FoundWood = true;
                                ifound = i;
                                jfound = j;
                                break;
                            }
                        }

                        if (FoundWood)
                        {
                            break;
                        }
                    }

                    bool checkforemptyslots = true;
                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            if (ifound != k && jfound != l)
                            {
                                if (CraftGridBytes[k, l] != 34)
                                {
                                    checkforemptyslots = false;
                                }
                            }
                        }
                    }

                    if (checkforemptyslots == true)
                    {
                        if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[14])
                        {
                            if (InstantiatedBlueprint == null)
                            {
                                CorrectAnswerChosen = false;
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[0], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 0;
                                Buttons.Clear();
                                Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                                Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                                Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);
                                Buttons[1].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                                ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                            }
                            else if (InstantiatedBlueprint != null && InstantiatedPrintIndex != 0)
                            {
                                CorrectAnswerChosen = false;
                                Destroy(InstantiatedBlueprint);
                                InstantiatedBlueprint = Instantiate(OtherBluePrints[0], new Vector3(0, 0, 0), Quaternion.identity);
                                InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                                InstantiatedBlueprint.GetComponent<RectTransform>().localScale = Vector3.one;
                                InstantiatedPrintIndex = 0;
                                Buttons.Clear();
                                Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(0).gameObject);
                                Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(1).gameObject);
                                Buttons.Add(InstantiatedBlueprint.transform.GetChild(0).GetChild(2).gameObject);
                                Buttons[1].GetComponent<Button>().onClick.AddListener(CorrectAnswer);
                                ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                            }
                            ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[14];
                            ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "4";
                            ResSlotByte = 15;
                        }
                    }
                    else
                    {
                        if (ResSlot.transform.GetChild(0).GetComponent<Image>().sprite != GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34])
                        {
                            CurrentBlueprintType = BlueprintType.NULL;
                            ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                            ResSlot.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
                            ResSlotByte = null;

                            if (InstantiatedBlueprint != null)
                            {
                                Destroy(InstantiatedBlueprint);
                                CorrectAnswerChosen = false;
                            }
                        }
                    }
                }
                UnityEngine.Debug.ClearDeveloperConsole();
            }
            else if (!Inventory.activeSelf && InitializeCraftGrid)
            {
                CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                ResSlot.transform.GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[34];
                CraftGrid = null;
                ResSlot = null;
                ResSlotByte = null;
                InitializeCraftGrid = false;
            }
        }
    }







    public void DestroyBlueprint() { 
        if(InstantiatedBlueprint != null)
        {
            print("Destroying blueprint..");
            Destroy(InstantiatedBlueprint);
            InstantiatedPrintIndex = 999;
        }
    }



    public void SetInvenCraftSlot(int row, int column, byte item)
    {
        if(row == 0)
        {
            if(column == 0)
            {
                CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
            else if(column == 1)
            {
                CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
        }
        else if(row == 1)
        {
            if (column == 0)
            {
                CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
            else if (column == 1)
            {
                CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
        }
    }

    public void SetCraftTableSlot(int row, int column, byte item)
    {
        if (row == 0)
        {
            if (column == 0)
            {
                CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
            else if (column == 1)
            {
                CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
            else if (column == 1)
            {
                CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
        }
        else if (row == 1)
        {
            if (column == 0)
            {
                CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
            else if (column == 1)
            {
                CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
            else if (column == 2)
            {
                CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
        }
        else if (row == 2)
        {
            if (column == 0)
            {
                CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
            else if (column == 1)
            {
                CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
            else if (column == 2)
            {
                CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetComponent<Image>().sprite = GameObject.Find("player").GetComponent<PlayerIO>().spritesByBlockID[item];
            }
        }
    }

    public void CorrectAnswer()
    {
        CorrectAnswerChosen = true;
        if (ResSlotByte == 36 && InstantiatedPrintIndex == 1)
        {
            Buttons[0].GetComponent<Image>().color = Color.green;
        }
        else if(ResSlotByte == 15 && InstantiatedPrintIndex == 0)
        {
            Buttons[1].GetComponent<Image>().color = Color.green;
        }
        ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.white;
    }

    public void InCorrectAnswer()
    {
        CorrectAnswerChosen = false;
        if (ResSlotByte == 36 && InstantiatedPrintIndex == 1)
        {
            Buttons[0].GetComponent<Image>().color = Color.white;
        }
        else if (ResSlotByte == 15 && InstantiatedPrintIndex == 0)
        {
            Buttons[1].GetComponent<Image>().color = Color.white;
        }
        ResSlot.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
    }
}

public enum BlueprintType
{
    NULL,
    FillIn,
    Buttons
}

/*
 *  if (InvenBytes[0, 1] == 13 || InvenBytes[0,1] == 4 || InvenBytes[0,1] == 53)
            {
                //Shovel
                if(InvenBytes[1, 1] == 36 && InvenBytes[2, 1] == 36 && InvenBytes[0, 0] == 34 && InvenBytes[0, 2] == 34 && InvenBytes[1, 0] == 34 && InvenBytes[1, 2] == 34 && InvenBytes[2, 0] == 34 && InvenBytes[2, 2] == 34)
                {
                    //Wood
                    if(InvenBytes[0,1] == 13)
                    {
                        InstantiatedBlueprint = Instantiate(WoodToolBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                    }
                    //Stone
                    else if(InvenBytes[0,1] == 4)
                    {
                        InstantiatedBlueprint = Instantiate(StoneToolBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                    }
                    //Iron
                    else if(InvenBytes[0,1] == 53)
                    {
                        InstantiatedBlueprint = Instantiate(IronToolBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                    }
                    InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                    InstantiatedBlueprint.transform.position = new Vector3(0, 0, 0);
                }
                //Wood Axe
                else if(InvenBytes[0, 1] == 13 && ( (InvenBytes[0,0] == 13 && InvenBytes[0,1] == 13) || (InvenBytes[0,2] == 13 && InvenBytes[1,2] == 13)  )   )
                {
                    if (InvenBytes[0,2] == 34 && InvenBytes[2,0] == 34 && InvenBytes[2,1] == 34 && InvenBytes[2, 2] == 34)
                    {
                        InstantiatedBlueprint = Instantiate(WoodToolBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                        InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                        InstantiatedBlueprint.transform.position = new Vector3(0, 0, 0);
                    }
                }
                //Stone Axe
                else if(InvenBytes[0, 1] == 4 && (    (InvenBytes[0, 0] == 4 && InvenBytes[0, 1] == 4) || (InvenBytes[0, 2] == 4 && InvenBytes[1, 2] == 4)    )     )
                {
                    if (InvenBytes[0, 2] == 34 && InvenBytes[2, 0] == 34 && InvenBytes[2, 1] == 34 && InvenBytes[2, 2] == 34)
                    {
                        InstantiatedBlueprint = Instantiate(StoneToolBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                        InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                        InstantiatedBlueprint.transform.position = new Vector3(0, 0, 0);
                    }
                }
                //Iron Axe
                else if ( InvenBytes[0,1] == 53 &&  (    (InvenBytes[0, 0] == 53 && InvenBytes[0, 1] == 53) || (InvenBytes[0, 2] == 53 && InvenBytes[1, 2] == 53)   )    )
                {
                    if (InvenBytes[0, 2] == 34 && InvenBytes[2, 0] == 34 && InvenBytes[2, 1] == 34 && InvenBytes[2, 2] == 34)
                    {
                        if(InstantiatedBlueprint != null)
                        {
                            Destroy(InstantiatedBlueprint);
                        }
                        InstantiatedBlueprint = Instantiate(StoneToolBluePrints[2], new Vector3(0, 0, 0), Quaternion.identity);
                        InstantiatedBlueprint.transform.SetParent(CodeSpace.transform);
                        InstantiatedBlueprint.transform.position = new Vector3(0, 0, 0);
                    }
                }
                //Wood Sword
                else if(InvenBytes[0,1] == 13 && InvenBytes[1,1] == 13 && InvenBytes[2,1] == 36)
                {

                }
                else
                {
                    if(InstantiatedBlueprint != null)
                    {
                        Destroy(InstantiatedBlueprint);
                    }
                }
            }
*/