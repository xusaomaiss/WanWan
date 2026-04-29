using UnityEngine;
using UnityEngine.Events;

namespace Wanwan.Runtime
{
    public class MainMenuController : MonoBehaviour
    {
        private UnityAction startAction;
        private UnityAction mapAction;
        private UnityAction exitAction;
        private UnityAction infoAction;
        private UnityAction settingsAction;
        private UnityAction controlAction;
        private UnityAction achievementAction;
        private UnityAction shopAction;
        private UnityAction helpAction;

        public void Initialize(
            UnityAction start,
            UnityAction map,
            UnityAction exit,
            UnityAction info,
            UnityAction settings,
            UnityAction control,
            UnityAction achievement,
            UnityAction shop,
            UnityAction help)
        {
            startAction = start;
            mapAction = map;
            exitAction = exit;
            infoAction = info;
            settingsAction = settings;
            controlAction = control;
            achievementAction = achievement;
            shopAction = shop;
            helpAction = help;
        }

        public void StartGame()
        {
            startAction?.Invoke();
        }

        public void OpenMap()
        {
            mapAction?.Invoke();
        }

        public void ExitGame()
        {
            exitAction?.Invoke();
        }

        public void ShowInfo()
        {
            infoAction?.Invoke();
        }

        public void OpenSettings()
        {
            settingsAction?.Invoke();
        }

        public void OpenControl()
        {
            controlAction?.Invoke();
        }

        public void OpenAchievement()
        {
            achievementAction?.Invoke();
        }

        public void OpenShop()
        {
            shopAction?.Invoke();
        }

        public void OpenHelp()
        {
            helpAction?.Invoke();
        }
    }
}
