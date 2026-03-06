using System.Numerics;
using BishRuntime;
using Raylib_cs;

namespace BishGL;

// ReSharper disable once InconsistentNaming
public class BishGL : BishObject
{
    public int Width = 800;
    public int Height = 600;
    public string Title = "BishGL";
    public int Fps = 60;
    public Color Background = Color.Black;
    public Image Canvas;
    public Texture2D Texture;
    public bool End;

    private static readonly BishGL Instance = new();

    public new static readonly BishType StaticType = new("GL");
    public override BishType DefaultType => StaticType;

    [Builtin("hook")]
    public static BishGL Create(BishObject _) => Instance;

    [Builtin("hook")]
    public static void Init(BishGL self, [DefaultNull] BishInt? width, [DefaultNull] BishInt? height,
        [DefaultNull] BishColor? background, [DefaultNull] BishString? title, [DefaultNull] BishInt? fps)
    {
        if (width is not null) self.Width = width.Value;
        if (height is not null) self.Height = height.Value;
        if (title is not null) self.Title = title.Value;
        if (fps is not null) self.Fps = fps.Value;
        if (background is not null) self.Background = background.Color;

        Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
        Raylib.InitWindow(self.Width, self.Height, self.Title);
        Raylib.SetTargetFPS(self.Fps);
        self.Canvas = Raylib.GenImageColor(self.Width, self.Height, self.Background);
        self.Texture = Raylib.LoadTextureFromImage(self.Canvas);
    }

    [Builtin(special: false)]
    public static void Loop(BishGL self, BishObject callback) => self.Run(gl => callback.Call([gl]));

    [Builtin(special: false)]
    public static void DrawPixel(BishGL self, BishInt x, BishInt y, BishColor color) =>
        Raylib.ImageDrawPixel(ref self.Canvas, x.Value, y.Value, color.Color);

    [Builtin(special: false)]
    public static void DrawLine(BishGL self, BishInt x1, BishInt y1, BishInt x2, BishInt y2, BishColor color) =>
        Raylib.ImageDrawLine(ref self.Canvas, x1.Value, y1.Value, x2.Value, y2.Value, color.Color);

    [Builtin(special: false)]
    public static void DrawRectangle(BishGL self, BishInt x, BishInt y, BishInt w, BishInt h, BishColor color) =>
        Raylib.ImageDrawRectangle(ref self.Canvas, x.Value, y.Value, w.Value, h.Value, color.Color);

    [Builtin(special: false)]
    public static void DrawCircle(BishGL self, BishInt x, BishInt y, BishInt r, BishColor color) =>
        Raylib.ImageDrawCircle(ref self.Canvas, x.Value, y.Value, r.Value, color.Color);

    [Builtin(special: false)]
    public static void DrawTriangle(BishGL self, BishInt x1, BishInt y1,
        BishInt x2, BishInt y2, BishInt x3, BishInt y3, BishColor color) =>
        Raylib.ImageDrawTriangle(ref self.Canvas, new Vector2(x1.Value, y1.Value),
            new Vector2(x2.Value, y2.Value), new Vector2(x3.Value, y3.Value), color.Color);

    [Builtin("hook")]
    public static BishNum Get_time(BishGL self) => new(Raylib.GetTime());

    [Builtin("hook")]
    public static BishInt Get_width(BishGL self) => BishInt.Of(self.Width);

    [Builtin("hook")]
    public static BishInt Get_height(BishGL self) => BishInt.Of(self.Height);

    [Builtin("hook")]
    public static BishString Get_title(BishGL self) => new(self.Title);

    [Builtin("hook")]
    public static BishInt Get_fps(BishGL self) => BishInt.Of(self.Fps);

    [Builtin("hook")]
    public static BishInt Get_realFps(BishGL self) => BishInt.Of(Raylib.GetFPS());

    [Builtin("hook")]
    public static BishNum Get_dt(BishGL self) => new(Raylib.GetFrameTime());

    [Builtin("hook")]
    public static BishColor Get_background(BishGL self) => new(self.Background);

    [Builtin("hook")]
    public static BishColor Set_background(BishGL self, BishColor color)
    {
        self.Background = color.Color;
        return color;
    }

    [Builtin(special: false)]
    public static BishInt Random(BishGL self, BishInt min, BishInt max) =>
        BishInt.Of(Raylib.GetRandomValue(min.Value, max.Value));

    [Builtin(special: false)]
    public static BishBool KeyDown(BishGL self, BishInt key) =>
        BishBool.Of(Raylib.IsKeyDown((KeyboardKey)key.Value));

    [Builtin(special: false)]
    public static BishBool Pressed(BishGL self, BishInt key) =>
        BishBool.Of(Raylib.IsKeyPressed((KeyboardKey)key.Value));

    [Builtin(special: false)]
    public static void Exit(BishGL self) => self.End = true;

    public void Run(Action<BishGL> callback)
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.ImageClearBackground(ref Canvas, Background);

            callback(this);

            if (End)
            {
                Raylib.CloseWindow();
                return;
            }

            unsafe
            {
                Raylib.UpdateTexture(Texture, Canvas.Data);
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Background);
            Raylib.DrawTexture(Texture, 0, 0, Color.White);
            Raylib.DrawFPS(10, 10);
            Raylib.EndDrawing();
        }

        Raylib.UnloadTexture(Texture);
        Raylib.UnloadImage(Canvas);
        Raylib.CloseWindow();
    }

    static BishGL() => BishBuiltinBinder.Bind<BishGL>();
}