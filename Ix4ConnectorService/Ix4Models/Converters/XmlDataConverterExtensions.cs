using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Ix4Models.Converters
{
    public static class XmlDataConverterExtensions
    {
        public static LICSRequest ConvertToLICSRequest(this OutputPayLoad navisionOutputPayload)
        {
            LICSRequest licsRequest = new LICSRequest();
            List<LICSRequestArticle> listLicsRequestArticles = new List<LICSRequestArticle>();
            List<LICSRequestOrderPosition> orderPositions = new List<LICSRequestOrderPosition>();

            LICSRequestOrder requestOrder = new LICSRequestOrder();

            requestOrder.ShipmentDate = DateTime.ParseExact(navisionOutputPayload.OrderHeader.Date, "dd.MM.yy", System.Globalization.CultureInfo.InvariantCulture);
            requestOrder.OrderNo = navisionOutputPayload.OrderHeader.OrderNumber;
            requestOrder.CustomerNo = Convert.ToString(navisionOutputPayload.OrderHeader.CustomerNumber);
            requestOrder.DistributionCenter = navisionOutputPayload.OrderHeader.WarehouseOrder;


            LICSRequestOrderRecipient orderRecipient = new LICSRequestOrderRecipient();
            orderRecipient.Name = navisionOutputPayload.OrderHeader.CustomerName;
            //    orderRecipient.AdditionalName = (string)navisionOutputPayload.OrderHeader.CustomerName2;  RESOLVE THIS PROBLEM
            orderRecipient.Street = navisionOutputPayload.OrderHeader.CustomerStreet;
            orderRecipient.ZIPCode = Convert.ToString(navisionOutputPayload.OrderHeader.CustomerZIP);
            orderRecipient.City = navisionOutputPayload.OrderHeader.CustomerTown;

            foreach (OutputPayLoadOutputPosition outputPosition in navisionOutputPayload.Positions)
            {

                LICSRequestOrderPosition orderPosition = new LICSRequestOrderPosition();
                orderPosition.ArticleGroup = outputPosition.VPECode;
                orderPosition.PositionNo = (int)outputPosition.ArticleRowNumber;
                orderPosition.ArticleNo = outputPosition.ArticleNumber;
                orderPosition.TargetQuantity = Convert.ToDouble(outputPosition.VPECount);

                LICSRequestArticle requestArticle = new LICSRequestArticle();
                requestArticle.ArticleNo = outputPosition.ArticleNumber;
                requestArticle.ArticleDescription = outputPosition.Description;
                requestArticle.ArticleDescription2 = outputPosition.Description2;
                requestArticle.ArticleGroup = outputPosition.VPECode;
                requestArticle.ArticleGroupFactor = outputPosition.BaseUnitQuantity;


                orderPositions.Add(orderPosition);

                listLicsRequestArticles.Add(requestArticle);
            }
            requestOrder.Positions = orderPositions.ToArray();
            requestOrder.Recipient = orderRecipient;

            licsRequest.ArticleImport = listLicsRequestArticles.ToArray();
            licsRequest.OrderImport = new LICSRequestOrder[] { requestOrder };
            return licsRequest;
        }
    }
}
