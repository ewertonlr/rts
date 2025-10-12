using Godot;

// Adicionamos [GlobalClass] para que possamos ver este tipo no editor Godot
[GlobalClass]
public partial class UnitData : Resource
{
    // [Export] é necessário aqui para salvar no arquivo .tres
    [Export]
    public int Id { get; set; } = 0;

    [Export]
    public string Name { get; set; } = "New Unit";

    [Export]
    public int Cost { get; set; } = 50;

    // O ícone deve ser um Texture2D (o sprite que você usará)
    [Export]
    public Texture2D Icon { get; set; }

    // Opcional: Referência à cena real para produção
    [Export]
    public PackedScene UnitScene { get; set; }
}