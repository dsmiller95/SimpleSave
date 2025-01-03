using System;
using Dman.Utilities.Logger;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Dman.SimpleJson
{
    public enum TokenMode
    {
        /// <summary>
        /// Used for primitive types like string, int, float, Vector2, etc.
        /// Because JsonUtility cannot serialize a string, float, or Vector2 alone we need a different serialization method.
        /// </summary>
        Primitive,
        
        /// <summary>
        /// Use Unity's JsonUtility to serialize the object. Useful for complex objects which comply with unity's
        /// serialization standards.
        /// </summary>
        SerializableObject,
    }
    
    /// <summary>
    /// Save data for a single file, loaded into memory. Basically a wrapper around a JObject which represents the whole json file.
    /// </summary>
    public class SaveData
    {
        public JToken SavedToken => _data;
        private readonly JObject _data;
        
        private SaveData(JObject data)
        {
            _data = data;
        }

        public static SaveData Empty() => new(new JObject());
        public static SaveData Loaded(JObject data) => new(data);
        
        public bool HasKey(string key) => _data.ContainsKey(key);
        public bool DeleteKey(string key) => _data.Remove(key);

        public void Set<T>(string key, T value, TokenMode mode)
        {
            try
            {
                _data[key] = TokenFromValue(value, mode);
            }
            catch (JsonSerializationException e)
            {
                throw new SaveDataException($"Failed to save data for key {key} of type {typeof(T)}", e);
            }
            catch (InvalidOperationException e)
            {
                throw new SaveDataException($"Failed to save data for key {key} of type {typeof(T)}", e);
            }
        }

        public bool TryGet<T>(string key, out T value, TokenMode mode)
        {
            if (TryGet(key, out var obj, typeof(T), mode))
            {
                value = (T)obj;
                return true;
            }
            value = default;
            return false;
        }
        
        public bool TryGet(string key, out object value, Type objectType, TokenMode mode)
        {
            if (!_data.TryGetValue(key, out JToken existing))
            {
                value = default;
                return false;
            }

            try
            {
                value = ValueFromToken(existing, objectType, mode);
                return true;
            }
            catch (Exception e) when (e is JsonException or ArgumentException) 
            {
                Log.Error($"Failed to load data of type {objectType} for key {key}. Raw json: {existing}");
                value = default;
                return false;
            }
        }
        
        private JToken TokenFromValue<T>(T value, TokenMode mode)
        {
            switch (mode)
            {
                case TokenMode.Primitive:
                    return JToken.FromObject(value, JsonSaveSystemSettings.Serializer);
                case TokenMode.SerializableObject:
                    var unityJson = JsonUtility.ToJson(value);
                    return JToken.Parse(unityJson);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
        
        private object ValueFromToken(JToken token, Type objectType, TokenMode mode)
        {
            switch (mode)
            {
                case TokenMode.Primitive:
                    return token.ToObject(objectType, JsonSaveSystemSettings.Serializer);
                case TokenMode.SerializableObject:
                    var unityJson = token.ToString();
                    return JsonUtility.FromJson(unityJson, objectType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}