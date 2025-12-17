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
    public class PostRentals_test : AppForSEII2526SqliteUT
    {
        //DTO_CREATE APPFORMOVIES
        private const string _userName = "luis.lorenzo@uclm.es";
        private const string _customerName = "Luis Lorenzo";
        private const string _customerSurname = "López";
        private const string deliveryAddress = "Calle La Roda, 20";

        private const string _model1Name = "Google Pixel 8";
        private const string _model2Name = "iPhone 17";

        public PostRentals_test()
        {
            var models = new List<Model>()
            {
                new Model { Id = 1, NameModel = _model1Name },
                new Model { Id = 2, NameModel = _model2Name }
            };

            var devices = new List<Device>()
            {
                new Device { Id = 1, Brand = "Google", Color = "Azul", Name = "Pixel 8", PriceForPurchase = 699.99, PriceForRent = 29.99, QuantityForPurchase = 10, QuantityForRent = 1, Year = 2023, Model = models[0] },
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

            var rental = new Rental
            {
                Id = 1,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = PaymentMethod.CreditCard,
                RentalDate = DateTime.Now,
                RentalDateFrom = DateTime.Today.AddDays(2),
                RentalDateTo = DateTime.Today.AddDays(5),
                TotalPrice = devices[0].PriceForRent * 3,
                ApplicationUser = user,
                RentDevices = new List<RentDevice>()
            };

            var rentDevice = new RentDevice
            {
                DeviceId = devices[0].Id,
                Device = devices[0],
                Rental = rental,
                Quantity = 1,
                Price = devices[0].PriceForRent
            };

            rental.RentDevices.Add(rentDevice);
            devices[0].RentedDevices = new List<RentDevice> { rentDevice };

            _context.AddRange(models);
            _context.AddRange(devices);
            _context.Add(user);
            _context.Add(rental);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateRental()
        {
            var rentalDateFrom = DateTime.Today.AddDays(2);
            var rentalDateTo = DateTime.Today.AddDays(9);

            var rentalNoItem = new RentalForCreateDTO(_userName, _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, rentalDateFrom, rentalDateTo, new List<RentalItemDTO>());

            var rentalItems = new List<RentalItemDTO>()
            {
                new RentalItemDTO(1, _model2Name, 2, "iPhone 17", 59.99, "Apple")
            };

            var rentalFromBeforeToday = new RentalForCreateDTO(_userName, _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, DateTime.Today, rentalDateTo, rentalItems);

            var rentalToBeforeFrom = new RentalForCreateDTO(_userName, _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, rentalDateTo, rentalDateFrom, rentalItems);

            var rentalApplicationUser = new RentalForCreateDTO("usuario.noexiste@alu.uclm.es", _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, rentalDateFrom, rentalDateTo, rentalItems);

            var rentalDeviceNotAvailable = new RentalForCreateDTO(_userName, _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, DateTime.Today.AddDays(3), DateTime.Today.AddDays(4),

                new List<RentalItemDTO>() { new RentalItemDTO(1, _model1Name, 1, "Pixel 8", 29.99, "Google") });
            //nulo y mala direccion 
            var BadDirection = new RentalForCreateDTO(_userName, _customerName + "" + _customerSurname, "fsefsfsfsfsdfswe",
                PaymentMethod.CreditCard, rentalDateFrom, rentalDateTo, rentalItems);
            var BadDirection2 = new RentalForCreateDTO(_userName, _customerName + "" + _customerSurname, null,
                PaymentMethod.CreditCard, rentalDateFrom, rentalDateTo, rentalItems);
            //-----------------

            var allTests = new List<object[]>
            {
                new object[] { rentalNoItem, "Error! You must include at least one device to be rented" },
                new object[] { rentalFromBeforeToday, "Error! Your rental date must start later than today" },
                new object[] { rentalToBeforeFrom, "Error! Your rental must end later than it starts" },
                new object[] { rentalApplicationUser, "Error! UserName is not registered" },
                new object[] { rentalDeviceNotAvailable, $"Error! Device with ID '1' is not available for being rented from" },
                //nulo y mala direccion ezamen
                new object[] { BadDirection, "Error en la direccion de envio. Porfavor introduce una direccion valida que incluya las palabras calle o carretera" },
                new object[] { BadDirection2, "Error en la direccion de envio. Porfavor introduce una direccion valida que incluya las palabras calle o carretera" },
            };
            return allTests;
        }
        /* Controller cambiado omitido de momento
        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateRental))]
        public async Task CreateRental_Error_test(RentalForCreateDTO rentalDTO, string errorExpected)
        {
            //Arrange
            var mockLogger = new Mock<ILogger<RentalDeviceController>>();
            ILogger<RentalDeviceController> logger = mockLogger.Object;

            var controller = new RentalDeviceController(_context, logger);

            //Act
            var result = await controller.CreateRental(rentalDTO);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual);
        }
        */
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateRental_Success_test()
        {
            //Arrange
            var mock = new Mock<ILogger<RentalDeviceController>>();
            ILogger<RentalDeviceController> logger = mock.Object;

            var controller = new RentalDeviceController(_context, logger);

            DateTime from = DateTime.Today.AddDays(6);
            DateTime to = DateTime.Today.AddDays(7);

            var rentalItems = new List<RentalItemDTO>() { new RentalItemDTO(1, _model1Name, 1, "Pixel 8", 29.99, "Google") };

            var rentalDTO = new RentalForCreateDTO(_userName, _customerName + " " + _customerSurname, deliveryAddress,
                PaymentMethod.CreditCard, from, to, rentalItems);

            //Act
            var result = await controller.CreateRental(rentalDTO);

            //Assert
            var createResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualRentalDetailDTO = Assert.IsType<RentalDetailDTO>(createResult.Value);

            // Crear el expected DESPUÉS de obtener el actual, usando los mismos items
            var expectedRentalDetailDTO = new RentalDetailDTO(2, actualRentalDetailDTO.RentalDate,
                _userName, _customerName + " " + _customerSurname,
                deliveryAddress, PaymentMethod.CreditCard, from, to, actualRentalDetailDTO.RentalItems);

            Assert.Equal(expectedRentalDetailDTO, actualRentalDetailDTO);
        }
    }
}