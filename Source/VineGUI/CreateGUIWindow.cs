using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace NesGUI
{
    public class CreateItemWinow : Window
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




        public CreateItemWinow(Type type, GUITextElement.TextElemType textType = GUITextElement.TextElemType.Button, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont font = GameFont.Tiny)
        {
            itemToMakeType = type;
            elemType = textType;
            this.font = font;
            this.anchor = anchor;
        }
        private Type itemToMakeType;
        private TextAnchor anchor;
        private GameFont font;
        private GUITextElement.TextElemType elemType;
        private string xSizeBuffer;
        private string ySizeBuffer;
        private string xTwoSizeBuffer;
        private string yTwoSizeBuffer;
        private GUIRect rectTouse;

        public List<FloatMenuOption> GetFonts()
        {
            List<FloatMenuOption> res = new List<FloatMenuOption>();

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

            List<FloatMenuOption> res = new List<FloatMenuOption>();

            foreach (TextAnchor anchor in Enum.GetValues(typeof(TextAnchor)))
            {
                res.Add(new FloatMenuOption($"{anchor}  {(this.anchor == anchor ? "(current)" : "")}", delegate ()
                {
                    this.anchor = anchor;
                }));
            }
            return res;
        }

        public List<FloatMenuOption> rectList()
        {
            List<FloatMenuOption> result = new List<FloatMenuOption>();
            foreach (GUIRect i in GuiMaker.Rectangles)
            {
                result.Add(new FloatMenuOption(i.name, delegate ()
                {
                    rectTouse = i;
                }));
            }
            return result;
        }



        string name;
        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Rect closeRect = new Rect(new Vector2(inRect.xMax - 25, 5), new Vector2(20, 20));
            if (Widgets.ButtonImage(closeRect, Widgets.CheckboxOffTex))
            {
                Close();
            }
            Rect labelRect = new Rect(new Vector2(inRect.x, 10), new Vector2(inRect.xMax, 40));
            if (itemToMakeType == typeof(GUIRect))
            {
                Vector2 size = new Vector2(100, 100);
                Vector2 pos = new Vector2(0, 0);
                //COMPILED BY NESGUI
                //Prepare varibles

                GameFont prevFont = Text.Font;
                TextAnchor textAnchor = Text.Anchor;

                //Rect pass

                Rect createrectanglelabel = new Rect(new Vector2(10f, 10f), new Vector2(540f, 30f));
                Rect namelabelrect = new Rect(new Vector2(10f, 45f), new Vector2(100f, 30f));
                Rect inputfieldname = new Rect(new Vector2(115f, 45f), new Vector2(420f, 30f));
                Rect Setsizelabel = new Rect(new Vector2(10f, 80f), new Vector2(100f, 30f));
                Rect sizex = new Rect(new Vector2(115f, 80f), new Vector2(30f, 30f));
                Rect sizey = new Rect(new Vector2(155f, 80f), new Vector2(30f, 30f));
                Rect createbuttonrect = new Rect(new Vector2(195f, 80f), new Vector2(340f, 30f));

                //Button pass

                prevFont = Text.Font;
                textAnchor = Text.Anchor;
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
                Vector2 posOne = new Vector2(0, 0);
                Vector2 posTwo = new Vector2(0, 0);
                Widgets.LabelFit(labelRect, "Create Line");
                Text.Font = prevFont;
                float rectSizePos = 40;
                Rect labelPosOneRect = new Rect(new Vector2(inRect.x, rectSizePos), new Vector2(40, 40));
                Rect xPosOneRect = new Rect(new Vector2(labelPosOneRect.xMax + 10, rectSizePos), new Vector2(30, 40));
                Rect yPosOneRect = new Rect(new Vector2(xPosOneRect.xMax, rectSizePos), new Vector2(30, 40));
                Rect labelPosTwoRect = new Rect(new Vector2(yPosOneRect.xMax + 20, rectSizePos), new Vector2(40, 40));
                Rect xPosTwoRect = new Rect(new Vector2(labelPosTwoRect.xMax + 10, rectSizePos), new Vector2(30, 40));
                Rect yPosTwoRect = new Rect(new Vector2(xPosTwoRect.xMax, rectSizePos), new Vector2(30, 40));

                Rect nameLabelRect = new Rect(new Vector2(inRect.x, rectSizePos * 2), new Vector2(40, 40));
                Rect nameFieldRect = new Rect(new Vector2(nameLabelRect.xMax + 5, rectSizePos * 2), new Vector2(100, 40));

                Widgets.Label(nameLabelRect, "Name:");
                name = Widgets.TextField(nameFieldRect, name);

                Widgets.Label(labelPosOneRect, "Start point:");
                Utility.NumericScrollWheelField(xPosOneRect, ref posOne.x, ref xSizeBuffer);
                Utility.NumericScrollWheelField(yPosOneRect, ref posOne.y, ref ySizeBuffer);

                Widgets.Label(labelPosTwoRect, "End point:");
                Utility.NumericScrollWheelField(xPosTwoRect, ref posTwo.x, ref xTwoSizeBuffer);
                Utility.NumericScrollWheelField(yPosTwoRect, ref posTwo.y, ref yTwoSizeBuffer);

                Widgets.DrawLine(new Vector2(posOne.x, posOne.y + 80), new Vector2(posTwo.x, posTwo.y + 80), Color.white, 1);

                Rect createRect = new Rect(new Vector2(inRect.x, rectSizePos * 3), new Vector2(inRect.xMax, 40));
                if (Widgets.ButtonText(createRect, "Create!") && rectTouse != null)
                {
                    //GuiMaker.MakeLine(posOne, posTwo, name);
                    this.Close();
                }
                return;
            }
            if (itemToMakeType == typeof(GUITextElement))
            {
                //COMPILED BY NESGUI
                //Prepare varibles

                GameFont prevFont = Text.Font;
                TextAnchor textAnchor = Text.Anchor;

                //Rect pass

                Rect creatinglabelrect = new Rect(new Vector2(5f, 5f), new Vector2(488f, 30f));
                Rect namelabel = new Rect(new Vector2(5f, 40f), new Vector2(100f, 30f));
                Rect namefield = new Rect(new Vector2(110f, 40f), new Vector2(383f, 30f));
                Rect assignrectbuttonrect = new Rect(new Vector2(5f, 75f), new Vector2(100f, 30f));
                Rect createbuttonrect = new Rect(new Vector2(393f, 75f), new Vector2(100f, 30f));
                Rect Setfontrect = new Rect(new Vector2(110f, 75f), new Vector2(130f, 30f));
                Rect Setanchorrect = new Rect(new Vector2(250f, 75f), new Vector2(130f, 30f));

                //Button pass


                bool AssignRect = Widgets.ButtonText(assignrectbuttonrect, "Assign Rect");



                bool Create = Widgets.ButtonText(createbuttonrect, "Create!");


                bool Setfont = Widgets.ButtonText(Setfontrect, "Set font");



                bool Setanchor = Widgets.ButtonText(Setanchorrect, "Set anchor");

                //Label pass


                Widgets.Label(namelabel, "Set name/label:");

                prevFont = Text.Font;
                textAnchor = Text.Anchor;
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
                    Find.WindowStack.Add(new FloatMenu(rectList()));
                }
                if (Setfont)
                {
                    Find.WindowStack.Add(new FloatMenu(GetFonts()));
                }
                if (Setanchor)
                {
                    Find.WindowStack.Add(new FloatMenu(GetAnchors()));
                }
                if (Create && rectTouse != null && !name.NullOrEmpty())
                {
                    if (GUITextElement.TextElemType.Button == elemType)
                    {
                        GuiMaker.MakeButton(rectTouse, name, anchor, font);
                        return;
                    }
                    if (GUITextElement.TextElemType.Label == elemType)
                    {
                        GuiMaker.MakeLabel(rectTouse, name, anchor, font);
                        return;
                    }
                    if (GUITextElement.TextElemType.Checkbox == elemType)
                    {
                        GuiMaker.MakeCheckBox(rectTouse, name, anchor, font);
                        return;
                    }
                    if (GUITextElement.TextElemType.Textfield == elemType)
                    {
                        GuiMaker.MakeTextField(rectTouse, name, anchor, font);
                        return;
                    }
                    Close();
                }
            }
        }



    }
}
