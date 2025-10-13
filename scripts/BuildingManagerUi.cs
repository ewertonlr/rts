using Godot;
using System;

public partial class BuildingManagerUi : Control
{
    private PackedScene unitPanelContainerUiScene = GD.Load<PackedScene>("uid://c1n7gt2l6n8qa");
    private GridContainer gridContainer;

    public override void _Ready()
    {
        gridContainer = GetNode<GridContainer>("GridContainer");

        this.Visible = false;
        Log.Info("BuildingManagerUi is Ready.");
    }

    public void UpdateUnitPanels(Building building)
    {

        // Clear existing panels
        foreach (var child in gridContainer.GetChildren())
        {
            child.QueueFree();
        }

        int localTeamId = GameManager.Instance?.LocalPlayerTeamId ?? 1; // Obtenha o TeamID com segurança

        // Create new panels based on the building's available units
        foreach (var unitData in building.AvailableUnits)
        {
            UnitPanelContainerUi unitUnitPanelContainerUi = unitPanelContainerUiScene.Instantiate<UnitPanelContainerUi>();
            unitUnitPanelContainerUi.UnitData = unitData;
            unitUnitPanelContainerUi.SetupUi(localTeamId);
            gridContainer.AddChild(unitUnitPanelContainerUi);

            // Log.Info($"BuildingManager: Added unit panel for Unit Data: {unitData}");
        }
    }
    public void OnBuildingSelected(Building selectedBuilding)
    {
        if (IsInstanceValid(selectedBuilding))
        {
            UpdateUnitPanels(selectedBuilding);
            this.Visible = true;
            Log.Info($"BuildingManager: Building selected: {selectedBuilding}");

        }
        // Here you can update the UI or perform other actions based on the selected building
    }

    public void OnBuildingUnselected()
    {
        this.Visible = false;
        Log.Info("BuildingManager: Building unselected.");
    }
}
