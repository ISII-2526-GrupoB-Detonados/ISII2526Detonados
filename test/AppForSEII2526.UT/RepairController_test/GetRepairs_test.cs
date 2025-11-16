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
            // Reparaciones: con diferentes datos.
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

        // Casos de prueba parametrizados. Cada fila contiene:
        // filterNombre, filterScaleNombre, clearDataBeforeAct, expectOk, expectedRepairs (si expectOk), expectedProblemMessage (si !expectOk)
        public static IEnumerable<object[]> TestCasesFor_GetRepairs()
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
            // Filtros específicos que devuelven resultados
            var tcName_bateria = new List<repairDTOrepa>() { repairsDTOs[1] }; // Solo batería
            var tcScale_BalanzaA = new List<repairDTOrepa>() { repairsDTOs[0] }; // Solo Balanza A

            var allTests = new List<object[]>
            {
                // CASOS QUE DEBEN DEVOLVER OK 

                // nombre nulo, scale nulo -> lista completa
                new object[] { null, null, false, true, allOrdered, null },
                // nombre "batería" -> solo batería
                new object[] { "batería", null, false, true, tcName_bateria, null },
                // scale "Balanza A" -> reparaciones de Balanza A
                new object[] { null, "Balanza A", false, true, tcScale_BalanzaA, null },
                // nombre y scale coincidentes
                new object[] { "placa", "Balanza C", false, true, new List<repairDTOrepa>() { repairsDTOs[2] }, null },

                // CASOS QUE DEBEN DEVOLVER BadRequest 

                // nombre inexistente
                new object[] { "Nombre inexistente", null, false, false, null, "No hay reparaciones con ese nombre" },
                // escala inexistente
                new object[] { null, "Balanza inexistente", false, false, null, "No hay reparaciones con esa balanza" },
                // ambos filtros presentes pero sin coincidencias
                new object[] { "NombreNoExiste", "BalanzaNoExiste", false, false, null, "No hay reparaciones que cumplan los filtros" },

                // CASO: sin filtros y lista vacía -> debe devolver OK con lista vacía.
                new object[] { null, null, true, true, new List<repairDTOrepa>(), null }
            };

            return allTests;
        }

        // TEST parametrizado que cubre resultados OK y BadRequest según la fila de datos.
        [Theory]
        [MemberData(nameof(TestCasesFor_GetRepairs))]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairs_TestCases(string? filterNombre, string? filterScaleNombre, bool clearDataBeforeAct, bool expectOk, List<repairDTOrepa>? expectedRepairs, string? expectedProblemMessage)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mockLogger.Object;
            var controller = new ReparacionesController(_context, logger);

            // Si la fila de datos indica que debe limpiarse la tabla de reparaciones para este caso, lo hacemos.
            if (clearDataBeforeAct)
            {
                _context.Repairs.RemoveRange(_context.Repairs);
                _context.SaveChanges();
            }

            // Act
            var result = await controller.GetRepairDTO(filterNombre, filterScaleNombre);

            // Assert: bifurcamos según lo esperado en los datos
            if (expectOk)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var repairsDTOsActual = Assert.IsType<List<repairDTOrepa>>(okResult.Value);
                Assert.Equal(expectedRepairs, repairsDTOsActual);
            }
            else
            {
                var badResult = Assert.IsType<BadRequestObjectResult>(result);
                var problemDetails = Assert.IsType<ValidationProblemDetails>(badResult.Value);
                var problem = problemDetails.Errors.First().Value[0];
                Assert.Equal(expectedProblemMessage, problem);
            }
        }
    }
}
