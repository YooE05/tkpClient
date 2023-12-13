using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInteraction : MonoBehaviour
{
    ViewController viewController;
    bool canDisableLaser = false;
    bool laserIsOn;
    [SerializeField] int laserDelay;
    [SerializeField] LaserLine ownLaserLine;
   // [SerializeField] Cannon parentCannon;

    [SerializeField] int maxCountOfDisabling = 2;
    int countOfDisabling = 0;

    [SerializeField] GameObject EnabledSpriteObj;
    [SerializeField] GameObject DisabledSpriteObj;



    private void Awake()
    {
        countOfDisabling = 0;
        canDisableLaser = false;
        laserIsOn = true;
        viewController = FindObjectOfType<ViewController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
        while (laserIsOn&& canDisableLaser)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EnabledSpriteObj.SetActive(false);
                DisabledSpriteObj.SetActive(true);
                countOfDisabling++;
                laserIsOn = false;
                ownLaserLine.StopBlasting();
               // parentCannon.StopShooting();
                viewController.ChangeLaserDisableText(false);
                StartCoroutine("LaserTimer");
            }
            yield return null;
        }
    }

    IEnumerator LaserTimer()
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
        CheckLaserInteraction();
        ownLaserLine.StartBlasting();
        // parentCannon.StartShooting();
    }

    public void ResetLaser()
    {
        laserIsOn = true;
        EnabledSpriteObj.SetActive(true);
        DisabledSpriteObj.SetActive(false);
       // parentCannon.GetComponent<SpriteRenderer>().color = Color.white;
        CheckLaserInteraction();
    }


}
