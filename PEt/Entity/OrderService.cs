namespace PEt.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderService
    {
        public int OrderServiceId { get; set; }

        public int OrderId { get; set; }

        public int ServiceId { get; set; }

        public virtual Order Order { get; set; }

        public virtual Service Service { get; set; }
    }
}
