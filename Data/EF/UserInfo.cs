namespace Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserInfo")]
    public partial class UserInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public long? UserId { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        public string Address { get; set; }

        [StringLength(150)]
        public string Province { get; set; }

        [StringLength(150)]
        public string District { get; set; }

        [StringLength(150)]
        public string Ward { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        public bool? IsDefault { get; set; }
    }
}
