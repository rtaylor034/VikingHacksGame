using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBuff
{
    public Func<bool> Condition { get; set; }
    public Construction<BuffEffect> Effect { get; set; }
}
