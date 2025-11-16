using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DevicesDTOrepa;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.RepairController_test
{
    public class GetRepairs_test : AppForSEII2526SqliteUT
    {
        public GetRepairs_test()
        {
            // Escalas: Balanza A, Balanza B, Balanza C
            var scales = new List<Scale>
            {
                new Scale { Id = 1, Name = "Balanza A" },
                new Scale { Id = 2, Name = "Balanza B" },
                new Scale { Id = 3, Name = "Balanza C" }
            };
            // Reparaciones con diferentes nombres, descripciones, costes y asociadas a las escalas creadas
            var repairs = new List<Repair>
            {
                new Repair { Id = 1, Name = "Reparación pantalla", Description = "Cambio de pantalla completa", Cost = 49.99, Scale = scales[0], ScaleId = scales[0].Id },
                new Repair { Id = 2, Name = "Reparación batería", Description = "Sustitución batería", Cost = 29.99, Scale = scales[1], ScaleId = scales[1].Id },
                new Repair { Id = 3, Name = "Reparación placa", Description = "Reparación placa base", Cost = 79.50, Scale = scales[2], ScaleId = scales[2].Id },
                // Escala para comprobar filtros
                new Repair { Id = 4, Name = "Reparación cámara", Description = "Sustitución cámara trasera", Cost = 39.00, Scale = scales[0], ScaleId = scales[0].Id }
            };
            // Inserción de datos en el contexto de pruebas
            _context.Scales.AddRange(scales);
            _context.Repairs.AddRange(repairs);
            _context.SaveChanges();
        }
        // Casos de prueba para GetRepairs_OK
        public static IEnumerable<object[]> TestCasesFor_GetRepairs_OK()
        {
            // Datos esperados para las diferentes combinaciones de filtros
            var repairsDTOs = new List<repairDTOrepa>()
            {
                new repairDTOrepa(1, "Reparación pantalla", "Cambio de pantalla completa", "Balanza A", 49.99),
                new repairDTOrepa(2, "Reparación batería", "Sustitución batería", "Balanza B", 29.99),
                new repairDTOrepa(3, "Reparación placa", "Reparación placa base", "Balanza C", 79.50)
            };
            // Lista completa ordenada por Id
            var allOrdered = new List<repairDTOrepa>
            {
                repairsDTOs[0],
                repairsDTOs[2],
                repairsDTOs[1]
            };
            // Filtros específicos
            var tcName_bateria = new List<repairDTOrepa>() { repairsDTOs[1] };
            var tcScale_BalanzaA = new List<repairDTOrepa>() { repairsDTOs[0] };
            // Combinaciones de filtros y resultados esperados
            var allTests = new List<object[]>
            {
                // nombre nulo, scale nulo 
                new object[] { null, null, allOrdered },
                new object[] { "batería", null, tcName_bateria },
                new object[] { null, "Balanza A", tcScale_BalanzaA },
                new object[] { "placa", "Balanza C", new List<repairDTOrepa>() { repairsDTOs[2] } }
            };

            return allTests;
        }
        // TEST 1: Comprobar que una petición válida devuelve Ok con la lista correcta de reparaciones.
        [Theory]
        [MemberData(nameof(TestCasesFor_GetRepairs_OK))]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairs_OK(string? filterNombre, string? filterScaleNombre, List<repairDTOrepa> expectedRepairs)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mockLogger.Object;
            var controller = new ReparacionesController(_context, logger);

            // Act
            var result = await controller.GetRepairDTO(filterNombre, filterScaleNombre);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var repairsDTOsActual = Assert.IsType<List<repairDTOrepa>>(okResult.Value);

            Assert.Equal(expectedRepairs, repairsDTOsActual);
        }
        // TEST 2: Comprobar que una petición con nombre o escala inexistente devuelve BadRequest.
        [Fact]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairs_badName_test()
        {
            // Arrange: Se crea un mock de ILogger para inyectarlo en el controlador si depender del sistema de logging real.
            var mock = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mock.Object;
            var controller = new ReparacionesController(_context, logger);

            // Act: Llamada con nombre inexistente
            var result = await controller.GetRepairDTO("Nombre inexistente", null);

            // Assert
            // El controlador actual devuelve Ok aunque no haya coincidencias.
            var badNameResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badNameResult.Value);
            var problem = problemDetails.Errors.First().Value[0];

            Assert.Equal("No hay reparaciones con ese nombre", problem);
        }
        // TEST 3: Comprobar que una petición con escala inexistente devuelve BadRequest.
        [Fact]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairs_badScale_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mock.Object;
            var controller = new ReparacionesController(_context, logger);

            // Act
            var result = await controller.GetRepairDTO(null, "Balanza inexistente");

            // Assert
            var badScaleResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badScaleResult.Value);
            var problem = problemDetails.Errors.First().Value[0];

            Assert.Equal("No hay reparaciones con esa balanza", problem);
        }
    }
}
