using Mirror;
using UnityEngine;

public class Room : NetworkBehaviour
{
    public Vector2 gridCoordinate;
    public string phraseDirections;
    public Grid GridComponent;

    public GameObject phrasesContainer;
    public GameObject articlesContainer;

  //  public Grid GridComponent => gridGO.GetComponent<Grid>();

    /*[Server]
    private void Awake()
    {
        gridGO = Instantiate(gridGO, transform);
        NetworkServer.Spawn(gridGO);
    }*/
}