using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : EntityController
{
    List<GameObject> tiles;
    public List<CubeScript> roadOfTiles;

    void Start()
    {
        canMove = true;
        canAttack = true;

        tiles = new List<GameObject>();

        Vector3 behind = -transform.TransformDirection(Vector3.up);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, behind, out hit))
        {
            actualPosition = new Vector2(hit.transform.position.x, hit.transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!TurnManager.Instance.canPlay(id)) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {

            Debug.Log("turn of player  " + id);

            repaint(tiles);
            if (canMove == true)
            {
                canMove = false;
            }
            else
            {
                EndTurn();
            }
        }
        else if (canMove && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                if (hit.transform.tag == "Tile" && hit.transform.gameObject.GetComponent<Renderer>().material.color == Color.blue)
                {
                    {
                        hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                        //transform.Translate(new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z));
                        //transform.position = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
                        canMove = false;
                        roadOfTiles = MovementManager.Instance.findPath(transform.position, hit.transform.position);

                        if (roadOfTiles.Count > 0)
                            StartCoroutine(move(roadOfTiles));
                    }
                }

            }
        }

        if (canAttack && !moovng && !canMove)
        {

            for (int i = (int)actualPosition.x - range; i <= (int)actualPosition.x + range; i++)
            {
                for (int j = (int)actualPosition.y - range; j <= (int)actualPosition.y + range; j++)
                {
                    if (Mathf.Abs(j - (int)actualPosition.y) + Mathf.Abs(i - (int)actualPosition.x) <= range)
                    {
                        GameObject go = ArenaManager.Instance.getTile(i, j); if (go)
                        {
                            go.gameObject.GetComponent<Renderer>().material.color = Color.red;
                            tiles.Add(go);
                        }
                    }
                }
            }
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray.origin, ray.direction, out hit) && (hit.transform.tag == "Enemy" || hit.transform.tag == "Player"))
                {
                    Vector3 behind = -hit.transform.TransformDirection(Vector3.up);

                    RaycastHit rhit;

                    if (Physics.Raycast(transform.position, behind, out rhit) && rhit.transform.gameObject.GetComponent<Renderer>().material.color == Color.red)
                    {
                        Debug.Log("attack enemy at position " + hit.transform.position);
                        hit.transform.gameObject.GetComponent<EntityController>().TakeDamage();
                        EndTurn();
                    }
                }
            }
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
                        go.gameObject.GetComponent<Renderer>().material.color = Color.blue;
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

    void repaint(List<GameObject> tiles)
    {
        foreach(GameObject tile in tiles)
        {
            tile.gameObject.GetComponent<Renderer>().material.color = Color.white;
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

        actualPosition = new Vector2(lastTarget.x, lastTarget.z);
        repaint(tiles);
        roadOfTiles.Clear();
        moovng = false;
    }

    public override void BeginTurn()
    {
        Debug.Log("begin player");
        recolor();
    }

    public override void TakeDamage()
    {
        Destroy(gameObject);
    }

}
