using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    //UDLR by index
    public Sprite[] gunSprites = new Sprite[4];
    public Sprite[] axeSprites = new Sprite[4];

    public void UpdateSprite(bool holdingGun, Direction dir)
    {
        int spriteIndex = 0;

        switch (dir)
        {
            case Direction.UP:
                spriteIndex = 0;
                break;
            case Direction.LEFT:
                spriteIndex = 2;
                break;
            case Direction.RIGHT:
                spriteIndex = 3;
                break;
            case Direction.DOWN:
                spriteIndex = 1;
                break;
        }

        if(holdingGun)
        {
            GetComponent<SpriteRenderer>().sprite = gunSprites[spriteIndex];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = axeSprites[spriteIndex];
        }
    }
}
