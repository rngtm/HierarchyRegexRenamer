namespace HierarchyRegexRenamer
{
    using System.Linq;
    using UnityEditor;

    /// <summary>
    /// データのロードを行うクラス
    /// </summary>
    public class DataLoader : AssetPostprocessor
    {
        /// <summary>
        /// プロジェクト内に存在するすべてのプリセットのロード
        /// </summary>
        public static PresetData[] LoadPresetDatas()
        {
             return AssetDatabase.FindAssets("t:ScriptableObject")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(PresetData)))
            .Where(obj => obj != null)
            .Select(obj => (PresetData)obj)
            .ToArray();
        }
    }
}
