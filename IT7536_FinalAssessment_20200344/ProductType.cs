using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT7536_FinalAssessment_20200344
{
    /// <summary>
    /// ProductType is used to help identify what product types are in ProductPallets, StorageRacks, StorageSlots
    /// </summary>
    internal class ProductType
    {
        /// <summary>
        /// private product type name property it holds a default string in the event one is not set
        /// </summary>
        private string _productName = "No Product Type has been set";
        /// <summary>
        /// Id property of ProductType Class, used for identifying the product type
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// public property getter and setter for product name
        /// </summary>
        public string ProductName { get => _productName; private set => _productName = value; }

        /// <summary>
        /// Private constructor for ProductType
        /// </summary>
        /// <param name="id">This is an int value, used for identifying the product type</param>
        /// <param name="productName">This is a string value used for telling users what product type this is</param>
        private ProductType(int id, string productName)
        {
            Id = id;
            ProductName = productName;
        }

        /// <summary>
        /// This is the public function used for creating Product type objects
        /// </summary>
        /// <param name="id">This is an int value, used for identifying the product type</param>
        /// <param name="productName">This is a string value used for telling users what product type this is</param>
        /// <returns>Returns an error if params dont match the requirements or returns a New ProductType</returns>
        /// <exception cref="Exception">If id is 0 or less, product name is null or empty, an error will be thrown</exception>
        public static ProductType CreateNewProductType(int id, string productName)
        {
            if(!(id > 0) || (productName == null || productName ==""))
            {
                throw new Exception("The id cannot be 0 or less and/or productName cannot be empty or null");
            }
            else
            {
                return new ProductType(id, productName);
            }
        }
    }
}
