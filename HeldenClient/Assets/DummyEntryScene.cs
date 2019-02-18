using Assets.Scripts.Scenes;
using UnityEngine;

namespace Assets
{
    public class DummyEntryScene : MonoBehaviour
    {

        #region Unity Methods

        private void Start()
        {
            SceneChanger.Instance.ChangeScene(AvailableScene.Connection);
        }

        #endregion

    }
}
