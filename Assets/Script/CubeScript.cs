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
    public List<CubeScript> neighbor = new List<CubeScript>();
    public int[] tile;

    bool interactable = false;
    bool interactableForMove = false;

    Color backupColor;
    Material myMat;

    public EntityController occupant;

    Color selectionColor;

    // Use this for initialization
    void Start () {
        position = transform.position + (Vector3.up / 2);
        //Invoke("getNeighbors", 1);

        selectionColor = SpellManager.Instance.tileSelectedColor;

        myMat = GetComponent<Renderer>().material;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseEnter()
    {
        if (!interactable && !interactableForMove) return;

        backupColor = myMat.color;
        myMat.color = selectionColor;
    }

    void OnMouseExit()
    {
        if (!interactable && !interactableForMove) return;

        myMat.color = backupColor;
    }

    void OnMouseDown()
    {
       if (!interactable && !interactableForMove) return;

        if (interactable)
        {
            SpellManager.Instance.CaseSelected(this);
        }
        else
        {
            TurnManager.Instance.currentPlayer.TileToMoveSelected();
        }
    }

    public void SetInteractable(bool b)
    {
        interactable = b;
    }

    public void SetInteractableForMove(bool b)
    {
        interactableForMove = b;
    }

    /*private void getNeighbors()
    {
        GameObject go;
        if (ArenaManager.Instance.getTile(tile[0]-1, tile[1]) != null)
        {
            go = ArenaManager.Instance.getTile(tile[0]-1, tile[1]);
            neighbor[2] = (go.GetComponent<CubeScript>());
        }
        if (ArenaManager.Instance.getTile(tile[0], tile[1] - 1) != null)
        {
            go = ArenaManager.Instance.getTile(tile[0], tile[1] - 1);
            neighbor[3] = (go.GetComponent<CubeScript>());
        }
        if (ArenaManager.Instance.getTile(tile[0] + 1, tile[1] ) != null)
        {
            go = ArenaManager.Instance.getTile(tile[0] + 1, tile[1]);
            neighbor[1] = (go.GetComponent<CubeScript>());
        }
        if (ArenaManager.Instance.getTile(tile[0] , tile[1] + 1) != null)
        {
            go = ArenaManager.Instance.getTile(tile[0], tile[1] + 1);
            neighbor[0] = (go.GetComponent<CubeScript>());
        }
       
    }*/
    public Vector3 getPosition()
    {
        return position;
    }
}
