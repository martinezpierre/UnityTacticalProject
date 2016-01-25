using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour {

    public static SpellManager instance = null;
    public static SpellManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    public enum SPELL
    {
        HEAL,
        TWOATTACKS,
        TELEPORTATION
    }

    public Dropdown dropdown;
    GameObject dropdownObject;

    GameObject currentPlayer;

    public SPELL[] spellInDropdown;
    SPELL currentSpell;

    public int nbSpellMax = 10;

    public List<CubeScript> cubes;

    CubeScript target;

    public Color attackZoneColor = Color.red;
    public Color nonAttackZoneColor = Color.green;
    public Color tileSelectedColor = Color.yellow;

    // Use this for initialization
    void Start () {
        dropdownObject = dropdown.gameObject;
        dropdownObject.SetActive(false);

        spellInDropdown = new SPELL[nbSpellMax];
    }
	
	// Update is called once per frame
	void Update () {
        /*if (Input.GetMouseButtonUp(0))
        {
            dropdownObject.SetActive(false);
            TurnManager.Instance.SkipAction();
        }*/
    }

    public void CreateDropDown(EntityController eC)
    {
        currentPlayer = eC.gameObject;

        dropdownObject.SetActive(true);

        dropdown.options.Clear();
        
        int index = 0;

        foreach (SPELL spell in eC.spells)
        {
            spellInDropdown[index] = spell;

            dropdown.options.Add(new Dropdown.OptionData() { text = ""+spell });

            index++;
        }
        dropdown.captionText = dropdown.captionText;
        SpellSelected();
    }

    public void SpellSelected()
    {
        currentSpell = spellInDropdown[dropdown.value];
        CastSpell(currentSpell);
    }

    public void CastSpell(SPELL spellName)
    {
        switch (spellName)
        {
            case SPELL.HEAL:
                Heal();
                break;
            case SPELL.TWOATTACKS:
                TwoAttacks();
                break;
            case SPELL.TELEPORTATION:
                Teleportation();
                break;
        }
    }
    
    void CreateSpellZone(int range, bool attack)
    {
        Color color = attack ? attackZoneColor : nonAttackZoneColor;
        Vector2 actualPosition = currentPlayer.GetComponent<EntityController>().actualPosition;
        
        ClearZone();

        for (int i = (int)actualPosition.x - range; i <= (int)actualPosition.x + range; i++)
        {
            for (int j = (int)actualPosition.y - range; j <= (int)actualPosition.y + range; j++)
            {
                if (Mathf.Abs(j - (int)actualPosition.y) + Mathf.Abs(i - (int)actualPosition.x) <= range)
                {
                    GameObject go = ArenaManager.Instance.getTile(i, j); if (go)
                    {
                        Debug.Log("create zone condition 2");
                        go.gameObject.GetComponent<Renderer>().material.color = color;
                        currentPlayer.GetComponent<EntityController>().tiles.Add(go);
                        go.GetComponent<CubeScript>().SetInteractable(true);
                        cubes.Add(go.GetComponent<CubeScript>());
                    }
                }
            }
        }
    }
    
    public void CaseSelected(CubeScript cS)
    {
        ClearZone();

        target = cS;

        CastSpell(currentSpell);
    }

    public void ClearZone()
    {
        PlayerController pC = currentPlayer.GetComponent<PlayerController>();

        pC.repaint(pC.tiles);

        foreach (CubeScript c in cubes)
        {
            c.SetInteractable(false);
        }
        cubes.Clear();
    }

    void Heal()
    {
        Debug.Log("heal");

        if (target)
        {
            //effet du sort sur la case target
            target.occupant.TakeDamage(-TurnManager.Instance.currentPlayer.damage);

            TurnManager.Instance.SkipAction();
        }
        else
        {
            //creation de la zone du sort
            CreateSpellZone(0, false);
        }
    }

    void TwoAttacks()
    {
        Debug.Log("two attacks");
        if (target && target.occupant)
        {
            target.occupant.TakeDamage(TurnManager.Instance.currentPlayer.damage);
            target.occupant.TakeDamage(TurnManager.Instance.currentPlayer.damage);

            TurnManager.Instance.SkipAction();
        }
        else
        {
            CreateSpellZone(1, true);
        }
    }

    void Teleportation()
    {
        Debug.Log("teleportation");
        if (target)
        {
            currentPlayer.transform.position = new Vector3(target.transform.position.x, currentPlayer.transform.position.y, target.transform.position.z);
            currentPlayer.GetComponent<EntityController>().UpdatePosition();

            TurnManager.Instance.SkipAction();
        }
        else
        {
            CreateSpellZone(10, false);
        }
    }

    public void ResetSpell()
    {

        target = null;

        dropdown.value = 0;
        
        ClearZone();

        dropdownObject.SetActive(false);
    }
}
