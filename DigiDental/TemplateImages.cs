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
    
    public partial class TemplateImages
    {
        public int TemplateImage_ID { get; set; }
        public string TemplateImage_Number { get; set; }
        public int Template_ID { get; set; }
        public Nullable<int> Image_ID { get; set; }
        public string Image_Path { get; set; }
        public string Patient_ID { get; set; }
    
        public virtual Images Images { get; set; }
        public virtual Patients Patients { get; set; }
        public virtual Templates Templates { get; set; }
    }
}
