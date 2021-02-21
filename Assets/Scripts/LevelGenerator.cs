using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    enum gridSpace
    {
        empty,
        floor,
        wall,
    };

    enum wallType
    {
        empty,
        floor,
        wall,
        wall_ne,
        wall_nw,
        wall_se,
        wall_sw,
        wall_n,
        wall_e,
        wall_s,
        wall_w,
        wall_ns,
        wall_we,
        wall_wne,
        wall_swn,
        wall_nes,
        wall_esw
    };

    gridSpace[,] grid;
    private wallType[,] wallGrid;
    int roomHeight, roomWidth;
    Vector2 roomSizeWorldUnits = new Vector2(100, 100);
    float worldUnitsInOneGridCell = 2;

    struct walker
    {
        public Vector2 dir;
        public Vector2 pos;
    }

    List<walker> walkers;
    float chanceWalkerChangeDir = 0.5f, chanceWalkerSpawn = 0.05f;
    float chanceWalkerDestoy = 0.05f;
    int maxWalkers = 10;
    float percentToFill = 0.2f;
    public GameObject wallObj;
    public GameObject wallEObj;
    public GameObject wallWObj;
    public GameObject wallNObj;
    public GameObject wallSObj;
    public GameObject wallNWObj;
    public GameObject wallNEObj;
    public GameObject wallSWObj;
    public GameObject wallSEObj;

    public GameObject wallNSObj;
    public GameObject wallWEObj;
    public GameObject wallWNEObj;
    public GameObject wallSWNObj;
    public GameObject wallNESObj;
    public GameObject wallESWObj;
    
    public GameObject floorObj;

    public NavMeshSurface surface;
    
    void Start()
    {
        Setup();
        CreateFloors();
        CreateWalls();
        RemoveSingleWalls();
        ReplaceWalls();
        SpawnLevel();
        
        surface.BuildNavMesh();
    }

    void Setup()
    {
        //find grid size
        roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.x / worldUnitsInOneGridCell);
        roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.y / worldUnitsInOneGridCell);
        //create grid
        grid = new gridSpace[roomWidth, roomHeight];
        wallGrid = new wallType[roomWidth, roomHeight];
        //set grid's default state
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //make every cell "empty"
                grid[x, y] = gridSpace.empty;
                
                // make every wall "center"
                wallGrid[x, y] = wallType.empty;
            }
        }

        //set first walker
        //init list
        walkers = new List<walker>();
        //create a walker 
        walker newWalker = new walker();
        newWalker.dir = RandomDirection();
        //find center of grid
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(roomWidth / 2.0f),
            Mathf.RoundToInt(roomHeight / 2.0f));
        newWalker.pos = spawnPos;
        //add walker to list
        walkers.Add(newWalker);
    }

    void CreateFloors()
    {
        int iterations = 0; //loop will not run forever

        while (iterations < 100000)
        {
            //create floor at position of every walker
            foreach (walker myWalker in walkers)
            {
                grid[(int) myWalker.pos.x, (int) myWalker.pos.y] = gridSpace.floor;
            }

            //chance: destroy walker
            int numberChecks = walkers.Count; //might modify count while in this loop
            for (int i = 0; i < numberChecks; i++)
            {
                //only if its not the only one, and at a low chance
                if (Random.value < chanceWalkerDestoy && walkers.Count > 1)
                {
                    walkers.RemoveAt(i);
                    break; //only destroy one per iteration
                }
            }

            //chance: walker pick new direction
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value < chanceWalkerChangeDir)
                {
                    walker thisWalker = walkers[i];
                    thisWalker.dir = RandomDirection();
                    walkers[i] = thisWalker;
                }
            }

            //chance: spawn new walker
            numberChecks = walkers.Count; //might modify count while in this loop
            for (int i = 0; i < numberChecks; i++)
            {
                //only if # of walkers < max, and at a low chance
                if (Random.value < chanceWalkerSpawn && walkers.Count < maxWalkers)
                {
                    //create a walker 
                    walker newWalker = new walker();
                    newWalker.dir = RandomDirection();
                    newWalker.pos = walkers[i].pos;
                    walkers.Add(newWalker);
                }
            }

            //move walkers
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                thisWalker.pos += thisWalker.dir;
                walkers[i] = thisWalker;
            }

            //avoid boarder of grid
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                //clamp x,y to leave a 1 space boarder: leave room for walls
                thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, roomWidth - 2);
                thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, roomHeight - 2);
                walkers[i] = thisWalker;
            }

            //check to exit loop
            if ((float) NumberOfFloors() / (float) grid.Length > percentToFill)
            {
                break;
            }

            iterations++;
        }
    }

    void CreateWalls()
    {
        //loop though every grid space
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //if theres a floor, check the spaces around it
                if (grid[x, y] == gridSpace.floor)
                {
                    //if any surrounding spaces are empty, place a wall
                    if (grid[x, y + 1] == gridSpace.empty)
                    {
                        grid[x, y + 1] = gridSpace.wall;
                    }

                    if (grid[x, y - 1] == gridSpace.empty)
                    {
                        grid[x, y - 1] = gridSpace.wall;
                    }

                    if (grid[x + 1, y] == gridSpace.empty)
                    {
                        grid[x + 1, y] = gridSpace.wall;
                    }

                    if (grid[x - 1, y] == gridSpace.empty)
                    {
                        grid[x - 1, y] = gridSpace.wall;
                    }

                    // also add walls for the corners
                    if (grid[x + 1, y + 1] == gridSpace.empty)
                    {
                        grid[x + 1, y + 1] = gridSpace.wall;
                    }

                    if (grid[x - 1, y - 1] == gridSpace.empty)
                    {
                        grid[x - 1, y - 1] = gridSpace.wall;
                    }

                    if (grid[x + 1, y - 1] == gridSpace.empty)
                    {
                        grid[x + 1, y - 1] = gridSpace.wall;
                    }

                    if (grid[x - 1, y + 1] == gridSpace.empty)
                    {
                        grid[x - 1, y + 1] = gridSpace.wall;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Replaces walls with the directional walls
    /// </summary>
    void ReplaceWalls()
    {
        
        //loop though every grid space
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                //if theres a wall, check the spaces around it
                if (grid[x, y] == gridSpace.wall)
                {
                    
                    bool wallAbove;
                    if (y == roomHeight - 1)
                    {
                        wallAbove = false;
                    }
                    else
                    {
                        wallAbove = grid[x, y + 1] == gridSpace.wall;
                    }

                    bool wallBelow;
                    if (y == 0)
                    {
                        wallBelow = false;
                    }
                    else
                    {
                        wallBelow = grid[x, y - 1] == gridSpace.wall;
                    }

                    bool wallToLeft;
                    if (x == 0)
                    {
                        wallToLeft = false;
                    }
                    else
                    {
                        wallToLeft = grid[x - 1, y] == gridSpace.wall;
                    }

                    bool wallToRight;
                    if (x == roomWidth - 1)
                    {
                        wallToRight = false;
                    }
                    else
                    {
                        wallToRight = grid[x + 1, y] == gridSpace.wall;
                    }

                    if (wallAbove && wallToLeft && !wallBelow && !wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_se;
                        continue;
                    }
                    
                    if (wallAbove && !wallToLeft && !wallBelow && wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_sw;
                        continue;
                    }
                    
                    if (!wallAbove && wallToLeft && wallBelow && !wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_ne;
                        continue;
                    }
                    
                    if (!wallAbove && !wallToLeft && wallBelow && wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_nw;
                        continue;
                    }
                    
                    if (wallAbove && !wallToLeft && !wallBelow && !wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_s;
                        continue;
                    }
                    
                    if (!wallAbove && wallToLeft && !wallBelow && !wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_e;
                        continue;
                    }
                    
                    if (!wallAbove && !wallToLeft && wallBelow && !wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_n;
                        continue;
                    }
                    
                    if (!wallAbove && !wallToLeft && !wallBelow && wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_w;
                        continue;
                    }
                    
                    if (!wallAbove && wallToLeft && !wallBelow && wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_we;
                        continue;
                    }
                    
                    if (wallAbove && !wallToLeft && wallBelow && !wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_ns;
                        continue;
                    }
                    
                    if (wallAbove && wallToLeft && wallBelow && !wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_swn;
                        continue;
                    }
                    if (!wallAbove && wallToLeft && wallBelow && wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_esw;
                        continue;
                    }
                    if (wallAbove && !wallToLeft && wallBelow && wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_nes;
                        continue;
                    }
                    if (wallAbove && wallToLeft && !wallBelow && wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall_wne;
                        continue;
                    }
                    if (wallAbove && wallToLeft && wallBelow && wallToRight)
                    {
                        wallGrid[x, y] = wallType.wall;
                        continue;
                    }
                }
            }
        }
    }

    void RemoveSingleWalls()
    {
        //loop though every grid space
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //if theres a wall, check the spaces around it
                if (grid[x, y] == gridSpace.wall)
                {
                    //assume all space around wall are floors
                    bool allFloors = true;
                    //check each side to see if they are all floors
                    for (int checkX = -1; checkX <= 1; checkX++)
                    {
                        for (int checkY = -1; checkY <= 1; checkY++)
                        {
                            if (x + checkX < 0 || x + checkX > roomWidth - 1 ||
                                y + checkY < 0 || y + checkY > roomHeight - 1)
                            {
                                //skip checks that are out of range
                                continue;
                            }

                            if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0))
                            {
                                //skip corners and center
                                continue;
                            }

                            if (grid[x + checkX, y + checkY] != gridSpace.floor)
                            {
                                allFloors = false;
                            }
                        }
                    }

                    if (allFloors)
                    {
                        grid[x, y] = gridSpace.floor;
                    }
                }
            }
        }
    }

    void SpawnLevel()
    {
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (wallGrid[x, y])
                {
                    case wallType.empty:
                        break;
                    case wallType.floor:
                        Spawn(x, y, floorObj);
                        break;
                    case wallType.wall:
                        Spawn(x, y, wallObj);
                        break;
                    case wallType.wall_e:
                        Spawn(x, y, wallEObj);
                        break;
                    case wallType.wall_w:
                        Spawn(x, y, wallWObj);
                        break;
                    case wallType.wall_s:
                        Spawn(x, y, wallSObj);
                        break;
                    case wallType.wall_n:
                        Spawn(x, y, wallNObj);
                        break;
                    case wallType.wall_nw:
                        Spawn(x, y, wallNWObj);
                        break;
                    case wallType.wall_ne:
                        Spawn(x, y, wallNEObj);
                        break;
                    case wallType.wall_se:
                        Spawn(x, y, wallSEObj);
                        break;
                    case wallType.wall_sw:
                        Spawn(x, y, wallSWObj);
                        break;
                    case wallType.wall_ns:
                        Spawn(x, y, wallNSObj);
                        break;
                    case wallType.wall_we:
                        Spawn(x, y, wallWEObj);
                        break;
                    case wallType.wall_wne:
                        Spawn(x, y, wallWNEObj);
                        break;
                    case wallType.wall_esw:
                        Spawn(x, y, wallESWObj);
                        break;
                    case wallType.wall_nes:
                        Spawn(x, y, wallNESObj);
                        break;
                    case wallType.wall_swn:
                        Spawn(x, y, wallSWNObj);
                        break;
                }
            }
        }
    }

    Vector2 RandomDirection()
    {
        //pick random int between 0 and 3
        int choice = Mathf.FloorToInt(Random.value * 3.99f);
        //use that int to chose a direction
        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            default:
                return Vector2.right;
        }
    }

    int NumberOfFloors()
    {
        int count = 0;
        foreach (gridSpace space in grid)
        {
            if (space == gridSpace.floor)
            {
                count++;
            }
        }

        return count;
    }

    void Spawn(float x, float y, GameObject toSpawn)
    {
        //find the position to spawn
        Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(x, y) * worldUnitsInOneGridCell - offset;
        //spawn object
        Instantiate(toSpawn, spawnPos, Quaternion.identity);
    }
}