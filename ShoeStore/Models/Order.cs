using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStore.Models
{
    public class Order
    {
        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public short Quantity { get; set; }

        public string Notes { get; set; }

        public float Size { get; set; }

        public System.DateTime DateRequired { get; set; }

        public string Value { get; set; }

        public bool ValidOrder { get; set; }

        public string Errors { get; set; }

        public Order()
        {
            ValidOrder = true;
        }

        public string ValidateOrder(Order order)
        {
            string errorString = string.Empty;

            var validationErrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(order.CustomerName))
            {
                validationErrors.Append("Customer name is required");
            }

            if (!MailAddress.TryCreate(order.CustomerEmail, out var mailAddress))
                validationErrors.Append("Email address is invalid");

            DateTime futureDate = DateTime.Now;
            switch (futureDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    futureDate.AddDays(16);
                    break;
                case DayOfWeek.Sunday:
                    futureDate.AddDays(15);
                    break;
                default:
                    futureDate.AddDays(14);
                    break;
            }

            if (order.DateRequired < futureDate)
                validationErrors.Append("Required date must be at least 10 working days in the future");

            if (!(order.Size > 11 && order.Size <= 15 && (order.Size * 2) % 1 == 0 ))
                validationErrors.Append("Size must be 11.5 to 15 including half sizes");

            if (order.Quantity % 1000 != 0)
                validationErrors.Append("Quantity must be in multiples of 1000");

            if (validationErrors.Length > 0)
                errorString = validationErrors.ToString();

            return errorString;
        }
    }
}
