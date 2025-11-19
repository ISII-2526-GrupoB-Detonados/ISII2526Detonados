using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.Rentals_DTO;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.RentalController_test
{
    public class GetRental_test : AppForSEII2526SqliteUT
    {
        public GetRental_test()
        {
            var model = new Model { Id = 1, NameModel = "Google Pixel 8" };
            var devices = new List<Device>()
            {
                new Device
                {
                    Id = 1,
                    Brand = "Google",
                    Color = "Azul",
                    Name = "Pixel 8",
                    PriceForPurchase = 699.99,
                    PriceForRent = 29.99,
                    QuantityForPurchase = 10,
                    QuantityForRent = 5,
                    Year = 2023,
                    Model = model
                }
            };

            ApplicationUser user = new ApplicationUser
            {
                Id = "user1",
                Name = "Luis Lorenzo",
                UserName = "luis.lorenzo@alu.uclm.es",
                Surname = "López",
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

            _context.Add(model);
            _context.AddRange(devices);
            _context.Add(user);
            _context.Add(rental);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRental_NotFound_test()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<RentalDeviceController>>();
            var controller = new RentalDeviceController(_context, mockLogger.Object);

            //Act
            var result = await controller.GetRental(0); // ID que no existe

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRental_Found_test()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<RentalDeviceController>>();
            ILogger<RentalDeviceController> logger = mockLogger.Object;
            var controller = new RentalDeviceController(_context, logger);

            var expectedRental = new RentalForCreateDTO(
                "luis.lorenzo@alu.uclm.es",
                "Luis Lorenzo López",
                "Calle La Roda, 20",
                PaymentMethod.CreditCard,
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(8),
                new List<RentalItemDTO>()
            );

            expectedRental.RentalItems.Add(new RentalItemDTO(
                1,                      // DeviceQuantity
                "Google Pixel 8",       // DeviceModel
                1,                      // DeviceId
                "Pixel 8",              // DeviceName
                29.99,                  // PriceForRenting
                "Google"                // Brand
            ));

            //Act
            var result = await controller.GetRental(1); // ID que existe

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var rentalDTOActual = Assert.IsType<RentalForCreateDTO>(okResult.Value);
            var eq = expectedRental.Equals(rentalDTOActual);
            Assert.Equal(expectedRental, rentalDTOActual);
        }
    }
}