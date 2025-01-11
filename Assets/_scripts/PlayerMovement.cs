using System;
using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] AudioSource wrongSound;

    [SerializeField] float speed;

    [SerializeField] Animator animator;
    [SerializeField] private TextMeshProUGUI _playerDebug;

    public Grid Grid;
    [SerializeField] const int countOfMoveBlock = 2;
    Cell[] nextCell = new Cell[countOfMoveBlock + 1];
    public bool canMove = false;

    private WorldInitializer _world;
    private Camera _mainCam;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _cameraOffset = new Vector3(0f, 0f, -10f);

    [SyncVar(hook = nameof(OnNickNameChanged))]
    private string _ownNickName;

    // Команда для отправки никнейма на сервер
    [Command]
    public void CmdSetNickName(string newNickName)
    {
        _ownNickName = newNickName;
        _playerDebug.text = newNickName;
    }

    private void OnNickNameChanged(string oldNick, string newNick)
    {
        _playerDebug.text = newNick;
    }

    private void OnEnable()
    {
        if (!isLocalPlayer) return;
        StartMoving();
    }

    void OnDisable()
    {
        if (!isLocalPlayer) return;
        StopAllCoroutines();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        var targetPos = transform.position + _cameraOffset;
        _mainCam.transform.localPosition =
            Vector3.SmoothDamp(_mainCam.transform.position, targetPos, ref _velocity, 1.5f);
    }

    void Start()
    {
        _mainCam = Camera.main;

        if (!isLocalPlayer)
        {
            _playerDebug.text = _ownNickName; 
            CmdSetupName(_ownNickName);
            return;
        }

        if (!_world)
        {
            _world = FindObjectOfType<WorldInitializer>();
        }
        
        if (_world.GameIsEnded)
        {
            canMove = false;
            return;
        }

        Grid = _world.GetCurrentGrid(out var gridSize);

        var nick = PlayerPrefs.GetString("Nickname");
        if (nick == String.Empty)
        {
            nick = "Player" + _world.numPlayers;
        }
        CmdSetNickName(nick);
        
        
        StartMoving();

//        var PlayerBaseID = Convert.ToInt32(GetComponent<NetworkIdentity>().netId) -
        //                     GameObject.Find("NetworkManager").GetComponent<NetworkManager>().numPlayers;
        //_playerDebug.text = PlayerBaseID.ToString();

        transform.position = new Vector3(1, 1, 0);
        canMove = true;
    }

    [Command]
    private void CmdSetupName(string nick)
    {
        _playerDebug.text = nick;
    }


    public void StartMoving()
    {
        StartCoroutine(Move2());
    }

    IEnumerator Move2()
    {
        while (true)
        {
            if (!isLocalPlayer)
            {
                yield return new WaitForSeconds(0.00001f);
                continue;
            }

            if (canMove)
            {
                float xOff = Input.GetAxisRaw("Horizontal");

                //animator.SetInteger("HorizDirection", (int)xOff);
                float yOff = Input.GetAxisRaw("Vertical");

                if (xOff < 0)
                {
                    transform.rotation = Quaternion.Euler(0, -180, 0);
                    _playerDebug.rectTransform.localRotation = Quaternion.Euler(0, -180, 0);
                }
                else if (xOff > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    _playerDebug.rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
                }

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
                {
                    yield return new WaitForSeconds(0.00001f);
                }
                else if (!(xOff != 0 && yOff != 0))
                {
                    if (Math.Abs(xOff) > 0)
                    {
                        // animator.SetTrigger("HorizMove");
                        moveCheck((int) xOff, 0, countOfMoveBlock);
                    }

                    if (Math.Abs(yOff) > 0)
                    {
                        moveCheck(0, (int) yOff, countOfMoveBlock);
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
        possibleNextCoordinate = new Vector2(transform.position.x + (index + 1) * xOffset * Grid.cellSize,
            transform.position.y + (index + 1) * yOffset * Grid.cellSize);
        possibleNextCoordinate.x = Mathf.Clamp(possibleNextCoordinate.x, -1, Grid.gridSideX + 1);
        possibleNextCoordinate.y = Mathf.Clamp(possibleNextCoordinate.y, -1, Grid.gridSideY + 1);

        nextCell[index] = Grid.cellsDictionary[possibleNextCoordinate];

        if (nextCell[index].exitDirection != "")
        {
            if (index == 0)
            {
                Vector2 direction;
                switch (nextCell[index].exitDirection)
                {
                    case "up":
                    {
                        direction = new Vector2(0, 1);
                    }
                        break;
                    case "down":
                    {
                        direction = new Vector2(0, -1);
                    }
                        break;
                    case "left":
                    {
                        direction = new Vector2(-1, 0);
                    }
                        break;
                    case "right":
                    {
                        direction = new Vector2(1, 0);
                    }
                        break;
                    default:
                        direction = new Vector2(0, 0);
                        break;
                }

                GameEvents.current.ExitTriggerEnter(direction);
            }
        }
        else if (nextCell[index].currentObject && nextCell[index].currentObject.tag != "trap")
        {
            if (nextCell[index].currentObject.tag == "article")
            {
                if (index < lastCheckCell)
                {
                    moveCheck(xOffset, yOffset, lastCheckCell, ++index);
                }
            }
        }
        else
        {
            if (!Grid.CellIsBorder(nextCell[index]) || index == 0)
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
                var pos = Vector3.Lerp(startPoint, endPoint, crntTime / translateTime);
                //  movedObject.transform.position = Vector3.Lerp(startPoint, endPoint, crntTime / translateTime);
                CmdSendPositionToServer(movedObject, pos);
                crntTime += Time.deltaTime * speed;
                yield return null;
            }
            else break;
        }

        if (canMove)
        {
            // movedObject.transform.position = endPoint;
            CmdSendPositionToServer(movedObject, endPoint);
        }
        else
        {
            if (movedObject.tag == "Player")
            {
                movedObject.transform.position = new Vector2(1, 1);
            }
        }
    }

    [Command]
    void CmdSendPositionToServer(GameObject go, Vector3 position)
    {
        RpcUpdatePosition(go, position);
    }

    [ClientRpc]
    void RpcUpdatePosition(GameObject go, Vector3 position)
    {
        go.transform.position = position;
    }
}