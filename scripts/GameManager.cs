using Godot;
using System.Collections.Generic;
using System.Numerics;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    public List<Unit> AllUnits { get; private set; } = new List<Unit>();
    public Building SelectedBuilding { get; set; } = null;
    public int LocalPlayerTeamId { get; set; } = 1;
    public PackedScene buildingScene = GD.Load<PackedScene>("uid://757kle2ca63q");

    public override void _Ready()
    {
        if (Instance != null)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public void AddUnit(Unit unit)
    {
        if (!AllUnits.Contains(unit))
        {
            AllUnits.Add(unit);
        }
    }

    public void RemoveUnit(Unit unit)
    {
        if (AllUnits.Contains(unit))
        {
            AllUnits.Remove(unit);
        }
    }

    public void RemoveAllUnits()
    {
        UnselectAll();
        AllUnits.Clear();
    }

    public void UnselectAll()
    {
        foreach (var unit in AllUnits)
        {
            if (IsInstanceValid(unit))
                unit.SetSelected(false);
        }

        if (SelectedBuilding != null && IsInstanceValid(SelectedBuilding))
        {
            SelectedBuilding.SetSelected(false);
            SelectedBuilding = null;
        }

    }
    public void SelectBuilding(Building building)
    {
        if (SelectedBuilding != null && IsInstanceValid(SelectedBuilding))
        {
            SelectedBuilding.SetSelected(false);
        }

        SelectedBuilding = building;

        if (SelectedBuilding != null && IsInstanceValid(SelectedBuilding))
        {
            SelectedBuilding.SetSelected(true);
        }
        else
        {
            SelectedBuilding = null;
        }
    }

    public Building GetGhostBuilding()
    {
        Building ghostBuilding = buildingScene.Instantiate<Building>();
        // ghostBuilding.GlobalPosition = placementPosition;
        // AddChild(ghostBuilding);
        return ghostBuilding;
    }
    public void OnUnitDied(Unit unit)
    {
        RemoveUnit(unit);
    }
}