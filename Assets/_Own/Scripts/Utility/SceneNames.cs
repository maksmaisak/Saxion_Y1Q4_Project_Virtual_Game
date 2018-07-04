using System;

public static class SceneNames
{
    // This stuff has to be hardcoded since you can't just assign scenes in the inspector.
    // Still better to keep it centralized anyway. A solution using ScriptableObjects might be
    // possible, but there's no time
    public const string managementName = "management";
    public const string managementPath = "Assets/_Own/Scenes/Final/management.unity";

    public const string tutorialName = "tutorial_level";
    public const string mainLevelName = "MainLevel";
    public const string mainMenuName = "MainMenu";
}
