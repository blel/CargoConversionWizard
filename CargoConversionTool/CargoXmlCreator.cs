using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoConversionTool
{
    /// <summary>
    /// Creates XML objects from a List of flat prime or flat rejection records.
    /// Needs the 
    /// </summary>
    public class CargoXmlCreator
    {
        private string _iATAPeriod;
        private InvoiceTransmission _invoiceTransmission;

        /// <summary>
        /// Returns a string for the xml filename
        /// </summary>
        /// <param name="sendToSandbox"></param>
        /// <returns></returns>
        public string GetXmlFileName(bool sendToSandbox, string iATAPeriod)
        {
            return
                (sendToSandbox ? Constants.SandboxPrefix : string.Empty) + Constants.ShortBillingCategory +
                Constants.XMLPrefix + Constants.SellerOrganizationID + "20" + IATAPeriodHelper.GetIATAPeriodForXML(iATAPeriod) +
                string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
        }

        /// <summary>
        /// Creates the TransmissionHeader
        /// </summary>
        /// <returns></returns>
        private TransmissionHeader GetTransmissionHeader()
        {
            TransmissionHeader transmissionHeader = new TransmissionHeader();
            transmissionHeader.BillingCategory = Constants.BillingCategory;
            transmissionHeader.IssuingOrganizationID = Constants.SellerOrganizationID;
            transmissionHeader.TransmissionDateTime = string.Format("{0:s}", DateTime.Now);
            transmissionHeader.Version = Constants.XmlVersion;
            return transmissionHeader;
        }

        /// <summary>
        /// Creates the InvoiceHeader
        /// </summary>
        /// <param name="headerData"></param>
        /// <returns></returns>
        private InvoiceHeader GetInvoiceHeader(FlatPrimeRecord headerData)
        {
            InvoiceHeader invoiceHeader = new InvoiceHeader();
            invoiceHeader.InvoiceNumber = headerData.InvoiceNumber;
            invoiceHeader.InvoiceDate = string.Format("{0:yyyy-MM-dd}", headerData.InvoiceDate);
            invoiceHeader.InvoiceType = InvoiceType.Invoice;
            invoiceHeader.ChargeCategory = Constants.ChargeCategory;
            //Seller Organization
            SellerOrganization sellerOrganization = new SellerOrganization();
            sellerOrganization.OrganizationID = Constants.SellerOrganizationID;
            invoiceHeader.SellerOrganization = sellerOrganization;
            //Buyer Organization
            BuyerOrganization buyerOrganization = new BuyerOrganization();
            buyerOrganization.OrganizationID = headerData.BuyerOrganizationID;
            invoiceHeader.BuyerOrganization = buyerOrganization;
            //Payment Terms
            PaymentTerms paymentTerms = new PaymentTerms();
            paymentTerms.CurrencyCode = string.IsNullOrWhiteSpace(headerData.CurrencyCode) ? Constants.ClearanceCurrency : headerData.CurrencyCode;
            paymentTerms.ClearanceCurrencyCode = Constants.ClearanceCurrency;
            paymentTerms.ExchangeRate = paymentTerms.CurrencyCode == Constants.ClearanceCurrency ? 1.00000m : headerData.ExchangeRate;
            paymentTerms.ExchangeRateSpecified = true;
            paymentTerms.SettlementMethod = SettlementMethod.I;
            paymentTerms.SettlementMethodSpecified = true;

            paymentTerms.SettlementMonthPeriod = IATAPeriodHelper.GetIATAPeriodForXML(_iATAPeriod);
            invoiceHeader.PaymentTerms = paymentTerms;
            //ISDetails
            ISDetails isDetails = new ISDetails();
            isDetails.DigitalSignatureFlag = DigitalSignatureFlag.Y;
            invoiceHeader.ISDetails = isDetails;
            return invoiceHeader;
        }

        /// <summary>
        /// Returns an InvoiceTransmission with values from the 
        /// FlatCargoFile parameter
        /// 
        /// </summary>
        /// <param name="primeFile"></param>
        /// <returns></returns>
        public InvoiceTransmission GetInvoiceTransmission(FlatPrimeFile primeFile, FlatRejectionFile rejectionFile, string iATAPeriod)
        {
            _iATAPeriod = iATAPeriod;
            List<Invoice> allInvoices = new List<Invoice>();

            //create transmission header
            _invoiceTransmission = new InvoiceTransmission();
            _invoiceTransmission.TransmissionHeader = GetTransmissionHeader();

            //add primeInvoices
            allInvoices.AddRange(GetInvoices(primeFile));
            //add rejection invoices
            allInvoices.AddRange(GetInvoices(rejectionFile));

            _invoiceTransmission.Invoice = allInvoices.ToArray();

            //Transmission Summary
            _invoiceTransmission.TransmissionSummary = new TransmissionSummary();
            _invoiceTransmission.TransmissionSummary.InvoiceCount = allInvoices.Count.ToString();
            _invoiceTransmission.TransmissionSummary.TotalAmount = GetAmountsPerCurrency().ToArray();
            return _invoiceTransmission;
        }

        /// <summary>
        /// Returns the number of currencies
        /// </summary>
        /// <returns></returns>
        private List<string> GetInvoicedCurrencies()
        {
            return
                (from Invoice inv in _invoiceTransmission.Invoice
                 select inv.InvoiceHeader.PaymentTerms.CurrencyCode).Distinct().ToList();
        }

        /// <summary>
        /// Returns the total amount per currency of all invoices
        /// </summary>
        /// <returns></returns>
        private List<TransmissionSummaryTotalAmount> GetAmountsPerCurrency()
        {
            List<TransmissionSummaryTotalAmount> amountsPerCurrency = new List<TransmissionSummaryTotalAmount>();
            foreach (string currency in GetInvoicedCurrencies())
            {
                TransmissionSummaryTotalAmount totalAmount = new TransmissionSummaryTotalAmount();
                totalAmount.CurrencyCode = currency;
                totalAmount.Value = (from inv in _invoiceTransmission.Invoice
                                     where inv.InvoiceHeader.PaymentTerms.CurrencyCode == currency
                                     select inv.InvoiceSummary.TotalAmount.Value).Sum();
                amountsPerCurrency.Add(totalAmount);
            }
            return amountsPerCurrency;

        }

        /// <summary>
        /// Returns the calculated isc percentage rounded to 2 decimals.
        /// </summary>
        /// <param name="WeightCharge"></param>
        /// <param name="ValuationCharge"></param>
        /// <param name="IscAmount"></param>
        /// <returns></returns>
        private decimal CalculateIscPercentage(decimal WeightCharge, decimal ValuationCharge, decimal IscAmount)
        {
            return Math.Round(IscAmount / (WeightCharge + ValuationCharge) * 100, 2);
        }

        /// <summary>
        /// Returns all rejection invoices in a list
        /// </summary>
        /// <param name="rejectionFile"></param>
        /// <returns></returns>
        private List<Invoice> GetInvoices(FlatRejectionFile rejectionFile)
        {
            List<Invoice> rejections = new List<Invoice>();
            int batchSequenceNumber = 1;
            foreach (List<FlatRejectionRecord> flatRejectionFileInvoice in rejectionFile.GetInvoices())
            {
                rejections.Add(GetInvoice(flatRejectionFileInvoice, batchSequenceNumber));
                batchSequenceNumber += 1;
            }
            return rejections;
        }

        /// <summary>
        /// Returns all primeAWB Billings
        /// </summary>
        /// <param name="primeFile"></param>
        /// <returns></returns>
        private List<Invoice> GetInvoices(FlatPrimeFile primeFile)
        {
            List<Invoice> primeBillings = new List<Invoice>();
            int batchSequenceNumber = 1;
            foreach (List<FlatPrimeRecord> flatCargoFileInvoice in primeFile.GetInvoices())
            {
                primeBillings.Add(GetInvoice(flatCargoFileInvoice, batchSequenceNumber));
                batchSequenceNumber += 1;
            }
            return primeBillings;
        }

        /// <summary>
        /// Returns a Rejection Memo Invoice
        /// </summary>
        /// <param name="flatRejectionFileInvoice"></param>
        /// <param name="batchSequenceNumber"></param>
        /// <returns></returns>
        private Invoice GetInvoice(List<FlatRejectionRecord> flatRejectionFileInvoice, int batchSequenceNumber)
        {
            Invoice invoice = new Invoice();
            invoice.InvoiceHeader = GetInvoiceHeader(flatRejectionFileInvoice.FirstOrDefault());
            List<LineItem> allLineItems = new List<LineItem>();
            int recordSequenceWithinBatch = 1;
            foreach (List<FlatRejectionRecord> flatRejectionFileLineItems in FlatRejectionFile.GetLineItems((flatRejectionFileInvoice)))                                                                                                  
            {
                List<LineItemDetail> allLineItemDetails = new List<LineItemDetail>();
                foreach (List<FlatRejectionRecord> flatRejectionMemoDetail in FlatRejectionFile.GetRejectionMemos((flatRejectionFileLineItems)))
                {
                    allLineItemDetails.Add(GetLineItemDetail(flatRejectionMemoDetail, allLineItems.Count(), allLineItemDetails.Count(), batchSequenceNumber, recordSequenceWithinBatch));
                    recordSequenceWithinBatch += 1;
                }
                //add collection to invoice
                if (invoice.LineItemDetail == null)
                    invoice.LineItemDetail = allLineItemDetails.ToArray();
                else
                {
                    List<LineItemDetail> tmpLiList = new List<LineItemDetail>();
                    tmpLiList = invoice.LineItemDetail.ToList<LineItemDetail>();
                    tmpLiList.AddRange(allLineItemDetails);
                    invoice.LineItemDetail = tmpLiList.ToArray();
                }
                allLineItems.Add(GetLineItem(allLineItemDetails, allLineItems.Count, Constants.RejectionMemoChargeCode));
            }
            invoice.LineItem = allLineItems.ToArray();
            invoice.InvoiceSummary = GetInvoiceSummary(invoice);
            return invoice;
        }


        private Invoice GetInvoice(List<FlatPrimeRecord> flatCargoFileInvoice, int batchSequenceNumber)
        {
            Invoice invoice = new Invoice();
            invoice.InvoiceHeader = GetInvoiceHeader(flatCargoFileInvoice.FirstOrDefault());
            List<LineItem> allLineItems = new List<LineItem>();
            int recordSequenceWithinBatch = 1;
            foreach (List<FlatPrimeRecord> flatCargoFileLineItems in FlatPrimeFile.GetLineItems(flatCargoFileInvoice))
            {
                List<LineItemDetail> allLineItemDetails = new List<LineItemDetail>();
                #region LineItemDetails
                foreach (FlatPrimeRecord flatCargoLineItemDetail in flatCargoFileLineItems)
                {
                    allLineItemDetails.Add(GetLineItemDetail(flatCargoLineItemDetail, allLineItems.Count(), allLineItemDetails.Count(), batchSequenceNumber, recordSequenceWithinBatch));
                    recordSequenceWithinBatch += 1;
                }
                //add collection to invoice
                if (invoice.LineItemDetail == null)
                    invoice.LineItemDetail = allLineItemDetails.ToArray();
                else
                {
                    List<LineItemDetail> tmpLiList = new List<LineItemDetail>();
                    tmpLiList = invoice.LineItemDetail.ToList<LineItemDetail>();
                    tmpLiList.AddRange(allLineItemDetails);
                    invoice.LineItemDetail = tmpLiList.ToArray();
                }
                #endregion
                allLineItems.Add(GetLineItem(allLineItemDetails, allLineItems.Count, flatCargoFileLineItems.FirstOrDefault().ChargeCode));

            }
            invoice.LineItem = allLineItems.ToArray();

            invoice.InvoiceSummary = GetInvoiceSummary(invoice);
            return invoice;
        }

        /// <summary>
        /// Returns the invoice header - this is currently used only by rejection invoices.
        /// </summary>
        /// <param name="headerData"></param>
        /// <returns></returns>
        private InvoiceHeader GetInvoiceHeader(FlatRejectionRecord headerData)
        {
            InvoiceHeader invoiceHeader = new InvoiceHeader();
            invoiceHeader.InvoiceNumber = headerData.InvoiceNumber;
            invoiceHeader.InvoiceDate = string.Format("{0:yyyy-MM-dd}", headerData.InvoiceDate);
            invoiceHeader.InvoiceType = InvoiceType.Invoice;
            invoiceHeader.ChargeCategory = Constants.ChargeCategory;
            //Seller Organization
            SellerOrganization sellerOrganization = new SellerOrganization();
            sellerOrganization.OrganizationID = Constants.SellerOrganizationID;
            invoiceHeader.SellerOrganization = sellerOrganization;
            //Buyer Organization
            BuyerOrganization buyerOrganization = new BuyerOrganization();
            buyerOrganization.OrganizationID = headerData.BuyerOrganizationID;
            invoiceHeader.BuyerOrganization = buyerOrganization;
            //Payment Terms
            PaymentTerms paymentTerms = new PaymentTerms();
            paymentTerms.CurrencyCode = string.IsNullOrWhiteSpace(headerData.CurrencyCode) ? Constants.ClearanceCurrency : headerData.CurrencyCode;
            paymentTerms.ClearanceCurrencyCode = Constants.ClearanceCurrency;
            paymentTerms.ExchangeRate = paymentTerms.CurrencyCode == Constants.ClearanceCurrency ? 1.00000m : headerData.ExchangeRate;
            paymentTerms.ExchangeRateSpecified = true;
            paymentTerms.SettlementMethod = SettlementMethod.I;
            paymentTerms.SettlementMethodSpecified = true;

            paymentTerms.SettlementMonthPeriod = IATAPeriodHelper.GetIATAPeriodForXML(_iATAPeriod);
            invoiceHeader.PaymentTerms = paymentTerms;
            //ISDetails
            ISDetails isDetails = new ISDetails();
            isDetails.DigitalSignatureFlag = DigitalSignatureFlag.Y;
            invoiceHeader.ISDetails = isDetails;
            return invoiceHeader;
        }

        /// <summary>
        /// Returns the InvoiceSummary of the invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        private InvoiceSummary GetInvoiceSummary(Invoice invoice)
        {
            InvoiceSummary invoiceSummary = new InvoiceSummary();
            invoiceSummary.LineItemCount = invoice.LineItem.Count().ToString();
            invoiceSummary.TotalLineItemAmount =
                (from cc in invoice.LineItem
                 select cc.ChargeAmount[0].Value + cc.ChargeAmount[1].Value).Sum();

            invoiceSummary.TotalAmount = new TotalAmount()
            {
                Value =
                    (from cc in invoice.LineItem
                     select cc.TotalNetAmount).Sum()
            };
            invoiceSummary.AddOnCharges = new InvoiceSummaryAddOnCharges[]
            {
                new InvoiceSummaryAddOnCharges(){AddOnChargeName = "ISCAllowed",
                                                 AddOnChargeAmount = (from cc in invoice.LineItem 
                                                                     select (from ccc in cc.AddOnCharges
                                                                            where ccc.AddOnChargeName == "ISCAllowed"
                                                                            select ccc.AddOnChargeAmount).FirstOrDefault()).Sum()
                },
                new InvoiceSummaryAddOnCharges(){AddOnChargeName = "OtherChargesAllowed",
                                                 AddOnChargeAmount = (from cc in invoice.LineItem 
                                                                     select (from ccc in cc.AddOnCharges
                                                                            where ccc.AddOnChargeName == "OtherChargesAllowed"
                                                                            select ccc.AddOnChargeAmount).FirstOrDefault()).Sum()
                }
            };
            invoiceSummary.TotalAmountWithoutVAT = invoiceSummary.TotalAmount.Value;
            invoiceSummary.TotalAmountWithoutVATSpecified = true;
            invoiceSummary.TotalAmountInClearanceCurrency = invoiceSummary.TotalAmount.Value;
            invoiceSummary.TotalAmountInClearanceCurrencySpecified = true;
            return invoiceSummary;
        }

        /// <summary>
        /// Returns a Line Item for a rejection memo invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="lineItemCount"></param>
        /// <returns></returns>
        private LineItem GetLineItem(List<LineItemDetail> allLineItemDetails, int lineItemCount, string chargeCode)
        {
            LineItem lineItem = new LineItem();
            lineItem.LineItemNumber = (lineItemCount + 1).ToString();
            lineItem.ChargeCode = chargeCode;

            decimal iSCTotal = 0m;
            decimal otherChargesTotal = 0m;

            foreach (LineItemDetail liDetail in allLineItemDetails)
            {
                if (liDetail.AddOnCharges != null)
                {
                    foreach (AddOnCharges aoc in liDetail.AddOnCharges)
                    {
                        if (chargeCode == Constants.RejectionMemoChargeCode)
                        {
                            if (aoc.AddOnChargeName == "ISCDifference")
                                iSCTotal += aoc.AddOnChargeAmount;
                            if (aoc.AddOnChargeName == "OtherChargesDifference")
                                otherChargesTotal += aoc.AddOnChargeAmount;
                        }
                        else
                        {
                            if (aoc.AddOnChargeName == "ISCAllowed")
                                iSCTotal += aoc.AddOnChargeAmount;
                            if (aoc.AddOnChargeName == "OtherChargesAllowed")
                                otherChargesTotal += aoc.AddOnChargeAmount;
                        }
                    }
                }
            }

            List<AddOnCharges> liAOC = new List<AddOnCharges>();

            if (iSCTotal != 0)
            {
                AddOnCharges addOnCharges = new AddOnCharges();
                addOnCharges.AddOnChargeName = "ISCAllowed";
                addOnCharges.AddOnChargeAmount = iSCTotal;
                liAOC.Add(addOnCharges);
            }

            if (otherChargesTotal != 0)
            {
                AddOnCharges addOnCharges = new AddOnCharges();
                addOnCharges.AddOnChargeName = "OtherChargesAllowed";
                addOnCharges.AddOnChargeAmount = otherChargesTotal;
                liAOC.Add(addOnCharges);
            }
            lineItem.AddOnCharges = liAOC.ToArray();

            lineItem.ChargeAmount = new LineItemChargeAmount[] 
                        {new LineItemChargeAmount 
                            {Name = LineItemChargeAmountName.Weight,
                                            NameSpecified = true,
                                            Value = (from cc in allLineItemDetails 
                                                    select cc.ChargeAmount[chargeCode == Constants.RejectionMemoChargeCode ? 2:0].Value).Sum()
                            },
                            new LineItemChargeAmount 
                            {Name = LineItemChargeAmountName.Valuation ,
                                            NameSpecified = true,
                                            Value = (from cc in allLineItemDetails
                                                    select cc.ChargeAmount[chargeCode == Constants.RejectionMemoChargeCode ? 5:1].Value).Sum()
                            }
                        };

            lineItem.TotalNetAmount = (from cc in allLineItemDetails
                                       select cc.TotalNetAmount).Sum();

            lineItem.TotalNetAmountSpecified = true;
            lineItem.DetailCount = (from cc in allLineItemDetails
                                    select cc).Count().ToString();
            return lineItem;

        }

        /// <summary>
        /// Returns a lineItemDetail for a Rejection Memo
        /// </summary>
        /// <param name="lineItemDetails"></param>
        /// <param name="lineItemNo"></param>
        /// <param name="lineItemDetailNo"></param>
        /// <param name="batchSequenceNumber"></param>
        /// <param name="recordSequenceWithinBatch"></param>
        /// <returns></returns>
        private LineItemDetail GetLineItemDetail(List<FlatRejectionRecord> lineItemDetails, int lineItemNo, int lineItemDetailNo, int batchSequenceNumber, int recordSequenceWithinBatch)
        {
            LineItemDetail lineItemDetail = new LineItemDetail();

            lineItemDetail.DetailNumber = (lineItemDetailNo + 1).ToString();
            lineItemDetail.LineItemNumber = (lineItemNo + 1).ToString();
            lineItemDetail.BatchSequenceNumber = batchSequenceNumber.ToString();
            lineItemDetail.RecordSequenceWithinBatch = recordSequenceWithinBatch.ToString();
            lineItemDetail.RejectionMemoDetails = GetRejectionMemoDetails(lineItemDetails);
            lineItemDetail.ChargeAmount =
                new ChargeAmount[] {
                                new ChargeAmount(){
                                    Name = ChargeAmountName.WeightBilled,
                                    NameSpecified=true,
                                    Value = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown
                                             select 
                                               (from ccc in cc.ChargeAmount
                                                where ccc.Name == ChargeAmountName.WeightBilled
                                                select ccc.Value).FirstOrDefault()).Sum()
                                },
                                new ChargeAmount(){
                                    Name = ChargeAmountName.WeightAccepted ,
                                    NameSpecified=true,
                                    Value = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown
                                             select
                                             (from ccc in cc.ChargeAmount 
                                             
                                            where ccc.Name == ChargeAmountName.WeightAccepted 
                                            select ccc.Value).FirstOrDefault()).Sum()
                                },
                                new ChargeAmount(){
                                    Name = ChargeAmountName.WeightDifference,
                                    NameSpecified=true,
                                    Value = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown
                                            select
                                             (from ccc in cc.ChargeAmount 
                                             where ccc.Name == ChargeAmountName.WeightDifference 
                                            select ccc.Value).FirstOrDefault()).Sum()
                                },
                                new ChargeAmount(){
                                    Name = ChargeAmountName.ValuationBilled,
                                    NameSpecified=true,
                                    Value = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown
                                             select 
                                             (from ccc in cc.ChargeAmount 
                                            where ccc.Name == ChargeAmountName.ValuationBilled
                                            select ccc.Value).FirstOrDefault()).Sum()
                                },
                                new ChargeAmount(){
                                    Name = ChargeAmountName.ValuationAccepted,
                                    NameSpecified=true,
                                    Value = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown
                                             select
                                             (from ccc in cc.ChargeAmount 
                                            where ccc.Name == ChargeAmountName.ValuationAccepted 
                                            select ccc.Value).FirstOrDefault()).Sum()
                                },
                                new ChargeAmount(){
                                    Name = ChargeAmountName.ValuationDifference,
                                    NameSpecified=true,
                                    Value = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown
                                             select
                                             (from ccc in cc.ChargeAmount 
                                            where ccc.Name == ChargeAmountName.ValuationDifference 
                                            select ccc.Value).FirstOrDefault()).Sum() } 
                            };
            lineItemDetail.AddOnCharges = new AddOnCharges[]{
                new AddOnCharges(){AddOnChargeName = "ISCAllowed",
                                   AddOnChargeAmount = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown
                                                        select
                                                        (from ccc in cc.AddOnCharges 
                                                        where ccc.AddOnChargeName =="ISCAllowed"
                                                        select ccc.AddOnChargeAmount).FirstOrDefault()).Sum()},
                new AddOnCharges(){AddOnChargeName = "ISCAccepted",
                                   AddOnChargeAmount = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown 
                                                        select(from ccc in cc.AddOnCharges 
                                                        where ccc.AddOnChargeName =="ISCAccepted"
                                                        select ccc.AddOnChargeAmount).FirstOrDefault()).Sum()},
                new AddOnCharges(){AddOnChargeName = "ISCDifference",
                                   AddOnChargeAmount = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown 
                                                        select(from ccc in cc.AddOnCharges 
                                                        where ccc.AddOnChargeName =="ISCDifference"
                                                        select ccc.AddOnChargeAmount).FirstOrDefault()).Sum()},
                new AddOnCharges(){AddOnChargeName = "OtherChargesAllowed",
                                   AddOnChargeAmount = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown 
                                                        select(from ccc in cc.AddOnCharges 
                                                        where ccc.AddOnChargeName =="OtherChargesAllowed"
                                                        select ccc.AddOnChargeAmount).FirstOrDefault()).Sum()},
                new AddOnCharges(){AddOnChargeName = "OtherChargesAccepted",
                                   AddOnChargeAmount = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown 
                                                        select(from ccc in cc.AddOnCharges  
                                                        where ccc.AddOnChargeName =="OtherChargesAccepted"
                                                        select ccc.AddOnChargeAmount).FirstOrDefault()).Sum()},
                new AddOnCharges(){AddOnChargeName = "OtherChargesDifference",
                                   AddOnChargeAmount = (from cc in lineItemDetail.RejectionMemoDetails.AirWaybillBreakdown 
                                                        select(from ccc in cc.AddOnCharges 
                                                        where ccc.AddOnChargeName =="OtherChargesDifference"
                                                        select ccc.AddOnChargeAmount).FirstOrDefault()).Sum()}
            };
            lineItemDetail.TotalNetAmount = (from cc in lineItemDetail.ChargeAmount
                                             where cc.Name == ChargeAmountName.WeightDifference
                                             select cc.Value).FirstOrDefault() +
                                            (from cc in lineItemDetail.ChargeAmount
                                             where cc.Name == ChargeAmountName.ValuationDifference
                                             select cc.Value).FirstOrDefault() +
                                             (from cc in lineItemDetail.AddOnCharges
                                              where cc.AddOnChargeName == "ISCDifference"
                                              select cc.AddOnChargeAmount).FirstOrDefault() +
                                             (from cc in lineItemDetail.AddOnCharges
                                              where cc.AddOnChargeName == "OtherChargesDifference"
                                              select cc.AddOnChargeAmount).FirstOrDefault();
            lineItemDetail.TotalNetAmountSpecified = true;
            return lineItemDetail;
        }


        private LineItemDetail GetLineItemDetail(FlatPrimeRecord flatCargoLineItemDetail, int lineItemNo, int lineItemDetailNo, int batchSequenceNumber, int recordSequenceWithinBatch)
        {
            LineItemDetail lineItemDetail = new LineItemDetail();
            lineItemDetail.DetailNumber = (lineItemDetailNo + 1).ToString();
            lineItemDetail.LineItemNumber = (lineItemNo + 1).ToString();
            lineItemDetail.BatchSequenceNumber = batchSequenceNumber.ToString();
            lineItemDetail.RecordSequenceWithinBatch = recordSequenceWithinBatch.ToString();

            lineItemDetail.ChargeAmount =
                new ChargeAmount[] {
                                new ChargeAmount(){
                                    Name = ChargeAmountName.WeightBilled,
                                    NameSpecified=true,
                                    Value = flatCargoLineItemDetail.DetailsWeightChargeAmount
                                },
                                new ChargeAmount(){
                                    Name = ChargeAmountName.ValuationBilled,
                                    NameSpecified=true,
                                    Value = flatCargoLineItemDetail.DetailsValuationChargeAmount
                                }                           
                            };

            #region LineItemDetail Addon Charges
            //----------------------------------------------------------------
            //--Addon Charges
            //----------------------------------------------------------------

            //TODO:Number Format??
            List<AddOnCharges> allAddonCharges = new List<AddOnCharges>();
            decimal allAddonChargesAmount = 0;
            //decimal allISCAmount = 0;
            //decimal allOtherChargesAmount = 0;
            if (flatCargoLineItemDetail.DetailsISCAllowed != 0)
            {
                AddOnCharges addonCharges = new AddOnCharges();
                addonCharges.AddOnChargeName = "AmountSubjectToISCAllowed";
                addonCharges.AddOnChargeAmount = flatCargoLineItemDetail.DetailsAmountSubjectToISCAllowed;
                allAddonCharges.Add(addonCharges);

                addonCharges = new AddOnCharges();
                addonCharges.AddOnChargeName = "ISCAllowed";
                addonCharges.AddOnChargeAmount = flatCargoLineItemDetail.DetailsISCAllowed;
                addonCharges.AddOnChargePercentageSpecified = true;
                addonCharges.AddOnChargePercentage = flatCargoLineItemDetail.ChargeCode == Constants.PrepaidChargeCode ? 
                   - flatCargoLineItemDetail.DetailsISCPercentage : flatCargoLineItemDetail.DetailsISCPercentage ;
                allAddonCharges.Add(addonCharges);
                allAddonChargesAmount += flatCargoLineItemDetail.DetailsISCAllowed;
                //allISCAmount += flatCargoFileLineItem.DetailsISCAllowed;
            }
            if (flatCargoLineItemDetail.DetailsOtherChargesAllowed != 0)
            {
                AddOnCharges addonCharges = new AddOnCharges();
                addonCharges.AddOnChargeName = "OtherChargesAllowed";
                addonCharges.AddOnChargeAmount = flatCargoLineItemDetail.DetailsOtherChargesAllowed;
                allAddonCharges.Add(addonCharges);
                allAddonChargesAmount += flatCargoLineItemDetail.DetailsOtherChargesAllowed;
                //allOtherChargesAmount += flatCargoFileLineItem.DetailsOtherChargesAllowed;
            }
            if (allAddonCharges.Count > 0) lineItemDetail.AddOnCharges = allAddonCharges.ToArray();

            #endregion

            lineItemDetail.TotalNetAmount = flatCargoLineItemDetail.DetailsValuationChargeAmount +
                                            flatCargoLineItemDetail.DetailsWeightChargeAmount +
                                            allAddonChargesAmount;
            lineItemDetail.TotalNetAmountSpecified = true;

            #region AWB DATA
            //----------------------------------------------------------------
            //--AirWaybill Data
            //----------------------------------------------------------------

            AirWaybillDetails awbDetails = new AirWaybillDetails();
            awbDetails.AWBDate = string.Format("{0:yyyy-MM-dd}", flatCargoLineItemDetail.AWBDate);
            awbDetails.AWBIssuingAirline = flatCargoLineItemDetail.AWBIssuingAirline.ToString();
            awbDetails.AWBSerialNumber = flatCargoLineItemDetail.AWBSerialNumber.ToString();
            awbDetails.AWBCheckDigit = flatCargoLineItemDetail.AWBCheckDigit.ToString();
            awbDetails.OriginAirportCode = flatCargoLineItemDetail.OriginAirportCode;
            awbDetails.DestinationAirportCode = flatCargoLineItemDetail.DestinationAirportCode;
            awbDetails.FromAirportCode = flatCargoLineItemDetail.FromAirportCode;
            awbDetails.ToAirportOrPointOfTransferCode = flatCargoLineItemDetail.ToAirportOrPointOfTransferCode;
            awbDetails.DateOfCarriageOrTransfer = string.Format("{0:yyyy-MM-dd}", flatCargoLineItemDetail.DateOfCarriageOrTransfer);
            awbDetails.CurrAdjustmentIndicator = flatCargoLineItemDetail.CurrAdjustmentIndicator;


            lineItemDetail.AirWaybillDetails = awbDetails;

            #endregion
            //add LineItem to allLineItems collection
            return lineItemDetail;
        }

        /// <summary>
        /// Returns the Rejection Memo details of a rejection memo
        /// </summary>
        /// <param name="lineItemDetails"></param>
        /// <returns></returns>
        private RejectionMemoDetails GetRejectionMemoDetails(List<FlatRejectionRecord> lineItemDetails)
        {
            RejectionMemoDetails rejectionMemoDetails = new RejectionMemoDetails();
            FlatRejectionRecord rejectionMemoHeaderData = lineItemDetails.FirstOrDefault();
            rejectionMemoDetails.RejectionMemoNumber = rejectionMemoHeaderData.RejectionMemoNumber;
            rejectionMemoDetails.RejectionStage = rejectionMemoHeaderData.RejectionStage.ToString();
            rejectionMemoDetails.ReasonCode = rejectionMemoHeaderData.ReasonCode;
            rejectionMemoDetails.YourInvoiceNumber = rejectionMemoHeaderData.YourInvoiceNumber;
            rejectionMemoDetails.YourInvoiceBillingDate = rejectionMemoHeaderData.YourInvoiceBillingDate; 
            rejectionMemoDetails.YourRejectionMemoNumber = rejectionMemoHeaderData.YourRejectionMemoNumber;
            //check whether we need attachment details on this level as well
            rejectionMemoDetails.AirWaybillBreakdown = GetAirwaybillBreakdown(lineItemDetails, rejectionMemoHeaderData.RejectionStage);
            return rejectionMemoDetails;
        }

        /// <summary>
        /// Returns an array of the AirwayBill Breakdown for a rejection memo
        /// </summary>
        /// <param name="lineItemDetails"></param>
        /// <returns></returns>
        private RejectionMemoDetailsAirWaybillBreakdown[] GetAirwaybillBreakdown(List<FlatRejectionRecord> lineItemDetails, int rejectionStage)
        {
            List<RejectionMemoDetailsAirWaybillBreakdown> allAwbBreakdowns = new List<RejectionMemoDetailsAirWaybillBreakdown>();
            int breakdownSerialnumber = 1;
            foreach (FlatRejectionRecord flatRejectionFileRejectionAwbs in lineItemDetails)
            {
                RejectionMemoDetailsAirWaybillBreakdown awbBreakdown = new RejectionMemoDetailsAirWaybillBreakdown();
                awbBreakdown.BreakdownSerialNumber = breakdownSerialnumber.ToString();
                awbBreakdown.AWBDate = string.Format("{0:yyyy-MM-dd}",flatRejectionFileRejectionAwbs.AWBDate);
                awbBreakdown.BillingCode = (BillingCode)Enum.Parse(typeof(BillingCode), flatRejectionFileRejectionAwbs.BillingCode);
                awbBreakdown.AWBIssuingAirline = flatRejectionFileRejectionAwbs.AWBIssuingAirline;
                awbBreakdown.AWBSerialNumber = flatRejectionFileRejectionAwbs.AWBSerialNumber.ToString();
                awbBreakdown.AWBCheckDigit = flatRejectionFileRejectionAwbs.AWBCheckDigit.ToString();
                awbBreakdown.OriginAirportCode = flatRejectionFileRejectionAwbs.OriginAirportCode;
                awbBreakdown.DestinationAirportCode = flatRejectionFileRejectionAwbs.DestinationAirportCode;
                awbBreakdown.FromAirportCode = flatRejectionFileRejectionAwbs.FromAirportCode;
                awbBreakdown.ToAirportOrPointOfTransferCode = flatRejectionFileRejectionAwbs.ToAirportOrPointOfTransferCode;
                awbBreakdown.DateOfCarriageOrTransfer = string.Format("{0:yyyy-MM-dd}",flatRejectionFileRejectionAwbs.DateOfCarriageOrTransfer);
                Attachment att = new Attachment();
                att.AttachmentIndicatorOriginal = (AttachmentIndicatorOriginalFlag)
                       Enum.Parse(typeof(AttachmentIndicatorOriginalFlag), flatRejectionFileRejectionAwbs.AttachmentIndicatorOriginal);
                att.AttachmentIndicatorOriginalSpecified = true;
                awbBreakdown.Attachment = new Attachment[1] { att };
                awbBreakdown.ChargeAmount = new ChargeAmount[] 
                {
                    new ChargeAmount(){Name= ChargeAmountName.WeightBilled,
                                       NameSpecified = true,
                                       Value = flatRejectionFileRejectionAwbs.ChargeAmountWeightBilled},
                    new ChargeAmount(){Name = ChargeAmountName.WeightAccepted,
                                       NameSpecified = true,
                                       Value = flatRejectionFileRejectionAwbs.ChargeAmountWeightAccepted},
                    new ChargeAmount(){Name = ChargeAmountName.WeightDifference,
                                       NameSpecified = true,
                                       Value = Math.Abs(flatRejectionFileRejectionAwbs.ChargeAmountWeightBilled -
                                               flatRejectionFileRejectionAwbs.ChargeAmountWeightAccepted)},

                    new ChargeAmount(){Name= ChargeAmountName.ValuationBilled,
                                       NameSpecified = true,
                                       Value = flatRejectionFileRejectionAwbs.ChargeAmountValuationBilled},
                    new ChargeAmount(){Name = ChargeAmountName.ValuationAccepted,
                                       NameSpecified = true,
                                       Value = flatRejectionFileRejectionAwbs.ChargeAmountValuationAccepted},
                    new ChargeAmount(){Name = ChargeAmountName.ValuationDifference,
                                       NameSpecified = true,
                                       Value = Math.Abs(flatRejectionFileRejectionAwbs.ChargeAmountValuationBilled -
                                               flatRejectionFileRejectionAwbs.ChargeAmountValuationAccepted)}
                };
                awbBreakdown.AddOnCharges = new AddOnCharges[]
                {
                    new AddOnCharges {AddOnChargeName = "ISCAllowed",
                                      AddOnChargeAmount = flatRejectionFileRejectionAwbs.AddOnChargesISCAllowed,
                                      AddOnChargePercentage = flatRejectionFileRejectionAwbs.AddOnChargesISCAllowedPercentage,
                                      AddOnChargePercentageSpecified = true
                    },
                    new AddOnCharges {AddOnChargeName = "ISCAccepted",
                                      AddOnChargeAmount = flatRejectionFileRejectionAwbs.AddOnChargesISCAccepted,
                                      AddOnChargePercentage = flatRejectionFileRejectionAwbs.AddOnChargesISCAcceptedPercentage,
                                      AddOnChargePercentageSpecified = true
                            },
                    new AddOnCharges {AddOnChargeName = "ISCDifference",
                                      AddOnChargeAmount = rejectionStage ==2 ?
                                      flatRejectionFileRejectionAwbs.AddOnChargesISCAccepted - flatRejectionFileRejectionAwbs.AddOnChargesISCAllowed:
                                      flatRejectionFileRejectionAwbs.AddOnChargesISCAllowed - flatRejectionFileRejectionAwbs.AddOnChargesISCAccepted},
                    
                    new AddOnCharges {AddOnChargeName = "OtherChargesAllowed",
                                      AddOnChargeAmount = flatRejectionFileRejectionAwbs.AddOnChargesOtherChargesAllowed},
                    new AddOnCharges {AddOnChargeName = "OtherChargesAccepted",
                                      AddOnChargeAmount = flatRejectionFileRejectionAwbs.AddOnChargesOtherChargesAccepted},
                    new AddOnCharges {AddOnChargeName = "OtherChargesDifference",
                                      AddOnChargeAmount = rejectionStage == 2 ?
                                      flatRejectionFileRejectionAwbs.AddOnChargesOtherChargesAccepted - flatRejectionFileRejectionAwbs.AddOnChargesOtherChargesAllowed:
                                      flatRejectionFileRejectionAwbs.AddOnChargesOtherChargesAllowed - flatRejectionFileRejectionAwbs.AddOnChargesOtherChargesAccepted}
                };
                awbBreakdown.TotalNetAmount = awbBreakdown.ChargeAmount[2].Value + awbBreakdown.ChargeAmount[5].Value +
                                              awbBreakdown.AddOnCharges[2].AddOnChargeAmount + awbBreakdown.AddOnCharges[5].AddOnChargeAmount;
                allAwbBreakdowns.Add(awbBreakdown);
                breakdownSerialnumber += 1;
            }

            return allAwbBreakdowns.ToArray();

        }
    }
}
