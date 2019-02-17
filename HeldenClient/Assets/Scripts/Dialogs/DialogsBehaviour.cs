using System;
using UnityEngine;

namespace Assets.Scripts.Dialogs
{
    public class DialogsBehaviour : SingletonBehaviour<DialogsBehaviour>
    {

        #region Fields

        [SerializeField] private TouchableDialogBehaviour _touchableDialog;
        private DialogBehaviour _currentDialog;

        #endregion

        #region Properties

        public bool IsOpen => _currentDialog != null;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        #endregion

        #region Public Methods

        public bool ShowTouchable(string title, string content, Action touchedAction)
        {
            if (IsOpen)
                return false;

            _touchableDialog.Show(title, content, touchedAction);
            _currentDialog = _touchableDialog;
            return true;
        }

        public void Close()
        {
            if (!IsOpen)
                return;
        
            _currentDialog.Close();
            _currentDialog = null;
        }

        #endregion

        #region Private Methods



        #endregion

    }
}
