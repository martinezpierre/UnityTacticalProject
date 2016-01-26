using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : EntityController
{
    public List<CubeScript> roadOfTiles;

    bool dropdownCreated = false;

    bool willMove = false;

    Color movColor;


    protected override void Start()
    {
        base.Start();
        
        canMove = true;
        canAttack = true;

        tiles = new List<GameObject>();

        UpdatePosition();

        movColor = MovementManager.Instance.movementColor;
    }

    // Update is called once per frame
    protected override void Update()
    {

        base.Update();

        if (!TurnManager.Instance.canPlay(id)) return;
        
        MoveAction();
        AttackAction();
        
    }

    public override void SkipAction()
    {
        repaint(tiles);
        if (canMove == true)
        {
            canMove = false;
            willMove = false;
        }
        else
        {
            dropdownCreated = false;
            EndTurn();
        }
    }
    
    public override void TileToMoveSelected()
    {
        willMove = true;
    }

    void MoveAction()
    {
        if (canMove && willMove)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                if (hit.transform.tag == "Tile" && hit.transform.gameObject.GetComponent<Renderer>().material.color == SpellManager.Instance.tileSelectedColor)
                {
                    {
                        //transform.Translate(new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z));
                        //transform.position = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
                        canMove = false;
                        willMove = false;
                        roadOfTiles = MovementManager.Instance.findPath(transform.position, hit.transform.position);

                        anim.Play("Walk");

                        if (roadOfTiles.Count > 0)
                            StartCoroutine(move(roadOfTiles));
                    }
                }

            }
        }
    }

    void AttackAction()
    {
        if (canAttack && !moovng && !canMove && !dropdownCreated)
        {

            SpellManager.Instance.CreateDropDown(this);
            dropdownCreated = true;

        }
    }

    void recolor()
    {
        for(int i =0; i < maxMove; i++)
        {
            Colorate();
        }
    }
    void Colorate()
    {
        GameObject[,] arena = ArenaManager.Instance.arena;
        int[,][] wallArena = ArenaManager.Instance.wallArena;
        int height = ArenaManager.Instance.height;
        int width = ArenaManager.Instance.width;
        tiles.Add(arena[(int)actualPosition.x, (int)actualPosition.y]);
        arena[(int)actualPosition.x, (int)actualPosition.y].gameObject.GetComponent<Renderer>().material.color = movColor;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (arena[i, j].gameObject.GetComponent<Renderer>().material.color == movColor)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (wallArena[i, j][k] == 0)
                        {
                            switch (k)
                            {
                                case 0:
                                    if (arena[i, j + 1].gameObject.GetComponent<Renderer>().material.color != movColor)
                                    {
                                        arena[i, j + 1].gameObject.GetComponent<Renderer>().material.color = Color.red;
                                    }
                                    break;
                                case 1:
                                    if (arena[i + 1, j].gameObject.GetComponent<Renderer>().material.color != movColor)
                                    {
                                        arena[i + 1, j].gameObject.GetComponent<Renderer>().material.color = Color.red;
                                    }
                                    break;
                                case 2:
                                    if (arena[i - 1, j].gameObject.GetComponent<Renderer>().material.color != movColor)
                                    {
                                        arena[i - 1, j].gameObject.GetComponent<Renderer>().material.color = Color.red;
                                    }
                                    break;
                                case 3:
                                    if (arena[i, j - 1].gameObject.GetComponent<Renderer>().material.color != movColor)
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
                    arena[i, j].gameObject.GetComponent<Renderer>().material.color = movColor;
                    arena[i, j].GetComponent<CubeScript>().SetInteractableForMove(true);
                    tiles.Add(arena[i, j]);
                }
            }
        }
    }


    public void repaint(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.gameObject.GetComponent<Renderer>().material.color = Color.white;
            tile.GetComponent<CubeScript>().SetInteractableForMove(false);
        }
        tiles.Clear();
    }

    IEnumerator move(List<CubeScript> target)
    {
        moovng = true;
        Vector3 lastTarget = target[target.Count - 1].getPosition();

        while (Vector3.Distance(transform.position, lastTarget) > 0.1f)
        {
            Vector3 nextTarget = target[0].getPosition();
            if (Vector3.Distance(transform.position, nextTarget) > 0.1f)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, nextTarget, step);

                transform.LookAt(new Vector3(nextTarget.x,transform.position.y,nextTarget.z));

                yield return 0;
            }
            else if (target.Count != 1)
            {
                target.RemoveAt(0);
                yield return 0;
            }
        }

        UpdatePosition();

        repaint(tiles);
        roadOfTiles.Clear();
        moovng = false;
        
        anim.Stop("Walk");
    }

    public override void BeginTurn()
    {
        base.BeginTurn();
        recolor();
    }

    public override void TakeDamage(int n)
    {

        base.TakeDamage(n);
        

        //Destroy(gameObject);
    }

}