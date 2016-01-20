using UnityEngine;
using System.Collections;

public class EnemyController : EntityController {
    
    // Use this for initialization
    void Start () {
        canMove = true;
        canAttack = true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TakeDamage()
    {
        Destroy(gameObject);
    }
}
