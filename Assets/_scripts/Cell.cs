using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameObject currentObject;

    //bool isExit=false;
    public string exitDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Isolation")
        {
            currentObject = collision.gameObject;

            if (collision.tag == "Player")
            {
                currentObject = null;
            }
        }


    }



    public Vector2Int GetGridPos()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y));
    }


}
