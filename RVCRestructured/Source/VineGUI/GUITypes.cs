using UnityEngine;

namespace NesGUI;

public class GUILine : GUIItem
{

}

public class GUITextElement : GUIItem
{
    GameFont font;
    TextAnchor anchor;
    bool placeholder;

    public TextAnchor GetTextAnchor
    {
        get
        {
            return anchor;
        }
    }
    public GameFont GetGameFont
    {
        get
        {
            return font;
        }
    }

    public void SetFont(GameFont font)
    {
        this.font = font;
    }
    public void SetAnchor(TextAnchor anchor)
    {
        this.anchor = anchor;
    }
    public enum TextElemType
    {
        Button,
        Label,
        Textfield,
        Checkbox
    }
    readonly TextElemType type;
    public GUITextElement(GUIRect rect, string label, string name, TextAnchor anchor, GameFont font, TextElemType type = TextElemType.Button)
    {
        Parent = rect;
        this.Label = label;
        this.Name = name;
        this.type = type;
        this.font = font;
        this.anchor = anchor;
    }
    public override void Draw()
    {
        TextAnchor prevAnchor = Text.Anchor;
        GameFont prevFont = Text.Font;
        Text.Anchor = anchor;
        Text.Font = font;
        if (TextElemType.Button == type)
        {
            Widgets.ButtonText(GetRectWithOffset, Label);
        }
        else if (TextElemType.Label == type)
        {
            Widgets.Label(GetRectWithOffset, Label);
        }
        else if (TextElemType.Textfield == type)
        {
            Label = Widgets.TextField(GetRectWithOffset, Label);
        }
        else
        {
            Widgets.CheckboxLabeled(GetRectWithOffset, Label, ref placeholder);
        }
        Text.Anchor = prevAnchor;
        Text.Font = prevFont;
    }
    public override void Delete()
    {

        GuiMaker.Remove(this, type == TextElemType.Button ? "buttons" : type == TextElemType.Label ? "labels" : type == TextElemType.Checkbox ? "checkboxes" : "textfields");

        base.Delete();
    }
}

public class GUIRect : GUIItem
{
    public Vector2 size;
    public Vector2 pos;
    public Rect rect;
    public List<GUIItem> Children
    {
        get
        {
            return GuiMaker.Items.Where(x => x.Parent == this).ToList();
        }
    }
    public GUIRect(Vector2 pos, Vector2 size, string name)
    {
        this.Name = name;
        this.size = size;
        this.pos = pos;
        rect = new Rect(pos, size);
    }

    public void UpdateRect()
    {

        rect = new Rect(pos, size);
    }

    public override void Draw()
    {
        if (!GuiMaker.enabledRects.ContainsKey(this))
        {
            Rect r = new(new Vector2(pos.x, pos.y + 50), rect.size);

            Widgets.DrawBoxSolidWithOutline(r, Color.clear, Color.red);
            Widgets.Label(r, Name);
        }
        else if (GuiMaker.enabledRects[this])
        {
            Rect r = new(new Vector2(pos.x, pos.y + 50), rect.size);

            Widgets.DrawBoxSolidWithOutline(r, Color.clear, Color.red);
            Widgets.Label(r, Name);
        }
        base.Draw();
    }
    public override void Delete()
    {
        GuiMaker.Remove(this, "rects");
        foreach (GUIItem child in Children)
        {
            child.Delete();
        }
        base.Delete();
    }
}

//TODO: Seperate GUITypes into seperate types, inheriting from GUIItem
public class GUIItem
{
    private GUIRect? parent = null;

    public GUIRect Parent
    {
        get => parent ?? throw new NullReferenceException();
        set => parent = value;
    }

    public Vector2 posOne;
    public Vector2 posTwo;
    public Color color;

    public virtual void Draw()
    {

    }

    public virtual void Delete()
    {

    }

    public Rect GetRectWithOffset => new(new Vector2(GetRect.position.x, GetRect.position.y + 50), GetRect.size);
    public Rect GetRect => Parent.rect;

    public Vector2 Size
    {
        get => Parent.size;
        set => Parent.size = value;
    }

    public Vector2 Pos
    {
        get => Parent.pos;
        set => Parent.pos = value;
    }

    public string Name 
    { 
        get => name ?? throw new NullReferenceException(); 
        set => name = value; 
    }

    public string Label 
    { 
        get => label ?? throw new NullReferenceException(); 
        set => label = value; 
    }

    private string? label;
    private string? name;
}
