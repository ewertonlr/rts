// GameDataCatalog.cs
using Godot;
using System.Collections.Generic;

public partial class GameDataCatalog : Node
{
    public static GameDataCatalog Instance { get; private set; }
    private const int TEAM_BLUE = 1;
    private const int TEAM_RED = 2;
    private const string UnitScenePath = "uid://b42ek7rmo23nq";
    public Dictionary<int, UnitData> UnitDataByID { get; private set; } = new Dictionary<int, UnitData>();
    private Dictionary<string, (string blue, string red)> UnitIconPaths = new()
    {
        {"Gunner", ("uid://c6x4erhsxulv8", "uid://dn2lbnvjrpe2f")},
        {"Rocketman", ("uid://cjcac6myan6e8", "uid://n86ymnbprklu")},
        {"Engineer", ("uid://ddfbey4qxpryy", "uid://cugunb1lalai4")},
        {"Tank", ("uid://ca3epiftdxg60", "uid://dq6e3q8dk6yx")},
        // Adicione mais tipos aqui
    };

    public override void _Ready()
    {
        if (Instance != null)
        {
            QueueFree();
            return;
        }
        Instance = this;

        // 1. Carrega todas as PackedScene's e Resources necessárias
        PackedScene unitScene = GD.Load<PackedScene>(UnitScenePath);
        InitializeUnitData(unitScene); // Chamada simplificada
    }

    private void InitializeUnitData(PackedScene unitScene)
    {
        // Funcao auxiliar para carregar as texturas e criar o UnitData
        void CreateUnit(int id, string typeName, int cost)
        {
            var paths = UnitIconPaths[typeName];

            // 1. Cria a UnitData
            UnitData data = new UnitData
            {
                // Id = id,
                Name = typeName,
                Cost = cost,
                UnitScene = unitScene
            };

            // 2. Carrega e armazena os ícones por equipe
            Texture2D blueIcon = GD.Load<Texture2D>(paths.blue);
            Texture2D redIcon = GD.Load<Texture2D>(paths.red);

            data.IconByTeam.Add(TEAM_BLUE, blueIcon);
            data.IconByTeam.Add(TEAM_RED, redIcon);

            // Se o sprite da unidade for o mesmo que o ícone:
            data.SpriteTextureByTeam = data.IconByTeam;

            // 3. Adiciona ao catálogo
            // UnitDataByID.Add(data.Id, data);
        }

        // CHAME A FUNÇÃO AUXILIAR PARA CADA TIPO DE UNIDADE
        CreateUnit(id: 1, typeName: "Gunner", cost: 10);
        CreateUnit(id: 2, typeName: "Rocketman", cost: 20);
        CreateUnit(id: 3, typeName: "Engineer", cost: 50);
        CreateUnit(id: 4, typeName: "Tank", cost: 200);
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