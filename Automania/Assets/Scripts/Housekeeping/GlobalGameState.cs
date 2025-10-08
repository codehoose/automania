using System.Collections.Generic;

public class GlobalGameState
{
    private static int PICK_UP_PART_POINTS = 100;
    private static int STARTING_LIVES = 3;

    public int CurrentCollectable = -1;
    public List<int> PlacedParts = new List<int>();
    public GameState State = GameState.NormalGameplay;

    public bool AllPartsCollected => PlacedParts.Count == 6;

    public int Score { get; private set; }

    public int Lives { get; set; } = STARTING_LIVES;

    internal void PickupObject() => Score += PICK_UP_PART_POINTS;

    internal void AddTimePoints(float time)
    {
        var points = (int)(time * 100);
        Score += points;
    }

    internal void DropObject()
    {
        if (CurrentCollectable < 0) return;
        PlacedParts.Add(CurrentCollectable);
        GameController.Instance.HoistCar?.AcceptPart(CurrentCollectable);
        CurrentCollectable = -1;
        Score += PICK_UP_PART_POINTS;
    }
}
