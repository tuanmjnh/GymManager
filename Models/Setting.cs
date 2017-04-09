namespace GymManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Setting
    {
        public Guid id { get; set; }

        [Required]
        [StringLength(255)]
        public string moduleKey { get; set; }

        [Required]
        [StringLength(255)]
        public string subKey { get; set; }

        public string value { get; set; }

        public string subValue { get; set; }

        [StringLength(2000)]
        public string desc { get; set; }

        public int? orders { get; set; }

        public int? flag { get; set; }

        public string extras { get; set; }
    }
}
