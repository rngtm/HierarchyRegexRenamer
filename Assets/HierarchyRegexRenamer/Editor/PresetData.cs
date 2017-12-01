///-----------------------------------
/// HierarchyRegexRenamer
/// @ 2017 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace HierarchyRegexRenamer
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PresetData : ScriptableObject
    {
        [SerializeField, Header("プリセット一覧")] private List<Preset> presetList;
        public List<Preset> PresetList { get { return this.presetList; } }

    }
}
