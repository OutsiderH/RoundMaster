namespace RoundMaster.Patches {
    using global::RoundMaster.Reflection;
    using EFT;
    using HarmonyLib;
    using MonoMod.Cil;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Mono.Cecil.Cil;
    using System.Collections;
    using System.Threading;
    using System.Diagnostics;

    internal abstract class BasePatch {
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
                MethodInfo[] methods = PatchType.GetMethods(BindingFlagsPreset.allStatic);
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
    internal class VanillaMagazinePresetFix : BasePatch {
        private readonly MethodBase _targetMethod;
        internal VanillaMagazinePresetFix() : base() {
            _targetMethod = typeof(MagazineBuildPresetClass).GetMethod("method_2", BindingFlagsPreset.publicInstance);
        }
        protected override MethodBase TargetMethod {
            get => _targetMethod;
        }
        protected override Type PatchType {
            get => typeof(VanillaMagazinePresetFix);
        }
        [HarmonyILManipulator]
        [HarmonyDebug]
        private static void ILManipulator(ILContext context, MethodBase original, ILLabel retLabel) {
            ILCursor cursorConstructMongoId = new ILCursor(context).GotoNext(instruction => instruction.MatchCall(typeof(MongoID).GetMethod("op_Implicit", BindingFlagsPreset.publicStatic, null, [typeof(string)], null)));
            ILLabel labelAfterConstructMongoId = context.DefineLabel(cursorConstructMongoId.Clone().Next);
            ILLabel labelContinueLoop = context.DefineLabel(cursorConstructMongoId.Clone().GotoNext(instruction => instruction.MatchCallvirt(typeof(IEnumerator).GetMethod("MoveNext", BindingFlagsPreset.publicInstance))).Prev);
            cursorConstructMongoId
                .Emit(OpCodes.Dup)
                .Emit(OpCodes.Call, typeof(VanillaMagazinePresetFix).GetMethod("IsValidId", BindingFlagsPreset.nonPublicStatic))
                .Emit(OpCodes.Brtrue_S, labelAfterConstructMongoId)
                .Emit(OpCodes.Pop)
                .Emit(OpCodes.Br, labelContinueLoop);
        }
        private static bool IsValidId(string id) {
            return int.TryParse(id, out _);
        }
    }
}