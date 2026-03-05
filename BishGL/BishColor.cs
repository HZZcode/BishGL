using BishRuntime;
using Raylib_cs;

namespace BishGL;

public class BishColor(Color color) : BishObject
{
    public Color Color { get; private set; } = color;

    public new static readonly BishType StaticType = new("Color");
    public override BishType DefaultType => StaticType;

    [Builtin("hook")]
    public static BishColor Create(BishObject _) => new(Color.Blank);

    [Builtin("hook")]
    public static BishNull Init(BishColor self, BishInt r, BishInt g, BishInt b, [DefaultNull] BishInt? a)
    {
        self.Color = new Color(r.Value, g.Value, b.Value, a?.Value ?? 255);
        return BishNull.Instance;
    }

    [Builtin("hook")]
    public static BishInt Get_r(BishColor self) => BishInt.Of(self.Color.R);

    [Builtin("hook")]
    public static BishInt Get_g(BishColor self) => BishInt.Of(self.Color.G);

    [Builtin("hook")]
    public static BishInt Get_b(BishColor self) => BishInt.Of(self.Color.B);

    [Builtin("hook")]
    public static BishInt Get_a(BishColor self) => BishInt.Of(self.Color.A);

    static BishColor() => BishBuiltinBinder.Bind<BishColor>();
}