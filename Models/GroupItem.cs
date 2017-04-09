namespace GymManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GroupItem")]
    public partial class GroupItem
    {
        public Guid id { get; set; }

        [StringLength(128)]
        public string appKey { get; set; }

        public Guid? groupId { get; set; }

        public Guid? itemId { get; set; }

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

        public virtual Group Group { get; set; }

        public virtual Item Item { get; set; }
    }
}
