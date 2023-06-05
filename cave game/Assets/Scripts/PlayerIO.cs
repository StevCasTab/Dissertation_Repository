using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerIO : MonoBehaviour
{
	public float maxReachDist = 5;
	public GameObject retDel;
	public GameObject retAdd;

	public GameObject blockholderPrefab;
	public GameObject InstantiatedBlockHolder;
	public GameObject[] InventoryRows;

	World world;

	public string[] blockSounds;

	public byte[] InvRow1, InvRow2, InvRow3 = new byte[9];
	public byte? BlockHolderbyte;
	public int[] hotbarBlocksAmount = new int[9];
	public byte[] hotbarBlocks = new byte[9];
	public float[] indicatorXPositions = new float[9];


	public bool MoveToPos = false;
	public Vector3 MoveToPosCoords;

	public Transform indicator;

	public int currentSlot = 0;
	private string PrevSlot;
	public Sprite[] spritesByBlockID;
	public Image[] hotbarBlockSprites = new Image[9];

	public GameObject inventory;

	GraphicRaycaster m_Raycaster;
	PointerEventData m_PointerEventData;
	EventSystem m_EventSystem;


	void Start()
	{
		InvRow1 = new byte[9];

		for(int i = 0; i < InvRow1.Length; i++)
        {
			InvRow1[i] = 34;
        }
		InvRow2 = new byte[9];
		for (int i = 0; i < InvRow2.Length; i++)
		{
			InvRow2[i] = 34;
		}
		InvRow3 = new byte[9];
		for (int i = 0; i < InvRow3.Length; i++)
		{
			InvRow3[i] = 34;
		}
		//Fetch the Raycaster from the GameObject (the Canvas)
		m_Raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
		//Fetch the Event System from the Scene
		m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		world = World.currentWorld;
	}

	void Update()
	{
		if (GetComponent<PlayerController>().controlsEnabled)
		{
			if (MoveToPos)
			{
				MoveToPos = false;
				GetComponent<CharacterController>().enabled = false;
				transform.position = MoveToPosCoords;
				GetComponent<CharacterController>().enabled = true;
			}
			if (world == null) return;

			if (Input.GetKeyUp(KeyCode.Escape))
			{
				Invoke("WorkAroundForUnitysStupidMouseHidingSystemLikeWhatTheHell", 0.1f);
			}

			if (this.gameObject.transform.GetChild(0).gameObject.activeSelf)
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
				if (Physics.Raycast(ray, out hit, maxReachDist) && hit.collider.tag == "Chunk" && !inventory.activeSelf && !PauseMenu.pauseMenu.paused)
				{
					Vector3 p = hit.point - hit.normal / 2;
					Vector3 p2 = hit.point + hit.normal / 2;

					float delX = Mathf.Floor(p.x) + 0.5f;
					float delY = Mathf.Floor(p.y) + 0.5f;
					float delZ = Mathf.Floor(p.z) + 0.5f;

					float addX = Mathf.Floor(p2.x) + 0.5f;
					float addY = Mathf.Floor(p2.y) + 0.5f;
					float addZ = Mathf.Floor(p2.z) + 0.5f;

					p = new Vector3(delX, delY, delZ);
					p2 = new Vector3(addX, addY, addZ);

					int blockDelX = (int)(delX - 0.5f);
					int blockDelY = (int)(delY - 0.5f);
					int blockDelZ = (int)(delZ - 0.5f) + 1;

					int blockAddX = (int)(addX - 0.5f);
					int blockAddY = (int)(addY - 0.5f);
					int blockAddZ = (int)(addZ - 0.5f) + 1;

					bool delSwitch = false;

					retDel.SetActive(true);
					if (world.GetBlock(blockAddX, blockAddY, blockAddZ) >= 29)
					{
						retDel.transform.position = p2;
						delSwitch = true;
					}
					else
						retDel.transform.position = p;

					retAdd.transform.position = p2;

					if (Input.GetMouseButtonDown(0) && GameObject.Find("Canvas").transform.Find("CodeImage").gameObject.activeSelf == false)
					{
						if (delSwitch)
						{
							blockDelX = blockAddX;
							blockDelY = blockAddY;
							blockDelZ = blockAddZ;
						}

						int b = world.world[blockDelX, blockDelY, blockDelZ];
						if (b == 0) b++;

						world.PlaceBlock(blockDelX, blockDelY, blockDelZ, 0);

						if (world.world[blockDelX, blockDelY, blockDelZ] == 0)
							SoundManager.PlayAudio(blockSounds[b - 1] + UnityEngine.Random.Range(1, 5).ToString(), 0.2f, UnityEngine.Random.Range(0.9f, 1.1f));

						Chunk chunk = hit.collider.gameObject.GetComponent<Chunk>();
						if (chunk == null) print(hit.transform.gameObject.name);

						int cX = blockDelX - (int)chunk.transform.position.x;
						int cY = blockDelY - (int)chunk.transform.position.y;
						int cZ = blockDelZ - (int)chunk.transform.position.z;

						Vector3Int cPos = new Vector3Int(Mathf.FloorToInt(blockDelX / 16f),
							Mathf.FloorToInt(blockDelY / 16f), Mathf.FloorToInt(blockDelZ / 16f));

						if (cX == 0)
						{
							if (world.ChunkIsWithinBounds(cPos.x - 1, cPos.y, cPos.z))
							{
								if (world.ChunkExistsAt(cPos.x - 1, cPos.y, cPos.z))
									world.chunks[cPos.x - 1, cPos.y, cPos.z].Regenerate();
								else
									world.ForceLoadChunkAt(cPos.x - 1, cPos.y, cPos.z);
							}
						}
						else if (cX == world.chunkSize - 1)
						{
							if (world.ChunkIsWithinBounds(cPos.x + 1, cPos.y, cPos.z))
							{
								if (world.ChunkExistsAt(cPos.x + 1, cPos.y, cPos.z))
									world.chunks[cPos.x + 1, cPos.y, cPos.z].Regenerate();
								else
									world.ForceLoadChunkAt(cPos.x + 1, cPos.y, cPos.z);
							}
						}

						if (cY == world.chunkSize - 1)
						{
							if (world.ChunkIsWithinBounds(cPos.x, cPos.y + 1, cPos.z))
							{
								if (world.ChunkExistsAt(cPos.x, cPos.y + 1, cPos.z))
									world.chunks[cPos.x, cPos.y + 1, cPos.z].Regenerate();
								else
									world.ForceLoadChunkAt(cPos.x, cPos.y + 1, cPos.z);
							}
						}

						if (cZ == 0)
						{
							if (world.ChunkIsWithinBounds(cPos.x, cPos.y, cPos.z - 1))
							{
								if (world.ChunkExistsAt(cPos.x, cPos.y, cPos.z - 1))
									world.chunks[cPos.x, cPos.y, cPos.z - 1].Regenerate();
								else
									world.ForceLoadChunkAt(cPos.x, cPos.y, cPos.z - 1);
							}
						}
						else if (cZ == world.chunkSize - 1)
						{
							if (world.ChunkIsWithinBounds(cPos.x, cPos.y, cPos.z + 1))
							{
								if (world.ChunkExistsAt(cPos.x, cPos.y, cPos.z + 1))
									world.chunks[cPos.x, cPos.y, cPos.z + 1].Regenerate();
								else
									world.ForceLoadChunkAt(cPos.x, cPos.y, cPos.z + 1);
							}
						}

						for (int y = cPos.y; y >= 0; y--)
						{
							if (world.ChunkExistsAt(cPos.x, y, cPos.z))
								world.chunks[cPos.x, y, cPos.z].Regenerate();
							else
								world.ForceLoadChunkAt(cPos.x, y, cPos.z);
						}

						chunk.Regenerate();
					}
					if (Input.GetMouseButtonDown(1) && GameObject.Find("Canvas").transform.Find("CodeImage").gameObject.activeSelf == false)
					{
						byte newBlock = hotbarBlocks[currentSlot];

						if (newBlock != 34)
						{
							if (!retAdd.GetComponent<AddReticule>().touchingPlayer)
							{

								world.PlaceBlock(blockAddX, blockAddY, blockAddZ, newBlock);

								SoundManager.PlayAudio(blockSounds[newBlock - 1] + UnityEngine.Random.Range(1, 5).ToString(), 0.2f, UnityEngine.Random.Range(0.9f, 1.1f));

								Chunk chunk = hit.collider.gameObject.GetComponent<Chunk>();

								int cX = blockDelX - (int)chunk.transform.position.x;
								int cY = blockDelY - (int)chunk.transform.position.y;
								int cZ = blockDelZ - (int)chunk.transform.position.z;

								Vector3Int cPos = new Vector3Int(Mathf.FloorToInt(blockDelX / 16f),
									Mathf.FloorToInt(blockDelY / 16f), Mathf.FloorToInt(blockDelZ / 16f));

								if (cX == 0)
								{
									if (world.ChunkIsWithinBounds(cPos.x - 1, cPos.y, cPos.z))
									{
										if (world.ChunkExistsAt(cPos.x - 1, cPos.y, cPos.z))
											world.chunks[cPos.x - 1, cPos.y, cPos.z].Regenerate();
										else
											world.ForceLoadChunkAt(cPos.x - 1, cPos.y, cPos.z);
									}
								}
								else if (cX == world.chunkSize - 1)
								{
									if (world.ChunkIsWithinBounds(cPos.x + 1, cPos.y, cPos.z))
									{
										if (world.ChunkExistsAt(cPos.x + 1, cPos.y, cPos.z))
											world.chunks[cPos.x + 1, cPos.y, cPos.z].Regenerate();
										else
											world.ForceLoadChunkAt(cPos.x + 1, cPos.y, cPos.z);
									}
								}

								if (cY == world.chunkSize - 1)
								{
									if (world.ChunkIsWithinBounds(cPos.x, cPos.y + 1, cPos.z))
									{
										if (world.ChunkExistsAt(cPos.x, cPos.y + 1, cPos.z))
											world.chunks[cPos.x, cPos.y + 1, cPos.z].Regenerate();
										else
											world.ForceLoadChunkAt(cPos.x, cPos.y + 1, cPos.z);
									}
								}

								if (cZ == 0)
								{
									if (world.ChunkIsWithinBounds(cPos.x, cPos.y, cPos.z - 1))
									{
										if (world.ChunkExistsAt(cPos.x, cPos.y, cPos.z - 1))
											world.chunks[cPos.x, cPos.y, cPos.z - 1].Regenerate();
										else
											world.ForceLoadChunkAt(cPos.x, cPos.y, cPos.z - 1);
									}
								}
								else if (cZ == world.chunkSize - 1)
								{
									if (world.ChunkIsWithinBounds(cPos.x, cPos.y, cPos.z + 1))
									{
										if (world.ChunkExistsAt(cPos.x, cPos.y, cPos.z + 1))
											world.chunks[cPos.x, cPos.y, cPos.z + 1].Regenerate();
										else
											world.ForceLoadChunkAt(cPos.x, cPos.y, cPos.z + 1);
									}
								}

								for (int y = cPos.y; y >= 0; y--)
								{
									if (world.ChunkExistsAt(cPos.x, y, cPos.z))
										world.chunks[cPos.x, y, cPos.z].Regenerate();
									else
										world.ForceLoadChunkAt(cPos.x, y, cPos.z);
								}

								chunk.Regenerate();

								hotbarBlocksAmount[currentSlot]--;
							}
						}
					}
					/*if (Input.GetMouseButtonDown(2))
					{
						int b = world.GetBlock(blockDelX, blockDelY, blockDelZ);
						if(delSwitch) b = world.GetBlock(blockAddX, blockAddY, blockAddZ);

						if (!HotbarContainsBlock((byte)b))
						{
							if (b != 0 && b != 4 && (b < 8 || b > 12))
								SetHotbarBlock(b);
						}
						else
						{
							currentSlot = GetHotbarSlotWith(b);
						}
					}*/
				}
				else
				{
					retDel.SetActive(false);
				}
			}

			/*if (Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				if (currentSlot == 8) currentSlot = 0;
				else currentSlot++;
			}
			if (Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				if (currentSlot == 0) currentSlot = 8;
				else currentSlot--;
			}*/
			if (GetComponent<PlayerController>().controlsEnabled == true && Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				if (currentSlot == 8) currentSlot = 0;
				else currentSlot++;
			}
			if (GetComponent<PlayerController>().controlsEnabled == true && Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				if (currentSlot == 0) currentSlot = 8;
				else currentSlot--;
			}


			//for(int i = 0; i < hotbarBlockSprites.Length; i++)
			//hotbarBlockSprites[i].sprite = spritesByBlockID[hotbarBlocks[i] - 1];

			if (world.worldInitialized)
			{
				int index = 999;
				int blocknum = 999;
				for (int i = 0; i < hotbarBlockSprites.Length; i++)
				{
					try
					{
						index = i;
						blocknum = hotbarBlocks[i] - 1;
						hotbarBlockSprites[i].sprite = spritesByBlockID[hotbarBlocks[i] - 1];
						if (hotbarBlocksAmount[i] == 0)
						{
							hotbarBlockSprites[i].gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
							hotbarBlockSprites[i].sprite = spritesByBlockID[33];
							hotbarBlocks[i] = 34;
						}
						else
						{
							hotbarBlockSprites[i].gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = hotbarBlocksAmount[i].ToString();
						}
					}
					catch (System.IndexOutOfRangeException e)
					{
						print(index + " : " + blocknum);
					}
				}
					
			}

			indicator.localPosition = new Vector3
				(indicatorXPositions[currentSlot],
				indicator.localPosition.y,
				indicator.localPosition.z);

			if (GetComponent<PlayerController>().controlsEnabled == true)
			{
				/*if (Input.GetKeyDown(KeyCode.E))
				{
					if (inventory.activeSelf)
					{
						GetComponent<PlayerController>().controlsEnabled = true;
						transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click");
						inventory.SetActive(false);
					}
					else { inventory.SetActive(true); GetComponent<PlayerController>().controlsEnabled = false; }
				}*/

				if (inventory.activeSelf && InstantiatedBlockHolder != null)
				{
					print("clicked with inventory open");
					if (Input.GetMouseButtonDown(0))
					{
						//Set up the new Pointer Event
						m_PointerEventData = new PointerEventData(m_EventSystem);
						//Set the Pointer Event Position to that of the mouse position
						m_PointerEventData.position = Input.mousePosition;

						//Create a list of Raycast Results
						List<RaycastResult> results = new List<RaycastResult>();

						//Raycast using the Graphics Raycaster and mouse click position
						m_Raycaster.Raycast(m_PointerEventData, results);

						//For every result returned, output the name of the GameObject on the Canvas hit by the Ray
						foreach (RaycastResult res in results)
						{
							print(res);
							if (res.gameObject.name == "hotbar")
							{
								HandleHotbarSlotSelected(results, Click.LeftClick);
								break;
							}
							else if (res.gameObject.name == "hotbarRow1" || res.gameObject.name == "hotbarRow1 (1)" || res.gameObject.name == "hotbarRow1 (2)")
							{
								HandleInventorySlotSelected(results, Click.LeftClick);
							}
							/*else if (inventory.transform.Find("CraftGrid").gameObject.activeSelf && (res.gameObject.name == "CraftSlot 1" || res.gameObject.name == "CraftSlot 2" || res.gameObject.name == "CraftSlot 3" || res.gameObject.name == "CraftSlot 4"))
							{
								HandleInvenCraftGrid(results, Click.LeftClick);
							}*/
							/*else if (inventory.transform.Find("CraftTableGrid").gameObject.activeSelf && (res.gameObject.name == "CraftSlot 1" || res.gameObject.name == "CraftSlot 2" || res.gameObject.name == "CraftSlot 3" || res.gameObject.name == "CraftSlot 4" || res.gameObject.name == "CraftSlot 5" || res.gameObject.name == "CraftSlot 6" || res.gameObject.name == "CraftSlot 7" || res.gameObject.name == "CraftSlot 8" || res.gameObject.name == "CraftSlot 9"))
							{
								HandleCTableGrid(results, Click.LeftClick);
							}*/
							else if (res.gameObject.name == "ResSlot")
							{
								print("Clicked on ResSlot");
								SelectedBlock("4,4");
							}
						}
					}
					else if (Input.GetMouseButtonDown(1))
					{
						//Set up the new Pointer Event
						m_PointerEventData = new PointerEventData(m_EventSystem);
						//Set the Pointer Event Position to that of the mouse position
						m_PointerEventData.position = Input.mousePosition;

						//Create a list of Raycast Results
						List<RaycastResult> results = new List<RaycastResult>();

						//Raycast using the Graphics Raycaster and mouse click position
						m_Raycaster.Raycast(m_PointerEventData, results);

						//For every result returned, output the name of the GameObject on the Canvas hit by the Ray
						foreach (RaycastResult res in results)
						{
							if (res.gameObject.name == "hotbar")
							{
								HandleHotbarSlotSelected(results, Click.RightClick);
								break;
							}
							else if (res.gameObject.name == "hotbarRow1" || res.gameObject.name == "hotbarRow1 (1)" || res.gameObject.name == "hotbarRow1 (2)")
							{
								HandleInventorySlotSelected(results, Click.RightClick);
							}
							/*else if (inventory.transform.Find("CraftGrid").gameObject.activeSelf && (res.gameObject.name == "CraftSlot 1" || res.gameObject.name == "CraftSlot 2" || res.gameObject.name == "CraftSlot 3" || res.gameObject.name == "CraftSlot 4"))
							{
								print("CraftGrid selected");
								HandleInvenCraftGrid(results, Click.RightClick);
							}*/
							/*else if (inventory.transform.Find("CraftTableGrid").gameObject.activeSelf && (res.gameObject.name == "CraftSlot 1" || res.gameObject.name == "CraftSlot 2" || res.gameObject.name == "CraftSlot 3" || res.gameObject.name == "CraftSlot 4" || res.gameObject.name == "CraftSlot 5" || res.gameObject.name == "CraftSlot 6" || res.gameObject.name == "CraftSlot 7" || res.gameObject.name == "CraftSlot 8" || res.gameObject.name == "CraftSlot 9"))
							{
								print("CraftTableGrid selected");
								HandleCTableGrid(results, Click.RightClick);
							}*/
							else if (res.gameObject.name == "ResSlot")
							{
								print("Clicked on ResSlot");
								SelectedBlock("4,4");
							}
						}
					}
				}
			}
		}
	}

	private void HandleHotbarSlotSelected(List<RaycastResult> results, Click mouseclick)
	{
		Debug.Log("Handling Hotbar slot");
		if (InstantiatedBlockHolder != null)
		{
			GameObject hotbar = GameObject.Find("Canvas").transform.Find("hotbar").gameObject;
			List<GameObject> slots = new List<GameObject>();
			bool skipindicator = false;
			foreach (Transform t in hotbar.transform)
			{
				if (!skipindicator)
				{
					skipindicator = true;
				}
				else
				{
					slots.Add(t.gameObject);
				}
			}

			for (int j = 0; j < slots.Count; j++)
			{
				foreach (RaycastResult result2 in results)
				{
					if (slots[j].name == result2.gameObject.name)
					{
						if (hotbarBlocks[j] == 34)
						{
							if (mouseclick == Click.LeftClick)
							{
								hotbarBlocks[j] = BlockHolderbyte.Value;
								BlockHolderbyte = null;
								slots[j].transform.GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
								hotbarBlocksAmount[j] = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);

								Destroy(InstantiatedBlockHolder);
								PrevSlot = null;
								break;
							}
							else if (mouseclick == Click.RightClick)
							{
								int hold = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);

								if (hold == 1)
								{
									hotbarBlocks[j] = BlockHolderbyte.Value;
									BlockHolderbyte = null;
									slots[j].transform.GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
									hotbarBlocksAmount[j] = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);

									Destroy(InstantiatedBlockHolder);
									PrevSlot = null;
									break;
								}
								else if (hold > 1)
								{
									hotbarBlocks[j] = BlockHolderbyte.Value;
									slots[j].transform.GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
									hotbarBlocksAmount[j] = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
									break;
								}
							}
						}
						else
						{
							Sprite temp = slots[j].GetComponent<Image>().sprite;
							string text = slots[j].transform.GetChild(0).GetComponent<TMP_Text>().text;

							hotbarBlocksAmount[j] = 0;

							slots[j].GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							slots[j].transform.GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;

							hotbarBlocksAmount[j] = Convert.ToInt32(slots[j].transform.GetChild(0).GetComponent<TMP_Text>().text);

							InstantiatedBlockHolder.GetComponent<Image>().sprite = temp;
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
							byte holder = hotbarBlocks[j];
							hotbarBlocks[j] = BlockHolderbyte.Value;
							BlockHolderbyte = holder;

							PrevSlot = null;
							break;
						}
					}
				}

				if (InstantiatedBlockHolder == null)
				{
					break;
				}
			}
		}
	}
	private void HandleInventorySlotSelected(List<RaycastResult> results, Click mouseclick)
	{
		Debug.Log("Handling Inventory Slot");
		bool ChangeHappened = false;
		if (InstantiatedBlockHolder != null)
		{
			foreach (RaycastResult result in results)
			{
				for (int i = 0; i < InventoryRows.Length; i++)
				{
					if (!ChangeHappened)
					{
						if (InventoryRows[i].name == result.gameObject.name)
						{
							List<GameObject> ChildList = new List<GameObject>();

							foreach (Transform t in InventoryRows[i].transform)
							{
								ChildList.Add(t.gameObject);
							}

							for (int j = 0; j < ChildList.Count; j++)
							{
								if (!ChangeHappened)
								{
									foreach (RaycastResult result2 in results)
									{

										if (ChildList[j].name == result2.gameObject.name)
										{
											Debug.Log("Inventory " + (i + 1) + " Selected");
											if (i + 1 == 1)
											{
												if (InvRow1[j] == 34)
												{
													if (mouseclick == Click.LeftClick)
													{
														if (ChildList[j].GetComponent<Image>().sprite != InstantiatedBlockHolder.GetComponent<Image>().sprite)
														{
															ChildList[j].GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
															ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
															InvRow1[j] = BlockHolderbyte.Value;
															BlockHolderbyte = null;
															Destroy(InstantiatedBlockHolder);
															ChangeHappened = true;
															PrevSlot = null;
															break;
														}
													}
													else if (mouseclick == Click.RightClick)
													{
														int temp = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
														temp -= 1;
														ChildList[j].GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
														ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = "1";
														InvRow1[j] = BlockHolderbyte.Value;
														ChangeHappened = true;
													}
												}
												else
												{
													if (mouseclick == Click.LeftClick)
													{
														if (ChildList[j].GetComponent<Image>().sprite != InstantiatedBlockHolder.GetComponent<Image>().sprite)
														{
															Sprite temp = ChildList[j].GetComponent<Image>().sprite;
															string text = ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text;

															ChildList[j].GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
															ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;

															InstantiatedBlockHolder.GetComponent<Image>().sprite = temp;
															InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
															byte holder;
															holder = InvRow1[j];
															InvRow1[j] = BlockHolderbyte.Value;
															BlockHolderbyte = holder;
															ChangeHappened = true;
															PrevSlot = null;
															break;
														}
														else if (ChildList[j].GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
														{
															int t1 = Convert.ToInt32(ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text);
															int t2 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
															int t3 = t1 + t2;

															if (t3 <= 64)
															{
																ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();

																Destroy(InstantiatedBlockHolder);
																BlockHolderbyte = null;
																ChangeHappened = true;
																PrevSlot = null;
																break;
															}
														}
													}
													else if (mouseclick == Click.RightClick)
													{
														if (ChildList[j].GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
														{
															int SlotAmount = Convert.ToInt32(ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text);

															SlotAmount += 1;

															ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = SlotAmount.ToString();

															int HolderAmount = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);

															if (HolderAmount - 1 == 0)
															{
																Destroy(InstantiatedBlockHolder);
																PrevSlot = null;
															}
															else
															{
																InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = (HolderAmount - 1).ToString();
															}

															ChangeHappened = true;
															break;
														}
													}
												}
											}
											else if (i + 1 == 2)
											{
												if (InvRow2[j] == 34)
												{
													if (mouseclick == Click.LeftClick)
													{
														ChildList[j].GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
														ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
														InvRow2[j] = BlockHolderbyte.Value;
														BlockHolderbyte = null;
														Destroy(InstantiatedBlockHolder);
														ChangeHappened = true;
														PrevSlot = null;
														break;
													}
													else if (mouseclick == Click.RightClick)
													{
														int temp = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
														temp -= 1;
														ChildList[j].GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
														ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = "1";
														InvRow2[j] = BlockHolderbyte.Value;
														ChangeHappened = true;
														break;
													}
												}
												else
												{
													if (mouseclick == Click.LeftClick)
													{
														if (ChildList[j].GetComponent<Image>().sprite != InstantiatedBlockHolder.GetComponent<Image>().sprite)
														{
															Sprite temp = ChildList[j].GetComponent<Image>().sprite;
															string text = ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text;

															ChildList[j].GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
															ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;

															InstantiatedBlockHolder.GetComponent<Image>().sprite = temp;
															InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
															byte holder;
															holder = InvRow2[j];
															InvRow2[j] = BlockHolderbyte.Value;
															BlockHolderbyte = holder;
															ChangeHappened = true;
															PrevSlot = null;
															break;
														}
														else if (ChildList[j].GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
														{
															int t1 = Convert.ToInt32(ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text);
															int t2 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
															int t3 = t1 + t2;

															if (t3 <= 64)
															{
																ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();

																Destroy(InstantiatedBlockHolder);
																BlockHolderbyte = null;
																ChangeHappened = true;
																PrevSlot = null;
																break;
															}
														}
													}
													else if (mouseclick == Click.RightClick)
													{
														if (ChildList[j].GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
														{
															int SlotAmount = Convert.ToInt32(ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text);

															SlotAmount += 1;

															ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = SlotAmount.ToString();

															int HolderAmount = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);

															if (HolderAmount - 1 == 0)
															{
																Destroy(InstantiatedBlockHolder);
																PrevSlot = null;
															}
															else
															{
																InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = (HolderAmount - 1).ToString();
															}

															ChangeHappened = true;
															break;
														}
													}
												}
											}
											else if (i + 1 == 3)
											{
												if (InvRow3[j] == 34)
												{
													if (mouseclick == Click.LeftClick)
													{
														ChildList[j].GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
														ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
														InvRow3[j] = BlockHolderbyte.Value;
														BlockHolderbyte = null;
														Destroy(InstantiatedBlockHolder);
														ChangeHappened = true;
														PrevSlot = null;
														break;
													}
													else if (mouseclick == Click.RightClick)
													{
														int hold = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
														hold -= 1;
														InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = hold.ToString();
														ChildList[j].GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
														ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = "1";
														InvRow3[j] = BlockHolderbyte.Value;
														ChangeHappened = true;
														break;
													}
												}
												else
												{
													if (mouseclick == Click.LeftClick)
													{
														if (ChildList[j].GetComponent<Image>().sprite != InstantiatedBlockHolder.GetComponent<Image>().sprite)
														{
															Sprite temp = ChildList[j].GetComponent<Image>().sprite;
															string text = ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text;

															ChildList[j].GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
															ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;

															InstantiatedBlockHolder.GetComponent<Image>().sprite = temp;
															InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
															byte holder;
															holder = InvRow3[j];
															InvRow3[j] = BlockHolderbyte.Value;
															BlockHolderbyte = holder;
															ChangeHappened = true;
															PrevSlot = null;
															break;
														}
														else if (ChildList[j].GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
														{
															int t1 = Convert.ToInt32(ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text);
															int t2 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
															int t3 = t1 + t2;

															if (t3 <= 64)
															{
																ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();

																Destroy(InstantiatedBlockHolder);
																BlockHolderbyte = null;
																ChangeHappened = true;
																PrevSlot = null;
																break;
															}
														}
													}
													else if (mouseclick == Click.RightClick)
													{
														if (ChildList[j].GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
														{
															int SlotAmount = Convert.ToInt32(ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text);

															SlotAmount += 1;

															ChildList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = SlotAmount.ToString();

															int HolderAmount = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);

															if (HolderAmount - 1 == 0)
															{
																Destroy(InstantiatedBlockHolder);
																PrevSlot = null;
															}
															else
															{
																InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = (HolderAmount - 1).ToString();
															}

															ChangeHappened = true;
															break;
														}
													}
												}
											}



										}
									}

									if (InstantiatedBlockHolder == null || ChangeHappened)
									{
										break;
									}
								}
								else
								{
									break;
								}
							}
						}
					}
					else
					{
						break;
					}
					if (InstantiatedBlockHolder == null || ChangeHappened)
					{
						break;
					}
				}
				if (InstantiatedBlockHolder == null || ChangeHappened)
				{
					break;
				}
			}
		}
	}
	/*public void HandleInvenCraftGrid(List<RaycastResult> results, Click mouseclick)
	{
		GameObject CraftGrid = inventory.transform.Find("CraftGrid").gameObject;
		if (InstantiatedBlockHolder != null)
		{
			foreach (RaycastResult r in results)
			{
				if (r.gameObject.name == "CraftSlot 1")
				{
					if (mouseclick == Click.LeftClick)
					{
						if (CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
						{
							CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0] = BlockHolderbyte.Value;
							BlockHolderbyte = null;
							Destroy(InstantiatedBlockHolder);
							PrevSlot = null;
						}
						else if (CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
						{
							int t1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
							int t2 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							int t3 = t1 + t2;

							if (t3 <= 64)
							{
								CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();
								Destroy(InstantiatedBlockHolder);
								BlockHolderbyte = null;
							}
						}
						else
						{
							Sprite PrevSprite = CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite;
							int PrevAmount = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);

							CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
							byte holder = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0];
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0] = BlockHolderbyte.Value;
							BlockHolderbyte = holder;

							InstantiatedBlockHolder.GetComponent<Image>().sprite = PrevSprite;
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = PrevAmount.ToString();
						}
						break;
					}
					else if (mouseclick == Click.RightClick)
					{
						if (CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
						{
							CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = "1";
							CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							int temp = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp -= 1;
							if (temp == 0)
							{
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0] = BlockHolderbyte.Value;
								BlockHolderbyte = null;
								Destroy(InstantiatedBlockHolder);
							}
							else
							{
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0] = BlockHolderbyte.Value;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = temp.ToString();
							}
							break;
						}
						else if (CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
						{
							int temp = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp -= 1;
							if (temp == 0)
							{
								Destroy(InstantiatedBlockHolder);
								BlockHolderbyte = null;
							}
							else
							{
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = temp.ToString();
							}
							int temp2 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp2 += 1;
							CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = temp2.ToString();
							break;
						}
					}

				}
				else if (r.gameObject.name == "CraftSlot 2")
				{
					if (mouseclick == Click.LeftClick)
					{
						if (CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
						{
							CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1] = BlockHolderbyte.Value;
							BlockHolderbyte = null;
							Destroy(InstantiatedBlockHolder);
							PrevSlot = null;
						}
						else if (CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
						{
							int t1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
							int t2 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							int t3 = t1 + t2;

							if (t3 <= 64)
							{
								CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();
								Destroy(InstantiatedBlockHolder);
								BlockHolderbyte = null;
							}
						}
						else
						{
							Sprite PrevSprite = CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite;
							int PrevAmount = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);

							CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
							byte holder = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1];
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1] = BlockHolderbyte.Value;
							BlockHolderbyte = holder;

							InstantiatedBlockHolder.GetComponent<Image>().sprite = PrevSprite;
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = PrevAmount.ToString();
						}
						break;
					}
					else if (mouseclick == Click.RightClick)
					{
						if (CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
						{
							CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = "1";
							CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							int temp = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp -= 1;
							if (temp == 0)
							{
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1] = BlockHolderbyte.Value;
								BlockHolderbyte = null;
								Destroy(InstantiatedBlockHolder);
							}
							else
							{
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1] = BlockHolderbyte.Value;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = temp.ToString();
							}
							break;
						}
						else if (CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
						{
							int temp = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp -= 1;
							if (temp == 0)
							{
								Destroy(InstantiatedBlockHolder);
								BlockHolderbyte = null;
							}
							else
							{
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = temp.ToString();
							}
							int temp2 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp2 += 1;
							CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = temp2.ToString();
							break;
						}
					}

				}
				else if (r.gameObject.name == "CraftSlot 3")
				{
					if (mouseclick == Click.LeftClick)
					{
						if (CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
						{
							CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0] = BlockHolderbyte.Value;
							BlockHolderbyte = null;
							Destroy(InstantiatedBlockHolder);
							PrevSlot = null;
						}
						else if (CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
						{
							int t1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
							int t2 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							int t3 = t1 + t2;

							if (t3 <= 64)
							{
								CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();
								Destroy(InstantiatedBlockHolder);
								BlockHolderbyte = null;
							}
						}
						else
						{
							Sprite PrevSprite = CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite;
							int PrevAmount = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);

							CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
							byte holder = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0];
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0] = BlockHolderbyte.Value;
							BlockHolderbyte = holder;

							InstantiatedBlockHolder.GetComponent<Image>().sprite = PrevSprite;
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = PrevAmount.ToString();
						}
						break;
					}
					else if (mouseclick == Click.RightClick)
					{
						if (CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
						{
							CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = "1";
							CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							int temp = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp -= 1;
							if (temp == 0)
							{
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0] = BlockHolderbyte.Value;
								BlockHolderbyte = null;
								Destroy(InstantiatedBlockHolder);
							}
							else
							{
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0] = BlockHolderbyte.Value;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = temp.ToString();
							}
							break;
						}
						else if (CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
						{
							int temp = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp -= 1;
							if (temp == 0)
							{
								Destroy(InstantiatedBlockHolder);
								BlockHolderbyte = null;
							}
							else
							{
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = temp.ToString();
							}
							int temp2 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp2 += 1;
							CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = temp2.ToString();
							break;
						}
					}

				}
				else if (r.gameObject.name == "CraftSlot 4")
				{
					if (mouseclick == Click.LeftClick)
					{
						if (CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
						{
							CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1] = BlockHolderbyte.Value;
							BlockHolderbyte = null;
							Destroy(InstantiatedBlockHolder);
							PrevSlot = null;
						}
						else if (CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
						{
							int t1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
							int t2 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							int t3 = t1 + t2;

							if (t3 <= 64)
							{
								CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();
								Destroy(InstantiatedBlockHolder);
								BlockHolderbyte = null;
							}
						}
						else
						{
							Sprite PrevSprite = CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite;
							int PrevAmount = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);

							CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;
							byte holder = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1];
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1] = BlockHolderbyte.Value;
							BlockHolderbyte = holder;

							InstantiatedBlockHolder.GetComponent<Image>().sprite = PrevSprite;
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = PrevAmount.ToString();
						}
						break;
					}
					else if (mouseclick == Click.RightClick)
					{
						if (CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
						{
							CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = "1";
							CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
							int temp = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp -= 1;
							if (temp == 0)
							{
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1] = BlockHolderbyte.Value;
								BlockHolderbyte = null;
								Destroy(InstantiatedBlockHolder);
							}
							else
							{
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = temp.ToString();
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1] = BlockHolderbyte.Value;
							}
							break;
						}
						else if (CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite == InstantiatedBlockHolder.GetComponent<Image>().sprite)
						{
							int temp = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp -= 1;
							if (temp == 0)
							{
								Destroy(InstantiatedBlockHolder);
								BlockHolderbyte = null;
							}
							else
							{
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = temp.ToString();
							}
							int temp2 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text);
							temp2 += 1;
							CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = temp2.ToString();
							break;
						}
					}

				}
			}
		}

	}*/

	public void SelectedBlock(string InvBlock)
	{
		if (InstantiatedBlockHolder == null)
		{
			print(InvBlock);
			string[] s = InvBlock.Split(",");
			int blockindex = Convert.ToInt32(s[1]);
			int Inventory = Convert.ToInt32(s[0]);

			PrevSlot = InvBlock;

			if (inventory.activeSelf)
			{
				if (Inventory == 0 && hotbarBlocks[blockindex] != 34)
				{
					Debug.Log("Getting block from hotbar");
					InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
					InstantiatedBlockHolder.GetComponent<Image>().sprite = spritesByBlockID[hotbarBlocks[blockindex] - 1];
					InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = hotbarBlocksAmount[blockindex].ToString();
					InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
					InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
					Debug.Log("Item Byte:" + hotbarBlocks[blockindex]);
					BlockHolderbyte = hotbarBlocks[blockindex];
					Debug.Log(BlockHolderbyte.Value.ToString());
					hotbarBlocks[blockindex] = 34;
					hotbarBlocksAmount[blockindex] = 0;
					hotbarBlockSprites[blockindex].gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
				}
				else if (Inventory > 0)
				{
					if (Inventory == 1)
					{

						if (InvRow1[blockindex] != 34)
						{
							InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
							InstantiatedBlockHolder.GetComponent<Image>().sprite = spritesByBlockID[InvRow1[blockindex] - 1];
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = InventoryRows[Inventory - 1].transform.GetChild(blockindex).GetChild(0).GetComponent<TMP_Text>().text;
							InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
							InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
							BlockHolderbyte = InvRow1[blockindex];
							InvRow1[blockindex] = 34;
							InventoryRows[Inventory - 1].transform.GetChild(blockindex).GetComponent<Image>().sprite = spritesByBlockID[33];
							InventoryRows[Inventory - 1].transform.GetChild(blockindex).GetChild(0).GetComponent<TMP_Text>().text = "";
						}
					}
					else if (Inventory == 2)
					{
						if (InvRow2[blockindex] != 34)
						{
							InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
							InstantiatedBlockHolder.GetComponent<Image>().sprite = spritesByBlockID[InvRow2[blockindex] - 1];
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = InventoryRows[Inventory - 1].transform.GetChild(blockindex).GetChild(0).GetComponent<TMP_Text>().text;
							InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
							InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
							BlockHolderbyte = InvRow2[blockindex];
							InvRow2[blockindex] = 34;
							InventoryRows[Inventory - 1].transform.GetChild(blockindex).GetComponent<Image>().sprite = spritesByBlockID[33];
							InventoryRows[Inventory - 1].transform.GetChild(blockindex).GetChild(0).GetComponent<TMP_Text>().text = "";
						}
					}
					else if (Inventory == 3)
					{
						if (InvRow3[blockindex] != 34)
						{
							InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
							InstantiatedBlockHolder.GetComponent<Image>().sprite = spritesByBlockID[InvRow3[blockindex] - 1];
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = InventoryRows[Inventory - 1].transform.GetChild(blockindex).GetChild(0).GetComponent<TMP_Text>().text;
							InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
							InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
							BlockHolderbyte = InvRow3[blockindex];
							InvRow3[blockindex] = 34;
							InventoryRows[Inventory - 1].transform.GetChild(blockindex).GetComponent<Image>().sprite = spritesByBlockID[33];
							InventoryRows[Inventory - 1].transform.GetChild(blockindex).GetChild(0).GetComponent<TMP_Text>().text = "";
						}
					}
					/*else if (Inventory == 4)
					{
						Debug.Log("CraftGrid Clicked");
						if (blockindex == 0)
						{
							if (inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0];
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0] = 34;
							}
						}
						else if (blockindex == 1)
						{
							if (inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1];
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1] = 34;
							}
						}
						else if (blockindex == 2)
						{
							if (inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0];
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0] = 34;
							}
						}
						else if (blockindex == 3)
						{
							if (inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1];
								GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1] = 34;
							}
						}
						else if (blockindex == 4)
						{
							GameObject CraftGrid = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGrid;
							if (CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								if (CraftGrid == inventory.transform.Find("CraftGrid").gameObject)
								{
									if (GameObject.Find("Canvas").GetComponent<CraftingManager>().CorrectAnswerChosen == true)
									{

										if (CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;
											print("Slot1 amount: " + slot1);

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;
											print("Slot2 amount: " + slot1);

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;
											print("Slot3 amount: " + slot1);

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;
											print("Slot4 amount: " + slot1);

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}

										if (InstantiatedBlockHolder == null)
										{
											InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
											InstantiatedBlockHolder.GetComponent<Image>().sprite = inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite;
											InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
											InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
											InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

											BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().ResSlotByte;
										}
										if (CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
										{
											inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
											inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										}

										byte[,] temp = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes;
										print(temp[0, 0] + " , " + temp[0, 1] + " , " + temp[1, 0] + " , " + temp[1, 1]);
										if (temp[0, 0] == 34 && temp[0, 1] == 34 && temp[1, 0] == 34 && temp[1, 1] == 34)
										{
											print("All Craft Slots empty");
											inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
											inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
											GameObject.Find("Canvas").GetComponent<CraftingManager>().ResSlotByte = null;

											if (inventory.transform.Find("CodeSpace").childCount > 1)
											{
												GameObject.Find("Canvas").GetComponent<CraftingManager>().DestroyBlueprint();
											}
										}
									}
									else
									{
										StartCoroutine(InvalidAnswer(CraftGrid));
									}
								}
								else if (CraftGrid == inventory.transform.Find("CraftTableGrid"))
								{
									if (GameObject.Find("Canvas").GetComponent<CraftingManager>().CorrectAnswerChosen == true)
									{
										if (CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 0] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 0] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 0] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 1] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 1] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 1] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 2] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 2] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}
										if (CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
										{
											int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
											slot1 -= 1;

											if (slot1 == 0)
											{
												CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
												CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
												GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 2] = 34;
											}
											else
											{
												CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
											}
										}

										if (InstantiatedBlockHolder == null)
										{
											InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
											InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite;
											InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
											InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
											InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

											BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().ResSlotByte;
										}
										if (CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
										{
											CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
											CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										}

										byte[,] temp = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes;
										print(temp[0, 0] + " , " + temp[0, 1] + " , " + temp[0, 2] + " , " + temp[1, 0] + " , " + temp[1, 1] + " , " + temp[1, 2] + " , " + temp[2, 0] + " , " + temp[2, 1] + " , " + temp[2, 2]);
										if (temp[0, 0] == 34 && temp[0, 1] == 34 && temp[0, 2] == 34 && temp[1, 0] == 34 && temp[1, 1] == 34 && temp[1, 2] == 34 && temp[2, 0] == 34 && temp[2, 1] == 34 && temp[2, 2] == 34)
										{
											print("All Craft Slots empty");
											CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
											CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
											GameObject.Find("Canvas").GetComponent<CraftingManager>().ResSlotByte = null;

											if (inventory.transform.Find("CodeSpace").childCount > 1)
											{
												GameObject.Find("Canvas").GetComponent<CraftingManager>().DestroyBlueprint();
											}
										}
									}
									else
									{
										StartCoroutine(InvalidAnswer(CraftGrid));
									}
								}
							}
						}
					}
					else if (Inventory == 5)
					{
						GameObject CraftGrid = inventory.transform.Find("CraftTableGrid").gameObject;
						if (blockindex == 0)
						{
							if (CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 0];
								print("BlockByte: " + BlockHolderbyte.Value);
								GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 0] = 34;
							}
						}
						else if (blockindex == 1)
						{
							if (CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 0];
								print("BlockByte: " + BlockHolderbyte.Value);
								GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 0] = 34;
							}
						}
						else if (blockindex == 2)
						{
							if (CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 0];
								print("BlockByte: " + BlockHolderbyte.Value);
								GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 0] = 34;
							}
						}
						else if (blockindex == 3)
						{
							if (CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 1];
								print("BlockByte: " + BlockHolderbyte.Value);
								GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 1] = 34;
							}
						}
						else if (blockindex == 4)
						{
							if (CraftGrid.transform.Find("CraftSlot 5").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 5").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 5").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								CraftGrid.transform.Find("CraftSlot 5").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								CraftGrid.transform.Find("CraftSlot 5").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 1];
								print("BlockByte: " + BlockHolderbyte.Value);
								GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 1] = 34;
							}
						}
						else if (blockindex == 5)
						{
							if (CraftGrid.transform.Find("CraftSlot 6").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 6").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 6").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								CraftGrid.transform.Find("CraftSlot 6").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								CraftGrid.transform.Find("CraftSlot 6").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 1];
								print("BlockByte: " + BlockHolderbyte.Value);
								GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 1] = 34;
							}
						}
						else if (blockindex == 6)
						{
							if (CraftGrid.transform.Find("CraftSlot 7").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 7").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 7").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								CraftGrid.transform.Find("CraftSlot 7").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								CraftGrid.transform.Find("CraftSlot 7").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 2];
								print("BlockByte: " + BlockHolderbyte.Value);
								GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 2] = 34;
							}
						}
						else if (blockindex == 7)
						{
							if (CraftGrid.transform.Find("CraftSlot 8").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 8").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 8").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								CraftGrid.transform.Find("CraftSlot 8").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								CraftGrid.transform.Find("CraftSlot 8").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 2];
								print("BlockByte: " + BlockHolderbyte.Value);
								GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 2] = 34;
							}
						}
						else if (blockindex == 8)
						{
							if (CraftGrid.transform.Find("CraftSlot 9").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
							{
								InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
								InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 9").transform.GetChild(0).GetComponent<Image>().sprite;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 9").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
								InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
								InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

								CraftGrid.transform.Find("CraftSlot 9").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
								CraftGrid.transform.Find("CraftSlot 9").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 2];
								print("BlockByte: " + BlockHolderbyte.Value);
								GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 2] = 34;
							}
						}

					}
					*/
				}
			}
		}
		else
		{
			string[] s = InvBlock.Split(",");
			int blockindex = Convert.ToInt32(s[1]);
			int Inventory = Convert.ToInt32(s[0]);

			print("Inventory: " + Inventory);
			print("Index: " + blockindex);
			/*if (Inventory == 4)
			{
				Debug.Log("CraftGrid Clicked");
				if (blockindex == 0)
				{
					if (inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						if (InstantiatedBlockHolder == null)
						{
							InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
							InstantiatedBlockHolder.GetComponent<Image>().sprite = inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite;
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
							InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
							InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

							inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
							inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
							BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0];
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0] = 34;
						}
						else
						{
							print(BlockHolderbyte + " , " + GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0]);
							if (BlockHolderbyte == GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0])
							{
								int t1 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
								int t2 = Convert.ToInt32(inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
								int t3 = t1 + t2;

								if (t3 <= 64)
								{
									inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();
									Destroy(InstantiatedBlockHolder);
									BlockHolderbyte = null;
								}
							}
							else
							{
								Sprite Temp = inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite;
								int TAmount = Convert.ToInt32(inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);

								inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
								inventory.transform.Find("CraftGrid").Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;

								InstantiatedBlockHolder.GetComponent<Image>().sprite = Temp;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = TAmount.ToString();
							}
						}
					}
				}
				else if (blockindex == 1)
				{
					if (inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						if (InstantiatedBlockHolder != null)
						{
							InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
							InstantiatedBlockHolder.GetComponent<Image>().sprite = inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite;
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
							InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
							InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

							inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
							inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
							BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1];
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1] = 34;
						}
						else
						{
							if (InstantiatedBlockHolder.GetComponent<Image>().sprite == inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite)
							{
								int t1 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
								int t2 = Convert.ToInt32(inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
								int t3 = t1 + t2;

								if (t3 <= 64)
								{
									InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();
									inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
									inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
									GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1] = 34;
								}
							}
							else
							{
								Sprite Temp = inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite;
								int TAmount = Convert.ToInt32(inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);

								inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
								inventory.transform.Find("CraftGrid").Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;

								InstantiatedBlockHolder.GetComponent<Image>().sprite = Temp;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = TAmount.ToString();
							}
						}
					}
				}
				else if (blockindex == 2)
				{
					if (inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						if (InstantiatedBlockHolder != null)
						{
							InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
							InstantiatedBlockHolder.GetComponent<Image>().sprite = inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite;
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
							InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
							InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

							inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
							inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
							BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0];
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0] = 34;
						}
						else
						{
							if (InstantiatedBlockHolder.GetComponent<Image>().sprite == inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite)
							{
								int t1 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
								int t2 = Convert.ToInt32(inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
								int t3 = t1 + t2;

								if (t3 <= 64)
								{
									InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();
									inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
									inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
									GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0] = 34;
								}
							}
							else
							{
								Sprite Temp = inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite;
								int TAmount = Convert.ToInt32(inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);

								inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
								inventory.transform.Find("CraftGrid").Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;

								InstantiatedBlockHolder.GetComponent<Image>().sprite = Temp;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = TAmount.ToString();
							}
						}
					}
				}
				else if (blockindex == 3)
				{
					if (inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						if (InstantiatedBlockHolder != null)
						{
							InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
							InstantiatedBlockHolder.GetComponent<Image>().sprite = inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite;
							InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
							InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
							InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

							inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
							inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
							BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1];
							GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1] = 34;
						}
						else
						{
							if (InstantiatedBlockHolder.GetComponent<Image>().sprite == inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite)
							{
								int t1 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
								int t2 = Convert.ToInt32(inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
								int t3 = t1 + t2;

								if (t3 <= 64)
								{
									InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();
									inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
									inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
									GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1] = 34;
								}
							}
							else
							{
								Sprite Temp = inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite;
								int TAmount = Convert.ToInt32(inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);

								inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite = InstantiatedBlockHolder.GetComponent<Image>().sprite;
								inventory.transform.Find("CraftGrid").Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text;

								InstantiatedBlockHolder.GetComponent<Image>().sprite = Temp;
								InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = TAmount.ToString();
							}
						}

					}
				}
				else if (blockindex == 4)
				{
					GameObject CraftGrid = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGrid;
					if (CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						if (GameObject.Find("Canvas").GetComponent<CraftingManager>().CorrectAnswerChosen == true)
						{
							if (CraftGrid == inventory.transform.Find("CraftGrid").gameObject)
							{
								if (CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 0] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[0, 1] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 0] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes[1, 1] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}


								if (InstantiatedBlockHolder == null)
								{
									InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
									InstantiatedBlockHolder.GetComponent<Image>().sprite = inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite;
									InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
									InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
									InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

									BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().ResSlotByte;
								}
								else if (BlockHolderbyte != null && BlockHolderbyte == GameObject.Find("Canvas").GetComponent<CraftingManager>().ResSlotByte)
								{
									print("Detected same block");
									int t1 = Convert.ToInt32(InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text);
									int t2 = Convert.ToInt32(inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									int t3 = t1 + t2;
									if (t3 <= 64)
									{
										print("Block stack under 64. Adding To Stack..");
										InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = t3.ToString();
									}
								}

								byte[,] temp = GameObject.Find("Canvas").GetComponent<CraftingManager>().InvenBytes;
								print(temp[0, 0] + " , " + temp[0, 1] + " , " + temp[1, 0] + " , " + temp[1, 1]);
								if (temp[0, 0] == 34 && temp[0, 1] == 34 && temp[1, 0] == 34 && temp[1, 1] == 34)
								{
									print("All Craft Slots empty");
									inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
									inventory.transform.Find("CraftGrid").Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
									GameObject.Find("Canvas").GetComponent<CraftingManager>().ResSlotByte = null;

									if (inventory.transform.Find("CodeSpace").childCount > 1)
									{
										GameObject.Find("Canvas").GetComponent<CraftingManager>().DestroyBlueprint();
									}
								}
							}
							else if (CraftGrid == inventory.transform.Find("CraftTableGrid").gameObject)
							{
								if (CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 0] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 0] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 0] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 1] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 1] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 1] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 2] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 2] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}
								if (CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
								{
									int slot1 = Convert.ToInt32(CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);
									slot1 -= 1;

									if (slot1 == 0)
									{
										CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
										CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
										GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 2] = 34;
									}
									else
									{
										CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slot1.ToString();
									}
								}

								if (InstantiatedBlockHolder == null)
								{
									InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
									InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite;
									InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
									InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
									InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

									BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().ResSlotByte;
								}
								if (CraftGrid.transform.Find("CraftSlot 1").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 2").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 3").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 4").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 5").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 6").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 7").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 8").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34] && CraftGrid.transform.Find("CraftSlot 9").GetChild(0).GetComponent<Image>().sprite == spritesByBlockID[34])
								{
									CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
									CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
								}

								byte[,] temp = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes;
								print(temp[0, 0] + " , " + temp[0, 1] + " , " + temp[1, 0] + " , " + temp[1, 1]);
								if (temp[0, 0] == 34 && temp[0, 1] == 34 && temp[1, 0] == 34 && temp[1, 1] == 34)
								{
									print("All Craft Slots empty");
									CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
									CraftGrid.transform.Find("ResSlot").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
									GameObject.Find("Canvas").GetComponent<CraftingManager>().ResSlotByte = null;

									if (inventory.transform.Find("CodeSpace").childCount > 1)
									{
										GameObject.Find("Canvas").GetComponent<CraftingManager>().DestroyBlueprint();
									}
								}
							}
						}
					}
				}
			}
			else if (Inventory == 5)
			{
				GameObject CraftGrid = inventory.transform.Find("CraftTableGrid").gameObject;
				if (blockindex == 0)
				{
					if (CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
						InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite;
						InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
						InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
						InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

						CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
						CraftGrid.transform.Find("CraftSlot 1").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
						BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 1];
						GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 0] = 34;
					}
				}
				else if (blockindex == 1)
				{
					if (CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
						InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite;
						InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
						InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
						InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

						CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
						CraftGrid.transform.Find("CraftSlot 2").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
						BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 0];
						GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 0] = 34;
					}
				}
				else if (blockindex == 2)
				{
					if (CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
						InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite;
						InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
						InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
						InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

						CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
						CraftGrid.transform.Find("CraftSlot 3").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
						BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 0];
						GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 0] = 34;
					}
				}
				else if (blockindex == 3)
				{
					if (CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
						InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite;
						InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
						InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
						InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

						CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
						CraftGrid.transform.Find("CraftSlot 4").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
						BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 1];
						GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 1] = 34;
					}
				}
				else if (blockindex == 4)
				{
					if (CraftGrid.transform.Find("CraftSlot 5").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
						InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 5").transform.GetChild(0).GetComponent<Image>().sprite;
						InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 5").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
						InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
						InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

						CraftGrid.transform.Find("CraftSlot 5").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
						CraftGrid.transform.Find("CraftSlot 5").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
						BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 1];
						GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 1] = 34;
					}
				}
				else if (blockindex == 5)
				{
					if (CraftGrid.transform.Find("CraftSlot 6").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
						InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 6").transform.GetChild(0).GetComponent<Image>().sprite;
						InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 6").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
						InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
						InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

						CraftGrid.transform.Find("CraftSlot 6").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
						CraftGrid.transform.Find("CraftSlot 6").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
						BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 1];
						GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 1] = 34;
					}
				}
				else if (blockindex == 6)
				{
					if (CraftGrid.transform.Find("CraftSlot 7").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
						InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 7").transform.GetChild(0).GetComponent<Image>().sprite;
						InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 7").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
						InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
						InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

						CraftGrid.transform.Find("CraftSlot 7").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
						CraftGrid.transform.Find("CraftSlot 7").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
						BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 2];
						GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[0, 2] = 34;
					}
				}
				else if (blockindex == 7)
				{
					if (CraftGrid.transform.Find("CraftSlot 8").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
						InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 8").transform.GetChild(0).GetComponent<Image>().sprite;
						InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 8").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
						InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
						InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

						CraftGrid.transform.Find("CraftSlot 8").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
						CraftGrid.transform.Find("CraftSlot 8").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
						BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 2];
						GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[1, 2] = 34;
					}
				}
				else if (blockindex == 8)
				{
					if (CraftGrid.transform.Find("CraftSlot 9").transform.GetChild(0).GetComponent<Image>().sprite != spritesByBlockID[34])
					{
						InstantiatedBlockHolder = Instantiate(blockholderPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z), Quaternion.identity);
						InstantiatedBlockHolder.GetComponent<Image>().sprite = CraftGrid.transform.Find("CraftSlot 9").transform.GetChild(0).GetComponent<Image>().sprite;
						InstantiatedBlockHolder.transform.GetChild(0).GetComponent<TMP_Text>().text = CraftGrid.transform.Find("CraftSlot 9").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
						InstantiatedBlockHolder.transform.SetParent(GameObject.Find("Canvas").transform);
						InstantiatedBlockHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

						CraftGrid.transform.Find("CraftSlot 9").transform.GetChild(0).GetComponent<Image>().sprite = spritesByBlockID[34];
						CraftGrid.transform.Find("CraftSlot 9").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
						BlockHolderbyte = GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 2];
						GameObject.Find("Canvas").GetComponent<CraftingManager>().CraftGridBytes[2, 2] = 34;
					}
				}

			}
			*/
		}
	}
	public void SetHotbarBlock(int block)
	{
		if (block == 0) block++;
		//if (HotbarContainsBlock((byte)block)) currentSlot = GetHotbarSlotWith(block);
		else hotbarBlocks[currentSlot] = (byte)block;
	}

	public void CloseInventory()
	{
		inventory.SetActive(false);
		GetComponent<PlayerController>().controlsEnabled = true;
	}

	bool HotbarContainsBlock(byte b)
	{
		for(int i = 0; i < hotbarBlocks.Length; i++)
			if (hotbarBlocks[i] == b) return true;
	
		return false;
	}

	int GetHotbarSlotWith(int b)
	{
		for (int i = 0; i < hotbarBlocks.Length; i++)
			if (hotbarBlocks[i] == b) return i;

		return -1;
	}

	void WorkAroundForUnitysStupidMouseHidingSystemLikeWhatTheHell()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

}
public enum Click
{
	LeftClick,
	RightClick
}