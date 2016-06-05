using System;
using System.ComponentModel.DataAnnotations;

namespace Caribs.Domain.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } // id заказа
        [Required]
        public DateTime Date { get; set; } // дата
        [Required]
        public string Sender { get; set; } // отправитель - кошелек в ЯД
        [Required]
        public string OperationId { get; set; } // id операции в ЯД
        [Required]
        public decimal Amount { get; set; } // сумма, которую заплатали с учетом комиссии
        [Required]
        public decimal WithdrawAmount { get; set; } // сумма, которую заплатали без учета комиссии
        [Required]
        public Guid AccountId { get; set; } // id пользователя в системе, который сделал заказ
    }
}