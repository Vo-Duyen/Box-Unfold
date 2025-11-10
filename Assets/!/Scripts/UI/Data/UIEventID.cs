namespace LongNC.UI.Data
{
    public enum UIEventID
    {
        // Game State Events
        OnStartGame,
        OnPauseGame,
        OnResumeGame,
        OnWinGame,
        OnLoseGame,
        OnLevelUp,
        OnQuitGame,
        
        // Gameplay Events
        OnTimeChanged,
        OnSoundChanged,
        
        OnRestartClicked,
        OnCloseRestartClicked,
        OnRestartButtonClicked,
        
        OnHelpClicked,
        OnCloseHelpClicked,
        
        OnSettingClicked,
        OnCloseSettingClicked,
        
        // Button Events
        OnPlayButtonClicked,
        OnPauseButtonClicked,
        OnResumeButtonClicked,
        OnNextLevelButtonClicked,
        OnCloseButtonClicked,
        
        
    }
}