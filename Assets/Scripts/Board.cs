using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Board : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform boardContainer;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject restartBtn;

    private GameInputActions inputActions;

    private const int BoardSize = 4;

    private Tile[,] tiles = new Tile[BoardSize, BoardSize];

    private int score;

    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private bool Move(Direction direction)
    {
        bool moved = false;

        for (int y = 0; y < BoardSize; y++)
        {
            int[] line = new int[BoardSize];

            
            for (int i = 0; i < BoardSize; i++)
            {
                int x = direction == Direction.Right
                    ? BoardSize - 1 - i
                    : i;

                int currentY = direction == Direction.Down
                    ? BoardSize - 1 - i
                    : i;

                if (direction == Direction.Left || direction == Direction.Right)
                {
                    line[i] = tiles[x, y].Value;
                }
                else
                {
                    line[i] = tiles[y, currentY].Value;
                }
            }

            int[] originalLine = (int[])line.Clone();

            line = CompressRow(line);
            line = MergeRow(line);

            if (!RowsEqual(originalLine, line))
            {
                moved = true;
            }

            // Devolve a linha/coluna para o tabuleiro
            for (int i = 0; i < BoardSize; i++)
            {
                int x = direction == Direction.Right
                    ? BoardSize - 1 - i
                    : i;

                int currentY = direction == Direction.Down
                    ? BoardSize - 1 - i
                    : i;

                if (direction == Direction.Left || direction == Direction.Right)
                {
                    tiles[x, y].SetValue(line[i]);
                }
                else
                {
                    tiles[y, currentY].SetValue(line[i]);
                }
            }
        }

        return moved;
    }

    private void Start()
    {
        CreateBoard();
        scoreText.text = "0";
        SpawnTile();

    }
    private void Awake()
    {
        inputActions = new GameInputActions();

        inputActions.Gameplay.Move.performed += OnMove;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        bool moved = false;

        if (input.x < 0)
            moved = Move(Direction.Left);
        else if (input.x > 0)
            moved = Move(Direction.Right);
        else if (input.y > 0)
            moved = Move(Direction.Up);
        else if (input.y < 0)
            moved = Move(Direction.Down);

        if (moved)
        {
            if (CheckWin())
            {
                Win();
                return;
            }

            SpawnTile();

            if (!CanMove())
            {
                GameOver();
            }
        }
    }

    private void CreateBoard()
    {
        for (int y = 0; y < BoardSize; y++)
        {
            for (int x = 0; x < BoardSize; x++)
            {
                Tile tile = Instantiate(tilePrefab, boardContainer);

                tile.SetValue(0);

                tiles[x, y] = tile;
            }
        }
    }
    private void SpawnTile()
    {
        int emptyCount = 0;

        for (int y = 0; y < BoardSize; y++)
        {
            for (int x = 0; x < BoardSize; x++)
            {
                if (tiles[x, y].Value == 0)
                {
                    emptyCount++;
                }
            }
        }

        if (emptyCount == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, emptyCount);

        for (int y = 0; y < BoardSize; y++)
        {
            for (int x = 0; x < BoardSize; x++)
            {
                if (tiles[x, y].Value == 0)
                {
                    if (randomIndex == 0)
                    {
                        int value = Random.value < 0.9f ? 2 : 4;

                        tiles[x, y].SetValue(value);
                        tiles[x, y].SpawnAnimation();

                        return;
                    }

                    randomIndex--;
                }
            }
        }
    }

    private bool RowsEqual(int[] a, int[] b)
    {
        for (int i = 0; i < BoardSize; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }

        return true;
    }

    private int[] CompressRow(int[] row)
    {
        int[] result = new int[BoardSize];
        int index = 0;

        for (int i = 0; i < BoardSize; i++)
        {
            if (row[i] != 0)
            {
                result[index] = row[i];
                index++;
            }
        }

        return result;
    }

    private int[] MergeRow(int[] row)
    {
        for (int i = 0; i < BoardSize - 1; i++)
        {
            if (row[i] != 0 && row[i] == row[i + 1])
            {
                row[i] *= 2;
                row[i + 1] = 0;

                AddScore(row[i]);
            }
        }

        return CompressRow(row);
    }

    private void SetRow(int y, int[] row)
    {
        for (int x = 0; x < BoardSize; x++)
        {
            tiles[x, y].SetValue(row[x]);
        }
    }

    private void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }

    private bool CanMove()
    {
        for (int y = 0; y < BoardSize; y++)
        {
            for (int x = 0; x < BoardSize; x++)
            {
                // Existe espaço vazio?
                if (tiles[x, y].Value == 0)
                {
                    return true;
                }

                // Podemos combinar com a direita?
                if (x < BoardSize - 1 &&
                    tiles[x, y].Value == tiles[x + 1, y].Value)
                {
                    return true;
                }

                // Podemos combinar para baixo?
                if (y < BoardSize - 1 &&
                    tiles[x, y].Value == tiles[x, y + 1].Value)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CheckWin()
    {
        for (int y = 0; y < BoardSize; y++)
        {
            for (int x = 0; x < BoardSize; x++)
            {
                if (tiles[x, y].Value >= 2048)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        restartBtn.SetActive(false);
    }

    private void Win()
    {
        winPanel.SetActive(true);
        restartBtn.SetActive(false);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }


}
