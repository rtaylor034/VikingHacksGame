using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Upgrade
{
    public Action OnBuyAction { get; private set; }
    public int Count { get; private set; }
    public List<Effect> Effects { get; }

}
