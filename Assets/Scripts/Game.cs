using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static int boardWidth = 4, boardHeight = 4;

    public static Transform[,] grid = new Transform[boardWidth, boardHeight];

    public Canvas gameOverCanvas;

    public Text scoreText;

    public int score = 0;
   
    void Start()
    {
        gameOverCanvas.gameObject.SetActive(false);
        GenerateNewTile(2);
    }
    
    
    void Update()
    {
        if (!CheckGameOver())
        {
            CheckInput();
        }
        else
        {
            gameOverCanvas.gameObject.SetActive(true);
        }
    }

    void CheckInput()
    {
        bool down = Input.GetKeyDown(KeyCode.DownArrow), up = Input.GetKeyDown(KeyCode.UpArrow), right = Input.GetKeyDown(KeyCode.RightArrow), left = Input.GetKeyDown(KeyCode.LeftArrow);
        
        if(up || down || right || left)
        {
            PrepareTilesForMerging();

            if (up)
            {
                //Debug.Log(GetRandomLocation());
                MoveAllTiles(Vector2.up);
            }
            if (right)
            {
                //Debug.Log(GetRandomLocation());
                MoveAllTiles(Vector2.right);
            }
            if (down)
            {
                //Debug.Log(GetRandomLocation());
                MoveAllTiles(Vector2.down);
            }
            if (left)
            {
                //Debug.Log(GetRandomLocation());
                MoveAllTiles(Vector2.left);
            }
        }
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString("000000000");
    }

    /// <summary>
    /// checking every single tile's moves  
    /// </summary>
    /// <returns>boolean</returns>
    bool CheckGameOver()
    {
        if(transform.childCount < boardWidth * boardHeight)
        {
            return false;
        }

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                Transform currentTile = grid[i, j];
                Transform tileBelow = null;
                Transform tileBeside = null;

                if(j != 0)
                {
                    tileBelow = grid[i, j - 1];
                }

                if(i != boardWidth - 1)
                {
                    tileBeside = grid[i + 1, j];
                }

                if(tileBeside != null)
                {
                    if(currentTile.GetComponent<Tile>().tileNumber == tileBeside.GetComponent<Tile>().tileNumber)
                    {
                        return false;
                    }
                }

                if(tileBelow != null)
                {
                    if(currentTile.GetComponent<Tile>().tileNumber == tileBelow.GetComponent<Tile>().tileNumber)
                    {
                        return false;
                    }
                }

                   
            }
        }
        return true;
    }

    void MoveAllTiles(Vector2 direction)
    {
        int movedTileNumber = 0;

        if(direction == Vector2.left)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        if (MoveTheTile(grid[x, y], direction))
                        {
                            movedTileNumber++;
                        }
                    }
                }
            }
        }
        if (direction == Vector2.right)
        {
            for (int x = boardWidth - 1; x >= 0; x--)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        if (MoveTheTile(grid[x, y], direction))
                        {
                            movedTileNumber++;
                        }
                    }
                }
            }
        }
        if(direction == Vector2.up)
        {
            for(int x = 0; x < boardWidth; x++)
            {
                for(int y = boardHeight - 1; y >= 0 ; y--)
                {
                    if (grid[x, y] != null)
                    {
                        if (MoveTheTile(grid[x, y], direction))
                        {
                            movedTileNumber++;
                        }
                    }
                }
            }
        }
        if (direction == Vector2.down)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        if (MoveTheTile(grid[x, y], direction))
                        {
                            movedTileNumber++;
                        }
                    }
                }
            }
        }

        if (movedTileNumber != 0)
        {
            GenerateNewTile(1);
        }
    }

    bool MoveTheTile(Transform tile , Vector2 direction)
    {
        Vector2 startPos = tile.localPosition;

        while(true)
        {
            tile.transform.localPosition += (Vector3)direction;

            Vector2 pos = tile.transform.localPosition;

            if (CheckIfInside(pos))
            {
                if (CheckIsAtValidPosition(pos))
                {
                    UpdateGrid();
                }
                else
                {
                    if (!CheckAndCombineTiles(tile))
                    {
                        tile.transform.localPosition += -(Vector3)direction;

                        if (tile.transform.localPosition == (Vector3)startPos)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                tile.transform.localPosition += -(Vector3)direction;

                if(tile.transform.localPosition == (Vector3)startPos)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

    void UpdateGrid()
    {
        for(int x = 0; x < boardWidth; ++x)
        {
            for(int y = 0; y < boardHeight; ++y)
            {
                if(grid[x,y] != null)
                {
                    if(grid[x,y].parent == transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach(Transform tile in transform)
        {
            Vector2 v = new Vector2(Mathf.Round(tile.position.x), Mathf.Round(tile.position.y));

            grid[(int)v.x, (int)v.y] = tile;
        }
    }

    bool CheckAndCombineTiles(Transform movingTile)
    {
        Vector2 pos = movingTile.transform.localPosition;

        Transform collidingTile = grid[(int)pos.x, (int)pos.y];

        int movingTileValue = movingTile.GetComponent<Tile>().tileNumber;
        int collidingTileValue = collidingTile.GetComponent<Tile>().tileNumber;

        if(movingTileValue == collidingTileValue && !movingTile.GetComponent<Tile>().isMergedThisTurn && !collidingTile.GetComponent<Tile>().isMergedThisTurn)
        {
            Destroy(movingTile.gameObject);
            Destroy(collidingTile.gameObject);

            grid[(int)pos.x, (int)pos.y] = null;

            string newTileName = "tile_" + movingTileValue * 2;

            GameObject newTile = (GameObject)Instantiate(Resources.Load(newTileName, typeof(GameObject)), pos, Quaternion.identity);

            newTile.transform.parent = transform;

            newTile.GetComponent<Tile>().isMergedThisTurn = true;

            UpdateGrid();

            score += movingTileValue * 2;

            UpdateScore();

            return true;
        }
        return false;
    }

    void GenerateNewTile(int howMany)
    {
        for (int i = 0; i < howMany; ++i)
        {
            Vector2 locationForTile = GetRandomLocation();

            string tile = "tile_2";

            float randomChanceForFour = Random.Range(0f, 1f);

            if(randomChanceForFour > 0.9f)
            {
                tile = "tile_4";
            }

            GameObject newTile = (GameObject)Instantiate(Resources.Load(tile, typeof(GameObject)), locationForTile, Quaternion.identity);

            newTile.transform.parent = transform;
        }

        UpdateGrid();
    }


    Vector2 GetRandomLocation()
    {
        List<int> AvailableX = new List<int>();
        List<int> AvailableY = new List<int>();

        for(int i = 0; i < boardWidth; i++)
        {
            for(int j = 0; j < boardHeight; j++)
            {
                if(grid[i,j] == null)
                {
                    AvailableX.Add(i);
                    AvailableY.Add(j);
                }
            }
        }
        int randXIndex = Random.Range(0, AvailableX.Count);
        int randYIndex = Random.Range(0, AvailableY.Count);

        int randX = AvailableX.ElementAt(randXIndex);
        int randY = AvailableY.ElementAt(randYIndex);

        return new Vector2(randX , randY);
    }

    bool CheckIfInside(Vector2 pos)
    {
        if (pos.x >= 0 && pos.x <= boardWidth - 1 && pos.y >= 0 && pos.y <= boardHeight - 1)
        {
            return true;
        }
        return false;
    }

    bool CheckIsAtValidPosition(Vector2 pos)
    {
        if(grid[(int)pos.x , (int)pos.y] == null)
        {
            return true;
        }

        return false;
    }

    void PrepareTilesForMerging()
    {
        foreach(Transform t in transform)
        {
            t.GetComponent<Tile>().isMergedThisTurn = false;
        }
    }


    /// <summary>
    /// Restart the Game
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }
}
