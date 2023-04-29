using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilestoneInfo
{
    //required
    public string ID { get; set; }
    public List<Construction<BuffEffect>> Choices { get; set; } = new();
    public Func<bool> Condition { get; set; }

}
