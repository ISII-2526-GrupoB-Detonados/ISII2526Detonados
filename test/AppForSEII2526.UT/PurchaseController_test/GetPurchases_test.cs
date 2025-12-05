using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.Devices_DTO_Comprar_J;
using AppForSEII2526.API.DTOs.Purchase_DTO;
using AppForSEII2526.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.PurchaseController_test
{
    public class GetPurchase_test : AppForSEII2526SqliteUT
    {
        public GetPurchase_test()
        {
            var model = new Model { Id = 1, NameModel = "Google Pixel 8" };
            var devices = new List<Device>()
            {
                new Device { Id = 1, Brand = "Google", Color = "Azul", Name = "Pixel 8", PriceForPurchase = 699.99, PriceForRent = 29.99, QuantityForPurchase = 10, QuantityForRent = 5, Year = 2023, Model = model }
            };

            ApplicationUser user = new ApplicationUser
            {
                Id = "user1",
                Name = "Alejandro Jara Sánchez",
                UserName = "alejandro.jara1@alu.uclm.es",
                Surname = "Jara Sánchez",
                Email = "alejandro.jara1@alu.uclm.es"
            };

            var purchase = new Purchase
            {
                Id = 1,
                DeliveryAddress = "Calle La Roda, 20",
                PaymentMethod = PaymentMethod.CreditCard,
                PurchaseDate = DateTime.Now,
                TotalPrice = devices[0].PriceForPurchase,
                TotalQuantity = 1,
                ApplicationUser = user,
                PurchaseItems = new List<PurchaseItem>
                {
                    new PurchaseItem { DeviceId = devices[0].Id, Quantity = 1, Price = devices[0].PriceForPurchase, Description = "Nuevo modelo con mejoras" }
                }
            };

            _context.Add(model);
            _context.AddRange(devices);
            _context.Add(user);
            _context.Add(purchase);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPurchase_NotFound_test()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<PurchaseController>>();
            var controller = new PurchaseController(_context, mockLogger.Object);

            //Act
            var result = await controller.GetPurchase(0); // ID que no existe

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPurchase_Found_test()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<PurchaseController>>();
            ILogger<PurchaseController> logger = mockLogger.Object;
            var controller = new PurchaseController(_context, logger);
            var expectedPurchase = new Purchase_Detail_DTO(1, DateTime.Now, "alejandro.jara1@alu.uclm.es", "Jara Sánchez", "Calle La Roda, 20", PaymentMethod.CreditCard, new List<Purchase_Item_DTO>());
            expectedPurchase.PurchaseItems.Add(new Purchase_Item_DTO(1, "Pixel 8", 699.99, "Google", "Azul", "Google Pixel 8", 1, "Nuevo modelo con mejoras"));


            //Act
            var result = await controller.GetPurchase(1); // ID que existe

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var purchaseDTOActual = Assert.IsType<Purchase_Detail_DTO>(okResult.Value);
            var eq = expectedPurchase.Equals(purchaseDTOActual);
            Assert.Equal(expectedPurchase, purchaseDTOActual);
        }

        
    }
}
