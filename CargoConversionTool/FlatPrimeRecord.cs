using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CsvParser;

namespace CargoConversionTool
{    
    /// <summary>
    /// This class represents a line of a prime csv file. 
    /// The fields must be in the same order as here. 
    /// </summary>
    [IsCsvRecord]
    public class FlatPrimeRecord
    {
        private decimal _exchangeRate;
        private string _chargeCode;
        private decimal _detailsISCPercentage;
        private decimal _detailsISCAllowed;
        private string _buyerOrganizationID;
        private string _aWBIssuingAirline;

        [MandatoryField]
        [MaxLength(10)]
        public  string InvoiceNumber { get; set; }

        [MandatoryField]
        public DateTime InvoiceDate { get; set; }

        [MandatoryField]
        public string BuyerOrganizationID {
            get { return _buyerOrganizationID; }
            set {_buyerOrganizationID = value.PadLeft(3, '0'); }
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
            set {
            if (value != Constants.CollectChargeCode && value != Constants.PrepaidChargeCode)
                throw new Exception (string.Format("ChargeCode must either be {0} or {1}.", Constants.CollectChargeCode,Constants.PrepaidChargeCode));
            _chargeCode = value;
            }
        }
 
        public decimal DetailsWeightChargeAmount { get; set; }

        public decimal DetailsValuationChargeAmount { get; set; }        
        
        public decimal DetailsAmountSubjectToISCAllowed { get; set; }

        public decimal DetailsISCPercentage
        {
            get { return _detailsISCPercentage; }
            set
            {
                _detailsISCPercentage = Math.Round(value, 2);
            }
        }

        public decimal DetailsISCAllowed {
            get {return _detailsISCAllowed; }
            set
            {
                if (CalculateISC(DetailsISCPercentage, DetailsAmountSubjectToISCAllowed, ChargeCode)
                    != Math.Round(Convert.ToDecimal(value), 3, MidpointRounding.AwayFromZero))
                {
                    throw new Exception("Wrong DetailsISCAllowed. Expected: PP: -(AmountSubjectToISC * CommissionRate)/100, CC: (AmountSubjectToISC * CommissionRate)/100.");
                }
                else
                {
                    _detailsISCAllowed = Math.Round((Convert.ToDecimal(value)), 3);
                }
            } 
        }

        public decimal DetailsOtherChargesAllowed { get; set; }

        [MandatoryField]
        public DateTime AWBDate { get; set; }

        [MandatoryField]
        public string AWBIssuingAirline
        {
            get { return _aWBIssuingAirline; }
            set { _aWBIssuingAirline = value.PadLeft(3, '0'); }
        }

        [MandatoryField]
        public int AWBSerialNumber { get; set; }

        [MandatoryField]
        public int AWBCheckDigit { get; set; }

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
        public string CurrAdjustmentIndicator { get; set; }

        /// <summary>
        /// Calculates the ISC amount based on percentage and amountSubjectto ISC
        /// Returns a negative amount in case of PP, otherwise a positive amount
        /// </summary>
        /// <param name="iscPercentage"></param>
        /// <param name="amountSubjectToISC"></param>
        /// <param name="isPrePaid"></param>
        /// <returns></returns>
        private decimal CalculateISC(decimal iscPercentage, decimal amountSubjectToISC, string chargeCode)
        {
            decimal iscAmount = Math.Round((amountSubjectToISC * iscPercentage / 100), 3, MidpointRounding.AwayFromZero);
            if (chargeCode == Constants.PrepaidChargeCode)
                return -iscAmount;
            return iscAmount;

        }
    }
}
