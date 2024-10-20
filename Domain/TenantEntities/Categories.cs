using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TenantEntities
{
    public class Categories
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Category Name")]
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public virtual ICollection<Products> Products { get; set; }
    }
}
