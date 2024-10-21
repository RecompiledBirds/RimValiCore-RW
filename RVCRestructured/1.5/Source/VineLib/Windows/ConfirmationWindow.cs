using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RVCRestructured.Windows;

public class ConfirmationWindow : Window
{
    private readonly Action acceptAction;
    private readonly Action cancelAction;
    private readonly string question;

    private Vector2 scrollVec;

    public override Vector2 InitialSize => new(450f, 150f);

    public ConfirmationWindow(Action acceptAction, Action cancelAction, string question, string title)
    {
        optionalTitle = title;
        onlyOneOfTypeAllowed = true;
        forcePause = true;
        doCloseX = true;

        this.acceptAction = acceptAction;
        this.cancelAction = cancelAction;
        this.question = question;
    }

    public override void DoWindowContents(Rect inRect)
    {
        Rect descriptionRect = inRect.TopPartPixels(inRect.height * 0.7f - Margin).Rounded();
        Rect buttonsRect = new Rect(descriptionRect.x, descriptionRect.yMax + Margin, descriptionRect.width, inRect.height * 0.3f).Rounded();
        Rect buttonAccept = new Rect(buttonsRect.LeftPartPixels((buttonsRect.width - Margin) / 2f)).Rounded();
        Rect buttonCancel = new Rect(buttonsRect.RightPartPixels((buttonsRect.width - Margin) / 2f)).Rounded();

        Widgets.LabelScrollable(descriptionRect, question, ref scrollVec);

        if (Widgets.ButtonText(buttonAccept, "RVC_Accept".Translate()))
        {
            SoundDefOf.Click.PlayOneShotOnCamera();
            acceptAction();
            Close();
        }

        if (Widgets.ButtonText(buttonCancel, "RVC_Cancel".Translate()))
        {
            SoundDefOf.ClickReject.PlayOneShotOnCamera();
            cancelAction();
            Close();
        }
    }
}
