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
    public class SelectRepairForSelectPO : PageObject
    {
        private By inputRepairName = By.Id("inputRepairName");
        private By selectScale = By.Id("selectScale");
        private By buttonSearchRepairs = By.Id("searchRepairs");
        private By tableOfRepairsBy = By.Id("TableOfRepairs");
        private By buttonCreateReceipt = By.Id("createReceiptButton");

        public SelectRepairForSelectPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void SearchRepairs(string repairName, string scale)
        {
            // Esperar a que el input esté listo
            WaitForBeingClickable(inputRepairName);

            if (!string.IsNullOrEmpty(repairName))
            {
                _driver.FindElement(inputRepairName).Clear();
                _driver.FindElement(inputRepairName).SendKeys(repairName);
            }

            // Seleccionar escala
            WaitForBeingClickable(selectScale);
            IWebElement scaleDropdown = _driver.FindElement(selectScale);
            SelectElement selectElement = new SelectElement(scaleDropdown);
            selectElement.SelectByValue(scale);

            // Click en botón buscar
            WaitForBeingClickable(buttonSearchRepairs);
            _driver.FindElement(buttonSearchRepairs).Click();
        }

        public bool CheckListOfRepairs(List<string[]> expectedRepairs)
        {
            return CheckBodyTable(expectedRepairs, tableOfRepairsBy);
        }

        public void AddRepairToReceipt(string repairName)
        {
            By addButton = By.Id("repairToAdd_" + repairName);
            WaitForBeingClickable(addButton);
            _driver.FindElement(addButton).Click();
        }

        public void RemoveRepairFromReceipt(string repairName)
        {
            By removeButton = By.Id("removeRepair_" + repairName);
            WaitForBeingClickable(removeButton);
            _driver.FindElement(removeButton).Click();
        }

        public bool CreateReceiptButtonNotAvailable()
        {
            // El botón no está disponible si el carrito está vacío
            try
            {
                return _driver.FindElement(buttonCreateReceipt).Displayed == false;
            }
            catch (Exception ex)
            {
                return true; // Si no existe, está oculto
            }
        }

        public void ClickCreateReceipt()
        {
            WaitForBeingClickable(buttonCreateReceipt);
            _driver.FindElement(buttonCreateReceipt).Click();
        }

        public bool CheckNoRepairsMessage()
        {
            // Verifica si aparece el mensaje "No repairs found"
            try
            {
                IWebElement element = _driver.FindElement(By.XPath("//*[contains(text(), 'No repairs found')]"));
                return element.Displayed;
            }
            catch
            {
                return false;
            }
        }
    }
}