using BishRuntime;

namespace BishGL;

public struct Module : IModule
{
    public static BishObject Exports => IModule.ExportsFrom(
        ("Color", BishColor.StaticType),
        ("GL", BishGl.StaticType)
    );

    static Module() => BuiltinsRegistry.Register();
}