using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT7536_FinalAssessment_20200344
{
    /// <summary>
    /// A Product pallet is a class designed for the pallets being put into storage slots
    /// </summary>
    internal class ProductPallet
    {
        /// <summary>
        /// Objects propertys see constructor for more definition of them
        /// </summary>
        public int Id { get; private set; }
        private string _storageLocation = "No Location Set";
        private double _palletHeight;
        private string _productType = "No Product Type Set";

        /// <summary>
        /// Private Constructor for a Product Pallet object
        /// </summary>
        /// <param name="id">This is an integer value that must be greater than 0 and is used to identify the pallet</param>
        /// <param name="storageLocation">This is a string for storing current pallet position i.e. storage rack 1, storage slot 2</param>
        /// <param name="palletHeight">This is a double and must be greater than 0, is used for calculating where it should be placed</param>
        /// <param name="productType">This is a string that holds the type of product on the pallet, for this assignment
        /// only one product type will be permitted</param>
        private ProductPallet(int id, string storageLocation, double palletHeight, string productType)
        {
            Id = id;
            StorageLocation = storageLocation;
            PalletHeight = palletHeight;
            ProductType = productType;
        }

        /// <summary>
        /// Public method for ProductPallet used in creating a product pallet object
        /// </summary>
        /// <param name="id">This is an integer value that must be greater than 0 and is used to identify the pallet</param>
        /// <param name="storageLocation">This is a string for storing current pallet position i.e. storage rack 1, storage slot 2</param>
        /// <param name="palletHeight">This is a double and must be greater than 0, is used for calculating where it should be placed</param>
        /// <param name="productType">This is a string that holds the type of product on the pallet, for this assignment
        /// only one product type will be permitted</param>
        /// <returns>An error if inputs are invalid or a new pallet object</returns>
        /// <exception cref="Exception">If id or palletHeight is not greater than 0 an exception will be thrown</exception>
        public static ProductPallet CreateProductPallet(int id, string storageLocation, double palletHeight, string productType)
        {
            if (!(id > 0 || palletHeight > 0))
            {
                throw new Exception("These values are not valid inputs");
            }
            else
            {
                return new ProductPallet(id, storageLocation, palletHeight, productType);
            }
        }
        /// <summary>
        /// Public getters and setters for private ProductType
        /// </summary>
        public string ProductType
        {
            get { return _productType; }
            set { _productType = value; }
        }

        /// <summary>
        /// Public getters and setters for private PalletHeight
        /// </summary>
        public double PalletHeight
        {
            get { return _palletHeight; }
            set { _palletHeight = value; }
        }

        /// <summary>
        /// Public getters and setters for private StorageLocation
        /// </summary>
        public string StorageLocation
        {
            get { return _storageLocation; }
            set { _storageLocation = value; }
        }


    }
}
