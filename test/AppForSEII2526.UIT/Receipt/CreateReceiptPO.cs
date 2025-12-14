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

        public CreateReceiptPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        /// <summary>
        /// Rellena el formulario de creación del recibo con los datos del cliente
        /// </summary>
        public void FillReceiptForm(string nombre, string apellidos, string direccion, string metodoPago)
        {
            // Esperar a que los inputs estén listos
            WaitForBeingClickable(inputName);
            _driver.FindElement(inputName).Clear();
            _driver.FindElement(inputName).SendKeys(nombre);

            WaitForBeingClickable(inputSurname);
            _driver.FindElement(inputSurname).Clear();
            _driver.FindElement(inputSurname).SendKeys(apellidos);

            WaitForBeingClickable(inputDeliveryAddress);
            _driver.FindElement(inputDeliveryAddress).Clear();
            _driver.FindElement(inputDeliveryAddress).SendKeys(direccion);

            // Seleccionar método de pago
            SelectPaymentMethodByText(metodoPago);
        }

        /// <summary>
        /// Rellena solo el campo de nombre
        /// </summary>
        public void FillNameField(string nombre)
        {
            WaitForBeingClickable(inputName);
            _driver.FindElement(inputName).Clear();
            _driver.FindElement(inputName).SendKeys(nombre);
        }

        /// <summary>
        /// Rellena solo el campo de apellidos
        /// </summary>
        public void FillSurnameField(string apellidos)
        {
            WaitForBeingClickable(inputSurname);
            _driver.FindElement(inputSurname).Clear();
            _driver.FindElement(inputSurname).SendKeys(apellidos);
        }

        /// <summary>
        /// Rellena solo el campo de dirección
        /// </summary>
        public void FillDeliveryAddressField(string direccion)
        {
            WaitForBeingClickable(inputDeliveryAddress);
            _driver.FindElement(inputDeliveryAddress).Clear();
            _driver.FindElement(inputDeliveryAddress).SendKeys(direccion);
        }

        /// <summary>
        /// Selecciona el método de pago por valor numérico
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
                // Intentar por texto visible primero (para "Tarjeta de Crédito", "PayPal", "Efectivo")
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
                WaitForBeingVisible(totalPriceElement);
                string text = _driver.FindElement(totalPriceElement).Text;
                // Extrae el número del texto "Total price: 150.75 €"
                var match = System.Text.RegularExpressions.Regex.Match(text, @"[\d.]+");
                return match.Value;
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