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
        /// Use newtonsoft to serialize the object. Used for primitive types like string, int, float, etc.
        /// </summary>
        Newtonsoft,
        
        /// <summary>
        /// Use Unity's JsonUtility to serialize the object. Useful for complex objects which comply with unity's
        /// serialization standards.
        /// </summary>
        UnityJson,
    }
    
    /// <summary>
    /// Save data for a single file
    /// </summary>
    public class SimpleSaveFile
    {
        public JToken SavedToken => _data;
        private readonly JObject _data;
        
        private readonly JsonSerializer _serializer;
        
        private SimpleSaveFile(JsonSerializer serializer, JObject data = null)
        {
            _serializer = serializer;
            _data = data ?? new JObject();
        }

        public static SimpleSaveFile Empty(JsonSerializer serializer) => new SimpleSaveFile(serializer);
        public static SimpleSaveFile Loaded(JObject data, JsonSerializer serializer) => new SimpleSaveFile(serializer, data);
            
        public void Save<T>(string key, T value, TokenMode mode)
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
        
        private JToken TokenFromValue<T>(T value, TokenMode mode)
        {
            switch (mode)
            {
                case TokenMode.Newtonsoft:
                    return JToken.FromObject(value, _serializer);
                case TokenMode.UnityJson:
                    var unityJson = JsonUtility.ToJson(value);
                    return JToken.Parse(unityJson);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public bool TryLoad(string key, out object value, Type objectType, TokenMode mode)
        {
            if (!_data.TryGetValue(key, out JToken existing))
            {
                value = default;
                return false;
            }

            try
            {
                value = ValueFromToken(existing, objectType, mode);
            }
            catch (JsonException)
            {
                Log.Error($"Failed to load data of type {objectType} for key {key}. Raw json: {existing}");
                value = default;
                return false;
            }
            return true;
        }
        
        private object ValueFromToken(JToken token, Type objectType, TokenMode mode)
        {
            switch (mode)
            {
                case TokenMode.Newtonsoft:
                    return token.ToObject(objectType, _serializer);
                case TokenMode.UnityJson:
                    var unityJson = token.ToString();
                    return JsonUtility.FromJson(unityJson, objectType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
        
        public bool TryLoad<T>(string key, out T value, TokenMode mode)
        {
            if (TryLoad(key, out var obj, typeof(T), mode))
            {
                value = (T)obj;
                return true;
            }
            value = default;
            return false;
        }

        public bool HasKey(string key)
        {
            return _data.ContainsKey(key);
        }
            
        public bool DeleteKey(string key)
        {
            return _data.Remove(key);
        }
    }
}