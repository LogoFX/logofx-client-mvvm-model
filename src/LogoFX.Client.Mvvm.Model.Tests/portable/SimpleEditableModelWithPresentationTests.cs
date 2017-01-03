using NUnit.Framework;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [TestFixture]
    class SimpleEditableModelWithPresentationTests
    {
        [Test]        
        public void SimpleEditableModelIsValidAndPresentationIsOverridden_ExternalErrorIsSet_ErrorIsNotNull()
        {
            var model = new SimpleEditableModelWithPresentation();
            model.SetError("external error", "Name");

            Assert.AreEqual("overridden presentation", model.Error);
        }
    }
}