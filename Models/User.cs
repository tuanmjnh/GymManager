namespace GymManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        public Guid id { get; set; }

        [StringLength(128)]
        public string username { get; set; }

        [StringLength(256)]
        public string password { get; set; }

        [StringLength(128)]
        public string salt { get; set; }

        [StringLength(256)]
        public string fullName { get; set; }

        [StringLength(128)]
        public string mobile { get; set; }

        [StringLength(256)]
        public string email { get; set; }

        public string address { get; set; }

        public string roles { get; set; }

        public int? orders { get; set; }

        [StringLength(128)]
        public string createdBy { get; set; }

        public DateTime? createdAt { get; set; }

        [StringLength(128)]
        public string updatedBy { get; set; }

        public DateTime? updatedAt { get; set; }

        public DateTime? lastLogin { get; set; }

        public Guid? staffId { get; set; }

        public int? flag { get; set; }

        public string extras { get; set; }
    }
}
