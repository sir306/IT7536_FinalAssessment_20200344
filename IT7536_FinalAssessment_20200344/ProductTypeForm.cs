using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT7536_FinalAssessment_20200344
{
    /// <summary>
    /// This partial class is for the ProductType of the form its either related to that partical tab or the methodology is mainly for Product Types
    /// </summary>
    public partial class Form1 : Form
    {

        /// <summary>
        /// Read productTypeFile for the program and create objects for there respective classes and append it to the form where needed
        /// </summary>
        /// <exception cref="Exception">If the file fails to read then throw error as the files should be created before being read</exception>
        private void ReadProductType()
        {
            streamReader = File.OpenText(path + _productTypeFile);
            if (streamReader == null)
            {
                throw new Exception("An error occured with stream reader");
            }

            var num = 1;
            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line == "") continue;// checks to see if line is empty then bad save or entry so skip
                var product = ProductType.CreateNewProductType(num, line); // create object from line
                productTypes.Add(product); // append to product type list
                currentProdTypeListBox.Items.Add(line);// add to listbox for product list
                num++;
            }
            streamReader.Close();
        }


        /// <summary>
        /// This event function is fired when the Add button on the ProductTypesTab is clicked and will create a new product type and 
        /// add it to the list box for the user
        /// </summary>
        /// <param name="sender">The object that fired the event, in this case the button</param>
        /// <param name="e">The event that triggered this event</param>
        private void addProdTypeBtn_Click(object sender, EventArgs e)
        {
            createProductTypeTextBox.Text.TrimStart();
            createProductTypeTextBox.Text.TrimEnd();
            if (!(createProductTypeTextBox.Text.Length <= 0))
            {
                CreateProductType(createProductTypeTextBox.Text);
                currentProdTypeListBox.Items.Add(createProductTypeTextBox.Text);
                createProductTypeTextBox.Text = "";
                LoadProductTypes();// reload product types
            }
        }

        /// <summary>
        /// Creates a new ProductType that is added to the file, the ProductTypeListBox and the ProductType List.
        /// By calling CreateNewProductType, AddToProductTypeList and SaveToProductTypeFile
        /// </summary>
        /// <param name="productType">The string value for the product type name</param>
        private void CreateProductType(string productType)
        {
            if (productTypes == null)
            {
                var product = ProductType.CreateNewProductType(1, productType);
                AddToProductTypeList(product);
            }
            else
            {
                int num = productTypes.Count();
                num += 1;
                var product = ProductType.CreateNewProductType(num, productType);
                AddToProductTypeList(product);
                SaveToProductTypeFile(product);
            }

        }
        /// <summary>
        /// Adds a ProductType to the end of the list of productTypes
        /// </summary>
        /// <param name="productType">The ProductType object being added to the list</param>
        private void AddToProductTypeList(ProductType productType)
        {
            productTypes.Add(productType);
        }
        /// <summary>
        /// Save the productType to the productTypeFile and place it on a new line
        /// </summary>
        /// <param name="productType">The new product type being added to the end of the file</param>
        private void SaveToProductTypeFile(ProductType productType)
        {
            string contents;
            var lines = File.ReadAllLines(path + _productTypeFile);
            if (lines.Length == 0)
            {
                contents = productType.ProductName;
                File.AppendAllText((path + _productTypeFile), contents);

            }
            else
            {
                contents = Environment.NewLine + productType.ProductName;
                File.AppendAllText((path + _productTypeFile), contents);
            }
        }

        /// <summary>
        /// This function is for clearing the createProductTypeTextBox, it is fired by pressing the cancel button
        /// </summary>
        /// <param name="sender">The object that fired the event</param>
        /// <param name="e">The event that triggered the event</param>
        private void cancelProdTypeBtn_Click(object sender, EventArgs e)
        {
            createProductTypeTextBox.Text = "";
        }

        /// <summary>
        /// This function is used for removing the selected product type from the ProductTypeFile, 
        /// the productTypes list and the currentProductTypesListBox, if no item is selected nothing will happen
        /// </summary>
        /// <param name="sender">The object that fired the event</param>
        /// <param name="e">The event that triggered the event</param>
        private void removeCurrentProdBtn_Click(object sender, EventArgs e)
        {
            int index = currentProdTypeListBox.SelectedIndex;

            if (index == -1) return; // if selected index is -1 then no item is selected and can't be removed

            RemoveSelectedProductType(index);

        }

        /// <summary>
        /// This function will create a temp file and write all the lines from the current _productTypeFile until the selected index
        /// matches the line that is being sought and skip over this line essentially removing it from the file. It creates a temp file 
        /// to save on memory and not allocating an extra array instead, it writes to a temporary file to prevent any data loss from 
        /// the original incase of a crash.
        /// </summary>
        /// <param name="index">The specified index of the productType to be deleted</param>
        /// <exception cref="Exception">If the selected index dooesn't match any in objects in memory or if the text file can't be opened</exception>
        private void RemoveSelectedProductType(int index)
        {
            // check to see the item exists in memory
            var selectedItem = productTypes[index];
            if (selectedItem == null)
            {
                throw new Exception("The ProductType selected could not be found in the list");
            }
            // remove from memory
            productTypes.RemoveAt(index);

            // open and read existing file throw error if cannot open
            streamReader = File.OpenText(path + _productTypeFile);
            if (streamReader == null)
            {
                throw new Exception("An error occured with stream reader");
            }
            // create temp variables and files that are needed
            var tempFile = Path.GetTempFileName();
            string? line = "";
            int currentLine = 0;
            StreamWriter streamWriter = new StreamWriter(tempFile);
            // loop through and find specified line write the lines to temp file that dont match the requested index
            while ((line = streamReader.ReadLine()) != null)
            {
                if (currentLine == index) { currentLine++; continue; }// this is the line being sought
                // this if check handles any instance where the requested removal index is the start of the file
                if (index == 0 && currentLine == 1)
                {
                    streamWriter.Write(line);
                    currentLine++;
                    continue;
                }
                // this is to add a new line to the file provided its not a the start or the search item
                if (currentLine != 0 && currentLine != index) streamWriter.Write(Environment.NewLine);
                streamWriter.Write(line); // write current line to temp
                currentLine++;
            }
            // close reader and writer
            streamReader.Close();
            streamWriter.Close();
            // delete old path
            File.Delete(path + _productTypeFile);
            // move temp file and name it to the previous file
            File.Move(tempFile, path + _productTypeFile);
            // all data manipulation is done now remove from the GUI
            currentProdTypeListBox.Items.RemoveAt(index);
            LoadProductTypes();// reload product types
        }

        /// <summary>
        /// This will Load All Product Types into the combo box on the new pallet tab under PalletProduct tab and create Storage Rack tab
        /// </summary>
        private void LoadProductTypes()
        {
            newPalletProdTypecomboBox.Items.Clear();// clear existing data
            newStorageRackProductTypecomboBox.Items.Clear();
            newPalletProdTypecomboBox.Text = "";
            if (productTypes.Count == 0)
            {
                newPalletProdTypecomboBox.Text = "Enter A New Product Type..";
            }
            else
            {
                foreach (var item in productTypes)
                {
                    newPalletProdTypecomboBox.Items.Add(item.ProductName);
                    newStorageRackProductTypecomboBox.Items.Add(item.ProductName);
                }
                newPalletProdTypecomboBox.Text = "Select the Pallets Product Type..";
            }
        }

    }
}
