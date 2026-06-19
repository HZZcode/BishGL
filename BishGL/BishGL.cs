using System.Numerics;
using BishRuntime;
using Raylib_cs;

namespace BishGL;

public class BishGl(int width, int height, string title, int fps, Color background) : BishObject
{
    public readonly int Width = width;
    public readonly int Height = height;
    public readonly string Title = title;
    public readonly int Fps = fps;
    public readonly Color Background = background;

    public Image Canvas;
    public Texture2D Texture;
    public bool End;

    private static BishGl? Instance { get; set; }

    public new static readonly BishType StaticType = new("GL");
    public override BishType DefaultType => StaticType;

    [Builtin("hook")]
    public static BishGl New([DefaultNull] BishInt? width, [DefaultNull] BishInt? height,
        [DefaultNull] BishColor? background, [DefaultNull] BishString? title, [DefaultNull] BishInt? fps)
    {
        if (Instance is not null) return Instance;
        Instance = new BishGl(width?.Value ?? 800, height?.Value ?? 600, title?.Value ?? "BishGL", fps?.Value ?? 60,
            background?.Color ?? Color.Black);
        Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
        Raylib.InitWindow(Instance.Width, Instance.Height, Instance.Title);
        Raylib.SetTargetFPS(Instance.Fps);
        Instance.Canvas = Raylib.GenImageColor(Instance.Width, Instance.Height, Instance.Background);
        Instance.Texture = Raylib.LoadTextureFromImage(Instance.Canvas);
        return Instance;
    }

    [Builtin]
    public static void Loop(BishGl self, BishObject callback) => self.Run(gl => callback.Call(new BishArgs([gl])));

    [Builtin]
    public static void DrawPixel(BishGl self, BishInt x, BishInt y, BishColor color) =>
        Raylib.ImageDrawPixel(ref self.Canvas, x.Value, y.Value, color.Color);

    [Builtin]
    public static void DrawLine(BishGl self, BishInt x1, BishInt y1, BishInt x2, BishInt y2, BishColor color) =>
        Raylib.ImageDrawLine(ref self.Canvas, x1.Value, y1.Value, x2.Value, y2.Value, color.Color);

    [Builtin]
    public static void DrawRectangle(BishGl self, BishInt x, BishInt y, BishInt w, BishInt h, BishColor color) =>
        Raylib.ImageDrawRectangle(ref self.Canvas, x.Value, y.Value, w.Value, h.Value, color.Color);

    [Builtin]
    public static void DrawCircle(BishGl self, BishInt x, BishInt y, BishInt r, BishColor color) =>
        Raylib.ImageDrawCircle(ref self.Canvas, x.Value, y.Value, r.Value, color.Color);

    [Builtin]
    public static void DrawTriangle(BishGl self, BishInt x1, BishInt y1,
        BishInt x2, BishInt y2, BishInt x3, BishInt y3, BishColor color) =>
        Raylib.ImageDrawTriangle(ref self.Canvas, new Vector2(x1.Value, y1.Value),
            new Vector2(x2.Value, y2.Value), new Vector2(x3.Value, y3.Value), color.Color);

    [Builtin("hook")]
    public static BishNum Get_time(BishGl _) => new(Raylib.GetTime());

    [Builtin("hook")]
    public static BishInt Get_width(BishGl self) => BishInt.Of(self.Width);

    [Builtin("hook")]
    public static BishInt Get_height(BishGl self) => BishInt.Of(self.Height);

    [Builtin("hook")]
    public static BishString Get_title(BishGl self) => new(self.Title);

    [Builtin("hook")]
    public static BishInt Get_fps(BishGl self) => BishInt.Of(self.Fps);

    [Builtin("hook")]
    public static BishInt Get_realFps(BishGl _) => BishInt.Of(Raylib.GetFPS());

    [Builtin("hook")]
    public static BishNum Get_dt(BishGl _) => new(Raylib.GetFrameTime());

    [Builtin("hook")]
    public static BishColor Get_background(BishGl self) => new(self.Background);

    [Builtin]
    public static BishInt Random(BishGl _, BishInt min, BishInt max) =>
        BishInt.Of(Raylib.GetRandomValue(min.Value, max.Value));

    [Builtin]
    public static BishBool KeyDown(BishGl _, BishInt key) =>
        BishBool.Of(Raylib.IsKeyDown((KeyboardKey)key.Value));

    [Builtin]
    public static BishBool Pressed(BishGl _, BishInt key) =>
        BishBool.Of(Raylib.IsKeyPressed((KeyboardKey)key.Value));

    [Builtin]
    public static void Close(BishGl self) => self.End = true;

    public void Run(Action<BishGl> callback)
    {
        while (!Raylib.WindowShouldClose() && !End)
        {
            Raylib.ImageClearBackground(ref Canvas, Background);

            callback(this);

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
}