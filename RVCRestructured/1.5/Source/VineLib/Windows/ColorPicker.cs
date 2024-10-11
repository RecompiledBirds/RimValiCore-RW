using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace RVCRestructured.Windows;

public class ColorPickerWindow : Window
{
    private const int CREATED_BOXES = 6;
    private const int COLOR_COMP_HEIGHT = 200;
    private const int HUE_BAR_WIDTH = 20;

    private const int HISTORY_COLS = 5;
    private const int HISTORY_ROWS = 2;

    private readonly string[] colorBuffers = ["255", "255", "255"];
    private readonly Color[] colorHistory;

    private readonly Regex hexRx = new(@"#[a-f0-9]{6}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Texture2D hueBarTexture = new(1, COLOR_COMP_HEIGHT);

    private readonly Rect rectColorInput;

    private readonly List<Rect> rectColorInputBoxes;
    private readonly Rect rectFull = new(0f, 0f, 600f, 300f);
    private readonly Rect[,] rectHistoryArray;
    private readonly Rect rectHistoryMain;
    private readonly Rect rectHueBar;
    private readonly Rect rectMain;
    private readonly List<Rect> rectRGBInputBoxes;

    private readonly int[] rectRGBValues = [0, 0, 0];
    private readonly Rect rectSaturationValueSquare;
    private bool hexChanged = true;
    private string hexCode = "#FFFFFF";
    private float hue;
    private bool keepTrackingMouseHue;

    private readonly Action<Color> setColor;
    private readonly Action<Color[]> setColorHistory;

    private bool keepTrackingMouseSaturation;
    private float oldHue = 1f;

    private Color selectedColor = Color.red;
    private Texture2D texture;

    public ColorPickerWindow(Action<Color> setColor, Action<Color[]> setColorHistory, Color color, Color[] colorHistory)
    {
        this.setColor = setColor;
        this.setColorHistory = setColorHistory;
        SelectedColor = color;

        closeOnClickedOutside = true;
        forcePause = true;
        preventCameraMotion = true;
        onlyOneOfTypeAllowed = true;

        rectMain = rectFull.ContractedBy(25f);
        rectSaturationValueSquare = new Rect(rectMain.position, new Vector2(COLOR_COMP_HEIGHT, COLOR_COMP_HEIGHT));
        rectHueBar = rectSaturationValueSquare.MoveRect(new Vector2(rectSaturationValueSquare.width + 10f, 0f)).LeftPartPixels(HUE_BAR_WIDTH);
        rectColorInput = rectHueBar.MoveRect(new Vector2(rectHueBar.width + 10f, 0f));
        rectColorInput.size = new Vector2(rectMain.width - rectColorInput.position.x + 25f, rectSaturationValueSquare.height);
        rectColorInputBoxes = rectColorInput.DivideVertical(CREATED_BOXES).ToList();
        rectRGBInputBoxes = rectColorInputBoxes[3].DivideHorizontal(3).ToList();
        rectHistoryMain = new Rect(rectMain.position.x, COLOR_COMP_HEIGHT + 25f, rectMain.width, rectMain.height - COLOR_COMP_HEIGHT);
        rectHistoryArray = new Rect[HISTORY_ROWS, HISTORY_COLS];
        this.colorHistory = new Color[HISTORY_COLS * HISTORY_ROWS];

        Rect[] historyRowRects = rectHistoryMain.DivideVertical(HISTORY_ROWS).ToArray();
        for (int i = 0; i < HISTORY_ROWS; i++)
        {
            Rect[] historyColumnRects = historyRowRects[i].DivideHorizontal(HISTORY_COLS).ToArray();
            for (int j = 0; j < HISTORY_COLS; j++)
            {
                rectHistoryArray[i, j] = historyColumnRects[j];
                this.colorHistory[j + i * HISTORY_COLS] = Color.black;
            }
        }

        for (int i = 0; i < colorHistory.Length; i++)
        {
            this.colorHistory[i] = colorHistory[i];
        }

        for (int y = 0; y < COLOR_COMP_HEIGHT; y++)
        {
            hueBarTexture.SetPixel(0, y, Color.HSVToRGB((float)y / COLOR_COMP_HEIGHT, 1, 1));
        }

        hueBarTexture.Apply();

        RefreshSaturationTexture();
    }

    private Color SelectedColor
    {
        get => selectedColor;
        set
        {
            selectedColor = value;
            setColor(SelectedColor);
            UpdateColor();
        }
    }

    public static Color LastSelectedColor { get; private set; }

    public override Vector2 InitialSize => rectFull.size;

    protected override float Margin => 0f;

    private void UpdateColor()
    {
        hexCode = "#";

        for (int i = 0; i < 3; i++)
        {
            rectRGBValues[i] = (int)(SelectedColor[i] * 255);
            colorBuffers[i] = ((int)(SelectedColor[i] * 255)).ToString();
            hexCode += ((int)(SelectedColor[i] * 255)).ToString("X2");
        }

        //If gray, don't update hue => gray tones don't have a hue and would lose the user input
        if (SelectedColor.r != SelectedColor.g || SelectedColor.g != selectedColor.b)
        {
            Color.RGBToHSV(SelectedColor, out hue, out _, out _);
            RefreshSaturationTexture();
        }
    }

    private void InsertColorInHistory(Color color)
    {
        if (colorHistory.Contains(color))
        {
            for (int i = colorHistory.FirstIndexOf(val => val == color); i >= 1; i--)
            {
                colorHistory[i] = colorHistory[i - 1];
            }
        }
        else
        {
            for (int i = colorHistory.Length - 1; i >= 1; i--)
            {
                colorHistory[i] = colorHistory[i - 1];
            }
        }

        colorHistory[0] = color;

        setColorHistory(colorHistory);
    }

    public override void DoWindowContents(Rect inRect)
    {
        DrawCloseButton(inRect);

        DrawSaturationValueSquare();
        DrawHueBar();

        Widgets.DrawBox(rectMain);
        Widgets.DrawBox(rectHistoryMain);

        DrawColorHistoryButtons();
        DrawInputFieldLabels();
        DrawHexCodeInputField();
        DrawRGBInputValues();
        DrawControls();

        //Color Preview
        Widgets.DrawBoxSolid(rectColorInputBoxes[4].ContractedBy(5f), Color.black);
        Widgets.DrawBoxSolid(rectColorInputBoxes[4].ContractedBy(10f), SelectedColor);

        //Save color button
        if (Widgets.ButtonText(rectColorInputBoxes[5].ContractedBy(5f), "CPW_SaveColor".Translate()))
        {
            InsertColorInHistory(SelectedColor);
        }
    }

    private void DrawColorHistoryButtons()
    {
        for (int y = 0; y < HISTORY_ROWS; y++)
        {
            for (int x = 0; x < HISTORY_COLS; x++)
            {
                Rect historyRect = rectHistoryArray[y, x];

                Widgets.DrawBoxSolidWithOutline(historyRect, colorHistory[x + y * HISTORY_COLS], new Color(255f, 255f, 255f, 0.5f), 2);
                if (Widgets.ButtonInvisible(historyRect))
                {
                    SelectedColor = colorHistory[x + y * HISTORY_COLS];
                    InsertColorInHistory(SelectedColor);
                }
            }
        }
    }

    private void DrawControls()
    {
        Color.RGBToHSV(SelectedColor, out float _, out float saturation, out float value);

        // Cross-hair
        Rect verticalLine = new(0f, (int)(COLOR_COMP_HEIGHT - value * COLOR_COMP_HEIGHT - 2f), COLOR_COMP_HEIGHT, 3f);
        Rect horizontalLine = new((int)(saturation * COLOR_COMP_HEIGHT - 2f), 0f, 3f, COLOR_COMP_HEIGHT);

        GUI.BeginGroup(rectSaturationValueSquare);

        GUI.color = Color.gray;
        Widgets.DrawBox(verticalLine);
        Widgets.DrawBox(horizontalLine);
        GUI.color = Color.white;

        Widgets.DrawBoxSolid(verticalLine.ContractedBy(1), Color.black);
        Widgets.DrawBoxSolid(horizontalLine.ContractedBy(1), Color.black);

        GUI.EndGroup();

        //HueLine
        Rect hueLine = new(0f, (int)(rectHueBar.height - rectHueBar.height * hue - 2f), rectHueBar.width, 3);
        GUI.BeginGroup(rectHueBar);

        GUI.color = Color.gray;
        Widgets.DrawBox(hueLine);
        GUI.color = Color.white;

        Widgets.DrawBoxSolid(hueLine.ContractedBy(1), Color.black);

        GUI.EndGroup();
    }

    private void DrawHueBar()
    {
        GUI.DrawTexture(rectHueBar, hueBarTexture);

        if ((Mouse.IsOver(rectHueBar) || keepTrackingMouseHue) && Input.GetMouseButton(0) && !keepTrackingMouseSaturation)
        {
            keepTrackingMouseHue = true;
            Vector2 mousePositionInRect = Event.current.mousePosition - rectHueBar.position;

            mousePositionInRect.x = Mathf.Clamp(mousePositionInRect.x, 0f, rectHueBar.width);
            mousePositionInRect.y = Mathf.Clamp(mousePositionInRect.y, 0f, rectHueBar.height);

            Color.RGBToHSV(SelectedColor, out float _, out float saturation, out float value);
            hue = 1f - mousePositionInRect.y / rectHueBar.height;

            SelectedColor = Color.HSVToRGB(hue, saturation, value);
            RefreshSaturationTexture();
        }

        keepTrackingMouseHue = keepTrackingMouseHue && Input.GetMouseButton(0);
    }

    private void DrawSaturationValueSquare()
    {
        GUI.DrawTexture(rectSaturationValueSquare, texture);

        if ((Mouse.IsOver(rectSaturationValueSquare) || keepTrackingMouseSaturation) && Input.GetMouseButton(0) && !keepTrackingMouseHue)
        {
            keepTrackingMouseSaturation = true;
            Vector2 mousePositionInRect = Event.current.mousePosition - rectSaturationValueSquare.position;

            mousePositionInRect.x = Mathf.Clamp(mousePositionInRect.x, 0f, COLOR_COMP_HEIGHT);
            mousePositionInRect.y = Mathf.Clamp(mousePositionInRect.y, 0f, COLOR_COMP_HEIGHT);

            SelectedColor = Color.HSVToRGB(hue, mousePositionInRect.x / COLOR_COMP_HEIGHT, 1f - mousePositionInRect.y / COLOR_COMP_HEIGHT);
        }

        keepTrackingMouseSaturation = keepTrackingMouseSaturation && Input.GetMouseButton(0);
    }

    private void RefreshSaturationTexture()
    {
        if (hue == oldHue) return;

        oldHue = hue;
        Texture2D newTexture = new(COLOR_COMP_HEIGHT, COLOR_COMP_HEIGHT)
        {
            wrapMode = TextureWrapMode.Clamp
        };

        Color[] colors = new Color[COLOR_COMP_HEIGHT * COLOR_COMP_HEIGHT];
        for (int x = 0; x < COLOR_COMP_HEIGHT; x++)
        {
            for (int y = 0; y < COLOR_COMP_HEIGHT; y++)
            {
                colors[x + y * COLOR_COMP_HEIGHT] = Color.HSVToRGB(hue, (float)x / COLOR_COMP_HEIGHT, (float)y / COLOR_COMP_HEIGHT);
            }
        }

        newTexture.SetPixels(colors);
        newTexture.Apply();
        texture = newTexture;
    }

    /// <summary>
    ///     Draws the input field labels
    /// </summary>
    private void DrawInputFieldLabels()
    {
        Text.Anchor = TextAnchor.MiddleCenter;
        Text.Font = GameFont.Medium;
        Widgets.Label(rectColorInputBoxes[0], "CPW_HEX".Translate());
        Widgets.Label(rectColorInputBoxes[2], "CPW_RGB".Translate());
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;
    }

    /// <summary>
    ///     Creates the <see cref="hexCode" /> value input field
    ///     Changes the <see cref="rectRGBValues" /> when a new value is inputted
    /// </summary>
    private void DrawHexCodeInputField()
    {
        if (!hexRx.IsMatch(hexCode)) //Mark the field red if there is an error
        {
            GUI.color = Color.red;
        }
        else if (hexChanged) //Only changes if the hex code is legal
        {
            int num = int.Parse(hexCode.Substring(1), NumberStyles.AllowHexSpecifier);

            float b = (num & 0xFF) / 255f;
            float g = ((num >> 8) & 0xFF) / 255f;
            float r = ((num >> 16) & 0xFF) / 255f;

            SelectedColor = new Color(r, g, b);

            hexChanged = false;
        }

        string hexBefore = hexCode;
        hexCode = Widgets.TextField(rectColorInputBoxes[1].ContractedBy(5f), hexCode);
        hexChanged = !hexBefore.Equals(hexCode) || hexChanged;
        GUI.color = Color.white;

        //Checks if a hex code starts with the # char and sets it if it's missing
        if (!hexCode.StartsWith("#"))
        {
            hexCode = $"#{hexCode}";

            //Fixes the # char being moved to the third position if someone writes before it
            if (hexCode.Length >= 3 && hexCode[2].Equals('#'))
            {
                hexCode = $"{hexCode.Substring(0, 2)}{(hexCode.Length >= 3 ? hexCode.Substring(3) : string.Empty)}";
            }
        }
    }

    /// <summary>
    ///     Creates the RGB value input fields and stores the inputs inside <see cref="rectRGBValues" />
    ///     Changes the <see cref="hexCode" /> when new values are inputted
    /// </summary>
    private void DrawRGBInputValues()
    {
        bool rgbChanged = false;

        //Creates the RGB value inputs and handles them
        for (int i = 0; i < 3; i++)
        {
            int value = rectRGBValues[i];
            string colorBuffer = colorBuffers[i];

            Widgets.TextFieldNumeric(rectRGBInputBoxes[i].ContractedBy(5f), ref value, ref colorBuffer, 0f, 255f);

            rgbChanged = rgbChanged || value != rectRGBValues[i];

            rectRGBValues[i] = value;
            colorBuffers[i] = colorBuffer;
        }

        if (rgbChanged)
        {
            SelectedColor = new Color(rectRGBValues[0] / 255f, rectRGBValues[1] / 255f, rectRGBValues[2] / 255f);
        }
    }

    private void DrawCloseButton(Rect inRect)
    {
        if (Widgets.CloseButtonFor(inRect)) Close();
    }

    public override void PostClose()
    {
        LastSelectedColor = SelectedColor;
        base.PostClose();
    }
}
