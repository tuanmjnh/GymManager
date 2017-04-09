namespace GymManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bill")]
    public partial class Bill
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bill()
        {
            SubBills = new HashSet<SubBill>();
        }

        public Guid id { get; set; }

        [StringLength(128)]
        public string codeKey { get; set; }

        [StringLength(128)]
        public string customerId { get; set; }

        public long? totalItem { get; set; }

        public long? totalQuantity { get; set; }

        public decimal? totalPrice { get; set; }

        [StringLength(2000)]
        public string desc { get; set; }

        public int? orders { get; set; }

        [StringLength(128)]
        public string createdBy { get; set; }

        public DateTime? createdAt { get; set; }

        [StringLength(128)]
        public string updatedBy { get; set; }

        public DateTime? updatedAt { get; set; }

        public int? flag { get; set; }

        public string extras { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubBill> SubBills { get; set; }
    }
}
