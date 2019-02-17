using UnityEngine;

namespace Assets.Scripts.Dialogs
{
    /// <summary>
    /// An abstract class for all the possible dialog types.
    /// All dialogs open from <see cref="DialogsBehaviour" /> and close themselves.
    /// </summary>
    public abstract class DialogBehaviour : MonoBehaviour
    {

        #region Public Methods

        public void Close()
        {
            gameObject.SetActive(false);
        }

        #endregion

    }
}
