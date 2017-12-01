///-----------------------------------
/// HierarchyRegexRenamer
/// @ 2017 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace HierarchyRegexRenamer
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Preset
    {
        [SerializeField, Header("プレセット名")] private string name = "";
        [SerializeField, Header("Regexパターン")] private string pattern = "";
        [SerializeField, Header("置換文字列")] private string replacement = "";

        public string Name { get { return this.name; } set { this.name = value; } }
        public string Pattern { get { return this.pattern; } set { this.pattern = value; } }
        public string Replacement { get { return this.replacement; } set { this.replacement = value; } }
    }
}
