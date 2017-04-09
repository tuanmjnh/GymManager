namespace GymManager.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TMShopsContext : DbContext
    {
        public TMShopsContext()
            : base("name=TMShopsContext")
        {
        }

        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupItem> GroupItems { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<SubBill> SubBills { get; set; }
        public virtual DbSet<SubItem> SubItems { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bill>()
                .Property(e => e.totalPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Bill>()
                .HasMany(e => e.SubBills)
                .WithOptional(e => e.Bill)
                .HasForeignKey(e => e.idKey);

            modelBuilder.Entity<Item>()
                .Property(e => e.priceOld)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Item>()
                .Property(e => e.price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Item>()
                .HasMany(e => e.SubItems)
                .WithOptional(e => e.Item)
                .HasForeignKey(e => e.idKey);

            modelBuilder.Entity<SubBill>()
                .Property(e => e.priceOld)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SubBill>()
                .Property(e => e.price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SubBill>()
                .Property(e => e.totalPrice)
                .HasPrecision(18, 0);
        }
    }
}
