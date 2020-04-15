﻿using LogoFX.Client.Mvvm.Model.Tests.Helpers;
using LogoFX.Client.Mvvm.Model.Tests.Objects;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests.Steps
{
    [Binding]
    internal sealed class EditableModelSteps
    {
        private readonly ModelSteps _modelSteps;

        public EditableModelSteps(
            ModelSteps modelSteps)
        {
            _modelSteps = modelSteps;
        }

        [When(@"The editable model is created with valid title")]
        public void WhenTheEditableModelIsCreatedWithValidTitle()
        {
            _modelSteps.CreateModel(() =>
                new EditableModelWithValidation(DataGenerator.ValidTitle, 21));
        }

        [When(@"The editable model is created with invalid title")]
        public void WhenTheEditableModelIsCreatedWithInvalidTitle()
        {
            _modelSteps.CreateModel(() =>
                new EditableModelWithValidation(DataGenerator.InvalidTitle, 21));
        }

        [When(@"The editable model with read only field is created")]
        public void WhenTheEditableModelWithReadOnlyFieldIsCreated()
        {
            _modelSteps.CreateModel(() =>
                new EditableModelWithReadOnlyField(1, "remark"));
        }

        [When(@"The editable model with undo redo and valid name is created")]
        public void WhenTheEditableModelWithUndoRedoAndValidNameIsCreated()
        {
            _modelSteps.CreateModel(() =>
                new SimpleEditableModelWithUndoRedo(DataGenerator.ValidName, 21));
        }

        [When(@"The editable model is updated with invalid value for property")]
        public void WhenTheEditableModelIsUpdatedWithInvalidValueForProperty()
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            model.Title = DataGenerator.InvalidTitle;
        }

        [When(@"The editable model is updated with value '(.*)' for property")]
        public void WhenTheEditableModelIsUpdatedWithValueForProperty(string value)
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            model.Title = value;
        }

        [When(@"The editable model is updated with external error")]
        public void WhenTheEditableModelIsUpdatedWithExternalError()
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            model.SetError("external error", "Title");
        }

        [When(@"The editable model is cleared from external errors")]
        public void WhenTheEditableModelIsClearedFromExternalErrors()
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            model.ClearError("Title");
        }

        [When(@"The editable model with read only field is updated with new status")]
        public void WhenTheEditableModelWithReadOnlyFieldIsUpdatedWithNewStatus()
        {
            var model = _modelSteps.GetModel<EditableModelWithReadOnlyField>();
            model.Status = 2;
        }

        [Then(@"The editable model has no errors")]
        public void ThenTheEditableModelHasNoErrors()
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            AssertHelper.AssertModelHasErrorIsFalse(model);
        }

        [Then(@"The editable model has errors")]
        public void ThenTheEditableModelHasErrors()
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            AssertHelper.AssertModelHasErrorIsTrue(model);
        }

        [Then(@"The editable model with read only field has no errors")]
        public void ThenTheEditableModelWithReadOnlyFieldHasNoErrors()
        {
            var model = _modelSteps.GetModel<EditableModelWithReadOnlyField>();
            AssertHelper.AssertModelHasErrorIsFalse(model);
        }

        [Then(@"The editable model with undo redo has no errors")]
        public void ThenTheEditableModelWithUndoRedoHasNoErrors()
        {
            var model = _modelSteps.GetModel<SimpleEditableModelWithUndoRedo>();
            AssertHelper.AssertModelHasErrorIsFalse(model);
        }
    }
}