using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilestoneInfo
{
    //required
    public Construction<BuffEffect>[] Choices { get; set; }
    public Func<bool> Condition { get; set; }

}
