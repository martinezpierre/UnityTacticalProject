using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaGeneration : MonoBehaviour
{
    public GameObject floor;
    public GameObject wall;
    public GameObject player;
    public List<CubeScript> listCubeScript;

    public int height;
    public int width;
    public int chanceSpawnWall;

    private GameObject[,] arena;
    private GameObject[,] wallArena;
    private float wallHeight;
    private float floorSide;
    private int randomSpawnWall;
    private int wallOrientation;
	private GameObject papaSol;
	private GameObject papaMur;

    static ArenaGeneration instance;

    public static ArenaGeneration Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
		GameObject GrandPa = new GameObject();
		GrandPa.name = "GrandPa";
		papaSol = new GameObject();
		papaSol.name = "PapaSol";
		papaSol.transform.parent = GrandPa.transform;
		papaMur = new GameObject();
		papaMur.name = "PapaMur";
		papaMur.transform.parent = GrandPa.transform;
        wallHeight = wall.transform.localScale.y / 2;
        floorSide = floor.transform.localScale.x / 2;
        arena = new GameObject[height, width];
        wallArena = new GameObject[height, width];
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                WallCreation(i, j);
                GameObject currentFloor = Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity) as GameObject;
                CubeScript CS = currentFloor.GetComponent<CubeScript>();
                CS.tile = new int[] { i,j };
                listCubeScript.Add(CS);
                arena[i, j] = currentFloor;
                currentFloor.transform.parent = papaSol.transform;

            }
        }

        GameObject go = Instantiate(player, new Vector3(0,player.transform.localScale.y/2,0), Quaternion.identity) as GameObject;
        Camera.main.transform.parent = go.transform;
        Camera.main.transform.localPosition = new Vector3(0, 10, 0);

    }

    void WallCreation(int a, int b)
    {
        if (a == 0)
        {
            GameObject currentWallContour = Instantiate(wall, new Vector3(a - floorSide, wallHeight, b), Quaternion.Euler(0, 90, 0)) as GameObject;
			currentWallContour.transform.parent = papaMur.transform;
        }
        if (a == height - 1)
        {
            GameObject currentWallContour = Instantiate(wall, new Vector3(a + floorSide, wallHeight, b), Quaternion.Euler(0, 90, 0)) as GameObject;
			currentWallContour.transform.parent = papaMur.transform;
        }
        if (b == 0)
        {
            GameObject currentWallContour = Instantiate(wall, new Vector3(a, wallHeight, b - floorSide), Quaternion.identity) as GameObject;
			currentWallContour.transform.parent = papaMur.transform;
        }
        if (b == width - 1)
        {
            GameObject currentWallContour = Instantiate(wall, new Vector3(a, wallHeight, b + floorSide), Quaternion.identity) as GameObject;
			currentWallContour.transform.parent = papaMur.transform;
        }
		
		
		
        randomSpawnWall = Random.Range(0, 100);
        if(randomSpawnWall < chanceSpawnWall)
        {
            GameObject currentWall = null;

            wallOrientation = Random.Range(0, 4);
            switch(wallOrientation)
            {
                case 0:
                    currentWall = Instantiate(wall, new Vector3(a, wallHeight, b - floorSide), Quaternion.Euler(0, 0, 0)) as GameObject;
                    break;
                case 1:
                    currentWall = Instantiate(wall, new Vector3(a - floorSide, wallHeight, b), Quaternion.Euler(0, 90, 0)) as GameObject;
                    break;
                case 2:
                    currentWall = Instantiate(wall, new Vector3(a, wallHeight, b + floorSide), Quaternion.Euler(0, 180, 0)) as GameObject;
                    break;
                case 3:
                    currentWall = Instantiate(wall, new Vector3(a + floorSide, wallHeight, b), Quaternion.Euler(0, 270, 0)) as GameObject;
                    break;
            }

			currentWall.transform.parent = papaMur.transform;
            wallArena[a, b] = currentWall;
        }
        else
        {
            wallArena[a, b] = null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }

    public GameObject getTile(int x, int y)
    {
        if (x < height-1 && y < width-1 && x>=0 && y>=0)
        {
            return arena[x, y];
        }
        else
        {
            return null;
        }
    }
}
