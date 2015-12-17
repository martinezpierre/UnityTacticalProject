using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeScript : MonoBehaviour {

    public bool isFirst = false;
    // pour l'a*

    protected Vector3 position;
    public float distance = 9999;
    public float heurystic = 9999;
    public CubeScript previousTile = null;
    public float cumule = 0;
    public List<CubeScript> neighbor;
    public int[] tile;

    // Use this for initialization
    void Start () {
        position = transform.position + (Vector3.up / 2);
        Invoke("getNeighbors", 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void getNeighbors()
    {
        GameObject go;
        if (ArenaManager.Instance.getTile(tile[0]-1, tile[1]) != null)
        {
            go = ArenaManager.Instance.getTile(tile[0]-1, tile[1]);
            neighbor.Add (go.GetComponent<CubeScript>());
        }
        if (ArenaManager.Instance.getTile(tile[0], tile[1] - 1) != null)
        {
            go = ArenaManager.Instance.getTile(tile[0], tile[1] - 1);
            neighbor.Add(go.GetComponent<CubeScript>());
        }
        if (ArenaManager.Instance.getTile(tile[0] + 1, tile[1] ) != null)
        {
            go = ArenaManager.Instance.getTile(tile[0] + 1, tile[1]);
            neighbor.Add(go.GetComponent<CubeScript>());
        }
        if (ArenaManager.Instance.getTile(tile[0] , tile[1] + 1) != null)
        {
            go = ArenaManager.Instance.getTile(tile[0], tile[1] + 1);
            neighbor.Add(go.GetComponent<CubeScript>());
        }
       
    }
    public Vector3 getPosition()
    {
        return position;
    }
}
