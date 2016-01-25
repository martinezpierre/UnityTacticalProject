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
        instance = this;

        playerIdList = new List<int>();
    }

    public List<int> playerIdList;
    public EntityController currentPlayer;
    public int currentPlayerIndex;

    GameObject cam;

    public bool gameFinished = false;
    
    public List<EntityController> entities;

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
    
    public void Remove(EntityController eC)
    {
        entities.Remove(eC);
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
        if (!gameFinished)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % entities.Count;

            currentPlayer = entities[currentPlayerIndex];
            SetCamera();

            if (currentPlayerIndex == 0)
            {
                EndOfGlobalTurn();
            }
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
