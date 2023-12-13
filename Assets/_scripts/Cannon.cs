using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float speed = 1f;

    [SerializeField] GameObject cannonballPrefab;
    [SerializeField] ParticleSystem laserSpark;

    List<GameObject> cannonBalls = new List<GameObject>();

    int countOfBalls = 50;
    int currentBall = 0;

    public bool isShoot;

    private void Awake()
    {
        for (int i = 0; i < countOfBalls; i++)
        {
            cannonBalls.Add(Instantiate(cannonballPrefab, new Vector3(transform.position.x, transform.position.y, 0.5f), Quaternion.identity, transform));
            cannonBalls[i].SetActive(false);
        }

        isShoot = true;
    }
    void Start()
    {

        StartCoroutine("Shoot");
    }

    public void StartShooting()
    {
        isShoot = true;
        StartCoroutine("Shoot");
    }
    public void StopShooting()
    {
        isShoot = false;
        StopAllCoroutines();
        foreach (var ball in cannonBalls)
        {
            ball.SetActive(false);
        }
    }

    IEnumerator BallFly(GameObject ball)
    {

        while (ball.activeSelf)
        {
            ball.transform.Translate((gameObject.transform.up * speed) * Time.deltaTime);
            yield return null;
        }
        if (laserSpark)
        {
            laserSpark.gameObject.transform.position = ball.transform.position+ gameObject.transform.up;
            laserSpark.Play();
        }

    }
    IEnumerator Shoot()
    {
        Vector3 startBallPosition = new Vector3(transform.position.x, transform.position.y, 0.5f) + gameObject.transform.up;
        
        while (true)
        {
            GameObject ball = cannonBalls[currentBall];

            ball.SetActive(true);
            ball.transform.position = startBallPosition;

            ball.GetComponent<Collider2D>().enabled = true;
            ball.GetComponent<SpriteRenderer>().enabled = true;

            StartCoroutine(BallFly(ball));

            currentBall++;
            if (currentBall == countOfBalls)
            { currentBall = 0; }

            yield return new WaitForSeconds(delay);
        }


    }

}
