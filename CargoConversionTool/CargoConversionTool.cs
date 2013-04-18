using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using Ionic.Zip;
using CsvParser;

namespace CargoConversionTool
{
    public partial class frmCargoConversionTool : Form
    {
        #region private fields
        private FlatPrimeFile _flatCargoFile = new FlatPrimeFile(new List<FlatPrimeRecord>());
        private FlatRejectionFile _flatRejectionFile = new FlatRejectionFile (new List<FlatRejectionRecord>());

        private bool _buttonPageChange = false;
        #endregion

        #region constructor
        public frmCargoConversionTool()
        {
            InitializeComponent();
            this.cmbIATAPeriod.DataSource = IATAPeriodHelper.GetIATAPeriods(true, 4, 4);
            this.cmbIATAPeriod.SelectedIndex = 4;
            chkHasHeaders.Checked = true;
            chkHasRejectionFileHeaders.Checked = true;
            chkValidateXSD.Checked = true;
            chkForSandbox.Checked = true;
            tabLoadFiles.SelectedIndex = 0;
            btnNext.Enabled = true;
            btnBack.Enabled = false;
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
        }
        #endregion
        
        /// <summary>
        /// Opens the File Dialog and displays the selected file in the txtFileName
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetFile_Click(object sender, EventArgs e)
        {
            txtFileName.Text = GetFileName(txtFileName.Text);
        }

        /// <summary>
        /// Loads the file into the FlatCargoFile private field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTryLoad_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtFileName.Text))
            {
                txtResults.Text = string.Empty;
                CsvParser<FlatPrimeRecord> csvParser = new CsvParser<FlatPrimeRecord>();
                csvParser.ValidationErrorOccurred += new CsvParser<FlatPrimeRecord>.ValidationErrorOccuredHandler(csvParser_ValidationErrorOccurred);

                try
                {
                    txtResults.Text += "Start reading file...\r\n";
                    _flatCargoFile = new FlatPrimeFile(csvParser.ReadFromFile(txtFileName.Text, ";", chkHasHeaders.Checked));
                    txtResults.Text += "\r\nEnd reading file.\r\n";
                    txtResults.Text += string.Format("Total {0} of {1} lines imported.", csvParser.ImportedLineCount, csvParser.TotalLineCount);

                }
                catch (IOException ex)
                {
                    txtResults.AppendText(ex.Message + "\r\n");
                }
                catch (Exception ex)
                {
                    txtResults.AppendText(ex.Message + "\r\n");
                }

                
            }
        }

        /// <summary>
        /// Gets the csv-filename through openfiledialog
        /// </summary>
        /// <param name="defaultFileName"></param>
        /// <returns></returns>
        private string GetFileName(string defaultFileName)
        {
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.FileName = defaultFileName;
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Loads teh rejection file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTryLoadRejectionFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtRejectionFile.Text))
            {
                txtRejectionFileLoadResults.Text = "";
                CsvParser<FlatRejectionRecord> csvParser = new CsvParser<FlatRejectionRecord>();
                csvParser.ValidationErrorOccurred += new CsvParser<FlatRejectionRecord>.ValidationErrorOccuredHandler(csvParser_RejectionErrorOccurred);
                try
                {
                    txtRejectionFileLoadResults.Text += "Start reading file...\r\n";
                    _flatRejectionFile = new FlatRejectionFile(csvParser.ReadFromFile(txtRejectionFile.Text, ";", chkHasHeaders.Checked));
                    txtRejectionFileLoadResults.Text += "\r\nEnd reading file.";
                    txtRejectionFileLoadResults.Text += string.Format("Total {0} of {1} lines imported.", csvParser.ImportedLineCount, csvParser.TotalLineCount);
                    if (_flatRejectionFile.GetInvoiceCount() > 0 || _flatCargoFile.GetInvoiceCount() > 0)
                        this.btnNext.Enabled = true;
                    else
                        this.btnNext.Enabled = false;
                }
                catch (IOException ex)
                {
                    txtRejectionFileLoadResults.AppendText(ex.Message + "\r\n");
                }
                catch (Exception ex)
                {
                    txtRejectionFileLoadResults.AppendText(ex.Message + "\r\n");
                }
            }
        }

        /// <summary>
        /// Creates the Xml File based on the _flatCargoFile field.
        /// If chkValidateXSD is true, file is loaded and validated against the XSD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateXML_Click(object sender, EventArgs e)
        {

            txtXMLResults.Text = string.Empty;
            if (_flatCargoFile != null)
            {
                CargoXmlCreator xmlCreator = new CargoXmlCreator();
                string xmlFileName = xmlCreator.GetXmlFileName(chkForSandbox.Checked, cmbIATAPeriod.Text);

                string xmlFilePath = "C:\\temp\\";

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(InvoiceTransmission));
               
                try
                {

                    //Get the filename
                    saveFileDialog1.FileName = xmlFileName;
                    DialogResult result = this.saveFileDialog1.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        xmlFilePath = Path.GetDirectoryName(saveFileDialog1.FileName) + "\\";


                        //modify encoding to ascii
                        System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
                        settings.Encoding = Encoding.ASCII;

                        //serialize the xml
                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                        using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(xmlFilePath + xmlFileName + ".xml", settings))
                        {
                            xmlSerializer.Serialize(xmlWriter, xmlCreator.GetInvoiceTransmission(_flatCargoFile, _flatRejectionFile , cmbIATAPeriod.Text));
                        }
                        txtXMLResults.Text = string.Format("XML {0} created (check Folder {1}.)\r\n", xmlFileName, xmlFilePath);
                        using (ZipFile zip = new ZipFile())
                        {
                            zip.AddFile(xmlFilePath + xmlFileName + ".xml", "");
                            zip.Save(xmlFilePath + xmlFileName + ".zip");
                        }
                        txtXMLResults.AppendText(string.Format("Zip-File created.\r\n", xmlFileName, xmlFilePath));

                        //validate xml
                        if (chkValidateXSD.Checked)
                        {
                            txtXMLResults.AppendText("Now validating.....\r\n");
                            ValidateXML(xmlFilePath, xmlFileName);
                            txtXMLResults.AppendText("Validation done. See above for any validation problems.");
                        }
                        btnNext.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    txtXMLResults.Text = ex.Message;
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default; 
                }
            }
            else
            {
                txtXMLResults.Text = "Please load a flat file first.";
            }
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default; 
        }

        /// <summary>
        /// Validates the xml File against the XSD
        /// Any validation erros are caught by the ValidationCallBack
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="xmlFileName"></param>
        private void ValidateXML(string xmlFilePath, string xmlFileName)
        {
            System.Xml.XmlReaderSettings readerSettings = new System.Xml.XmlReaderSettings();
            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.Schemas.Add("http://www.IATA.com/IATAAviationInvoiceStandard",
                "http://www.iata.org/whatwedo/finance/clearing/sis/Documents/schemas/IATA_IS_XML_Invoice_Standard_V3.2.xsd");
            readerSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            readerSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            readerSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            readerSettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            // Create the XmlReader object. 
            XmlReader reader = XmlReader.Create(xmlFilePath + xmlFileName + ".xml", readerSettings);
            // Parse the file.  
            while (reader.Read());
        }

        /// <summary>
        /// The ValidationCallBack is executed in case an xsd validation error occurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private  void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
            {
                txtXMLResults.AppendText("\r\nWarning: Matching schema not found.  No validation occurred." + args.Message);
            }
            else
            {
                txtXMLResults.AppendText("\r\nValidation error: " + args.Message);
            }
        }

        /// <summary>
        /// Callback to display error messages
        /// </summary>
        /// <param name="line"></param>
        /// <param name="col"></param>
        /// <param name="message"></param>
        void csvParser_ValidationErrorOccurred(int line, int col, string message)
        {
            this.txtResults.AppendText(string.Format("\r\nError at line {0}, column {1}: {2}", line, col, message));
        }

        /// <summary>
        /// Callback to display error messages
        /// </summary>
        /// <param name="line"></param>
        /// <param name="col"></param>
        /// <param name="message"></param>
        void csvParser_RejectionErrorOccurred(int line, int col, string message)
        {
            this.txtRejectionFileLoadResults.AppendText(string.Format("\r\nError at line {0}, column {1}: {2}", line, col, message));
        }

        /// <summary>
        /// Switches to the next tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            _buttonPageChange = true;
            if (tabLoadFiles.SelectedIndex < tabLoadFiles.TabPages.Count-1)
            {
                tabLoadFiles.SelectedIndex += 1;
                btnBack.Enabled = true;
                if (tabLoadFiles.SelectedIndex == 1 && _flatCargoFile.GetInvoiceCount() == 0 && _flatRejectionFile.GetInvoiceCount() == 0)
                {
                    btnNext.Enabled = false;
                }

                if (tabLoadFiles.SelectedIndex == tabLoadFiles.TabPages.Count - 1)
                {
                    btnNext.Text = "Finish";
                    btnNext.Enabled = false;
                    txtXMLResults.Text = "";
                }
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Switches to the previous tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, EventArgs e)
        {
            _buttonPageChange = true;
            if (tabLoadFiles.SelectedIndex > 0)
            {
                tabLoadFiles.SelectedIndex -= 1;
                btnNext.Text = "Next";
                btnNext.Enabled = true;
            }
            if (tabLoadFiles.SelectedIndex == 0)
            {
                btnBack.Enabled = false;
                if (_flatCargoFile.GetInvoiceCount() > 0)
                {
                    btnNext.Enabled = true;
                }
            }
        }

        //The following events are implemented to avoid tab change by mouse click
        private void tabLoadFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
                tabLoadFiles.TabPages[tabLoadFiles.SelectedIndex].Focus();  
        }

        private void tabPage1_Validating(object sender, CancelEventArgs e)
        {
            if (!_buttonPageChange)
                e.Cancel = true;
            _buttonPageChange = false;
        }

        private void tabPage2_Validating(object sender, CancelEventArgs e)
        {
            if (!_buttonPageChange)
                e.Cancel = true;
            _buttonPageChange = false;
        }

        private void tabPage3_Validating(object sender, CancelEventArgs e)
        {
            if (!_buttonPageChange)
                e.Cancel = true;
            _buttonPageChange = false;
        }

        private void tabPage4_Validating(object sender, CancelEventArgs e)
        {
            if (!_buttonPageChange)
                e.Cancel = true;
            _buttonPageChange = false;
        }

        private void frmCargoConversionTool_Activated(object sender, EventArgs e)
        {
            txtFileName.Focus();
        }

        private void frmCargoConversionTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        private void btnOpenRejectionFile_Click(object sender, EventArgs e)
        {
            txtRejectionFile.Text = GetFileName(txtRejectionFile.Text);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
