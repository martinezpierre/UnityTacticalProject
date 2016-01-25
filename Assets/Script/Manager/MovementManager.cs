using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MovementManager : MonoBehaviour {

    static MovementManager instance;
    public static MovementManager Instance
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
    protected class DuplicateKeyComparer<TKey> :
    IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;
            else
                return result;
        }

        #endregion
    }

    static private DuplicateKeyComparer<float> Comparer = new DuplicateKeyComparer<float>();

    public List<CubeScript> findPath(Vector3 start, Vector3 end)
    {
        CubeScript endTile = foundTile(end);
        CubeScript startTile = foundTile(start);
        return findPathFromTile(startTile, endTile);
    }
    public CubeScript foundTile(Vector3 tile)
    {
        float distance = 0;
        float smallDistance = 99999;
        CubeScript nearTile = null;
        foreach (CubeScript currentTile in ArenaManager.Instance.listCubeScript)        {
            distance = Vector3.Distance(tile, currentTile.getPosition());
            if (distance < smallDistance)
            {
                smallDistance = distance;
                nearTile = currentTile;
            }
        }
        return nearTile;
    }
    public List<CubeScript> findPathFromTile(CubeScript start, CubeScript end)
    {
        List<CubeScript> road = foundRoad(start, end);
        /*List<CubeScript> r = new List<CubeScript>(road.Count);
        foreach (CubeScript node in road)
        {
            r.Add(node.getPosition());
        }*/
        
        return road;
    }


    public List<CubeScript> foundRoad(CubeScript tileStart, CubeScript tileEnd)
    {
        List<CubeScript> checkedTile = new List<CubeScript>();
        SortedList<float, CubeScript> tryTile = new SortedList<float, CubeScript>(Comparer);

        float cumule = 0;

        float currentDist;
        float currentEury;

        CubeScript currentTile = null;
        CubeScript previousNode = tileStart;
        List<CubeScript> listTile;
        tryTile.Add(Vector3.Distance(tileStart.getPosition(), tileEnd.getPosition()), tileStart);
        while ((currentTile != tileEnd) && (tryTile.Count > 0))
        {

            currentTile = tryTile.ElementAt(0).Value;
            tryTile.RemoveAt(0);
            listTile = currentTile.neighbor;
            foreach (CubeScript node in listTile)
            {
                if (!checkedTile.Contains(node))
                {
                    currentDist = Vector3.Distance(previousNode.getPosition(), node.getPosition()) + previousNode.distance;
                    currentEury = Vector3.Distance(node.getPosition(), tileEnd.getPosition());

                    cumule = currentDist + currentEury;

                    if (cumule < (node.heurystic + node.distance))
                    {
                        node.heurystic = currentDist;
                        node.distance = currentEury;
                        node.cumule = currentDist + currentEury;
                        node.previousTile = currentTile;

                        tryTile.Add(node.cumule, node);
                    }
                }

            }
            checkedTile.Add(currentTile);
        }

        if (currentTile == tileEnd)
        {
            return returnRoad(currentTile, checkedTile);
        }
        else
        {
            return null;
        }

    }
    public List<CubeScript> returnRoad(CubeScript currentTile,  List<CubeScript> checkedTile)
    {
        List<CubeScript> onTheRoad = new List<CubeScript>();
        while (currentTile.previousTile != null)
        {
            onTheRoad.Insert(0, currentTile);
            currentTile = currentTile.previousTile;
        }
        ResetTile(checkedTile);
        return onTheRoad;
    }


    void ResetTile(List<CubeScript> checkedTile)
    {
        foreach(CubeScript CS in checkedTile)
        {
            CS.distance = 9999;
            CS.heurystic = 9999;
            CS.previousTile = null;
            CS.cumule = 0;
        }
    }

}
