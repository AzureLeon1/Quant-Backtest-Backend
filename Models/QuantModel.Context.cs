﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Quant_BackTest_Backend.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class quantEntities : DbContext
    {
        public quantEntities()
            : base("name=quantEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<backtest> backtest { get; set; }
        public virtual DbSet<data> data { get; set; }
        public virtual DbSet<strategy> strategy { get; set; }
        public virtual DbSet<user> user { get; set; }
    }
}
