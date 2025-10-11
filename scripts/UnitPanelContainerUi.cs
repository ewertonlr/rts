using Godot;
using System;

public partial class UnitPanelContainerUi : PanelContainer
{
    public PackedScene UnitScene { get; set; }

    private Label unitNameLabel;
    private Label unitCostLabel;
    private Button produceUnitButton;

    public override void _Ready()
    {
        unitNameLabel = GetNode<Label>("VBoxContainer/UnitNameLabel");
        unitCostLabel = GetNode<Label>("VBoxContainer/UnitCostLabel");
        produceUnitButton = GetNode<Button>("VBoxContainer/ProduceUnitButton");

        produceUnitButton.Pressed += OnProduceUnitButtonPressed;

        if (UnitScene != null)
        {
            Unit newUnit = UnitScene.Instantiate<Unit>();
            unitNameLabel.Text = newUnit.Name;
            unitCostLabel.Text = "Cost: 50"; // Placeholder cost, replace with actual cost if available
        }
        else
        {
            unitNameLabel.Text = "No Unit";
            unitCostLabel.Text = "Cost: N/A";
            produceUnitButton.Disabled = true;
        }
    }

    public void OnProduceUnitButtonPressed()
    {
        var selectedBuilding = GameManager.Instance.SelectedBuilding;
        if (selectedBuilding != null && UnitScene != null)
        {
            selectedBuilding.AddToProductionQueue(UnitScene);
            // Log.Info($"Requested production of unit from building: {selectedBuilding}");
        }
        else
        {
            // Log.Warn("No building selected or unit scene is null.");
        }
    }
}
