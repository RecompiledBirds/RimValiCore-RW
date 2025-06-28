using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace RVCRestructured;

public class VineColorGeneDef :GeneDef
{
    public Color color1;
    public Color color2;
    public Color color3;
    [AllowNull]
    public string channelKey;

    public VineColorGeneDef()
    {
        this.geneClass = typeof(VineColorGene);
    }
}
