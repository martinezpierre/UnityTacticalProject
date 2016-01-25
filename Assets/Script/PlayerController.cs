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
    void Update()
    {

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
        Debug.Log(actualPosition);
        for (int i = (int)actualPosition.x - maxMove - range; i <= (int)actualPosition.x + maxMove + range; i++)
        {
            for (int j = (int)actualPosition.y - maxMove - range; j <= (int)actualPosition.y + maxMove + range; j++)
            {
                if (Mathf.Abs(j - (int)actualPosition.y) + Mathf.Abs(i - (int)actualPosition.x) <= maxMove)
                {
                    GameObject go = ArenaManager.Instance.getTile(i, j);
                    if (go)
                    {
                        go.gameObject.GetComponent<Renderer>().material.color = movColor;
                        go.GetComponent<CubeScript>().SetInteractableForMove(true);
                        tiles.Add(go);
                    }
                }
                else if (Mathf.Abs(j - (int)actualPosition.y) + Mathf.Abs(i - (int)actualPosition.x) <= maxMove + range)
                {
                    GameObject go = ArenaManager.Instance.getTile(i, j);
                    if (go)
                    {
                        go.gameObject.GetComponent<Renderer>().material.color = Color.red;
                        tiles.Add(go);
                    }
                }
            }
        }
        Debug.Log("recolor");
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
    }

    public override void BeginTurn()
    {
        Debug.Log("begin player");
        recolor();
    }

    public override void TakeDamage(int n)
    {
        base.TakeDamage(n);
        //Destroy(gameObject);
    }

}