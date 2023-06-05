using System.Collections.Generic;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;
using SimplexNoise;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using Photon.Pun;
using UnityEditor;

public enum GraphicsMode
{
    Fast, Fancy, Insane
}

public class World : MonoBehaviour
{
    public string seed;
    int s;

    public bool triggershaveworld = false;
    public GraphicsMode gMode = GraphicsMode.Fancy;

    public bool StartedTutorial = false;
    public CameraMotionBlur motionBlurEffect;
    public BloomAndFlares bloomEffect;
    public ScreenSpaceAmbientOcclusion occlusionEffect;
    public Light sun;

    public Material epicImageEffectsChunkMat;
    public Material standardChunkMat;
    public Material fastChunkMat;

    public Material gravelMat2;
    public Material sandMat2;

    public GameObject VillagerPrefab;
    public GameObject Marker;
    public List<GameObject> MarkersSpawned = new List<GameObject>();

    public GameObject physicsGravel;
    public GameObject physicsSand;

    public List<GameObject> TablesSpawned = new List<GameObject>();

    public bool DontSaveWorld = false;

    public bool Singleplayer = false;

    private GameObject InstantiatedObjectiveCheck;
    private GameObject InstantiatedObjectiveFall;
    public GameObject Objective1Check;
    public GameObject Objective1Fall;

    public string WorldName;

    public int chunkSize = 16;
    public float viewDistance = 6;

    public int worldWidth = 255;
    public int worldHeight = 200;

    public static World currentWorld;

    public byte[,,] world;

    public Chunk chunk;
    public Transform player;

    public GameObject particles;

    public Texture[] particleTextures;

    public Chunk[,,] chunks;

    bool finishedGeneratingChunks = false;
    int counter = 0;

    Vector3Int lastPlayerChunkPos = Vector3Int.up * 1000;
    Vector3Int playerChunkPos;

    Vector3Int playerPos;

    List<Vector3Int> chunkSpawnCues = new List<Vector3Int>();

    public Text loadingWorldInfo;
    public Text loadingWorldInfoShadow;

    public GameObject loadingScreen;

    public AudioSource music;

    public WorldData currentWorldData;
    public GameObject BlockAccessTable;

    bool worldIsFromSaveFile = false;

    [HideInInspector] public bool worldInitialized = false;

    public int worldBlockWidth
    {
        get
        {
            return worldWidth * chunkSize;
        }
    }

    public int worldBlockHeight
    {
        get
        {
            return worldHeight * chunkSize;
        }
    }

    float curNoise = 0;

    void Awake()
    {
        if (WorldName == "Tutorial")
        {
            Application.wantsToQuit += GameObject.Find("GameManager").GetComponent<DataToHold>().StopQuit;
            GameObject Marker1 = Instantiate(Marker, new Vector3(81.4f, 60f, 74.9f), Quaternion.identity);
            MarkersSpawned.Add(Marker1);
            GameObject bat1 = Instantiate(BlockAccessTable, new Vector3(0, 0, 0), Quaternion.identity);
            bat1.transform.position = new Vector3(81.4f, 51.5f, 74.9f);
            bat1.transform.eulerAngles = new Vector3(0, 0, 0);
            bat1.AddComponent<BlockAccessTable>();
            bat1.GetComponent<BlockAccessTable>().CodeImage = GameObject.Find("Canvas").transform.Find("CodeImage").gameObject;
            bat1.GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.Conditions;
            bat1.GetComponent<BlockAccessTable>().thisTableMarker = Marker1;

            GameObject Marker2 = Instantiate(Marker, new Vector3(142.4f, 60f, 138.05f), Quaternion.identity);
            MarkersSpawned.Add(Marker2);
            GameObject bat2 = Instantiate(BlockAccessTable, new Vector3(0, 0, 0), Quaternion.identity);
            bat2.transform.position = new Vector3(142.4f, 52.5f, 138.05f);
            bat2.transform.eulerAngles = new Vector3(0, 0, 0);
            bat2.AddComponent<BlockAccessTable>();
            bat2.GetComponent<BlockAccessTable>().CodeImage = GameObject.Find("Canvas").transform.Find("CodeImage").gameObject;
            bat2.GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.DataTypes;
            bat2.GetComponent<BlockAccessTable>().thisTableMarker = Marker2;

            GameObject Marker3 = Instantiate(Marker, new Vector3(176.4f, 60f, 37f), Quaternion.identity);
            MarkersSpawned.Add(Marker3);
            GameObject bat3 = Instantiate(BlockAccessTable, new Vector3(0, 0, 0), Quaternion.identity);
            bat3.transform.position = new Vector3(176.4f, 51.5f, 37f);
            bat3.transform.eulerAngles = new Vector3(0, 0, 0);
            bat3.AddComponent<BlockAccessTable>();
            bat3.GetComponent<BlockAccessTable>().CodeImage = GameObject.Find("Canvas").transform.Find("CodeImage").gameObject;
            bat3.GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.Loops;
            bat3.GetComponent<BlockAccessTable>().thisTableMarker = Marker3;

            GameObject bat4 = Instantiate(BlockAccessTable, new Vector3(0, 0, 0), Quaternion.identity);
            bat4.transform.position = new Vector3(92.46f, 43.5f, 92.11f);
            bat4.transform.eulerAngles = new Vector3(0, 0, 0);
            bat4.AddComponent<BlockAccessTable>();
            bat4.GetComponent<BlockAccessTable>().CodeImage = GameObject.Find("Canvas").transform.Find("CodeImage").gameObject;
            bat4.GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.None;
            bat4.GetComponent<BlockAccessTable>().norestart = true;
            bat4.name = "SpawnBat";
        }
        else if (WorldName == "Singleplayer")
        {
            GameObject Marker1 = Instantiate(Marker, new Vector3(94.7f, 52f, 91.6f), Quaternion.identity);
            Marker1.transform.eulerAngles = new Vector3(0, 90f, 0);
            MarkersSpawned.Add(Marker1);
            GameObject bat1 = Instantiate(BlockAccessTable, new Vector3(0, 0, 0), Quaternion.identity);
            bat1.transform.position = new Vector3(94f, 46.5f, 92.6f);
            bat1.transform.eulerAngles = new Vector3(0, 90f, 0);
            bat1.AddComponent<BlockAccessTable>();
            bat1.GetComponent<BlockAccessTable>().CodeImage = GameObject.Find("Canvas").transform.Find("CodeImage").gameObject;
            bat1.GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.Conditions;
            bat1.GetComponent<BlockAccessTable>().thisTableMarker = Marker1;
            bat1.GetComponent<BlockAccessTable>().TutorialMode = false;
            bat1.GetComponent<BlockAccessTable>().UnlockBlocks1 = true;
            bat1.GetComponent<BlockAccessTable>().UnlockBlocks2 = true;
            bat1.GetComponent<BlockAccessTable>().UnlockBlocks3 = true;
            bat1.GetComponent<BlockAccessTable>().CompletedConditions = true;
            bat1.GetComponent<BlockAccessTable>().CompletedDataTypes = true;
            bat1.GetComponent<BlockAccessTable>().CompletedLoops = true;
            bat1.GetComponent<BlockAccessTable>().norestart = true;
            if (GameObject.Find("MultiplayerManager") != null)
            {
                GameObject.Find("PlayerConnector2").GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1]);

                if (PhotonNetwork.IsMasterClient)
                {
                    GameObject.Find("PlayerConnector2").GetComponent<PlayerConnector>().ObjectToMove = GameObject.Find("PlayerMimic").gameObject;
                    GameObject.Find("PlayerMimic").transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().text = PhotonNetwork.PlayerList[1].NickName;
                    GameObject.Find("PlayerMimic").transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().color = Color.red;
                }
                else
                {
                    GameObject.Find("PlayerConnector").GetComponent<PlayerConnector>().ObjectToMove = GameObject.Find("PlayerMimic").gameObject;
                    GameObject.Find("PlayerMimic").transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().text = PhotonNetwork.PlayerList[0].NickName;
                    GameObject.Find("PlayerMimic").transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().color = Color.blue;
                }

                GameObject.Find("Canvas").transform.Find("ObjectiveImage").Find("P1Name").GetComponent<Text>().text = PhotonNetwork.PlayerList[0].NickName;
                GameObject.Find("Canvas").transform.Find("ObjectiveImage").Find("P2Name").GetComponent<Text>().text = PhotonNetwork.PlayerList[1].NickName;
                GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables.Add(bat1);
            }
            else
            {
                GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables.Add(bat1);
            }

            GameObject Marker2 = Instantiate(Marker, new Vector3(151f, 52.5f, 87f), Quaternion.identity);
            Marker2.transform.eulerAngles = new Vector3(0, 0, 0);
            MarkersSpawned.Add(Marker2);
            GameObject bat2 = Instantiate(BlockAccessTable, new Vector3(0, 0, 0), Quaternion.identity);
            bat2.transform.position = new Vector3(150f, 37.55f, 85f);
            bat2.transform.eulerAngles = new Vector3(0, 0, 0);
            bat2.AddComponent<BlockAccessTable>();
            bat2.GetComponent<BlockAccessTable>().CodeImage = GameObject.Find("Canvas").transform.Find("CodeImage").gameObject;
            bat2.GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.Conditions;
            bat2.GetComponent<BlockAccessTable>().thisTableMarker = Marker2;
            bat2.GetComponent<BlockAccessTable>().TutorialMode = false;
            bat2.GetComponent<BlockAccessTable>().UnlockBlocks1 = true;
            bat2.GetComponent<BlockAccessTable>().UnlockBlocks2 = true;
            bat2.GetComponent<BlockAccessTable>().UnlockBlocks3 = true;
            bat2.GetComponent<BlockAccessTable>().CompletedConditions = true;
            bat2.GetComponent<BlockAccessTable>().CompletedDataTypes = true;
            bat2.GetComponent<BlockAccessTable>().CompletedLoops = true;
            bat2.GetComponent<BlockAccessTable>().norestart = true;
            bat2.SetActive(false);
            Marker2.SetActive(false);

            if (GameObject.Find("MultiplayerManager") != null)
            {
                GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables.Add(bat2);
            }
            else
            {
                GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables.Add(bat2);
            }

            GameObject Marker3 = Instantiate(Marker, new Vector3(45.5f, 52.5f, 153f), Quaternion.identity);
            Marker3.transform.eulerAngles = new Vector3(0, 45f, 0);
            MarkersSpawned.Add(Marker3);
            GameObject bat3 = Instantiate(BlockAccessTable, new Vector3(0, 0, 0), Quaternion.identity);
            bat3.transform.position = new Vector3(45.5f, 42.5f, 153f);
            bat3.transform.eulerAngles = new Vector3(0, 180f, 0);
            bat3.AddComponent<BlockAccessTable>();
            bat3.GetComponent<BlockAccessTable>().CodeImage = GameObject.Find("Canvas").transform.Find("CodeImage").gameObject;
            bat3.GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.Conditions;
            bat3.GetComponent<BlockAccessTable>().thisTableMarker = Marker3;
            bat3.GetComponent<BlockAccessTable>().TutorialMode = false;
            bat3.GetComponent<BlockAccessTable>().UnlockBlocks1 = true;
            bat3.GetComponent<BlockAccessTable>().UnlockBlocks2 = true;
            bat3.GetComponent<BlockAccessTable>().UnlockBlocks3 = true;
            bat3.GetComponent<BlockAccessTable>().CompletedConditions = true;
            bat3.GetComponent<BlockAccessTable>().CompletedDataTypes = true;
            bat3.GetComponent<BlockAccessTable>().CompletedLoops = true;
            bat3.GetComponent<BlockAccessTable>().norestart = true;
            bat3.SetActive(false);
            Marker3.SetActive(false);

            if (GameObject.Find("MultiplayerManager") != null)
            {
                GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables.Add(bat3);
            }
            else
            {
                GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables.Add(bat3);
            }

            GameObject Marker4 = Instantiate(Marker, new Vector3(10.39f, 58f, 24.7f), Quaternion.identity);
            Marker4.transform.eulerAngles = new Vector3(0, 90f, 0);
            Marker4.transform.localScale = new Vector3(2f, 2f, 2f);
            MarkersSpawned.Add(Marker4);
            GameObject bat4 = Instantiate(BlockAccessTable, new Vector3(0, 0, 0), Quaternion.identity);
            bat4.transform.position = new Vector3(10.39f, 47.43f, 24.7f);
            bat4.transform.eulerAngles = new Vector3(0, 180f, 0);
            bat4.AddComponent<BlockAccessTable>();
            bat4.GetComponent<BlockAccessTable>().CodeImage = GameObject.Find("Canvas").transform.Find("CodeImage").gameObject;
            bat4.GetComponent<BlockAccessTable>().ThisTableType = BlockAccessType.Conditions;
            bat4.GetComponent<BlockAccessTable>().thisTableMarker = Marker4;
            bat4.GetComponent<BlockAccessTable>().TutorialMode = false;
            bat4.GetComponent<BlockAccessTable>().UnlockBlocks1 = true;
            bat4.GetComponent<BlockAccessTable>().UnlockBlocks2 = true;
            bat4.GetComponent<BlockAccessTable>().UnlockBlocks3 = true;
            bat4.GetComponent<BlockAccessTable>().CompletedConditions = true;
            bat4.GetComponent<BlockAccessTable>().CompletedDataTypes = true;
            bat4.GetComponent<BlockAccessTable>().CompletedLoops = true;
            bat4.GetComponent<BlockAccessTable>().norestart = true;
            bat4.SetActive(false);
            Marker4.SetActive(false);

            if (GameObject.Find("MultiplayerManager") != null)
            {
                GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>().BlockAccessTables.Add(bat4);
            }
            else
            {
                GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>().BlockAccessTables.Add(bat4);
            }
            TablesSpawned.Add(bat1);
            TablesSpawned.Add(bat2);
            TablesSpawned.Add(bat3);
            TablesSpawned.Add(bat4);
            GameObject Villager1 = Instantiate(VillagerPrefab, new Vector3(14.24f, 48.8f, 22.5355f), Quaternion.identity);
            Villager1.transform.eulerAngles = new Vector3(-90f, 0, -150f);
            Villager1.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            GameObject Villager2 = Instantiate(VillagerPrefab, new Vector3(12.906f, 48.8f, 23.308f), Quaternion.identity);
            Villager2.transform.eulerAngles = new Vector3(-90f, 0, -155f);
            Villager2.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            GameObject Villager3 = Instantiate(VillagerPrefab, new Vector3(15.929f, 48.8f, 21.898f), Quaternion.identity);
            Villager3.transform.eulerAngles = new Vector3(-90f, 0, -140f);
            Villager3.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            GameObject Villager4 = Instantiate(VillagerPrefab, new Vector3(7.37f, 43.75f, 8.1f), Quaternion.identity);
            Villager4.transform.eulerAngles = new Vector3(-90f, 0, 0);

            GameObject Villager5 = Instantiate(VillagerPrefab, new Vector3(6.85f, 43.75f, 8.4f), Quaternion.identity);
            Villager5.transform.eulerAngles = new Vector3(-90f, 0, 30f);

            GameObject Villager6 = Instantiate(VillagerPrefab, new Vector3(6.26f, 43.75f, 8.74f), Quaternion.identity);
            Villager6.transform.eulerAngles = new Vector3(-90f, 0, 60f);

            InstantiatedObjectiveCheck = Instantiate(Objective1Check, new Vector3(96f, 46.93f, 96f), Quaternion.identity);
            InstantiatedObjectiveFall = Instantiate(Objective1Fall, new Vector3(93.3f, 47.3f, 95.6f), Quaternion.identity);

            if (GameObject.Find("MultiplayerManager") != null)
            {
                GameObject.Find("GameManager").GetComponent<DataToHold>().mp = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>();
            }
        }
        currentWorld = this;

        world = new byte[worldBlockWidth, worldBlockHeight, worldBlockWidth];
        chunks = new Chunk[worldWidth, worldHeight, worldWidth];

        int playerChunkX = Mathf.RoundToInt((player.position.x) / chunkSize);
        int playerChunkY = Mathf.RoundToInt((player.position.y) / chunkSize);
        int playerChunkZ = Mathf.RoundToInt((player.position.z) / chunkSize);

        playerChunkPos = new Vector3Int(playerChunkX, playerChunkY, playerChunkZ);

        int playerX = Mathf.RoundToInt(player.position.x);
        int playerY = Mathf.RoundToInt(player.position.y);
        int playerZ = Mathf.RoundToInt(player.position.z);

        playerPos = new Vector3Int(playerX, playerY, playerZ);

        LoadWorld();

        if (!worldIsFromSaveFile) { s = Random.Range(0, 100000); print("World is not from save File"); }

        if (world == null)
        {
            print("World is null");
            worldIsFromSaveFile = false;
            world = new byte[worldBlockWidth, worldBlockHeight, worldBlockWidth];

            player.GetComponent<PlayerIO>().hotbarBlocks = new byte[] { 5, 13, 14, 15, 5, 23, 24, 1, 2 };
            player.GetComponent<PlayerIO>().hotbarBlocksAmount = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            player.GetComponent<PlayerIO>().currentSlot = 0;

            s = Random.Range(0, 100000);
        }

        Random.InitState(s);

        Invoke("GenerateWorld", 2f);

        if (gMode == GraphicsMode.Insane)
        {
            bloomEffect.enabled = true;
            motionBlurEffect.enabled = true;
            occlusionEffect.enabled = true;
            RenderSettings.fog = true;
            sun.shadows = LightShadows.Soft;
        }
        else
        {
            bloomEffect.enabled = false;
            motionBlurEffect.enabled = false;
            RenderSettings.fog = false;
            occlusionEffect.enabled = false;
            sun.shadows = LightShadows.None;
        }
    }

    void Update()
    {
        if (triggershaveworld)
        {
            triggershaveworld = false;
            ShaveWorld();
        }
        int playerChunkX = Mathf.RoundToInt((player.position.x) / chunkSize);
        int playerChunkY = Mathf.RoundToInt((player.position.y) / chunkSize);
        int playerChunkZ = Mathf.RoundToInt((player.position.z) / chunkSize);

        playerChunkPos = new Vector3Int(playerChunkX, playerChunkY, playerChunkZ);

        int playerX = Mathf.RoundToInt(player.position.x);
        int playerY = Mathf.RoundToInt(player.position.y);
        int playerZ = Mathf.RoundToInt(player.position.z);

        playerPos = new Vector3Int(playerX, playerY, playerZ);

        if (playerChunkPos != lastPlayerChunkPos)
        {
            finishedGeneratingChunks = false;

            StartCoroutine(GenerateChunks());

            lastPlayerChunkPos = playerChunkPos;
            counter = 0;
        }
    }

    void GenerateWorld()
    {
        if (worldIsFromSaveFile)
        {
            loadingWorldInfo.text = "Loading World";
            loadingWorldInfoShadow.text = "Loading World";

            Invoke("GenerateStartChunks", 1f);

            return;
        }

        loadingWorldInfo.text = "Generating World";
        loadingWorldInfoShadow.text = "Generating World";

        for (int x = 0; x < worldBlockWidth; x++)
        {
            for (int y = 0; y < worldBlockHeight; y++)
            {
                for (int z = 0; z < worldBlockWidth; z++)
                {
                    float noiseScale = 30 + (Mathf.PerlinNoise((x + s) / 75, (z + s) / 75) * 30);
                    float sandBiome = Mathf.PerlinNoise((x - 512 + s) / 100f, (z - 512 + s) / 100f) * 50f;

                    float noise = Noise.Generate((x + s) / noiseScale, y / noiseScale, (z + s) / noiseScale);

                    float dividendSub = ((sandBiome - 20) / 3f);

                    noise += (worldBlockHeight - y - 25) / (10f - dividendSub);
                    curNoise = noise;

                    if (sandBiome > 30)
                    {
                        if (noise > 0.1f)
                        {
                            world[x, y, z] = 6;
                        }
                        if (noise > 0.8f) world[x, y, z] = 3;
                    }
                    else
                    {
                        if (noise > 0.1f)
                        {
                            world[x, y, z] = 1;
                        }
                        if (noise > 0.4f) world[x, y, z] = 3;
                    }

                    // BEDROCK
                    if (y == 0) world[x, y, z] = 4;
                    if (x == 0 || x == worldBlockWidth - 1 || z == 0 || z == worldBlockWidth - 1)
                    {
                        if (noise > 0.4f)
                            world[x, y, z] = 4;
                    }
                }
            }
        }
        Invoke("SpawnOres", 0.05f);
    }

    void GenerateCaves()
    {
        loadingWorldInfo.text = "Digging";
        loadingWorldInfoShadow.text = "Digging";

        for (int x = 0; x < worldBlockWidth; x++)
        {
            for (int y = 0; y < worldBlockHeight; y++)
            {
                for (int z = 0; z < worldBlockWidth; z++)
                {
                    float caves = Noise.Generate((x + s) / 40f, (y + s) / 40f, (z + s) / 40f);

                    // CAVES
                    if (caves >= 0.8f && world[x, y, z] != 4) world[x, y, z] = 0;
                }
            }
        }

        Invoke("Plant", 0.25f);
    }

    void SpawnOres()
    {
        loadingWorldInfo.text = "Spawning Ores";
        loadingWorldInfoShadow.text = "Spawning Ores";

        for (int x = 0; x < worldBlockWidth; x++)
        {
            for (int y = 0; y < worldBlockHeight; y++)
            {
                for (int z = 0; z < worldBlockWidth; z++)
                {
                    float coal = Noise.Generate((x + s + 512) / 10f, (y + s + 512) / 10f, (z + s + 512) / 10f);
                    float iron = Noise.Generate((x + s + 256) / 14f, (y + s + 256) / 18f, (z + s + 256) / 14f);
                    float redstone = Noise.Generate((x + s + 1028) / 17f, (y + s + 1028) / 17f, (z + s + 1028) / 17f);
                    float gold = Noise.Generate((x + s + 2048) / 20f, (y + s + 2048) / 20f, (z + s + 2048) / 20f);
                    float diamond = Noise.Generate((x + s + 4096) / 20f, (y + s + 4096) / 20f, (z + s + 4096) / 20f);

                    float groundDirt = Noise.Generate((x + s + 1000) / 30f, (y + s + 1000) / 30f, (z + s + 1000) / 30f);
                    float groundGravel = Noise.Generate((x + s + 2000) / 32f, (y + s + 2000) / 32f, (z + s + 2000) / 32f);

                    // ORES
                    if (coal >= 0.875f && curNoise > 0.4f) world[x, y, z] = 8;
                    if (iron >= 0.92f && curNoise > 0.4f && y < 64) world[x, y, z] = 9;
                    if (redstone >= 0.94f && curNoise > 0.4f && y < 32) world[x, y, z] = 10;
                    if (gold >= 0.94f && curNoise > 0.4f && y < 52) world[x, y, z] = 11;
                    if (diamond >= 0.955f && curNoise > 0.4f && y < 45) world[x, y, z] = 12;

                    // UNDERGROUND DIRT
                    if (groundDirt >= 0.9f && curNoise > 0.4f) world[x, y, z] = 1;

                    // UNDERGROUND GRAVEL
                    if (groundGravel >= 0.9f && curNoise > 0.4f) world[x, y, z] = 7;
                }
            }
        }
        Invoke("GenerateCaves", 0.25f);
    }

    void Plant()
    {
        loadingWorldInfo.text = "Planting";
        loadingWorldInfoShadow.text = "Planting";

        int treeCount = 0;
        for (int x = 0; x < worldBlockWidth; x++)
        {
            for (int z = 0; z < worldBlockWidth; z++)
            {
                bool treeHere = Random.Range(0, 1000) < 5;

                if (treeHere)
                {
                    treeCount++;

                    int groundLevel = 0;

                    bool startUp = Random.Range(0, 2) == 0;
                    if (startUp)
                    {
                        for (int y = worldBlockHeight; y <= 0; y--)
                        {
                            if (world[x, y, z] == 1 && GetBlock(x, y + 1, z) == 0)
                            {
                                groundLevel = y;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int y = 0; y < worldBlockHeight; y++)
                        {
                            if (world[x, y, z] == 1 && GetBlock(x, y + 1, z) == 0)
                            {
                                groundLevel = y;
                                break;
                            }
                        }
                    }

                    if (groundLevel != 0)
                    {
                        int treeHeight = Random.Range(4, 8);

                        for (int y = groundLevel + 1; y <= groundLevel + treeHeight; y++)
                            TrySetBlock(x, y, z, 13);

                        for (int x1 = x - 2; x1 <= x + 2; x1++)
                        {
                            for (int y1 = groundLevel + treeHeight - 2; y1 <= groundLevel + treeHeight + 2; y1++)
                            {
                                for (int z1 = z - 2; z1 <= z + 2; z1++)
                                {
                                    if (GetBlock(x1, y1, z1) == 13) continue;

                                    if (y1 <= groundLevel + treeHeight)
                                    {
                                        TrySetBlock(x1, y1, z1, 14);
                                    }
                                    else
                                    {
                                        if (Mathf.Abs(x1 - x) == 1 && Mathf.Abs(z1 - z) == 0 ||
                                            Mathf.Abs(x1 - x) == 0 && Mathf.Abs(z1 - z) == 1 ||
                                            (x1 - x == 0 && z1 - z == 0))
                                        {
                                            TrySetBlock(x1, y1, z1, 14);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        for (int x = 0; x < worldBlockWidth; x++)
        {
            for (int y = 0; y < worldBlockHeight; y++)
            {
                for (int z = 0; z < worldBlockWidth; z++)
                {
                    if (world[x, y, z] == 1 && GetBlock(x, y + 1, z) == 0 && y > 18)
                    {
                        float gNoise = Mathf.PerlinNoise((x + s) / 30f, (z + s) / 30f) * 200;
                        bool g = Random.Range(0, 30 + gNoise) < 2;
                        bool r = Random.Range(0, 600 + gNoise) < 2;
                        bool d = Random.Range(0, 600 + gNoise) < 2;

                        world[x, y, z] = 2;

                        if (g) TrySetBlock(x, y + 1, z, 29);
                        if (r) TrySetBlock(x, y + 1, z, 30);
                        if (d) TrySetBlock(x, y + 1, z, 31);
                    }
                }
            }
        }
        Invoke("GenerateStartChunks", 1f);
    }

    void GenerateStartChunks()
    {
        loadingWorldInfo.text = "Building Terrain";
        loadingWorldInfoShadow.text = "Building Terrain";
        chunkSpawnCues.Clear();

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                for (int z = 0; z < worldWidth; z++)
                {
                    Vector3Int chunkPos = new Vector3Int(x, y, z);

                    if (Vector3Int.Distance(playerChunkPos, chunkPos) <= (viewDistance / 2) &&
                        !InvisibleChunk(x, y, z) && !chunks[x, y, z])
                        chunkSpawnCues.Add(chunkPos);
                }
            }
        }

        for (int y = worldHeight - 1; y > 0; y--)
        {
            int x = playerChunkPos.x;
            int z = playerChunkPos.z;

            Vector3Int chunkPos = new Vector3Int(x, y, z);

            if (!InvisibleChunk(x, y, z) && !chunks[x, y, z]) chunkSpawnCues.Add(chunkPos);
        }

        chunkSpawnCues.Sort
    (
        delegate (Vector3Int a, Vector3Int b)
        {
            return Vector3Int.Distance(playerPos, a * chunkSize).CompareTo(Vector3Int.Distance(playerPos, b * chunkSize));
        }
    );

        for (int i = 0; i < chunkSpawnCues.Count; i++)
        {
            int x = chunkSpawnCues[i].x;
            int y = chunkSpawnCues[i].y;
            int z = chunkSpawnCues[i].z;

            if (chunks[x, y, z]) continue;

            Vector3 spawnPos = new Vector3(x, y, z) * 16;
            Chunk newChunk = Instantiate(chunk, spawnPos, Quaternion.identity, this.transform) as Chunk;

            if (gMode == GraphicsMode.Fast)
                newChunk.GetComponent<Renderer>().material = fastChunkMat;
            if (gMode == GraphicsMode.Fancy)
                newChunk.GetComponent<Renderer>().material = standardChunkMat;
            if (gMode == GraphicsMode.Insane)
                newChunk.GetComponent<Renderer>().material = epicImageEffectsChunkMat;

            chunks[x, y, z] = newChunk;
        }

        Invoke("FloorPlayer", 1.75f);
    }

    void GenerateChunksAroundPlayer()
    {
        chunkSpawnCues.Clear();

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                for (int z = 0; z < worldWidth; z++)
                {
                    Vector3Int chunkPos = new Vector3Int(x, y, z);

                    if (Vector3Int.Distance(playerChunkPos, chunkPos) <= (viewDistance / 2) &&
                        !InvisibleChunk(x, y, z) && !chunks[x, y, z])
                        chunkSpawnCues.Add(chunkPos);
                }
            }
        }

        for (int y = worldHeight - 1; y > 0; y--)
        {
            int x = playerChunkPos.x;
            int z = playerChunkPos.z;

            Vector3Int chunkPos = new Vector3Int(x, y, z);

            if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight || z < 0 || z >= worldWidth)
                continue;

            if (!InvisibleChunk(x, y, z) && !chunks[x, y, z]) chunkSpawnCues.Add(chunkPos);
        }

        chunkSpawnCues.Sort
        (
            delegate (Vector3Int a, Vector3Int b)
            {
                return Vector3Int.Distance(playerPos, a * chunkSize).CompareTo(Vector3Int.Distance(playerPos, b * chunkSize));
            }
        );

        for (int i = 0; i < chunkSpawnCues.Count; i++)
        {
            int x = chunkSpawnCues[i].x;
            int y = chunkSpawnCues[i].y;
            int z = chunkSpawnCues[i].z;

            Vector3 spawnPos = new Vector3(x, y, z) * 16;
            Chunk newChunk = Instantiate(chunk, spawnPos, Quaternion.identity, this.transform) as Chunk;


            if (gMode == GraphicsMode.Fast)
                newChunk.GetComponent<Renderer>().material = fastChunkMat;
            if (gMode == GraphicsMode.Fancy)
                newChunk.GetComponent<Renderer>().material = standardChunkMat;
            if (gMode == GraphicsMode.Insane)
                newChunk.GetComponent<Renderer>().material = epicImageEffectsChunkMat;

            chunks[x, y, z] = newChunk;
        }
    }

    void FloorPlayer()
    {
        RaycastHit hit;
        Ray ray = new Ray(player.position, Vector3.down);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 motion = new Vector3(0, -Vector3.Distance(player.position, hit.point) + 0.5f, 0);
            player.GetComponent<CharacterController>().Move(motion);
        }
        player.GetComponent<PlayerController>().enabled = true;
        loadingScreen.SetActive(false);
        /*if (WorldName != "Tutorial" && WorldName != "TutorialNew" && WorldName != "" && WorldName != " ")
        {
            //GameObject.Find("GameManager").GetComponent<DataToHold>().PreRoundTimerText = GameObject.Find("Canvas").transform.Find("TimerText").gameObject;
            GameObject t = GameObject.Find("Canvas").transform.Find("TutorialCanvas").gameObject;
            t.SetActive(true);
            //GameObject.Find("GameManager").GetComponent<DataToHold>().StartGame();
        }*/
        if (PauseMenu.pauseMenu.music) music.Play();

        worldInitialized = true;

        if (WorldName == "Tutorial" && !StartedTutorial)
        {
            GameObject.Find("player").GetComponent<PlayerController>().controlsEnabled = false;
            GameObject.Find("Canvas").transform.Find("crosshair").gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameObject.Find("Canvas").transform.Find("CodeImage").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("TitleText").GetComponent<Text>().text = "Tutorial Started";
            if (GameObject.Find("GameManager").GetComponent<DataToHold>().SelectedLanguage == Language.CSharp)
            {
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("ContentText").GetComponent<Text>().text = "Welcome to the tutorial!\n\nIn the tutorial, you will learn about the programming concepts you will encounter utilizing the C# language.\n\n Look for the yellow markers pointing to the B.A.Ts (Block access table). Each one will teach you a specific concept to learn.";

            }
            else if (GameObject.Find("GameManager").GetComponent<DataToHold>().SelectedLanguage == Language.Python)
            {
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("ContentText").GetComponent<Text>().text = "Welcome to the tutorial!\n\nIn the tutorial, you will learn about the programming concepts you will encounter utilizing the Python language.\n\n Look for the yellow markers pointing to the B.A.Ts (Block access table). Each one will teach you a specific concept to learn.";
            }
            GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ok!";
            GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(StartingTutorial);
        }
        else if (WorldName == "Singleplayer")
        {
            PlayerIO pi = GameObject.Find("player").GetComponent<PlayerIO>();

            foreach (Image i in pi.hotbarBlockSprites)
            {
                i.sprite = pi.spritesByBlockID[33];
            }
            GameObject.Find("player").GetComponent<PlayerController>().controlsEnabled = false;
            GameObject.Find("Canvas").transform.Find("crosshair").gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameObject.Find("Canvas").transform.Find("CodeImage").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ok!";
            if (GameObject.Find("SinglePlayerManager") != null)
            {
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("TitleText").GetComponent<Text>().text = "Singleplayer";
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("ContentText").GetComponent<Text>().text = "Welcome to the Singleplayer Mode!\n\nGet to the Markers and complete all objectives!\n\nAll objectives require you to have blocks to reach them which means you will have to complete code questions at the B.A.Ts.\n\n";
            }
            else
            {
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("TitleText").GetComponent<Text>().text = "Multiplayer";
                GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("ContentText").GetComponent<Text>().text = "Welcome to the Multiplayer Mode!\n\nGet to the Markers and complete all objectives!\n\nAll objectives require you to have blocks to reach them which means you will have to complete code questions at the B.A.Ts.\n\n";
            }
            GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(InitSingle);
        }
    }

    void InitSingle()
    {
        if (GameObject.Find("SinglePlayerManager") != null)
        {
            SinglePlayerManager sp = GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>();
            sp.PreSingle();
        }
        else if(GameObject.Find("MultiplayerManager") != null)
        {
            MultiplayerManager mp = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>();
            mp.PreMulti();
        }
    }

    void StartingTutorial()
    {
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<RectTransform>().sizeDelta = new Vector2(293f, 61.4f);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Start Tutorial";
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("ContentText").GetComponent<Text>().text = "After completing a specific concept's tutorial, a set of blocks will be unlocked\n\nThe first two concepts will unlock 4 new blocks each and the third concept will unlock 8.\n\nUpon completing a B.A.T's concept tutorial, you can use it to answer a quiz question and get 8 of any selected block type you choose from the blocks you have unlocked";
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(CloseTutorialExplanation);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.AddListener(delegate { GameObject.Find("player").transform.GetChild(0).Find("Sounds").GetComponent<SoundManager>().PlayAudio("click"); });
    }

    void CloseTutorialExplanation()
    {
        GameObject.Find("player").GetComponent<PlayerController>().controlsEnabled = true;
        GameObject.Find("Canvas").transform.Find("crosshair").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetChild(0).GetComponent<Text>().text = "Ok!";
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<RectTransform>().sizeDelta = new Vector2(245f, 61.4f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("TitleText").GetComponent<Text>().text = "";
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("ContentText").GetComponent<Text>().text = "";

        GameObject.Find("Canvas").transform.Find("CodeImage").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("CodeImage").transform.Find("OkButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("TutorialTimer").GetComponent<TutorialTimer>().StartTimer = true;
    }

    IEnumerator GenerateChunks()
    {
        chunkSpawnCues.Clear();

        for (int x = playerChunkPos.x - (int)viewDistance; x <= playerChunkPos.x + viewDistance; x++)
        {
            for (int y = playerChunkPos.y - (int)(viewDistance / 2); y <= playerChunkPos.y + (viewDistance / 2); y++)
            {
                for (int z = playerChunkPos.z - (int)viewDistance; z <= playerChunkPos.z + viewDistance; z++)
                {
                    if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight || z < 0 || z >= worldWidth)
                        continue;

                    Vector3Int chunkPos = new Vector3Int(x, y, z);

                    if (Vector3Int.Distance(playerChunkPos, chunkPos) <= viewDistance &&
                        !InvisibleChunk(x, y, z) && !chunks[x, y, z])
                        chunkSpawnCues.Add(chunkPos);

                    if (!InvisibleChunk(x, y, z) && Random.Range(0, 10) < 5) yield return 0;
                }
            }
        }

        chunkSpawnCues.Sort
        (
            delegate (Vector3Int a, Vector3Int b)
            {
                return Vector3Int.Distance(playerPos, a * chunkSize).CompareTo(Vector3Int.Distance(playerPos, b * chunkSize));
            }
        );

        counter = 0;
        SpawnNextChunk();
    }

    void SpawnNextChunk()
    {
        if (counter >= chunkSpawnCues.Count) counter = 0;
        if (chunkSpawnCues.Count == 0) return;

        int x = chunkSpawnCues[counter].x;
        int y = chunkSpawnCues[counter].y;
        int z = chunkSpawnCues[counter].z;

        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight || z < 0 || z >= worldWidth)
        {
            if (counter >= chunkSpawnCues.Count - 1)
            {
                finishedGeneratingChunks = true;
                counter = 0;
            }
            else
            {
                counter++;
                SpawnNextChunk();
            }
            return;
        }

        if (!chunks[x, y, z])
        {
            Vector3 spawnPos = new Vector3(x, y, z) * 16;
            Chunk newChunk = Instantiate(chunk, spawnPos, Quaternion.identity, this.transform) as Chunk;

            if (gMode == GraphicsMode.Fast)
                newChunk.GetComponent<Renderer>().material = fastChunkMat;
            if (gMode == GraphicsMode.Fancy)
                newChunk.GetComponent<Renderer>().material = standardChunkMat;
            if (gMode == GraphicsMode.Insane)
                newChunk.GetComponent<Renderer>().material = epicImageEffectsChunkMat;

            chunks[x, y, z] = newChunk;
        }
        if (counter >= chunkSpawnCues.Count - 1)
        {
            finishedGeneratingChunks = true;
            counter = 0;
        }
        else
        {
            Invoke("SpawnNextChunk", 0.05f);
            counter++;
        }
    }

    public void ForceLoadChunkAt(int x, int y, int z)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight || z < 0 || z >= worldWidth)
            return;

        if (chunks[x, y, z]) return;
        if (InvisibleChunk(x, y, z)) return;

        Vector3 spawnPos = new Vector3(x, y, z) * 16;
        Chunk newChunk = Instantiate(chunk, spawnPos, Quaternion.identity, this.transform) as Chunk;


        if (gMode == GraphicsMode.Fast)
            newChunk.GetComponent<Renderer>().material = fastChunkMat;
        if (gMode == GraphicsMode.Fancy)
            newChunk.GetComponent<Renderer>().material = standardChunkMat;
        if (gMode == GraphicsMode.Insane)
            newChunk.GetComponent<Renderer>().material = epicImageEffectsChunkMat;

        chunks[x, y, z] = newChunk;
    }

    public byte GetBlockUnderPlayer()
    {
        int x = playerPos.x;
        int y = playerPos.y;
        int z = playerPos.z;

        return GetBlock(x, y - 2, z);
    }

    public void SpawnLandParticles()
    {
        if (GetBlockUnderPlayer() == 0) return;

        float x = player.position.x;
        float y = player.position.y;
        float z = player.position.z;

        GameObject p = Instantiate(particles, new Vector3(x, y - 1, z), particles.transform.rotation) as GameObject;
        p.GetComponent<Renderer>().material.mainTexture = particleTextures[(int)GetBlockUnderPlayer() - 1];
    }

    public byte GetBlock(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= worldBlockWidth || z >= worldBlockWidth) return 1;
        if (y >= worldBlockHeight) return 0;

        if (world == null) return 0;

        return world[x, y, z];
    }

    public bool BlockIsObscured(int x, int y, int z)
    {
        bool up = GetBlock(x, y + 1, z) != 0;
        bool down = GetBlock(x, y - 1, z) != 0;
        bool right = GetBlock(x + 1, y, z) != 0;
        bool left = GetBlock(x - 1, y, z) != 0;
        bool fwd = GetBlock(x, y, z + 1) != 0;
        bool back = GetBlock(x, y, z - 1) != 0;

        return up && down && right && left && fwd && back;
    }

    public bool InvisibleChunk(int xPos, int yPos, int zPos)
    {
        bool invisible = true;

        for (int x = xPos * chunkSize; x < xPos * chunkSize + chunkSize; x++)
        {
            for (int y = yPos * chunkSize; y < yPos * chunkSize + chunkSize; y++)
            {
                for (int z = zPos * chunkSize; z < zPos * chunkSize + chunkSize; z++)
                {
                    if (GetBlock(x, y, z) != 0)
                    {
                        if (!BlockIsObscured(x, y, z))
                        {
                            invisible = false;
                        }
                    }
                    if (!invisible) break;
                    else invisible = true;
                }
                if (!invisible) break;
            }
            if (!invisible) break;
        }

        return invisible;
    }

    public bool ChunkIsObscured(int x, int y, int z)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight || z < 0 || z >= worldWidth)
            return false;

        bool up = ChunkExistsAt(x, y + 1, z);
        bool down = ChunkExistsAt(x, y - 1, z);
        bool right = ChunkExistsAt(x + 1, y, z);
        bool left = ChunkExistsAt(x - 1, y, z);
        bool fwd = ChunkExistsAt(x, y, z + 1);
        bool back = ChunkExistsAt(x, y, z - 1);

        return up && down && right && left && fwd && back && chunks[x, y, z];
    }

    public bool ChunkExistsAt(int x, int y, int z)
    {
        if (x < 0 || x >= worldWidth || y < 0 || z < 0 || z >= worldWidth)
            return false;

        return chunks[x, y, z] != null;
    }

    public bool ChunkIsWithinBounds(int x, int y, int z)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight || z < 0 || z >= worldWidth)
            return false;
        return true;
    }

    void TrySetBlock(int x, int y, int z, byte block)
    {
        if (x < 0 || x >= worldBlockWidth || y < 0 || y >= worldBlockHeight || z < 0 || z >= worldBlockWidth)
        {
            return;
        }
        world[x, y, z] = block;
    }

    public void PlaceBlock(int x, int y, int z, byte block)
    {
        print("y level: " + y + " : WorldHeight: " + worldBlockHeight);
        if (x < 0 || x >= worldBlockWidth || y < 0 || y >= worldBlockHeight || z < 0 || z >= worldBlockWidth)
            return;

        /*if (block == 0 && world[x, y, z] != 0 && world[x, y, z] != 4 && world[x, y, z] < 30)
        {
            GameObject p = Instantiate(particles, new Vector3(x + 0.5f, y + 0.3f, z - 0.5f), particles.transform.rotation) as GameObject;
            p.GetComponent<Renderer>().material.mainTexture = particleTextures[(int)world[x, y, z] - 1];
        }*/

        if (world[x, y, z] == 4) return;

        world[x, y, z] = block;
        if (block == 0 && y <= worldBlockHeight - 1 && GetBlock(x, y + 1, z) >= 29) world[x, y + 1, z] = 0;
    }

    public bool BlockIsShadedAt(int x, int y, int z)
    {
        if (x < 0 || x >= worldBlockWidth || y < 0 || y >= worldBlockHeight || z < 0 || z >= worldBlockWidth)
            return false;

        for (int y1 = y; y1 < worldBlockHeight; y1++)
        {
            if (y1 == y) continue;

            if (GetBlock(x, y1, z) != 0) return true;
        }
        return false;
    }

    public void ChangeGraphicsMode()
    {
        StartCoroutine(UpdateGraphicsMode());
    }

    IEnumerator UpdateGraphicsMode()
    {
        if (gMode == GraphicsMode.Fast) gMode = GraphicsMode.Fancy;
        else if (gMode == GraphicsMode.Fancy) gMode = GraphicsMode.Insane;
        else gMode = GraphicsMode.Fast;

        if (gMode == GraphicsMode.Insane)
        {
            bloomEffect.enabled = true;
            motionBlurEffect.enabled = true;
            RenderSettings.fog = true;
            occlusionEffect.enabled = true;
            sun.shadows = LightShadows.Soft;
        }
        else
        {
            bloomEffect.enabled = false;
            motionBlurEffect.enabled = false;
            RenderSettings.fog = false;
            occlusionEffect.enabled = false;
            sun.shadows = LightShadows.None;
        }

        Material chunkMat;

        if (gMode == GraphicsMode.Fast) chunkMat = fastChunkMat;
        else if (gMode == GraphicsMode.Fancy) chunkMat = standardChunkMat;
        else chunkMat = epicImageEffectsChunkMat;

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                for (int z = 0; z < worldWidth; z++)
                {
                    if (chunks[x, y, z]) Destroy(chunks[x, y, z].gameObject);
                    else continue;

                    Vector3 spawnPos = new Vector3(x, y, z) * 16;
                    Chunk newChunk = Instantiate(chunk, spawnPos, Quaternion.identity, this.transform) as Chunk;

                    if (gMode == GraphicsMode.Fast)
                        newChunk.GetComponent<Renderer>().material = fastChunkMat;
                    if (gMode == GraphicsMode.Fancy)
                        newChunk.GetComponent<Renderer>().material = standardChunkMat;
                    if (gMode == GraphicsMode.Insane)
                        newChunk.GetComponent<Renderer>().material = epicImageEffectsChunkMat;

                    chunks[x, y, z] = newChunk;

                    if (Random.Range(0, 100) < 5) yield return 0;
                }
            }
        }

        if (gMode == GraphicsMode.Fast)
        {
            GenerateChunksAroundPlayer();
            GenerateChunks();
        }
    }

    public void SaveWorld()
    {
        print("Saving world..");
        PlayerIO io = FindObjectOfType<PlayerIO>();

        currentWorldData.music = PauseMenu.pauseMenu.music;
        currentWorldData.hotbar = io.hotbarBlocks;
        currentWorldData.curHotbarSlot = io.currentSlot;
        currentWorldData.gMode = gMode;
        currentWorldData.invertMouse = PauseMenu.pauseMenu.invertMouse;
        currentWorldData.map = world;

        FileStream stream = new FileStream(WorldName + ".data", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();

        try
        {
            formatter.Serialize(stream, currentWorldData);
            print("Save Successful");
        }
        catch (System.Runtime.Serialization.SerializationException e)
        {
            Debug.Log("Failed to serialize world data. Reason: " + e.Message);
            return;
        }
        finally
        {
            stream.Close();
        }
    }

    public void LoadWorld()
    {
        print("Searching for world");
        if (!File.Exists(WorldName + ".data"))
        {
            print("World Not found");
            return;
        }

        FileStream stream = new FileStream(WorldName + ".data", FileMode.Open);
        if (stream != null)
        {
            print("World Found");
        }
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            currentWorldData = formatter.Deserialize(stream) as WorldData;

            formatter = null;
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


        PlayerIO io = FindObjectOfType<PlayerIO>();

        if (WorldName == "Tutorial")
        {
            io.hotbarBlocksAmount = new int[] { 20, 0, 0, 0, 0, 0, 0, 0, 0 };
            io.hotbarBlocks = new byte[] { 1, 34, 34, 34, 34, 34, 34, 34, 34 };

            for (int i = 0; i < io.hotbarBlockSprites.Length; i++)
            {
                if (i == 0)
                {
                    io.hotbarBlockSprites[i].sprite = io.spritesByBlockID[0];
                    io.hotbarBlockSprites[i].gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "16";
                }
                else
                {
                    io.hotbarBlockSprites[i].sprite = io.spritesByBlockID[33];
                }
            }
        }
        else if(WorldName == "SinglePlayer")
        {
            io.hotbarBlocksAmount = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            io.hotbarBlocks = new byte[] { 34, 34, 34, 34, 34, 34, 34, 34, 34 };

            for (int i = 0; i < io.hotbarBlockSprites.Length; i++)
            {
                if (i == 0)
                {
                    io.hotbarBlockSprites[i].sprite = io.spritesByBlockID[0];
                    io.hotbarBlockSprites[i].gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                }
                else
                {
                    io.hotbarBlockSprites[i].sprite = io.spritesByBlockID[33];
                }
            }
        }
        else
        {
            io.hotbarBlocks = currentWorldData.hotbar;
            io.currentSlot = currentWorldData.curHotbarSlot;
        }
        gMode = currentWorldData.gMode;
        PauseMenu.pauseMenu.music = currentWorldData.music;
        PauseMenu.pauseMenu.invertMouse = currentWorldData.invertMouse;
        world = currentWorldData.map;

        worldIsFromSaveFile = true;


    }

    public void GenerateNewWorld()
    {
        Invoke("NewWorldGen", 0.2f);
    }

    void NewWorldGen()
    {
        if (!DontSaveWorld)
        {
            SaveWorld();
        }

        currentWorldData.map = null;

        FileStream stream = new FileStream(WorldName + ".data", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();

        try
        {
            formatter.Serialize(stream, currentWorldData);
        }
        catch (System.Runtime.Serialization.SerializationException e)
        {
            Debug.Log("Failed to serialize world data. Reason: " + e.Message);
            return;
        }
        finally
        {
            stream.Close();
        }

        SceneManager.LoadScene(1);
    }

    void OnApplicationQuit()
    {
        if (!DontSaveWorld)
        {
            SaveWorld();
        }
    }

    void ShaveWorld()
    {
        int temp1 = 0;
        int temp2 = 0;
        try
        {
            for (int i = 0; i < worldWidth; i++)
            {
                temp1 = i;
                for (int k = 0; k < worldBlockWidth; k++)
                {
                    for (int j = 0; j < worldWidth; j++)
                    {
                        temp2 = j;
                        PlaceBlock(i, k, j, 0);
                    }
                }
            }

            for (int i = 0; i < worldWidth; i++)
            {
                temp1 = i;
                for (int k = 0; k < worldBlockWidth; k++)
                {
                    for (int j = 0; j < worldWidth; j++)
                    {
                        chunks[i, k, j].Regenerate();
                    }
                }
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            print("Error at world[" + temp1 + ",37," + temp2 + "]");
        }
    }
}

[System.Serializable]
public class WorldData
{
    public byte[,,] map;
    public float xPos;
    public float yPos;
    public float zPos;
    public float playerYRot;
    public float playerXRot;

    public byte[] hotbar;
    public int curHotbarSlot;

    public bool music;
    public GraphicsMode gMode;
    public bool invertMouse;
}