using UnityEngine;

namespace Assets.Scripts.UI.Validation.Validators
{
    [CreateAssetMenu(menuName = "Ui Validators/Required")]
    public class RequiredUiValidator : UiValidator
    {

        #region Public Methods

        public override bool Validate(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case string s when string.IsNullOrWhiteSpace(s):
                    return false;
                default:
                    return true;
            }
        }

        #endregion


    }
}
