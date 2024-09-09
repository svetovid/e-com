using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace btm.paas.Models
{
    //[PrimaryKey(nameof(Id), nameof(PaymentReference))]
    public class PaymentModel
    {
        public long Id { get; set; }

        [Key]
        public string Reference { get; set; }

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; }

        public string CustomerReference { get; set; }

        public int MethodActionId { get; set; }

        public virtual MethodActionModel MethodAction { get; set; }

        public string PublicPaymentId { get; set; }

        public string ProviderAccountName { get; set; }

        public string Status { get; set; }

        public virtual ICollection<PaymentHistoryModel> PaymentHistories { get; set; }
    }
}
