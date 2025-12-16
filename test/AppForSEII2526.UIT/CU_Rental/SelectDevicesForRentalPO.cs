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
    public class SelectDevicesForRentalPO : PageObject
    {
        // Localizadores de elementos
        private By selectModel = By.Id("selectModel");
        private By inputPrice = By.Id("inputPrice");
        private By buttonSearchDevices = By.Id("searchDevices");
        private By tableOfDevicesBy = By.XPath("//table[contains(@class, 'table-hover')]");
        private By errorMessageElement = By.Id("ErrorsShown");
        private By shoppingCartSection = By.XPath("//h3[contains(text(), 'Shopping Cart')]");

        public SelectDevicesForRentalPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        /// <summary>
        /// Busca dispositivos aplicando filtros de modelo y precio
        /// </summary>
        public void SearchDevices(string model, int? maxPrice)
        {
            // Seleccionar modelo
            WaitForBeingClickable(selectModel);
            IWebElement modelDropdown = _driver.FindElement(selectModel);
            SelectElement selectElement = new SelectElement(modelDropdown);
            selectElement.SelectByValue(model);
            Thread.Sleep(200);

            // Ingresar precio máximo si se especifica
            if (maxPrice.HasValue)
            {
                WaitForBeingClickable(inputPrice);
                IWebElement priceInput = _driver.FindElement(inputPrice);
                priceInput.Clear();
                Thread.Sleep(100);
                priceInput.SendKeys(maxPrice.Value.ToString());
                Thread.Sleep(200);
            }

            // Click en buscar
            WaitForBeingClickable(buttonSearchDevices);
            _driver.FindElement(buttonSearchDevices).Click();
            Thread.Sleep(500);
        }

        /// <summary>
        /// Verifica la lista de dispositivos mostrados
        /// </summary>
        public bool CheckListOfDevices(List<string[]> expectedDevices)
        {
            return CheckBodyTable(expectedDevices, tableOfDevicesBy);
        }

        /// <summary>
        /// Añade un dispositivo al carrito de alquiler
        /// </summary>
        public void AddDeviceToCart(string deviceName)
        {
            try
            {
                // Buscar el botón Add dentro de la fila del dispositivo
                By addButton = By.XPath($"//td[contains(text(), '{deviceName}')]/ancestor::tr//button[contains(text(), 'Add')]");
                WaitForBeingClickable(addButton);
                _driver.FindElement(addButton).Click();
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                throw new NoSuchElementException($"No se pudo añadir el dispositivo '{deviceName}' al carrito", ex);
            }
        }

        /// <summary>
        /// Elimina un dispositivo del carrito de alquiler
        /// </summary>
        public void RemoveDeviceFromCart(string deviceName)
        {
            try
            {
                // Buscar el botón × (times) en la tarjeta del dispositivo
                By removeButton = By.XPath($"//strong[contains(text(), '{deviceName}')]/ancestor::div[contains(@class, 'card')]//button[contains(@class, 'btn-danger')]");
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
        /// Verifica si el carrito de compras está visible
        /// </summary>
        public bool IsShoppingCartVisible()
        {
            try
            {
                IWebElement cartSection = _driver.FindElement(shoppingCartSection);
                return cartSection.Displayed;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si hay dispositivos en el carrito
        /// </summary>
        public bool HasDevicesInCart()
        {
            try
            {
                // Buscar si hay tarjetas de dispositivos en el carrito
                var deviceCards = _driver.FindElements(By.XPath("//div[contains(@class, 'card')]//strong"));
                return deviceCards.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el precio total mostrado en el carrito
        /// </summary>
        public string GetTotalPriceInCart()
        {
            try
            {
                // Buscar el elemento que contiene "Total:"
                By totalElement = By.XPath("//div[contains(@class, 'alert-info')]//strong[contains(text(), 'Total:')]");
                WaitForBeingVisible(totalElement);
                string fullText = _driver.FindElement(totalElement).Text;

                // Extraer solo el valor numérico
                var match = System.Text.RegularExpressions.Regex.Match(fullText, @"[\d.,]+");
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
        /// Obtiene el número de dispositivos en el carrito
        /// </summary>
        public int GetDeviceCountInCart()
        {
            try
            {
                // Buscar el texto que indica cantidad: "X device(s) selected"
                By countElement = By.XPath("//div[contains(@class, 'alert-info')]//small");
                WaitForBeingVisible(countElement);
                string text = _driver.FindElement(countElement).Text;

                // Extraer el número
                var match = System.Text.RegularExpressions.Regex.Match(text, @"(\d+)\s+device");
                if (match.Success)
                {
                    return int.Parse(match.Groups[1].Value);
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Hace click en el botón "Rent devices"
        /// </summary>
        public void ClickRentDevices()
        {
            By rentButton = By.XPath("//button[contains(text(), 'Rent devices')]");
            WaitForBeingClickable(rentButton);
            _driver.FindElement(rentButton).Click();
            Thread.Sleep(500);
        }

        /// <summary>
        /// Verifica si el botón "Rent devices" está deshabilitado
        /// </summary>
        public bool IsRentDevicesButtonDisabled()
        {
            try
            {
                By rentButton = By.XPath("//button[contains(text(), 'Rent devices')]");
                WaitForBeingVisible(rentButton);
                var button = _driver.FindElement(rentButton);
                var disabledAttr = button.GetAttribute("disabled");
                return disabledAttr != null || !button.Enabled;
            }
            catch
            {
                return true; // Si no existe, considerarlo deshabilitado
            }
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
        /// Verifica si la tabla de dispositivos está vacía o no se muestran resultados
        /// </summary>
        public bool CheckNoDevicesFound()
        {
            try
            {
                // Buscar si aparece el mensaje "Loading devices..."
                var loadingMessage = _driver.FindElements(By.XPath("//*[contains(text(), 'Loading devices...')]"));
                if (loadingMessage.Count > 0 && loadingMessage[0].Displayed)
                {
                    return false; // Aún está cargando
                }

                // Verificar si la tabla existe y tiene filas
                var table = _driver.FindElements(tableOfDevicesBy);
                if (table.Count == 0)
                {
                    return true; // No hay tabla, no hay dispositivos
                }

                // Verificar si el tbody está vacío
                var rows = _driver.FindElements(By.XPath("//table[contains(@class, 'table-hover')]//tbody/tr"));
                return rows.Count == 0;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Verifica si un dispositivo específico está visible en la lista
        /// </summary>
        public bool IsDeviceVisible(string deviceName)
        {
            try
            {
                By deviceCell = By.XPath($"//td[contains(text(), '{deviceName}')]");
                WaitForBeingVisible(deviceCell);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el precio de un dispositivo específico de la tabla
        /// </summary>
        public string GetDevicePrice(string deviceName)
        {
            try
            {
                // Buscar la fila del dispositivo y obtener el precio (5ta columna)
                By priceCell = By.XPath($"//td[contains(text(), '{deviceName}')]/ancestor::tr/td[5]");
                WaitForBeingVisible(priceCell);
                return _driver.FindElement(priceCell).Text;
            }
            catch
            {
                return "";
            }
        }
    }
}