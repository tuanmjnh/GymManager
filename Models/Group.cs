namespace GymManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Group")]
    public partial class Group
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Group()
        {
            GroupItems = new HashSet<GroupItem>();
        }

        public Guid id { get; set; }

        [StringLength(128)]
        public string appKey { get; set; }

        [StringLength(128)]
        public string idKey { get; set; }

        [StringLength(255)]
        public string title { get; set; }

        public string parentId { get; set; }

        public string parentsId { get; set; }

        public int? levels { get; set; }

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
        public virtual ICollection<GroupItem> GroupItems { get; set; }
    }
}
