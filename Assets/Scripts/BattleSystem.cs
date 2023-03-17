using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public enum BattleState { START, WHITETURN, BLACKTURN, WHITEWON, BLACKWON }

[RequireComponent(typeof(PhotonView))]
public class BattleSystem : MonoBehaviour
{
    PhotonView photonView;

    string player;
    string attacker;
    string defender;

    int matrixX;
    int matrixY;

    public BattleState state;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public TMP_Text dialogueText;
    public GameObject attackButton;
    public GameObject healButton;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public Sprite black_king, black_queen, black_knight, black_bishop, black_rook, black_pawn;
    public Sprite white_king, white_queen, white_knight, white_bishop, white_rook, white_pawn;

    public GameObject PawnExplosion, BishopKnightExplosion, RookExplosion, QueenExplosion, KingExplosion;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        player = GameObject.Find("Controller").GetComponent<GameScript>().playerColor;
    }

    bool IsItMyTurn()
    {
        if ((state == BattleState.WHITETURN && player == "white") || (state == BattleState.BLACKTURN && player == "black"))
            return true;
        return false;
    }

    public IEnumerator SetupBattle(string att, string def, int mx, int my, int attHP, int defHP)
    {
        attacker = att;
        defender = def;
        matrixX = mx;
        matrixY = my;

        photonView.RPC(nameof(RPC_SetupBattle), RpcTarget.OthersBuffered, attacker, defender, mx, my, attHP, defHP);

        state = BattleState.START;

        //name: dmg/hp/heal
        //Pawn: 2/4/1, Rook: 3/10/2, Knight: 3/8/2, Bishop: 3/8/2, Queen: 4/12/3, King: 3/15/2

        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        if (player == "white")
        {
            switch (att.Contains("white") ? att : def)
            {
                case string a when a.Contains("pawn"): playerGO.GetComponentInChildren<Image>().sprite = white_pawn; playerUnit.unitName = "White Pawn"; playerUnit.damage = 2; playerUnit.maxHP = 4; playerUnit.currentHP = 4; playerUnit.heal = 1; break;
                case string b when b.Contains("rook"): playerGO.GetComponentInChildren<Image>().sprite = white_rook; playerUnit.unitName = "White Rook"; playerUnit.damage = 3; playerUnit.maxHP = 10; playerUnit.currentHP = 10; playerUnit.heal = 2; break;
                case string c when c.Contains("knight"): playerGO.GetComponentInChildren<Image>().sprite = white_knight; playerUnit.unitName = "White Knight"; playerUnit.damage = 3; playerUnit.maxHP = 8; playerUnit.currentHP = 8; playerUnit.heal = 2; break;
                case string d when d.Contains("bishop"): playerGO.GetComponentInChildren<Image>().sprite = white_bishop; playerUnit.unitName = "White Bishop"; playerUnit.damage = 3; playerUnit.maxHP = 8; playerUnit.currentHP = 8; playerUnit.heal = 2; break;
                case string e when e.Contains("queen"): playerGO.GetComponentInChildren<Image>().sprite = white_queen; playerUnit.unitName = "White Queen"; playerUnit.damage = 4; playerUnit.maxHP = 12; playerUnit.currentHP = 12; playerUnit.heal = 3; break;
                case string f when f.Contains("king"): playerGO.GetComponentInChildren<Image>().sprite = white_king; playerUnit.unitName = "White King"; playerUnit.damage = 3; playerUnit.maxHP = 15; playerUnit.currentHP = 15; playerUnit.heal = 2; break;
            }
        } else if (player == "black")
        {
            switch (att.Contains("black") ? att : def)
            {
                case string a when a.Contains("pawn"): playerGO.GetComponentInChildren<Image>().sprite = black_pawn; playerUnit.unitName = "Black Pawn"; playerUnit.damage = 2; playerUnit.maxHP = 4; playerUnit.currentHP = 4; playerUnit.heal = 1; break;
                case string b when b.Contains("rook"): playerGO.GetComponentInChildren<Image>().sprite = black_rook; playerUnit.unitName = "Black Rook"; playerUnit.damage = 3; playerUnit.maxHP = 10; playerUnit.currentHP = 10; playerUnit.heal = 2; break;
                case string c when c.Contains("knight"): playerGO.GetComponentInChildren<Image>().sprite = black_knight; playerUnit.unitName = "Black Knight"; playerUnit.damage = 3; playerUnit.maxHP = 8; playerUnit.currentHP = 8; playerUnit.heal = 2; break;
                case string d when d.Contains("bishop"): playerGO.GetComponentInChildren<Image>().sprite = black_bishop; playerUnit.unitName = "Black Bishop"; playerUnit.damage = 3; playerUnit.maxHP = 8; playerUnit.currentHP = 8; playerUnit.heal = 2; break;
                case string e when e.Contains("queen"): playerGO.GetComponentInChildren<Image>().sprite = black_queen; playerUnit.unitName = "Black Queen"; playerUnit.damage = 4; playerUnit.maxHP = 12; playerUnit.currentHP = 12; playerUnit.heal = 3; break;
                case string f when f.Contains("king"): playerGO.GetComponentInChildren<Image>().sprite = black_king; playerUnit.unitName = "Black King"; playerUnit.damage = 3; playerUnit.maxHP = 15; playerUnit.currentHP = 15; playerUnit.heal = 2; break;
            }
        }

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        if (player == "black")
        {
            //Pawn: 2/4/1, Rook: 3/10/2, Knight: 3/8/2, Bishop: 3/8/2, Queen: 4/12/3, King: 5/15/4
            switch (att.Contains("white") ? att : def)
            {
                case string a when a.Contains("pawn"): enemyGO.GetComponentInChildren<Image>().sprite = white_pawn; enemyUnit.unitName = "White Pawn"; enemyUnit.damage = 2; enemyUnit.maxHP = 4; enemyUnit.currentHP = 4; enemyUnit.heal = 1; break;
                case string b when b.Contains("rook"): enemyGO.GetComponentInChildren<Image>().sprite = white_rook; enemyUnit.unitName = "White Rook"; enemyUnit.damage = 3; enemyUnit.maxHP = 10; enemyUnit.currentHP = 10; enemyUnit.heal = 2; break;
                case string c when c.Contains("knight"): enemyGO.GetComponentInChildren<Image>().sprite = white_knight; enemyUnit.unitName = "White Knight"; enemyUnit.damage = 3; enemyUnit.maxHP = 8; enemyUnit.currentHP = 8; enemyUnit.heal = 2; break;
                case string d when d.Contains("bishop"): enemyGO.GetComponentInChildren<Image>().sprite = white_bishop; enemyUnit.unitName = "White Bishop"; enemyUnit.damage = 3; enemyUnit.maxHP = 8; enemyUnit.currentHP = 8; enemyUnit.heal = 2; break;
                case string e when e.Contains("queen"): enemyGO.GetComponentInChildren<Image>().sprite = white_queen; enemyUnit.unitName = "White Queen"; enemyUnit.damage = 4; enemyUnit.maxHP = 12; enemyUnit.currentHP = 12; enemyUnit.heal = 3; break;
                case string f when f.Contains("king"): enemyGO.GetComponentInChildren<Image>().sprite = white_king; enemyUnit.unitName = "White King"; enemyUnit.damage = 3; enemyUnit.maxHP = 15; enemyUnit.currentHP = 15; enemyUnit.heal = 2; break;
            }
        }
        else if (player == "white")
        {
            switch (att.Contains("black") ? att : def)
            {
                case string a when a.Contains("pawn"): enemyGO.GetComponentInChildren<Image>().sprite = black_pawn; enemyUnit.unitName = "Black Pawn"; enemyUnit.damage = 2; enemyUnit.maxHP = 4; enemyUnit.currentHP = 4; enemyUnit.heal = 1; break;
                case string b when b.Contains("rook"): enemyGO.GetComponentInChildren<Image>().sprite = black_rook; enemyUnit.unitName = "Black Rook"; enemyUnit.damage = 3; enemyUnit.maxHP = 10; enemyUnit.currentHP = 10; enemyUnit.heal = 2; break;
                case string c when c.Contains("knight"): enemyGO.GetComponentInChildren<Image>().sprite = black_knight; enemyUnit.unitName = "Black Knight"; enemyUnit.damage = 3; enemyUnit.maxHP = 8; enemyUnit.currentHP = 8; enemyUnit.heal = 2; break;
                case string d when d.Contains("bishop"): enemyGO.GetComponentInChildren<Image>().sprite = black_bishop; enemyUnit.unitName = "Black Bishop"; enemyUnit.damage = 3; enemyUnit.maxHP = 8; enemyUnit.currentHP = 8; enemyUnit.heal = 2; break;
                case string e when e.Contains("queen"): enemyGO.GetComponentInChildren<Image>().sprite = black_queen; enemyUnit.unitName = "Black Queen"; enemyUnit.damage = 4; enemyUnit.maxHP = 12; enemyUnit.currentHP = 12; enemyUnit.heal = 3; break;
                case string f when f.Contains("king"): enemyGO.GetComponentInChildren<Image>().sprite = black_king; enemyUnit.unitName = "Black King"; enemyUnit.damage = 3; enemyUnit.maxHP = 15; enemyUnit.currentHP = 15; enemyUnit.heal = 2; break; 
            }
        }

        if (player == "white" && att.Contains("white") && attHP > 0)
            playerUnit.currentHP = attHP;
        else if (player == "white" && def.Contains("white") && defHP > 0)
            playerUnit.currentHP = defHP;
        else if (player == "black" && att.Contains("black") && attHP > 0)
            playerUnit.currentHP = attHP;
        else if (player == "black" && def.Contains("black") && defHP > 0)
            playerUnit.currentHP = defHP;

        if (player == "white" && att.Contains("black") && attHP > 0)
            enemyUnit.currentHP = attHP;
        else if (player == "white" && def.Contains("black") && defHP > 0)
            enemyUnit.currentHP = defHP;
        else if (player == "black" && att.Contains("white") && attHP > 0)
            enemyUnit.currentHP = attHP;
        else if (player == "black" && def.Contains("white") && defHP > 0)
            enemyUnit.currentHP = defHP;

        dialogueText.text = "The fight between " + playerUnit.unitName + " and " + enemyUnit.unitName + " begins!";
        attackButton.SetActive(false);
        healButton.SetActive(false);

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        if (attacker.Contains("white"))
            ChangeState(BattleState.WHITETURN);
        else
            ChangeState(BattleState.BLACKTURN);
    }

    [PunRPC]
    public void RPC_SetupBattle(string att, string def, int mx, int my, int attHP, int defHP)
    {
        attacker = att;
        defender = def;
        matrixX = mx;
        matrixY = my;

        state = BattleState.START;

        //Pawn: 2/4/1, Rook: 3/10/2, Knight: 3/8/2, Bishop: 3/9/2, Queen: 4/12/3, King: 5/15/4

        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        if (player == "white")
        {
            switch (att.Contains("white") ? att : def)
            {
                case string a when a.Contains("pawn"): playerGO.GetComponentInChildren<Image>().sprite = white_pawn; playerUnit.unitName = "White Pawn"; playerUnit.damage = 2; playerUnit.maxHP = 4; playerUnit.currentHP = 4; playerUnit.heal = 1; break;
                case string b when b.Contains("rook"): playerGO.GetComponentInChildren<Image>().sprite = white_rook; playerUnit.unitName = "White Rook"; playerUnit.damage = 3; playerUnit.maxHP = 10; playerUnit.currentHP = 10; playerUnit.heal = 2; break;
                case string c when c.Contains("knight"): playerGO.GetComponentInChildren<Image>().sprite = white_knight; playerUnit.unitName = "White Knight"; playerUnit.damage = 3; playerUnit.maxHP = 8; playerUnit.currentHP = 8; playerUnit.heal = 2; break;
                case string d when d.Contains("bishop"): playerGO.GetComponentInChildren<Image>().sprite = white_bishop; playerUnit.unitName = "White Bishop"; playerUnit.damage = 3; playerUnit.maxHP = 8; playerUnit.currentHP = 8; playerUnit.heal = 2; break;
                case string e when e.Contains("queen"): playerGO.GetComponentInChildren<Image>().sprite = white_queen; playerUnit.unitName = "White Queen"; playerUnit.damage = 4; playerUnit.maxHP = 12; playerUnit.currentHP = 12; playerUnit.heal = 3; break;
                case string f when f.Contains("king"): playerGO.GetComponentInChildren<Image>().sprite = white_king; playerUnit.unitName = "White King"; playerUnit.damage = 3; playerUnit.maxHP = 15; playerUnit.currentHP = 15; playerUnit.heal = 2; break;
            }
        }
        else if (player == "black")
        {
            switch (att.Contains("black") ? att : def)
            {
                case string a when a.Contains("pawn"): playerGO.GetComponentInChildren<Image>().sprite = black_pawn; playerUnit.unitName = "Black Pawn"; playerUnit.damage = 2; playerUnit.maxHP = 4; playerUnit.currentHP = 4; playerUnit.heal = 1; break;
                case string b when b.Contains("rook"): playerGO.GetComponentInChildren<Image>().sprite = black_rook; playerUnit.unitName = "Black Rook"; playerUnit.damage = 3; playerUnit.maxHP = 10; playerUnit.currentHP = 10; playerUnit.heal = 2; break;
                case string c when c.Contains("knight"): playerGO.GetComponentInChildren<Image>().sprite = black_knight; playerUnit.unitName = "Black Knight"; playerUnit.damage = 3; playerUnit.maxHP = 8; playerUnit.currentHP = 8; playerUnit.heal = 2; break;
                case string d when d.Contains("bishop"): playerGO.GetComponentInChildren<Image>().sprite = black_bishop; playerUnit.unitName = "Black Bishop"; playerUnit.damage = 3; playerUnit.maxHP = 8; playerUnit.currentHP = 8; playerUnit.heal = 2; break;
                case string e when e.Contains("queen"): playerGO.GetComponentInChildren<Image>().sprite = black_queen; playerUnit.unitName = "Black Queen"; playerUnit.damage = 4; playerUnit.maxHP = 12; playerUnit.currentHP = 12; playerUnit.heal = 3; break;
                case string f when f.Contains("king"): playerGO.GetComponentInChildren<Image>().sprite = black_king; playerUnit.unitName = "Black King"; playerUnit.damage = 3; playerUnit.maxHP = 15; playerUnit.currentHP = 15; playerUnit.heal = 2; break;
            }
        }

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        if (player == "black")
        {
            switch (att.Contains("white") ? att : def)
            {
                case string a when a.Contains("pawn"): enemyGO.GetComponentInChildren<Image>().sprite = white_pawn; enemyUnit.unitName = "White Pawn"; enemyUnit.damage = 2; enemyUnit.maxHP = 4; enemyUnit.currentHP = 4; enemyUnit.heal = 1; break;
                case string b when b.Contains("rook"): enemyGO.GetComponentInChildren<Image>().sprite = white_rook; enemyUnit.unitName = "White Rook"; enemyUnit.damage = 3; enemyUnit.maxHP = 10; enemyUnit.currentHP = 10; enemyUnit.heal = 2; break;
                case string c when c.Contains("knight"): enemyGO.GetComponentInChildren<Image>().sprite = white_knight; enemyUnit.unitName = "White Knight"; enemyUnit.damage = 3; enemyUnit.maxHP = 8; enemyUnit.currentHP = 8; enemyUnit.heal = 2; break;
                case string d when d.Contains("bishop"): enemyGO.GetComponentInChildren<Image>().sprite = white_bishop; enemyUnit.unitName = "White Bishop"; enemyUnit.damage = 3; enemyUnit.maxHP = 8; enemyUnit.currentHP = 8; enemyUnit.heal = 2; break;
                case string e when e.Contains("queen"): enemyGO.GetComponentInChildren<Image>().sprite = white_queen; enemyUnit.unitName = "White Queen"; enemyUnit.damage = 4; enemyUnit.maxHP = 12; enemyUnit.currentHP = 12; enemyUnit.heal = 3; break;
                case string f when f.Contains("king"): enemyGO.GetComponentInChildren<Image>().sprite = white_king; enemyUnit.unitName = "White King"; enemyUnit.damage = 3; enemyUnit.maxHP = 15; enemyUnit.currentHP = 15; enemyUnit.heal = 2; break;
            }
        }
        else if (player == "white")
        {
            switch (att.Contains("black") ? att : def)
            {
                case string a when a.Contains("pawn"): enemyGO.GetComponentInChildren<Image>().sprite = black_pawn; enemyUnit.unitName = "Black Pawn"; enemyUnit.damage = 2; enemyUnit.maxHP = 4; enemyUnit.currentHP = 4; enemyUnit.heal = 1; break;
                case string b when b.Contains("rook"): enemyGO.GetComponentInChildren<Image>().sprite = black_rook; enemyUnit.unitName = "Black Rook"; enemyUnit.damage = 3; enemyUnit.maxHP = 10; enemyUnit.currentHP = 10; enemyUnit.heal = 2; break;
                case string c when c.Contains("knight"): enemyGO.GetComponentInChildren<Image>().sprite = black_knight; enemyUnit.unitName = "Black Knight"; enemyUnit.damage = 3; enemyUnit.maxHP = 8; enemyUnit.currentHP = 8; enemyUnit.heal = 2; break;
                case string d when d.Contains("bishop"): enemyGO.GetComponentInChildren<Image>().sprite = black_bishop; enemyUnit.unitName = "Black Bishop"; enemyUnit.damage = 3; enemyUnit.maxHP = 8; enemyUnit.currentHP = 8; enemyUnit.heal = 2; break;
                case string e when e.Contains("queen"): enemyGO.GetComponentInChildren<Image>().sprite = black_queen; enemyUnit.unitName = "Black Queen"; enemyUnit.damage = 4; enemyUnit.maxHP = 12; enemyUnit.currentHP = 12; enemyUnit.heal = 3; break;
                case string f when f.Contains("king"): enemyGO.GetComponentInChildren<Image>().sprite = black_king; enemyUnit.unitName = "Black King"; enemyUnit.damage = 3; enemyUnit.maxHP = 15; enemyUnit.currentHP = 15; enemyUnit.heal = 2; break;
            }
        }

        if (player == "white" && att.Contains("white") && attHP > 0)
            playerUnit.currentHP = attHP;
        else if (player == "white" && def.Contains("white") && defHP > 0)
            playerUnit.currentHP = defHP;
        else if (player == "black" && att.Contains("black") && attHP > 0)
            playerUnit.currentHP = attHP;
        else if (player == "black" && def.Contains("black") && defHP > 0)
            playerUnit.currentHP = defHP;

        if (player == "white" && att.Contains("black") && attHP > 0)
            enemyUnit.currentHP = attHP;
        else if (player == "white" && def.Contains("black") && defHP > 0)
            enemyUnit.currentHP = defHP;
        else if (player == "black" && att.Contains("white") && attHP > 0)
            enemyUnit.currentHP = attHP;
        else if (player == "black" && def.Contains("white") && defHP > 0)
            enemyUnit.currentHP = defHP;

        attackButton.SetActive(false);
        healButton.SetActive(false);

        dialogueText.text = "The fight between " + playerUnit.unitName + " and " + enemyUnit.unitName + " begins!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
    }

    private void ChangeState(BattleState battleState)
    {
        photonView.RPC(nameof(RPC_ChangeState), RpcTarget.OthersBuffered, battleState.ToString());

        state = battleState;

        if (IsItMyTurn())
        {
            PlayerTurn();
        }
        else
        {
            EnemyTurn();
        }
    }

    [PunRPC]
    private void RPC_ChangeState(string battleState)
    {
        System.Enum.TryParse(battleState, out state);
        
        if (IsItMyTurn())
        {
            PlayerTurn();
        }
        else
        {
            EnemyTurn();
        }
    }

    IEnumerator PlayerAttack()
    {
        photonView.RPC(nameof(RPC_EnemyAttack), RpcTarget.OthersBuffered);

        GameObject.Find("Board").GetComponent<AudioScript>().PlayDamage();

        //Damage the enemy
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHP(enemyUnit.currentHP, enemyUnit.maxHP);
        dialogueText.text = "The attack is successful!";

        yield return new WaitForSeconds(2f);

        //Check if enemy is dead
        if (isDead)
        {
            ChangeState(player == "white" ? BattleState.WHITEWON : BattleState.BLACKWON);
            EndBattle();
        }
        else
        {
            ChangeState(state == BattleState.WHITETURN ? BattleState.BLACKTURN : BattleState.WHITETURN);
            EnemyTurn();
        }
    }

    [PunRPC]
    void RPC_EnemyAttack()
    {
        //Damage the player
        playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.currentHP, playerUnit.maxHP);
        dialogueText.text = "The enemy's attack was successful!";
    }

    IEnumerator PlayerHeal()
    {
        photonView.RPC(nameof(RPC_EnemyHeal), RpcTarget.OthersBuffered);

        dialogueText.text = playerUnit.unitName + " is healing...";
        yield return new WaitForSeconds(2f);
        GameObject.Find("Board").GetComponent<AudioScript>().PlayHeal();
        playerUnit.HealSelf();
        playerHUD.SetHP(playerUnit.currentHP, playerUnit.maxHP);
        dialogueText.text = "Healing successful! +" + playerUnit.heal + "HP";
        yield return new WaitForSeconds(1f);

        ChangeState(state == BattleState.WHITETURN ? BattleState.BLACKTURN : BattleState.WHITETURN);
        EnemyTurn();
    }

    [PunRPC]
    IEnumerator RPC_EnemyHeal()
    {
        dialogueText.text = "Enemy is healing...";
        yield return new WaitForSeconds(2f);
        enemyUnit.HealSelf();
        enemyHUD.SetHP(enemyUnit.currentHP, enemyUnit.maxHP);
        dialogueText.text = "Enemy healed successfully!";
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
        attackButton.SetActive(true);
        healButton.SetActive(true);
    }

    void EnemyTurn()
    {
        dialogueText.text = "Waiting for enemy to choose an action...";
        attackButton.SetActive(false);
        healButton.SetActive(false);
    }

    void EndBattle()
    {
        photonView.RPC(nameof(RPC_EndBattle), RpcTarget.OthersBuffered);

        StartCoroutine(CoroutineEndBattle());
    }

    [PunRPC]
    void RPC_EndBattle()
    {
        StartCoroutine(CoroutineEndBattle());
    }

    IEnumerator CoroutineEndBattle()
    {
        bool attackerWon = (attacker.Contains("white") && state == BattleState.WHITEWON) ? true : ((attacker.Contains("black") && state == BattleState.BLACKWON) ? true : false);

        dialogueText.text = (attackerWon ? attacker : defender) + " won the battle!";
        yield return new WaitForSeconds(4f);

        GameObject.Find("Controller").GetComponent<GameScript>().FightOver(attacker, defender, attackerWon, matrixX, matrixY);
        GameObject.Find(attackerWon ? attacker : defender).GetComponent<PieceScript>().hp = attackerWon ? (((attacker.Contains("white") && player == "white") || (attacker.Contains("black") && player == "black")) ? (playerUnit.currentHP) : (enemyUnit.currentHP)) : (((defender.Contains("white") && player == "white") || (defender.Contains("black") && player == "black")) ? (playerUnit.currentHP) : (enemyUnit.currentHP));
    }

    public void OnAttackButton()
    {
        attackButton.SetActive(false);
        healButton.SetActive(false);
        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        attackButton.SetActive(false);
        healButton.SetActive(false);
        StartCoroutine(PlayerHeal());
    }
}
