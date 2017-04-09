namespace GymManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SubItem")]
    public partial class SubItem
    {
        public Guid id { get; set; }

        [StringLength(128)]
        public string appKey { get; set; }

        public Guid? idKey { get; set; }

        public Guid? itemId { get; set; }

        [StringLength(255)]
        public string mainKey { get; set; }

        [StringLength(255)]
        public string value { get; set; }

        [StringLength(255)]
        public string subValue { get; set; }

        public string images { get; set; }

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

        public virtual Item Item { get; set; }
    }
}
