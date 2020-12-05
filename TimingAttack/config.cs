﻿using MelonLoader;
using System.Reflection;

namespace TimingAttack
{
    public static class Config
    {
        public const string Category = "TimingAttack";

        public static bool HiddenClouds;
        public static bool HiddenDarts;


        public static void RegisterConfig()
        {
            MelonPrefs.RegisterBool(Category, nameof(HiddenClouds), true, "Hide nebula clouds inside the approach circle");
            MelonPrefs.RegisterBool(Category, nameof(HiddenDarts), true, "Hide note darts");
            
            OnModSettingsApplied();
        }

        public static void OnModSettingsApplied()
        {
            foreach (var fieldInfo in typeof(Config).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (fieldInfo.FieldType == typeof(int))
                    fieldInfo.SetValue(null, MelonPrefs.GetInt(Category, fieldInfo.Name));

                if (fieldInfo.FieldType == typeof(bool))
                    fieldInfo.SetValue(null, MelonPrefs.GetBool(Category, fieldInfo.Name));

                if (fieldInfo.FieldType == typeof(float))
                    fieldInfo.SetValue(null, MelonPrefs.GetFloat(Category, fieldInfo.Name));
            }
        }
    }
}


