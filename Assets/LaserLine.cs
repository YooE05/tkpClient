using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLine : MonoBehaviour
{

    [SerializeField] Texture[] textures;

    int animationStep;

    [SerializeField] float fps = 30f;
    float fpsCounter = 0f;

    LineRenderer lineRenderer;
    public bool canBlast;

    IEnumerator blasCoroutine;

    [SerializeField] ParticleSystem laserSparks;

    // Use this for initialization

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    void Start()
    {
        blasCoroutine = Blasting();

        StartBlasting();
    }


    Vector3 lastPosition;
    void Update()
    {
        if (canBlast)
        {
            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1f / fps)
            {
                animationStep++;
                if (animationStep == textures.Length)
                { animationStep = 0; }

                lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);

                fpsCounter = 0f;
            }

        }

    }

    private IEnumerator Blasting()
    {
        while (canBlast)
        {
            lineRenderer.SetPosition(0, transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 100f, LayerMask.GetMask("Water"));
            if (hit.collider)
            {

                if (hit.collider.tag == "Player")
                {
                    GameEvents.current.TakeDamage();
                }
                lastPosition = new Vector3(hit.point.x, hit.point.y, transform.position.z);
                lineRenderer.SetPosition(1, lastPosition);
                laserSparks.gameObject.transform.position = new Vector3(lastPosition.x, lastPosition.y, lastPosition.z); 

            }
            else
            {
                lineRenderer.SetPosition(1, transform.up * 2000);
            }
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void StopBlasting()
    {
        laserSparks.Stop();
        canBlast = false;
        gameObject.SetActive(false);

        StopCoroutine(blasCoroutine);
    }
    public void StartBlasting()
    {
        laserSparks.Play();
        gameObject.SetActive(true);
        canBlast = true;

        blasCoroutine = Blasting();
        StartCoroutine(blasCoroutine);
    }

}
