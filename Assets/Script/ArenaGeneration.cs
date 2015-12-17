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
    private int[,][] wallArena;
    private float wallHeight;
    private float floorSide;
    private int randomSpawnWall;
    private int wallOrientation;
	private GameObject papaSol;
	private GameObject papaMur;

    public static ArenaGeneration instance = null;

    void Awake()
    {
        if(ArenaGeneration.instance == null)
        {
            instance = this;
        }
        else if(ArenaGeneration.instance != this)
        {
            Destroy(this.gameObject);
        }
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
        wallArena = new int[height, width][];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // Création du tableau des murs dans la case [i, j]
                wallArena[i, j] = new int[4];
            }
        }
        for (int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                // Création du sol de l'arène et envoi dans la fonction de création des murs random
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
        // Création des murs autour de l'arène
        if (a == 0)
        {
            // left
            wallArena[a, b][2] = 1;
            GameObject currentWallContour = Instantiate(wall, new Vector3(a - floorSide, wallHeight, b), Quaternion.Euler(0, 90, 0)) as GameObject;
			currentWallContour.transform.parent = papaMur.transform;
        }
        if (a == height - 1)
        {
            // right
            wallArena[a, b][1] = 1;
            GameObject currentWallContour = Instantiate(wall, new Vector3(a + floorSide, wallHeight, b), Quaternion.Euler(0, 90, 0)) as GameObject;
			currentWallContour.transform.parent = papaMur.transform;
        }
        if (b == 0)
        {
            // bottom
            wallArena[a, b][3] = 1;
            GameObject currentWallContour = Instantiate(wall, new Vector3(a, wallHeight, b - floorSide), Quaternion.identity) as GameObject;
			currentWallContour.transform.parent = papaMur.transform;
        }
        if (b == width - 1)
        {
            // top
            wallArena[a, b][0] = 1;
            GameObject currentWallContour = Instantiate(wall, new Vector3(a, wallHeight, b + floorSide), Quaternion.identity) as GameObject;
			currentWallContour.transform.parent = papaMur.transform;
        }
		
		
		// Création des murs au milieu de l'arène
        randomSpawnWall = Random.Range(0, 100);
        if(randomSpawnWall < chanceSpawnWall)
        {
            GameObject currentWall = null;

            wallOrientation = Random.Range(0, 4);
            switch (wallOrientation)
            {
                case 0:
                    // bottom
                    try
                    {
                        // La case du dessous récupère le mur au dessus
                        wallArena[a, b - 1][0] = 1;
                    }
                    catch
                    {
                        // La cellule [a, b - 1] n'existe pas
                    }
                    wallArena[a, b][3] = 1;
                    currentWall = Instantiate(wall, new Vector3(a, wallHeight, b - floorSide), Quaternion.Euler(0, 0, 0)) as GameObject;
                    break;
                case 1:
                    // left
                    try
                    {
                        // La case à gauche récupère le mur à sa droite
                        wallArena[a - 1, b][1] = 1;
                    }
                    catch
                    {
                        // La cellule [a - 1, b] n'existe pas
                    }
                    wallArena[a, b][2] = 1;
                    currentWall = Instantiate(wall, new Vector3(a - floorSide, wallHeight, b), Quaternion.Euler(0, 90, 0)) as GameObject;
                    break;
                case 2:
                    // top
                    try
                    {
                        // La case du dessous récupère le mur au dessous
                        wallArena[a, b + 1][3] = 1;
                    }
                    catch
                    {
                        // La cellule [a, b + 1] n'existe pas
                    }
                    wallArena[a, b][0] = 1;
                    currentWall = Instantiate(wall, new Vector3(a, wallHeight, b + floorSide), Quaternion.Euler(0, 180, 0)) as GameObject;
                    break;
                case 3:
                    // right
                    try
                    {
                        // La case de droite récupère le mur à sa gauche
                        wallArena[a + 1, b][2] = 1;
                    }
                    catch
                    {
                        // La cellule [a + 1, b] n'existe pas
                    }
                    wallArena[a, b][1] = 1;
                    currentWall = Instantiate(wall, new Vector3(a + floorSide, wallHeight, b), Quaternion.Euler(0, 270, 0)) as GameObject;
                    break;
            }
            currentWall.transform.parent = papaMur.transform;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Commande de Debug pour tester la génération de l'arène
            Application.LoadLevel(Application.loadedLevel);
        }
    }


    // Ce n'est pas moi qui ait écrit cette fonction, du moins je ne me souviens pas
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
