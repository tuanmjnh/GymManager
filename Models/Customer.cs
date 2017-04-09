namespace GymManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Customer
    {
        public Guid id { get; set; }

        [StringLength(150)]
        public string fullName { get; set; }

        [StringLength(50)]
        public string dateOfBirth { get; set; }

        [StringLength(150)]
        public string mobile { get; set; }

        public string address { get; set; }

        [StringLength(255)]
        public string facebook { get; set; }

        [StringLength(255)]
        public string email { get; set; }

        [StringLength(128)]
        public string cardId { get; set; }

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
    }
}
