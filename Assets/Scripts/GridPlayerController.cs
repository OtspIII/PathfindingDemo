using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlayerController : TileContentsController
{
    //The script for the player
    //Note that it inherits from TileContentsController, so it has all its variables/functions
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //If I hit right, go right
            Move(1,0);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(-1,0);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(0,1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(0,-1);
        }
    }

}
