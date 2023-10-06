﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace NesGUI
{
    public class OpenGUIWindow : MainButtonWorker
    {
        static bool isOpen = false;
        public override void Activate()
        {
            if (!isOpen)
            {
                Find.WindowStack.Add(new NesGUIWindow());
                isOpen = true;
            }
            else
            {
                if(Find.WindowStack.WindowOfType< NesGUIWindow>()!= null)
                {
                    Find.WindowStack.TryRemove(typeof(NesGUIWindow));
                }
                isOpen = false;
            }
        }
    }

    public static class DebugActionsMisc
    {
        [DebugAction("Mods", null, false, false, allowedGameStates = AllowedGameStates.Entry)]
        private static void ActivateNesGuiWindow()
        {
            Find.WindowStack.Add(new NesGUIWindow());
        }
                
    }

    public class NesGUIWindow : Window
    {
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1500, 700);
            }
        }

        private static Vector2 rectSize = new Vector2(100, 100);
        private static Vector2 rectPos = new Vector2(0, 0);
        private string xSizeBuffer;
        private string ySizeBuffer;
        private string xPosBuffer;
        private string yPosBuffer;

        public List<FloatMenuOption> GetItemOptions()
        {
            List<FloatMenuOption> result = new List<FloatMenuOption>
            {
                new FloatMenuOption("Rectangle", delegate ()
                {
                    Find.WindowStack.Add(new CreateItemWinow(typeof(GUIRect)));
                }),
                new FloatMenuOption("Button", delegate ()
                {
                    Find.WindowStack.Add(new CreateItemWinow(typeof(GUITextElement)));
                }),
                new FloatMenuOption("CheckBox", delegate ()
                {
                    Find.WindowStack.Add(new CreateItemWinow(typeof(GUITextElement),GUITextElement.TextElemType.Checkbox));
                }),
                new FloatMenuOption("Label", delegate ()
                {
                    Find.WindowStack.Add(new CreateItemWinow(typeof(GUITextElement),GUITextElement.TextElemType.Label));
                }),
                /*new FloatMenuOption("Line", delegate ()
                {
                    Find.WindowStack.Add(new CreateItemWinow(GUIType.Line));
                }),*/
                  new FloatMenuOption("Textfield", delegate ()
                {
                    Find.WindowStack.Add(new CreateItemWinow(typeof(GUITextElement),GUITextElement.TextElemType.Textfield));
                })
            };

            return result;
        }


        public List<FloatMenuOption> GetEditableItems()
        {
            List<FloatMenuOption> result = new List<FloatMenuOption>();

            foreach (GUIItem i in GuiMaker.Items)
            {
                result.Add(new FloatMenuOption(i.name, delegate () {
                    Find.WindowStack.Add(new EditableItemWindow(i));
                }));
            }

            return result;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Tiny;
            Rect closeRect = new Rect(new Vector2(inRect.xMax - 25, 5), new Vector2(20, 20));
            if (Widgets.ButtonImage(closeRect, Widgets.CheckboxOffTex))
            {
                Close();
            }

            Widgets.DrawLine(new Vector2(inRect.x, 50), new Vector2(inRect.xMax, 50), Color.white, 1f);
            Rect labelRect = new Rect(new Vector2(inRect.x, 10), new Vector2(50, 40));
            Rect addItemButtonRect = new Rect(new Vector2(labelRect.xMax + 5, 10), new Vector2(80, 40));
            Rect labelSizeRect = new Rect(new Vector2(addItemButtonRect.xMax + 10, 10), new Vector2(100, 40));

            Rect xSizeInputField = new Rect(new Vector2(labelSizeRect.xMax + 5, 10), new Vector2(90, 40));
            Rect ySizeInputField = new Rect(new Vector2(xSizeInputField.xMax + 5, 10), new Vector2(90, 40));

            Rect labelPosRect = new Rect(new Vector2(ySizeInputField.xMax + 5, 10), new Vector2(90, 40));
            Rect xPosInputField = new Rect(new Vector2(labelPosRect.xMax + 5, 10), new Vector2(90, 40));
            Rect yPosInputField = new Rect(new Vector2(xPosInputField.xMax + 5, 10), new Vector2(90, 40));

            Rect changeRectPos = new Rect(new Vector2(yPosInputField.xMax + 5, 10), new Vector2(80, 40));

            

            Rect enableOrDisableDrawnRects = new Rect(new Vector2(changeRectPos.xMax + 10, 10), new Vector2(80, 40));

            Rect exportButtonRect = new Rect(new Vector2(inRect.xMax - 80, 10), new Vector2(40, 40));
            Rect deleteItemRect = new Rect(new Vector2(enableOrDisableDrawnRects.xMax + 10, 10), new Vector2(80, 40));
            bool deleteItem = Widgets.ButtonText(deleteItemRect, "Delete item");

            bool export = Widgets.ButtonText(exportButtonRect, "Export GUI");
            if (export)
            {
                NesGUI_OutputGen.ReadAndWriteGUI();
            }
            
            bool toggleRect = Widgets.ButtonText(enableOrDisableDrawnRects, "Toggle rects");
            

            Widgets.Label(labelRect, "NesGUI \n V 0.0.2A");
            Widgets.Label(labelSizeRect, "Change window size:");
            Widgets.Label(labelPosRect, "Change window pos:");
            bool makeNewItem = Widgets.ButtonText(addItemButtonRect, "New item");
            Utility.NumericScrollWheelField(xSizeInputField, ref rectSize.x, ref xSizeBuffer);
            Utility.NumericScrollWheelField(ySizeInputField, ref rectSize.y, ref ySizeBuffer);

            Utility.NumericScrollWheelField(xPosInputField, ref rectPos.x, ref xPosBuffer);
            Utility.NumericScrollWheelField(yPosInputField, ref rectPos.y, ref yPosBuffer, min: 50);

            bool changePosOfRect = Widgets.ButtonText(changeRectPos, "Edit item");

            
            Widgets.DrawBox(new Rect(rectPos, rectSize), 4);
            if (toggleRect)
            {
                Find.WindowStack.Add(new FloatMenu(ToggleRects()));
            }
            if (makeNewItem)
            {
                Find.WindowStack.Add(new FloatMenu(GetItemOptions()));
            }
            if (changePosOfRect)
            {
                Find.WindowStack.Add(new FloatMenu(GetEditableItems()));
            }
            if (deleteItem)
            {
                Find.WindowStack.Add(new FloatMenu(DeleteItems()));
            }
            foreach(GUIItem item in GuiMaker.Items)
            {
                item.Draw();
            }
        }

        public bool IsEnabled(GUIItem rect)
        {
            return ( !GuiMaker.enabledRects.ContainsKey(rect) || GuiMaker.enabledRects[rect]);
        }
        
        public List<FloatMenuOption > DeleteItems()
        {
            List<FloatMenuOption> res = new List<FloatMenuOption>();

            foreach (GUIItem item in GuiMaker.Items)
            {
                res.Add(new FloatMenuOption($"{item.name}", delegate ()
                {
                    GuiMaker.DeleteItem(item);
                }));
            }

            return res;
        }
        public List<FloatMenuOption> ToggleRects()
        {
            List<FloatMenuOption> res = new List<FloatMenuOption>();

            foreach (GUIItem rect in GuiMaker.Rectangles)
            {
                res.Add(new FloatMenuOption($"{rect.name} {(IsEnabled(rect) ? "(on)":"(off)")}", delegate()
                {
                    if (!GuiMaker.enabledRects.ContainsKey(rect))
                    {
                        GuiMaker.enabledRects.Add(rect, false);
                    }
                    else
                    {
                        GuiMaker.enabledRects[rect] = !GuiMaker.enabledRects[rect];
                    }
                }));
            }

            return res;
        }
    }
}
