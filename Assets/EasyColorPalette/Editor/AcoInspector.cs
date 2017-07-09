﻿///-------------------------------------
/// EasyColorPalette
/// @ 2017 RNGTM(https://github.com/rngtm)
///-------------------------------------
namespace EasyColorPalette
{
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Acoファイルのインスペクター表示
    /// </summary>
    [CustomEditor(typeof(DefaultAsset), false)]
    public class AcoInspector : Editor
    {
        const float FieldSpace = 4f;
        const float ColorMargin1 = 20f;
        const float ColorMargin2 = 12f;
        const float ColorSpace = 0f;
        const float ColorHeight = 12f;
        private string acoName;
        private Color[] colorArray;

        void OnEnable()
        {
            if (!this.IsAco()) { return; }
            this.acoName = target.name;
            this.colorArray = AcoExtractor.GetColors(target).ToArray();
        }

        public override void OnInspectorGUI()
        {
            if (!this.IsAco()) { return; }

            this.NameField();
            GUILayout.Space(FieldSpace);
            this.ColorField();
            GUILayout.Space(FieldSpace);
            GUI.enabled = true;
            this.ButtonAdd();

            base.OnInspectorGUI();
        }

        /// <summary>
        /// acoの名前の表示
        /// </summary>
        void NameField()
        {
            EditorGUILayout.LabelField("Name");
            GUILayout.Space(-5);
            GUILayout.BeginVertical("Box");
            GUILayout.TextField(this.acoName);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// acoの色の表示
        /// </summary>
        void ColorField()
        {
            EditorGUILayout.LabelField("Swatches");
            GUILayout.Space(-5);
            GUILayout.BeginVertical("Box");
            EditorGUILayout.BeginHorizontal();
            var rect = GUILayoutUtility.GetRect(0, ColorHeight);
            var screenWidth = Screen.width - 2; // 実際のInspectorの幅よりも2px大きくなっているので補正する (Unity5.6.11f1の場合)
            float colorWidth = (screenWidth - ColorMargin1 - ColorMargin2 - ColorSpace * (this.colorArray.Length - 1)) / this.colorArray.Length;
            var colorRect = new Rect(ColorMargin1, rect.y, colorWidth, ColorHeight);

            foreach (var color in this.colorArray)
            {
                EditorGUI.ColorField(
                    position: colorRect,
                    label: new GUIContent(""),
                    value: color,
                    showEyedropper: false,
                    showAlpha: false,
                    hdr: false,
                    hdrConfig: null
                );
                colorRect.x += ColorSpace + colorWidth;
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Addボタンの表示
        /// </summary>
        void ButtonAdd()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add to Presets"))
            {
                // プリセットへ追加
                ColorDatabase.Add(this.acoName, this.colorArray);

                // ウィンドウを開く
                ColorWindow.Open();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// このアセットがacoかどうか判定
        /// </summary>
        bool IsAco()
        {
            var path = AssetDatabase.GetAssetPath(target.GetInstanceID()); // ファイルパス
            var ext = Path.GetExtension(path); // 拡張子
            return ext == ".aco";
        }
    }
}
