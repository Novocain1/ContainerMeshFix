using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ContainerMeshFix
{
    [HarmonyPatch(typeof(BlockLiquidContainerTopOpened), "GenMesh", new Type[] { typeof(ICoreClientAPI), typeof(ItemStack), typeof(BlockPos) })]
    internal class LCGenMeshFix
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Stack<CodeInstruction> instructionsStack = new Stack<CodeInstruction>();

            var decMethod = AccessTools.GetDeclaredMethods(typeof(ITesselatorAPI))
                .Where(m => m.Name == "TesselateShape" && m.GetParameters().Types().Contains(typeof(CollectibleObject)))
                .Single();

            var proxyMethod = AccessTools.Method(typeof(ProxyMethods), "TesslateShape");

            foreach (var inst in instructions)
            {
                if (inst.Calls(decMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, proxyMethod);

                    continue;
                }
                yield return inst;
            }
        }
    }
}
