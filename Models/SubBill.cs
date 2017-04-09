namespace GymManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SubBill")]
    public partial class SubBill
    {
        public Guid id { get; set; }

        public Guid? idKey { get; set; }

        [StringLength(128)]
        public string codeKey { get; set; }

        public Guid? itemId { get; set; }

        [StringLength(255)]
        public string title { get; set; }

        public long? quantity { get; set; }

        public decimal? priceOld { get; set; }

        public decimal? price { get; set; }

        public decimal? totalPrice { get; set; }

        public int? orders { get; set; }

        public int? flag { get; set; }

        public string extras { get; set; }

        public virtual Bill Bill { get; set; }
    }
}
