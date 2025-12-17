using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.Purchase_DTO;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.PurchaseController_test
{
    public class PostPurchases_test : AppForSEII2526SqliteUT
    {
        private const string _userName = "alejandro.jara1@uclm.es";
        private const string _customerName = "Alejandro";
        private const string _customerSurname = "Jara Sánchez";
        private const string deliveryAddress = "Calle La Roda, 20";

        private const string _model1Name = "Google Pixel 8";
        private const string _model2Name = "iPhone 17";

        public PostPurchases_test()
        {

            var models = new List<Model>()
            {
                new Model { Id = 1, NameModel = _model1Name },
                new Model { Id = 2, NameModel = _model2Name }
            };

            var devices = new List<Device>()
            {
                new Device { Id = 1, Brand = "Google", Color = "Azul", Name = "Pixel 8", PriceForPurchase = 699.99, PriceForRent = 29.99, QuantityForPurchase = 10, QuantityForRent = 5, Year = 2023, Model = models[0] },
                new Device { Id = 2, Brand = "Apple", Color = "Naranja", Name = "iPhone 17", PriceForPurchase = 999.99, PriceForRent = 59.99, QuantityForPurchase = 15, QuantityForRent = 7, Year = 2025, Model = models[1] }
            };

            ApplicationUser user = new ApplicationUser
            {
                Id = "user1",
                Name = _customerName,
                UserName = _userName,
                Surname = _customerSurname,
                Email = _userName
            };

            var purchase = new Purchase
            {
                Id = 1,
                DeliveryAddress = deliveryAddress,
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

            _context.AddRange(models);
            _context.AddRange(devices);
            _context.Add(user);
            _context.Add(purchase);
            _context.SaveChanges();

        }

        public static IEnumerable<object[]> TestCasesFor_CreatePurchase()
        {
            var purchaseNoItem = new Purchase_ForCreate_DTO(_userName, _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, new List<Purchase_Item_DTO>());

            var purchaseItems = new List<Purchase_Item_DTO>() { new Purchase_Item_DTO(1, "iPhone17", 999.99, "Apple", "Naranja", _model2Name, 1, "Último modelo") };

            var purchaseApplicationUser = new Purchase_ForCreate_DTO("luis.lorenzo@alu.uclm.es", _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, purchaseItems);

            var purchaseDeviceNotExisting = new Purchase_ForCreate_DTO(_userName, _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, new List<Purchase_Item_DTO>() { new Purchase_Item_DTO(1, "iPhone17", 999.99, "Apple", "Naranja", "iPhone 18", 1, "Último modelo") });

            var purchaseDeviceNotAvailable = new Purchase_ForCreate_DTO(_userName, _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, new List<Purchase_Item_DTO>() { new Purchase_Item_DTO(1, "iPhone17", 999.99, "Apple", "Naranja", "iPhone 17", 20, "Último modelo") });
            //EXAMEN=========================
           // var purchaseBadPaymentMethod = new Purchase_ForCreate_DTO(_userName, _customerName + " " + _customerSurname, deliveryAddress,
            //    PaymentMethod.PayPal, new List<Purchase_Item_DTO>() { new Purchase_Item_DTO(1, "iPhone17", 999.99, "Apple", "Naranja", "iPhone 17", 1, "Último modelo") });
            //===============================
            var allTests = new List<object[]>
            {
                new object[] { purchaseNoItem, "Error! You must include at least one device to be purchased" },
                new object[] { purchaseApplicationUser, "Error! UserName is not registered" },
                new object[] { purchaseDeviceNotExisting, $"Error! Device {"Apple"} {"iPhone 18"} {"Naranja"} does not exist"},
                new object[] { purchaseDeviceNotAvailable, $"Error! Device {"Apple"} {"iPhone 17"} does not have enough stock. Available: {15}, Requested: {20}" },
                //EXAMEN===================
               // new object[] {purchaseBadPaymentMethod, "¡Error! Solo aceptamos compras pagadas con Tarjeta de Crédito" }
                //==========================
            };
            return allTests;
        }

        //Theory
        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreatePurchase))]
        public async Task CreatePurchase_Error_test(Purchase_ForCreate_DTO purchaseDTO, string errorExpected)
        {
            //Arrange
            var mockLogger = new Mock<ILogger<PurchaseController>>();
            ILogger<PurchaseController> logger = mockLogger.Object;

            var controller = new PurchaseController(_context, logger);

            //Act
            var result = await controller.CreatePurchase(purchaseDTO);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreatePurchase_Success_test()
        {
            //Arrange
            var mock = new Mock<ILogger<PurchaseController>>();
            ILogger<PurchaseController> logger = mock.Object;

            var controller = new PurchaseController(_context, logger);

            var purchaseItems = new List<Purchase_Item_DTO>() { new Purchase_Item_DTO(1, "iPhone17", 999.99, "Apple", "Naranja", _model2Name, 1, "Último modelo") };
            var purchaseDTO = new Purchase_ForCreate_DTO(_userName, _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, purchaseItems);
            var expectedPurchaseDetailDTO = new Purchase_Detail_DTO(2, DateTime.Now, _userName, _customerName + " " + _customerSurname,
                deliveryAddress, PaymentMethod.CreditCard, purchaseItems);

            //Act
            var result = await controller.CreatePurchase(purchaseDTO);

            //Assert
            var createResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualPurchaseDetailDTO = Assert.IsType<Purchase_Detail_DTO>(createResult.Value);

            Assert.Equal(expectedPurchaseDetailDTO, actualPurchaseDetailDTO);
        }
    }
}
