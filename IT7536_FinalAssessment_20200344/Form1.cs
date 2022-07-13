using Microsoft.VisualBasic.FileIO;

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
            ReadProductType();
            ReadProductPallet();
            if (productPallets.Count > 0) CreatePalletDataGrid(allocatedPalletsDataGridView, productPallets);
            if (nonAllocatedPallets.Count > 0) CreatePalletDataGrid(nonAllocatedPalletsDataGridView, nonAllocatedPallets);
            ReadStorageRackFile();
            if (storageRacks.Count > 0) CreateStorageRackDataGrid();
        }



        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // TODO
        }

        private void allocatePalletToSlotButton_Click(object sender, EventArgs e)
        {
            // TODO
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            // TODO
            if (palletBeingAllocatedDataGridView.SelectedCells.Count > 0)
            {
                palletBeingAllocatedDataGridView.SelectAll();
                int id = (int)palletBeingAllocatedDataGridView.SelectedCells[0].Value;
                string storageLocation = (string)palletBeingAllocatedDataGridView.SelectedCells[1].Value;
                double palletHeight = (double)palletBeingAllocatedDataGridView.SelectedCells[2].Value;
                string productType = (string)palletBeingAllocatedDataGridView.SelectedCells[3].Value;
                ProductPallet pallet = ProductPallet.CreateProductPallet(id, storageLocation, palletHeight, productType);
                bool isCheckboxChecked = allocatePalletAnyRackCheckBox.Checked; // false means no check and isn't locked to product type
                // this value comes from pallet height of 140mm and 200mm clearance above pallet as rack height is in cm the height is 34
                double minClearance = 34;
                List<StorageSlot>? storageSlots = new();


                if (isCheckboxChecked)
                {
                    foreach (StorageRack storage in storageRacks)
                    {
                        if (storage.AllocatedSlots == null) continue; // no allocated slots so continue
                        if (storage.AllocatedProductType != productType) continue; // product types don't match

                        if (storage.Full != false) continue; // this rack is full

                        // check rack height to see if smaller than pallet minus min clearance
                        if (storage.RackHeight - minClearance <= pallet.PalletHeight) continue;

                        
                        foreach (var item in storage.AllocatedSlots)
                        {
                            if(item == null) continue; // shouldn't be null so skip


                            if(item.Full == true) continue; // if full then not useable
                            List<ProductPallet>? allocatedPallets = GetAllocatedSlotPallets(storage.Id, item.Id);

                            if(allocatedPallets != null && allocatedPallets.Count > 0)
                            {
                                double palletHeightStack = 0;
                                foreach (var allocatedPallet in allocatedPallets)
                                {
                                    palletHeightStack += allocatedPallet.PalletHeight;
                                }
                                if ((pallet.PalletHeight + palletHeightStack + minClearance) > storage.RackHeight) continue; // current stack with new pallet too high
                                else
                                {
                                    // slot can handle this pallet aswell
                                    storageSlots.Add(item);
                                }
                            } 
                            // slot is empty and passed height check already so add slot
                            else
                            {
                                storageSlots.Add(item);
                            }

                        }

                    }
                }
                else
                {
                    foreach (StorageRack storage in storageRacks)
                    {
                        if (storage.AllocatedSlots == null) continue; // no allocated slots so continue

                        if (storage.Full != false) continue; // this rack is full

                        // check rack height to see if smaller than pallet minus min clearance
                        if (storage.RackHeight - minClearance <= pallet.PalletHeight) continue;


                        foreach (var item in storage.AllocatedSlots)
                        {
                            if (item == null) continue; // shouldn't be null so skip

                            if (item.Full == true) continue; // if full then not useable

                            List<ProductPallet>? allocatedPallets = GetAllocatedSlotPallets(storage.Id, item.Id);

                            // this checks the pallets currently in this slot and checks height
                            if (allocatedPallets != null && allocatedPallets.Count > 0)
                            {
                                double palletHeightStack = 0;
                                foreach (var allocatedPallet in allocatedPallets)
                                {
                                    palletHeightStack += allocatedPallet.PalletHeight;
                                }
                                if ((pallet.PalletHeight + palletHeightStack + minClearance) > storage.RackHeight) continue; // current stack with new pallet too high
                                else
                                {
                                    // slot can handle this pallet aswell
                                    storageSlots.Add(item);
                                }
                            }
                            // slot is empty and passed height check already so add slot
                            else
                            {
                                storageSlots.Add(item);
                            }

                        }

                    }
                }

                /// TODO CREATE DATA GRID AND ADD SLOTS
            }
        }
    }
}