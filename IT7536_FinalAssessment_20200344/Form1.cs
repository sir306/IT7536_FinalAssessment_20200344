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
        }

        /// <summary>
        /// Create files for the program if they do not already exist
        /// </summary>
        private void CreateFiles()
        {
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
            if(!File.Exists(path +_storageRackFile))
            {
                fileStream = File.Create(path +_storageRackFile);
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
        }
    }
}