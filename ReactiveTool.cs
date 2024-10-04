using Godot;
using zombie_shooter.SourceGeneration;

namespace godotsourcegenerationchainexample;

[Tool]
public partial class ReactiveTool : Label
{
    [ExportReactive] private string text1;
    [ExportReactive] private string text2;
    [ExportReactive] private string text3;

    private void React()
    {
        Text = $"{text1}\n{text2}\n{text3}";
    }
}