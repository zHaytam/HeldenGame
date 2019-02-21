using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screens
{
    public class LoginScreen : MonoBehaviour
    {

        #region Fields

        [SerializeField] private Button _registerBtn;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Debug.Log("OnAwake");
            _registerBtn.onClick.AddListener(RegisterBtnClicked);
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy");
            _registerBtn.onClick.RemoveListener(RegisterBtnClicked);
        }

        #endregion

        #region Private Methods

        private static void RegisterBtnClicked()
        {
            SceneChanger.Instance.ChangeScene(AvailableScene.Registration);
        }

        #endregion

    }
}
