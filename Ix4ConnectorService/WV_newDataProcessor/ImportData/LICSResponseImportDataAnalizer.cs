using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WV_newDataProcessor.ImportData
{
    class LICSResponseImportDataAnalizer
    {
        private SimplestLogger.Logger _logger = SimplestLogger.Logger.GetLogger();
        private List<string> _ordersWithErrors = new List<string>();

        public void CheckImportOrderResult(LICSResponseOrderImport importOrderResult)
        {
            if (importOrderResult == null || importOrderResult.CountOfFailed == 0)
            {
                return;
            }

            foreach (LICSResponseOrderImportOrder orderResultInfo in importOrderResult.Order.Where(o => o.State < 0).ToList())
            {
                if (_ordersWithErrors.Contains(orderResultInfo.OrderNo))
                {
                    _logger.Log(string.Format("Order {0} was not fixed on the previous iteration"));
                    continue;
                }
                if (orderResultInfo.State == -9010002)
                {
                    _ordersWithErrors.Add(orderResultInfo.OrderNo);
                    _logger.Log(string.Format("Order {0} was added with error {1} for fixing on the next iteration", orderResultInfo.OrderNo, orderResultInfo.Message));
                }
            }
        }

        public void AutoFixOrdersExceptions(List<LICSRequestOrder> orders)
        {
            var ordersWithException = orders.Where(o => _ordersWithErrors.Contains(o.OrderNo)).ToList();
            if(ordersWithException!=null)
            {
                foreach (LICSRequestOrder orderForFixing in ordersWithException)
                {
                    try
                    {
                        Fix9010002OrderException(orderForFixing);
                        _ordersWithErrors.Remove(orderForFixing.OrderNo);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log("Error while try to fix order " + orderForFixing.OrderNo);
                        _logger.Log(ex);
                    }
                }
            }
        }


        private void Fix9010002OrderException(LICSRequestOrder orderWithError)
        {
            _logger.Log(string.Format("Order with exception state BEFORE FIXING: Name = {0}, FirstName = {1} , AdditionalName = {2}", orderWithError.Recipient.Name, orderWithError.Recipient.FirstName, orderWithError.Recipient.AdditionalName));
            int nameLenghtLimit = 50;
            int specificFullNamesLength = orderWithError.Recipient.Name.Length + orderWithError.Recipient.FirstName.Length;
            if (specificFullNamesLength > nameLenghtLimit)
            {
                int maxNameLength = nameLenghtLimit - orderWithError.Recipient.FirstName.Length;

                int lastSpaceIndex = orderWithError.Recipient.Name.Substring(0, maxNameLength - 1).LastIndexOf(' ');

                string extraStringFromName = orderWithError.Recipient.Name.Substring(lastSpaceIndex, orderWithError.Recipient.Name.Length - lastSpaceIndex).Trim();
                orderWithError.Recipient.Name = orderWithError.Recipient.Name.Remove(lastSpaceIndex, orderWithError.Recipient.Name.Length - lastSpaceIndex).Trim();

                if (((extraStringFromName + ' ').Length + orderWithError.Recipient.AdditionalName.Length) > 50)
                {
                    int extraCharsInAdditionalName = 50 - ((extraStringFromName + ' ').Length + orderWithError.Recipient.AdditionalName.Length);
                    if (extraCharsInAdditionalName > 0)
                    {
                        extraStringFromName = extraStringFromName.Remove(extraStringFromName.Length - extraCharsInAdditionalName, extraCharsInAdditionalName);
                    }
                    else
                    {
                        _logger.Log(string.Format("Recipients Name is too long in order {0}. We need cut this part: {1}.", orderWithError.OrderNo, extraStringFromName));
                        extraStringFromName = string.Empty;
                    }
                }
                orderWithError.Recipient.AdditionalName = string.Format("{0} {1}", extraStringFromName, orderWithError.Recipient.AdditionalName).Trim();
            }
            _logger.Log(string.Format("Order with exception state AFTER FIXING: Name = {0}, FirstName = {1} , AdditionalName = {2}", orderWithError.Recipient.Name, orderWithError.Recipient.FirstName, orderWithError.Recipient.AdditionalName));
        }

        //private void Fix9010002OrderException(LICSRequestOrder orderWithError)
        //{
        //    _logger.Log(string.Format("Order with exception state: Name = {0}, FirstName = {1} , AdditionalName = {2}",orderWithError.Recipient.Name, orderWithError.Recipient.FirstName, orderWithError.Recipient.AdditionalName));
        //    int nameLenghtLimit = 50;
        //    int specificFullNamesLength = orderWithError.Recipient.Name.Length + orderWithError.Recipient.FirstName.Length;
        //    if (specificFullNamesLength>nameLenghtLimit)
        //    {
        //        int maxNameLength = nameLenghtLimit - orderWithError.Recipient.FirstName.Length;

        //        int lastSpaceIndex = orderWithError.Recipient.Name.Substring(0, maxNameLength - 1).LastIndexOf(' ');

        //        string extraStringFromName = orderWithError.Recipient.Name.Substring(lastSpaceIndex, orderWithError.Recipient.Name.Length - lastSpaceIndex);
        //        orderWithError.Recipient.Name = orderWithError.Recipient.Name.Remove(lastSpaceIndex, orderWithError.Recipient.Name.Length - lastSpaceIndex);

        //        if (((extraStringFromName + ' ').Length + orderWithError.Recipient.AdditionalName.Length) > 50)
        //        {
        //            int extraCharsInAdditionalName = 50 - ((extraStringFromName + ' ').Length + orderWithError.Recipient.AdditionalName.Length);
        //            extraStringFromName = extraStringFromName.Remove(extraStringFromName.Length - extraCharsInAdditionalName, extraCharsInAdditionalName);
        //        }

        //        orderWithError.Recipient.AdditionalName = string.Format("{0} {1}", extraStringFromName, orderWithError.Recipient.AdditionalName);

        //    }
        //}
    }
}
