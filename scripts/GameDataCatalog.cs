// GameDataCatalog.cs
using Godot;
using System.Collections.Generic;

public partial class GameDataCatalog : Node
{
    public static GameDataCatalog Instance { get; private set; }

    // Dicionário principal: ID da Unidade -> Dados da Unidade
    public Dictionary<int, UnitData> UnitDataByID { get; private set; } = new Dictionary<int, UnitData>();

    // Exemplo de Paths para Scenes (ajuste para suas UIDs)
    private const string SoldierScenePath = "uid://b42ek7rmo23nq"; // A UID da sua Unit.tscn
    private const string SoldierIconPath = "uid://dhuy3j2atec8i"; // Exemplo de Path

    public override void _Ready()
    {
        if (Instance != null)
        {
            QueueFree();
            return;
        }
        Instance = this;

        // 1. Carrega todas as PackedScene's e Resources necessárias
        PackedScene soldierScene = GD.Load<PackedScene>(SoldierScenePath);
        Texture2D soldierIcon = GD.Load<Texture2D>(SoldierIconPath);

        // 2. Inicializa o Catálogo
        InitializeUnitData(soldierScene, soldierIcon);

        Log.Info($"Catálogo de Dados carregado. Unidades registradas: {UnitDataByID.Count}");
    }

    private void InitializeUnitData(PackedScene soldierScene, Texture2D soldierIcon)
    {
        // Cria a instância do Resource em memória
        UnitData soldierData = new UnitData
        {
            Id = 1,
            Name = "Soldado de Infantaria",
            Cost = 50,
            Icon = soldierIcon,
            UnitScene = soldierScene
        };

        // Adiciona ao dicionário pelo ID
        UnitDataByID.Add(soldierData.Id, soldierData);

        // Repita este bloco para outras unidades (ex: TankData, WorkerData)
    }

    // Método helper para outras classes (ex: Building)
    public UnitData GetUnitData(int id)
    {
        if (UnitDataByID.TryGetValue(id, out UnitData data))
        {
            return data;
        }
        Log.Error($"Tentativa de acessar UnitData com ID desconhecido: {id}");
        return null;
    }
}