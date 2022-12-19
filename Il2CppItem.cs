﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using UnhollowerRuntimeLib;

namespace DillyzRoleApi_Rewritten
{
    [AttributeUsage(AttributeTargets.Class)]
    class Il2CppItemAttribute : Attribute
    {
        public static HashSet<Assembly> _registeredAssemblies = new HashSet<Assembly>();

        public Type[] Interfaces { get; }

        public Il2CppItemAttribute()
        {
            this.Interfaces = Type.EmptyTypes;
        }

        public Il2CppItemAttribute(params Type[] interfaces)
        {
            this.Interfaces = interfaces;
        }

        public static void TypeReg(Type type, Type[] interfaces) {
            Il2CppItemAttribute attribute = type.BaseType?.GetCustomAttribute<Il2CppItemAttribute>();
            if (attribute != null)
                TypeReg(type.BaseType, attribute.Interfaces);
            if (ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                return;
            try
            {
                RegisterTypeOptions options = new RegisterTypeOptions() { Interfaces = interfaces };
                ClassInjector.RegisterTypeInIl2Cpp(type, options);
            }
            catch (Exception e) {
                HarmonyMain.Instance.Log.LogError("epic reg fail " + e + "   " + type.FullDescription());
            }
        }

        [HarmonyPatch(typeof(IL2CPPChainloader), nameof(IL2CPPChainloader.LoadPlugin))]
        public class IL2CPPChainloaderPatch {
            public static void Postfix(IL2CPPChainloaderPatch __instance, PluginInfo pluginInfo, Assembly pluginAssembly) {
                if (_registeredAssemblies.Contains(pluginAssembly))
                    return;
                _registeredAssemblies.Add(pluginAssembly);

                foreach (Type type in pluginAssembly.GetTypes())
                {
                    var attribute = type?.GetCustomAttribute<Il2CppItemAttribute>();
                    if (attribute != null)
                        TypeReg(type, attribute.Interfaces);
                }
            } 
        }
    }
}
