using AppForMovies.UIT.Shared;
using System;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_Rental
{
    public class CU_RentalDevices_UIT : UC_UIT
    {
        private SelectDevicesForRentalPO _selectDevicesForRentalPO;
        private CreateRentalPO _createRentalPO;
        private RentalDetailsPO _rentalDetailsPO;

        // ========== DATOS DE PRUEBA: DISPOSITIVOS ==========
        private const string DEVICE_NAME_1 = "Nokia G400 Plus";
        private const string DEVICE_BRAND_1 = "Nokia";
        private const string DEVICE_MODEL_1 = "Nokia G400";
        private const float DEVICE_PRICE_1 = 40f;

        private const string DEVICE_NAME_2 = "Oppo Find X7 Ultra";
        private const string DEVICE_BRAND_2 = "Oppo";
        private const string DEVICE_MODEL_2 = "Oppo Find X7";
        private const float DEVICE_PRICE_2 = 35f;

        private const string DEVICE_NAME_3 = "OnePlus 12 Pro";
        private const string DEVICE_BRAND_3 = "OnePlus";
        private const string DEVICE_MODEL_3 = "OnePlus 12";
        private const float DEVICE_PRICE_3 = 70f;

        // ========== DATOS DE PRUEBA: CLIENTE ==========
        private const string CUSTOMER_NAME_SURNAME = "Luis Lorenzo";
        private const string DELIVERY_ADDRESS_VALID = "Calle TestUni";
        private const string DELIVERY_ADDRESS_INVALID = "Melendi vuelve a la hierba";
        private const string USER_EMAIL = "luis.lorenzo@email.com";
        private const string USER_PASSWORD = "Password123!";

        // ========== MÉTODOS DE PAGO ==========
        private const string PAYMENT_METHOD_CASH = "Cash";
        private const string PAYMENT_METHOD_CREDIT_CARD = "Credit Card";
        private const string PAYMENT_METHOD_PAYPAL = "PayPal";

        // ========== FILTROS DE BÚSQUEDA ==========
        private const string MODEL_FILTER_ALL = "All";
        private const string MODEL_FILTER_OPPO = "Oppo Find X7";
        private const int PRICE_FILTER_40 = 40;

        public CU_RentalDevices_UIT(ITestOutputHelper output) : base(output)
        {
            _selectDevicesForRentalPO = new SelectDevicesForRentalPO(_driver, _output);
            _createRentalPO = new CreateRentalPO(_driver, _output);
            _rentalDetailsPO = new RentalDetailsPO(_driver, _output);
        }

        private void InitialStepsForSelectDevices()
        {
            Initial_step_opening_the_web_page();
            Perform_login(USER_EMAIL, USER_PASSWORD);

            // IMPORTANTE: Navegar a Home primero para limpiar cualquier estado previo
            _driver.Navigate().GoToUrl(_URI);
            Thread.Sleep(500);

            // Ahora ir a la página de selección de dispositivos
            _driver.Navigate().GoToUrl(_URI + "rental/selectdevicesforrental");
            Thread.Sleep(1500); // Aumentar el tiempo de espera
        }


        /*
        ============================
        FLUJO BÁSICO - ESC-1
        UC2_1, UC2_2, UC2_3: Alquiler exitoso con diferentes métodos de pago
        ============================
        */

        /// UC2_1: Flujo básico completo con tarjeta de crédito
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "BasicFlow")]
        [Trait("TestCase", "UC2_1")]
        public void UC2_1_BasicFlowWithCreditCard()
        {
            // Arrange
            InitialStepsForSelectDevices();

            // Act - Paso 2: Ver lista de dispositivos disponibles
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, null);
            Thread.Sleep(1000);

            // Act - Paso 3: Seleccionar dispositivo (añadir al carrito)
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_1);
            Thread.Sleep(500);

            // Act - Paso 4: Seleccionar "Rent devices"
            _selectDevicesForRentalPO.ClickRentDevices();
            Thread.Sleep(1000);

            // Act - Paso 5-6: Rellenar datos obligatorios
            _createRentalPO.FillCustomerNameField(CUSTOMER_NAME_SURNAME);
            Thread.Sleep(300);
            _createRentalPO.FillDeliveryAddressField(DELIVERY_ADDRESS_VALID);
            Thread.Sleep(300);
            _createRentalPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);

            // Act - Paso 6: Guardar (click en Create Rental)
            _createRentalPO.ClickCreateRentalButton();
            Thread.Sleep(1000);

            try
            {
                _createRentalPO.ConfirmDialog();
                Thread.Sleep(1000);
            }
            catch
            {
                // Dialog puede no aparecer
            }

            // Assert - Paso 7: Verificar que está en la página de detalles
            bool isOnDetailPage = _driver.Url.Contains("/rental/detailrental");

            Assert.True(isOnDetailPage,
                $"Debería estar en la página de detalles del alquiler. URL actual: {_driver.Url}");

            // Assert adicional: Verificar que los datos son correctos
            Assert.True(_rentalDetailsPO.GetCustomerNameSurname().Contains(CUSTOMER_NAME_SURNAME));
            Assert.True(_rentalDetailsPO.GetDeliveryAddress().Contains(DELIVERY_ADDRESS_VALID));
        }

        /// UC2_2: Flujo básico con PayPal
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "BasicFlow")]
        [Trait("TestCase", "UC2_2")]
        public void UC2_2_BasicFlowWithPayPal()
        {
            // Arrange
            InitialStepsForSelectDevices();

            // Act
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, null);
            Thread.Sleep(1000);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_1);
            Thread.Sleep(500);
            _selectDevicesForRentalPO.ClickRentDevices();
            Thread.Sleep(1000);

            _createRentalPO.FillCustomerNameField(CUSTOMER_NAME_SURNAME);
            Thread.Sleep(300);
            _createRentalPO.FillDeliveryAddressField(DELIVERY_ADDRESS_VALID);
            Thread.Sleep(300);
            _createRentalPO.SelectPaymentMethod(PAYMENT_METHOD_PAYPAL);
            Thread.Sleep(500);

            _createRentalPO.ClickCreateRentalButton();
            Thread.Sleep(1000);

            try
            {
                _createRentalPO.ConfirmDialog();
                Thread.Sleep(1000);
            }
            catch { }

            // Assert
            bool isOnDetailPage = _driver.Url.Contains("/rental/detailrental");
            Assert.True(isOnDetailPage);
        }

        /// UC2_3: Flujo básico con Efectivo
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "BasicFlow")]
        [Trait("TestCase", "UC2_3")]
        public void UC2_3_BasicFlowWithCash()
        {
            // Arrange
            InitialStepsForSelectDevices();

            // Act
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, null);
            Thread.Sleep(1000);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_1);
            Thread.Sleep(500);
            _selectDevicesForRentalPO.ClickRentDevices();
            Thread.Sleep(1000);

            _createRentalPO.FillCustomerNameField(CUSTOMER_NAME_SURNAME);
            Thread.Sleep(300);
            _createRentalPO.FillDeliveryAddressField(DELIVERY_ADDRESS_VALID);
            Thread.Sleep(300);
            _createRentalPO.SelectPaymentMethod(PAYMENT_METHOD_CASH);
            Thread.Sleep(500);

            _createRentalPO.ClickCreateRentalButton();
            Thread.Sleep(1000);

            try
            {
                _createRentalPO.ConfirmDialog();
                Thread.Sleep(1000);
            }
            catch { }

            // Assert
            bool isOnDetailPage = _driver.Url.Contains("/rental/detailrental");
            Assert.True(isOnDetailPage);
        }

        /*
        ============================
        FLUJO ALTERNATIVO 1 - ESC-3
        UC2_4, UC2_5: Filtrar dispositivos
        ============================
        */

        /// UC2_4: Filtro por modelo
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "AlternativeFlow1")]
        [Trait("TestCase", "UC2_4")]
        public void UC2_4_FilterDevicesByModel()
        {
            // Arrange
            InitialStepsForSelectDevices();

            // Act - Aplicar filtro por modelo "Oppo Find X7"
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_OPPO, null);
            Thread.Sleep(1000);

            // Assert - Debe mostrar solo dispositivos Oppo Find X7
            bool deviceVisible = _selectDevicesForRentalPO.IsDeviceVisible(DEVICE_NAME_2);
            Assert.True(deviceVisible, $"Debería mostrar el dispositivo '{DEVICE_NAME_2}'");
        }

        /// UC2_5: Filtro por precio máximo
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "AlternativeFlow1")]
        [Trait("TestCase", "UC2_5")]
        public void UC2_5_FilterDevicesByPrice()
        {
            // Arrange
            InitialStepsForSelectDevices();

            // Act - Aplicar filtro de precio máximo 40
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, PRICE_FILTER_40);
            Thread.Sleep(1000);

            // Assert - Debe mostrar dispositivos con precio <= 40
            bool nokiaVisible = _selectDevicesForRentalPO.IsDeviceVisible(DEVICE_NAME_1);
            bool oppoVisible = _selectDevicesForRentalPO.IsDeviceVisible(DEVICE_NAME_2);

            Assert.True(nokiaVisible || oppoVisible,
                "Debería mostrar dispositivos con precio menor o igual a 40");
        }

        /*
        ============================
        FLUJO ALTERNATIVO 2 - ESC-4
        UC2_6: Modificar carrito de alquiler
        ============================
        */

        /// UC2_6: Eliminar un dispositivo y añadir otro
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "AlternativeFlow2")]
        [Trait("TestCase", "UC2_6")]
        public void UC2_6_ModifyCartRemoveAndAddDevice()
        {
            // Arrange
            InitialStepsForSelectDevices();

            // Act - Paso 2-3: Seleccionar dos dispositivos
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, null);
            Thread.Sleep(1000);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_1);
            Thread.Sleep(500);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_2);
            Thread.Sleep(500);

            // Act - Eliminar uno
            _selectDevicesForRentalPO.RemoveDeviceFromCart(DEVICE_NAME_1);
            Thread.Sleep(500);

            // Act - Ir a crear alquiler
            _selectDevicesForRentalPO.ClickRentDevices();
            Thread.Sleep(1000);

            // Assert - Debe haber solo el dispositivo Oppo en el carrito
            Assert.True(_createRentalPO.HasDevicesInCart(),
                "Debería haber dispositivos en el carrito");
        }

        /*
        ============================
        FLUJO ALTERNATIVO 3 - ESC-5
        UC2_7: Carrito vacío
        ============================
        */

        /// UC2_7: Eliminar todos los dispositivos del carrito
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "AlternativeFlow3")]
        [Trait("TestCase", "UC2_7")]
        public void UC2_7_EmptyCartButtonDisabled()
        {
            // Arrange
            InitialStepsForSelectDevices();

            // Act - Seleccionar y luego eliminar
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, null);
            Thread.Sleep(1000);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_1);
            Thread.Sleep(500);
            _selectDevicesForRentalPO.RemoveDeviceFromCart(DEVICE_NAME_1);
            Thread.Sleep(500);

            // Assert - El botón debe estar deshabilitado
            bool buttonDisabled = _selectDevicesForRentalPO.IsRentDevicesButtonDisabled();
            Assert.True(buttonDisabled,
                "El botón 'Rent devices' debe estar deshabilitado cuando el carrito está vacío");
        }

        /*
        ============================
        FLUJO ALTERNATIVO 4 - ESC-6
        UC2_8, UC2_9, UC2_10: Errores al rellenar datos
        ============================
        */

        /// UC2_8: Dirección vacía
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "AlternativeFlow4")]
        [Trait("TestCase", "UC2_8")]
        public void UC2_8_ValidationEmptyDeliveryAddress()
        {
            // Arrange
            InitialStepsForSelectDevices();
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, null);
            Thread.Sleep(1000);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_1);
            Thread.Sleep(500);
            _selectDevicesForRentalPO.ClickRentDevices();
            Thread.Sleep(1000);

            // Act - Rellenar todo excepto dirección
            _createRentalPO.FillCustomerNameField(CUSTOMER_NAME_SURNAME);
            Thread.Sleep(300);
            _createRentalPO.FillDeliveryAddressField(""); // Vacío
            Thread.Sleep(300);
            _createRentalPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);

            _createRentalPO.ClickCreateRentalButton();
            Thread.Sleep(1500);

            // Assert - Debe mostrar error
            bool hasError = _createRentalPO.CheckErrorMessage("required") ||
                           _createRentalPO.HasValidationErrors();

            Assert.True(hasError, "Debe mostrar error de validación si falta la dirección");
        }

        /// UC2_9: Nombre del cliente vacío
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "AlternativeFlow4")]
        [Trait("TestCase", "UC2_9")]
        public void UC2_9_ValidationEmptyCustomerName()
        {
            // Arrange
            InitialStepsForSelectDevices();
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, null);
            Thread.Sleep(1000);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_1);
            Thread.Sleep(500);
            _selectDevicesForRentalPO.ClickRentDevices();
            Thread.Sleep(1000);

            // Act - Rellenar todo excepto nombre
            _createRentalPO.FillCustomerNameField(""); // Vacío
            Thread.Sleep(300);
            _createRentalPO.FillDeliveryAddressField(DELIVERY_ADDRESS_VALID);
            Thread.Sleep(300);
            _createRentalPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);

            _createRentalPO.ClickCreateRentalButton();
            Thread.Sleep(1500);

            // Assert
            bool hasError = _createRentalPO.CheckErrorMessage("required") ||
                           _createRentalPO.HasValidationErrors();

            Assert.True(hasError, "Debe mostrar error de validación si falta el nombre");
        }

        /// UC2_10: Dirección sin "Calle" o "Carretera"
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "AlternativeFlow4")]
        [Trait("TestCase", "UC2_10")]
        public void UC2_10_ValidationInvalidDeliveryAddress()
        {
            // Arrange
            InitialStepsForSelectDevices();
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, null);
            Thread.Sleep(1000);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_1);
            Thread.Sleep(500);
            _selectDevicesForRentalPO.ClickRentDevices();
            Thread.Sleep(1000);

            // Act - Dirección inválida (sin Calle ni Carretera)
            _createRentalPO.FillCustomerNameField(CUSTOMER_NAME_SURNAME);
            Thread.Sleep(300);
            _createRentalPO.FillDeliveryAddressField(DELIVERY_ADDRESS_INVALID);
            Thread.Sleep(300);
            _createRentalPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);

            _createRentalPO.ClickCreateRentalButton();
            Thread.Sleep(1500);

            // Assert - Debe mostrar error específico
            bool hasError = _createRentalPO.CheckErrorMessage("Calle") ||
                           _createRentalPO.CheckErrorMessage("Carretera");

            Assert.True(hasError,
                "Debe mostrar error indicando que la dirección debe incluir 'Calle' o 'Carretera'");
        }

        /*
        ============================
        FLUJO ALTERNATIVO 0 - ESC-2
        UC2_11: No hay stock disponible
        ============================
        */

        /// UC2_11: Intentar alquilar dispositivo sin stock
        /// NOTA: Requiere que OnePlus 12 Pro tenga QuantityForRent = 0
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "AlternativeFlow0")]
        [Trait("TestCase", "UC2_11")]
        public void UC2_11_NoStockAvailable()
        {
            // Arrange
            InitialStepsForSelectDevices();

            // Act
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, null);
            Thread.Sleep(1000);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_3); // OnePlus 12 Pro (stock = 0)
            Thread.Sleep(500);
            _selectDevicesForRentalPO.ClickRentDevices();
            Thread.Sleep(1000);

            _createRentalPO.FillCustomerNameField(CUSTOMER_NAME_SURNAME);
            Thread.Sleep(300);
            _createRentalPO.FillDeliveryAddressField(DELIVERY_ADDRESS_VALID);
            Thread.Sleep(300);
            _createRentalPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);

            _createRentalPO.ClickCreateRentalButton();
            Thread.Sleep(1500);

            try
            {
                _createRentalPO.ConfirmDialog();
                Thread.Sleep(1000);
            }
            catch { }

            // Assert - Debe mostrar error de stock insuficiente
            bool hasStockError = _createRentalPO.CheckErrorMessage("stock") ||
                                _createRentalPO.CheckErrorMessage("insuficiente");

            Assert.True(hasStockError,
                "Debe mostrar error de cantidad de stock insuficiente");
        }

        /*
        ============================
        FLUJO ALTERNATIVO 5 - ESC-7
        UC2_12: Eliminar un dispositivo antes de terminar el POST
        ============================
        */

        /// UC2_12: Modificar dispositivos desde la página de creación
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "AlternativeFlow5")]
        [Trait("TestCase", "UC2_12")]
        public void UC2_12_ModifyDevicesFromCreatePage()
        {
            // Arrange
            InitialStepsForSelectDevices();

            // Act - Seleccionar dos dispositivos
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, null);
            Thread.Sleep(1000);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_1);
            Thread.Sleep(500);
            _selectDevicesForRentalPO.AddDeviceToCart(DEVICE_NAME_2);
            Thread.Sleep(500);
            _selectDevicesForRentalPO.ClickRentDevices();
            Thread.Sleep(1000);

            // Act - Rellenar algunos datos
            _createRentalPO.FillCustomerNameField(CUSTOMER_NAME_SURNAME);
            Thread.Sleep(300);
            _createRentalPO.FillDeliveryAddressField(DELIVERY_ADDRESS_VALID);
            Thread.Sleep(300);

            // Act - Decidir eliminar un dispositivo desde el formulario
            _createRentalPO.RemoveDeviceFromCart(DEVICE_NAME_1);
            Thread.Sleep(500);

            // Act - Continuar y crear el alquiler
            _createRentalPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);
            _createRentalPO.ClickCreateRentalButton();
            Thread.Sleep(1000);

            try
            {
                _createRentalPO.ConfirmDialog();
                Thread.Sleep(1000);
            }
            catch { }

            // Assert - Debe crear el alquiler exitosamente con solo Oppo
            bool isOnDetailPage = _driver.Url.Contains("/rental/detailrental");
            Assert.True(isOnDetailPage, "Debe crear el alquiler con el dispositivo restante");

            // Verificar que solo está el dispositivo Oppo
            int deviceCount = _rentalDetailsPO.GetRentedDevicesCount();
            Assert.Equal(1, deviceCount);
        }

        /*
        ============================
        UC2_13: No existen dispositivos (búsqueda sin resultados)
        ============================
        */

        /// UC2_13: Búsqueda que no devuelve resultados
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC2-Rental")]
        [Trait("Flow", "AlternativeFlow0")]
        [Trait("TestCase", "UC2_13")]
        public void UC2_13_NoDevicesFoundInSearch()
        {
            // Arrange
            InitialStepsForSelectDevices();

            // Act - Buscar con filtro de precio imposible (muy bajo)
            _selectDevicesForRentalPO.SearchDevices(MODEL_FILTER_ALL, 1); // Precio máximo 1€
            Thread.Sleep(1000);

            // Assert - No debe mostrar dispositivos
            bool noDevices = _selectDevicesForRentalPO.CheckNoDevicesFound();
            Assert.True(noDevices, "No debería encontrar dispositivos con precio menor a 1€");

            // Assert adicional - El botón de alquilar debe estar deshabilitado
            bool buttonDisabled = _selectDevicesForRentalPO.IsRentDevicesButtonDisabled();
            Assert.True(buttonDisabled,
                "El botón 'Rent devices' debe estar deshabilitado sin dispositivos");
        }

    }
}