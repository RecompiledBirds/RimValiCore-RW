using System.Text;
using UnityEngine;
using Verse;

namespace NesGUI;

public static class NesGUI_OutputGen
{
    static readonly StringBuilder program = new();
    public static void ReadRects()
    {
        int rects = 0;
        foreach(GUIRect item in GuiMaker.Rectangles)
        {
            string varName = item.name;
            varName = new string(varName.ToCharArray().Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            
            program.AppendLine($"Rect {varName} = new Rect(new Vector2({item.pos.x}f,{item.pos.y}f),new Vector2({item.size.x}f,{item.size.y}f));");
            rects++;
        }
        Log.Message($"Read {rects} rects.");
    }

    public static void ReadButtons()
    {
        int buttons = 0;
        foreach (GUIItem button in GuiMaker.Buttons)
        {
            string rectName = button.parent.name;
            rectName= new string(rectName.ToCharArray().Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            string varName = button.name;
            varName = new string(varName.ToCharArray().Where(ch=>!char.IsWhiteSpace(ch)).ToArray());


            GUITextElement elem = (GUITextElement)button;
            string fontName = elem.GetGameFont.ToString();
            string anchorName = elem.GetTextAnchor.ToString();
            program.AppendLine($"Text.Font = GameFont.{fontName};");
            program.AppendLine($"Text.Anchor = TextAnchor.{anchorName};");

            program.AppendLine();
            program.AppendLine($"bool {varName} = Widgets.ButtonText({rectName},\"{button.label}\");");
            program.AppendLine();

            program.AppendLine($"Text.Font = prevFont;");
            program.AppendLine($"Text.Anchor = textAnchor;");
            buttons++;
        }
        Log.Message($"Read {buttons} buttons.");
    }



    public static void ReadLabels()
    {
        int labels = 0;
        foreach (GUIItem label in GuiMaker.Labels)
        {
            string rectName = label.parent.name;
            rectName = new string(rectName.ToCharArray().Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            string varName = label.name;
            varName = new string(varName.ToCharArray().Where(ch => !char.IsWhiteSpace(ch)).ToArray());

            GUITextElement elem = (GUITextElement)label;

            string fontName = elem.GetGameFont.ToString();
            string anchorName = elem.GetTextAnchor.ToString();

            program.AppendLine($"Text.Font = GameFont.{fontName};");
            program.AppendLine($"Text.Anchor = TextAnchor.{anchorName};");
            
            program.AppendLine();
            
            program.AppendLine($"Widgets.Label({rectName},\"{label.label}\");");
            
            program.AppendLine();

            program.AppendLine($"Text.Font = prevFont;");
            program.AppendLine($"Text.Anchor = textAnchor;");
            labels++;
        }
        Log.Message($"Read {labels} labels.");
    }


    public static void ReadTextfields()
    {
        int tf = 0;
        foreach (GUIItem field in GuiMaker.Textfields)
        {
            string rectName = field.parent.name;
            rectName = new string(rectName.ToCharArray().Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            string varName = field.name;
            varName = new string(varName.ToCharArray().Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            //Get current font & anchor
            GUITextElement elem = (GUITextElement)field;
            
           
            string fontName = elem.GetGameFont.ToString();
            string anchorName = elem.GetTextAnchor.ToString();

            //Set font & anchor
            program.AppendLine($"Text.Font = GameFont.{fontName};");
            program.AppendLine($"Text.Anchor = TextAnchor.{anchorName};");
            
            program.AppendLine();
            
            program.AppendLine($"string {varName};");
            program.AppendLine($"{varName} = Widgets.TextField({rectName},{varName});");
            program.AppendLine();
            //Reset font & anchor

            program.AppendLine($"Text.Font = prevFont;");
            program.AppendLine($"Text.Anchor = textAnchor;");
            tf++;
        }
        Log.Message($"Read {tf} textfields.");
    }



    public static void ReadCheckBoxes()
    {
        int box = 0;
        foreach (GUIItem checkbox in GuiMaker.Checkboxes)
        {
            string rectName = checkbox.parent.name;
            rectName = new string(rectName.ToCharArray().Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            string varName = checkbox.name;
            varName = new string(varName.ToCharArray().Where(ch => !char.IsWhiteSpace(ch)).ToArray());

            GUITextElement elem = (GUITextElement)checkbox;
            string fontName = elem.GetGameFont.ToString();
            string anchorName = elem.GetTextAnchor.ToString();

            program.AppendLine($"Text.Font = GameFont.{fontName};");
            program.AppendLine($"Text.Anchor = TextAnchor.{anchorName};");

            program.AppendLine();
            program.AppendLine($"bool {varName} = false;");
            program.AppendLine($" Widgets.CheckboxLabeled({rectName},\"{checkbox.label}\",ref {varName});");
            program.AppendLine();

            program.AppendLine($"Text.Font = prevFont;");
            program.AppendLine($"Text.Anchor = textAnchor;");
            box++;
        }
        Log.Message($"Read {box} boxes.");
    }

    static string path;
    public static void ReadAndWriteGUI()
    {
        path = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\"));
        path = $"{path}NesGUI/Output";
        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
        path += "/output.txt";
        program.AppendLine("//COMPILED BY NESGUI");
        program.AppendLine("//Prepare varibles");
        program.AppendLine();
        program.AppendLine("GameFont prevFont = Text.Font;");
        program.AppendLine("TextAnchor textAnchor = Text.Anchor;");
        program.AppendLine();
        program.AppendLine("//Rect pass");
        program.AppendLine();
        ReadRects();
        program.AppendLine();
        program.AppendLine("//Button pass");
        program.AppendLine();
        ReadButtons();
        program.AppendLine();
        program.AppendLine("//Checkbox pass");
        program.AppendLine();
        ReadCheckBoxes();
        program.AppendLine();
        program.AppendLine("//Label pass");
        program.AppendLine();
        ReadLabels();
        program.AppendLine();
        program.AppendLine("//Textfield pass");
        program.AppendLine();
        ReadTextfields();
        program.AppendLine();
        program.AppendLine("//END NESGUI CODE");

        Log.Error($"Hey! This isn't an error. Just wanted to say:\n Wrote code file to: {path}");
        File.WriteAllText(path, program.ToString());
        program.Clear();
    }
}
