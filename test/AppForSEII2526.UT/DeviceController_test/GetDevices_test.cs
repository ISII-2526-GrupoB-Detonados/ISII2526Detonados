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
    public class GetDevices_test : AppForSEII2526SqliteUT
    {
        public GetDevices_test()
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

            var rental = new Rental
            {
                Id = 1,
                DeliveryAddress = "Calle La Roda, 20",
                PaymentMethod = PaymentMethod.CreditCard,
                RentalDate = DateTime.Now,
                RentalDateFrom = DateTime.Today.AddDays(2),
                RentalDateTo = DateTime.Today.AddDays(5),
                TotalPrice = devices[1].PriceForRent,
                ApplicationUser = user,
                RentDevices = new List<RentDevice>
                {
                    new RentDevice { DeviceId = devices[1].Id, RentId = 1 ,Quantity = 1, Price = devices[1].PriceForRent}
                }
            };

            _context.ApplicationUsers.Add(user);
            _context.Models.AddRange(models);
            _context.Devices.AddRange(devices);
            _context.Purchases.Add(purchase);
            _context.Rentals.Add(rental);
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
                new object[] { null, "Redmi", tcName_Redmi },
                new object[] { null, null, allOrdered },
                new object[] { null, null, allOrdered },
                new object[] { "Naranja", null, Color_Naranja },
                new object[] { null, null, allOrdered },
                new object[] { "Naranja", null, Color_Naranja }
            };

            return allTests;
        }

        // public static IEnumerable<Object[]> TestCasesFor_GetDevicesForRental_OK(){}

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

        //Luis
        public static IEnumerable<object[]> TestCasesFor_GetDevicesForRental_OK()
        {
            var devicesDTOs = new List<Device_DTO_Alquilar>()
    {
        new Device_DTO_Alquilar(1, "Azul", "Pixel 8", 29.99, 2023, "Google Pixel 8", "Google"),
        new Device_DTO_Alquilar(2, "Naranja", "iPhone 17", 59.99, 2025, "iPhone 17", "Apple"),
        new Device_DTO_Alquilar(3, "Negro", "Redmi Note 14", 19.99, 2024, "Redmi Note 14", "Xiaomi")
    };

            // Orden natural
            var naturalOrder = new List<Device_DTO_Alquilar>()
    {
        devicesDTOs[0], // Google - ID 1
        devicesDTOs[1], // Apple - ID 2  
        devicesDTOs[2]  // Xiaomi - ID 3
    };

            var model_iPhone = new List<Device_DTO_Alquilar>() { devicesDTOs[1] };
            var priceBelow30 = new List<Device_DTO_Alquilar>() {
        devicesDTOs[0], // Google - 29.99
        devicesDTOs[2]  // Xiaomi - 19.99
    };

            var tests = new List<object[]>
    {
        new object[] { null, null, naturalOrder },
        new object[] { "iPhone", null, model_iPhone },
        new object[] { null, 30, priceBelow30 }
    };

            return tests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDevicesForRental_OK))]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForRental_OK(string? model, int? maxPrice, List<Device_DTO_Alquilar> expectedList)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DeviceControllerDefault>>();
            var controller = new DeviceControllerDefault(_context, mockLogger.Object);

            // Act
            var result = await controller.GetDevices(model, maxPrice);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var actualList = Assert.IsType<List<Device_DTO_Alquilar>>(ok.Value);

            Assert.Equal(expectedList.Count, actualList.Count);

            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.Equal(expectedList[i].Id, actualList[i].Id);
                Assert.Equal(expectedList[i].Brand, actualList[i].Brand);
                Assert.Equal(expectedList[i].Name, actualList[i].Name);
                Assert.Equal(expectedList[i].PriceForRent, actualList[i].PriceForRent);
                Assert.Equal(expectedList[i].Model, actualList[i].Model);
            }
        }

        //test del filtro modelo incorrecto
        [Fact]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForRental_badModel_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DeviceControllerDefault>>();
            var controller = new DeviceControllerDefault(_context, mockLogger.Object);

            // Act
            var result = await controller.GetDevices("ModeloInventado123", null);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Device_DTO_Alquilar>>(ok.Value);

            Assert.Empty(list);
        }

        //test del filtro price negativo
        [Fact]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForRental_badPrice_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DeviceControllerDefault>>();
            var controller = new DeviceControllerDefault(_context, mockLogger.Object);

            // Act
            var result = await controller.GetDevices(null, -5);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var msg = Assert.IsType<string>(bad.Value);

            Assert.Equal("El precio no puede ser negativo ❤️❤️❤️ ", msg);
        }

        //

        [Fact]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForPurchase_badName_test()
        {
            //Arrange
            var mock = new Mock<ILogger<DeviceControllerPurchases>>();
            ILogger<DeviceControllerPurchases> logger = mock.Object;
            var controller = new DeviceControllerPurchases(_context, logger);

            //Act
            var result = await controller.GetDevicesComprarDTOs(null, "Samsung Galaxy A3");

            //Assert
            var badNameResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badNameResult.Value);
            var problem = problemDetails.Errors.First().Value[0];

            Assert.Equal("No hay dispositivos con ese nombre", problem);
        }

        [Fact]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForPurchase_badColor_test()
        {
            //Arrange
            var mock = new Mock<ILogger<DeviceControllerPurchases>>();
            ILogger<DeviceControllerPurchases> logger = mock.Object;
            var controller = new DeviceControllerPurchases(_context, logger);

            //Act
            var result = await controller.GetDevicesComprarDTOs("Blanco", null);

            //Assert
            var badColorResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badColorResult.Value);
            var problem = problemDetails.Errors.First().Value[0];

            Assert.Equal("No hay dispositivos con ese color", problem);
        }
    }
}
