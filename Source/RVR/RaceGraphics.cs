using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR
{
    public class RaceGraphics
    {
        public bool hasHair = false;
        public List<RaceColors> colorGenerators = new List<RaceColors>();
        public string skinColorSet;

        public string bodyTex;
        public string headTex;
        public string skeleton = "Things/Pawn/Humanlike/HumanoidDessicated";
        public string skull = "Things/Pawn/Humanlike/Heads/None_Average_Skull";
        public string stump = "Things/Pawn/Humanlike/Heads/None_Average_Stump";
        public Vector2 headSize;
        public Vector2 bodySize = new Vector2(1f, 1f);

        public RaceColors GetSkinColorGenerator
        {
            get
            {
                //If none is defined, use generator 0
                if (skinColorSet == null)
                    return colorGenerators[0];

                for(int i = 0; i<colorGenerators.Count; i++)
                {
                    if (colorGenerators[i].name==skinColorSet)
                        return colorGenerators[i];
                }

                //If we can't find any that match, log an error and create a temporary/debug color generator.
                Log.Error($"Could not find a color generator named {skinColorSet}.");
                RaceColors debugColors = new RaceColors()
                {
                    name = skinColorSet,
                    colorGenerator = new TriColorGenerator()
                    {
                        colorOne = new ColorGenerator_Single() { color = Color.red },
                        colorThree = new ColorGenerator_Single() { color = Color.green },
                        colorTwo = new ColorGenerator_Single() { color = Color.blue }
                    }
                };
                colorGenerators.Add(debugColors);
                return debugColors;
            }
        }
    }
}
