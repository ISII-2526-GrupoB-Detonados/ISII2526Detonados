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
    public class ReceiptDetailsPO : PageObject
    {
        private By totalPriceElement = By.Id("TotalPrice");

        public ReceiptDetailsPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        // Verifica los detalles principales del recibo
       
        public bool CheckReceiptDetail(string nombre, string apellidos, string direccion,
            DateTime fechaOperacion, float precioTotal)
        {
            WaitForBeingVisible(totalPriceElement);

            bool result = true;
            var nombreApellidos = nombre + " " + apellidos;

            result = result && _driver.FindElement(By.Id("NameSurname")).Text.Contains(nombreApellidos);
            result = result && _driver.FindElement(By.Id("DeliveryAddress")).Text.Contains(direccion);
            result = result && _driver.FindElement(By.Id("OperationDate")).Text.Contains(fechaOperacion.ToString("dd/MM/yyyy"));
            result = result && _driver.FindElement(By.Id("TotalPrice")).Text.Contains(precioTotal.ToString("F2"));

            return result;
        }

        // Verifica la lista de reparaciones en el recibo
        public bool CheckListOfRepairs(List<string[]> expectedRepairs)
        {
            return CheckBodyTable(expectedRepairs, By.Id("Repairs"));
        }

        // Obtiene el nombre del cliente desde el recibo
        public string GetClientName()
        {
            WaitForBeingVisible(By.Id("NameSurname"));
            return _driver.FindElement(By.Id("NameSurname")).Text;
        }

        // Obtiene la dirección de entrega desde el recibo
        public string GetDeliveryAddress()
        {
            WaitForBeingVisible(By.Id("DeliveryAddress"));
            return _driver.FindElement(By.Id("DeliveryAddress")).Text;
        }

        // Obtiene la fecha de operación desde el recibo
        public string GetOperationDate()
        {
            WaitForBeingVisible(By.Id("OperationDate"));
            return _driver.FindElement(By.Id("OperationDate")).Text;
        }

        // Obtiene el precio total desde el recibo
        public string GetTotalPrice()
        {
            WaitForBeingVisible(totalPriceElement);
            return _driver.FindElement(totalPriceElement).Text;
        }

        // Verifica si el recibo contiene un mensaje de error
        public bool CheckErrorMessage()
        {
            try
            {
                IWebElement errorElement = _driver.FindElement(By.XPath("//*[contains(text(), 'Error')]"));
                return errorElement.Displayed;
            }
            catch
            {
                return false;
            }
        }
    }
}