using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

    public int maxLife = 100;
    int life;

    public GameObject EntityCanvas;
    Slider lifeSlider;

    public int damage = 10;

    public List<SpellManager.SPELL> spells;

    CubeScript previousTile;

    // Use this for initialization
    protected virtual void Start () {

        life = 100;

        GameObject go = Instantiate(EntityCanvas);
        go.transform.parent = gameObject.transform;
        go.GetComponent<RectTransform>().localPosition = Vector3.zero;

        lifeSlider = go.transform.Find("Life").GetComponent<Slider>();

        lifeSlider.value = life / maxLife;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void TileToMoveSelected(){}

    public virtual void BeginTurn()
    {
        Debug.Log("begin entity");
    }

    public virtual void TakeDamage(int n){

        life -= n;
        
        Mathf.Clamp(life, 0, maxLife);

        lifeSlider.value = life*1.0f / maxLife*1.0f;
        
        if (life <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

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

    public void UpdatePosition()
    {
        Vector3 behind = -transform.TransformDirection(Vector3.up);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, behind, out hit))
        {
            actualPosition = new Vector2(hit.transform.position.x, hit.transform.position.z);

            hit.transform.GetComponent<CubeScript>().occupant = this;

            if (previousTile)
            {
                previousTile.occupant = null;
            }

            previousTile = hit.transform.GetComponent<CubeScript>();
        }
    }

    public virtual void SkipAction() { }
    
    public void AddSpell(SpellManager.SPELL spell)
    {
        spells.Add(spell);
    }

    public void RemoveSpell(SpellManager.SPELL spell)
    {
        spells.Remove(spell);
    }
}
