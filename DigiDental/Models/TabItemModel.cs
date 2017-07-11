namespace DigiDental.Models
{
    public class TabItemModel
    {
        /// <summary>
        /// Tab Header
        /// </summary>
        public virtual string Header { get; set; }
        public virtual Patients Patients { get; set; }
    }
}
