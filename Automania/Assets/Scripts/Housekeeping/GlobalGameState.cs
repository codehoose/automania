using System;
using System.Collections.Generic;

public class GlobalGameState
{
    public int CurrentCollectable = -1;
    public List<int> PlacedParts = new List<int>();

    internal void DropObject()
    {
        if (CurrentCollectable < 0) return;
        PlacedParts.Add(CurrentCollectable);
        CurrentCollectable = -1;
    }
}
