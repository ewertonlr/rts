using Godot;
using System;

public partial class UnitPanelContainerUi : PanelContainer
{
    private Label unitNameLabel;
    private Label unitCostLabel;
    private Button produceUnitButton;
    private Texture2D teamIcon;
    public UnitData UnitData { get; set; }

    public void SetupUi(int localTeamId)
    {

        unitNameLabel = GetNode<Label>("%UnitNameLabel");
        unitCostLabel = GetNode<Label>("%UnitCostLabel");
        produceUnitButton = GetNode<Button>("%ProduceUnitButton");

        produceUnitButton.Pressed += OnProduceUnitButtonPressed;

        if (UnitData == null)
        {
            Log.Error("SetupUi chamado sem UnitData.");
            return;
        }

        unitNameLabel.Text = UnitData.Name;
        unitCostLabel.Text = $"Cost: {UnitData.Cost}";
        produceUnitButton.Icon = UnitData.DefaultIcon;
        produceUnitButton.Disabled = false;
    }

    public void OnProduceUnitButtonPressed()
    {
        var selectedBuilding = GameManager.Instance.SelectedBuilding;
        if (selectedBuilding != null && UnitData != null)
        {
            selectedBuilding.AddToProductionQueue(UnitData);
            Log.Info($"Requested production of unit from building: {selectedBuilding}");
        }
    }
}
