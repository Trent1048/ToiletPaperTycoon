using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorNode
{
    public GameObject storedObject;
    public ConveyorNode next;

    public ConveyorNode()
    {
        storedObject = null;
        next = null;
    }

    public ConveyorNode(ConveyorNode next) : this()
    {
        this.next = next;
    }
}

