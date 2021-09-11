using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> Sprites = new List<Sprite>();
    public GameObject TilePrefab;
    public int GridDimension = 8;
    public float Distance = 1.0f;
    private GameObject[,] Grid;
    HashSet<SpriteRenderer> matchedTilesRemaining = new HashSet<SpriteRenderer>();




    public int StartingMoves = 50; // 2
    private int _numMoves; // 3
    public int NumMoves
    {
        get
        {
            return _numMoves;
        }

        set
        {
            _numMoves = value;
            MovesText.text = _numMoves.ToString();
        }
    }

    private int _score;
    public int Score
    {
        get
        {
            return _score;
        }

        set
        {
            _score = value;
            ScoreText.text = _score.ToString();
        }
    }
    public GameObject GameOverMenu; // 2
    public TextMeshProUGUI MovesText;
    public TextMeshProUGUI ScoreText;
    public static GridManager Instance { get; private set; }
    void Awake() { 
        Instance = this;
        Score = 0;
        NumMoves = StartingMoves;
        GameOverMenu.SetActive(false);
    }
    void Start()
    {
        Grid = new GameObject[GridDimension, GridDimension];
        InitGrid();
        SoundManager.Instance.PlaySound(SoundType.TypeGameOver);
        //TiteCheck();
    }

    // Update is called once per frame
    void InitGrid()
    {
        
        Vector3 positionOffset = transform.position - new Vector3(GridDimension * Distance / 2.0f, GridDimension * Distance / 2.0f, 0); // 1
        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++) // 2
            {
                GameObject newTile = Instantiate(TilePrefab);
                List<Sprite> possibleSprites = new List<Sprite>(); // 1

                //Choose what sprite to use for this cell
                Sprite left1 = GetSpriteAt(column - 1, row); //2
                Sprite left2 = GetSpriteAt(column - 2, row);
                if (left2 != null && left1 == left2) // 3
                {
                    possibleSprites.Remove(left1); // 4
                }

                Sprite down1 = GetSpriteAt(column, row - 1); // 5
                Sprite down2 = GetSpriteAt(column, row - 2);
                if (down2 != null && down1 == down2)
                {
                    possibleSprites.Remove(down1);
                }


                 
                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>(); // 4
                renderer.sprite = Sprites[Random.Range(0, Sprites.Count)]; // 5
                newTile.transform.parent = transform; // 6
                Tile tile = newTile.AddComponent<Tile>();
                tile.Position = new Vector2Int(column, row);
                newTile.transform.position = new Vector3(column * Distance, row * Distance, 0) + positionOffset; // 7

                Grid[column, row] = newTile; // 8
            }
        }


        
    }
    Sprite GetSpriteAt(int column, int row)
    {
        if (column < 0 || column >= GridDimension
            || row < 0 || row >= GridDimension)
            return null;
        GameObject tile = Grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        
        return renderer.sprite;
    }
    public void SwapTiles(Vector2Int tile1Position, Vector2Int tile2Position) // 1
    {

        // 2
        GameObject tile1 = Grid[tile1Position.x, tile1Position.y];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();

        GameObject tile2 = Grid[tile2Position.x, tile2Position.y];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        // 3
        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;
        
        SoundManager.Instance.PlaySound(SoundType.TypeMove);
        bool changesOccurs = CheckMatches();
        if (!changesOccurs)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundType.TypePop);
            NumMoves--;
            do
            {
                FillHoles();
            } while (CheckMatches());
            if (NumMoves <= 0)
            {
                NumMoves = 0;
                GameOver();
            }
            
        }
    }

    void GameOver()
    {
        Debug.Log("GameOver");
        PlayerPrefs.SetInt("score", Score);
        GameOverMenu.SetActive(true);
        SoundManager.Instance.PlaySound(SoundType.TypeGameOver);
    }
    SpriteRenderer GetSpriteRendererAt(int column, int row)
    {
        if (column < 0 || column >= GridDimension
             || row < 0 || row >= GridDimension)
            return null;
        GameObject tile = Grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer;
    }
    int gemCanBlowUP = 0;
    public void TiteCheck()
    {
        
        
        /*GetSpriteRendererAt(0, 0);//
        GetSpriteRendererAt(1, 0);
        SpriteRenderer renderer1 = GetSpriteRendererAt(0, 0);
        SpriteRenderer renderer2 = GetSpriteRendererAt(1, 0);
        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;
        bool changesOccurs = CheckBoarMatches();
        if (!changesOccurs)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }*/
        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++) // 2
            {
                

                SpriteRenderer renderer1 = GetSpriteRendererAt(row, column);
                //Debug.Log("row: "+row+" column: "+column);
                CheckTiteUp(renderer1, row, column);
                CheckTiteDown(renderer1, row, column);
                CheckTiteLeft(renderer1, row, column);
                CheckTiteRight(renderer1, row, column);
                
                //Debug.Log("TiteCheck");
                
            }
        }
        if (gemCanBlowUP == 0)
        {
            GameOver();
        }
        Debug.Log("Gem can be Moved " + gemCanBlowUP);
        gemCanBlowUP = 0;
        
    }

    public void CheckTiteUp(SpriteRenderer renderer1, int column, int row)
    {
        int resultColumn = column + 1;


        if (resultColumn == 8 || resultColumn == -1)
            return;
            SpriteRenderer renderer2 = GetSpriteRendererAt(column + 1, row);
            Sprite temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
            bool changesOccurs = CheckBoarMatches();
            if (changesOccurs)
            {
                gemCanBlowUP += 1;
            }
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
            //Debug.Log("Up Check");
        

        /*bool changesOccurs = CheckMatches();
        if (!changesOccurs)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundType.TypePop);
            NumMoves--;
            do
            {
                FillHoles();
            } while (CheckMatches());
            if (NumMoves <= 0)
            {
                NumMoves = 0;
                GameOver();
            }

        }*/
    }
    public void CheckTiteDown(SpriteRenderer renderer1, int column, int row)
    {
        
        int columnResult = column - 1;

        if (columnResult == 8 || columnResult == -1)
            return;
            SpriteRenderer renderer2 = GetSpriteRendererAt(column - 1, row);
            Sprite temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
            bool changesOccurs = CheckBoarMatches();
            if (changesOccurs)
            {
                gemCanBlowUP += 1;
            }
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
            //Debug.Log("CheckTiteDown");
        
        /*bool changesOccurs = CheckMatches();
        if (!changesOccurs)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundType.TypePop);
            NumMoves--;
            do
            {
                FillHoles();
            } while (CheckMatches());
            if (NumMoves <= 0)
            {
                NumMoves = 0;
                GameOver();
            }

        }*/
    }
    public void CheckTiteLeft(SpriteRenderer renderer1, int column, int row)
    {
        int rowResult = row - 1;

        if (rowResult == 8 || rowResult == -1)
            return;


            SpriteRenderer renderer2 = GetSpriteRendererAt(column, row - 1);
            Sprite temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
            bool changesOccurs = CheckBoarMatches();
            if (changesOccurs)
            {
                gemCanBlowUP += 1;
            }
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
            //Debug.Log("CheckTiteLeft");
        
        /*bool changesOccurs = CheckMatches();
        if (!changesOccurs)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundType.TypePop);
            NumMoves--;
            do
            {
                FillHoles();
            } while (CheckMatches());
            if (NumMoves <= 0)
            {
                NumMoves = 0;
                GameOver();
            }

        }*/
    }
    public void CheckTiteRight(SpriteRenderer renderer1, int column, int row)
    {
        int rowResult = row + 1;

        if (rowResult == 8 || rowResult == -1)
            return;


            SpriteRenderer renderer2 = GetSpriteRendererAt(column, row + 1);
            Sprite temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
            bool changesOccurs = CheckBoarMatches();
            if (changesOccurs)
            {
                gemCanBlowUP += 1;
            }
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
            //Debug.Log("CheckTiteRight");
        
        /*bool changesOccurs = CheckMatches();
        if (!changesOccurs)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundType.TypePop);
            NumMoves--;
            do
            {
                FillHoles();
            } while (CheckMatches());
            if (NumMoves <= 0)
            {
                NumMoves = 0;
                GameOver();
            }

        }*/
    }
    
    bool CheckMatches()
    {
        HashSet<SpriteRenderer> matchedTiles = new HashSet<SpriteRenderer>(); // 1
        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++) // 2
            {
                SpriteRenderer current = GetSpriteRendererAt(column, row); // 3

                List<SpriteRenderer> horizontalMatches = FindColumnMatchForTile(column, row, current.sprite); // 4
                if (horizontalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(horizontalMatches);
                    matchedTiles.Add(current); // 5
                }

                List<SpriteRenderer> verticalMatches = FindRowMatchForTile(column, row, current.sprite); // 6
                if (verticalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(verticalMatches);
                    matchedTiles.Add(current);
                }
            }
        }

        foreach (SpriteRenderer renderer in matchedTiles) // 7
        {
            renderer.sprite = null;
        }
        Score += matchedTiles.Count;
        return matchedTiles.Count > 0; // 8
    }
     
    bool CheckBoarMatches()
    {
        HashSet<SpriteRenderer> matchedTiles = new HashSet<SpriteRenderer>(); // 1
        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++) // 2
            {
                SpriteRenderer current = GetSpriteRendererAt(column, row); // 3

                List<SpriteRenderer> horizontalMatches = FindColumnMatchForTile(column, row, current.sprite); // 4
                if (horizontalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(horizontalMatches);
                    matchedTiles.Add(current); // 5
                    
                }

                List<SpriteRenderer> verticalMatches = FindRowMatchForTile(column, row, current.sprite); // 6
                if (verticalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(verticalMatches);
                    matchedTiles.Add(current);
                    
                }
            }
        }
        //Debug.Log("CheckBoarMatches");
        return matchedTiles.Count > 0;
    }

    List<SpriteRenderer> FindColumnMatchForTile(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = col + 1; i < GridDimension; i++)
        {
            SpriteRenderer nextColumn = GetSpriteRendererAt(i, row);
            if (nextColumn.sprite != sprite)
            {
                break;
            }
            result.Add(nextColumn);
        }
        return result;
    }
    List<SpriteRenderer> FindRowMatchForTile(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = row + 1; i < GridDimension; i++)
        {
            SpriteRenderer nextRow = GetSpriteRendererAt(col, i);
            if (nextRow.sprite != sprite)
            {
                break;
            }
            result.Add(nextRow);
        }
        return result;
    }

    void FillHoles()
    {
        for (int column = 0; column < GridDimension; column++)
            for (int row = 0; row < GridDimension; row++)
            {
                while (GetSpriteRendererAt(column, row).sprite == null)
                {
                    SpriteRenderer current = GetSpriteRendererAt(column, row);
                    SpriteRenderer next = current;
                    for (int filler = row; filler < GridDimension - 1; filler++)
                    {
                        next = GetSpriteRendererAt(column, filler + 1);
                        current.sprite = next.sprite;
                        current = next;
                    }
                    next.sprite = Sprites[Random.Range(0, Sprites.Count)];
                }
            }
    }
    
}
