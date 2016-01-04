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
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(canMove == true)
            {
                repaint(tiles);
                canMove = false;
            }
            else
            {
                Debug.Log("end of turn");
                canMove = true;
                canAttack = true;

            }
        }
        else if (canMove)
        {
            for(int i = (int)actualPosition.x - maxMove - range; i <= (int)actualPosition.x + maxMove + range; i++)
            {
                for (int j = (int)actualPosition.y - maxMove - range; j <= (int)actualPosition.y + maxMove + range; j++)
                {
                    //Debug.Log(i + " " + j);
                    if (Mathf.Abs(j- (int)actualPosition.y) + Mathf.Abs(i - (int)actualPosition.x) <= maxMove)
                    {
                        GameObject go = ArenaGeneration.instance.getTile(i, j);
                        if (go)
                        {
                            go.gameObject.GetComponent<Renderer>().material.color = Color.blue;
                            tiles.Add(go);
                        }
                    }else if (Mathf.Abs(j - (int)actualPosition.y) + Mathf.Abs(i - (int)actualPosition.x) <= maxMove + range)
                    {
                        GameObject go = ArenaGeneration.instance.getTile(i, j);
                        if (go)
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
                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    if (hit.transform.tag == "Tile" && hit.transform.gameObject.GetComponent<Renderer>().material.color == Color.blue)
                    {
                        Debug.Log(hit.transform.position.x + " " + hit.transform.position.z);
                        hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                        //transform.Translate(new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z));
                        //transform.position = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z);
                        canMove = false;

                        roadOfTiles = MovementManager.Instance.findPath(transform.position, hit.transform.position);

                        //StartCoroutine(move(hit.transform.position));
                    }
                }
            }
        }
        else if (canAttack && !moovng)
        {
            for (int i = (int)actualPosition.x - range; i <= (int)actualPosition.x + range; i++)
            {
                for (int j = (int)actualPosition.y - range; j <= (int)actualPosition.y + range; j++)
                {
                    if (Mathf.Abs(j - (int)actualPosition.y) + Mathf.Abs(i - (int)actualPosition.x) <= range)
                    {
                        GameObject go = ArenaGeneration.instance.getTile(i, j);
                        if (go)
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

    void repaint(List<GameObject> tiles)
    {
        foreach(GameObject tile in tiles)
        {
            tile.gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
        tiles.Clear();
    }

    IEnumerator move(Vector3 target)
    {
        moovng = true;
        while(transform.position != new Vector3(target.x, transform.position.y, target.z))
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, transform.position.y, target.z), step);
            yield return new WaitForEndOfFrame();
        }
        actualPosition = new Vector2(target.x, target.z);

        repaint(tiles);

        moovng = false;
    }
}
