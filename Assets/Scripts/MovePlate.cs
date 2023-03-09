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

            Destroy(cp);
        }

        controller.GetComponent<GameScript>().SetPositionEmpty(reference.GetComponent<PieceScript>().GetXBoard(), reference.GetComponent<PieceScript>().GetYBoard());

        reference.GetComponent<PieceScript>().SetXBoard(matrixX);
        reference.GetComponent<PieceScript>().SetYBoard(matrixY);
        reference.GetComponent<PieceScript>().SetCoordinates();

        controller.GetComponent<GameScript>().SetPosition(reference);

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
