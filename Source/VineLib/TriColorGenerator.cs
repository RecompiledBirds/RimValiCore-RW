using Verse;

namespace RVCRestructured
{
    public class TriColorGenerator
    {
        public ColorGenerator colorOne;
        public ColorGenerator colorTwo;
        public ColorGenerator colorThree;
        public bool dyeable = true;
        public TriColorSet GenerateColors()
        {
            //Make sure we have no null generators and fill what we can, if there are.
            if (colorOne == null)
                Log.Error("colorOne in ColorGenerator cannot be left empty!");
            if (colorTwo == null)
                colorTwo = colorOne;
            if (colorThree == null)
                colorThree = colorOne;

            return new TriColorSet(colorOne.NewRandomizedColor(), colorTwo.NewRandomizedColor(), colorThree.NewRandomizedColor(), dyeable);
        }
    }

}
