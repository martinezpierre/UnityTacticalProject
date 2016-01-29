using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Networking;

public class TurnManager : MonoBehaviour {

    public static TurnManager instance = null;
    public static TurnManager Instance
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

        playerIdList = new List<int>();
    }

    public List<int> playerIdList;
    public EntityController currentPlayer;
    public int currentPlayerIndex;

    GameObject cam;

    public bool gameFinished = false;
    
    public List<EntityController> entities;

    public List<Color> playersColor;

    int nbColorAffected = 0;

    // Use this for initialization
    void Start () {
        cam = Camera.main.gameObject;

        entities = new List<EntityController>();

        EntityController[] entitabs = FindObjectsOfType<EntityController>();

        int id = 0;

        foreach(EntityController eC in entitabs)
        {
            eC.id = id;
            entities.Add(eC);
            id++;
        }
        
        currentPlayerIndex = 0;
        currentPlayer = entities[currentPlayerIndex];
        SetCamera();
    }
    
    public Color GetColor()
    {
        nbColorAffected++;

        return playersColor[nbColorAffected-1];
    }

    public void Add(EntityController eC)
    {
        eC.id = entities.Count;
        entities.Add(eC);
    }

    public void Remove(EntityController eC)
    {
        entities.Remove(eC);
        if(currentPlayer == eC)
        {
            SkipAction();
            SkipAction();
        }
    }

    public void SkipAction()
    {
        currentPlayer.SkipAction();
    }

    public void Endturn()
    {
        StartCoroutine(EndTurnActions());
    }

    void SetCamera()
    {
        cam.transform.parent = currentPlayer.transform;
        cam.transform.localPosition = new Vector3(0f, cam.transform.localPosition.y, 0f);
        currentPlayer.BeginTurn();
    }

    IEnumerator EndTurnActions()
    {
        yield return new WaitForEndOfFrame();

        bool canPlay = true;

        if (!gameFinished)
        {
            SpellManager.Instance.ResetSpell();

            do
            {
                canPlay = true;

                currentPlayerIndex = (currentPlayerIndex + 1) % entities.Count;

                currentPlayer = entities[currentPlayerIndex];

                if (currentPlayer.id != currentPlayerIndex)
                {
                    currentPlayer.id = currentPlayerIndex;
                }

                SetCamera();

                if (currentPlayerIndex == 0)
                {
                    EndOfGlobalTurn();
                }

                if (currentPlayer.stunned)
                {
                    canPlay = false;
                    currentPlayer.stunned = false;
                }
                
            } while (!canPlay);
            

        }
    }

    public void EndOfGlobalTurn()
    {
    }

    public bool canPlay(int idPlayer)
    {
        return idPlayer == currentPlayerIndex;
    }
}
