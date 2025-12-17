using AppForSEII2526.UIT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.PurchaseDevices
{
    public class ListDevicesForPurchase_PO : PageObject
    {
        // IDs exactos según tu HTML
        private By _deviceColorBy = By.Id("inputColor");
        private By _deviceNameBy = By.Id("inputName");
        private By _searchDevicesBy = By.Id("searchDevices");
        private By _purchaseButtonBy = By.Id("purchaseDeviceButton");
        private By _tableOfDevicesBy = By.Id("TableOfDevices");
        private By _errorsShownBy = By.Id("ErrorsShown");
        private By _modalBy = By.Id("DialogOkSaveDelete");
        private By _cartTotalPriceBy = By.Id("cartTotalPrice"); // <-- AÑADIDO

        private IWebElement _deviceColor() => _driver.FindElement(_deviceColorBy);
        private IWebElement _deviceName() => _driver.FindElement(_deviceNameBy);
        private IWebElement _searchDevices() => _driver.FindElement(_searchDevicesBy);
        private IWebElement _purchaseButton() => _driver.FindElement(_purchaseButtonBy);

        public ListDevicesForPurchase_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void FilterDevices(string color, string name)
        {
            WaitForBeingVisible(_deviceColorBy);
            WaitForBeingVisible(_deviceNameBy);

            if (!string.IsNullOrEmpty(color))
                _driver.FindElement(_deviceColorBy).SendKeys(color);

            if (!string.IsNullOrEmpty(name))
                _driver.FindElement(_deviceNameBy).SendKeys(name);

            _searchDevices().Click();
            System.Threading.Thread.Sleep(2000);
        }

        public bool CheckListOfDevices(List<string[]> devicesEsperados)
        {
            WaitForBeingVisible(_tableOfDevicesBy);
            return CheckBodyTable(devicesEsperados, _tableOfDevicesBy);
        }

        public bool IsEnabledPurchase()
        {
            Thread.Sleep(2000);
            try
            {

                var cartDiv = _driver.FindElement(By.CssSelector("div.col-2"));

                return !cartDiv.GetAttribute("hidden")?.Equals("true") ?? true;
            }
            catch
            {
                return false;
            }
        }


        public void SelectDevices(List<string> deviceNames)
        {

            foreach (var deviceName in deviceNames)
            {

                var idBoton = $"deviceToPurchase_{deviceName}";
                var selector = By.Id(idBoton);

                WaitForBeingVisible(selector);

                var element = _driver.FindElement(selector);

                ((IJavaScriptExecutor)_driver).ExecuteScript(
                    "arguments[0].scrollIntoView({block: 'center', behavior: 'smooth'});",
                    element);

                System.Threading.Thread.Sleep(300);

                ((IJavaScriptExecutor)_driver).ExecuteScript(
                    "arguments[0].click();",
                    element);


                System.Threading.Thread.Sleep(800);
            }
        }

        public bool CheckSelectedDevices(List<string> deviceNames)
        {
            foreach (var deviceName in deviceNames)
            {
                var idBoton = $"removeDevice_{deviceName}";
                try
                {
                    _driver.FindElement(By.Id(idBoton));
                }
                catch (OpenQA.Selenium.NoSuchElementException)
                {
                    return false;
                }
            }

            return true;
        }

        public void Purchase()
        {
            WaitForBeingClickable(_purchaseButtonBy);
            _purchaseButton().Click();
            System.Threading.Thread.Sleep(200);
        }

        public void DeselectDevice(string deviceName)
        {
            var idBotonEliminar = $"removeDevice_{deviceName}";
            var selector = By.Id(idBotonEliminar);

            WaitForBeingVisible(selector);
            _driver.FindElement(selector).Click();

            System.Threading.Thread.Sleep(500);
        }

        public bool IsDeviceInCart(string deviceName)
        {
            var idBoton = $"removeDevice_{deviceName}";

            try
            {
                _driver.FindElement(By.Id(idBoton));
                return true;
            }
            catch (OpenQA.Selenium.NoSuchElementException)
            {
                return false;
            }
        }

        public bool CheckErrorMessage(string expectedError)
        {
            return CheckModalBodyText(expectedError, _modalBy);
        }

        public bool CheckErrorMessageNotAvailableDevices(string expectedError)
        {
            try
            {
                WaitForBeingVisible(_errorsShownBy);
                var errorElement = _driver.FindElement(_errorsShownBy);
                if (errorElement.Displayed && errorElement.Text.Contains(expectedError))
                    return true;
            }
            catch
            {
            }

            return _driver.PageSource.Contains(expectedError);
        }

        public bool CheckShoppingCart(string price)
        {
            try
            {
                var cartItems = _driver.FindElements(By.CssSelector("button[id^='removeDevice_']"));

                foreach (var item in cartItems)
                {
                    var itemText = item.Text;

                    if (itemText.Contains("-"))
                    {
                        var parts = itemText.Split('-');
                        if (parts.Length > 1)
                        {
                            var itemPrice = parts[1].Trim();
                            var normalizedItemPrice = itemPrice.Replace("€", "").Replace(" ", "").Trim();
                            var normalizedExpectedPrice = price.Replace(",", ".").Replace(" ", "");

                            if (normalizedItemPrice.Contains(normalizedExpectedPrice) ||
                                normalizedExpectedPrice.Contains(normalizedItemPrice.Replace(",", ".")))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool IsPurchaseButtonDisabled()
        {
            try
            {
                var purchaseButton = _purchaseButton();

                if (!purchaseButton.Displayed)
                    return true;

                var isDisabled = purchaseButton.GetAttribute("disabled") != null ||
                                !purchaseButton.Enabled ||
                                purchaseButton.GetAttribute("class").Contains("disabled");

                return !purchaseButton.Enabled || isDisabled;
            }
            catch
            {
                return true;
            }
        }

        public bool IsTableEmpty()
        {
            try
            {
                WaitForBeingVisible(_tableOfDevicesBy);
                var table = _driver.FindElement(_tableOfDevicesBy);
                var rows = table.FindElements(By.TagName("tr"));

                return rows.Count <= 1;
            }
            catch
            {
                return true;
            }
        }

        public string GetErrorMessage()
        {
            try
            {
                WaitForBeingVisible(_errorsShownBy);
                return _driver.FindElement(_errorsShownBy).Text;
            }
            catch
            {
                return string.Empty;
            }
        }


        public string ObtenerTotalCarrito()
        {
            try
            {
                WaitForBeingVisible(_cartTotalPriceBy);
                var elemento = _driver.FindElement(_cartTotalPriceBy);
                var texto = elemento.Text;

                var numero = texto.Replace("€", "").Trim();
                return numero;
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool VerificarTotalCarrito(string precioEsperado)
        {
            try
            {
                var totalActual = ObtenerTotalCarrito();
                return totalActual.Contains(precioEsperado);
            }
            catch
            {
                return false;
            }
        }
    }
}