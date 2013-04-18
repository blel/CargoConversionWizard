using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;


namespace CargoConversionTool
{
    /// <summary>
    /// This class allows to extract from a List of FlatPrimeRecords invoices and lineitems 
    /// </summary>
    public class FlatPrimeFile
    {
        private List<FlatPrimeRecord> _flatPrimeFile;
        
        #region Constructor
        public FlatPrimeFile(List<FlatPrimeRecord> flatPrimeFile)
        {
            _flatPrimeFile = flatPrimeFile;
        }
        #endregion

        /// <summary>
        /// Returns the number of different invoice numbers in the FlatCargoFile
        /// </summary>
        /// <returns></returns>
        public int GetInvoiceCount()
        {
            return
                (from cc in _flatPrimeFile
                 select cc.InvoiceNumber.Distinct()).Count();
        }

        /// <summary>
        /// Returns a list of strings containing the invoice numbers
        /// </summary>
        /// <returns></returns>
        private List<string> GetInvoiceNumbers()
        {
            return
                (from cc in _flatPrimeFile
                 orderby cc.InvoiceNumber
                 select cc.InvoiceNumber).Distinct().ToList();
        }

        /// <summary>
        /// Returns an enumerable set of cargorecords per invoice number 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<List<FlatPrimeRecord>> GetInvoices()
        {
            foreach (string invoiceNumber in GetInvoiceNumbers())
            {
                yield return
                    (from cc in _flatPrimeFile
                     where (cc.InvoiceNumber == invoiceNumber)
                     select cc).ToList<FlatPrimeRecord>();
            }
        }

        /// <summary>
        /// Returns a list of strings containing the ChargeCodes
        /// </summary>
        /// <param name="invoiceRecords"></param>
        /// <returns></returns>
        private static List<string> GetChargeCodes(List<FlatPrimeRecord> invoiceRecords)
        {
            return
                (from cc in invoiceRecords
                 orderby cc.ChargeCode
                 select cc.ChargeCode).Distinct().ToList();

        }

        /// <summary>
        /// Returns a list of flat cargo record for every charge code
        /// </summary>
        /// <param name="invoiceRecords"></param>
        /// <returns></returns>
        public static IEnumerable<List<FlatPrimeRecord>> GetLineItems(List<FlatPrimeRecord> invoiceRecords)
        {
            foreach (string chargeCode in GetChargeCodes(invoiceRecords))
            {
                yield return
                    (from cc in invoiceRecords
                     where (cc.ChargeCode == chargeCode)
                     select cc).ToList<FlatPrimeRecord>();
            }
        }
    }
}
