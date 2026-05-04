using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridAIController : TileContentsController
{
    

    public override List<TileController> FindPath(TileController goal)
    {
        //This is an incomplete demo
        //First we figure out where we are
        TileController start = Tile;
        //We make a list that will be the path we're taking
        List<TileController> r = new List<TileController>();
        //I'm just going to walk right. What tile is to my right?
        TileController right = start.Neighbor(1, 0);
        //Add it to my path
        r.Add(right);
        //Okay, let's just walk right once and see if we win
        return r;
    }
}
