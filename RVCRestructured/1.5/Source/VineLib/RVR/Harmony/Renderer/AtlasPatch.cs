using HarmonyLib;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public class RenderTexturePatch
{
    private static readonly int texSize = 8000;

    public static RenderTexture NewTexture()
    {
        Vector2Int size = new(GetAtlasSizeWithPawnsOnMap(), GetAtlasSizeWithPawnsOnMap());
        return new RenderTexture(size.x, size.y, 40, RenderTextureFormat.ARGBFloat, 0)
        {
            antiAliasing = 0,
            useMipMap = true,
            mipMapBias = 50f
        };
    }

    public static int GetAtlasSizeWithPawnsOnMap()
    {

        int pawnCount = Find.CurrentMap.mapPawns.AllPawnsSpawnedCount;
        float texSizeDivider = pawnCount/1024;
        int textureSize = texSize;
        if (texSizeDivider > 1)
        {
            textureSize *= (int)(texSizeDivider);
            return textureSize > 4096 ? textureSize : 4096;
        }
        return texSize;
    }
}


public static class RenderTextureTranspiler
{
    public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
    {
        List<CodeInstruction> codes = instructions.ToList();
        int cont = instructions.Count();
        for (int index = 0; index < cont; index++)
        {
            if (VerifyLocation(codes, index))
            {
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RenderTexturePatch),
                                                                                  nameof(RenderTexturePatch.NewTexture)));
                index += 5;
            }
            else
            {
                yield return codes[index];
            }
        }
    }

    private static bool VerifyLocation(List<CodeInstruction> codes, int i)
    {
        return i < codes.Count - 5 &&
               codes[i].opcode == OpCodes.Ldc_I4 && (int)codes[i].operand == 0x800 &&
               codes[i + 1].opcode == OpCodes.Ldc_I4 && (int)codes[i + 1].operand == 0x800 &&
               codes[i + 2].opcode == OpCodes.Ldc_I4_S &&
               codes[i + 3].opcode == OpCodes.Ldc_I4_0 &&
               codes[i + 4].opcode == OpCodes.Ldc_I4_0 &&
               codes[i + 5].opcode == OpCodes.Newobj;
    }
}
