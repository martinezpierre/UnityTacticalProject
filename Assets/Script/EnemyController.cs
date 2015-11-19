using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    bool canMove = true;
    bool canAttack = true;

    Vector2 actualPosition;

    public int maxMove = 3;
    public int range = 1;

    public float speed = 1f;

    bool moovng = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TakeDamage()
    {
        Destroy(gameObject);
    }
}
