using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ArenaManager: MonoBehaviour
{
    public GameObject floor;
    public GameObject wall;
    public GameObject player;
    public List<CubeScript> listCubeScript;

    public int height;
    public int width;
    public int chanceSpawnWall;

    public GameObject[,] arena;
    public int[,][] wallArena;

    private GameObject[,][] wallObject;
    private float wallHeight;
    private float floorSide;
    private int randomSpawnWall;
    private int wallOrientation;
	private GameObject papaSol;
	private GameObject papaMur;
    private bool end;

    GameObject[] players;

    static ArenaManager instance;
    public static ArenaManager Instance
    {
        get
        {
            return instance;
        }
    }
    void Awake()
    {
        end = false;
        GameObject GrandPa = new GameObject();
        instance = this;

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
        wallObject = new GameObject[height, width][];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // Création du tableau des murs dans la case [i, j]
                wallArena[i, j] = new int[4];
                wallObject[i, j] = new GameObject[4];
            }
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // Création du sol de l'arène et envoi dans la fonction de création des murs random
                WallCreation(i, j);
                GameObject currentFloor = Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity) as GameObject;
                CubeScript CS = currentFloor.GetComponent<CubeScript>();
                CS.tile = new int[] { i, j };
                listCubeScript.Add(CS);
                arena[i, j] = currentFloor;
                currentFloor.transform.parent = papaSol.transform;
            }
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                WallDestruct(i, j);
            }
        }

        //Check voisins
        CheckNeighboors();


        while (end == false)
        {
            Colorate();
            Check();
        }


        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                arena[i, j].GetComponent<Renderer>().material.color = Color.white;
            }
        }

        GameObject go = Instantiate(player, new Vector3(0, player.transform.localScale.y / 2, 0), Quaternion.identity) as GameObject;
        Camera.main.transform.parent = go.transform;
        Camera.main.transform.localPosition = new Vector3(0, 10, 0);
        GameObject go2 = Instantiate(player, new Vector3(width - 1, player.transform.localScale.y / 2, 0), Quaternion.identity) as GameObject;

        GameObject go3 = Instantiate(player, new Vector3(0, player.transform.localScale.y / 2, height - 1), Quaternion.identity) as GameObject;

        GameObject go4 = Instantiate(player, new Vector3(width - 1, player.transform.localScale.y / 2, height - 1), Quaternion.identity) as GameObject;

        players = new GameObject[10];

        players[1] = go;
        players[2] = go2;
        players[3] = go3;
        players[4] = go4;

    }

    void Start()
    {
        foreach (SpellManager.SPELL spell in SpellManager.Instance.spellsP1)
        {
            players[1].GetComponent<EntityController>().AddSpell(spell);
        }
        foreach (SpellManager.SPELL spell in SpellManager.Instance.spellsP2)
        {
            players[2].GetComponent<EntityController>().AddSpell(spell);
        }
        foreach (SpellManager.SPELL spell in SpellManager.Instance.spellsP3)
        {
            players[3].GetComponent<EntityController>().AddSpell(spell);
        }
        foreach (SpellManager.SPELL spell in SpellManager.Instance.spellsP4)
        {
            players[4].GetComponent<EntityController>().AddSpell(spell);
        }
    }

    void CheckNeighboors()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                CubeScript cs = arena[i, j].GetComponent<CubeScript>();
                if (getTile(i, j+1) != null)
                {
                    if (wallArena[i, j][0] == 0) cs.neighbor.Add(arena[i, j + 1].GetComponent<CubeScript>());
                }
                if (getTile(i+1, j) != null)
                {
                    if (wallArena[i, j][1] == 0) cs.neighbor.Add(arena[i+1,j].GetComponent<CubeScript>());
                }
                if (getTile(i - 1, j) != null)
                {
                    if (wallArena[i, j][2] == 0) cs.neighbor.Add(arena[i-1,j].GetComponent<CubeScript>());
                }
                if (getTile(i,j- 1) != null)
                {
                    if (wallArena[i, j][3] == 0) cs.neighbor.Add(arena[i,j-1].GetComponent<CubeScript>());
                }


            }
        }
    }

    // Cette méthode permet de créer les murs autour et dans l'arène
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
		
        randomSpawnWall = Random.Range(0, 100);
        if(randomSpawnWall < chanceSpawnWall)
        {
            GameObject currentWall = null;

            wallOrientation = Random.Range(0, 4);
            switch (wallOrientation)
            {
                case 0:
                    // bottom
                    if(wallArena[a, b][3] != 1)
                    {
                        wallArena[a, b][3] = 1;
                        currentWall = Instantiate(wall, new Vector3(a, wallHeight, b - floorSide), Quaternion.Euler(0, 0, 0)) as GameObject;
                        currentWall.transform.parent = papaMur.transform;
                        wallObject[a, b][3] = currentWall;
                        try
                        {
                            // La case du dessous récupère le mur au dessus
                            wallArena[a, b - 1][0] = 1;
                            wallObject[a, b - 1][0] = currentWall;
                        }
                        catch
                        {
                            // La cellule [a, b - 1] n'existe pas
                        }
                    }
                    break;
                case 1:
                    // left
                    if(wallArena[a, b][2] != 1)
                    {
                        wallArena[a, b][2] = 1;
                        currentWall = Instantiate(wall, new Vector3(a - floorSide, wallHeight, b), Quaternion.Euler(0, 90, 0)) as GameObject;
                        currentWall.transform.parent = papaMur.transform;
                        wallObject[a, b][2] = currentWall;
                        try
                        {
                            // La case à gauche récupère le mur à sa droite
                            wallArena[a - 1, b][1] = 1;
                            wallObject[a - 1, b][1] = currentWall;
                        }
                        catch
                        {
                            // La cellule [a - 1, b] n'existe pas
                        }
                    }
                    break;
                case 2:
                    // top
                    if(wallArena[a, b][0] != 1)
                    {
                        wallArena[a, b][0] = 1;
                        currentWall = Instantiate(wall, new Vector3(a, wallHeight, b + floorSide), Quaternion.Euler(0, 180, 0)) as GameObject;
                        currentWall.transform.parent = papaMur.transform;
                        wallObject[a, b][0] = currentWall;
                        try
                        {
                            // La case du dessous récupère le mur au dessous
                            wallArena[a, b + 1][3] = 1;
                            wallObject[a, b + 1][3] = currentWall;
                        }
                        catch
                        {
                            // La cellule [a, b + 1] n'existe pas
                        }
                    }
                    break;
                case 3:
                    // right
                    if(wallArena[a, b][1] != 1)
                    {
                        wallArena[a, b][1] = 1;
                        currentWall = Instantiate(wall, new Vector3(a + floorSide, wallHeight, b), Quaternion.Euler(0, 270, 0)) as GameObject;
                        currentWall.transform.parent = papaMur.transform;
                        wallObject[a, b][1] = currentWall;
                        try
                        {
                            // La case de droite récupère le mur à sa gauche
                            wallArena[a + 1, b][2] = 1;
                            wallObject[a + 1, b][2] = currentWall;
                        }
                        catch
                        {
                            // La cellule [a + 1, b] n'existe pas
                        }
                    }
                    break;
            }
        }
    }

    // Cette méthode permet de détruire un mur qui est sur une case avec 4 murs
    void WallDestruct(int a, int b)
    {
        int nbMurs = 0;
        for(int i = 0; i < 4; i++)
        {
            if(wallArena[a,b][i] == 1)
            {
                nbMurs++;
            }
        }
        if(nbMurs == 4)
        {
            RandomDestruct(a, b);
        }
    }

    // Destruction aléatoire d'un mur, en prenant garde que ça ne soit pas un qui est sur le bord de l'arène (boucle jusqu'à ce que la destruction soit valable)
    void RandomDestruct(int a, int b)
    {
        bool isPossible = true;
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0:
                //Top
                if (b == width - 1)
                {
                    isPossible = false;
                }
                break;
            case 1:
                //Right
                if (a == height - 1)
                {
                    isPossible = false;
                }
                break;
            case 2:
                //Left
                if(a == 0)
                {
                    isPossible = false;
                }
                break;
            case 3:
                //Bottom
                if (b == 0)
                {
                    isPossible = false;
                }
                break;
        }
        if(wallArena[a,b][rand] == 0)
        {
            isPossible = false;
        }
        if(isPossible)
        {
            GameObject currentWall = wallObject[a, b][rand];
            wallArena[a, b][rand] = 0;
            Destroy(currentWall.gameObject);
            try
            {
                switch (rand)
                {
                    case 0:
                        //Top
                        wallArena[a, b + 1][3] = 0;
                        break;
                    case 1:
                        //Right
                        wallArena[a + 1, b][2] = 0;
                        break;
                    case 2:
                        //Left
                        wallArena[a - 1, b][1] = 0;
                        break;
                    case 3:
                        //Bottom
                        wallArena[a, b - 1][0] = 0;
                        break;
                }
            }
            catch
            {
                // La case n'existe pas
            }
        }
        else
        {
            RandomDestruct(a, b);
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

    // Permet de détecter toutes les cases accessibles
    void Colorate()
    {
        bool test = false;
        arena[0, 0].gameObject.GetComponent<Renderer>().material.color = Color.blue;
        while (test == false)
        {
            test = true;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (arena[i, j].gameObject.GetComponent<Renderer>().material.color == Color.blue)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (wallArena[i, j][k] == 0)
                            {
                                switch (k)
                                {
                                    case 0:
                                        if (arena[i, j + 1].gameObject.GetComponent<Renderer>().material.color != Color.blue)
                                        {
                                            arena[i, j + 1].gameObject.GetComponent<Renderer>().material.color = Color.red;
                                        }
                                        break;
                                    case 1:
                                        if (arena[i + 1, j].gameObject.GetComponent<Renderer>().material.color != Color.blue)
                                        {
                                            arena[i + 1, j].gameObject.GetComponent<Renderer>().material.color = Color.red;
                                        }
                                        break;
                                    case 2:
                                        if (arena[i - 1, j].gameObject.GetComponent<Renderer>().material.color != Color.blue)
                                        {
                                            arena[i - 1, j].gameObject.GetComponent<Renderer>().material.color = Color.red;
                                        }
                                        break;
                                    case 3:
                                        if (arena[i, j - 1].gameObject.GetComponent<Renderer>().material.color != Color.blue)
                                        {
                                            arena[i, j - 1].gameObject.GetComponent<Renderer>().material.color = Color.red;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (arena[i, j].gameObject.GetComponent<Renderer>().material.color == Color.red)
                    {
                        test = false;
                        arena[i, j].gameObject.GetComponent<Renderer>().material.color = Color.blue;
                    }
                }
            }
        }
    }

    // Permet de savoir si toutes les cases sont accessibles
    void Check()
    {
        bool ok = true;
        int a = 0, b = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if(arena[i, j].GetComponent<Renderer>().material.color != Color.blue)
                {
                    if(ok)
                    {
                        a = i;
                        b = j;
                    }
                    ok = false;
                }
            }
        }
        if(!ok)
        {
            RandomDestruct(a, b);
        }
        else
        {
            end = true;
        }
    }

    // Ce n'est pas moi qui ait écrit cette fonction, du moins je ne me souviens pas
    public GameObject getTile(int x, int y)
    {
        if (x < height && y < width  && x>=0 && y>=0)
        {
            return arena[x, y];
        }
        else
        {
            return null;
        }
    }
}
