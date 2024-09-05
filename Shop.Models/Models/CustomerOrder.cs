using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Models.Models
{
    public class CustomerOrder
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMode { get; set; }
        public string ShippingAddress { get; set; }
        public int? ShippingCharges { get; set; }
        public int? SubTotal { get; set; }
        public int? Total { get; set; }
        public string ShippingStatus { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
    }
}