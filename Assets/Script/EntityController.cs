using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityController : MonoBehaviour {


    public List<GameObject> tiles;

    public bool canMove;
    public bool canAttack;
    
    public Vector2 actualPosition;

    public int maxMove = 3;
    public int range = 1;

    public float speed = 1f;

    public bool moovng = false;

    public int id;

    public List<SpellManager.SPELL> spells;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public virtual void BeginTurn()
    {
        Debug.Log("begin entity");
    }

    public virtual void TakeDamage(){}

    void OnDestroy()
    {
        TurnManager.Instance.Remove(this);
    }

    public void EndTurn()
    {
        Debug.Log("end turn");

        canMove = true;
        canAttack = true;

        TurnManager.Instance.Endturn();
    }

    public virtual void SkipAction() { }

    public virtual void ChooseSpell(){}

    public void AddSpell(SpellManager.SPELL spell)
    {
        spells.Add(spell);
    }

    public void RemoveSpell(SpellManager.SPELL spell)
    {
        spells.Remove(spell);
    }
}
