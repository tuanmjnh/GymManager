namespace GymManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Item")]
    public partial class Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item()
        {
            GroupItems = new HashSet<GroupItem>();
            SubItems = new HashSet<SubItem>();
        }

        public Guid id { get; set; }

        [StringLength(255)]
        public string appKey { get; set; }

        [StringLength(255)]
        public string idKey { get; set; }

        [StringLength(255)]
        public string codeKey { get; set; }

        [StringLength(255)]
        public string title { get; set; }

        public long? quantity { get; set; }

        public long? quantityTotal { get; set; }

        public decimal? priceOld { get; set; }

        public decimal? price { get; set; }

        public string images { get; set; }

        [Column(TypeName = "image")]
        public byte[] image { get; set; }

        [StringLength(2000)]
        public string desc { get; set; }

        public int? orders { get; set; }

        public DateTime? startedAt { get; set; }

        public DateTime? endedAt { get; set; }

        [StringLength(128)]
        public string createdBy { get; set; }

        public DateTime? createdAt { get; set; }

        [StringLength(128)]
        public string updatedBy { get; set; }

        public DateTime? updatedAt { get; set; }

        public int? flag { get; set; }

        public string extras { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GroupItem> GroupItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubItem> SubItems { get; set; }
    }
}
