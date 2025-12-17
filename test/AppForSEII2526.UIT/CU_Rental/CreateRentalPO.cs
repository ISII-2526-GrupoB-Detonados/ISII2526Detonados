using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_Rental
{
    public class CreateRentalPO : PageObject
    {
        private By inputCustomerName = By.Id("customerName");
        private By inputDeliveryAddress = By.Id("deliveryAddress");
        private By selectPaymentMethod = By.Id("paymentMethod");
        private By inputRentalFrom = By.Id("rentalFrom");
        private By inputRentalTo = By.Id("rentalTo");
        private By buttonCreateRental = By.Id("createRentalButton");
        private By dialogOkButton = By.XPath("//button[contains(@class, 'btn-primary') and contains(., 'Yes')]");

        private By modifyDevicesButton = By.XPath("//button[contains(text(), 'Modify Devices')]");
        private By tableOfRentalItems = By.Id("RentalItemsTable");
        private By totalPriceElement = By.Id("TotalPrice");
        private By errorMessageElement = By.Id("ErrorsShown");
        private By validationSummary = By.XPath("//div[contains(@class, 'alert alert-danger')]");

        public CreateRentalPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        /// <summary>
        /// Rellena el formulario de creación del alquiler con los datos del cliente
        /// </summary>
        public void FillRentalForm(string customerName, string deliveryAddress, string paymentMethod)
        {
            // Rellenar nombre del cliente
            FillCustomerNameField(customerName);
            Thread.Sleep(300);

            // Rellenar dirección de entrega
            FillDeliveryAddressField(deliveryAddress);
            Thread.Sleep(300);

            // Seleccionar método de pago
            SelectPaymentMethod(paymentMethod);
            Thread.Sleep(300);
        }


        /// <summary>
        /// Verifica si se muestra el mensaje de carrito vacío
        /// </summary>
        public bool HasEmptyCartMessage()
        {
            try
            {
                By emptyCartMessage = By.XPath("//div[contains(@class, 'alert-warning') and contains(text(), 'No devices selected')]");
                WaitForBeingVisible(emptyCartMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Rellena solo el campo de nombre del cliente con disparo de evento blur
        /// </summary>
        public void FillCustomerNameField(string customerName)
        {
            WaitForBeingClickable(inputCustomerName);
            IWebElement nameField = _driver.FindElement(inputCustomerName);

            // Limpiar completamente el campo
            nameField.Clear();
            Thread.Sleep(100);

            // Usar Ctrl+A para seleccionar todo (alternativa a Clear)
            nameField.SendKeys(Keys.Control + "a");
            Thread.Sleep(50);

            // Escribir el nuevo valor
            nameField.SendKeys(customerName);
            Thread.Sleep(100);

            // Disparar evento blur para que Blazor procese la validación
            DispatchBlurEvent(nameField);
        }

        /// <summary>
        /// Rellena solo el campo de dirección de entrega con disparo de evento blur
        /// </summary>
        public void FillDeliveryAddressField(string deliveryAddress)
        {
            WaitForBeingClickable(inputDeliveryAddress);
            IWebElement addressField = _driver.FindElement(inputDeliveryAddress);

            // Limpiar completamente el campo
            addressField.Clear();
            Thread.Sleep(100);
            addressField.SendKeys(Keys.Control + "a");
            Thread.Sleep(50);

            // Escribir el nuevo valor
            addressField.SendKeys(deliveryAddress);
            Thread.Sleep(100);

            // Disparar evento blur
            DispatchBlurEvent(addressField);
        }

        /// <summary>
        /// Dispara el evento blur en un elemento para que Blazor valide
        /// </summary>
        private void DispatchBlurEvent(IWebElement element)
        {
            try
            {
                // Usar JavaScript para disparar el evento blur
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].blur();", element);
                Thread.Sleep(200);
            }
            catch
            {
                // Si falla el JavaScript, intentar con Tab
                element.SendKeys(Keys.Tab);
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Selecciona el método de pago por texto visible
        /// </summary>
        public void SelectPaymentMethod(string paymentMethod)
        {
            WaitForBeingClickable(selectPaymentMethod);
            IWebElement paymentDropdown = _driver.FindElement(selectPaymentMethod);
            SelectElement selectElement = new SelectElement(paymentDropdown);

            try
            {
                // Intentar por texto visible primero
                selectElement.SelectByText(paymentMethod);
            }
            catch
            {
                try
                {
                    // Si falla, intentar por valor
                    selectElement.SelectByValue(paymentMethod);
                }
                catch
                {
                    // Si ambos fallan, intentar encontrar una opción parcial
                    var options = selectElement.Options;
                    var matchingOption = options.FirstOrDefault(o => o.Text.Contains(paymentMethod));
                    if (matchingOption != null)
                    {
                        selectElement.SelectByText(matchingOption.Text);
                    }
                    else
                    {
                        throw new NoSuchElementException($"No se encontró opción de pago para: {paymentMethod}");
                    }
                }
            }

            // Disparar evento change en el dropdown
            DispatchChangeEvent(paymentDropdown);
        }

        /// <summary>
        /// Dispara el evento change en un elemento
        /// </summary>
        private void DispatchChangeEvent(IWebElement element)
        {
            try
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].dispatchEvent(new Event('change', { bubbles: true }));", element);
                Thread.Sleep(200);
            }
            catch { }
        }

        /// <summary>
        /// Obtiene el valor del campo nombre del cliente
        /// </summary>
        public string GetCustomerNameFieldValue()
        {
            WaitForBeingVisible(inputCustomerName);
            return _driver.FindElement(inputCustomerName).GetAttribute("value");
        }

        /// <summary>
        /// Obtiene el valor del campo dirección de entrega
        /// </summary>
        public string GetDeliveryAddressFieldValue()
        {
            WaitForBeingVisible(inputDeliveryAddress);
            return _driver.FindElement(inputDeliveryAddress).GetAttribute("value");
        }

        /// <summary>
        /// Obtiene el método de pago seleccionado
        /// </summary>
        public string GetSelectedPaymentMethod()
        {
            WaitForBeingVisible(selectPaymentMethod);
            IWebElement paymentDropdown = _driver.FindElement(selectPaymentMethod);
            SelectElement selectElement = new SelectElement(paymentDropdown);
            return selectElement.SelectedOption.Text;
        }

        /// <summary>
        /// Obtiene la fecha de inicio del alquiler
        /// </summary>
        public string GetRentalFromDate()
        {
            try
            {
                WaitForBeingVisible(inputRentalFrom);
                return _driver.FindElement(inputRentalFrom).GetAttribute("value");
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Obtiene la fecha de fin del alquiler
        /// </summary>
        public string GetRentalToDate()
        {
            try
            {
                WaitForBeingVisible(inputRentalTo);
                return _driver.FindElement(inputRentalTo).GetAttribute("value");
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Verifica si hay dispositivos en el carrito (tabla de items)
        /// </summary>
        public bool HasDevicesInCart()
        {
            try
            {
                WaitForBeingVisible(tableOfRentalItems);
                var rows = _driver.FindElements(By.XPath("//table[@id='RentalItemsTable']//tbody/tr"));
                return rows.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica la lista de dispositivos en el carrito
        /// </summary>
        public bool CheckListOfDevicesInCart(List<string[]> expectedDevices)
        {
            return CheckBodyTable(expectedDevices, tableOfRentalItems);
        }

        /// <summary>
        /// Elimina un dispositivo del carrito desde la página de creación
        /// </summary>
        public void RemoveDeviceFromCart(string deviceName)
        {
            try
            {
                // Buscar el botón × (times) en la fila del dispositivo
                By removeButton = By.XPath($"//tr[@id='RentalItem_{deviceName}']//button[contains(@class, 'btn-danger')]");
                WaitForBeingClickable(removeButton);
                _driver.FindElement(removeButton).Click();
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                throw new NoSuchElementException($"No se pudo eliminar el dispositivo '{deviceName}' del carrito", ex);
            }
        }

        /// <summary>
        /// Obtiene el número de dispositivos en el carrito
        /// </summary>
        public int GetDeviceCountInCart()
        {
            try
            {
                var rows = _driver.FindElements(By.XPath("//table[@id='RentalItemsTable']//tbody/tr"));
                return rows.Count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Obtiene el precio total del alquiler
        /// </summary>
        public string GetTotalPrice()
        {
            try
            {
                WaitForBeingVisible(totalPriceElement);
                string priceText = _driver.FindElement(totalPriceElement).Text;

                // Extrae el número del texto
                var match = System.Text.RegularExpressions.Regex.Match(priceText, @"[\d.,]+");
                if (match.Success)
                {
                    return match.Value;
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Hace click en el botón de crear alquiler
        /// </summary>
        public void ClickCreateRentalButton()
        {
            WaitForBeingClickable(buttonCreateRental);
            _driver.FindElement(buttonCreateRental).Click();
            Thread.Sleep(500); // Esperar a que Blazor procese
        }

        /// <summary>
        /// Confirma el diálogo de confirmación
        /// </summary>

        /// <summary>
        /// Confirma el diálogo de confirmación
        /// </summary>
        public void ConfirmDialog()
        {
            try
            {
                // Esperar a que el modal esté visible
                By modalDialog = By.ClassName("modal-dialog");
                WaitForBeingVisible(modalDialog);
                Thread.Sleep(500);

                // Hacer clic en el botón Yes
                WaitForBeingClickable(dialogOkButton);
                _driver.FindElement(dialogOkButton).Click();
                Thread.Sleep(1000); // Aumentar a 1 segundo para dar tiempo a Blazor

                // Esperar a que el modal desaparezca (evita race conditions)
                try
                {
                    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                    wait.Until(driver => driver.FindElements(modalDialog).Count == 0 ||
                                        !driver.FindElements(modalDialog).Any(el => el.Displayed));
                }
                catch
                {
                    // Si el modal ya desapareció, continuar
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error al confirmar diálogo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Hace click en el botón para modificar dispositivos
        /// </summary>
        public void ClickModifyDevicesButton()
        {
            WaitForBeingClickable(modifyDevicesButton);
            _driver.FindElement(modifyDevicesButton).Click();
        }

        /// <summary>
        /// Verifica si se muestra un mensaje de error específico
        /// </summary>
        public bool CheckErrorMessage(string expectedError)
        {
            try
            {
                WaitForBeingVisible(errorMessageElement);
                string errorText = _driver.FindElement(errorMessageElement).Text;
                return errorText.Contains(expectedError, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si hay errores de validación visibles en el formulario
        /// </summary>
        public bool HasValidationErrors()
        {
            try
            {
                // 1. Buscar mensajes de validación de Blazor: <ul class="validation-errors">
                var blazorValidationErrors = _driver.FindElements(By.XPath("//ul[contains(@class, 'validation-errors')]//li[contains(@class, 'validation-message')]"));
                if (blazorValidationErrors.Count > 0)
                {
                    bool hasVisibleErrors = blazorValidationErrors.Any(el =>
                    {
                        try
                        {
                            return el.Displayed && !string.IsNullOrWhiteSpace(el.Text);
                        }
                        catch { return false; }
                    });

                    if (hasVisibleErrors)
                    {
                        return true;
                    }
                }

                // 2. Buscar alert alert-danger (mensajes del servidor)
                var validationElements = _driver.FindElements(validationSummary);
                if (validationElements.Count > 0)
                {
                    return validationElements.Any(el =>
                    {
                        try
                        {
                            return el.Displayed && !string.IsNullOrWhiteSpace(el.Text);
                        }
                        catch { return false; }
                    });
                }

                // 3. Buscar mensajes de error inline (field-validation-error)
                var inlineErrors = _driver.FindElements(By.XPath("//div[contains(@class, 'invalid-feedback') or contains(@class, 'field-validation-error')]"));
                return inlineErrors.Count > 0 && inlineErrors.Any(el =>
                {
                    try
                    {
                        return el.Displayed && !string.IsNullOrWhiteSpace(el.Text);
                    }
                    catch { return false; }
                });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el texto de los errores de validación
        /// </summary>
        /// <summary>
        /// Obtiene el texto de los errores de validación
        /// </summary>
        public string GetValidationErrorText()
        {
            try
            {
                var errorMessages = new List<string>();

                // 1. Errores de Blazor validation
                var blazorErrors = _driver.FindElements(By.XPath("//ul[contains(@class, 'validation-errors')]//li[contains(@class, 'validation-message')]"));
                if (blazorErrors.Count > 0)
                {
                    errorMessages.AddRange(blazorErrors
                        .Where(el =>
                        {
                            try { return el.Displayed; }
                            catch { return false; }
                        })
                        .Select(el => el.Text));
                }

                // 2. Errores del servidor (alert alert-danger)
                var validationElements = _driver.FindElements(validationSummary);
                if (validationElements.Count > 0)
                {
                    errorMessages.AddRange(validationElements
                        .Where(el =>
                        {
                            try { return el.Displayed; }
                            catch { return false; }
                        })
                        .Select(el => el.Text));
                }

                // 3. Errores inline
                var inlineErrors = _driver.FindElements(By.XPath("//div[contains(@class, 'invalid-feedback') or contains(@class, 'field-validation-error')]"));
                if (inlineErrors.Count > 0)
                {
                    errorMessages.AddRange(inlineErrors
                        .Where(el =>
                        {
                            try { return el.Displayed; }
                            catch { return false; }
                        })
                        .Select(el => el.Text));
                }

                return string.Join(" | ", errorMessages);
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// Verifica si el botón de crear alquiler está deshabilitado
        /// </summary>
        public bool IsCreateRentalButtonDisabled()
        {
            try
            {
                WaitForBeingVisible(buttonCreateRental);
                var button = _driver.FindElement(buttonCreateRental);
                var disabledAttr = button.GetAttribute("disabled");
                return disabledAttr != null || !button.Enabled;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si un dispositivo específico está en el carrito
        /// </summary>
        public bool IsDeviceInCart(string deviceName)
        {
            try
            {
                By deviceRow = By.Id($"RentalItem_{deviceName}");
                WaitForBeingVisible(deviceRow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un dispositivo específico del carrito
        /// </summary>
        public (string name, string model, int quantity, float price) GetDeviceDetails(string deviceName)
        {
            try
            {
                By deviceRow = By.Id($"RentalItem_{deviceName}");
                WaitForBeingVisible(deviceRow);

                IWebElement row = _driver.FindElement(deviceRow);
                var cells = row.FindElements(By.TagName("td"));

                if (cells.Count >= 4)
                {
                    string name = cells[0].Text;
                    string model = cells[1].Text;
                    int quantity = int.Parse(cells[2].Text);

                    // Extraer precio (eliminar símbolo €)
                    string priceText = cells[3].Text.Replace("€", "").Trim();
                    float price = float.Parse(priceText);

                    return (name, model, quantity, price);
                }

                return ("", "", 0, 0f);
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error obteniendo detalles del dispositivo '{deviceName}': {ex.Message}");
                return ("", "", 0, 0f);
            }
        }
    }
}
