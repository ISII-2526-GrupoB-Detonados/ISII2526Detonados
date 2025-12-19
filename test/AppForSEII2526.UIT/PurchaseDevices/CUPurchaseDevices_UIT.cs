using AppForMovies.UIT.Shared;
using AppForSEII2526.UIT.PurchaseDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.PurchaseDevices
{
    public class CUPurchaseDevices_UIT : UC_UIT
    {
        private ListDevicesForPurchase_PO ListDevicesForPurchase_PO;

        public CUPurchaseDevices_UIT(ITestOutputHelper output) : base(output)
        {
            ListDevicesForPurchase_PO = new ListDevicesForPurchase_PO(_driver, _output);
        }

        // Constantes para dispositivos de prueba
        private const int deviceID1 = 1;
        private const string deviceName1 = "Xiaomi x3 Premium";
        private const string deviceColor1 = "Black";
        private const string devicePriceForPurchase1 = "299,99";

        private const string deviceName2 = "Samsung Galaxy S24 Ultra";
        private const string deviceColor2 = "White";
        private const string devicePriceForPurchase2 = "1.199,99";

        private void Precondition_perform_login()
        {
            Perform_login("alejandro.jara@email.com", "Password123!");
        }

        //FLUJO BÁSICO
        [Theory]
        [InlineData("CreditCard")]
        [InlineData("PayPal")]
        [InlineData("Cash")]
        public void UC1_Flujo_Basico_MetodoPago(string metodoPago)
        {
            // Arrange
            var listDevices_PO = new ListDevicesForPurchase_PO(_driver, _output);
            var createPurchase_PO = new CreatePurchase_PO(_driver, _output);
            var detailPurchase_PO = new DetailPurchase_PO(_driver, _output);

            string nombreCliente = "alejandro.jara@email.com";
            string apellidoCliente = "Jara Sánchez";
            string direccionEntrega = "Calle La Feria, 23";
            string descripcionDispositivo = "mola";
            string precioTotal = "299,99";

            // Act
            Precondition_perform_login();
            System.Threading.Thread.Sleep(2000);

            // 1. Navegar hasta la página de Comprar Dispositivos
            ListDevicesForPurchase_PO.WaitForBeingVisible(By.LinkText("Purchase"));
            _driver.FindElement(By.LinkText("Purchase")).Click();

            // 2. Seleccionar el dispositivo
            listDevices_PO.SelectDevices(new List<string>() { deviceName1 });

            // 3. Pulsar el botón de comprar
            listDevices_PO.Purchase();

            // 4. Rellenar los datos del cliente
            createPurchase_PO.setDatos(nombreCliente, apellidoCliente, direccionEntrega, metodoPago);
            createPurchase_PO.setDescripcionDispositivo(deviceID1, descripcionDispositivo);

            // 5. Pulsar comprar para finalizar la compra
            createPurchase_PO.Comprar();
            createPurchase_PO.ConfirmarPedido();

            System.Threading.Thread.Sleep(2000);

            // Assert
            // 6. Verificar los detalles de la compra
            Assert.True(detailPurchase_PO.CheckPurchaseDetail($"{nombreCliente} Jara",
                direccionEntrega, metodoPago, precioTotal));
        }

        //EXAMEN==================================================================================
        [Fact]
        public void ExamenSprint3()
        {
            // Arrange
            var listDevices_PO = new ListDevicesForPurchase_PO(_driver, _output);
            var createPurchase_PO = new CreatePurchase_PO(_driver, _output);
            var detailPurchase_PO = new DetailPurchase_PO(_driver, _output);
            string nombreCliente = "alejandro.jara@email.com";
            string apellidoCliente = "Jara Sánchez";
            string direccionEntrega = "Calle La Feria, 23";
            string descripcionDispositivo = "mola";
            string precioTotal = "299,99";
            string metodoPago = "CreditCard";

            // Act INICIAR SESIÓN
            Precondition_perform_login();
            System.Threading.Thread.Sleep(2000);

            // 1. Navegar hasta la página de Comprar Dispositivos
            ListDevicesForPurchase_PO.WaitForBeingVisible(By.LinkText("Purchase"));
            _driver.FindElement(By.LinkText("Purchase")).Click();

            // 2. Seleccionar el dispositivo
            listDevices_PO.SelectDevices(new List<string>() { deviceName2 });

            // 3. Filtrar por model
            listDevices_PO.FilterDevices("", deviceName1);

            // 4. Seleccionar por filtro
            listDevices_PO.SelectDevices(new List<string>() { deviceName1 });

            // 5. Eliminar primero
            listDevices_PO.DeselectDevice(deviceName2);

            // 6. Pulsar el botón de comprar
            listDevices_PO.Purchase();

            // 7. Rellenar los datos del cliente
            createPurchase_PO.setDatos(nombreCliente, apellidoCliente, direccionEntrega, metodoPago);
            createPurchase_PO.setDescripcionDispositivo(deviceID1, descripcionDispositivo);

            // 8. Pulsar comprar para finalizar la compra
            createPurchase_PO.Comprar();
            createPurchase_PO.ConfirmarPedido();

            System.Threading.Thread.Sleep(2000);

            // Assert
            // 9. Verificar los detalles de la compra
            Assert.True(detailPurchase_PO.CheckPurchaseDetail($"{nombreCliente} Jara",
                direccionEntrega, metodoPago, precioTotal));
        }
        
        //========================================================================================
        //FLUJO ALTERNATIVO 0 - No hay dispositivos disponibles
        [Fact]
        public void UC2_FA0_NoHayDispositivosDisponibles()
        {
            // Arrange
            var textoEsperado = "No devices available";
            var listDevices_PO = new ListDevicesForPurchase_PO(_driver, _output);

            // Act
            Precondition_perform_login();
            System.Threading.Thread.Sleep(2000);

            // Navegar hasta la página de Comprar Dispositivos
            ListDevicesForPurchase_PO.WaitForBeingVisible(By.LinkText("Purchase"));
            _driver.FindElement(By.LinkText("Purchase")).Click();

            System.Threading.Thread.Sleep(500);

            // Assert
            Assert.True(listDevices_PO.CheckErrorMessageNotAvailableDevices(textoEsperado));
        }

        //FLUJO ALTERNATIVO 1 - Filtrar dispositivos por color y nombre
        [Theory]
        [InlineData(deviceColor1, deviceName1, deviceColor1, "")]
        [InlineData(deviceName2, "", "", deviceName2)]
        public void UC3_FA1_FiltradoDispositivos(string resultadoEsperado, string nombreEsperado, string filtroColor, string filtroNombre)
        {
            // Arrange

            var listDevices_PO = new ListDevicesForPurchase_PO(_driver, _output);

            // Datos esperados según el filtro
            List<string[]> dispositivosEsperados = new List<string[]>();

            if (filtroColor == deviceColor1) // Filtro por color Black
            {
                dispositivosEsperados.Add(new string[] {
            "Xiaomi",
            deviceColor1,
            deviceName1,
            "Xiaomi x3",
            devicePriceForPurchase1
                });
            }
            else if (filtroNombre == deviceName2) // Filtro por nombre Samsung Galaxy S24 Ultra
            {
                dispositivosEsperados.Add(new string[] {
            "Samsung",
            deviceColor2,
            deviceName2,
            "Samsung Galaxy S24",
            devicePriceForPurchase2
                });
            }

            // Act
            Precondition_perform_login();
            System.Threading.Thread.Sleep(2000);

            ListDevicesForPurchase_PO.WaitForBeingVisible(By.LinkText("Purchase"));
            _driver.FindElement(By.LinkText("Purchase")).Click();

            // Filtrar los dispositivos
            listDevices_PO.FilterDevices(filtroColor, filtroNombre);

            // Assert
            // Comprobar que la lista de dispositivos que ha devuelto es la correcta.
            Assert.True(listDevices_PO.CheckListOfDevices(dispositivosEsperados));
        }

        //FLUJO ALTERNATIVO 3 - Modificar carrito y actualizar precio
        [Fact]
        public void UC4_FA3_ModificarCarritoYActualizarPrecio()
        {
            // Arrange
            var listDevices_PO = new ListDevicesForPurchase_PO(_driver, _output);

            // Precios esperados
            string precioXiaomi = "299,99";
            string precioSamsung = "1.199,99";
            string precioTotalDosDispositivos = "1.499,98";

            // Act
            Precondition_perform_login();
            System.Threading.Thread.Sleep(2000);

            ListDevicesForPurchase_PO.WaitForBeingVisible(By.LinkText("Purchase"));
            _driver.FindElement(By.LinkText("Purchase")).Click();

            // 1. Seleccionar dos dispositivos
            listDevices_PO.SelectDevices(new List<string>() { deviceName1, deviceName2 });

            System.Threading.Thread.Sleep(1000);

            // 2. Verificar que el total del carrito es la suma de ambos precios
            var totalActualDos = listDevices_PO.ObtenerTotalCarrito();
            _output.WriteLine($"Total con 2 dispositivos: {totalActualDos}");
            Assert.Contains(precioTotalDosDispositivos, totalActualDos);

            // 3. Eliminar un dispositivo del carrito
            listDevices_PO.DeselectDevice(deviceName2);

            System.Threading.Thread.Sleep(1000);

            // 4. Verificar que el total se actualizó (solo queda Xia)
            var totalActualUno = listDevices_PO.ObtenerTotalCarrito();
            _output.WriteLine($"Total con 1 dispositivo: {totalActualUno}");
            Assert.Contains(precioXiaomi, totalActualUno);

        }

        //FLUJO ALTERNATIVO 4 - Botón comprar no disponible
        [Fact]
        public void UC5_FA4_BotonComprarNoDisponible()
        {
            // Arrange
            var listDevices_PO = new ListDevicesForPurchase_PO(_driver, _output);

            // Act
            Precondition_perform_login();
            System.Threading.Thread.Sleep(2000);

            ListDevicesForPurchase_PO.WaitForBeingVisible(By.LinkText("Purchase"));
            _driver.FindElement(By.LinkText("Purchase")).Click();

            // Assert - El botón de comprar debería estar deshabilitado cuando el carrito está vacío
            Assert.False(listDevices_PO.IsEnabledPurchase());
        }

        //FLUJO ALTERNATIVO 5 - Faltan datos obligatorios
        [Theory]
        [InlineData("", "Calle La Feria, 23", "CreditCard", "The CustomerNameSurname field is required")]
        [InlineData("Jara", "Calle La Feria, 23", "CreditCard", "The field CustomerNameSurname must be a string with a minimum length of 10 and a maximum length of 50.")]
        [InlineData("Jara Sánchez", "", "CreditCard", "The DeliveryAddress field is required")]
        [InlineData("Jara Sánchez", "Calle", "CreditCard", "The field DeliveryAddress must be a string with a minimum length of 10 and a maximum length of 50.")]
        public void UC6_FA5_FaltanDatosObligatorios(string apellido, string direccion, string metodoPago, string mensajeError)
        {
            // Arrange
            var listDevices_PO = new ListDevicesForPurchase_PO(_driver, _output);
            var createPurchase_PO = new CreatePurchase_PO(_driver, _output);

            // Act
            Precondition_perform_login();
            System.Threading.Thread.Sleep(2000);

            ListDevicesForPurchase_PO.WaitForBeingVisible(By.LinkText("Purchase"));
            _driver.FindElement(By.LinkText("Purchase")).Click();

            // Seleccionar dispositivo
            listDevices_PO.SelectDevices(new List<string>() { deviceName1 });
            listDevices_PO.Purchase();

            // Rellenar datos con campos malos
            createPurchase_PO.setDatos("alejandro.jara@email.com", apellido, direccion, metodoPago);

            // Intentar comprar
            createPurchase_PO.Comprar();
            System.Threading.Thread.Sleep(2000);

            // Assert
            Assert.True(createPurchase_PO.MensajeError(mensajeError));
        }

        //FLUJO ALTERNATIVO 6 - Modificar dispositivos conservando datos
        [Fact]
        public void UC7_FA6_ModificarDispositivosConservandoDatos()
        {
            // Arrange
            var listDevices_PO = new ListDevicesForPurchase_PO(_driver, _output);
            var createPurchase_PO = new CreatePurchase_PO(_driver, _output);

            string nombre = "alejandro.jara@email.com";
            string apellido = "Jara Sánchez";
            string direccion = "Calle La Feria, 23";
            string metodoPago = "CreditCard";

            // Act
            Precondition_perform_login();
            System.Threading.Thread.Sleep(2000);

            ListDevicesForPurchase_PO.WaitForBeingVisible(By.LinkText("Purchase"));
            _driver.FindElement(By.LinkText("Purchase")).Click();

            // 1. Seleccionar dispositivos
            listDevices_PO.SelectDevices(new List<string>() { deviceName1, deviceName2 });
            listDevices_PO.Purchase();

            // 2. Rellenar datos
            createPurchase_PO.setDatos(nombre, apellido, direccion, metodoPago);

            // 3. Volver a modificar
            createPurchase_PO.Volver();

            // 4. Eliminar un dispositivo
            listDevices_PO.DeselectDevice(deviceName1);

            // 5. Volver a comprar
            listDevices_PO.Purchase();

            System.Threading.Thread.Sleep(2000);

            // Assert
            string nombreActual = createPurchase_PO.ObtenerValorCampo("name");
            string apellidoActual = createPurchase_PO.ObtenerValorCampo("surname");
            string direccionActual = createPurchase_PO.ObtenerValorCampo("address");
            string metodoPagoActual = createPurchase_PO.ObtenerValorCampo("payment");

            _output.WriteLine($"Datos conservados:");
            _output.WriteLine($"  Nombre: Esperado='{nombre}' | Actual='{nombreActual}'");
            _output.WriteLine($"  Apellido: Esperado='{apellido}' | Actual='{apellidoActual}'");
            _output.WriteLine($"  Dirección: Esperado='{direccion}' | Actual='{direccionActual}'");
            _output.WriteLine($"  Método Pago: Esperado='{metodoPago}' | Actual='{metodoPagoActual}'");

            // Verificar que todos los datos se conservaron
            bool datosConservados = nombreActual.Contains(nombre) &&
                                   apellidoActual.Contains(apellido) &&
                                   direccionActual.Contains(direccion) &&
                                   metodoPagoActual.Contains(metodoPago);

            Assert.True(datosConservados);

            Assert.True(createPurchase_PO.isEnabledComprar());
        }
    }
}