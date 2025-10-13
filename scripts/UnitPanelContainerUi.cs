using Godot;
using System;

public partial class UnitPanelContainerUi : PanelContainer
{
    private Label unitNameLabel;
    private Label unitCostLabel;
    private Button produceUnitButton;
    private Texture2D teamIcon;
    public UnitData UnitData { get; set; }

    // public override void _Ready()
    // {
    // }

    public void SetupUi(int localTeamId)
    {

        unitNameLabel = GetNode<Label>("%UnitNameLabel");
        unitCostLabel = GetNode<Label>("%UnitCostLabel");
        produceUnitButton = GetNode<Button>("%ProduceUnitButton");

        produceUnitButton.Pressed += OnProduceUnitButtonPressed;

        // Aqui garantimos que UnitData não é nulo, pois SetupUi só deve ser chamado se for válido.
        if (UnitData == null)
        {
            // Se ainda for nulo aqui, algo deu muito errado, mas o _Ready já tratou a parte dos GetNode.
            Log.Error("SetupUi chamado sem UnitData.");
            return;
        }

        // Agora, a lógica de configuração da UI que depende dos dados

        Texture2D teamIcon = UnitData.GetIcon(localTeamId);

        if (teamIcon != null)
        {
            unitNameLabel.Text = UnitData.Name;
            unitCostLabel.Text = $"Cost: {UnitData.Cost}";
            produceUnitButton.Icon = teamIcon;
            produceUnitButton.Disabled = false; // Habilita se tudo estiver ok
        }
        else
        {
            unitNameLabel.Text = $"{UnitData.Name} (ERRO Ícone)";
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
