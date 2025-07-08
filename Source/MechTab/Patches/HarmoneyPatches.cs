using System.Reflection;
using System.Runtime;
using HarmonyLib;
using MechTab.Source;
using Verse;

namespace SpaceMoth
{
	[StaticConstructorOnStartup]
	static public class HarmonyPatches
	{
		public static Harmony harmonyInstance;
		
		static HarmonyPatches()
		{

			harmonyInstance = new Harmony("SpaceMoth.MechTab");
			harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
		}
	}

	/*
    [HarmonyPatch(typeof(Type))]
    [HarmonyPatch("Function")]
    internal static class DrawStaticButton
    {
        public static bool Prefix(ref Letter __instance, float topY)
        {
            __instance.def.bounce = false;          // Never bounce
            __instance.def.flashInterval = 0.0f;    // No flashing
            return true; // Use old code
        }
    }*/

}
