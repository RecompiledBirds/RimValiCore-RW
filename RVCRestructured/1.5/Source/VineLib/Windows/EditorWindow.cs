using RVCRestructured.Windows;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RVCRestructured;


public static class ConfigurePatch
{
    public static void Patch()
    {
        bool button = Widgets.ButtonText(new Rect(new Vector2(450, 0), new Vector2(100, 30)), "RVC_EditPawn".Translate());
        if (button)
        {
            Find.WindowStack.Add(new EditorWindow());
        }
    }
}

public class EditorWindow : Window
{
    private const float RectColorFieldHeight = 40f;

    private readonly List<Pawn> pawns = Find.GameInitData.startingAndOptionalPawns;

    private readonly Rect RectWindowMain = new(0f, 0f, 1000f, 400f);
    private readonly Rect RectWindowSub;
    private readonly Rect RectWindowEdit;
    private readonly Rect RectPawnSelectOuter;
    private readonly Rect RectColorSelectOuter;

    private readonly Rect[] RectEditSections;
    private readonly Rect[] RectNamingRects;
    private readonly Rect RectColoringPart;
    private readonly Rect RectPawnBig;
    private readonly Rect RectInfoBox;

    private Dictionary<string, TriColorSet> colorSets = [];
    private Rect[] RectColorFields;
    private Rect RectColorSelectInner;
    private Rect RectPawnSelectInner;
    private Vector2 PawnSelectorScroll;
    private Vector2 ColorSelectorScroll;

    private Pawn selectedPawn;

    public override Vector2 InitialSize => RectWindowMain.size;

    protected override float Margin => 0f;

    public EditorWindow()
    {
        doCloseX = true;
        closeOnClickedOutside = true;

        SelectedPawn = Find.GameInitData.startingAndOptionalPawns[0];

        RectWindowSub = RectWindowMain.ContractedBy(25f);
        RectPawnSelectOuter = RectWindowSub.LeftPartPixels(172f);
        RectPawnSelectInner = RectPawnSelectOuter.GetInnerScrollRect(55f * pawns.Count);

        RectWindowEdit = RectWindowSub.RightPartPixels(RectWindowSub.width - RectPawnSelectOuter.width - 5f);

        RectEditSections = RectWindowEdit.DivideVertical(2).ToArray();
        RectColoringPart = RectEditSections[0];
        RectNamingRects = RectEditSections[1].TopPartPixels(39f).ContractVertically(5).DivideHorizontal(3).ToArray();
        RectColorSelectOuter = RectColoringPart.RightPartPixels(300f).ContractVertically(5);
        RectColorSelectOuter.width -= 5;

        CalcInnerRect();

        RectPawnBig = RectColoringPart.LeftPartPixels(RectEditSections[0].height);
        RectInfoBox = RectPawnBig.MoveRect(new Vector2(RectPawnBig.width + 5f, 0f)).ContractedBy(5f);
        RectInfoBox.width = RectColoringPart.width - RectPawnBig.width - RectColorSelectOuter.width - 20f;

        //replace saved colors with black if missing at this point to prevent errors
        VineSettings.savedColors = VineSettings.savedColors ?? [Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black, Color.black];
    }

    /// <summary>
    ///     Recalculates the height and width of the inner scroll view for the color picking part
    /// </summary>
    private void CalcInnerRect()
    {
        List<Rect> rectList = [];

        RectColorSelectInner = new Rect(RectColorSelectOuter)
        {
            height = colorSets.Count * RectColorFieldHeight - 5f
        };

        if (HasOpenColorField) RectColorSelectInner.height += RectColorFieldHeight * 3f;

        if (RectColorSelectInner.height > RectColorSelectOuter.height) RectColorSelectInner.width -= 17f;

        RectColorSelectInner.height += 5f;

        for (int i = 0; i < colorSets.Count; i++)
        {
            Vector2 mod = new(0f, RectColorFieldHeight * i + ((HasOpenColorField && (i > OpenColorField)) ? RectColorFieldHeight * 3f : 0f));
            Rect tempRect = RectColorSelectInner.TopPartPixels(RectColorFieldHeight).MoveRect(mod);
            tempRect.height -= 5f;

            rectList.Add(tempRect);
        }

        RectColorFields = rectList.ToArray();
    }

    private bool HasOpenColorField => OpenColorField > -1;

    private int OpenColorField { get; set; }

    private Pawn SelectedPawn
    {
        get => selectedPawn;
        set
        {
            selectedPawn = value;
            RVRComp comp = SelectedPawn.TryGetComp<RVRComp>();
            if ( comp!=null)
            {
                
                colorSets = comp.Colors;
            }
            else
            {
                colorSets = [];
            }

            OpenColorField = -1;
            CalcInnerRect();
        }
    }

    public override void DoWindowContents(Rect _)
    {
        DrawPawnSelectionArea();
        DrawPawn();
        DrawColorSelection();
        DrawNameEdit();
        DrawInfoBox();
    }

    /// <summary>
    /// Creates a small box with information for the user
    /// </summary>
    private void DrawInfoBox()
    {
        Widgets.DrawHighlight(RectInfoBox);
        Widgets.DrawBox(RectInfoBox, 2);
        Widgets.Label(RectInfoBox.ContractedBy(2f + 5f), $"{"RVC_Tutorial".Translate($"<color=green>{SelectedPawn.def.label}</color>")}");
    }

    /// <summary>
    /// Draws the color selection ScrollView
    /// </summary>
    private void DrawColorSelection()
    {
        Widgets.BeginScrollView(RectColorSelectOuter, ref ColorSelectorScroll, RectColorSelectInner);
        int pos = 0;
        foreach (KeyValuePair<string, TriColorSet> kvp in colorSets)
        {
            string name = kvp.Key;
            Rect rectTemp = RectColorFields[pos];
            Rect rectName = rectTemp;
            Rect rectExpandCollapseIcon = rectTemp.LeftPartPixels(rectTemp.height);

            Text.Anchor = TextAnchor.MiddleLeft;

            Widgets.DrawLightHighlight(rectName);
            Widgets.DrawHighlightIfMouseover(rectTemp);
            Widgets.DrawBox(rectName, 2);
            Widgets.Label(rectName.RightPartPixels(rectName.width - rectTemp.height - 5f), name);
            Widgets.DrawTextureFitted(rectExpandCollapseIcon.ContractedBy(11f), pos == OpenColorField ? TexButton.Collapse : TexButton.Reveal, 1f);

            if (Widgets.ButtonInvisible(rectTemp))
            {
                if (pos == OpenColorField)
                {
                    OpenColorField = -1;
                    SoundDefOf.TabClose.PlayOneShotOnCamera();
                }
                else
                {
                    if (HasOpenColorField) SoundDefOf.TabClose.PlayOneShotOnCamera();

                    OpenColorField = pos;
                    SoundDefOf.TabOpen.PlayOneShotOnCamera();
                }

                CalcInnerRect();
            }
            if (pos == OpenColorField)
            {
                for (int i = 0; i < 3; i++)
                {
                    float indent = 15f;

                    string colorName = $"RVC_Edit{(Count)i}".Translate();
                    Rect rectColorField = rectTemp.MoveRect(new Vector2(indent, RectColorFieldHeight * (i + 1)));
                    rectColorField.width -= indent + 1f;

                    Rect rectColorLabel = rectColorField.RightPartPixels(rectColorField.width - 5f);
                    Rect rectColorColor = rectColorField.RightPartPixels(rectColorLabel.width - 100f - 5f);

                    Widgets.DrawLightHighlight(rectColorField);
                    Widgets.DrawBoxSolidWithOutline(rectColorColor, kvp.Value[i], new Color(255f, 255f, 255f, 0.5f), 3);
                    Widgets.DrawHighlightIfMouseover(rectColorColor);
                    Widgets.DrawBox(rectColorField);
                    Widgets.Label(rectColorLabel, colorName);

                    if (Widgets.ButtonInvisible(rectColorColor))
                    {
                        int k = i; //save the current i to k so that the value of i isn't overridden during the for loop
                        Find.WindowStack.Add(new ColorPickerWindow(color => SetColor(color, kvp, k), (newSavedColors) => { VineSettings.savedColors = newSavedColors.ToList(); RimValiCore.settings.Write(); }, kvp.Value.Colors[k], VineSettings.savedColors.ToArray()));
                    }
                    TooltipHandler.TipRegion(rectColorColor, $"RVC_EditColor".Translate());
                }
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;

            pos++;
        }

        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;
        Widgets.EndScrollView();
    }

    /// <summary>
    /// Draws the name editor fields
    /// </summary>
    private void DrawNameEdit()
    {
        if (SelectedPawn.Name is NameTriple name)
        {
            string first = Widgets.TextField(RectNamingRects[0], name.First, 25, CharacterCardUtility.ValidNameRegex);

            if (name.First.Equals(name.Nick) || name.Last.Equals(name.Nick)) GUI.color = new Color(255f, 255f, 255f, 0.4f);
            string nick = Widgets.TextField(RectNamingRects[1].ContractHorizontally(5), name.Nick, 25, CharacterCardUtility.ValidNameRegex);
            GUI.color = Color.white;

            string last = Widgets.TextField(RectNamingRects[2], name.Last, 25, CharacterCardUtility.ValidNameRegex);

            SelectedPawn.Name = new NameTriple(first, nick, last);
        }
        else
        {
            //Fixes names that aren't NameTriple based by converting them into one
            string[] fullName = SelectedPawn.Name.ToString().Split(' ');

            string first = fullName[0];
            string nick = "";
            string last = "";

            if (fullName.Length > 1)
            {
                last = fullName[fullName.Length - 1];

                for (int i = 1; i < fullName.Length - 1; i++)
                {
                    nick += $"{fullName[i]} ";
                }

                if (nick.EndsWith(" "))
                {
                    nick.Substring(0, nick.Length - 1);
                }
            }

            SelectedPawn.Name = new NameTriple(first, nick, last);
        }
    }

    /// <summary>
    /// Sets a new <see cref="Color"/> <paramref name="color"/> into the given <see cref="ColorSet"/> provided in the <see cref="KeyValuePair"/> <paramref name="kvp"/> into the <paramref name="index"/>
    /// </summary>
    /// <param name="color">The new <see cref="Color"/></param>
    /// <param name="kvp">A <see cref="KeyValuePair"/> with <see cref="string"/> and <see cref="ColorSet"/></param>
    /// <param name="index"></param>
    private void SetColor(Color color, KeyValuePair<string, TriColorSet> kvp, int index)
    {
        Color[] colors = kvp.Value.Colors;
        colors[index] = color;
        kvp.Value.Colors = colors;

        SelectedPawn.TryGetComp<RVRComp>()?.InformGraphicsDirty();
        SelectedPawn.Drawer.renderer.renderTree.SetDirty();
    }

    /// <summary>
    /// Draws an image of the selected Pawn
    /// </summary>
    private void DrawPawn()
    {
        Widgets.DrawBox(RectColoringPart);
        RenderTexture image = PortraitsCache.Get(SelectedPawn, new Vector2(1024f, 1024f), Rot4.South, new Vector3(0f, 0f, 0.14f), cameraZoom: 2f, supersample: false);
        GUI.DrawTexture(RectPawnBig, image, ScaleMode.StretchToFill);
    }

    /// <summary>
    /// Draws a list of pawns that can be edited, highlighting the currently selected pawn yellow
    /// </summary>
    private void DrawPawnSelectionArea()
    {
        Widgets.BeginScrollView(RectPawnSelectOuter, ref PawnSelectorScroll, RectPawnSelectInner);
        GUI.BeginGroup(RectPawnSelectInner);

        for (int i = 0; i < pawns.Count; i++)
        {
            Pawn pawn = pawns[i];
            Rect rectPawnBox = new(5f, 55f * i, RectPawnSelectInner.width - 10f, 50f);
            Rect rectPawnContent = rectPawnBox.ContractedBy(5f);
            rectPawnContent.height += 5f;

            Rect rectPawnPortraitArea = rectPawnContent.RightPartPixels(rectPawnContent.height);

            RenderTexture image = PortraitsCache.Get(pawn, new Vector2(256f, 256f), Rot4.South, new Vector3(0f, 0f, 0.25f), stylingStation: true, cameraZoom: 2.5f, supersample: false);

            Widgets.DrawBox(rectPawnBox);
            Widgets.DrawHighlight(rectPawnBox);
            Widgets.DrawHighlightIfMouseover(rectPawnBox);

            Text.Font = GameFont.Tiny;

            if (pawn.Name is NameTriple name && name.Nick is string nick)
            {
                Widgets.Label(rectPawnContent, nick);
            }
            else
            {
                Widgets.Label(rectPawnContent, pawn.Name.ToString());
            }

            Text.Anchor = TextAnchor.LowerLeft;

            Widgets.Label(rectPawnContent.MoveRect(new Vector2(0f, -5f)), pawn.story.TitleCap);

            GUI.color = new Color(1f, 1f, 1f, 0.2f);
            Widgets.DrawTextureFitted(rectPawnPortraitArea, image, 1f);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;

            if (SelectedPawn == pawn)
            {
                Widgets.DrawBoxSolid(rectPawnBox, new Color(181f, 141f, 0f, 0.2f));
            }

            if (Widgets.ButtonInvisible(rectPawnBox))
            {
                SelectedPawn = pawn;
            }
        }

        GUI.EndGroup();
        Widgets.EndScrollView();
    }

    /// <summary>
    /// Used to differentiate the language keys
    /// </summary>
    private enum Count
    {
        Primary,
        Secondary,
        Tetriary
    }
}