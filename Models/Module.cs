namespace GymManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Module
    {
        public Guid id { get; set; }

        [Required]
        [StringLength(128)]
        public string appKey { get; set; }

        [Required]
        [StringLength(128)]
        public string title { get; set; }

        [Required]
        [StringLength(256)]
        public string url { get; set; }

        [Required]
        [StringLength(50)]
        public string menuId { get; set; }

        public int orders { get; set; }

        [Required]
        [StringLength(128)]
        public string createdBy { get; set; }

        public DateTime createdAt { get; set; }

        [Required]
        [StringLength(128)]
        public string updatedBy { get; set; }

        public DateTime updatedAt { get; set; }

        public int flag { get; set; }

        public string extras { get; set; }
    }
}
