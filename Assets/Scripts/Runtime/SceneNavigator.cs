using UnityEngine.SceneManagement;

namespace Wanwan.Runtime
{
    public static class SceneNavigator
    {
        public const string MenuScene = "Menu";
        public const string GameScene = "Game";
        public const string GameOverScene = "GameOver";

        public static void LoadMenu()
        {
            SceneManager.LoadScene(MenuScene);
        }

        public static void LoadGame()
        {
            SessionState.ResetRun();
            SceneManager.LoadScene(GameScene);
        }

        public static void LoadGame(GameDifficulty difficulty)
        {
            SessionState.SelectDifficulty(difficulty);
            LoadGame();
        }

        public static void LoadGameOver()
        {
            SceneManager.LoadScene(GameOverScene);
        }
    }
}
