﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DigiDental
{
    using DigiDental.DataAccess.DbObject;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class DigiDentalEntities : DbContext
    {
        public DigiDentalEntities()
            : base(new ConnectionString().EFConnectionString())
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Registrations> Registrations { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Connecting_Logs> Connecting_Logs { get; set; }
        public virtual DbSet<Patients> Patients { get; set; }
        public virtual DbSet<Agencys> Agencys { get; set; }
        public virtual DbSet<Functions> Functions { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<TemplateImages> TemplateImages { get; set; }
        public virtual DbSet<Templates> Templates { get; set; }
    }
}
