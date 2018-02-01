namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class SelfEditableModel : EditableModel
    {
        public string Value { get; set; }

        public SelfEditableModel Self { get; set; }
    }
}