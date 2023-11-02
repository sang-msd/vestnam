namespace Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductCategory")]
    public partial class ProductCategory
    {
        public long Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(50)]
        public string MetaTitle { get; set; }

        public long? ParentId { get; set; }

        public int DisplayOrder { get; set; }

        [StringLength(250)]
        public string SeoTitle { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(250)]
        public string MetaKeywords { get; set; }

        [StringLength(250)]
        public string MetaDescription { get; set; }

        public bool Status { get; set; }
        public IEnumerable<ProductCategory> ParentCategories { get; set; }
    }
}
