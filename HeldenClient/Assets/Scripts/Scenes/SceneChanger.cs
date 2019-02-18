using System.Collections;
using Assets.Scripts.UI.Elements;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Scenes
{
    public class SceneChanger : UndestroyableSingletonBehaviour<SceneChanger>
    {

        #region Fields

        private const float SceneLoadedPercentage = 0.9f;
        [SerializeField] private GameObject _panel;
        [SerializeField] private ProgressBarUiElement _pb;

        #endregion

        #region Properties

        public AvailableScene CurrentScene { get; private set; }

        #endregion

        #region Public Methods

        public void ChangeScene(AvailableScene newScene)
        {
            StartCoroutine(LoadScene(newScene));
        }

        #endregion

        #region Private Methods

        private IEnumerator LoadScene(AvailableScene newScene)
        {
            _panel.SetActive(true);
            yield return new WaitForSeconds(1f);
            var operation = SceneManager.LoadSceneAsync($"{newScene}Scene");
            operation.allowSceneActivation = false;

            while (operation.progress < SceneLoadedPercentage)
            {
                _pb.Value = operation.progress;
                yield return null;
            }

            _pb.Value = 1f;
            yield return new WaitForSeconds(1f);
            operation.allowSceneActivation = true;
            _panel.SetActive(false);
            CurrentScene = newScene;
        }

        #endregion

    }
}
