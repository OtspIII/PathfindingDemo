using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileContentsController : MonoBehaviour
{
    //A generic script for anything that can live inside of a tile
    //GridPlayerController and GridEnemyController both inherit from this
    //That means they inherit all of my variables and functions
    
    //My art
    public SpriteRenderer SR;
    //The tile I'm currently inside of
    public TileController Tile;
    
    public float Speed = 1;
    public List<TileController> Path;

    //This function moves me in a direction
    //I use this to move instead of ever touching transform.position or rb.velocity
    public void Move(int x, int y)
    {
        //If I'm not already in a tile, I can't move to an adjacent tile because there are none
        if (Tile == null)
        {
            Debug.Log("ERROR: " + gameObject + " TRIED TO MOVE DESPITE NOT EXISTING ANYWHERE");
            return;
        }
        //Find the tile exists x squares to my left and y squares above me
        TileController dest = Tile.Neighbor(x, y);
        //If there is no tile there (it's off the side of the map?) abort my attempt to move
        if (dest == null)
        {
            return;
        }

        //If the tile I want to move into already has something inside of it
        if (dest.Contents != null)
        {
            //Call the HandleBump function to handle exactly what happens,
              //and tell it both where I was going and who's there
            HandleBump(dest,dest.Contents);
        }
        else
        {
            //If the tile's empty, just move into it
            SetTile(dest);
        }
    }

    //This gets called when I bump into another object in a tile I'm trying to move into
    //It's virtual, so that means the scripts that inherit from me will override it with new code
    //That's why it's okay that it's totally empty for me
    public virtual void HandleBump(TileController where, TileContentsController who)
    {
        
    }

    //All the things that need to happen when I move into a tile
    public void SetTile(TileController where)
    {
        //If I try to move into a tile that already has something in it, abort the process
        if (where.Contents != null)
        {
            Debug.Log("ERROR: " + gameObject + " TRIED TO GO INTO A NON-EMPTY TILE");
            return;
        }
        //If I was already in a tile, tell it that I'm leaving
        if(Tile != null) LeaveTile();
        //Set my tile to my new location
        Tile = where;
        //Tell my new tile that I'm in it
        Tile.Contents = this;
        //Teleport me in front of my tile
        transform.position = Tile.transform.position;

    }

    public void LeaveTile()
    {
        //If I'm not in a tile, I don't need to do this
        if (Tile == null) return;
        //Safety check--if the tile lists me as what's in it, then mark it as empty
        if(Tile.Contents == this)
            Tile.Contents = null;
    }

    //Code for being destroyed
    public void Die()
    {
        //Mark my tile as empty
        LeaveTile();
        //And destroy my game object
        Destroy(gameObject);
    }
    void Start() { StartCoroutine(Run()); }
    
    public IEnumerator Run()
    {
        //Ideally this should only run once, but just in case. . .
        while (Tile != GridManager.Me.Goal)
        {
            //What path do we take to get to the exit?
            Path = FindPath(GridManager.Me.Goal);
            //Move to each tile on the path, one by one
            foreach (TileController t in Path)
            {
                //If I made an invalid move, don't do it
                if (t == null || t.Type == TileTypes.Wall ||
                    Mathf.Abs(Tile.X - t.X) + Mathf.Abs(Tile.Y - t.Y) > 1)
                {
                    Debug.Log("INVALID MOVE");
                    if(t != null) t.SetType(TileTypes.Error);
                    GridManager.Me.YouWin.gameObject.SetActive(true);
                    GridManager.Me.YouWin.text = "INVALID MOVE";
                    while (!Input.GetKeyDown(KeyCode.Space))
                        yield return null;
                    SceneManager.LoadScene("GridGame");
                    break;
                }
                //Move to the next tile and wait a sec before next steps
                SetTile(t);
                if(Input.GetKey(KeyCode.Space))
                    yield return new WaitForSeconds(Speed / 5);
                else if(Input.GetKey(KeyCode.LeftShift))
                    yield return new WaitForSeconds(Speed / 10);
                else
                    yield return new WaitForSeconds(Speed);
            }
            //Wait a frame, just in case we're in an infinite loop
            yield return null;
        }
        //If we escaped that while loop, we must be at the goal!
        GridManager.Me.YouWin.gameObject.SetActive(true);
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        GridManager.CurrentLevel = (MazeTypes)((int)GridManager.CurrentLevel + 1);
        SceneManager.LoadScene("GridGame");
    }

    public virtual List<TileController> FindPath(TileController goal)
    {
        return new List<TileController>();
    }
    
}
