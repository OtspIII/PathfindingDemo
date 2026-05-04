using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //A static variable to make me easy to find
    public static GridManager Me;
    
    //How big is the map we'll build?
    private Vector2 MapSize = new Vector2(10,7);
    
    //Prefabs for spawning
    public TileContentsController PlayerPrefab;
    public TileController TilePrefab;
    public TextMeshPro YouWin;

    //A set of coordinates for the map
    //Don't worry about the details of this too hard, it's very ugly
    public Dictionary<int, Dictionary<int, TileController>>
        Map = new Dictionary<int, Dictionary<int, TileController>>();

    //Track everything we spawn
    public List<TileController> AllTiles;
    public TileContentsController Player;
    public TileController Goal;
    public static MazeTypes CurrentLevel = MazeTypes.None;
    public MazeTypes Maze = MazeTypes.Empty;

    void Awake()
    {
        //Register myself to the static variable
        GridManager.Me = this;
        if (CurrentLevel == MazeTypes.None) CurrentLevel = Maze;
    }
    
    void Start()
    {
        //Spawn every row of tiles
        for (int x = 0; x < MapSize.x; x++)
        {
            //Spawn each tile in the row
            for (int y = 0; y < MapSize.y; y++)
            {
                //Calculate where the tile should spawn
                //The one in the middle should be at 0,0
                Vector3 where = new Vector3(x - (MapSize.x / 2), y - (MapSize.y / 2), 10);
                //Spawn it
                TileController t = Instantiate(TilePrefab, where, Quaternion.identity);
                //Tell the tile that it exists and what its coordinate is
                t.Setup(x,y);
                //Add it to the list of tiles that exist in our tracker
                AllTiles.Add(t);
                //Add it to the map so we can find it later
                //Again, don't worry about this dictionary stuff too hard yet
                //This is an ugly use of it
                if(!Map.ContainsKey(x))
                    Map.Add(x,new Dictionary<int, TileController>());
                if(!Map[x].ContainsKey(y))
                    Map[x].Add(y,t);
            }
        }

        //Make a list of all the tiles that exist
        //We'll remove tiles from this list as we fill them with things
        //That means this list will be a list of all the non-filled tiles
        List<TileController> emptyTiles = new List<TileController>();
        emptyTiles.AddRange(AllTiles);

        //Choose a random tile
        TileController chosen = GetTile(1, 3);
        //Spawn the player and then put them in the chosen tile
        Player = Instantiate(PlayerPrefab);
        Player.SetTile(chosen);
        //Remove the filled tile from the list of tiles--it's no longer empty
        emptyTiles.Remove(chosen);
        Goal = GetTile(8, 3);
        if (CurrentLevel == MazeTypes.Offset)
        {
            Goal = GetTile(8, 5);
        }
        Goal.SetType(TileTypes.Exit);
        emptyTiles.Remove(Goal);
        List<TileController> walls = new List<TileController>();
        switch (CurrentLevel)
        {
            case MazeTypes.Dot:
            {
                walls.Add(GetTile(6,3));
                break;
            }
            case MazeTypes.Wall:
            {
                for(int y=1;y<6;y++)
                    walls.Add(GetTile(6,y));
                break;
            }
            case MazeTypes.Cup:
            {
                for(int y=1;y<6;y++)
                    walls.Add(GetTile(6,y));
                walls.Add(GetTile(5,1));
                walls.Add(GetTile(5,5));
                walls.Add(GetTile(4,1));
                walls.Add(GetTile(4,5));
                break;
            }
        }
        foreach(TileController t in walls)
            t.SetType(TileTypes.Wall);
    }

    //A shortcut to make it easy to get a specific tile
    //Feed it an X/Y coordinate and it'll tell you the tile there
    public TileController GetTile(int x, int y)
    {
        //If you ask for a tile that doesn't exist, return null
        if (!Map.ContainsKey(x) || !Map[x].ContainsKey(y))
            return null;
        //Return the asked for tile
        return Map[x][y];
        //The Map variable is annoying to interact with, so I just wrote a function that will have nicer grammar
    }
}

public enum MazeTypes
{
    None=0,
    Empty=1,
    Offset=2,
    Dot=3,
    Wall=4,
    Cup=5,
    Forest=6,
    Random=7,
}