using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Cannon : NetworkBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float speed = 1f;

    [SerializeField] GameObject cannonballPrefab;
    [SerializeField] ParticleSystem laserSpark;

    private List<GameObject> _cannonBalls = new List<GameObject>();

    [SyncVar] int countOfBalls = 50;
    [SyncVar] int currentBall = 0;

    [SyncVar] public bool isShoot;
    [SyncVar] public bool isDelayEnded;

    private void Awake()
    {
        InitCannonBalls();
    }

    private void Start()
    {
        StartCoroutine("Shoot");
    }

    private void InitCannonBalls()
    {
        for (int i = 0; i < countOfBalls; i++)
        {
            _cannonBalls.Add(Instantiate(cannonballPrefab,
                new Vector3(transform.position.x, transform.position.y, 0.5f),
                Quaternion.identity, transform));
            // NetworkServer.Spawn(cannonBalls[i]);

            _cannonBalls[i].SetActive(false);
            //            RpcSetActivePortals(cannonBalls[i], false);
        }
    }

    [ClientRpc]
    public void StartShooting()
    {
        isShoot = true;
    }

    public void StopShooting()
    {
        isShoot = false;
        StopAllCoroutines();
        foreach (var ball in _cannonBalls)
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
            laserSpark.gameObject.transform.position = ball.transform.position + gameObject.transform.up;
            laserSpark.Play();
        }
    }

    IEnumerator Shoot()
    {
        Vector3 startBallPosition =
            new Vector3(transform.position.x, transform.position.y, 0.5f) + gameObject.transform.up;

        while (true)
        {
            if (isShoot && isDelayEnded)
            {
                isDelayEnded = false;
                GameObject ball = _cannonBalls[currentBall];

                ball.SetActive(true);
                ball.transform.position = startBallPosition;

                ball.GetComponent<Collider2D>().enabled = true;
                ball.GetComponent<SpriteRenderer>().enabled = true;

                StartCoroutine(BallFly(ball));

                currentBall++;
                if (currentBall == countOfBalls)
                {
                    currentBall = 0;
                }

                yield return new WaitForSeconds(delay);
                isDelayEnded = true;
            }

            yield return null;
        }
    }
}