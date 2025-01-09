using Mirror;
using UnityEngine;

public class SpritesSettingsNet : NetworkBehaviour
{
    public SyncList<Sprite> floorCells = new SyncList<Sprite>();
    public SyncList<Sprite> horizontalUpWalls = new SyncList<Sprite>();
    public SyncList<Sprite> horizontalDownWalls = new SyncList<Sprite>();
    public SyncList<Sprite> verticalLeftWalls = new SyncList<Sprite>();
    public SyncList<Sprite> verticalRightWalls = new SyncList<Sprite>();

    public SyncList<Sprite> leftUpCorner = new SyncList<Sprite>();
    public SyncList<Sprite> rightUpCorner = new SyncList<Sprite>();
    public SyncList<Sprite> leftDownCorner = new SyncList<Sprite>();
    public SyncList<Sprite> rightDownCorner = new SyncList<Sprite>();

    public void Construct(SpritesSettings settings)
    {
        foreach (var sprite in settings.floorCells)
        {
            floorCells.Add(sprite);
        }

        foreach (var sprite in settings.horizontalUpWalls)
        {
            horizontalUpWalls.Add(sprite);
        }

        foreach (var sprite in horizontalDownWalls)
        {
            horizontalDownWalls.Add(sprite);
        }

        foreach (var sprite in verticalLeftWalls)
        {
            verticalLeftWalls.Add(sprite);
        }

        foreach (var sprite in verticalRightWalls)
        {
            verticalRightWalls.Add(sprite);
        }

        foreach (var sprite in rightUpCorner)
        {
            rightUpCorner.Add(sprite);
        }

        foreach (var sprite in leftUpCorner)
        {
            leftUpCorner.Add(sprite);
        }

        foreach (var sprite in leftDownCorner)
        {
            leftDownCorner.Add(sprite);
        }

        foreach (var sprite in rightDownCorner)
        {
            rightDownCorner.Add(sprite);
        }
    }
}