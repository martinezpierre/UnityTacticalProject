using UnityEngine;
using System.Collections;

public class EnemyController : EntityController {

    // Use this for initialization
    protected override void Start () {

        base.Start();

        canMove = true;
        canAttack = true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void BeginTurn()
    {
        Debug.Log("begin enemy");
    }

    public override void TakeDamage(int n)
    {
        base.TakeDamage(n);
        //Destroy(gameObject);
    }
}
