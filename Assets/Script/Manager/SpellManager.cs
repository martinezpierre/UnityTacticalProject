using UnityEngine;
using System.Collections;
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

    public SPELL[] spellinDropdown;

    public int nbSpellMax = 10;

    // Use this for initialization
    void Start () {
        dropdownObject = dropdown.gameObject;
        dropdownObject.SetActive(false);

        spellinDropdown = new SPELL[nbSpellMax];
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
            spellinDropdown[index] = spell;

            dropdown.options.Add(new Dropdown.OptionData() { text = ""+spell });

            index++;
        }
        dropdown.captionText = dropdown.captionText;
        SpellSelected();
    }

    public void SpellSelected()
    {
        CastSpell(spellinDropdown[dropdown.value]);
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
        Color color = attack ? Color.red : Color.green;
        Vector2 actualPosition = currentPlayer.GetComponent<EntityController>().actualPosition;

        PlayerController pC = currentPlayer.GetComponent<PlayerController>();

        pC.repaint(pC.tiles);

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
                    }
                }
            }
        }
    }

    void Heal()
    {
        Debug.Log("heal");
        CreateSpellZone(2, false);
    }

    void TwoAttacks()
    {
        Debug.Log("two attacks");
        CreateSpellZone(1, true);
    }

    void Teleportation()
    {
        Debug.Log("teleportation");
        CreateSpellZone(10, false);
    }
}
