using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;

[RequireComponent(typeof(PhotonView))]
public class GameScript : MonoBehaviour
{
    private PhotonView photonView;

    public GameObject gamepiece;
    public GameObject fightCanvas;

    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    public string playerColor;
    public string currentPlayer = "white";

    private bool gameOver = false;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerColor = PhotonNetwork.IsMasterClient ? "white" : "black";

        Debug.Log(playerColor);

        playerWhite = new GameObject[]
        {
            Create("white_rook1", 0, 0), Create("white_knight1", 1, 0), Create("white_bishop1", 2, 0), Create("white_queen", 3, 0),
            Create("white_king", 4, 0), Create("white_bishop2", 5, 0), Create("white_knight2", 6, 0), Create("white_rook2", 7, 0),
            Create("white_pawn1", 0, 1), Create("white_pawn2", 1, 1), Create("white_pawn3", 2, 1), Create("white_pawn4", 3, 1),
            Create("white_pawn5", 4, 1), Create("white_pawn6", 5, 1), Create("white_pawn7", 6, 1), Create("white_pawn8", 7, 1)
        };
        playerBlack = new GameObject[]
        {
            Create("black_rook1", 0, 7), Create("black_knight1", 1, 7), Create("black_bishop1", 2, 7), Create("black_queen", 3, 7),
            Create("black_king", 4, 7), Create("black_bishop2", 5, 7), Create("black_knight2", 6, 7), Create("black_rook2", 7, 7),
            Create("black_pawn1", 0, 6), Create("black_pawn2", 1, 6), Create("black_pawn3", 2, 6), Create("black_pawn4", 3, 6),
            Create("black_pawn5", 4, 6), Create("black_pawn6", 5, 6), Create("black_pawn7", 6, 6), Create("black_pawn8", 7, 6)
        };

        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(gamepiece, new Vector3(0, 0, -1), Quaternion.identity);
        PieceScript ps = obj.GetComponent<PieceScript>();
        ps.name = name;
        ps.SetXBoard(x);
        ps.SetYBoard(y);
        ps.Activate();
        return obj;
    }

    public static string RemoveDigits(string key)
    {
        return Regex.Replace(key, @"\d", "");
    }

    public void StartFight(string attacker, string defender, int matrixX, int matrixY)
    {
        photonView.RPC(nameof(RPC_StartFight), RpcTarget.OthersBuffered, attacker, defender, matrixX, matrixY);

        if (attacker.Contains("white"))
        {
            StaticClass.whiteFightPiece = attacker;
            StaticClass.blackFightPiece = defender;
        }
        else
        {
            StaticClass.blackFightPiece = attacker;
            StaticClass.whiteFightPiece = defender;
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Chess"))
        {
            obj.GetComponent<SpriteRenderer>().enabled = false;
            if (obj.name != "Board")
                obj.GetComponent<BoxCollider2D>().enabled = false;
        }

        fightCanvas.SetActive(true);

        StaticClass.fighting = true;

        StartCoroutine(GameObject.Find("BattleSystem").GetComponent<BattleSystem>().SetupBattle(attacker, defender, matrixX, matrixY, GameObject.Find(attacker).GetComponent<PieceScript>().hp, GameObject.Find(defender).GetComponent<PieceScript>().hp));
    }

    [PunRPC]
    public void RPC_StartFight(string attacker, string defender, int matrixX, int matrixY)
    {
        if (attacker.Contains("white"))
        {
            StaticClass.whiteFightPiece = attacker;
            StaticClass.blackFightPiece = defender;
        }
        else
        {
            StaticClass.blackFightPiece = attacker;
            StaticClass.whiteFightPiece = defender;
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Chess"))
        {
            obj.GetComponent<SpriteRenderer>().enabled = false;
            if (obj.name != "Board")
                obj.GetComponent<BoxCollider2D>().enabled = false;
        }

        fightCanvas.SetActive(true);

        StaticClass.fighting = true;
    }

    public void FightOver(string attacker, string defender, bool attackerWon, int matrixX, int matrixY)
    {
        StaticClass.fighting = false;
        StaticClass.whiteFightPiece = null;
        StaticClass.blackFightPiece = null;
        //attacker = reference, defender = cp

        Debug.Log(attacker + defender + attackerWon + matrixX + matrixY);

        GameObject reference = GameObject.Find(attacker);
        GameObject cp = GameObject.Find(defender);

        if (attackerWon)
        {
            Destroy(cp);

            SetPositionEmpty(reference.GetComponent<PieceScript>().GetXBoard(), reference.GetComponent<PieceScript>().GetYBoard());

            SetReferencePiecePosition(reference.name, matrixX, matrixY);

            SetPosition(reference.name);

            if (cp.name == "white_king")
                StartCoroutine(GameOver("black"));
            else if (cp.name == "black_king")
                StartCoroutine(GameOver("white"));
        }
        else
        {
            Destroy(reference);

            if (reference.name == "white_king")
                StartCoroutine(GameOver("black"));
            else if (reference.name == "black_king")
                StartCoroutine(GameOver("white"));
        }

        NextTurn();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Chess"))
        {
            obj.GetComponent<SpriteRenderer>().enabled = true;
            if (obj.name != "Board")
                obj.GetComponent<BoxCollider2D>().enabled = true;
        }
        fightCanvas.SetActive(false);
    }

    public void SetPosition(GameObject obj)
    {
        PieceScript ps = obj.GetComponent<PieceScript>();

        positions[ps.GetXBoard(), ps.GetYBoard()] = obj;
    }

    public void SetPosition(string name)
    {
        photonView.RPC(nameof(RPC_SetPosition), RpcTarget.OthersBuffered, name);

        GameObject obj = GameObject.Find(name);

        PieceScript ps = obj.GetComponent<PieceScript>();

        positions[ps.GetXBoard(), ps.GetYBoard()] = obj;
    }

    [PunRPC]
    private void RPC_SetPosition(string name)
    {
        GameObject obj = GameObject.Find(name);

        PieceScript ps = obj.GetComponent<PieceScript>();

        positions[ps.GetXBoard(), ps.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        photonView.RPC(nameof(RPC_SetPositionEmpty), RpcTarget.OthersBuffered, x, y);

        positions[x, y] = null;
    }

    [PunRPC]
    public void RPC_SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public void SetReferencePiecePosition(string name, int matrixX, int matrixY)
    {
        photonView.RPC(nameof(RPC_SetReferencePiecePosition), RpcTarget.OthersBuffered, name, matrixX, matrixY);

        GameObject reference = GameObject.Find(name);

        reference.GetComponent<PieceScript>().SetXBoard(matrixX);
        reference.GetComponent<PieceScript>().SetYBoard(matrixY);
        reference.GetComponent<PieceScript>().SetCoordinates();
    }

    [PunRPC]
    private void RPC_SetReferencePiecePosition(string name, int matrixX, int matrixY)
    {
        GameObject reference = GameObject.Find(name);

        reference.GetComponent<PieceScript>().SetXBoard(matrixX);
        reference.GetComponent<PieceScript>().SetYBoard(matrixY);
        reference.GetComponent<PieceScript>().SetCoordinates();
    }

    public void DestroyPiece(int x, int y)
    {
        photonView.RPC(nameof(RPC_DestroyPiece), RpcTarget.OthersBuffered, x, y);
        GameObject obj = GetPosition(x, y);
        Destroy(obj);
    }

    [PunRPC]
    private void RPC_DestroyPiece(int x, int y)
    {
        GameObject obj = GetPosition(x, y);
        Destroy(obj);
    }


    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1))
            return false;
        else
            return true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public string GetAssignedPlayer()
    {
        return playerColor;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
            photonView.RPC(nameof(RPC_NextTurn), RpcTarget.OthersBuffered, "black");
        }
        else
        {
            currentPlayer = "white";
            photonView.RPC(nameof(RPC_NextTurn), RpcTarget.OthersBuffered, "white");

        }
        Debug.Log(currentPlayer);
    }

    [PunRPC]
    private void RPC_NextTurn(string curPlayer)
    {
        currentPlayer = curPlayer;
    }

    IEnumerator GameOver(string winner)
    {
        Debug.Log("Game OVER! " + winner + "won!");
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
