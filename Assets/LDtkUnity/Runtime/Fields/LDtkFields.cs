﻿using System.Linq;
using UnityEngine;

namespace LDtkUnity
{
    /// <summary>
    /// This is a component that stores the field instances for entities/levels, Conveniently converted for use in Unity.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(LDtkAddComponentMenu.ROOT + "Fields")]
    [HelpURL(LDtkHelpURL.COMPONENT_FIELDS)]
    public class LDtkFields : MonoBehaviour
    {
        public const string PROP_FIELDS = nameof(_fields);
        
        [SerializeField] private LDtkField[] _fields;
        
        /// <summary>
        /// Gets a single int field's value.
        /// </summary>
        /// <param name="identifier">
        /// The field instance's identifier. Case sensitive.
        /// </param>
        public int GetInt(string identifier) => GetFieldSingle(identifier, element => element.GetIntValue());
        public int[] GetIntArray(string identifier) => GetFieldArray(identifier, element => element.GetIntValue());

        //FLOAT
        public float GetFloat(string identifier) => GetFieldSingle(identifier, element => element.GetFloatValue());
        public float[] GetFloatArray(string identifier) => GetFieldArray(identifier, element => element.GetFloatValue());
        
        //BOOL
        public bool GetBool(string identifier) => GetFieldSingle(identifier, element => element.GetBoolValue());
        public bool[] GetBoolArray(string identifier) => GetFieldArray(identifier, element => element.GetBoolValue());
        
        //STRING
        public string GetString(string identifier) => GetFieldSingle(identifier, element => element.GetStringValue());
        public string[] GetStringArray(string identifier) => GetFieldArray(identifier, element => element.GetStringValue());
        
        //MULTILINE
        public string GetMultiline(string identifier) => GetFieldSingle(identifier, element => element.GetMultilineValue());
        public string[] GetMultilineArray(string identifier) => GetFieldArray(identifier, element => element.GetMultilineValue());
        
        //FILEPATH
        public string GetFilePath(string identifier) => GetFieldSingle(identifier, element => element.GetFilePathValue());
        public string[] GetFilePathArray(string identifier) => GetFieldArray(identifier, element => element.GetFilePathValue());
        
        //COLOR
        public Color GetColor(string identifier) => GetFieldSingle(identifier, element => element.GetColorValue());
        public Color[] GetColorArray(string identifier) => GetFieldArray(identifier, element => element.GetColorValue());

        //ENUM
        public TEnum GetEnum<TEnum>(string identifier) where TEnum : struct => GetFieldSingle(identifier, element => element.GetEnumValue<TEnum>());
        public TEnum[] GetEnumArray<TEnum>(string identifier) where TEnum : struct => GetFieldArray(identifier, element => element.GetEnumValue<TEnum>());

        //POINT
        public Vector2 GetPoint(string identifier) => GetFieldSingle(identifier, element => element.GetPointValue());
        public Vector2[] GetPointArray(string identifier) => GetFieldArray(identifier, element => element.GetPointValue());

        /// <summary>
        /// Get the first occuring color. Used by entities to decide their color in LDtk.
        /// </summary>
        public bool GetFirstColor(out Color firstColor)
        {
            foreach (LDtkField field in _fields)
            {
                if (!field.GetFieldElementByType(LDtkFieldType.Color, out LDtkFieldElement element))
                {
                    continue;
                }
                firstColor = element.GetColorValue();
                return true;
            }
            
            firstColor = Color.white;
            return false;
        }
        
        
        internal void SetFieldData(LDtkField[] fields)
        {
            _fields = fields;
        }
        
        private delegate T LDtkElementSelector<out T>(LDtkFieldElement element);

        private T GetFieldSingle<T>(string identifier, LDtkElementSelector<T> selector)
        {
            if (!TryGetField(identifier, out LDtkField field))
            {
                return default;
            }
            
            LDtkFieldElement element = field.GetSingle();
            return selector.Invoke(element);
        }
        
        private T[] GetFieldArray<T>(string identifier, LDtkElementSelector<T> selector)
        {
            if (!TryGetField(identifier, out LDtkField field))
            {
                return default;
            }
            
            LDtkFieldElement[] elements = field.GetArray();
            return elements.Select(selector.Invoke).ToArray();
        }
        
        private bool TryGetField(string identifier, out LDtkField field)
        {
            field = _fields.FirstOrDefault(fld => fld.Identifier == identifier);
            return field != null;
        }
    }
}