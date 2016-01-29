using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance = null;
    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        instance = this;
    }

    public List<GameObject> spellsLists;

    Dropdown[][] spellsDropdowns;

    public SpellManager.SPELL[] spellInDropdown;

    public List<SpellManager.SPELL>[] SelectedSpells;

    public int nbPlayer = 4;
    
    // Use this for initialization
    void Start () {

        int nbSpell = System.Enum.GetValues(typeof(SpellManager.SPELL)).Length;

        spellsDropdowns = new Dropdown[nbPlayer][];

        spellInDropdown = new SpellManager.SPELL[nbSpell];

        SelectedSpells = new List<SpellManager.SPELL>[nbPlayer];

        for (int i = 0; i< nbPlayer; i++)
        {
            spellsDropdowns[i] = spellsLists[i].GetComponentsInChildren<Dropdown>();
        }

        foreach (Dropdown dd in FindObjectsOfType<Dropdown>())
        {
            int index = 0;
            dd.options.Clear();
            foreach (SpellManager.SPELL spell in System.Enum.GetValues(typeof(SpellManager.SPELL)))
            {
                dd.options.Add(new Dropdown.OptionData() { text = "" + spell });
                spellInDropdown[index] = spell;
                index++;
            }
            dd.captionText = dd.captionText;
        }
    }
	
    public void Confirmed()
    {
        for(int i = 0; i < nbPlayer; i++)
        {
            SelectedSpells[i] = new List<SpellManager.SPELL>();
            foreach (Dropdown dropdown in spellsDropdowns[i])
            {
                SelectedSpells[i].Add(spellInDropdown[dropdown.value]);
            }
        }
        Application.LoadLevel(1);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
