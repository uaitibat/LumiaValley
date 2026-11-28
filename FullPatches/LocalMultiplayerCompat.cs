// Compat UWP do LocalMultiplayer original.
// O original usa System.Reflection.Emit (DynamicMethod/AssemblyBuilder/TypeBuilder/ILGenerator),
// que nao existe no perfil UWP. O comportamento observavel e mantido com FieldInfo:
// staticFields/staticDefaults, StaticSave/StaticLoad/StaticSetDefault e StaticVarHolderType.
// Splitscreen/local coop nao e suportado no Lumia, entao a lista de campos fica vazia.
using System;
using System.Collections.Generic;
using System.Reflection;

namespace StardewValley;

public class LocalMultiplayer
{
    public delegate void StaticInstanceMethod(object staticVarsHolder);

    internal static List<FieldInfo> staticFields = new List<FieldInfo>();

    internal static List<object> staticDefaults = new List<object>();

    public static Type StaticVarHolderType = typeof(StaticVarHolder);

    public static StaticInstanceMethod StaticSetDefault;

    public static StaticInstanceMethod StaticSave;

    public static StaticInstanceMethod StaticLoad;

    public sealed class StaticVarHolder
    {
        public object[] Values;
    }

    public static bool IsLocalMultiplayer(bool is_local_only = false)
    {
        if (is_local_only)
        {
            return Game1.hasLocalClientsOnly;
        }
        return GameRunner.instance.gameInstances.Count > 1;
    }

    public static void Initialize()
    {
        StaticSetDefault = SetStaticVarsToDefault;
        StaticSave = SaveStaticVars;
        StaticLoad = LoadStaticVars;
    }

    private static StaticVarHolder AsHolder(object holder)
    {
        return (StaticVarHolder)holder;
    }

    private static void EnsureCapacity(StaticVarHolder holder)
    {
        if (holder.Values == null || holder.Values.Length != staticFields.Count)
        {
            holder.Values = new object[staticFields.Count];
        }
    }

    private static void SetStaticVarsToDefault(object holder)
    {
        StaticVarHolder staticVarHolder = AsHolder(holder);
        EnsureCapacity(staticVarHolder);
        for (int i = 0; i < staticFields.Count; i++)
        {
            staticFields[i].SetValue(null, staticDefaults[i]);
        }
    }

    private static void SaveStaticVars(object holder)
    {
        StaticVarHolder staticVarHolder = AsHolder(holder);
        EnsureCapacity(staticVarHolder);
        for (int i = 0; i < staticFields.Count; i++)
        {
            staticVarHolder.Values[i] = staticFields[i].GetValue(null);
        }
    }

    private static void LoadStaticVars(object holder)
    {
        StaticVarHolder staticVarHolder = AsHolder(holder);
        EnsureCapacity(staticVarHolder);
        for (int i = 0; i < staticFields.Count; i++)
        {
            staticFields[i].SetValue(null, staticVarHolder.Values[i]);
        }
    }

    public static void SaveOptions()
    {
        if (Game1.player != null && (bool)Game1.player.isCustomized)
        {
            if (!Game1.splitscreenOptions.ContainsKey(Game1.player.UniqueMultiplayerID))
            {
                Game1.splitscreenOptions.Add(Game1.player.UniqueMultiplayerID, Game1.options);
            }
            else
            {
                Game1.splitscreenOptions[Game1.player.uniqueMultiplayerID] = Game1.options;
            }
        }
    }
}
