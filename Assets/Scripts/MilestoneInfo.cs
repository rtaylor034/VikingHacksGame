using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilestoneInfo : Thing
{
    //required
    public List<Construction<BuffEffect>> Choices { get; set; } = new();
    public Func<bool> Condition { get; set; }

}
