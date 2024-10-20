using System;
using System.Collections.Generic;
namespace Domain.TenantEntities
{
    public class Products
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Categories Categories { get; set; }


    }
}
