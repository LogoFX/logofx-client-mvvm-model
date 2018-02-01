namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class TreeNodeEditableModel : EditableModel
    {
        public string Value { get; set; }

        public TreeNodeEditableModel Parent { get; set; }

        public TreeNodeEditableModel Next { get; set; }
    }
}