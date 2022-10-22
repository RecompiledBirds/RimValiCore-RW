using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.Graphics
{
    [StaticConstructorOnStartup]
    public class RVG_GraphicDataBase
    {
        private static readonly Dictionary<RVG_GraphicRequest, RVG_Graphic> graphics = new Dictionary<RVG_GraphicRequest, RVG_Graphic>();

        private static T GetInner<T>(RVG_GraphicRequest req) where T : RVG_Graphic, new()
        {
            if(!graphics.TryGetValue(req,out RVG_Graphic graphic))
            {
                graphic = new T();
                graphic.Init(req);
                graphics.Add(req, graphic);
            }
            return (T)graphic;
        }

        public static RVG_Graphic Get<T>(string path, string maskPath=null) where T: RVG_Graphic, new()
        {
            return GetInner<T>(new RVG_GraphicRequest(typeof(T), path, TriColorShader.tricolorshader, Vector2.one, Color.white, Color.white, Color.white, null, 0, null, maskPath));
        }

        public static RVG_Graphic Get<T>(string path,Vector2 drawSize, string maskPath = null) where T : RVG_Graphic, new()
        {
            return GetInner<T>(new RVG_GraphicRequest(typeof(T), path, TriColorShader.tricolorshader, drawSize, Color.white, Color.white, Color.white, null, 0, null, maskPath));
        }

        public static RVG_Graphic Get<T>(string path, Vector2 drawSize, Color colorOne, Color colorTwo, Color colorThree,string maskPath = null) where T : RVG_Graphic, new()
        {
            return GetInner<T>(new RVG_GraphicRequest(typeof(T), path, TriColorShader.tricolorshader, drawSize, colorOne, colorTwo, colorThree, null, 0, null, maskPath));
        }

        public static RVG_Graphic Get(Type graphicClass, string path, Vector2 drawSize, Color colorOne, Color colorTwo, Color colorThree, string maskPath = null)
        {
            return Get(graphicClass, path, drawSize, colorOne, colorTwo, colorThree,null, maskPath);
        }

        public static RVG_Graphic Get(Type graphicClass, string path, Vector2 drawSize, Color colorOne, Color colorTwo, Color colorThree,RVG_GraphicData data, string maskPath = null)
        {
            RVG_GraphicRequest req = new RVG_GraphicRequest(graphicClass, path, TriColorShader.tricolorshader, drawSize, colorOne, colorTwo, colorThree, data, 0, null, maskPath);


            if(req.graphicClass==typeof(Graphic_Multi)|| req.graphicClass == typeof(RVG_Graphic_Multi))
            {

            }

            return RVGBaseContent.BadGraphic;
        }
    }
}
