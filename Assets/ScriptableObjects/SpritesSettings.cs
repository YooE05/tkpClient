using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpritesSettings", menuName = "ScriptableObjects/SpritesSettings")]
public class SpritesSettings : ScriptableObject
{
    public List<Sprite> floorCells;
    public List<Sprite> horizontalUpWalls;
    public List<Sprite> horizontalDownWalls;
    public List<Sprite> verticalLeftWalls;
    public List<Sprite> verticalRightWalls;

    public List<Sprite> leftUpCorner;
    public List<Sprite> rightUpCorner;
    public List<Sprite> leftDownCorner;
    public List<Sprite> rightDownCorner;
}
