using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ECategory
    {

    }
    //required
    public string ID { get; set; }
    public string DisplayName { get; set; }
    public ECategory Category { get; set; }
    public Func<Item, float> PriceFunction { get; set; }
    

    //defaulted
    public (Modifier mod, bool f) SustianMod { get; set; } = (v => v, false);
    public (Modifier mod, bool f) IdleMod { get; set; } = (v => v, false);
    public (Modifier mod, bool f) ClickMod { get; set; } = (v => v, false);
    public Func<Item, float> SustainEffectFunction { get; set; } = _ => 0;
    public Func<Item, bool> AdditionalBuyConditions { get; set; } = _ => true;

    //non set
    public int AmountOwned { get; private set; }
    public float Price { get; private set; }
    public float SustainEffect { get; private set; }

    public void Buy()
    {
        GameManager.GAME.Cash -= Price;
        GameManager.GAME.Sustian += SustainEffect;
        AmountOwned++;
        GameManager.GAME.AddClickMod(ClickMod.mod, ClickMod.f);
        GameManager.GAME.AddIdleMod(IdleMod.mod, IdleMod.f);
        GameManager.GAME.AddSustainMod(SustianMod.mod, SustianMod.f);
        Price = PriceFunction(this);
        SustainEffect = SustainEffectFunction(this);
        
    }
}
