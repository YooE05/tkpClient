using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using System;

public class RoundData
{
    //edit. Change type and rename
    private object data;

    public RoundData()
    {
    }

    public override string ToString()
    {
        return $"RoundData : [data - {data}]";
    }
}
