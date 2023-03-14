using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    int matrixX;
    int matrixY;

    public bool attack = false;

    public void Start()
    {
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 1f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        if (attack)
        {
            GameObject cp = controller.GetComponent<GameScript>().GetPosition(matrixX, matrixY);

            //Destroy(reference) or Destroy(cp) depending on who wins the fight

            //initialize fight
            controller.GetComponent<GameScript>().StartFight(reference.name, cp.name);

            //controller.GetComponent<GameScript>().DestroyPiece(matrixX, matrixY);
        }
        controller.GetComponent<GameScript>().SetPositionEmpty(reference.GetComponent<PieceScript>().GetXBoard(), reference.GetComponent<PieceScript>().GetYBoard());

        controller.GetComponent<GameScript>().SetReferencePiecePosition(reference.name, matrixX, matrixY);

        controller.GetComponent<GameScript>().SetPosition(reference.name);

        controller.GetComponent<GameScript>().NextTurn();

        reference.GetComponent<PieceScript>().DestroyMovePlates();
    }

    public void SetCoordinates(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}
