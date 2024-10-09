namespace RoundMaster {
    using HarmonyLib;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    public abstract class Patches {
        public abstract class BasePatch {
            private readonly List<MethodInfo> prefixList;
            private readonly List<MethodInfo> postfixList;
            private readonly List<MethodInfo> transpilerList;
            private readonly List<MethodInfo> finalizerList;
            private readonly List<MethodInfo> ilmanipulatorList;
            protected BasePatch() {
                prefixList = [];
                postfixList = [];
                transpilerList = [];
                finalizerList = [];
                ilmanipulatorList = [];
                MethodInfo[] methods = PatchType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic);
                foreach (MethodInfo method in methods) {
                    foreach (Attribute attribute in method.GetCustomAttributes()) {
                        switch (attribute) {
                            case HarmonyPrefix:
                                prefixList.Add(method);
                                break;
                            case HarmonyPostfix:
                                postfixList.Add(method);
                                break;
                            case HarmonyTranspiler:
                                transpilerList.Add(method);
                                break;
                            case HarmonyFinalizer:
                                finalizerList.Add(method);
                                break;
                            case HarmonyILManipulator:
                                ilmanipulatorList.Add(method);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            protected abstract MethodBase TargetMethod {
                get;
            }
            protected abstract Type PatchType {
                get;
            }
            public void Enable(Harmony harmonyInstance) {
                foreach (MethodInfo method in prefixList) {
                    harmonyInstance.Patch(TargetMethod, prefix: new HarmonyMethod(method));
                }
                foreach (MethodInfo method in postfixList) {
                    harmonyInstance.Patch(TargetMethod, postfix: new HarmonyMethod(method));
                }
                foreach (MethodInfo method in transpilerList) {
                    harmonyInstance.Patch(TargetMethod, transpiler: new HarmonyMethod(method));
                }
                foreach (MethodInfo method in finalizerList) {
                    harmonyInstance.Patch(TargetMethod, finalizer: new HarmonyMethod(method));
                }
                foreach (MethodInfo method in ilmanipulatorList) {
                    harmonyInstance.Patch(TargetMethod, ilmanipulator: new HarmonyMethod(method));
                }
            }
            public void Disable(Harmony harmonyInstance) {
                foreach (MethodInfo method in prefixList) {
                    harmonyInstance.Unpatch(TargetMethod, method);
                }
                foreach (MethodInfo method in postfixList) {
                    harmonyInstance.Unpatch(TargetMethod, method);
                }
                foreach (MethodInfo method in transpilerList) {
                    harmonyInstance.Unpatch(TargetMethod, method);
                }
                foreach (MethodInfo method in finalizerList) {
                    harmonyInstance.Unpatch(TargetMethod, method);
                }
                foreach (MethodInfo method in ilmanipulatorList) {
                    harmonyInstance.Unpatch(TargetMethod, method);
                }
            }
        }
        public class VanillaMagazinePresetFix : BasePatch {
            public VanillaMagazinePresetFix() : base() {

            }
            protected override MethodBase TargetMethod {
                get => typeof(MagazineBuildPresetClass).GetMethod("method_2");
            }
            protected override Type PatchType {
                get => typeof(VanillaMagazinePresetFix);
            }
            [HarmonyTranspiler]
            private static void Transpiler(IEnumerable<CodeInstruction> instructions) {
                List<CodeInstruction> instructionsList = new(instructions);
                instructionsList.InsertRange(110, [
                    new(OpCodes.Dup), // 110 (string, string)
                    new(OpCodes.Call, typeof(VanillaMagazinePresetFix).GetMethod("IsValidId")), // 111 (bool, string)
                    new(OpCodes.Brtrue, 115), // 112 string -> 115 call valuetype EFT.MongoID EFT.MongoID::op_Implicit(string)
                    new(OpCodes.Pop), // 113 ()
                    new(OpCodes.Br, 153) // 114 ()
                ]);
            }
            private static bool IsValidId(string id) {
                return int.TryParse(id, out _);
            }
        }
    }
}