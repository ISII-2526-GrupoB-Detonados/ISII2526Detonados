using AppForSEII2526.UIT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.PurchaseDevices
{
    public class CreatePurchase_PO : PageObject
    {
        private By _botonComprarBy = By.Id("Submit");
        private By _botonModificarDevicesBy = By.Id("ModifyDevices");
        private By _seleccionarMetodoPagoBy = By.Id("PaymentMethod");
        private By _precioTotalBy = By.XPath("//p[b[contains(text(), 'Total price:')]]");
        private By _nombreBy = By.Id("Name");
        private By _apellidoBy = By.Id("Surname");
        private By _direccionEntregaBy = By.Id("DeliveryAddress");
        private By _erroresBy = By.CssSelector(".validation-message, .validation-errors, ul.validation-errors li, .alert-danger");
        private By _errorsShownBy = By.Id("ErrorsShown");
        private By _tablaPurchaseItemsBy = By.Id("TableOfPurchaseItems");

        private IWebElement _botonComprar() => _driver.FindElement(_botonComprarBy);
        private IWebElement _botonModificarDevices() => _driver.FindElement(_botonModificarDevicesBy);
        private IWebElement _metodoPago() => _driver.FindElement(_seleccionarMetodoPagoBy);
        private IWebElement _nombre() => _driver.FindElement(_nombreBy);
        private IWebElement _apellido() => _driver.FindElement(_apellidoBy);
        private IWebElement _direccionEntrega() => _driver.FindElement(_direccionEntregaBy);

        public CreatePurchase_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void Volver()
        {
            WaitForBeingClickable(_botonModificarDevicesBy);
            _botonModificarDevices().Click();
            System.Threading.Thread.Sleep(200);
        }

        public void Comprar()
        {
            WaitForBeingClickable(_botonComprarBy);
            _botonComprar().Click();
        }

        public void ConfirmarPedido()
        {
            WaitForBeingVisible(By.XPath("//button[contains(text(), 'Guardar')]"));
            WaitForBeingClickable(By.XPath("//button[contains(text(), 'Guardar')]"));

            try
            {
                _driver.FindElement(By.XPath("//button[contains(text(), 'Guardar')]")).Click();
            }
            catch (StaleElementReferenceException)
            {
                _driver.FindElement(By.XPath("//button[contains(text(), 'Guardar')]")).Click();
            }

            System.Threading.Thread.Sleep(2000);
        }

        public void CancelarPedido()
        {
            WaitForBeingVisible(By.XPath("//button[contains(text(), 'No Guardar')]"));
            WaitForBeingClickable(By.XPath("//button[contains(text(), 'No Guardar')]"));

            try
            {
                _driver.FindElement(By.XPath("//button[contains(text(), 'No Guardar')]")).Click();
            }
            catch (StaleElementReferenceException)
            {
                _driver.FindElement(By.XPath("//button[contains(text(), 'No Guardar')]")).Click();
            }

            System.Threading.Thread.Sleep(2000);
        }

        public string ObtenerPrecioTotal()
        {
            try
            {
                WaitForBeingVisible(_precioTotalBy);
                return _driver.FindElement(_precioTotalBy).Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void setMetodoPago(string metodoPago)
        {
            WaitForBeingClickable(_seleccionarMetodoPagoBy);
            SelectElement selectElement = new SelectElement(_metodoPago());
            selectElement.SelectByText(metodoPago);
            _metodoPago().SendKeys(Keys.Tab);
        }

        public void setDatos(string nombre, string apellido, string direccionEntrega, string metodoPago)
        {
            WaitForBeingVisible(_nombreBy);
            WaitForBeingVisible(_apellidoBy);
            WaitForBeingVisible(_direccionEntregaBy);
            WaitForBeingClickable(_seleccionarMetodoPagoBy);

            
            _apellido().SendKeys(apellido);
            _direccionEntrega().SendKeys(direccionEntrega);

            SelectElement selectElement = new SelectElement(_metodoPago());
            selectElement.SelectByText(metodoPago);
        }

        public void setDescripcionDispositivo(int deviceId, string descripcion)
        {
            var descripcionBy = By.Id($"description_{deviceId}");
            WaitForBeingVisible(descripcionBy);
            _driver.FindElement(descripcionBy).SendKeys(descripcion);
        }

        public bool MensajeError(string mensajeError)
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
                return wait.Until(driver =>
                {
                    var elementos = driver.FindElements(_erroresBy);
                    return elementos.Any(e => e.Displayed && !string.IsNullOrEmpty(e.Text) && e.Text.Contains(mensajeError));
                });
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool isEnabledComprar()
        {
            WaitForBeingVisible(_botonComprarBy);
            return _botonComprar().Enabled;
        }

        public bool ComprobarListaDispositivos(List<string[]> dispositivosEsperados)
        {
            var tablaBy = By.Id("TableOfPurchaseItems");
            WaitForBeingVisible(tablaBy);
            return CheckBodyTable(dispositivosEsperados, tablaBy);
        }

        public string ObtenerValorCampo(string campo)
        {
            try
            {
                switch (campo.ToLower())
                {
                    case "name":
                        WaitForBeingVisible(_nombreBy);
                        return _driver.FindElement(_nombreBy).GetAttribute("value");

                    case "surname":
                        WaitForBeingVisible(_apellidoBy);
                        return _driver.FindElement(_apellidoBy).GetAttribute("value");

                    case "address":
                    case "deliveryaddress":
                        WaitForBeingVisible(_direccionEntregaBy);
                        return _driver.FindElement(_direccionEntregaBy).GetAttribute("value");

                    case "payment":
                    case "paymentmethod":
                        WaitForBeingVisible(_seleccionarMetodoPagoBy);
                        var select = new SelectElement(_driver.FindElement(_seleccionarMetodoPagoBy));
                        return select.SelectedOption.Text;

                    default:
                        return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}