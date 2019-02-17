using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Dialogs
{
    /// <summary>
    /// A TouchableDialog is a Dialog where the user must touch
    /// the screen for an action to happen.
    /// </summary>
    public class TouchableDialogBehaviour : DialogBehaviour
    {

        #region Fields

        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _contentText;
        private Action _touchedAction;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (Input.touchCount <= 0)
                return;

            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Ended) 
                    continue;

                _touchedAction?.Invoke();
                DialogsBehaviour.Instance.Close();
                return;
            }
        }

        #endregion

        #region Public Methods

        public void Show(string title, string text, Action touchedAction)
        {
            _titleText.SetText(title);
            _contentText.SetText(text);
            _touchedAction = touchedAction;
            gameObject.SetActive(true);
        }

        #endregion

    }
}
