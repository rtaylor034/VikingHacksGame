using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public void ShopClick(int index)
    {
        GameManager.GAME.ShopTryBuy(index);
    }
}
