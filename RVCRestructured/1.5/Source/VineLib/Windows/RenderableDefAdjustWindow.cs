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
    private readonly string[] labels = ["North", "East", "South", "West"];

    private readonly string[][] numInputBuffers = new string[4][];

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

        InitBuffers();

        foreach (RenderableDef def in renderableDefs) floatMenuOptions.Add(new FloatMenuOption(def.defName, () =>
        {
            selectedDef = def;
            InitBuffers();
        }));
    }

    private void InitBuffers()
    {
        for (int i = 0; i < numInputBuffers.Length; i++)
        {
            numInputBuffers[i] = new string[3];
            numInputBuffers[i].AsSpan().Fill(string.Empty);
        }
    }

    public override void DoWindowContents(Rect _)
    {
        if (Widgets.ButtonText(buttonDefSelect, selectedDef.defName)) Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
        if (Widgets.ButtonText(buttonPosSize, isPos ? "Position" : "Size")) isPos = !isPos;

        //North, East, South, West
        for (int i = 0; i < 4; i++)
        {
            DrawInputFields(i);
        }
    }

    private void DrawInputFields(int rowIndex)
    {
        Vector2 moveVec = new Vector2(110f, 0f) * rowIndex;
        Rect tempLabel = label.MoveRect(moveVec);
        Rect tempX = inputX.MoveRect(moveVec);
        Rect tempY = inputY.MoveRect(moveVec);
        Rect tempZ = inputZ.MoveRect(moveVec);

        Widgets.Label(tempLabel, labels[rowIndex]);

        float tempValX = isPos ? selectedDef.GetPosRef(rowIndex).x : selectedDef.GetSizeRef(rowIndex).x;
        float tempValY = isPos ? selectedDef.GetPosRef(rowIndex).y : selectedDef.GetSizeRef(rowIndex).y;
        float tempValZ = selectedDef.GetPosRef(rowIndex).z;

        Widgets.TextFieldNumeric(tempX, ref tempValX, ref numInputBuffers[rowIndex][0], -10f, 10f);
        Widgets.TextFieldNumeric(tempY, ref tempValY, ref numInputBuffers[rowIndex][1], -10f, 10f);

        if (isPos)
        {
            Widgets.TextFieldNumeric(tempZ, ref tempValZ, ref numInputBuffers[rowIndex][2], -10f, 10f);
            selectedDef.GetPosRef(rowIndex) = new(tempValX, tempValY, tempValZ);
            return;
        }

        selectedDef.GetSizeRef(rowIndex) = new(tempValX, tempValY);
    }
}
