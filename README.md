This is an example Godot project that shows how the implementations of the Godot source generators could be applied to the output of another source generator.

[ReactiveTool](https://github.com/benjiwolff/godot-source-generation-chain-example/blob/main/ReactiveTool.cs) defines three private fields and annotates them with [ExportReactive].

```
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
```
In a first step the [ReactivePropertySourceGenerator](https://github.com/benjiwolff/godot-source-generation-chain-example/blob/main/godot-source-generation-chain-example.SourceGeneration/ReactivePropertySourceGenerator.cs) generates the following.
```
using System;
using Godot;

namespace godotsourcegenerationchainexample
{
    public partial class ReactiveTool
    {
        [Export]
        private string Text1
        {
            get => text1;
            set
            {
                text1 = value;
                React();
            }
        }
        [Export]
        private string Text2
        {
            get => text2;
            set
            {
                text2 = value;
                React();
            }
        }
        [Export]
        private string Text3
        {
            get => text3;
            set
            {
                text3 = value;
                React();
            }
        }
    }
}
```
Subsequently, the Godot source generators are applied to **the new compilation that includes the generated syntax trees**, rendering the generated [Export] annotations. The [Export] annotations would not be rendered, if [Godot.SourceGenerators](https://www.nuget.org/packages/Godot.SourceGenerators/4.4.0-dev.3) was simply referenced in [godot-source-generation-chain-example.csproj](https://github.com/benjiwolff/godot-source-generation-chain-example/blob/main/godot-source-generation-chain-example.csproj) because both generators would be applied to the same initial compilation. Instead, [Godot.SourceGenerators.Implementation](https://github.com/benjiwolff/godot/pkgs/nuget/Godot.SourceGenerators.Implementation) from [benjiwolff/godot](https://github.com/benjiwolff/godot) is used.
