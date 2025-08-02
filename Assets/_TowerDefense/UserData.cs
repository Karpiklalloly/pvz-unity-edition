using System;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    public static class UserData
    {
        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        
        public static float GetFloat(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        
        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        
        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        
        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
        
        public static string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        
        public static void SetStrings(string key, params string[] values)
        {
            PlayerPrefs.SetString(key, string.Join(";", values));
        }

        public static void AddStrings(string key, params string[] values)
        {
            var strings = GetStrings(key);
            SetStrings(key, strings.Concat(values).ToArray());
        }
        
        public static string[] GetStrings(string key, string[] defaultValues = null)
        {
            return PlayerPrefs.GetString(key, string.Join(";", defaultValues ?? Array.Empty<string>())).Split(';');
        }
        
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }
        
        public static bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }
        
        public static void SetVector2(string key, Vector2 value)
        {
            PlayerPrefs.SetString(key, $"{value.x},{value.y}");
        }
        
        public static Vector2 GetVector2(string key, Vector2 defaultValue = default)
        {
            var str = PlayerPrefs.GetString(key, $"{defaultValue.x},{defaultValue.y}");
            var parts = str.Split(',');
            if (parts.Length == 2 && float.TryParse(parts[0], out var x) && float.TryParse(parts[1], out var y))
            {
                return new Vector2(x, y);
            }
            return defaultValue;
        }
        
        public static void SetVector3(string key, Vector3 value)
        {
            PlayerPrefs.SetString(key, $"{value.x},{value.y},{value.z}");
        }
        
        public static Vector3 GetVector3(string key, Vector3 defaultValue = default)
        {
            var str = PlayerPrefs.GetString(key, $"{defaultValue.x},{defaultValue.y},{defaultValue.z}");
            var parts = str.Split(',');
            if (parts.Length == 3 && float.TryParse(parts[0], out var x) && float.TryParse(parts[1], out var y) && float.TryParse(parts[2], out var z))
            {
                return new Vector3(x, y, z);
            }
            return defaultValue;
        }
        
        // TODO: прокидывать растения на уровень

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        
        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        
        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}