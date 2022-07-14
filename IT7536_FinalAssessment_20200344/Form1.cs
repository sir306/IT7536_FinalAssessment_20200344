using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

namespace IT7536_FinalAssessment_20200344
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// This Property is for getting the root file folder where all txt, excel and any other required docs will be located
        /// </summary>
        private string path = Path.GetDirectoryName(Application.ExecutablePath) + "\\files";
        /// <summary>
        /// The text file that holds all the required product types
        /// </summary>
        private string _productTypeFile = "\\productType.txt";
        /// <summary>
        /// Holds all the Product pallet data in the .csv file
        /// </summary>
        private string _productPalletFile = "\\productPallet.csv";
        /// <summary>
        /// This is the filename for storage racks that holds all storage rack data in a .csv file
        /// </summary>
        private string _storageRackFile = "\\storageRacks.csv";
        /// <summary>
        /// Holds all the slots in relation to the storage rack
        /// </summary>
        private string _storageRackSlotFile = "\\storageRackSlots.csv";
        /// <summary>
        /// This file holds the complete information about a slot
        /// </summary>
        private string _slots = "\\slots.csv";
        /// <summary>
        /// A Filestream property used for creating, editing
        /// </summary>
        private FileStream? fileStream;
        /// <summary>
        /// A StreamReader property used for reading files
        /// </summary>
        private StreamReader? streamReader;
        /// <summary>
        /// A TextFieldParser property for reading csv files
        /// </summary>
        private TextFieldParser? csvTextFieldParser;

        /// <summary>
        /// List for holding the product type objects
        /// </summary>
        private List<ProductType> productTypes = new();
        /// <summary>
        /// List for holding the product pallet objects
        /// </summary>
        private List<ProductPallet> productPallets = new();
        /// <summary>
        /// This is for holding non allocated product pallets
        /// </summary>
        private List<ProductPallet> nonAllocatedPallets = new();
        /// <summary>
        /// This value is used for holding the new pallet ID and ensures new pallets are created with unique IDs
        /// by default it is set to 1 in case no pallets have been created
        /// </summary>
        private int newPalletID = 1;
        /// <summary>
        /// This list holds all the storage racks that have been loaded from the storageRack file or been since created
        /// </summary>
        private List<StorageRack> storageRacks = new();

        /// <summary>
        /// The minimum height for a pallet
        /// </summary>
        private double _minHeight = 0.0;

        // this value comes from pallet height of 140mm and 200mm clearance above pallet as rack height is in cm the height is 34
        private const double _minClearance = 34;

        /// <summary>
        /// Declare custom event handler
        /// </summary>
        public EventHandler? OpenPdfHandle;


        public Form1()
        {
            InitializeComponent();

            CreateFiles();
            ReadFiles();
            LoadProductTypes();
            // clear selected values
            nonAllocatedPalletsDataGridView.ClearSelection();
            allocatedPalletsDataGridView.ClearSelection();
            currentStorageRackDataGridView.ClearSelection();
            // set forms min rack height on the create new storage rack tab
            newRackHeightNumericUpDown.Minimum = new decimal(_rackMinHeight);
        }

        /// <summary>
        /// Create files for the program if they do not already exist
        /// </summary>
        private void CreateFiles()
        {
            // check if file folder exists create if not
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // check Product Type File exists if not create 
            if (!File.Exists(path + _productTypeFile))
            {
                fileStream = File.Create(path + _productTypeFile);
                fileStream.Close();
            }
            // check Product Pallet File Exists if not Create
            if (!File.Exists(path + _productPalletFile))
            {
                fileStream = File.Create(path + _productPalletFile);
                fileStream.Close();
            }
            // check if Storage Racks File Exist if not Create
            if (!File.Exists(path + _storageRackFile))
            {
                fileStream = File.Create(path + _storageRackFile);
                fileStream.Close();
            }
            // check if storage rack slot file exists
            if (!File.Exists(path + _storageRackSlotFile))
            {
                fileStream = File.Create(path + _storageRackSlotFile);
                fileStream.Close();
            }
            // check if slots file exists
            if (!File.Exists(path + _slots))
            {
                fileStream = File.Create(path + _slots);
                fileStream.Close();
            }
        }

        /// <summary>
        /// Call all the readfiles for application
        /// </summary>
        private void ReadFiles()
        {
            ReadProductType(path+_productTypeFile);
            ReadProductPallet(path+_productPalletFile);
            if (productPallets.Count > 0) CreatePalletDataGrid(allocatedPalletsDataGridView, productPallets);
            if (nonAllocatedPallets.Count > 0) CreatePalletDataGrid(nonAllocatedPalletsDataGridView, nonAllocatedPallets);
            ReadStorageRackFile(path+_storageRackFile);
            if (storageRacks.Count > 0) CreateStorageRackDataGrid();
        }


        /// <summary>
        /// This function deletes the currently selected rack provided it exists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            string msg = "";
            string caption = "";
            // TODO
            if (currentStorageRackDataGridView.SelectedCells.Count > 0)
            {
                // select the whole row if not already
                int index = currentStorageRackDataGridView.SelectedCells[0].RowIndex;
                if (index != -1)
                {
                    currentStorageRackDataGridView.Rows[index].Selected = true;
                }

                // check if any pallets allocated to the rack
                int storageRackId = (int)currentStorageRackDataGridView.Rows[index].Cells[0].Value;

                // the currently selected rack in memory
                StorageRack selectedRack = storageRacks[index];
                // null check rack.allocatedSlots = if null can procceed without any further checks as no slots = no pallets in storage
                if (selectedRack.AllocatedSlots != null)
                {
                    foreach (var item in selectedRack.AllocatedSlots)
                    {
                        List<ProductPallet>? slotsPallets = GetAllocatedSlotPallets(storageRackId, item.Id);
                        // pallets are in this rack stop and alert the user to move pallets
                        if (slotsPallets != null && slotsPallets.Count > 0)
                        {
                            msg = "There are Pallets that have been allocated to this rack, you need to move them or delete them to delete this Rack";
                            caption = "ALERT";
                            MessageBox.Show(msg, caption);
                            return;
                        }
                    }
                }
                // warn user they are going to delete a storage rack
                DialogResult result;
                msg = "You are about to delete a Storage Rack are you sure you want to do that?";
                caption = "WARNING";
                result = MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                // if no then don't continue
                if (result == DialogResult.No)
                {
                    return;
                }
                DeleteStorageRack(selectedRack);
            }
            else
            {
                msg = "You haven't selected a Storage Rack or none exist?";
                caption = "Error";
                MessageBox.Show(msg, caption);
            }
        }

        /// <summary>
        /// Calls the custom event that opens a pdf to explain how to create a new rack
        /// </summary>
        /// <param name="sender">the object calling this</param>
        /// <param name="e">the event triggering this</param>
        private void newStorageRackHelpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // call custom event handler
            OpenPdf(EventArgs.Empty);
        }


        /// <summary>
        /// Custom event execution
        /// </summary>
        /// <param name="e">Event arg calling function</param>
        protected virtual void OpenPdf(EventArgs e)
        {
            // bind event handler
            OpenPdfHandle?.Invoke(this, e);

            try
            {
                string? helpPdf = "Help.Pdf";
                Process.Start(new ProcessStartInfo { FileName = helpPdf, UseShellExecute = true }); // .net 6 uses start process like this!
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        /// <summary>
        /// opens a browser to my website 
        /// </summary>
        /// <param name="sender">the object calling this</param>
        /// <param name="e">the event triggering this</param>
        private void viewDevelopersSiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string? website = @"https://nickscoding.website";
                Process.Start(new ProcessStartInfo { FileName = website, UseShellExecute = true }); // .net 6 uses start process like this!
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// This is for opening a new file for the corresponding tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenExistingFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Warning before loading custom files make sure they match the expected format as they will fail to load otherwise", "WARNING");
            if(tabControl1.SelectedIndex == 0)
            {
                // tab is the product type
                if(openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog1.FileName;
                    ReadProductType(filePath);
                }
            }
            if (tabControl1.SelectedIndex == 1)
            {
                // tab is the product pallet
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog1.FileName;
                    ReadProductPallet(filePath);
                }
            }
            if (tabControl1.SelectedIndex == 2)
            {
                // tab is the storage rack
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog1.FileName;
                    ReadStorageRackFile(filePath);
                }
            }
        }

        /// <summary>
        /// The save back up file tool strip allows a user to back up a copy of the file that belongs to the currently opened tab
        /// </summary>
        /// <param name="sender">the object that called the method</param>
        /// <param name="e">the event that fired the method</param>
        private void SaveBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // give the warning to user on how this save works
            MessageBox.Show("Warning this is to save a backup of the currently selected tab i.e. if you are in the Storage Rack tab you will back up the storage rack csv file", "WARNING");
            
            // work out which tab is open
            if (tabControl1.SelectedIndex == 0) // product type
            {
                saveFileDialog1.Filter = "Text file (*.txt)|*.txt";
                saveFileDialog1.FileName = "productType";
                // tab is the product type
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var filePath = saveFileDialog1.FileName;
                    if(filePath != "")
                    {
                        SaveFileToNewFileLocation(filePath, path + _productTypeFile);
                    }
                }
            }
            if (tabControl1.SelectedIndex == 1)// product pallet
            {
                saveFileDialog1.Filter = "CSV file (.csv)|.csv";
                saveFileDialog1.FileName = "productPallet";
                // tab is the product pallet
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var filePath = saveFileDialog1.FileName;
                    if (filePath != "")
                    {
                        SaveFileToNewFileLocation(filePath, path + _productPalletFile);
                    }
                }
            }
            if (tabControl1.SelectedIndex == 2)// storage rack
            {
                saveFileDialog1.Filter = "CSV file (.csv)|.csv";
                saveFileDialog1.FileName = "storageRack";
                // tab is the storage rack
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var filePath = saveFileDialog1.FileName;
                    if (filePath != "")
                    {
                        SaveFileToNewFileLocation(filePath, path + _storageRackFile);
                    }
                }
            }
        }

        /// <summary>
        /// This is for backing up files in particular the storage rack, the product pallets, and the product types
        /// </summary>
        /// <param name="newFilePath">The destination of the new file and/or new filename</param>
        /// <param name="currentFilePath">The current file path of the file selected</param>
        /// <exception cref="Exception">Fires if creating new file path fails</exception>
        private void SaveFileToNewFileLocation(string newFilePath, string currentFilePath)
        {
            // create new file
            fileStream = File.Create(newFilePath);
            fileStream.Close();
            string[] lines = File.ReadAllLines(currentFilePath);

            if (fileStream == null)
            {
                throw new Exception("An error occured with stream reader");
            }
            int i = 0;
            foreach (string item in lines)
            {
                if(i == 0)
                {
                    File.AppendAllText((newFilePath), item);
                }
                else
                {
                    string contents = Environment.NewLine + item;
                    File.AppendAllText((newFilePath), contents);
                }
                i++;
            }
            
        }
    }
}