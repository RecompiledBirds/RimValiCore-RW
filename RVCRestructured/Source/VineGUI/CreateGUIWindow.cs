﻿using UnityEngine;

namespace NesGUI;

public class CreateItemWindow(Type type, GUITextElement.TextElemType textType = GUITextElement.TextElemType.Button, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont font = GameFont.Tiny) : Window
{
    public override void OnCancelKeyPressed()
    {
        //Do nothing
    }

    public override void OnAcceptKeyPressed()
    {
        //Do nothing
    }

    public override Vector2 InitialSize
    {
        get
        {
            return new Vector2(575, 155);
        }
    }

    private readonly Type itemToMakeType = type;
    private TextAnchor anchor = anchor;
    private GameFont font = font;
    private readonly GUITextElement.TextElemType elemType = textType;
    private string xSizeBuffer = string.Empty;
    private string ySizeBuffer = string.Empty;
    private string xTwoSizeBuffer = string.Empty;
    private string yTwoSizeBuffer = string.Empty;
    private GUIRect? rectToUse;

    public List<FloatMenuOption> GetFonts()
    {
        List<FloatMenuOption> res = [];

        foreach (GameFont font in Enum.GetValues(typeof(GameFont)))
        {
            res.Add(new FloatMenuOption($"{font}  {(this.font == font ? "(current)" : "")}", delegate ()
            {
                this.font = font;
            }));
        }
        return res;
    }

    public List<FloatMenuOption> GetAnchors()
    {

        List<FloatMenuOption> res = [];

        foreach (TextAnchor anchor in Enum.GetValues(typeof(TextAnchor)))
        {
            res.Add(new FloatMenuOption($"{anchor}  {(this.anchor == anchor ? "(current)" : "")}", delegate ()
            {
                this.anchor = anchor;
            }));
        }
        return res;
    }

    public List<FloatMenuOption> RectList()
    {
        List<FloatMenuOption> result = [];
        foreach (GUIRect rect in GuiMaker.Rectangles.Cast<GUIRect>())
        {
            result.Add(new FloatMenuOption(rect.Name, delegate ()
            {
                rectToUse = rect;
            }));
        }

        return result;
    }

    private string? name;
    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        Rect closeRect = new(new Vector2(inRect.xMax - 25, 5), new Vector2(20, 20));
        if (Widgets.ButtonImage(closeRect, Widgets.CheckboxOffTex))
        {
            Close();
        }

        Rect labelRect = new(new Vector2(inRect.x, 10), new Vector2(inRect.xMax, 40));
        if (itemToMakeType == typeof(GUIRect))
        {
            Vector2 size = new(100, 100);
            Vector2 pos = new(0, 0);
            //COMPILED BY NESGUI
            
            //Rect pass

            Rect createrectanglelabel = new(new Vector2(10f, 10f), new Vector2(540f, 30f));
            Rect namelabelrect = new(new Vector2(10f, 45f), new Vector2(100f, 30f));
            Rect inputfieldname = new(new Vector2(115f, 45f), new Vector2(420f, 30f));
            Rect Setsizelabel = new(new Vector2(10f, 80f), new Vector2(100f, 30f));
            Rect sizex = new(new Vector2(115f, 80f), new Vector2(30f, 30f));
            Rect sizey = new(new Vector2(155f, 80f), new Vector2(30f, 30f));
            Rect createbuttonrect = new(new Vector2(195f, 80f), new Vector2(340f, 30f));

            //Button pass

            GameFont prevFont = Text.Font;
            TextAnchor textAnchor = Text.Anchor;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleLeft;

            bool Createbutton = Widgets.ButtonText(createbuttonrect, "Create button");

            Text.Font = prevFont;
            Text.Anchor = textAnchor;

            //Checkbox pass


            //Label pass

            prevFont = Text.Font;
            textAnchor = Text.Anchor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;

            Widgets.Label(namelabelrect, "Set name:");

            Text.Font = prevFont;
            Text.Anchor = textAnchor;
            prevFont = Text.Font;
            textAnchor = Text.Anchor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleRight;

            Widgets.Label(Setsizelabel, "Size:");

            Text.Font = prevFont;
            Text.Anchor = textAnchor;

            Widgets.Label(createrectanglelabel, "Create rectangle");

            //Textfield pass
            name = Widgets.TextField(inputfieldname, name);

            Utility.NumericScrollWheelField(sizex, ref size.y, ref ySizeBuffer);


            Utility.NumericScrollWheelField(sizey, ref size.x, ref xSizeBuffer);


            //END NESGUI CODE

            

            if (Createbutton && name != null)
            {
                GuiMaker.MakeRect(size, pos, name);
            }
            return;
        }
        if (itemToMakeType == typeof(GUILine))
        {
            GameFont prevFont = Text.Font;
            Text.Font = GameFont.Medium;
            Vector2 posOne = new(0, 0);
            Vector2 posTwo = new(0, 0);
            Widgets.LabelFit(labelRect, "Create Line");
            Text.Font = prevFont;
            float rectSizePos = 40;
            Rect labelPosOneRect = new(new Vector2(inRect.x, rectSizePos), new Vector2(40, 40));
            Rect xPosOneRect = new(new Vector2(labelPosOneRect.xMax + 10, rectSizePos), new Vector2(30, 40));
            Rect yPosOneRect = new(new Vector2(xPosOneRect.xMax, rectSizePos), new Vector2(30, 40));
            Rect labelPosTwoRect = new(new Vector2(yPosOneRect.xMax + 20, rectSizePos), new Vector2(40, 40));
            Rect xPosTwoRect = new(new Vector2(labelPosTwoRect.xMax + 10, rectSizePos), new Vector2(30, 40));
            Rect yPosTwoRect = new(new Vector2(xPosTwoRect.xMax, rectSizePos), new Vector2(30, 40));

            Rect nameLabelRect = new(new Vector2(inRect.x, rectSizePos * 2), new Vector2(40, 40));
            Rect nameFieldRect = new(new Vector2(nameLabelRect.xMax + 5, rectSizePos * 2), new Vector2(100, 40));

            Widgets.Label(nameLabelRect, "Name:");
            name = Widgets.TextField(nameFieldRect, name);

            Widgets.Label(labelPosOneRect, "Start point:");
            Utility.NumericScrollWheelField(xPosOneRect, ref posOne.x, ref xSizeBuffer);
            Utility.NumericScrollWheelField(yPosOneRect, ref posOne.y, ref ySizeBuffer);

            Widgets.Label(labelPosTwoRect, "End point:");
            Utility.NumericScrollWheelField(xPosTwoRect, ref posTwo.x, ref xTwoSizeBuffer);
            Utility.NumericScrollWheelField(yPosTwoRect, ref posTwo.y, ref yTwoSizeBuffer);

            Widgets.DrawLine(new Vector2(posOne.x, posOne.y + 80), new Vector2(posTwo.x, posTwo.y + 80), Color.white, 1);

            Rect createRect = new(new Vector2(inRect.x, rectSizePos * 3), new Vector2(inRect.xMax, 40));
            if (Widgets.ButtonText(createRect, "Create!") && rectToUse != null)
            {
                //GuiMaker.MakeLine(posOne, posTwo, name);
                Close();
            }
            return;
        }
        if (itemToMakeType == typeof(GUITextElement))
        {
            //COMPILED BY NESGUI
            //Prepare varibles

            //Rect pass

            Rect creatinglabelrect = new(new Vector2(5f, 5f), new Vector2(488f, 30f));
            Rect namelabel = new(new Vector2(5f, 40f), new Vector2(100f, 30f));
            Rect namefield = new(new Vector2(110f, 40f), new Vector2(383f, 30f));
            Rect assignrectbuttonrect = new(new Vector2(5f, 75f), new Vector2(100f, 30f));
            Rect createbuttonrect = new(new Vector2(393f, 75f), new Vector2(100f, 30f));
            Rect Setfontrect = new(new Vector2(110f, 75f), new Vector2(130f, 30f));
            Rect Setanchorrect = new(new Vector2(250f, 75f), new Vector2(130f, 30f));

            //Button pass


            bool AssignRect = Widgets.ButtonText(assignrectbuttonrect, "Assign Rect");



            bool Create = Widgets.ButtonText(createbuttonrect, "Create!");


            bool Setfont = Widgets.ButtonText(Setfontrect, "Set font");



            bool Setanchor = Widgets.ButtonText(Setanchorrect, "Set anchor");

            //Label pass


            Widgets.Label(namelabel, "Set name/label:");

            GameFont prevFont = Text.Font;
            TextAnchor textAnchor = Text.Anchor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;

            Widgets.Label(creatinglabelrect, $"Creating {elemType}");

            Text.Font = prevFont;
            Text.Anchor = textAnchor;

            //Textfield pass


            name = Widgets.TextField(namefield, name);


            //END NESGUI CODE
            if (AssignRect)
            {
                Find.WindowStack.Add(new FloatMenu(RectList()));
            }
            if (Setfont)
            {
                Find.WindowStack.Add(new FloatMenu(GetFonts()));
            }
            if (Setanchor)
            {
                Find.WindowStack.Add(new FloatMenu(GetAnchors()));
            }
            if (Create && rectToUse != null && !name.NullOrEmpty())
            {
                if (GUITextElement.TextElemType.Button == elemType)
                {
                    GuiMaker.MakeButton(rectToUse, name, anchor, font);
                    return;
                }
                if (GUITextElement.TextElemType.Label == elemType)
                {
                    GuiMaker.MakeLabel(rectToUse, name, anchor, font);
                    return;
                }
                if (GUITextElement.TextElemType.Checkbox == elemType)
                {
                    GuiMaker.MakeCheckBox(rectToUse, name, anchor, font);
                    return;
                }
                if (GUITextElement.TextElemType.Textfield == elemType)
                {
                    GuiMaker.MakeTextField(rectToUse, name, anchor, font);
                    return;
                }
                Close();
            }
        }
    }



}
