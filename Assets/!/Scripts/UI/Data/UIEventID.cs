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
        
        // Button Events
        OnPlayButtonClicked,
        OnPauseButtonClicked,
        OnResumeButtonClicked,
        OnRestartButtonClicked,
        OnNextLevelButtonClicked,
        OnCloseButtonClicked,
    }
}