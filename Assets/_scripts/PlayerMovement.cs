using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] AudioSource wrongSound;

    [SerializeField] float speed;

    [SerializeField] Animator animator;

    public Grid grid;
    [SerializeField] const int countOfMoveBlock = 2;
    Cell[] nextCell = new Cell[countOfMoveBlock + 1];
    public bool canMove = false;

    private void Start()
    {

    }


    void OnDisable()
    {
        StopAllCoroutines();
    }
    void OnEnable()
    {
        StartCoroutine(Move2());
    }

    IEnumerator Move2()
    {
        while (true)
        {
            if (canMove)
            {
                float xOff = Input.GetAxisRaw("Horizontal");

                //animator.SetInteger("HorizDirection", (int)xOff);
                float yOff = Input.GetAxisRaw("Vertical");

                if (xOff < 0)
                {
                    transform.rotation = Quaternion.Euler(0, -180, 0);
                }
                else if (xOff > 0)
                { transform.rotation = Quaternion.Euler(0, 0, 0); }
                animator.SetFloat("HorizDirection", Mathf.Abs(xOff));


                if (yOff < 0)
                {
                    animator.SetFloat("Up", 0);
                    animator.SetFloat("Down", Mathf.Abs(yOff));
                }
                else if (yOff > 0)
                {
                    animator.SetFloat("Down", 0);
                    animator.SetFloat("Up", Mathf.Abs(yOff));
                }
                else
                {
                    animator.SetFloat("Down", 0);
                    animator.SetFloat("Up", 0);
                }


                if (xOff == 0 && yOff == 0)
                { yield return new WaitForSeconds(0.00001f); }
                else
                if (!(xOff != 0 && yOff != 0))
                {
                    if (Math.Abs(xOff) > 0)
                    {
                        // animator.SetTrigger("HorizMove");
                        moveCheck((int)xOff, 0, countOfMoveBlock);
                    }
                    if (Math.Abs(yOff) > 0)
                    {
                        moveCheck(0, (int)yOff, countOfMoveBlock);
                    }
                    yield return new WaitForSeconds(1f / speed);
                }

            }
            yield return new WaitForSeconds(0.00001f);
        }
    }

    Vector2 possibleNextCoordinate;
    void moveCheck(int xOffset, int yOffset, int lastCheckCell, int index = 0)
    {
        possibleNextCoordinate = new Vector2(transform.position.x + (index + 1) * xOffset * grid.cellSize, transform.position.y + (index + 1) * yOffset * grid.cellSize);
        possibleNextCoordinate.x = Mathf.Clamp(possibleNextCoordinate.x, -1, grid.gridSideX + 1);
        possibleNextCoordinate.y = Mathf.Clamp(possibleNextCoordinate.y, -1, grid.gridSideY + 1);

        nextCell[index] = grid.cellsDictionary[possibleNextCoordinate];

        if (nextCell[index].exitDirection != "")
        {
            if (index == 0)
            {
                Vector2 direction;
                switch (nextCell[index].exitDirection)
                {
                    case "up":
                        { direction = new Vector2(0, 1); }
                        break;
                    case "down":
                        { direction = new Vector2(0, -1); }
                        break;
                    case "left":
                        { direction = new Vector2(-1, 0); }
                        break;
                    case "right":
                        { direction = new Vector2(1, 0); }
                        break;
                    default:
                        direction = new Vector2(0, 0);
                        break;
                }

                GameEvents.current.ExitTriggerEnter(direction);
            }
        }
        else
        if (nextCell[index].currentObject && nextCell[index].currentObject.tag != "trap")
        {

            if (nextCell[index].currentObject.tag == "article")
            {
                if (index < lastCheckCell)
                { moveCheck(xOffset, yOffset, lastCheckCell, ++index); }
            }

        }
        else
        {
            if (!grid.CellIsBorder(nextCell[index]) || index == 0)
            {
                // animator.SetInteger("HorizDirection", xOffset);
                // animator.SetInteger("VerticalDirection", yOffset);
                if (xOffset > 0)
                {
                    //   animator.SetInteger("HorizDirection",1);
                }
                if (xOffset < 0)
                {
                    //     animator.SetInteger("HorizDirection", -1);
                }
                StartCoroutine(TranslatePlayer(gameObject, new Vector3(xOffset, yOffset, 0), 0.1f));
                for (int i = 0; i < index; i++)
                {
                    StartCoroutine(TranslatePlayer(nextCell[i].currentObject, new Vector3(xOffset, yOffset, 0), 0.1f));

                }
            }
            else
            {
                wrongSound.Play();
            }

        }

    }

    IEnumerator TranslatePlayer(GameObject movedObject, Vector3 offset, float translateTime)
    {

        float crntTime = 0f;
        Vector3 startPoint = movedObject.transform.position;
        Vector3 endPoint = movedObject.transform.position + offset;
        while (crntTime < translateTime)
        {
            if (canMove)
            {
                movedObject.transform.position = Vector3.Lerp(startPoint, endPoint, crntTime / translateTime);
                crntTime += Time.deltaTime * speed;
                yield return null;
            }
            else break;
        }
        if (canMove)
        { movedObject.transform.position = endPoint; }
        else
        {
            if (movedObject.tag == "Player")
            {
                movedObject.transform.position = new Vector2(1, 1);
            }
        }


    }


    /*//old code
     * void FixedUpdate()
    {
        Move();   
    }

    private void Move()
    {
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
    }*/

}