using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace CargoConversionTool
{
    
    
    /// <summary>
    /// Represents a flat Cargo File. The object contains a List of FlatCargoRecords, each one representing a line in
    /// the RACAS output file.
    /// </summary>
    public class FlatRejectionFile 
    {
        private List<FlatRejectionRecord> _flatRejectionFile = new List<FlatRejectionRecord>();

        #region Constructor
        public FlatRejectionFile(List<FlatRejectionRecord> flatRejectionFile)
        {
            _flatRejectionFile = flatRejectionFile;
        }
        #endregion

        /// <summary>
        /// Returns the number of different invoice numbers in the FlatCargoFile
        /// </summary>
        /// <returns></returns>
        public int GetInvoiceCount()
        {
            return
                (from cc in _flatRejectionFile
                 select cc.InvoiceNumber.Distinct()).Count();
        }

        /// <summary>
        /// Returns a list of strings containing the invoice numbers
        /// </summary>
        /// <returns></returns>
        private List<string> GetInvoiceNumbers()
        {
            return
                (from cc in _flatRejectionFile
                 orderby cc.InvoiceNumber
                 select cc.InvoiceNumber).Distinct().ToList();
        }

        /// <summary>
        /// Returns an enumerable set of cargorecords per invoice number 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<List<FlatRejectionRecord>> GetInvoices()
        {
            foreach (string invoiceNumber in GetInvoiceNumbers())
            {
                yield return
                    (from cc in _flatRejectionFile
                     where (cc.InvoiceNumber == invoiceNumber)
                     select cc).ToList<FlatRejectionRecord>();
            }
        }

        /// <summary>
        /// Returns a list of strings containing the ChargeCodes
        /// </summary>
        /// <param name="invoiceRecords"></param>
        /// <returns></returns>
        private static List<string> GetChargeCodes(List<FlatRejectionRecord> invoiceRecords)
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
        public static IEnumerable<List<FlatRejectionRecord>> GetLineItems(List<FlatRejectionRecord> invoiceRecords)
        {
            foreach (string chargeCode in GetChargeCodes(invoiceRecords))
            {
                yield return
                    (from cc in invoiceRecords
                     where (cc.ChargeCode == chargeCode)
                     select cc).ToList<FlatRejectionRecord>();
            }
        }

        /// <summary>
        /// Returns List of FlatRejectionRecords for every Rejection Memo
        /// </summary>
        /// <param name="lineItems"></param>
        /// <returns></returns>
        public static IEnumerable<List<FlatRejectionRecord>> GetRejectionMemos(List<FlatRejectionRecord> lineItems)
        {
            foreach (string rejectionMemoNumber in GetRejectionMemoNumbers(lineItems))
            {
                yield return
                    (from cc in lineItems
                     where (cc.RejectionMemoNumber == rejectionMemoNumber)
                     select cc).ToList<FlatRejectionRecord>();
            }
        }

        /// <summary>
        /// Returns a list containing the rejection memo numbers based on the 
        /// imported csv lines
        /// </summary>
        /// <param name="lineItems"></param>
        /// <returns></returns>
        private static List<string> GetRejectionMemoNumbers(List<FlatRejectionRecord> lineItems)
        {
            return
                (from cc  in lineItems
                 orderby cc.RejectionMemoNumber
                 select cc.RejectionMemoNumber).Distinct().ToList<string>();
        }
      
    }
}
