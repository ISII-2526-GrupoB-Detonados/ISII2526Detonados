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
    public class DeviceController4Rental
    {
        private readonly AppForSEII2526DbContext _context;

        public DeviceController4Rental()
        {
            // Configurar DbContext en memoria para las pruebas
            var options = new DbContextOptionsBuilder<AppForSEII2526DbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_Rental_" + Guid.NewGuid())
                .Options;

            _context = new AppForSEII2526DbContext(options);

            // Aquí deberías seedear los datos necesarios para las pruebas
            // _context.Devices.AddRange(...);
            // _context.SaveChanges();
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDevicesForRental_OK))]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForRental_OK(string? filterModel, double? filterMaxPrice, List<Device_DTO_Alquilar> expectedDevices)
        {
            //Arrange
            var mockLogger = new Mock<ILogger<DeviceControllerRentals>>();
            ILogger<DeviceControllerRentals> logger = mockLogger.Object;
            var controller = new DeviceControllerRentals(_context, logger);

            //Act
            var result = await controller.GetDevicesAlquilarDTOs(filterModel, filterMaxPrice);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var devicesDTOsActual = Assert.IsType<List<Device_DTO_Alquilar>>(okResult.Value);
            Assert.Equal(expectedDevices, devicesDTOsActual);
        }

        public static IEnumerable<object[]> TestCasesFor_GetDevicesForRental_OK()
        {
            var devicesDTOs = new List<Device_DTO_Alquilar>()
            {
                new Device_DTO_Alquilar(1, "Azul", "Pixel 8", 29.99, 2023, "Google Pixel 8", "Google"),
                new Device_DTO_Alquilar(2, "Naranja", "iPhone 17", 59.99, 2025, "iPhone 17", "Apple"),
                new Device_DTO_Alquilar(3, "Negro", "Redmi Note 14", 19.99, 2024, "Redmi Note 14", "Xiaomi")
            };

            // Orden natural (sin filtros) - todos los dispositivos
            var naturalOrder = new List<Device_DTO_Alquilar>()
            {
                devicesDTOs[0], // Google - ID 1
                devicesDTOs[1], // Apple - ID 2  
                devicesDTOs[2]  // Xiaomi - ID 3
            };

            // Filtrado por modelo "iPhone"
            var model_iPhone = new List<Device_DTO_Alquilar>()
            {
                devicesDTOs[1] // iPhone 17
            };

            // Filtrado por precio máximo inferior a 30
            var priceBelow30 = new List<Device_DTO_Alquilar>()
            {
                devicesDTOs[0], // Google - 29.99
                devicesDTOs[2]  // Xiaomi - 19.99
            };

            var tests = new List<object[]>
            {
                new object[] { null, null, naturalOrder },           // Sin filtros
                new object[] { "iPhone", null, model_iPhone },        // Filtro por modelo
                new object[] { null, 30.0, priceBelow30 }            // Filtro por precio máximo
            };

            return tests;
        }
    }
}