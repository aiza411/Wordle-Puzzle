using TMPro;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;
    public Tile.State correctState;

    [Header("Ui")]
    public GameObject invalidtxt;
    public GameObject tryAgainButton;
    public GameObject newWordButton;
      

    private static readonly KeyCode[] KEYS = new KeyCode[] { 
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z,};

    private Row[] rows;
    private int rowIndex;
    private int columnIndex;

    private string[] solutions;
    private string[] ValidWords;
    private string word;


    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
    }

    private void Start()
    {
        LoadData();
        NewGame();
    }

    private void SetRandomWord()
    {
        word=solutions[Random.Range(0, solutions.Length)];
        word=word.ToLower().Trim();
    }

    private void LoadData()
    {
        TextAsset textfile=Resources.Load("official_wordle_all") as TextAsset;
        ValidWords=textfile.text.Split('\n');

        textfile = Resources.Load("official_wordle_common") as TextAsset;
        solutions=textfile.text.Split('\n');
    }

    void Update()
    {
        Row currentRow = rows[rowIndex];
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            columnIndex=Mathf.Max(columnIndex-1, 0);
            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);

            invalidtxt.gameObject.SetActive(false);
        }

        else if (columnIndex >= rows[rowIndex].tiles.Length)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(currentRow);
            }    
        }

        else
        {
            for (int i = 0; i < KEYS.Length; i++)
            {
                if (Input.GetKeyDown(KEYS[i]))
                {
                    currentRow.tiles[columnIndex].SetLetter((char)KEYS[i]);
                    currentRow.tiles[columnIndex].SetState(occupiedState);
                    columnIndex++;
                    break;
                }
            }
        }
        
    }

    void SubmitRow(Row row)
    {
        if(!CheckForValidWord(row.word))
        {
            invalidtxt.gameObject.SetActive(true);
            return;
        }
            
        string remaining = word;

        for(int i=0; i<row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.letter==word[i])
            {
                tile.SetState(correctState);
                remaining = remaining.Remove(i,1);
                remaining=remaining.Insert(i," ");
            }

            else if (!word.Contains(tile.letter))
            {
                tile.SetState(incorrectState);
            }
        }


        for (int i = 0; i<row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.state!=correctState && tile.state!=incorrectState)
            {
                if(remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);

                    int index=remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index,1);
                    remaining=remaining.Insert(index," ");
                }

                else
                {
                    tile.SetState(incorrectState);
                }
            }
        }


        //for(int i=0; i<row.tiles.Length; i++)
        //{
        //    Tile tile = row.tiles[i];
        //    if (tile.letter==word[i])
        //    {
        //        tile.SetState(correctState);
        //    }

        //    else if(word.Contains(tile.letter))
        //    {
        //        tile.SetState(wrongSpotState);
        //    }

        //    else
        //    {
        //        tile.SetState(incorrectState);
        //    }
        //}

        if(HasWon(row))
        {
            enabled = false;

        }

        rowIndex++;
        columnIndex=0;

        if(rowIndex>=rows.Length)
        {
            enabled = false;
        }
    }

    private bool CheckForValidWord(string word)
    {
        for(int i=0; i<ValidWords.Length; i++) 
        {
            if (ValidWords[i] == word)
            {
                return true;
            }
        }

        return false;
    }


    private bool HasWon(Row row)
    {
        for (int i = 0; i<row.tiles.Length; i++)
        {
            if (row.tiles[i].state != correctState)
            {
                return false;
            }
        }

        return true;
    }

    private void OnEnable()
    {
        tryAgainButton.SetActive(false);
        newWordButton.SetActive(false);
    }

    private void OnDisable() 
    {
        tryAgainButton.SetActive(true);
        newWordButton.SetActive(true);
    }

    public void NewGame()
    {
        ClearBoard();
        SetRandomWord();
        enabled = true;
    }

    public void TryAgain()
    {
        ClearBoard();
        enabled = true;
    }

    private void ClearBoard()
    {
        for(int row=0; row<rows.Length; row++)
        { 
            for(int col=0; col<rows[row].tiles.Length; col++)
            {
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetState(emptyState);
            }
        }

        rowIndex=0;
        columnIndex=0;  
    }

}
