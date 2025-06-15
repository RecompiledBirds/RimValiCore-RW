using UnityEngine;
using Verse;

namespace NesGUI;

public class GuiMaker
{
    // Gui item list name, guiitems
    static readonly Dictionary<string, List<GUIItem>> guiItems = [];
    //TODO: Convert lists to Dictionary<string, GUIItem> and condense.
    public static List<GUIItem> Items
    {
        get
        {
            List<GUIItem> res = [];
            foreach(string key in guiItems.Keys)
            {
                res.AddRange(guiItems[key]);
            }
            return res;
        }
    }
    public static List<GUIItem> Buttons
    {
        get
        {
            if (!guiItems.ContainsKey("buttons"))
                return [];

            return guiItems["buttons"];
        }
    }

    public static List<GUIItem> Rectangles
    {
        get
        {

            if (!guiItems.ContainsKey("rects"))
                return []; 
            return guiItems["rects"];
        }
    }
    public static List<GUIItem> Labels
    {
        get
        {
            
            if (!guiItems.ContainsKey("labels"))
                return []; ;

            return guiItems["labels"];
        }
    }

    public static List<GUIItem> Checkboxes
    {
        get
        {
            if (!guiItems.ContainsKey("checkboxes"))
                return []; 

            return guiItems["checkboxes"];
        }
    }
    public static List<GUIItem> lines = [];
    public static List<GUIItem> Textfields
    {
        get
        {
        
            if (!guiItems.ContainsKey("textfields"))
                return [];

            return guiItems["textfields"];
        }
    }
    public static Dictionary<GUIItem, bool> enabledRects = [];


    public static void MakeRect(Vector2 size, Vector2 pos, string name)
    {
        GUIRect GI = new(pos,size, name);
        AddItem(GI, "rects");
    }

    public static void AddItem(GUIItem item, string targ)
    {
        if (!guiItems.ContainsKey(targ))
        {
            guiItems.Add(targ, []);
        }
        guiItems[targ].Add(item);
    }

    public static void MakeButton(GUIRect rect, string label, TextAnchor anchor, GameFont font)
    {
        GUITextElement GI = new(rect, label, label, anchor, font);
        AddItem(GI, "buttons");
    }
    public static void MakeTextField(GUIRect rect, string label,TextAnchor anchor, GameFont font)
    {
        GUITextElement GI = new(rect, label, label,anchor,font, GUITextElement.TextElemType.Textfield);
        AddItem(GI, "textfields");
    }


    public static void MakeLabel(GUIRect rect, string label, TextAnchor anchor, GameFont font)
    {
        GUITextElement GI = new(rect,label,label, anchor, font, GUITextElement.TextElemType.Label);
        AddItem(GI, "labels");
    }


    public static void MakeCheckBox(GUIRect rect, string label, TextAnchor anchor, GameFont font)
    {
        GUITextElement GI = new(rect, label, label, anchor, font, GUITextElement.TextElemType.Checkbox);
        AddItem(GI, "checkboxes");
    }

    public static void Remove(GUIItem GI, string targ)
    {
        if (guiItems.ContainsKey(targ) && guiItems[targ].Contains(GI))
            guiItems[targ].Remove(GI);
    }

    public static void DeleteItem(GUIItem item)
    {
        item.Delete();
    }
}
