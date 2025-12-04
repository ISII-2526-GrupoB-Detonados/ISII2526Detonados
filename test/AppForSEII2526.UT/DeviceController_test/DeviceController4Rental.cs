using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DevicesDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.DeviceController_test
{
    public class DeviceController4Rental : AppForSEII2526SqliteUT
    {
        public DeviceController4Rental()
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
                Name = "Luis Lorenzo",
                UserName = "luis.lorenzo@alu.uclm.es",
                Surname = "Lorenzo",
                Email = "luis.lorenzo@alu.uclm.es"

            };

            var rental = new Rental
            {
                Id = 1,
                DeliveryAddress = "Calle La Roda, 20",
                PaymentMethod = PaymentMethod.CreditCard,
                RentalDate = DateTime.Now,
                RentalDateFrom = DateTime.Now.AddDays(1),
                RentalDateTo = DateTime.Now.AddDays(8),
                TotalPrice = devices[0].PriceForRent * 7, // 7 días
                ApplicationUser = user,
                RentDevices = new List<RentDevice>
                {
                    new RentDevice
                    {
                        DeviceId = devices[0].Id,
                        Quantity = 1,
                        Price = devices[0].PriceForRent
                    }
                }
            };

            _context.ApplicationUsers.Add(user);
            _context.Models.AddRange(models);
            _context.Devices.AddRange(devices);
            _context.Rentals.Add(rental);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetDevicesForRental_OK()
        {
            var devicesDTOs = new List<DeviceDTOAlquilar>()
    {
        new DeviceDTOAlquilar(1, "Azul", "Pixel 8", 29.99, 2023, "Google Pixel 8", "Google"),
        new DeviceDTOAlquilar(2, "Naranja", "iPhone 17", 59.99, 2025, "iPhone 17", "Apple"),
        new DeviceDTOAlquilar(3, "Negro", "Redmi Note 14", 19.99, 2024, "Redmi Note 14", "Xiaomi")
        // NOTA: El dispositivo con ID 4 NO está incluido porque no tiene stock para alquiler
    };

            // Ordenar por ID para consistencia
            var allOrdered = devicesDTOs.OrderBy(d => d.Id).ToList();
            var tcModel_iPhone = devicesDTOs.Where(d => d.Model == "iPhone 17").OrderBy(d => d.Id).ToList();
            var tcMaxPrice_30 = devicesDTOs.Where(d => d.PriceForRent <= 30).OrderBy(d => d.Id).ToList();

            var allTests = new List<object[]>
    {
        new object[] { null, null, allOrdered },
        new object[] { "iPhone 17", null, tcModel_iPhone },
        new object[] { null, 30, tcMaxPrice_30 }
    };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDevicesForRental_OK))]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForRental_OK(string? model, int? maxPrice, List<DeviceDTOAlquilar> expectedDevices)
        {
            //Arrange
            var mockLogger = new Mock<ILogger<DeviceControllerDefault>>();
            ILogger<DeviceControllerDefault> logger = mockLogger.Object;
            var controller = new DeviceControllerDefault(_context, logger);

            //Act
            var result = await controller.GetDevices(model, maxPrice);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var devicesDTOsActual = Assert.IsType<List<DeviceDTOAlquilar>>(okResult.Value);

            // Ordenar ambas listas por ID antes de comparar
            var expectedSorted = expectedDevices.OrderBy(d => d.Id).ToList();
            var actualSorted = devicesDTOsActual.OrderBy(d => d.Id).ToList();

            Assert.Equal(expectedSorted, actualSorted);
        }
    }
}