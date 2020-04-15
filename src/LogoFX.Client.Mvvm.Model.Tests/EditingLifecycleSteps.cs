﻿using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class EditingLifecycleSteps
    {
        private readonly ModelSteps _modelSteps;

        public EditingLifecycleSteps(
            ModelSteps modelSteps)
        {
            _modelSteps = modelSteps;
        }

        [When(@"The editable model changes are cancelled")]
        public void WhenTheEditableModelChangesAreCancelled()
        {
            var model =_modelSteps.GetModel<EditableModelWithValidation>();
            model.CancelChanges();
        }

        [When(@"The editable model with read only field changes are cancelled")]
        public void WhenTheEditableModelWithReadOnlyFieldChangesAreCancelled()
        {
            var model = _modelSteps.GetModel<EditableModelWithReadOnlyField>();
            model.CancelChanges();
        }
    }
}
