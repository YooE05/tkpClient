using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LaserInteraction : NetworkBehaviour
{
    ViewController viewController;
    [SyncVar] public bool canDisableLaser = false;
    [SyncVar] private bool laserIsOn;
    [SerializeField] int laserDelay;

    [SerializeField] LaserLine ownLaserLine;
    // [SerializeField] Cannon parentCannon;

    [SerializeField] int maxCountOfDisabling = 2;
    [SyncVar] private int countOfDisabling = 0;

    [SerializeField] GameObject EnabledSpriteObj;
    [SerializeField] GameObject DisabledSpriteObj;

    private void Awake()
    {
        InitLaser();

        viewController = FindObjectOfType<ViewController>();
    }

    private void InitLaser()
    {
        countOfDisabling = 0;
        canDisableLaser = false;
        laserIsOn = true;
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canDisableLaser = true;
        }

        CheckLaserInteraction();
    }

    private void CheckLaserInteraction()
    {
        if (laserIsOn && canDisableLaser && countOfDisabling < maxCountOfDisabling)
        {
            StartCoroutine("WaitForDisableLaser");
            viewController.ChangeLaserDisableText(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canDisableLaser = false;
            viewController.ChangeLaserDisableText(false);
        }
    }

    IEnumerator WaitForDisableLaser()
    {
        while (laserIsOn && canDisableLaser)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Disable();
            }

            yield return null;
        }
    }
*/
    public void Disable()
    {
        EnabledSpriteObj.SetActive(false);
        DisabledSpriteObj.SetActive(true);
        countOfDisabling++;
        laserIsOn = false;
        ownLaserLine.StopBlasting();
        // parentCannon.StopShooting();
        viewController.ChangeLaserDisableText(false);
        StartCoroutine(nameof(StartLaserTimer));
    }

    private IEnumerator StartLaserTimer()
    {
        ParticleSystem laserSparks = DisabledSpriteObj.transform.Find("FastSparks").GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(laserDelay - 2f);
        for (float i = 0; i < 2f; i += 0.5f)
        {
            laserSparks.Play();
            //parentCannon.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.5f);
        }

        EnabledSpriteObj.SetActive(true);
        DisabledSpriteObj.SetActive(false);

        laserIsOn = true;
        // CheckLaserInteraction();
        ownLaserLine.StartBlasting();
    }

    public void ResetLaser()
    {
        laserIsOn = true;
        EnabledSpriteObj.SetActive(true);
        DisabledSpriteObj.SetActive(false);
        // CheckLaserInteraction();
    }

    public bool CanBeTurnedOff()
    {
        return laserIsOn && canDisableLaser;
    }
}