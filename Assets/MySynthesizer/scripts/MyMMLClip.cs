﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Reflection;
using System.Collections;

namespace MySpace
{

    [Serializable]
    public class MyMMLClip
    {
        [SerializeField]
        private string name = "";
        [MyMMLClipCustom]
        [SerializeField]
        private AudioClip audioClip;
        [HideInInspector]
        [SerializeField]
        private TextAsset textA = null;
        [HideInInspector]
        [SerializeField]
        [TextArea]
        private string textB = null;
        [HideInInspector]
        [SerializeField]
        private TextAsset textC = null;
        [HideInInspector]
        [SerializeField]
        private bool generateAudioClip = false;
        [SerializeField]
        private bool loop = false;
        [SerializeField]
        [Range(0.0f, 2.0f)]
        private float volume = 1.0f;
        [SerializeField]
        [Range(0.0f, 2.0f)]
        private float tempo = 1.0f;
        [SerializeField]
        [Range(-12, +12)]
        private int key = 0;

        private bool dirty = true;
        private string text = null;
        private AudioClip generatedAudioClip;
        private MyMMLSequenceUnit unit;

        public MyMMLClip()
        {
        }
        public MyMMLClip(string Name, string MmlText)
        {
            name = Name;
            textB = MmlText;
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = (value == null) ? "" : value;
            }
        }
        public TextAsset TextA
        {
            get
            {
                return textA;
            }
            set
            {
                textA = value;
                Dirty = true;
            }
        }
        public string TextB
        {
            get
            {
                return textB;
            }
            set
            {
                textB = value;
                Dirty = true;
            }
        }
        public TextAsset TextC
        {
            get
            {
                return textC;
            }
            set
            {
                textC = value;
                Dirty = true;
            }
        }
        /// <summary>TextA + TextB + TextC</summary>
        public string Text
        {
            get
            {
                if((text == null) || dirty)
                {
                    int length = 0;
                    if (textA != null)
                    {
                        length += textA.text.Length;
                    }
                    if (textB != null)
                    {
                        length += textB.Length;
                    }
                    if (textC != null)
                    {
                        length += textC.text.Length;
                    }
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(length);
                    if(textA != null)
                    {
                        sb.Append(textA.text);
                    }
                    if (textB != null)
                    {
                        sb.Append(textB);
                    }
                    if (textC != null)
                    {
                        sb.Append(textC.text);
                    }
                    text = sb.ToString();
                }
                return text;
            }
        }
        public bool GenerateAudioClip
        {
            get
            {
                return generateAudioClip;
            }
            set
            {
                generateAudioClip = value;
                dirty = true;
            }
        }
        public bool Loop
        {
            get
            {
                return loop;
            }
            set
            {
                loop = value;
            }
        }
        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
            }
        }
        public float Tempo
        {
            get
            {
                return tempo;
            }
            set
            {
                tempo = value;
            }
        }
        public int Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }
        public AudioClip AudioClip
        {
            get
            {
                return generateAudioClip ? generatedAudioClip : audioClip;
            }
            set
            {
                if (generateAudioClip)
                {
                    generatedAudioClip = value;
                }
                else
                {
                    audioClip = value;
                }
            }
        }
        public MyMMLSequenceUnit Unit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
                generatedAudioClip = null;
#if true
                if ((unit != null) && (unit.Error.Length > 0))
                {
                    Debug.LogWarning(unit.Error);
                }
#endif
            }
        }
        public void Flush()
        {
            text = null;
            unit = null;
            generatedAudioClip = null;
            dirty = true;
        }
        public bool Dirty
        {
            get
            {
                return dirty;
            }
            set
            {
                dirty = value;
            }
        }
        public string Error
        {
            get
            {
                return (unit != null) ? unit.Error : "";
            }
        }
        public bool Prepared
        {
            get
            {
                return !dirty && (unit != null);
            }
        }
        public bool Valid
        {
            get
            {
                return !dirty && (unit != null) && (unit.Sequence != null) && (unit.ToneMap != null) && (unit.ToneSet != null);
            }
        }



        public class MyMMLClipCustomAttribute : PropertyAttribute
        {
            public MyMMLClipCustomAttribute()
            {
            }
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(MyMMLClipCustomAttribute))]
        public class MyMMLClipCustomDrawer : PropertyDrawer
        {
            private float textAreaHeight;
            private float CalcTextAreaHeight(string stringValue, int minLines, int maxLines)
            {
                int nl = 1;
                {
                    foreach (var c in stringValue)
                    {
                        if (c == '\n')
                        {
                            nl++;
                        }
                    }
                }
                if (nl < minLines)
                {
                    nl = minLines;
                }
                if (nl > maxLines)
                {
                    nl = maxLines;
                }
                float h = EditorGUIUtility.singleLineHeight;
                float s = EditorGUIUtility.standardVerticalSpacing + 1;
                return (h - s) * nl + s * 2;
            }
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                MyMMLClip clip = getParent(property) as MyMMLClip;
                if (clip == null)
                {
                    return 0.0f;
                }
                float y = 0;
                float h = EditorGUIUtility.singleLineHeight;
                float s = EditorGUIUtility.standardVerticalSpacing;
                textAreaHeight = CalcTextAreaHeight((clip.textB != null) ? clip.textB : "", 1, 8);
                y += h; // textA
                y += h + textAreaHeight; // textB
                y += h; // textC
                y += h; // generateAudioClip
                if (clip.Error.Length > 0)
                {
                    y += h + s * 2 + s;
                }
                y += h; // audioClip;
                return y;
            }
            private SerializedProperty getParentProperty(SerializedProperty property)
            {
                var elements = property.propertyPath.Split('.');
                if(elements.Length < 2)
                {
                    return null;
                }
                SerializedProperty p = property.serializedObject.FindProperty(elements[0]);
                for (int i = 1; i < elements.Length - 1; i++)
                {
                    p = p.FindPropertyRelative(elements[i]);
                }
                return p;
            }
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                MyMMLClip clip = getParent(property) as MyMMLClip;
                if (clip == null)
                {
                    return;
                }
                SerializedProperty clipProp = getParentProperty(property);
                float y = position.yMin;
                float h = EditorGUIUtility.singleLineHeight;
                float s = EditorGUIUtility.standardVerticalSpacing;
                bool tx = (clip.textA != null) || ((clip.textB != null) && (clip.textB.Length > 0)) || (clip.textC != null);
                bool ac = (clip.audioClip != null);
                EditorGUI.BeginDisabledGroup(ac && !tx);
                position.yMin = y;
                position.yMax = y + h;
                EditorGUI.PropertyField(position, clipProp.FindPropertyRelative("textA"));
                y = position.yMax;

                position.yMin = y;
                position.yMax = y + h;
                EditorGUI.LabelField(position, "Text B");
                y = position.yMax;
                position.yMin = y;
                position.yMax = y + textAreaHeight;
                EditorGUI.PropertyField(position, clipProp.FindPropertyRelative("textB"), GUIContent.none);
                y = position.yMax + s;

                position.yMin = y;
                position.yMax = y + h;
                EditorGUI.PropertyField(position, clipProp.FindPropertyRelative("textC"));
                y = position.yMax;

                position.yMin = y;
                position.yMax = y + h;
                EditorGUI.PropertyField(position, clipProp.FindPropertyRelative("generateAudioClip"));
                y = position.yMax;

                if(clip.Error.Length > 0)
                {
                    position.yMin = y;
                    position.yMax = y + h + s * 2;
                    EditorGUI.HelpBox(EditorGUI.IndentedRect(position), clip.Error, MessageType.Error);
                    y = position.yMax + s;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(tx);
                position.yMin = y;
                position.yMax = y + h;
                EditorGUI.PropertyField(position, clipProp.FindPropertyRelative("audioClip"));
                y = position.yMax;
                EditorGUI.EndDisabledGroup();
            }
            private object getParent(SerializedProperty prop)
            {
                string path = prop.propertyPath;
                object obj = prop.serializedObject.targetObject;
                return getInstance(obj, path.Substring(0, path.LastIndexOf('.')));
            }
            private object getInstance(SerializedProperty prop)
            {
                string path = prop.propertyPath;
                object obj = prop.serializedObject.targetObject;
                return getInstance(obj, path);
            }
            private object getInstance(object obj, string path)
            {
                var elements = path.Replace(".Array.data[", "[").Split('.');
                foreach (var element in elements)
                {
                    int bp0 = element.IndexOf("[");
                    int bp1 = element.IndexOf("]");
                    if ((bp0 >= 0) && (bp1 >= 0))
                    {
                        var elementName = element.Substring(0, bp0);
                        int index;
                        int.TryParse(element.Substring(bp0 + 1, bp1 - bp0 - 1), out index);
                        obj = getValue(obj, elementName, index);
                    }
                    else
                    {
                        obj = getValue(obj, element);
                    }
                }
                return obj;
            }
            private object getValue(object parent, string name)
            {
                if (parent == null)
                {
                    return null;
                }
                var type = parent.GetType();
                var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field == null)
                {
                    return null;
                }
                return field.GetValue(parent);
            }
            private object getValue(object source, string name, int index)
            {
                var enumerable = getValue(source, name) as IEnumerable;
                var enm = enumerable.GetEnumerator();
                while (index-- >= 0)
                {
                    enm.MoveNext();
                }
                return enm.Current;
            }
        }
#endif // UNITY_EDITOR


#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(TextAreaAttribute))]
        public class TextAreaDrawer : PropertyDrawer
        {
            private float height;
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (property.propertyType != SerializedPropertyType.String)
                {
                    return EditorGUI.GetPropertyHeight(property, label, true);
                }
                TextAreaAttribute attr = attribute as TextAreaAttribute;
                int nl = 1;
                {
                    foreach (var c in property.stringValue)
                    {
                        if (c == '\n')
                        {
                            nl++;
                        }
                    }
                }
                if (nl < attr.minLines)
                {
                    nl = attr.minLines;
                }
                if (nl > attr.maxLines)
                {
                    nl = attr.maxLines;
                }
                //Debug.Log("space" + EditorGUIUtility.standardVerticalSpacing.ToString());
                height = EditorGUIUtility.singleLineHeight - 3;
                return height * (nl + 1) + 3 * 2;
            }
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.propertyType != SerializedPropertyType.String)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                    return;
                }
                EditorGUI.LabelField(position, label);
                position.yMin = position.yMin + height + 3;
                property.stringValue = EditorGUI.TextArea(position, property.stringValue);
            }
        }
#endif // UNITY_EDITOR
    }
}
