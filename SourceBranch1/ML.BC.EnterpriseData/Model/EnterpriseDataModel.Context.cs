﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ML.BC.EnterpriseData.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BCEnterpriseContext : DbContext
    {
        public BCEnterpriseContext()
            : base("name=BCEnterpriseContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<EnterpriseProfession> EnterpriseProfessions { get; set; }
        public virtual DbSet<EnterpriseProperty> EnterprisePropertys { get; set; }
        public virtual DbSet<Enterprise> Enterprises { get; set; }
        public virtual DbSet<FrontUser> FrontUsers { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<RFAAuthorization> RFAAuthorizations { get; set; }
        public virtual DbSet<RFAFunction> RFAFunctions { get; set; }
        public virtual DbSet<RFARole> RFARoles { get; set; }
        public virtual DbSet<Scene> Scenes { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<MaterialType> MaterialTypes { get; set; }
        public virtual DbSet<UserLoginState> UserLoginStates { get; set; }
        public virtual DbSet<SyncState> SyncStates { get; set; }
        public virtual DbSet<SceneType> SceneTypes { get; set; }
        public virtual DbSet<UserLoginLog> UserLoginLogs { get; set; }
    }
}
