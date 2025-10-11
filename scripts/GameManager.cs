using Godot;
using System.Collections.Generic;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    public List<Unit> AllUnits { get; private set; } = new List<Unit>();
    public Building SelectedBuilding { get; set; } = null;

    public override void _Ready()
    {
        if (Instance != null)
        {
            // Log.Error("GameManager já existe. O novo nó será removido.");
            QueueFree();
            return;
        }

        Instance = this;
        // Log.Warn("GameManager inicializado.");
    }

    public void AddUnit(Unit unit)
    {
        if (!AllUnits.Contains(unit))
        {
            AllUnits.Add(unit);
            // Log.Info($"Unidade {unit} adicionada. Total de unidades: {AllUnits.Count}");
        }
        else
        {
            // Log.Warn($"Unidade {unit} já está na lista.");
        }
    }

    public void RemoveUnit(Unit unit)
    {
        if (AllUnits.Contains(unit))
        {
            AllUnits.Remove(unit);
            // unit.SetSelected(false);
            // Log.Info($"Unidade {unit} removida. Total de unidades: {AllUnits.Count}");
        }
        else
        {
            // Log.Warn($"Unidade {unit} não encontrada na lista.");
        }
    }

    public void RemoveAllUnits()
    {
        UnselectAll();
        AllUnits.Clear();
        // Log.Info("Todas as unidades foram removidas.");
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
            // Log.Info($"Building {building} selected.");
        }
        else
        {
            SelectedBuilding = null;
            // Log.Info("No building selected.");
        }
    }
    public void OnUnitDied(Unit unit)
    {
        RemoveUnit(unit);
    }
}