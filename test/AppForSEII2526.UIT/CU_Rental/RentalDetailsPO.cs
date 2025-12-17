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
    public class RentalDetailsPO : PageObject
    {
        // Localizadores de elementos
        private By nameSurnameElement = By.Id("NameSurname");
        private By deliveryAddressElement = By.Id("DeliveryAddress");
        private By paymentMethodElement = By.Id("PaymentMethod");
        private By rentalPeriodElement = By.Id("RentalPeriod");
        private By totalPriceElement = By.Id("TotalPrice");
        private By rentedDevicesTable = By.Id("RentedDevices");
        private By errorMessageElement = By.XPath("//*[contains(text(), 'Error')]");

        public RentalDetailsPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        /// <summary>
        /// Verifica que la página de detalles del alquiler se haya cargado correctamente
        /// </summary>
        public bool IsRentalDetailPageLoaded()
        {
            try
            {
                WaitForBeingVisible(nameSurnameElement);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica los detalles principales del alquiler
        /// </summary>
        public bool CheckRentalDetail(string customerNameSurname, string deliveryAddress,
            string paymentMethod, DateTime rentalFrom, DateTime rentalTo, float totalPrice)
        {
            try
            {
                WaitForBeingVisible(totalPriceElement);
                bool result = true;

                // Verificar nombre y apellidos
                string actualNameSurname = _driver.FindElement(nameSurnameElement).Text;
                result = result && actualNameSurname.Contains(customerNameSurname);

                // Verificar dirección
                string actualAddress = _driver.FindElement(deliveryAddressElement).Text;
                result = result && actualAddress.Contains(deliveryAddress);

                // Verificar método de pago
                string actualPaymentMethod = _driver.FindElement(paymentMethodElement).Text;
                result = result && actualPaymentMethod.Contains(paymentMethod);

                // Verificar período de alquiler (formato: dd/MM/yyyy - dd/MM/yyyy)
                string actualPeriod = _driver.FindElement(rentalPeriodElement).Text;
                string expectedPeriod = $"{rentalFrom:dd/MM/yyyy} - {rentalTo:dd/MM/yyyy}";
                result = result && actualPeriod.Contains(rentalFrom.ToString("dd/MM/yyyy"));
                result = result && actualPeriod.Contains(rentalTo.ToString("dd/MM/yyyy"));

                // Verificar precio total
                string actualPrice = _driver.FindElement(totalPriceElement).Text;
                // Eliminar símbolos de moneda y espacios
                string priceClean = actualPrice.Replace("€", "").Replace(" ", "").Trim();
                result = result && priceClean.Contains(totalPrice.ToString("F2"));

                return result;
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error verificando detalles del alquiler: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica la lista de dispositivos alquilados
        /// </summary>
        public bool CheckListOfRentedDevices(List<string[]> expectedDevices)
        {
            return CheckBodyTable(expectedDevices, rentedDevicesTable);
        }

        /// <summary>
        /// Verifica si un dispositivo específico está en la lista de alquilados
        /// </summary>
        public bool CheckDeviceInList(string deviceName, string brand, string model, int quantity, float price)
        {
            try
            {
                By deviceRow = By.Id($"RentalItem_{deviceName}");
                WaitForBeingVisible(deviceRow);

                IWebElement row = _driver.FindElement(deviceRow);
                var cells = row.FindElements(By.TagName("td"));

                if (cells.Count < 5)
                {
                    return false;
                }

                // Verificar cada campo
                bool nameMatch = cells[0].Text.Contains(deviceName);
                bool brandMatch = cells[1].Text.Contains(brand);
                bool modelMatch = cells[2].Text.Contains(model);
                bool quantityMatch = cells[3].Text.Contains(quantity.ToString());
                bool priceMatch = cells[4].Text.Contains(price.ToString("F2")) || cells[4].Text.Contains(price.ToString("F1"));

                return nameMatch && brandMatch && modelMatch && quantityMatch && priceMatch;
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error verificando dispositivo '{deviceName}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene el nombre y apellidos del cliente desde el alquiler
        /// </summary>
        public string GetCustomerNameSurname()
        {
            WaitForBeingVisible(nameSurnameElement);
            return _driver.FindElement(nameSurnameElement).Text;
        }

        /// <summary>
        /// Obtiene la dirección de entrega desde el alquiler
        /// </summary>
        public string GetDeliveryAddress()
        {
            WaitForBeingVisible(deliveryAddressElement);
            return _driver.FindElement(deliveryAddressElement).Text;
        }

        /// <summary>
        /// Obtiene el método de pago desde el alquiler
        /// </summary>
        public string GetPaymentMethod()
        {
            WaitForBeingVisible(paymentMethodElement);
            return _driver.FindElement(paymentMethodElement).Text;
        }

        /// <summary>
        /// Obtiene el período de alquiler desde el alquiler
        /// </summary>
        public string GetRentalPeriod()
        {
            WaitForBeingVisible(rentalPeriodElement);
            return _driver.FindElement(rentalPeriodElement).Text;
        }

        /// <summary>
        /// Obtiene el precio total desde el alquiler
        /// </summary>
        public string GetTotalPrice()
        {
            WaitForBeingVisible(totalPriceElement);
            return _driver.FindElement(totalPriceElement).Text;
        }

        /// <summary>
        /// Verifica si el alquiler contiene un mensaje de error
        /// </summary>
        public bool CheckErrorMessage()
        {
            try
            {
                IWebElement errorElement = _driver.FindElement(errorMessageElement);
                return errorElement.Displayed;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el texto del mensaje de error si existe
        /// </summary>
        public string GetErrorMessageText()
        {
            try
            {
                WaitForBeingVisible(errorMessageElement);
                return _driver.FindElement(errorMessageElement).Text;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Verifica si hay un mensaje de error de autorización
        /// </summary>
        public bool HasAuthorizationError()
        {
            try
            {
                // Buscar el párrafo con el mensaje de error específico
                var errorParagraph = _driver.FindElements(By.XPath("//p[contains(text(), 'Error!') and contains(text(), 'not authorized')]"));
                return errorParagraph.Count > 0 && errorParagraph[0].Displayed;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cuenta el número de dispositivos alquilados en la tabla
        /// </summary>
        public int GetRentedDevicesCount()
        {
            try
            {
                var rows = _driver.FindElements(By.XPath("//table[@id='RentedDevices']//tbody/tr"));
                return rows.Count;
            }
            catch
            {
                return 0;
            }
        }
    }
}