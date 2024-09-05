using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Models.CustomModels
{
    public class ProductModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product Price is required")]
        public int? Price { get; set; }

        [Required(ErrorMessage = "Product Stock is required")]
        public int? Stock { get; set; }

        [Required(ErrorMessage = "Product Category is required")]
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }

        public byte[]? FileContent { get; set; }
        public bool CartFlag { get; set; }
    }
}