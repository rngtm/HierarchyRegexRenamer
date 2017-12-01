///-----------------------------------
/// HierarchyRegexRenamer
/// @ 2017 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace HierarchyRegexRenamer
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class RenameWindow : EditorWindow
    {
        [SerializeField] string pattern = ""; // Regexパターン
        [SerializeField] string replacement = ""; // 置換文字列
        private ReorderableList reorderableList;
        private Vector2 scrollPos = Vector2.zero;
        private PresetData presetData;

        /// <summary>
        /// EditorWindowの描画処理
        /// </summary>
        void OnGUI()
        {
            EditorGUILayout.LabelField("Hierarchy上で選択しているオブジェクトをリネームします");
            GUILayout.Space(2f);

            this.pattern = EditorGUILayout.TextField("Regex", this.pattern);
            this.replacement = EditorGUILayout.TextField("Replacement", this.replacement);

            // ボタンを表示
            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length == 0);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("リネーム"))
            {
                this.DoRename();
            }
            if (GUILayout.Button("保存", GUILayout.Width(80f)))
            {
                var presetName = "新しいプリセット";

                Debug.LogFormat("保存: {0}", presetName);  
                this.presetData.PresetList.Add(new Preset
                {
                    Name = presetName,
                    Pattern = this.pattern,
                    Replacement = this.replacement,
                });

                EditorUtility.SetDirty(this.presetData);
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.EndHorizontal();

            if (this.reorderableList == null)
            {
                this.RebuildList();
            }

            if (this.reorderableList != null)
            {
                this.reorderableList.DoLayoutList();
            }
        }

        /// <summary>
        /// ReorderbleList再作成
        /// </summary>
        void RebuildList()
        {
            this.reorderableList = this.CreateReorderableList();
        }

        /// <summary>
        /// ReorderableList作成
        /// </summary>
        ReorderableList CreateReorderableList()
        {
            var data = DataLoader.LoadPresetDatas()[0];
            this.presetData = data;

            var list = new ReorderableList(data.PresetList, typeof(Preset));

            // Label用スタイル
            var labelStyle = new GUIStyle(EditorStyles.label);
            // labelStyle.alignment = TextAnchor.MiddleCenter;

            // ヘッダー描画
            var headerRect = default(Rect);
            list.drawHeaderCallback = (rect) =>
            {
                headerRect = rect;
                EditorGUI.LabelField(rect, Config.TEXT_LIST_HEADER);
            };

            // フッター描画
            list.drawFooterCallback = (rect) =>
            {
                rect.y = headerRect.y + 3;
                ReorderableList.defaultBehaviours.DrawFooter(rect, list);
            };

            // 要素の描画
            float fieldHeight = list.elementHeight;
            list.elementHeight *= 3f;
            list.elementHeight += Config.SPACE_ELEMENT;
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 1;
                rect.y += Config.SPACE_ELEMENT;
                rect.height = fieldHeight;
                rect.height -= 4;

                var preset = data.PresetList[index];

                // ロードボタン
                var loadButtonRect = new Rect(rect);
                loadButtonRect.width = Config.WIDTH_BUTTON_LOAD;
                if (GUI.Button(loadButtonRect, Config.TEXT_BUTTON_LOAD))
                {
                    LoadPreset(preset);
                }

                // リネームボタン
                var renemaButtonRect = new Rect(rect);
                renemaButtonRect.width = Config.WIDTH_BUTTON_LOAD;
                if (GUI.Button(renemaButtonRect, Config.TEXT_BUTTON_LOAD))
                {
                    LoadPreset(preset);
                }

                // プリセット名入力
                var nameTextRect = new Rect(rect);
                nameTextRect.x = loadButtonRect.x + loadButtonRect.width + Config.SPACE + Config.WIDTH_LABEL - Config.WIDTH_BUTTON_LOAD;
                nameTextRect.width = rect.width - loadButtonRect.width - Config.SPACE;
                preset.Name = EditorGUI.TextField(nameTextRect, preset.Name);

                rect.y += fieldHeight; // 下へずらす

                // Regexパターン
                var regexLabelRect = new Rect(rect);
                regexLabelRect.width = Config.WIDTH_LABEL;
                EditorGUI.LabelField(regexLabelRect, Config.TEXT_LABEL_REGEX, labelStyle);

                // Regex入力
                var regexTextRect = new Rect(rect);
                regexTextRect.x = regexLabelRect.x + regexLabelRect.width + Config.SPACE;
                regexTextRect.width = rect.width - regexLabelRect.width - Config.SPACE;
                // preset.Pattern = EditorGUI.TextField(regexTextRect, preset.Pattern);
                EditorGUI.TextField(regexTextRect, preset.Pattern);
                
                rect.y += fieldHeight; // 下へずらす

                // Regexパターン
                var replacementLabelRect = new Rect(rect);
                replacementLabelRect.width = Config.WIDTH_LABEL;
                EditorGUI.LabelField(replacementLabelRect, Config.TEXT_LABEL_REPLACEMENT, labelStyle);

                // replacement入力
                var replacementTextRect = new Rect(rect);
                replacementTextRect.x = replacementLabelRect.x + replacementLabelRect.width + Config.SPACE;
                replacementTextRect.width = rect.width - replacementLabelRect.width - Config.SPACE;
                // preset.Replacement = EditorGUI.TextField(replacementTextRect, preset.Replacement);
                EditorGUI.TextField(replacementTextRect, preset.Replacement);

            };

            // 変更時処理
            list.onChangedCallback += (l) =>
            {
                EditorUtility.SetDirty(data);
            };

            return list;
        }

        /// <summary>
        /// プリセットのロード
        /// </summary>
        void LoadPreset(Preset preset)
        {
            this.pattern = preset.Pattern;
            this.replacement = preset.Replacement;
        }

        static void DoRemoveButton(ReorderableList list, int index)
        {
            Debug.Log("削除: " + index);
        }

        /// <summary>
        /// ウィンドウを開く
        /// </summary>
        [MenuItem("Tools/Hierarchy Regex Renamer")]
        static void Open()
        {
            GetWindow<RenameWindow>();
        }

        /// <summary>
        /// Hierarchy上で選択しているオブジェクトをリネームする
        /// </summary>
        void DoRename()
        {
            var gameObjects = Selection.gameObjects.Where(go => !AssetDatabase.IsMainAsset(go)).ToArray(); // リネーム対象のGameObject

            // Undoに登録
            Undo.RecordObjects(gameObjects, "Regex Rename");

            // 名前を変える
            foreach (var go in gameObjects)
            {
                go.name = Regex.Replace(go.name, this.pattern, this.replacement);
            }
        }
    }
}