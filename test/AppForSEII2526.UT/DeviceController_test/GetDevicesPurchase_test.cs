using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.Devices_DTO_Comprar_J;
using AppForSEII2526.API.DTOs.DevicesDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.DeviceController_test
{
    public class GetDevicesPurchase_test : AppForSEII2526SqliteUT
    {
        public GetDevicesPurchase_test()
        {
            var models = new List<Model>
            {
                new Model { Id = 1, NameModel = "Google Pixel 8" },
                new Model { Id = 2, NameModel = "iPhone 17" },
                new Model { Id = 3, NameModel = "Redmi Note 14" },
                new Model { Id = 4, NameModel = "iPhone 16 Pro Max" }
            };

            var devices = new List<Device>()
            {
                new Device { Id = 1, Brand = "Google", Color = "Azul", Name = "Pixel 8", PriceForPurchase = 699.99, PriceForRent = 29.99, QuantityForPurchase = 10, QuantityForRent = 5, Year = 2023, Model = models[0] },
                new Device { Id = 2, Brand = "Apple", Color = "Naranja", Name = "iPhone 17", PriceForPurchase = 999.99, PriceForRent = 59.99, QuantityForPurchase = 15, QuantityForRent = 7, Year = 2025, Model = models[1] },
                new Device { Id = 3, Brand = "Xiaomi", Color = "Negro", Name = "Redmi Note 14", PriceForPurchase = 299.99, PriceForRent = 19.99, QuantityForPurchase = 20, QuantityForRent = 10, Year = 2024, Model = models[2] },
                //Sin stock
                new Device { Id = 4, Brand = "Apple", Color = "Blanco", Name = "iPhone 16 Pro Max", PriceForPurchase = 1099.99, PriceForRent = 69.99, QuantityForPurchase = 0, QuantityForRent = 0, Year = 2022, Model = models[3] }
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

            _context.ApplicationUsers.Add(user);
            _context.Models.AddRange(models);
            _context.Devices.AddRange(devices);
            _context.Purchases.Add(purchase);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetDevicesForPurchase_OK()
        {
            var devicesDTOs = new List<Device_DTO_Comprar>()
            {
                new Device_DTO_Comprar { Id = 1, Brand = "Google", Color = "Azul", Name = "Pixel 8", PriceForPurchase = 699.99, Model = "Google Pixel 8" },
                new Device_DTO_Comprar { Id = 2, Brand = "Apple", Color = "Naranja", Name = "iPhone 17", PriceForPurchase = 999.99, Model = "iPhone 17" },
                new Device_DTO_Comprar { Id = 3, Brand = "Xiaomi", Color = "Negro", Name = "Redmi Note 14", PriceForPurchase = 299.99, Model = "Redmi Note 14" }
            };

            var allOrdered = new List<Device_DTO_Comprar>
            {
                devicesDTOs[0], 
                devicesDTOs[2],   
                devicesDTOs[1] 
            };

            var tcName_iPhone = new List<Device_DTO_Comprar>() { devicesDTOs[1] };
            var tcName_Redmi = new List<Device_DTO_Comprar>() { devicesDTOs[2] };
            var Color_Naranja = new List<Device_DTO_Comprar>() { devicesDTOs[1] };

            var allTests = new List<object[]>
            {
                new object[] { null, null, allOrdered },
                new object[] { null, "iPhone 17", tcName_iPhone },
                new object[] { "Naranja", null, Color_Naranja }
            };

            return allTests;
        }

       

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDevicesForPurchase_OK))]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForPurchase_OK(string? filterColor, string? filterName, List<Device_DTO_Comprar> expectedDevices)
        {
            //Arrange
            var mockLogger = new Mock<ILogger<DeviceControllerPurchases>>();
            ILogger<DeviceControllerPurchases> logger = mockLogger.Object;
            var controller = new DeviceControllerPurchases(_context, logger);

            //Act
            var result = await controller.GetDevicesComprarDTOs(filterColor, filterName);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var devicesDTOsActual = Assert.IsType<List<Device_DTO_Comprar>>(okResult.Value);

            Assert.Equal(expectedDevices, devicesDTOsActual);
        }

        
    }
}
