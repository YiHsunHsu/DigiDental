namespace DigiDental.ViewModels.Class
{
    public class CusComboBoxItem
    {
        public string DisplayName { get; set; }
        public int SelectedValue { get; set; }

        public CusComboBoxItem() { }
        public CusComboBoxItem(string displayName, int selectedValue)
        {
            DisplayName = displayName;
            SelectedValue = selectedValue;
        }
    }
}
