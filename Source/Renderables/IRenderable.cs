using RVCRestructured.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public interface IRenderable
    {
        bool CanDisplay(Pawn pawn, bool portrait=false);
        string GetTexPath(Pawn pawn);
        string GetMaskPath(Pawn pawn);
        bool ShowsInBed();
        TriColorSet ColorSet(Pawn pawn);
        BodyPartGraphicPos GetPos(Rot4 rotation);

    }
}
