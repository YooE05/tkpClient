using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticleIsolation : MonoBehaviour
{
    public bool havePlayer;

    private void Awake()
    {
        havePlayer = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            havePlayer = true;
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            havePlayer = false;
        }
    }
}
