using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public int width;
    public int height;
    public List<tile> tiles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    List<tile> accessableTilesFromIndex(Vector2 pos)
    {
        List<tile> til = new List<tile>();
        int index = convertToTileIndex(pos);
        if (!tiles[index].edge)
        {
            if(tiles[index + 1].isAccesible)
            {
                til.Add(tiles[index + 1]);
            }
            if (tiles[index - 1].isAccesible)
            {
                til.Add(tiles[index - 1]);
            }
            if (tiles[index + width].isAccesible)
            {
                til.Add(tiles[index + width]);
            }
            if (tiles[index - width].isAccesible)
            {
                til.Add(tiles[index - width]);
            }

        }
        return til;
    }
    int convertToTileIndex(Vector2 pos)
    {
        Vector2 roundedPos = new Vector2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        return (int)roundedPos.x * (int)roundedPos.y;
    }
}
