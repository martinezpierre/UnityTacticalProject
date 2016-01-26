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
        ATTACK,
        HEAL,
        TWOATTACKS,
        TELEPORTATION,
        REDUCTIONDAMAGE,
        ATTACKLONGRANGE,
        COUNTERATTACK,
        STUN,
        INVOCATION
    }

    public List<SPELL> spellsP1;
    public List<SPELL> spellsP2;
    public List<SPELL> spellsP3;
    public List<SPELL> spellsP4;

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

    public int teleportationRange = 6;

    public float damageReduction = 0.5f;
    public int nbTurnDamageReducMax = 3;

    public int nbCounterattack = 1;
    public int nbTurnCounterMax = 3;

    public GameObject InvocationPrefab;
    
    // Use this for initialization
    void Start () {
        dropdownObject = dropdown.gameObject;
        dropdownObject.SetActive(false);

        spellInDropdown = new SPELL[nbSpellMax];
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
            case SPELL.ATTACK:
                Attack();
                break;
            case SPELL.HEAL:
                Heal();
                break;
            case SPELL.TWOATTACKS:
                TwoAttacks();
                break;
            case SPELL.TELEPORTATION:
                Teleportation();
                break;
            case SPELL.REDUCTIONDAMAGE:
                ReductionDamage();
                break;
            case SPELL.ATTACKLONGRANGE:
                AtackLongRange();
                break;
            case SPELL.COUNTERATTACK:
                CounterAttack();
                break;
            case SPELL.STUN:
                Stun();
                break;
            case SPELL.INVOCATION:
                Invocation();
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

    void Attack()
    {
        if (target && target.occupant)
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.GetAttackSound(), target.transform.position);
            SendDamage(TurnManager.Instance.currentPlayer, target.occupant, 1);
        }
        else
        {
            CreateSpellZone(1, true);
        }
    }

    void Heal()
    {
        if (target && target.occupant)
        {
            //effet du sort sur la case target
            AudioSource.PlayClipAtPoint(SoundManager.Instance.healSound, target.transform.position);
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
        if (target && target.occupant)
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.GetAttackSound(), target.transform.position);
            SendDamage(TurnManager.Instance.currentPlayer, target.occupant, 2);
        }
        else
        {
            CreateSpellZone(1, true);
        }
    }

    void Teleportation()
    {
        if (target && !target.occupant)
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.teleportationSound, target.transform.position);
            currentPlayer.transform.position = new Vector3(target.transform.position.x, currentPlayer.transform.position.y, target.transform.position.z);
            currentPlayer.GetComponent<EntityController>().UpdatePosition();

            TurnManager.Instance.SkipAction();
        }
        else
        {
            CreateSpellZone(teleportationRange, false);
        }
    }

    void ReductionDamage()
    {
        if (target)
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.reducDamageSound, target.transform.position);
            currentPlayer.GetComponent<EntityController>().SetDamageReduction(damageReduction, nbTurnDamageReducMax);

            TurnManager.Instance.SkipAction();
        }
        else
        {
            CreateSpellZone(0, false);
        }
    }

    void AtackLongRange()
    {
        if (target && target.occupant)
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.GetLaserSound(), target.transform.position);
            SendDamage(TurnManager.Instance.currentPlayer, target.occupant, 1);

        }
        else
        {
            CreateSpellZone(2, true);
        }
    }

    void CounterAttack()
    {
        if (target)
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.buffSound, target.transform.position);
            currentPlayer.GetComponent<EntityController>().SetCounter(nbCounterattack, nbTurnCounterMax);

            TurnManager.Instance.SkipAction();
        }
        else
        {
            CreateSpellZone(0, false);
        }
    }

    void Stun()
    {
        if (target && target.occupant)
        {
            if (Random.Range(0f, 1f) > 0.8f)
            {
                AudioSource.PlayClipAtPoint(SoundManager.Instance.stunSound, target.transform.position);
                target.occupant.stunned = true;
            }

            TurnManager.Instance.SkipAction();
        }
        else
        {
            CreateSpellZone(1, true);
        }
    }

    void Invocation()
    {
        if (target && !target.occupant)
        {
            StartCoroutine(InvocAnim(new Vector3(target.transform.position.x, InvocationPrefab.transform.localScale.y / 2, target.transform.position.z)));
        }
        else
        {
            CreateSpellZone(1, false);
        }
    }

    public void ResetSpell()
    {

        target = null;

        dropdown.value = 0;
        
        ClearZone();

        dropdownObject.SetActive(false);
    }

    public void SendDamage(EntityController attacker, EntityController defenser, int nbAttack)
    {
        if (!attacker || !defenser) return;

        StartCoroutine(AttackAnim(attacker, defenser, nbAttack));
    }

    IEnumerator AttackAnim(EntityController attacker, EntityController defenser, int nbAttack)
    {
        attacker.transform.LookAt(defenser.transform);
        attacker.anim.Play("Attack");

        for (int i = 0; i < nbAttack; i++)
        {
            defenser.TakeDamage(attacker.damage);
        }

        if (defenser.counterattackCount > 0)
        {
            defenser.transform.LookAt(attacker.transform);
            defenser.anim.Play("Attack");
        }
        for (int j = 0; j < defenser.counterattackCount; j++)
        {
            attacker.TakeDamage(defenser.damage);
        }

        yield return new WaitForSeconds(2f);

        TurnManager.Instance.SkipAction();
    }

    IEnumerator InvocAnim(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(SoundManager.Instance.GetInvocSound(), target.transform.position);

        GameObject go = Instantiate(InvocationPrefab, pos, Quaternion.identity) as GameObject;
        EntityController eC = go.GetComponent<EntityController>();
        eC.AddSpell(SPELL.ATTACK);
        eC.lifeBar.color = TurnManager.Instance.currentPlayer.lifeBar.color;
        
        TurnManager.Instance.Add(eC);

        yield return new WaitForSeconds(1.5f);

        TurnManager.Instance.SkipAction();
    }
}
