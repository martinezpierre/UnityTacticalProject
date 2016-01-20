using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    bool canMove = true;
    bool canAttack = true;

    Vector2 actualPosition;

    public int maxMove = 3;
    public int range = 1;

    public float speed = 1f;
    bool deguelass = true;
    bool moovng = false;

    List<GameObject> tiles;
    public List<CubeScript> roadOfTiles;

    void Start()
    {
        tiles = new List<GameObject>();

        Vector3 behind = -transform.TransformDirection(Vector3.up);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, behind, out hit))
        {
            actualPosition = new Vector2(hit.transform.position.x, hit.transform.position.z);
        }
        recolor();
    }

	// Update is called once per frame
    void Update()
    {
        Debug.Log("j'update");

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (canMove == true)
            {
                Debug.Log("coucou");
                repaint(tiles);
                canMove = false;
            }
            else
            {
                Debug.Log("end of turn");
                recolor();
                canMove = true;
                canAttack = true;

            }
        }
        else if (canMove && Input.GetMouseButtonDown(0))
        {
            Debug.Log("je clic");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                if (hit.transform.tag == "Tile" && hit.transform.gameObject.GetComponent<Renderer>().material.color == Color.blue)
                {
                    {
                        Debug.Log(hit.transform.position.x + " " + hit.transform.position.z);
                        hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                        //transform.Translate(new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z));
                        //transform.position = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
                        canMove = false;
                        Debug.Log("je peux bouger !");
                        roadOfTiles = MovementManager.Instance.findPath(transform.position, hit.transform.position);

                        Debug.Log("ça marche encore?");
                        deguelass = false;
                        if (roadOfTiles.Count > 0)
                            StartCoroutine(move(roadOfTiles));
                    }
                }

            }
            if (canAttack && !moovng)
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
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray.origin, ray.direction, out hit) && hit.transform.tag == "Enemy")
                    {
                        Vector3 behind = -hit.transform.TransformDirection(Vector3.up);

                        RaycastHit rhit;

                        if (Physics.Raycast(transform.position, behind, out rhit) && rhit.transform.gameObject.GetComponent<Renderer>().material.color == Color.red)
                        {
                            Debug.Log("attack enemy at position " + hit.transform.position);
                            hit.transform.gameObject.GetComponent<EnemyController>().TakeDamage();
                        }
                    }
                }
            }
        }
    }

    void recolor()
    {
        for (int i = (int)actualPosition.x - maxMove - range; i <= (int)actualPosition.x + maxMove + range; i++)
        {
            for (int j = (int)actualPosition.y - maxMove - range; j <= (int)actualPosition.y + maxMove + range; j++)
            {
                //Debug.Log(i + " " + j);
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
            Debug.Log("je bouge");
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

        actualPosition = new Vector2(lastTarget.x, lastTarget.y);
        repaint(tiles);
        roadOfTiles.Clear();
        moovng = false;
    }
}
