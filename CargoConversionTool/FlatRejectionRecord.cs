using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvParser;

namespace CargoConversionTool
{
    /// <summary>
    /// This class represents a line of a rejection csv file. 
    /// The fields must be in the same order as here. 
    /// </summary>
    [CsvParser.IsCsvRecord]
    public class FlatRejectionRecord 
    {
        private decimal _exchangeRate;
        private string _chargeCode;
        private string _yourInvoiceBillingDate;
        private string _yourRejectionMemoNumber;
        private string _buyerOrganizationID;
     
        [MandatoryField]
        [MaxLength(10)]
        public  string InvoiceNumber { get; set; }

        [MandatoryField]
        public DateTime InvoiceDate { get; set; }

        [MandatoryField]
        public string BuyerOrganizationID
        {
            get { return _buyerOrganizationID; }
            set { _buyerOrganizationID = value.PadLeft(3, '0'); }
        }

        public string CurrencyCode { get; set; }

        public decimal ExchangeRate
        {
            get { return _exchangeRate; }
            set
            {
                if (CurrencyCode != "USD")
                {
                    if (value == 0)
                        throw new Exception("Exchange rate is mandatory if the currency is different from USD.");
                    _exchangeRate = value;
                }
            }
        }

        public  string ChargeCode
        {
            get { return _chargeCode; }
            set
            {
                if (value != Constants.RejectionMemoChargeCode)
                    throw new Exception(string.Format("ChargeCode must be {0}.", Constants.RejectionMemoChargeCode));
                _chargeCode = value;
            }
        }

        [MandatoryField]
        public string RejectionMemoNumber { get; set; }

        [MandatoryField]
        public int RejectionStage {get; set; }

        [MandatoryField]
        public string ReasonCode { get; set; }

        [MandatoryField]
        public string YourInvoiceNumber { get; set; }


        public string YourInvoiceBillingDate { 
        get {return _yourInvoiceBillingDate;}
            set
            {
                if (string.IsNullOrWhiteSpace(value) && (RejectionStage == 2 || RejectionStage == 3))
                    throw new Exception("Your Invoice Billing Date is mandatory for Rejection Stage 2 and 3.");
                _yourInvoiceBillingDate = value;
            }
        }

        [MaxLength(11)]
        public string YourRejectionMemoNumber
        {
            get { return _yourRejectionMemoNumber; }
            set
            {
                if ((RejectionStage == 2 || RejectionStage == 3) && string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("Your Rejection Memo Number is mandatory for Rejection Stage 2 and 3.");
                }
                _yourRejectionMemoNumber = value;
            }
        }

        [MandatoryField]
        public DateTime AWBDate { get; set; }

        [MandatoryField]
        public string BillingCode { get; set; }

        [MandatoryField]
        public string  AWBIssuingAirline { get; set; }

        [MandatoryField]
        public int AWBSerialNumber { get; set; }

        [MandatoryField]
        public int AWBCheckDigit { get; set; }

        public decimal ChargeAmountWeightBilled { get; set; }

        public decimal ChargeAmountWeightAccepted { get; set; }

        public decimal ChargeAmountValuationBilled { get; set; }

        public decimal ChargeAmountValuationAccepted { get; set; }

        public decimal AddOnChargesISCAllowedPercentage { get; set; }

        public decimal AddOnChargesISCAllowed { get; set; }

        public decimal AddOnChargesISCAcceptedPercentage { get; set; }

        public decimal AddOnChargesISCAccepted { get; set; }

        public decimal AddOnChargesOtherChargesAllowed { get; set; }

        public decimal AddOnChargesOtherChargesAccepted { get; set; }

        [MandatoryField]
        public string OriginAirportCode { get; set; }

        [MandatoryField]
        public string DestinationAirportCode { get; set; }

        [MandatoryField]
        public string FromAirportCode { get; set; }

        [MandatoryField]
        public string ToAirportOrPointOfTransferCode { get; set; }

        [MandatoryField]
        public DateTime DateOfCarriageOrTransfer { get; set; }

        [MandatoryField]
        public string AttachmentIndicatorOriginal { get; set; }
    }
}
