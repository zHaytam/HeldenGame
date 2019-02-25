using UnityEngine;

namespace Assets.Scripts.UI.Validation.Validators
{
    public abstract class UiValidator : ScriptableObject
    {

        #region Public Methods

        public abstract bool Validate(object obj);

        #endregion

    }
}
