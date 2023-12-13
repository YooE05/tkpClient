using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBlast : CannonBall
{
   [HideInInspector] public ParticleSystem collisionPrticles;

    [SerializeField] List<Sprite> laserSprites;

    private void Start()
    {
        gameObject.GetComponent <SpriteRenderer>().sprite = laserSprites[UnityEngine.Random.Range(0, laserSprites.Count)];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Untagged")
        {

            #region old code for article laser bag DELETE later
            /*  Debug.Log(collision.tag);

                          if (collision.tag == "Isolation")
                          {
                              if (isLaserBlast)
                              { // Debug.Log(collision.gameObject.GetComponent<ArticleIsolation>().havePlayer);
                                  if (collision.gameObject.GetComponent<ArticleIsolation>().havePlayer)
                                  {
                                      gameObject.GetComponent<Collider2D>().enabled = false;
                                      Invoke("ballDisappear", 0.5f);
                                  }
                              }

                          }
                          else
                          {

                              gameObject.SetActive(false);
                          }*/
            //gameObject.GetComponent<Collider2D>().enabled = false;
            // Invoke("ballDisappear", 0f);
            #endregion

            gameObject.SetActive(false);

        }
    }

   /* IEnumerator DestroyLaserBlast()
    {
        SpriteRenderer blastRenderer = gameObject.GetComponent<SpriteRenderer>();
        Collider2D blastCollider = gameObject.GetComponent<Collider2D>();
        blastRenderer.enabled = false;
        blastCollider.enabled = false;

        //включить партиклы
        //collisionPrticles.Play();

        yield return new WaitForSeconds(0.5f);
       // collisionPrticles.Stop();
        gameObject.SetActive(false);

    }*/

}
