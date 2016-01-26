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
    protected int life;

    public GameObject EntityCanvas;
    Slider lifeSlider;

    public int damage = 10;

    float damageReduction = 1f;
    int nbTurn = 1;
    int nbTurnMax = 3;

    public int counterattackCount = 0;
    int nbTurnCounter = 1;
    int nbTurnMaxCounter = 3;

    public List<SpellManager.SPELL> spells;

    [HideInInspector]
    public bool dead = false;

    [HideInInspector]
    public bool stunned = false;

    CubeScript previousTile;
    
    [HideInInspector]public  Animation anim;



    // Use this for initialization
    protected virtual void Start () {

        life = 100;

        GameObject go = Instantiate(EntityCanvas);
        go.transform.parent = gameObject.transform;
        go.GetComponent<RectTransform>().localPosition = Vector3.zero;

        lifeSlider = go.transform.Find("Life").GetComponent<Slider>();

        lifeSlider.value = life / maxLife;
        
        anim = GetComponentInChildren<Animation>();
    }

    // Update is called once per frame
    protected virtual void Update () {
        if (!anim.isPlaying && !dead)
        {
            anim.Play("Wait");
        }
	}

    public virtual void TileToMoveSelected(){}

    public virtual void BeginTurn()
    {
        if(damageReduction != 1f)
        {
            nbTurn++;
            if (nbTurn >= nbTurnMax)
            {
                nbTurn = 1;
                damageReduction = 1f;
            }
        }

        if(counterattackCount != 0)
        {
            nbTurnCounter++;
            if(nbTurnCounter >= nbTurnMaxCounter)
            {
                nbTurnCounter = 1;
                counterattackCount = 0;
            }
        }
        
    }

    public virtual void TakeDamage(int n){
        
        anim.Play("Damage");

        life -= (int)(n * damageReduction);
        
        Mathf.Clamp(life, 0, maxLife);

        lifeSlider.value = life*1.0f / maxLife*1.0f;
        
        if (life <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        dead = true;

        anim.Play("Dead");

        yield return new WaitForSeconds(2f);

        if (previousTile)
        {
            previousTile.occupant = null;
        }
        
        TurnManager.Instance.Remove(this);
        //Destroy(gameObject);
    }
    
    public void EndTurn()
    {

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

    public void SetDamageReduction(float f, int n)
    {
        damageReduction = f;
        nbTurnMax = n;
    }

    public void SetCounter(int nbCounter, int nbTurn)
    {
        counterattackCount = nbCounter;
        nbTurnMaxCounter = nbTurn;
    }
}
