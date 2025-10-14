using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class UnitData : Resource
{
    [Export]
    public string Id { get; set; }

    [Export]
    public string Name { get; set; } = "New Unit";

    [Export]
    public int Cost { get; set; } = 50;

    [Export]
    public Texture2D DefaultIcon { get; set; }

    // NOVO: Dicionário para Ícones da UI (usado em BuildingManagerUi)
    public Dictionary<int, Texture2D> IconByTeam { get; set; } = new Dictionary<int, Texture2D>();

    // NOVO: Dicionário para Texturas do Sprite da Unidade (usado em Unit.cs)
    public Dictionary<int, Texture2D> SpriteTextureByTeam { get; set; } = new Dictionary<int, Texture2D>();

    // A UnitScene é a mesma, pois a única diferença é a textura
    [Export]
    public PackedScene UnitScene { get; set; }

    // NOVO MÉTODO: Obtém o ícone para a equipe (ex: UnitData.GetIcon(GameManager.LocalPlayerTeamId))
    public Texture2D GetIcon(int teamId)
    {
        // Se já estiver inicializado acima, não há risco de NullReferenceException aqui.
        if (IconByTeam.TryGetValue(teamId, out Texture2D icon))
        {
            return icon;
        }
        return null;
    }
}