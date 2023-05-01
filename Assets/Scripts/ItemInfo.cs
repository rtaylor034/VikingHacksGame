using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : Thing
{
    [Flags]
    public enum ECategory
    {
        Sustainable = 1,
        Nonsustainable = 2,
        OtherThing = 4
    }

    //required
    public Sprite Sprite { get; set; }
    public ECategory Categories { get; set; }

    private Func<ItemInfo, float> _priceFunction;
    public Func<ItemInfo, float> PriceFunction { get => _priceFunction; set { _priceFunction = value; Price = _priceFunction(this); } }
    

    //defaulted
    public Modifier SustianMod { get; set; } = (v => (v, false));
    public Modifier IdleMod { get; set; } = (v => (v, false));
    public Modifier ClickMod { get; set; } = (v => (v, false));

    private Func<ItemInfo, float> _sustainEffectFunction = _ => 0;
    public Func<ItemInfo, float> SustainEffectFunction { get => _sustainEffectFunction; set { _sustainEffectFunction = value; SustainEffect = _sustainEffectFunction(this); } }
    public Func<ItemInfo, bool> AdditionalBuyConditions { get; set; } = _ => true;

    //non set
    public int AmountOwned { get; private set; } = 0;
    public float Price { get; private set; }
    public float SustainEffect { get; private set; } = 0;

    public void Buy()
    {
        GameManager.GAME.Cash -= Price;
        GameManager.GAME.Sustain += SustainEffect;
        AmountOwned++;
        GameManager.GAME.AddClickMod(ClickMod);
        GameManager.GAME.AddIdleMod(IdleMod);
        GameManager.GAME.AddSustainMod(SustianMod);
        Price = PriceFunction(this);
        SustainEffect = SustainEffectFunction(this);
        
    }
}
