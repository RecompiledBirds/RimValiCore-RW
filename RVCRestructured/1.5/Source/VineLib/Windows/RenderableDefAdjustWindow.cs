using LudeonTK;
using RVCRestructured.Defs;
using UnityEngine;
using Verse;

namespace RVCRestructured.Windows;

public static class DebugActions
{
    [DebugAction("RimValiCore", "Open Renderable Def Adjustment Window", allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void OpenRenderableDefAdjustWindow() => Find.WindowStack.Add(new RenderableDefAdjustWindow());
}

public class RenderableDefAdjustWindow : Window
{
    private readonly List<FloatMenuOption> floatMenuOptions = [];
    private readonly List<RenderableDef> renderableDefs;
    private readonly List<string> labels = ["North", "East", "South", "West"];

    private readonly Rect rectFull = new(0f, 0f, 460f, 270f);
    private readonly Rect buttonDefSelect = new(20f, 20f, 90f, 30f);
    private readonly Rect buttonPosSize = new(130f, 20f, 90f, 30f);

    private readonly Rect label = new(20f, 70f, 90f, 30f);
    private readonly Rect inputX = new(20f, 120f, 90f, 30f);
    private readonly Rect inputY = new(20f, 170f, 90f, 30f);
    private readonly Rect inputZ = new(20f, 220f, 90f, 30f);

    private RenderableDef selectedDef;
    private bool isPos = true;

    protected override float Margin => 0f;

    public override Vector2 InitialSize => rectFull.size;

    public RenderableDefAdjustWindow()
    {
        draggable = true;
        preventCameraMotion = false;

        renderableDefs = DefDatabase<RenderableDef>.AllDefsListForReading;
        selectedDef = renderableDefs.FirstOrDefault();

        foreach (RenderableDef def in renderableDefs) floatMenuOptions.Add(new FloatMenuOption(def.defName, () => selectedDef = def));
    }

    public override void DoWindowContents(Rect _)
    {
        if (Widgets.ButtonText(buttonDefSelect, selectedDef.defName)) Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
        if (Widgets.ButtonText(buttonPosSize, isPos ? "Position" : "Size")) isPos = !isPos;

        for (int i = 0; i <= 3; i++)
        {
            DrawInputFields(i);
        }
    }

    private void DrawInputFields(int i)
    {
        Vector2 moveVec = new Vector2(110f, 0f) * i;
        Rect tempLabel = MoveRect(label, moveVec);
        Rect tempX = MoveRect(inputX, moveVec);
        Rect tempY = MoveRect(inputY, moveVec);
        Rect tempZ = MoveRect(inputZ, moveVec);

        Widgets.Label(tempLabel, labels[i]);

        float tempValX = isPos ? selectedDef[i].position.x : selectedDef[i].size.x;
        float tempValY = isPos ? selectedDef[i].position.y : selectedDef[i].size.y;
        float tempValZ = selectedDef[i].position.z;

        string bufferX = tempValX.ToString();
        string bufferY = tempValY.ToString();
        string bufferZ = tempValZ.ToString();

        Widgets.TextFieldNumeric(tempX, ref tempValX, ref bufferX, float.MinValue, float.MaxValue);
        Widgets.TextFieldNumeric(tempY, ref tempValY, ref bufferY, float.MinValue, float.MaxValue);

        if (isPos)
        {
            //   Widgets.TextField(tempZ, bufferZ, int.MaxValue);
            Widgets.TextFieldNumeric(tempZ, ref tempValZ, ref bufferZ, float.MinValue, float.MaxValue);
            //   Widgets.TextFieldPercent(tempZ, ref tempValZ, ref bufferZ, float.MinValue, float.MaxValue);

            selectedDef[i].position.x = tempValX;
            selectedDef[i].position.y = tempValY;
            selectedDef[i].position.z = tempValZ;
        }
        else
        {
            selectedDef[i].size.x = tempValX;
            selectedDef[i].size.y = tempValY;
        }
    }

    /// <summary>
    ///     Creates a copy of this <see cref="Rect" /> moved by a <see cref="Vector2" />
    /// </summary>
    /// <param name="rect">the <see cref="Rect" /> to move</param>
    /// <param name="vec">the distance to move <paramref name="rect" /></param>
    /// <returns>A copy of <paramref name="rect" />, moved by the distance specified in <paramref name="vec" /></returns>
    public static Rect MoveRect(Rect rect, Vector2 vec)
    {
        Rect newRect = new(rect);
        newRect.position += vec;
        return newRect;
    }
}
