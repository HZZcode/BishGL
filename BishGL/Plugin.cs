using BishSdk;

namespace BishGL;

// ReSharper disable once UnusedType.Global
public class Plugin : IPlugin
{
    public void Initialize(PluginExports exports)
    {
        BuiltinsRegistry.Register();
        exports.Exports.Add("Color", BishColor.StaticType);
        exports.Exports.Add("GL", BishGl.StaticType);
    }
}