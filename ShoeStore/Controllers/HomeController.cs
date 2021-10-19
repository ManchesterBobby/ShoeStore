using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoeStore.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ShoeStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var orderViewModel = new OrderViewModel();
            orderViewModel.Orders = new List<Order>();
            return View(orderViewModel);
        }

        [HttpPost]
        public IActionResult OrderImport(IFormFile importFile)
        {
            var orderViewModel = new OrderViewModel();
            orderViewModel.Orders = new List<Order>();

            string fileName = Path.GetFileName(importFile.FileName);
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    orderViewModel.OrderImportFileError = "An import file was not selected";
                }
                else
                {
                    var serializer = new XmlSerializer(typeof(BigShoeDataImport));
                    using var orderFileStream = new FileStream(fileName, FileMode.Open);
                    var orders = (BigShoeDataImport)serializer.Deserialize(orderFileStream);

                    foreach (var order in orders.Order)
                    {
                        var newOrder = new Order()
                        {
                            CustomerEmail = order.CustomerEmail,
                            CustomerName = order.CustomerName,
                            Quantity = order.Quantity,
                            Notes = order.Notes,
                            Value = order.Value,
                            Size = order.Size,
                            DateRequired = order.DateRequired
                        };

                        newOrder.Errors = newOrder.ValidateOrder(newOrder);
                        newOrder.ValidOrder = string.IsNullOrWhiteSpace(newOrder.Errors) ? true : false;

                        orderViewModel.Orders.Add(newOrder);
                    }
                }

            }
            catch (Exception e)
            {
                orderViewModel.OrderImportFileError = $"Exception occurred : {e.Message} {e.StackTrace}";
            }

            return View("Index", orderViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
