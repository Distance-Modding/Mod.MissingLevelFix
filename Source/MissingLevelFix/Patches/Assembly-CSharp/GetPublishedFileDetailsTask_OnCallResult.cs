using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MissingLevelFix.Patches.Assembly_CSharp
{
    /// <summary>
    /// The game validates the file name of workshop levels by calling <see cref="Resource.IsValidFileName"/> and
    /// prevents downloading levels with an invalid file name, even though the game could handle them.
    /// Instead of patching <see cref="Resource.IsValidFileName"/>, we only replace the call to it with a
    /// more generous validation method.
    /// This way, we allow invalid file names when downloading from the workshop,
    /// but still require valid file names when uploading to the workshop.
    /// </summary>
    [HarmonyPatch]
    internal class GetPublishedFileDetailsTask_OnCallResult
    {
        internal static MethodBase TargetMethod()
        {
            const string targetTypeName = "GetPublishedFileDetailsTask";
            const string targetMethodName = "OnCallResult";

            Type targetType = typeof(SteamworksUGC).GetNestedType(targetTypeName, BindingFlags.NonPublic);
            MethodBase targetMethod = targetType.GetMethod(targetMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
            return targetMethod;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo strictFilenameValidationMethod = AccessTools.Method(typeof(Resource), nameof(Resource.IsValidFileName));
            MethodInfo generousFilenameValidationMethod = AccessTools.Method(typeof(GetPublishedFileDetailsTask_OnCallResult), nameof(GenerousIsValidFileName));

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Call && instruction.operand == strictFilenameValidationMethod)
                    instruction.operand = generousFilenameValidationMethod;

                yield return instruction;
            }
        }

        private static bool GenerousIsValidFileName(string filename)
        {
            return !string.IsNullOrEmpty(filename.Trim());
        }
    }
}
