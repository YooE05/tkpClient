using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WorldInitializer : NetworkBehaviour
{
    [SerializeField] private GameObject _roomPrefab;
    [SerializeField] private List<SpritesSettings> _spriteSettings;

    private Room _room;
    // [SerializeField] 
    //private GridClone _currentGrid;

    private void Start()
    {
        if (isServer)
        {
            RCPRoom();
        }
        else
        {
            _room = FindObjectOfType<Room>();
            _room.GridComponent.SetupSprites(_spriteSettings);
        }
    }

    [Server]
    private void RCPRoom()
    {
        var roomGO = Instantiate(_roomPrefab, transform);
        _room = roomGO.GetComponent<Room>();
        // _room = roomGO.GetComponent<Room>();
        RpcSetupGrid();
        NetworkServer.Spawn(_room.gameObject);
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.I) && isServer)
        {
            RpcSetupGrid();
        }*/
    }

    private void RpcSetupGrid()
    {
        _room.GridComponent.GenerateGrid(false, false, false, false, _spriteSettings);
        //_currentGrid.Construct(_room.GridComponent);
    }

    public Grid GetCurrentGrid(out string hasGrid)
    {
        /*if (isServer)
        {
            RpcSetupGrid();
        }*/

        hasGrid = _room.GridComponent.cellsDictionary.Count.ToString();
        return _room.GridComponent;
    }
}