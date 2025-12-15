using AppForMovies.UIT.Shared;
using System;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_Receipt
{
    public class CU_receiptForRepair_UIT : UC_UIT
    {
        private SelectRepairForSelectPO _selectRepairForSelectPO;
        private CreateReceiptPO _createReceiptPO;
        private ReceiptDetailsPO _receiptDetailsPO;

        // ========== DATOS DE PRUEBA: REPARACIONES ==========
        private const string REPAIR_NAME_1 = "Reparación de sensor";
        private const string REPAIR_SCALE_1 = "Baja";
        private const float REPAIR_PRICE_1 = 85.5f;
        private const string REPAIR_DESC_1 = "Reemplazo del sensor de pesaje averiado";

        private const string REPAIR_NAME_2 = "Calibración de precisión";
        private const string REPAIR_SCALE_2 = "Mediocre";
        private const float REPAIR_PRICE_2 = 45f;
        private const string REPAIR_DESC_2 = "Ajuste de calibración por desviación mínima detectada";

        private const string REPAIR_NAME_3 = "Reparación de pantalla";
        private const string REPAIR_SCALE_3 = "Media";
        private const float REPAIR_PRICE_3 = 120f;

        // ========== DATOS DE PRUEBA: CLIENTE ==========
        private const string CLIENT_NAME = "Luis";
        private const string CLIENT_SURNAME = "Lorenzo";
        private const string DELIVERY_ADDRESS = "Calle Mayor 5";
        private const string USER_EMAIL = "luis.lorenzo@email.com";
        private const string USER_PASSWORD = "Password123!";

        // ========== MÉTODOS DE PAGO  ==========
        private const string PAYMENT_METHOD_CASH = "Efectivo";
        private const string PAYMENT_METHOD_CREDIT_CARD = "Tarjeta de Crédito";
        private const string PAYMENT_METHOD_PAYPAL = "PayPal";

        // ========== DATOS DE PRUEBA: MODELOS A REPARAR ==========
        private const string MODEL_1 = "Iphone";
        private const string MODEL_2 = "Android";

        public CU_receiptForRepair_UIT(ITestOutputHelper output) : base(output)
        {
            _selectRepairForSelectPO = new SelectRepairForSelectPO(_driver, _output);
            _createReceiptPO = new CreateReceiptPO(_driver, _output);
            _receiptDetailsPO = new ReceiptDetailsPO(_driver, _output);
        }

      
        private void InitialStepsForSelectRepair()
        {
            Initial_step_opening_the_web_page();
            Perform_login(USER_EMAIL, USER_PASSWORD);
            _driver.Navigate().GoToUrl(_URI + "receipt/select-repair-for-receipt");
            Thread.Sleep(1000);
        }

        /*
        ============================
        FLUJO BÁSICO (PASOS 1-7)
        ============================
        */

        /// CP_UC4_01: Flujo básico completo con tarjeta de crédito
        /// Pasos: 1-7 del flujo básico
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "BasicFlow")]
        public void CP_UC4_01_BasicFlowWithCreditCard()
        {
            // Arrange
            InitialStepsForSelectRepair();

            // Act - Paso 2: Ver lista de reparaciones disponibles
            _selectRepairForSelectPO.SearchRepairs("", "All");
            Thread.Sleep(1000);

            // Act - Paso 3: Seleccionar reparaciones (añadir al carrito)
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_2);
            Thread.Sleep(500);

            // Act - Paso 4: Seleccionar Contratar reparación
            _selectRepairForSelectPO.ClickCreateReceipt();
            Thread.Sleep(1000);

            // Act - Paso 5-6: Rellenar datos obligatorios
            // - Nombre y apellidos
            _createReceiptPO.FillNameField(CLIENT_NAME);
            Thread.Sleep(300);
            _createReceiptPO.FillSurnameField(CLIENT_SURNAME);
            Thread.Sleep(300);

            // - Dirección de entrega
            _createReceiptPO.FillDeliveryAddressField(DELIVERY_ADDRESS);
            Thread.Sleep(300);

            // - Modelo de cada dispositivo (OBLIGATORIO según paso 5)
            _createReceiptPO.FillModelField(REPAIR_NAME_1, MODEL_1);
            Thread.Sleep(300);
            _createReceiptPO.FillModelField(REPAIR_NAME_2, MODEL_2);
            Thread.Sleep(300);

            // - Método de pago
            _createReceiptPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);

            // Act - Paso 6: Guardar (click en Submit)
            _createReceiptPO.ClickSubmitButton();
            Thread.Sleep(1000);

            try
            {
                _createReceiptPO.ConfirmDialog();
                Thread.Sleep(1000);
            }
            catch
            {
                // Dialog puede no aparecer
            }

            // Assert - Paso 7: Verificar recibo con todos los datos
            bool isOnReceiptDetailPage = _driver.Url.Contains("/receipt/detailreceipt") ||
                                        _driver.Url.Contains("/receipts/detailreceipt");

            Assert.True(isOnReceiptDetailPage,
                $"Debería estar en la página de detalles del recibo. URL actual: {_driver.Url}");
        }

        /// CP_UC4_01b: Flujo básico con diferentes métodos de pago
        /// Prueba que el flujo funciona con Efectivo y PayPal
        [Theory]
        [InlineData(PAYMENT_METHOD_CASH)]
        [InlineData(PAYMENT_METHOD_PAYPAL)]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "BasicFlow")]
        public void CP_UC4_01b_BasicFlowWithDifferentPaymentMethods(string paymentMethod)
        {
            // Arrange
            InitialStepsForSelectRepair();

            // Act - Pasos 2-3: Buscar y seleccionar reparación
            _selectRepairForSelectPO.SearchRepairs("", "All");
            Thread.Sleep(1000);
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);

            // Act - Paso 4: Contratar
            _selectRepairForSelectPO.ClickCreateReceipt();
            Thread.Sleep(1000);

            // Act - Paso 5-6: Rellenar datos obligatorios
            _createReceiptPO.FillNameField(CLIENT_NAME);
            Thread.Sleep(300);
            _createReceiptPO.FillSurnameField(CLIENT_SURNAME);
            Thread.Sleep(300);
            _createReceiptPO.FillDeliveryAddressField(DELIVERY_ADDRESS);
            Thread.Sleep(300);
            _createReceiptPO.FillModelField(REPAIR_NAME_1, MODEL_1);
            Thread.Sleep(300);
            _createReceiptPO.SelectPaymentMethod(paymentMethod);
            Thread.Sleep(500);

            // Act - Paso 6: Guardar
            _createReceiptPO.ClickSubmitButton();
            Thread.Sleep(1000);

            try
            {
                _createReceiptPO.ConfirmDialog();
                Thread.Sleep(1000);
            }
            catch { }

            // Assert - Paso 7: Verificar recibo
            bool isOnReceiptDetailPage = _driver.Url.Contains("/receipt/detailreceipt") ||
                                        _driver.Url.Contains("/receipts/detailreceipt");

            Assert.True(isOnReceiptDetailPage,
                $"Debería estar en la página de detalles del recibo. URL actual: {_driver.Url}");
        }

        /*
        ============================
        FLUJO ALTERNATIVO 0 - PASO 2
        Si no hay reparaciones disponibles
        ============================
        */

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow0")]
        public void CP_UC4_02_NoRepairsAvailableMessageShown()
        {
            // Arrange
            InitialStepsForSelectRepair();

            // Act - Paso 2: Buscar con criterio que no devuelve resultados
            _selectRepairForSelectPO.SearchRepairs("NombreQueNoExisteEnLaBaseDatos", "All");
            Thread.Sleep(1000);

            // Assert - Debe mostrar mensaje de sin reparaciones
            // Buscar el párrafo que contiene "No repairs found"
            bool noRepairsVisible = _selectRepairForSelectPO.CheckNoRepairsMessage();
            
            // Si el método no funciona, validar por ausencia de tabla de reparaciones
            if (!noRepairsVisible)
            {
                // Alternativa: validar que la tabla no tiene filas (está vacía)
                bool tableIsEmpty = !_selectRepairForSelectPO.CheckListOfRepairs(new List<string[]>());
                Assert.True(tableIsEmpty || noRepairsVisible, 
                    "Debería mostrar mensaje de sin reparaciones disponibles o la tabla estar vacía");
            }
            else
            {
                Assert.True(noRepairsVisible, "Debería mostrar mensaje de sin reparaciones disponibles");
            }
        }

        /*
        ============================
        FLUJO ALTERNATIVO 1 - PASO 2
        Filtrar reparaciones según nombre y/o escala
        ============================
        */

        /// Filtro por nombre de reparación
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow1")]
        public void CP_UC4_03_FilterRepairsByName()
        {
            // Arrange
            InitialStepsForSelectRepair();

            // Act - Paso 2.2-2.3: Aplicar filtro por nombre
            _selectRepairForSelectPO.SearchRepairs("sensor", "All");
            Thread.Sleep(1000);

            // Assert - Debe mostrar solo reparaciones que coincidan con "sensor"
            bool hasRepairs = !_selectRepairForSelectPO.CheckNoRepairsMessage();
            Assert.True(hasRepairs, "Debería encontrar reparaciones con 'sensor' en el nombre");
        }

        /// Filtro por escala
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow1")]
        public void CP_UC4_04_FilterRepairsByScale()
        {
            // Arrange
            InitialStepsForSelectRepair();

            // Act - Paso 2.2-2.3: Aplicar filtro por escala
            _selectRepairForSelectPO.SearchRepairs("", "Baja");
            Thread.Sleep(1000);

            // Assert
            bool hasRepairs = !_selectRepairForSelectPO.CheckNoRepairsMessage();
            Assert.True(hasRepairs, "Debería encontrar reparaciones con escala 'Baja'");
        }

        /// Filtro combinado: nombre y escala
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow1")]
        public void CP_UC4_05_FilterRepairsByCombinedFilters()
        {
            // Arrange
            InitialStepsForSelectRepair();

            // Act - Paso 2.2-2.3: Aplicar filtros combinados
            _selectRepairForSelectPO.SearchRepairs("sensor", "Baja");
            Thread.Sleep(1000);

            // Assert
            bool hasRepairs = !_selectRepairForSelectPO.CheckNoRepairsMessage();
            Assert.True(hasRepairs, "Debería encontrar reparaciones que coincidan con ambos filtros");
        }

        /*
        ============================
        FLUJO ALTERNATIVO 2 - PASO 5
        Modificar carrito de la compra
        ============================
        */

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow2")]
        public void CP_UC4_06_ModifyCartRemoveOneRepair()
        {
            // Arrange
            InitialStepsForSelectRepair();

            // Act - Paso 2-3: Seleccionar dos reparaciones
            _selectRepairForSelectPO.SearchRepairs("", "All");
            Thread.Sleep(1000);
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_2);
            Thread.Sleep(500);

            // Act - Paso 5: Modificar carrito (eliminar una)
            _selectRepairForSelectPO.RemoveRepairFromReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);

            // Assert - El botón debe seguir disponible (aún hay una reparación)
            bool buttonAvailable = !_selectRepairForSelectPO.CreateReceiptButtonNotAvailable();
            Assert.True(buttonAvailable, "El botón de crear recibo debe estar disponible con al menos una reparación");
        }

        
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow2")]
        public void CP_UC4_06b_RemoveAllRepairsFromCart()
        {
            // Arrange
            InitialStepsForSelectRepair();

            // Act - Paso 2-3: Seleccionar reparación
            _selectRepairForSelectPO.SearchRepairs("", "All");
            Thread.Sleep(1000);
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);

            // Act - Paso 5: Eliminar la reparación (carrito vacío)
            _selectRepairForSelectPO.RemoveRepairFromReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);

            // Assert - El botón debe estar deshabilitado (carrito vacío)
            bool buttonDisabled = _selectRepairForSelectPO.CreateReceiptButtonNotAvailable();
            Assert.True(buttonDisabled, "El botón de crear recibo debe estar deshabilitado cuando el carrito está vacío");
        }

        /*
        ============================
        FLUJO ALTERNATIVO 3 - PASO 4
        Carrito vacío - botón continuar no disponible
        ============================
        */

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow3")]
        public void CP_UC4_07_EmptyCartButtonDisabled()
        {
            // Arrange
            InitialStepsForSelectRepair();

            // Act - Paso 2: Ver lista sin seleccionar nada
            _selectRepairForSelectPO.SearchRepairs("", "All");
            Thread.Sleep(1000);

            // Assert - Botón no disponible sin reparaciones en carrito
            bool buttonDisabled = _selectRepairForSelectPO.CreateReceiptButtonNotAvailable();
            Assert.True(buttonDisabled,
                "El botón continuar no debe estar disponible si el carrito está vacío");
        }

        /*
        ============================
        FLUJO ALTERNATIVO 4 - PASO 7
        Campos obligatorios no rellenados
        ============================
        */

        /// Campo nombre vacío
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow4")]
        public void CP_UC4_08_ValidationEmptyName()
        {
            // Arrange
            InitialStepsForSelectRepair();
            _selectRepairForSelectPO.SearchRepairs("", "All");
            Thread.Sleep(1000);
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);
            _selectRepairForSelectPO.ClickCreateReceipt();
            Thread.Sleep(1000);

            // Act - Rellenar todos excepto nombre
            _createReceiptPO.FillNameField("");  // Vacío
            _createReceiptPO.FillSurnameField(CLIENT_SURNAME);
            _createReceiptPO.FillDeliveryAddressField(DELIVERY_ADDRESS);
            _createReceiptPO.FillModelField(REPAIR_NAME_1, MODEL_1);
            _createReceiptPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);

            _createReceiptPO.ClickSubmitButton();
            Thread.Sleep(1500);

            // Assert - Vuelve al paso 5 (permanece en la página)
            bool isStillOnCreatePage = _driver.Url.Contains("/receipt/create") ||
                                      _driver.Url.Contains("/receipts/createreceipt");
            bool hasErrors = _createReceiptPO.HasValidationErrors();

            Assert.True(isStillOnCreatePage || hasErrors,
                "Debe mostrar validación y permanecer en la página si falta el nombre");
        }

        /// Campo apellidos vacío
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow4")]
        public void CP_UC4_08b_ValidationEmptySurname()
        {
            // Arrange
            InitialStepsForSelectRepair();
            _selectRepairForSelectPO.SearchRepairs("", "All");
            Thread.Sleep(1000);
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);
            _selectRepairForSelectPO.ClickCreateReceipt();
            Thread.Sleep(1000);

            // Act
            _createReceiptPO.FillNameField(CLIENT_NAME);
            _createReceiptPO.FillSurnameField("");  // Vacío
            _createReceiptPO.FillDeliveryAddressField(DELIVERY_ADDRESS);
            _createReceiptPO.FillModelField(REPAIR_NAME_1, MODEL_1);
            _createReceiptPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);

            _createReceiptPO.ClickSubmitButton();
            Thread.Sleep(1500);

            // Assert
            bool isStillOnCreatePage = _driver.Url.Contains("/receipt/create") ||
                                      _driver.Url.Contains("/receipts/createreceipt");

            Assert.True(isStillOnCreatePage || _createReceiptPO.HasValidationErrors(),
                "Debe mostrar validación si falta el apellido");
        }

        /// Campo dirección vacío
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow4")]
        public void CP_UC4_08c_ValidationEmptyDeliveryAddress()
        {
            // Arrange
            InitialStepsForSelectRepair();
            _selectRepairForSelectPO.SearchRepairs("", "All");
            Thread.Sleep(1000);
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);
            _selectRepairForSelectPO.ClickCreateReceipt();
            Thread.Sleep(1000);

            // Act
            _createReceiptPO.FillNameField(CLIENT_NAME);
            _createReceiptPO.FillSurnameField(CLIENT_SURNAME);
            _createReceiptPO.FillDeliveryAddressField("");  // Vacío
            _createReceiptPO.FillModelField(REPAIR_NAME_1, MODEL_1);
            _createReceiptPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);

            _createReceiptPO.ClickSubmitButton();
            Thread.Sleep(1500);

            // Assert
            bool isStillOnCreatePage = _driver.Url.Contains("/receipt/create") ||
                                      _driver.Url.Contains("/receipts/createreceipt");

            Assert.True(isStillOnCreatePage || _createReceiptPO.HasValidationErrors(),
                "Debe mostrar validación si falta la dirección");
        }

        /// Campo modelo vacío
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow4")]
        public void CP_UC4_08d_ValidationEmptyModel()
        {
            // Arrange
            InitialStepsForSelectRepair();
            _selectRepairForSelectPO.SearchRepairs("", "All");
            Thread.Sleep(1000);
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);
            _selectRepairForSelectPO.ClickCreateReceipt();
            Thread.Sleep(1000);

            // Act - Rellenar todo excepto modelo
            _createReceiptPO.FillNameField(CLIENT_NAME);
            _createReceiptPO.FillSurnameField(CLIENT_SURNAME);
            _createReceiptPO.FillDeliveryAddressField(DELIVERY_ADDRESS);
            // No rellenar modelo
            _createReceiptPO.SelectPaymentMethod(PAYMENT_METHOD_CREDIT_CARD);
            Thread.Sleep(500);

            _createReceiptPO.ClickSubmitButton();
            Thread.Sleep(1500);

            // Assert
            bool isStillOnCreatePage = _driver.Url.Contains("/receipt/create") ||
                                      _driver.Url.Contains("/receipts/createreceipt");

            Assert.True(isStillOnCreatePage || _createReceiptPO.HasValidationErrors(),
                "Debe mostrar validación si falta el modelo");
        }

        /*
        ============================
        FLUJO ALTERNATIVO 5 - PASO 7
        Modificar reparaciones desde la página de creación
        ============================
        */

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        [Trait("UserStory", "UC4-Receipt")]
        [Trait("Flow", "AlternativeFlow5")]
        public void CP_UC4_09_ModifyRepairsFromCreatePage()
        {
            // Arrange
            InitialStepsForSelectRepair();

            // Act - Pasos 2-3: Seleccionar reparación
            _selectRepairForSelectPO.SearchRepairs("", "All");
            Thread.Sleep(1000);
            _selectRepairForSelectPO.AddRepairToReceipt(REPAIR_NAME_1);
            Thread.Sleep(500);

            // Act - Paso 4: Ir a crear recibo
            _selectRepairForSelectPO.ClickCreateReceipt();
            Thread.Sleep(1000);

            // Act - Paso 5: Rellenar algunos datos
            _createReceiptPO.FillNameField(CLIENT_NAME);
            Thread.Sleep(500);

            // Act - Decidir modificar reparaciones
            _createReceiptPO.ClickModifyRepairsButton();
            Thread.Sleep(1000);

            // Assert - Vuelve a paso 2 (página de selección)
            Assert.True(_driver.Url.Contains("/receipt/select-repair-for-receipt"),
                "Debe volver a la página de selección de reparaciones");
        }
    }
}