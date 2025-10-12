using Godot;
using System;

public partial class UnitPanelContainerUi : PanelContainer
{
    private Label unitNameLabel;
    private Label unitCostLabel;
    private Button produceUnitButton;
    public UnitData UnitData { get; set; }

    public override void _Ready()
    {
        unitNameLabel = GetNode<Label>("VBoxContainer/UnitNameLabel");
        unitCostLabel = GetNode<Label>("VBoxContainer/UnitCostLabel");
        produceUnitButton = GetNode<Button>("VBoxContainer/ProduceUnitButton");

        produceUnitButton.Pressed += OnProduceUnitButtonPressed;

        if (UnitData != null)
        {
            unitNameLabel.Text = UnitData.Name;
            unitCostLabel.Text = $"Cost: {UnitData.Cost}"; // Placeholder cost, replace with actual cost if available
            produceUnitButton.Icon = UnitData.Icon;
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
        Log.Info($"Produce button pressed for unit: {UnitData?.Name}");

        var selectedBuilding = GameManager.Instance.SelectedBuilding;

        Log.Info($"Selected Building: {selectedBuilding != null}");
        Log.Info($"Selected Building: {UnitData != null}");

        if (selectedBuilding != null && UnitData != null)
        {
            selectedBuilding.AddToProductionQueue(UnitData.UnitScene);
            Log.Info($"Requested production of unit from building: {selectedBuilding}");
        }
        else
        {
            // Log.Warn("No building selected or unit scene is null.");
        }
    }
}
