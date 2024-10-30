using RVCRestructured.Defs;
using Verse;

namespace RVCRestructured;

public interface IRenderable
{
    bool CanDisplay(Pawn pawn, bool portrait=false);
    string GetTexPath(Pawn pawn);
    string GetMaskPath(Pawn pawn);
    bool ShowsInBed();
    TriColorSet ColorSet(RVRComp comp);
    TriColorSet ColorSet(Pawn pawn);
    BodyPartGraphicPos GetPos(Rot4 rotation);
    BodyPartGraphicPos GetPos(Rot4 rotation, PawnRenderTree tree, bool inBed,bool portrait=false);

}
