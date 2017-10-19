//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DigiDental
{
    using System;
    using System.Collections.Generic;
    
    public partial class Patients
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Patients()
        {
            this.Registrations = new HashSet<Registrations>();
            this.PatientCategories = new HashSet<PatientCategories>();
            this.TemplateImages = new HashSet<TemplateImages>();
        }
    
        public string Patient_ID { get; set; }
        public string Patient_Number { get; set; }
        public string Patient_Name { get; set; }
        public bool Patient_Gender { get; set; }
        public System.DateTime Patient_Birth { get; set; }
        public string Patient_IDNumber { get; set; }
        public string Patient_Photo { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public System.DateTime CreateDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Registrations> Registrations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatientCategories> PatientCategories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TemplateImages> TemplateImages { get; set; }
    }
}
