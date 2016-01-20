using UnityEngine;
using System.Collections;

public class EntityController : MonoBehaviour {
    
    public bool canMove;
    public bool canAttack;
    
    public Vector2 actualPosition;

    public int maxMove = 3;
    public int range = 1;

    public float speed = 1f;

    public bool moovng = false;

    public int id;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public void EndTurn()
    {
        Debug.Log("end turn");

        canMove = true;
        canAttack = true;
        TurnManager.Instance.Endturn();
    }
}
