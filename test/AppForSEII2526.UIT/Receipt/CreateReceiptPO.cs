using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_Receipt
{
    public class CreateReceiptPO : PageObject
    {
        private By inputName = By.Id("Name");
        private By inputSurname = By.Id("Surname");
        private By inputDeliveryAddress = By.Id("DeliveryAddress");
        private By selectPaymentMethod = By.Id("PaymentMethod");
        private By buttonSubmit = By.Id("Submit");
        private By dialogOkButton = By.Id("Button_DialogOK");
        private By modifyRepairsButton = By.Id("ModifyRepairs");
        private By tableOfReceiptItemsBy = By.Id("TableOfReceiptItems");
        private By totalPriceElement = By.XPath("//*[contains(text(), 'Total price:')]");
        private By errorMessageElement = By.Id("ErrorsShown");
        private By validationSummary = By.XPath("//div[contains(@class, 'alert alert-danger')]");

        public CreateReceiptPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        /// <summary>
        /// Rellena el formulario de creación del recibo con los datos del cliente
        /// </summary>
        public void FillReceiptForm(string nombre, string apellidos, string direccion, string metodoPago)
        {
            // Rellenar nombre
            FillNameField(nombre);
            Thread.Sleep(300);

            // Rellenar apellidos
            FillSurnameField(apellidos);
            Thread.Sleep(300);

            // Rellenar dirección
            FillDeliveryAddressField(direccion);
            Thread.Sleep(300);

            // Seleccionar método de pago
            SelectPaymentMethodByText(metodoPago);
            Thread.Sleep(300);
        }

        /// <summary>
        /// Rellena solo el campo de nombre con disparo de evento blur
        /// </summary>
        public void FillNameField(string nombre)
        {
            WaitForBeingClickable(inputName);
            IWebElement nameField = _driver.FindElement(inputName);
            
            // Limpiar completamente el campo
            nameField.Clear();
            Thread.Sleep(100);
            
            // Usar triple-click para seleccionar todo (alternativa a Clear)
            nameField.SendKeys(Keys.Control + "a");
            Thread.Sleep(50);
            
            // Escribir el nuevo valor
            nameField.SendKeys(nombre);
            Thread.Sleep(100);
            
            // Disparar evento blur para que Blazor procese la validación
            DispatchBlurEvent(nameField);
        }

        /// <summary>
        /// Rellena solo el campo de apellidos con disparo de evento blur
        /// </summary>
        public void FillSurnameField(string apellidos)
        {
            WaitForBeingClickable(inputSurname);
            IWebElement surnameField = _driver.FindElement(inputSurname);
            
            // Limpiar completamente el campo
            surnameField.Clear();
            Thread.Sleep(100);
            surnameField.SendKeys(Keys.Control + "a");
            Thread.Sleep(50);
            
            // Escribir el nuevo valor
            surnameField.SendKeys(apellidos);
            Thread.Sleep(100);
            
            // Disparar evento blur
            DispatchBlurEvent(surnameField);
        }

        /// <summary>
        /// Rellena solo el campo de dirección con disparo de evento blur
        /// </summary>
        public void FillDeliveryAddressField(string direccion)
        {
            WaitForBeingClickable(inputDeliveryAddress);
            IWebElement addressField = _driver.FindElement(inputDeliveryAddress);
            
            // Limpiar completamente el campo
            addressField.Clear();
            Thread.Sleep(100);
            addressField.SendKeys(Keys.Control + "a");
            Thread.Sleep(50);
            
            // Escribir el nuevo valor
            addressField.SendKeys(direccion);
            Thread.Sleep(100);
            
            // Disparar evento blur
            DispatchBlurEvent(addressField);
        }

        /// <summary>
        /// Rellena el campo de modelo para una reparación específica en la tabla
        /// </summary>
        public void FillModelField(string repairName, string model)
        {
            try
            {
                // Buscar la fila de la reparación por su id
                By rowBy = By.Id($"RepairData_{repairName}");
                WaitForBeingVisible(rowBy);
                IWebElement row = _driver.FindElement(rowBy);

                // Dentro de la fila, buscar el input de modelo (debe ser el tercer td con un input)
                var inputs = row.FindElements(By.TagName("input"));
                if (inputs.Count > 0)
                {
                    IWebElement modelInput = inputs[0]; // El primer input debe ser el de modelo
                    modelInput.Clear();
                    Thread.Sleep(100);
                    modelInput.SendKeys(model);
                    Thread.Sleep(100);
                    DispatchBlurEvent(modelInput);
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                throw new NoSuchElementException($"No se pudo rellenar el campo de modelo para la reparación '{repairName}'", ex);
            }
        }

        /// <summary>
        /// Obtiene el valor del campo de modelo para una reparación específica
        /// </summary>
        public string GetModelFieldValue(string repairName)
        {
            try
            {
                By rowBy = By.Id($"RepairData_{repairName}");
                WaitForBeingVisible(rowBy);
                IWebElement row = _driver.FindElement(rowBy);

                var inputs = row.FindElements(By.TagName("input"));
                if (inputs.Count > 0)
                {
                    return inputs[0].GetAttribute("value");
                }
                return "";
            }
            catch
            {
                return "";
            }
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
        /// Selecciona el método de pago
        /// </summary>
        public void SelectPaymentMethod(string metodoPago)
        {
            SelectPaymentMethodByText(metodoPago);
        }

        /// <summary>
        /// Selecciona el método de pago por texto visible
        /// </summary>
        private void SelectPaymentMethodByText(string metodoPago)
        {
            WaitForBeingClickable(selectPaymentMethod);
            IWebElement paymentDropdown = _driver.FindElement(selectPaymentMethod);
            SelectElement selectElement = new SelectElement(paymentDropdown);
            
            try
            {
                // Intentar por texto visible primero
                selectElement.SelectByText(metodoPago);
            }
            catch
            {
                try
                {
                    // Si falla, intentar por valor numérico
                    selectElement.SelectByValue(metodoPago);
                }
                catch
                {
                    // Si ambos fallan, intentar encontrar una opción parcial
                    var options = selectElement.Options;
                    var matchingOption = options.FirstOrDefault(o => o.Text.Contains(metodoPago));
                    if (matchingOption != null)
                    {
                        selectElement.SelectByText(matchingOption.Text);
                    }
                    else
                    {
                        throw new NoSuchElementException($"No se encontró opción de pago para: {metodoPago}");
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
        /// Obtiene el valor del campo nombre
        /// </summary>
        public string GetNameFieldValue()
        {
            WaitForBeingVisible(inputName);
            return _driver.FindElement(inputName).GetAttribute("value");
        }

        /// <summary>
        /// Obtiene el valor del campo apellidos
        /// </summary>
        public string GetSurnameFieldValue()
        {
            WaitForBeingVisible(inputSurname);
            return _driver.FindElement(inputSurname).GetAttribute("value");
        }

        /// <summary>
        /// Obtiene el valor del campo dirección
        /// </summary>
        public string GetDeliveryAddressFieldValue()
        {
            WaitForBeingVisible(inputDeliveryAddress);
            return _driver.FindElement(inputDeliveryAddress).GetAttribute("value");
        }

        /// <summary>
        /// Verifica la lista de reparaciones en el recibo
        /// </summary>
        public bool CheckListOfRepairs(List<string[]> expectedRepairs)
        {
            return CheckBodyTable(expectedRepairs, tableOfReceiptItemsBy);
        }

        /// <summary>
        /// Obtiene el precio total del recibo
        /// </summary>
        public string GetTotalPrice()
        {
            try
            {
                // Buscar el texto que contiene "Precio total:" o "Total price:"
                var priceElements = _driver.FindElements(By.XPath("//*[contains(text(), 'Precio total:') or contains(text(), 'Total price:')]"));
                
                if (priceElements.Count > 0)
                {
                    string text = priceElements[0].Text;
                    // Extrae el número del texto
                    var match = System.Text.RegularExpressions.Regex.Match(text, @"[\d.,]+");
                    if (match.Success)
                    {
                        return match.Value;
                    }
                }
                
                // Si no se encuentra por XPath, intentar buscar el elemento con ID TotalPrice
                var totalPriceByIdElements = _driver.FindElements(By.Id("TotalPrice"));
                if (totalPriceByIdElements.Count > 0)
                {
                    return totalPriceByIdElements[0].Text;
                }
                
                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Hace click en el botón de envío del formulario
        /// </summary>
        public void ClickSubmitButton()
        {
            WaitForBeingClickable(buttonSubmit);
            _driver.FindElement(buttonSubmit).Click();
            Thread.Sleep(500); // Esperar a que Blazor procese
        }

        /// <summary>
        /// Confirma el diálogo de confirmación
        /// </summary>
        public void ConfirmDialog()
        {
            WaitForBeingClickable(dialogOkButton);
            _driver.FindElement(dialogOkButton).Click();
        }

        /// <summary>
        /// Hace click en el botón para modificar reparaciones
        /// </summary>
        public void ClickModifyRepairsButton()
        {
            WaitForBeingClickable(modifyRepairsButton);
            _driver.FindElement(modifyRepairsButton).Click();
        }

        /// <summary>
        /// Verifica si se muestra un mensaje de error
        /// </summary>
        public bool CheckErrorMessage(string expectedError)
        {
            try
            {
                WaitForBeingVisible(errorMessageElement);
                string errorText = _driver.FindElement(errorMessageElement).Text;
                return errorText.Contains(expectedError);
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
                // Buscar cualquier elemento con clase alert alert-danger
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

                // También buscar mensajes de error inline (data-validation-summary)
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
        public string GetValidationErrorText()
        {
            try
            {
                var validationElements = _driver.FindElements(validationSummary);
                if (validationElements.Count > 0)
                {
                    return string.Join(" | ", validationElements
                        .Where(el => 
                        {
                            try { return el.Displayed; }
                            catch { return false; }
                        })
                        .Select(el => el.Text));
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Verifica si el botón submit está deshabilitado
        /// </summary>
        public bool IsSubmitButtonDisabled()
        {
            try
            {
                WaitForBeingVisible(buttonSubmit);
                var button = _driver.FindElement(buttonSubmit);
                var disabledAttr = button.GetAttribute("disabled");
                return disabledAttr != null || !button.Enabled;
            }
            catch
            {
                return false;
            }
        }
    }
}