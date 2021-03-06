﻿using System.Xml.Serialization;
using MBOptionScreen.Attributes;
using MBOptionScreen.Settings;

namespace CharacterCreation
{
    public class Settings : AttributeSettings<Settings>
    {
        public override string ModName => "Detailed Character Creation";
        public override string ModuleFolderName => SubModule.ModuleFolderName;

        [XmlElement]
        public override string Id { get; set; } = "DCCSettings_v1";

        [XmlElement]
        [SettingProperty("Enable debug output", "", hintText: "Enable DCC's debug output.")]
        [SettingPropertyGroup("Section 0: Debug Mode")]
        public bool DebugMode { get; set; } = false;

        #region Overrides
        [SettingProperty("Overrides", "", hintText: "Keep this on to prevent the game from reverting your appearance.")]
        [SettingPropertyGroup("Section 1: Overrides", true)]
        public bool IgnoreDailyTick { get; set; } = true;
        [XmlElement]
        [SettingProperty("Override Age", "", hintText: "When enabled, this will prevent FaceGen from changing a hero's age.")]
        [SettingPropertyGroup("Section 1: Overrides", false)]
        public bool OverrideAge { get; set; } = false;
        [SettingProperty("Disable Auto Aging", "", hintText: "Enable this to prevent the game from changing the age physical appearance.")]
        [SettingPropertyGroup("Section 1: Overrides", false)]
        public bool DisableAutoAging { get; set; } = false;
        #endregion

        #region AgeModel
        [SettingProperty("Infant Age Stage", 0, 3, hintText: "Set the default infant stage age.")]
        [SettingPropertyGroup("Section 2: Age Model", false)]
        public int BecomeInfantAge { get; set; } = 3;
        [SettingProperty("Child Age Stage", 1, 6, hintText: "Set the default child stage age.")]
        [SettingPropertyGroup("Section 2: Age Model", false)]
        public int BecomeChildAge { get; set; } = 6;
        [SettingProperty("Teenager Age Stage", 2, 14, hintText: "Set the default teenager stage age.")]
        [SettingPropertyGroup("Section 2: Age Model", false)]
        public int BecomeTeenagerAge { get; set; } = 14;
        [SettingProperty("Adult Age Stage", 3, 18, hintText: "Set the default adult stage age.")]
        [SettingPropertyGroup("Section 2: Age Model", false)]
        public int BecomeAdultAge { get; set; } = 18;
        [SettingProperty("Old Age Stage", 30, 60, hintText: "Set the default old stage age.")]
        [SettingPropertyGroup("Section 2: Age Model", false)]
        public int BecomeOldAge { get; set; } = 47;
        [SettingProperty("Max Age Stage", 60, 128, hintText: "Set the default max age.")]
        [SettingPropertyGroup("Section 2: Age Model", false)]
        public int MaxAge { get; set; } = 128;
        #endregion
    }
}
