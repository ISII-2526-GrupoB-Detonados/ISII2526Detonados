using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace AppForSEII2526.UIT.PurchaseDevices
{
    public class DetailPurchase_PO : PageObject
    {
        private By _purchasedDevicesTableBy = By.Id("PurchasedDevices");

        private By _nameSurnameBy = By.Id("NameSurname");
        private By _deliveryAddressBy = By.Id("DeliveryAddress");
        private By _purchaseDateBy = By.Id("PurchaseDate");
        private By _totalPriceBy = By.Id("TotalPrice");
        private By _paymentMethodBy = By.Id("PaymentMethod");

        private By _tituloDetalleBy = By.XPath("//*[contains(text(), 'DetailPurchase') or contains(text(), 'DetalleCompra')]");

        public DetailPurchase_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public bool CheckListOfDevices(List<string[]> expectedDevices)
        {
            return CheckBodyTable(expectedDevices, _purchasedDevicesTableBy);
        }

        public bool CheckPurchaseDetails(List<string[]> expectedDetails)
        {
            WaitForBeingVisible(_tituloDetalleBy);

            string textoPagina = _driver.FindElement(By.TagName("body")).Text;

            textoPagina = textoPagina.Replace("\r\n", " ").Replace("\n", " ");

            foreach (var fila in expectedDetails)
            {
                foreach (var datoEsperado in fila)
                {
 
                    string datoLimpio = System.Text.RegularExpressions.Regex.Replace(datoEsperado, "<.*?>", String.Empty);

                    if (!string.IsNullOrWhiteSpace(datoLimpio) && !textoPagina.Contains(datoLimpio))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CheckTotal(List<string[]> expectedTotal)
        {
            return CheckFooterTable(expectedTotal, _purchasedDevicesTableBy);
        }

        public bool CheckPurchaseDetail(string expectedName, string expectedAddress,
                                       string expectedPaymentMethod, string expectedTotalPrice)
        {
            WaitForBeingVisible(_tituloDetalleBy);

            bool nameOk = CheckField(_nameSurnameBy, expectedName, "nombre");
            bool addressOk = CheckField(_deliveryAddressBy, expectedAddress, "dirección");
            bool paymentOk = CheckField(_paymentMethodBy, expectedPaymentMethod, "método de pago");
            bool priceOk = CheckField(_totalPriceBy, expectedTotalPrice, "precio total");

            bool dateOk = CheckFieldExists(_purchaseDateBy, "fecha");

            return nameOk && addressOk && paymentOk && priceOk && dateOk;
        }

        private bool CheckField(By selector, string expectedValue, string fieldName)
        {
            try
            {
                WaitForBeingVisible(selector);
                string actualValue = _driver.FindElement(selector).Text;

                if (actualValue.Contains(expectedValue))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool CheckFieldExists(By selector, string fieldName)
        {
            try
            {
                WaitForBeingVisible(selector);
                string value = _driver.FindElement(selector).Text;
                bool exists = !string.IsNullOrEmpty(value);
                return exists;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckFooterTable(List<string[]> filasEsperadas, By IdTable)
        {
            WaitForBeingVisible(IdTable);
            IWebElement table = _driver.FindElement(IdTable);

            IList<IWebElement> footerRows = table.FindElements(By.CssSelector("tfoot tr"));

            if (footerRows.Count == 0)
            {
                footerRows = table.FindElements(By.CssSelector("tbody tr:last-child"));
            }

            IList<IWebElement> filasActuales = footerRows.ToList();

            if (filasActuales.Count == 0 && filasEsperadas.Count > 0)
            {
                return false;
            }

            bool result = true;
            int maxCount = Math.Min(filasEsperadas.Count, filasActuales.Count);

            for (int i = 0; i < maxCount; i++)
            {
                string filaEsperada = string.Join(" ", filasEsperadas[i]);
                string filaActual = filasActuales[i].Text.Replace("\r\n", " ").Replace("\n", " ");

                if (!filaActual.Contains(filaEsperada))
                {
                    result = false;
                }
            }
            return result;
        }
    }
}