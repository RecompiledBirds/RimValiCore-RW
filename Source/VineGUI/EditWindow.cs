using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace NesGUI
{
    public class EditableItemWindow : Window
    {
        private string xSizeBuffer;
        private string ySizeBuffer;
        private string xPosBuffer;
        private string yPosBuffer;
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
                return new Vector2(440, 190);
            }
        }
        private GUIItem gI;
        public EditableItemWindow(GUIItem item)
        {
            gI = item;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Rect editingitemlabel = new Rect(new Vector2(10f, 5f), new Vector2(480f, 30f));
            Rect closeRect = new Rect(new Vector2(inRect.xMax - 25, 5), new Vector2(20, 20));
            if (Widgets.ButtonImage(closeRect, Widgets.CheckboxOffTex))
            {
                Close();
            }
            Widgets.Label(editingitemlabel, $"Editing: {gI.name}");
            if (gI is GUIRect rect)
            {
                //COMPILED BY NESGUI
                //Rect pass

                Rect setPosLabel = new Rect(new Vector2(15f, 35f), new Vector2(100f, 30f));
                Rect posXRect = new Rect(new Vector2(105f, 35f), new Vector2(40f, 30f));
                Rect posyrect = new Rect(new Vector2(155f, 35f), new Vector2(40f, 30f));
                Rect setSizeRect = new Rect(new Vector2(15f, 70f), new Vector2(100f, 30f));
                Rect sizeXRect = new Rect(new Vector2(105f, 70f), new Vector2(40f, 30f));
                Rect sizeYRect = new Rect(new Vector2(155f, 70f), new Vector2(40f, 30f));

                //Button pass


                //Checkbox pass


                //Label pass

                GameFont prevFont = Text.Font;
                TextAnchor textAnchor = Text.Anchor;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;

                Widgets.Label(setPosLabel, "Set position:");

                Text.Font = prevFont;
                Text.Anchor = textAnchor;
                prevFont = Text.Font;
                textAnchor = Text.Anchor;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;

                Widgets.Label(setSizeRect, "Set size:");

                Text.Font = prevFont;
                Text.Anchor = textAnchor;

                //Textfield pass
                //These weren't done by NesGUI 100%, but I did use it to position them.
                Utility.NumericScrollWheelField(posXRect, ref rect.pos.x, ref xPosBuffer);
                Utility.NumericScrollWheelField(posyrect, ref rect.pos.y, ref yPosBuffer);
                Utility.NumericScrollWheelField(sizeXRect, ref rect.size.x, ref xSizeBuffer);
                Utility.NumericScrollWheelField(sizeYRect, ref rect.size.y, ref ySizeBuffer);
                //END NESGUI CODE

                //Scroll controls
                

                rect.UpdateRect();

                return;
            }


            if (gI.GetType() == typeof(GUITextElement))
            {
                //COMPILED BY NESGUI

                //As a note, each varible name is made from the name the user gave the editor.


                Rect selectRectButton = new Rect(new Vector2(240f, 90f), new Vector2(120f, 40f));
                Rect nameInputLabel = new Rect(new Vector2(10f, 45f), new Vector2(100f, 40f));
                Rect inputField = new Rect(new Vector2(130f, 45f), new Vector2(230f, 40f));
                Rect selectFontRect = new Rect(new Vector2(10f, 90f), new Vector2(120f, 40f));
                Rect selectAnchorRect = new Rect(new Vector2(135f, 90f), new Vector2(100f, 40f));

                bool selectRect = Widgets.ButtonText(selectRectButton, gI.parent == null ? "Aelect rect" : $"Using rec: {gI.parent.name}");
                bool selectFont = Widgets.ButtonText(selectFontRect, "Select font");
                bool selectAnchor = Widgets.ButtonText(selectAnchorRect, "Select anchor");

                Widgets.Label(nameInputLabel, "Set name/label:");
                //END NESGUI CODE
                gI.name = Widgets.TextField(inputField, gI.name);
                gI.label = gI.name;

                if (selectRect)
                {
                    Find.WindowStack.Add(new FloatMenu(SetRect()));
                }

                if (selectFont)
                {
                    Find.WindowStack.Add(new FloatMenu(GetFonts()));
                }

                if (selectAnchor)
                {
                    Find.WindowStack.Add(new FloatMenu(GetAnchors()));
                }

            }
        }
        public List<FloatMenuOption> SetRect()
        {
            List<FloatMenuOption> result = new List<FloatMenuOption>();
            foreach (GUIRect i in GuiMaker.Rectangles)
            {
                result.Add(new FloatMenuOption(i.name, delegate ()
                {
                    gI.parent = i;

                }));
            }
            return result;
        }

        public List<FloatMenuOption> GetFonts()
        {
            List<FloatMenuOption> res = new List<FloatMenuOption>();
            if (gI is GUITextElement elem)
            {
                foreach (GameFont font in Enum.GetValues(typeof(GameFont)))
                {
                    res.Add(new FloatMenuOption($"{font}  {(elem.GetGameFont == font ? "(current)" : "")}", delegate ()
                    {
                        elem.SetFont(font);
                    }));
                }
            }
            return res;
        }

        public List<FloatMenuOption> GetAnchors()
        {

            List<FloatMenuOption> res = new List<FloatMenuOption>();
            if (gI is GUITextElement elem)
            {
                foreach (TextAnchor anchor in Enum.GetValues(typeof(TextAnchor)))
                {
                    res.Add(new FloatMenuOption($"{anchor}  {(elem.GetTextAnchor == anchor ? "(current)" : "")}", delegate ()
                    {
                        elem.SetAnchor(anchor);
                    }


                ));
                }
            }

            return res;
        }
    }
}
