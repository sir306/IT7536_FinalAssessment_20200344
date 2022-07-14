using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT7536_FinalAssessment_20200344
{
    /// <summary>
    /// This partial class is for methods involving the Pallets Tab or where the dominant functionality around the method is pallet related
    /// </summary>
    public partial class Form1 : Form
    {

        private const string _palletHeightInputDefault = "Enter the Pallet in Centimeters..";

        /// <summary>
        /// Read productPalletFile for the program and create objects for there respective classes and append it to the form where needed
        /// </summary>
        /// <exception cref="Exception">If the file fails to read then throw error as the files should be created before being read</exception>
        private void ReadProductPallet(string filepath)
        {
            // clear any old data previously loaded
            productPallets.Clear();
            nonAllocatedPallets.Clear();

            csvTextFieldParser = new TextFieldParser(filepath);
            if (csvTextFieldParser == null)
            {
                throw new Exception("An error occured with csvTextFieldParser");
            }
            csvTextFieldParser.SetDelimiters(new string[] { "," });
            while (!csvTextFieldParser.EndOfData)
            {
                string[]? values = csvTextFieldParser.ReadFields();
                if (values == null || values.Length == 0) continue; // null check
                int id = int.Parse(values[0]);
                if (id == 0) continue; // check for ID 0 values which is not allowed

                // value[1] is storage location and should be a location or No Location Set
                if (values[1] == null || values[1] == "") continue;

                // value[2] is the pallet height and must be taller than min height
                double height = double.Parse(values[2]);
                if (!(height > _minHeight)) continue; // check for height being less than min allowed height TODO

                // value[3] is the pallets product type and is either "No Product Type Set" or a product type not null
                if (values[3] == null || values[3] == "") continue;

                // otherwise pallet has passed file load checks and can be created
                var pallet = ProductPallet.CreateProductPallet(id, values[1], height, values[3]); // create object from line

                // check which list it needs to be allocated to
                if (values[1] == "No Location Set")
                {
                    nonAllocatedPallets.Add(pallet); // append to non allocated list
                }
                else // pallet has been allocated
                {
                    productPallets.Add(pallet); // append to product type list
                }

                // check if the pallets ID is greater than or equal to the newPalletID if so change to pallet ID +1
                if (pallet.Id >= newPalletID)
                {
                    newPalletID = pallet.Id + 1;
                    newPalletIDTextBox.Text = newPalletID.ToString(); // insert new value into form
                }
            }
            csvTextFieldParser.Close();
        }

        /// <summary>
        /// This is a simple function to clear the intial default text and will only happen on a default click
        /// </summary>
        /// <param name="sender">The object that fired the event</param>
        /// <param name="e">The event that triggered the event</param>
        public void PalletHeightTextBoxClicked(object sender, EventArgs e)
        {
            if (newPalletHeightTextBox.Text == _palletHeightInputDefault)
            {
                newPalletHeightTextBox.Text = "";
            }
        }

        /// <summary>
        /// This is a simple function that calls the PalletHeightInputValid to see if input is valid
        /// </summary>
        /// <param name="sender">The object that fired the event</param>
        /// <param name="e">The event that triggered the event</param>
        public void PalletHeightTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = newPalletHeightTextBox;
            PalletHeightInputValid(textBox);
        }

        /// <summary>
        /// This is a simple function to reset the Textbox to default text if left empty when the user leaves or
        /// if the user inputs a value that is not a double reset to default and show corresponding warning
        /// </summary>
        /// <param name="textBox">The textbox value should be where a user is inputting the pallet height into</param>
        /// <returns>Returns a bool true if the input is valid or false if not</returns>
        private bool PalletHeightInputValid(TextBox textBox)
        {
            string? text = textBox.Text;
            text = text.TrimEnd();
            text = text.TrimStart();
            // is text empty
            if (text == "")
            {
                textBox.Text = _palletHeightInputDefault;
                return false;
            }
            // is text not numeric or is the value less than min height
            else if (!double.TryParse(text, out double height) | height <= _minHeight)
            {
                textBox.Text = _palletHeightInputDefault;
                MessageBox.Show("You must enter a valid height with numeric values larger than: " + _minHeight.ToString(), "Input Error");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates the Columns for the specifed DataGridView for the pallets list, it then iterates through the list allocating it to the grid
        /// via the CreatePalletRow method
        /// </summary>
        /// <param name="dataGridView">The dataGridView should be the grid associated with pallets list</param>
        /// <param name="pallets">The pallets is the list that is associated with the supplied dataGridView</param>
        private void CreatePalletDataGrid(DataGridView dataGridView, List<ProductPallet> pallets)
        {
            // clear any existing data
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            // create table heads
            dataGridView.Columns.Add("ID", "ID");
            dataGridView.Columns.Add("Location", "Location");
            dataGridView.Columns.Add("Height", "Height");
            dataGridView.Columns.Add("Product Type", "Product Type");

            // Resize the datagrid dynamically
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // loop through pallet list and allocate them depending on requirement
            foreach (var pallet in pallets)
            {
                CreatePalletRow(dataGridView, pallet);
            }
        }

        /// <summary>
        /// Creates a DataGridViewRow and enters the ProductPallet supplied and adds the row to the supplied DataGridView
        /// </summary>
        /// <param name="dataGridView">The dataGridView should be the targeted DataGridView you wish to attach 'pallet' to</param>
        /// <param name="pallet">The pallet should be the desired pallet you wish to attach to the supplied dataGridView</param>
        private void CreatePalletRow(DataGridView dataGridView, ProductPallet pallet)
        {
            int rowId = dataGridView.Rows.Add(); // create new row and add it, store new row id

            DataGridViewRow row = dataGridView.Rows[rowId];// store the new row to a DataGridViewRow variable

            // append the pallet data to the row in the specified cell
            row.Cells["ID"].Value = pallet.Id;
            row.Cells["Location"].Value = pallet.StorageLocation;
            row.Cells["Height"].Value = pallet.PalletHeight;
            row.Cells["Product Type"].Value = pallet.ProductType;
        }

        /// <summary>
        /// This Method is for moving created pallets to a new location that they can go to, a pallet must be selected in the datagridview to fire
        /// otherwise a message box will be created and display an error message
        /// </summary>
        /// <param name="sender">The object that sent the request, in this case the move button</param>
        /// <param name="e">The event that triggered the method, in this case the click event</param>
        private void MovePalletButton_Click(object sender, EventArgs e)
        {
            // clear old data out
            palletSlotResultDataGridView.Rows.Clear();
            palletSlotResultDataGridView.Columns.Clear();
            // Is it in the allocated pallets?
            if (allocatedPalletsDataGridView.SelectedRows.Count > 0)
            {
                List<ProductPallet> pallet = new();
                int id = (int)allocatedPalletsDataGridView.SelectedRows[0].Cells[0].Value;
                string storageLocation = (string)allocatedPalletsDataGridView.SelectedRows[0].Cells[1].Value;
                double palletHeight = (double)allocatedPalletsDataGridView.SelectedRows[0].Cells[2].Value;
                string productType = (string)allocatedPalletsDataGridView.SelectedRows[0].Cells[3].Value;
                ProductPallet productPalletToAdd = ProductPallet.CreateProductPallet(id, storageLocation, palletHeight, productType);
                pallet.Add(productPalletToAdd);
                CreatePalletDataGrid(palletBeingAllocatedDataGridView, pallet);
                tabControl2.SelectedIndex = 2;
            }
            // is it in the non allocated pallets
            else if (nonAllocatedPalletsDataGridView.SelectedRows.Count > 0)
            {
                List<ProductPallet> pallet = new();
                int id = (int)nonAllocatedPalletsDataGridView.SelectedRows[0].Cells[0].Value;
                string storageLocation = (string)nonAllocatedPalletsDataGridView.SelectedRows[0].Cells[1].Value;
                double palletHeight = (double)nonAllocatedPalletsDataGridView.SelectedRows[0].Cells[2].Value;
                string productType = (string)nonAllocatedPalletsDataGridView.SelectedRows[0].Cells[3].Value;
                ProductPallet productPalletToAdd = ProductPallet.CreateProductPallet(id, storageLocation, palletHeight, productType);
                pallet.Add(productPalletToAdd);
                CreatePalletDataGrid(palletBeingAllocatedDataGridView, pallet);
                tabControl2.SelectedIndex = 2;
            }
            // not located at all
            else
            {
                string message = "You need to select a pallet before you can move it";
                string caption = "No Pallet Selected Error";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This Method is for deselecting any pallet selected in the allocated dataGridView
        /// </summary>
        /// <param name="sender">The object that sent the request, in this case the non allocated dataGridViewCell</param>
        /// <param name="e">The event that triggered the method, in this case the click event</param>
        public void NonAllocatedPalletsDataGridView_Click(object sender, EventArgs e)
        {
            allocatedPalletsDataGridView.ClearSelection();

            if (nonAllocatedPalletsDataGridView.SelectedCells.Count > 0)
            {
                int index = nonAllocatedPalletsDataGridView.SelectedCells[0].RowIndex;
                if (index != -1)
                {
                    nonAllocatedPalletsDataGridView.Rows[index].Selected = true;
                }
            }
        }

        /// <summary>
        /// This Method is for deselecting any pallet selected in the non allocated dataGridView
        /// </summary>
        /// <param name="sender">The object that sent the request, in this case the allocated dataGridViewCell</param>
        /// <param name="e">The event that triggered the method, in this case the click event</param>
        public void AllocatedPalletsDataGridViewClick(object sender, EventArgs e)
        {
            nonAllocatedPalletsDataGridView.ClearSelection();

            if(allocatedPalletsDataGridView.SelectedCells.Count > 0)
            {
                int index = allocatedPalletsDataGridView.SelectedCells[0].RowIndex;
                if (index != -1)
                {
                    allocatedPalletsDataGridView.Rows[index].Selected = true;
                }
            }
        }

        /// <summary>
        /// This Method is for deleting created pallets, a pallet must be selected in the datagridview to fire
        /// otherwise a message box will be created and display an error message
        /// </summary>
        /// <param name="sender">The object that sent the request, in this case the delete button</param>
        /// <param name="e">The event that triggered the method, in this case the click event</param>
        private void DeletePalletButton_Click(object sender, EventArgs e)
        {
            // create ref to allocate a pallet to, set to null for error checking
            ProductPallet? pallet = null;

            // is it an allocated pallet selected
            if (allocatedPalletsDataGridView.SelectedRows.Count > 0)
            {
                // make sure the selected row holds value before string cast
                if (allocatedPalletsDataGridView.SelectedRows[0].Cells[0].Value != null)
                {
                    string? s = allocatedPalletsDataGridView.SelectedRows[0].Cells[0].Value.ToString();
                    int Id = -1;
                    // check the id cell and cast to int
                    if (s != null)
                    {
                        Id = int.Parse(s);
                    }
                    // check the id is not -1 or 0
                    if (Id > 0)
                    {
                        // loop through pallet list to find the pallet to be deleted
                        foreach (var item in productPallets)
                        {
                            if (item.Id == Id)// found pallet end loop
                            {
                                pallet = item;
                                DeletePallet(pallet, productPallets);
                                break;
                            }
                            continue;
                        }
                    }
                    // error check as pallet not found if reached here
                    else if (!(Id != -1 && Id > 0 && pallet != null))
                    {
                        string message = "There appears to be an issuse with the selected pallet, and can not be found, " +
                            "has the file been modified while this was running if so close and restart the application.";
                        string caption = "No Pallet Selected Error";
                        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            // is an non allocated pallet selected
            else if (nonAllocatedPalletsDataGridView.SelectedRows.Count > 0)
            {
                // make sure the selected row holds value before string cast
                if (nonAllocatedPalletsDataGridView.SelectedRows[0].Cells[0].Value != null)
                {
                    string? s = nonAllocatedPalletsDataGridView.SelectedRows[0].Cells[0].Value.ToString();
                    int Id = -1;
                    // check the id cell and cast to int
                    if (s != null)
                    {
                        Id = int.Parse(s);
                    }
                    // check the id is not -1 or 0
                    if (Id > 0)
                    {
                        // loop through pallet list to find the pallet to be deleted
                        foreach (var item in nonAllocatedPallets)
                        {
                            if (item.Id == Id) // found pallet end loop
                            {
                                pallet = item;
                                DeletePallet(pallet, nonAllocatedPallets);
                                break;
                            }
                            continue;
                        }
                    }
                    // error check as pallet not found if reached here
                    else if (!(Id != -1 && Id > 0 && pallet != null))
                    {
                        string message = "There appears to be an issuse with the selected pallet, and can not be found, " +
                            "has the file been modified while this was running if so close and restart the application.";
                        string caption = "No Pallet Selected Error";
                        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            // no pallet selected
            else
            {
                string message = "You need to select a pallet before you can delete it";
                string caption = "No Pallet Selected Error";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This function is called to remove the specified pallet from the pallet file, the specified pallet list and 
        /// from the data grid associated with the specified pallet
        /// </summary>
        /// <param name="palletToBeDeleted">The ProductPallet that is required to be deleted</param>
        /// <param name="palletList">The List<ProductPallet> that is associated with the specified pallet</param>
        /// <exception cref="Exception">When illegal params are entered these errors will fire and notify the user on what line it occured on</exception>
        private void DeletePallet(ProductPallet palletToBeDeleted, List<ProductPallet> palletList)
        {
            string? tempFile = Path.GetTempFileName(); // create temp file
            StreamWriter streamWriter = new StreamWriter(tempFile); // create steamwriter for temp file
            csvTextFieldParser = new TextFieldParser(path + _productPalletFile); // read the current pallet file with the csvTextfieldParser
            // null check for csvTextFieldParser
            if (csvTextFieldParser == null)
            {
                streamWriter.Close();
                throw new Exception("An error occured with csvTextFieldParser");
            }
            // set Delimiters for textFieldParser
            csvTextFieldParser.SetDelimiters(new string[] { "," });
            int currentLine = 0; // stores current line
            int foundAt = -1;// stores what line the pallet was found on

            // loop till the file reaches the end
            while (!csvTextFieldParser.EndOfData)
            {
                try
                {
                    string? line = csvTextFieldParser.ReadLine(); // create line string from readline
                    // null check on line before progressing as no null lines are permitted
                    if (line == null)
                    {
                        csvTextFieldParser.Close();
                        streamWriter.Close();
                        string message = "There has been a problem in reading the CSV file in regards to the Product Pallet File, t" +
                            "here appears to be an ilegal entry at line: " + currentLine.ToString();
                        string caption = "ERROR";
                        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw new Exception("An error occured with csvTextFieldParser"); ;
                    }
                    // split the line, this is used for checking id against the specified id
                    string[]? values = line.Split(",");
                    // null check on values before progressing as no null lines are permitted
                    if (values == null)
                    {
                        csvTextFieldParser.Close();
                        streamWriter.Close();
                        string message = "There has been a problem in reading the CSV file in regards to the Product Pallet File, t" +
                            "here appears to be an ilegal entry at line: " + currentLine.ToString();
                        string caption = "ERROR";
                        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw new Exception("An error occured with csvTextFieldParser");
                    }
                    // found don't write line and add the line at what found at, move onto next line
                    if (int.Parse(values[0]) == palletToBeDeleted.Id) { foundAt = currentLine; currentLine++; continue; }
                    // check current line is not on the second and didn't just delete line 1 if so don't write new line into temp file
                    else if (currentLine == 1 && foundAt == 0)
                    { streamWriter.Write(line); currentLine++; continue; }
                    // first line of file and didn't find the pallet there, write line but no new line
                    else if (currentLine == 0)
                    {
                        streamWriter.Write(line);
                        currentLine++;
                        continue;
                    }
                    // line has not matched any previous requirements so write new line and line and continue as normal
                    else
                    {
                        streamWriter.Write(Environment.NewLine);
                        streamWriter.Write(line);
                        currentLine++;
                    }
                }
                // an error has occured throw error and stop, inform the user there has been an issue with the reading of the file
                catch (Exception)
                {
                    string message = "There has been a problem in reading the CSV file in regards to the Product Pallet File";
                    string caption = "ERROR";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("An error occured with csvTextFieldParser");
                }
            }
            // close reader and writer
            csvTextFieldParser.Close();
            streamWriter.Close();
            // delete old path
            File.Delete(path + _productPalletFile);
            // move temp file and name it to the previous file
            File.Move(tempFile, path + _productPalletFile);

            // delete it from the corresponding list and reload the data grid
            if (palletList == productPallets)
            {
                productPallets.Remove(palletToBeDeleted);
                CreatePalletDataGrid(allocatedPalletsDataGridView, productPallets);
            }
            else
            {
                nonAllocatedPallets.Remove(palletToBeDeleted);
                CreatePalletDataGrid(nonAllocatedPalletsDataGridView, nonAllocatedPallets);
            }

        }


        /// <summary>
        /// This Method will add the pallet to the _productPalletFile and save it
        /// </summary>
        /// <param name="pallet">The pallet should be a new pallet to be added and saved into the file</param>
        /// <exception cref="Exception">If any existing data has been edited in file it will throw an error and notify the user where the illegal entry was</exception>
        private void AddPalletToFile(ProductPallet pallet)
        {
            string newline = $"{pallet.Id},{pallet.StorageLocation},{pallet.PalletHeight},{pallet.ProductType}";

            string? tempFile = Path.GetTempFileName(); // create temp file
            StreamWriter streamWriter = new StreamWriter(tempFile); // create steamwriter for temp file
            csvTextFieldParser = new TextFieldParser(path + _productPalletFile); // read the current pallet file with the csvTextfieldParser
            // null check for csvTextFieldParser
            if (csvTextFieldParser == null)
            {
                streamWriter.Close();
                throw new Exception("An error occured with csvTextFieldParser");
            }
            // set Delimiters for textFieldParser
            csvTextFieldParser.SetDelimiters(new string[] { "," });
            int currentLine = 0; // stores current line for errors
            while (!csvTextFieldParser.EndOfData)
            {
                string? line = csvTextFieldParser.ReadLine(); // create line string from readline
                                                              // null check on line before progressing as no null lines are permitted
                if (line == null)
                {
                    csvTextFieldParser.Close();
                    streamWriter.Close();
                    string message = "There has been a problem in reading the CSV file in regards to the Product Pallet File, t" +
                        "here appears to be an ilegal entry at line: " + currentLine.ToString();
                    string caption = "ERROR";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("An error occured with csvTextFieldParser"); ;
                }
                streamWriter.WriteLine(line);
                currentLine++;
            }
            streamWriter.Write(newline);
            // close reader and writer
            csvTextFieldParser.Close();
            streamWriter.Close();
            // delete old path
            File.Delete(path + _productPalletFile);
            // move temp file and name it to the previous file
            File.Move(tempFile, path + _productPalletFile);
            newPalletID++;
        }

        /// <summary>
        /// This method is called whent the Create Pallet button is clicked and will validate the form and call methods to create a new pallet
        /// </summary>
        /// <param name="sender">The object calling this method in this case CreatePalletButton</param>
        /// <param name="e">The event that called this, in this case it is a click event</param>
        private void createPalletButton_Click(object sender, EventArgs e)
        {
            // check pallet height input valid
            bool palletHeightValid = PalletHeightInputValid(newPalletHeightTextBox);
            if (!palletHeightValid) return;
            double height = double.Parse(newPalletHeightTextBox.Text);
            // create variables for pallet type and pallet type checking
            string? palletType;
            string? inputText = newPalletProdTypecomboBox.Text;
            var selectedItem = newPalletProdTypecomboBox.SelectedItem;
            string? errorMsg;
            // check if a productType has been selected
            if (selectedItem != null)
            {
                palletType = selectedItem.ToString();
            }
            // check if productType has been created if not selected and not default
            else if (inputText != "Enter A New Product Type.." && inputText != "" && inputText != "Select the Pallets Product Type..")
            {
                palletType = inputText.ToString();
                bool doesItExist = false;
                // check to see if it exists
                foreach (ProductType item in productTypes)
                {
                    if (item.ProductName == palletType)
                    {
                        doesItExist = true;
                    }
                }
                // product type doesn't exist so create new one
                if(!doesItExist)
                {
                    CreateProductType(palletType);
                }
               
            }
            else
            {
                errorMsg = "You haven't selected a product type or entered a new one in.";
                MessageBox.Show(errorMsg, "Error");
                newPalletProdTypecomboBox.Text = "Enter A New Product Type..";
                return;
            }
            if (palletType != null)
            {
                var pallet = ProductPallet.CreateProductPallet(newPalletID, "No Location Set", height, palletType.ToString());
                nonAllocatedPallets.Add(pallet);
                CreatePalletDataGrid(nonAllocatedPalletsDataGridView, nonAllocatedPallets);
                AddPalletToFile(pallet);

                // reset form
                newPalletHeightTextBox.Text = _palletHeightInputDefault;
                LoadProductTypes();
                newPalletIDTextBox.Text = newPalletID.ToString();
            }
        }

        /// <summary>
        /// The Pallet has been allocated so update location and save to file
        /// </summary>
        /// <param name="productPallet">This should be the pallet thats being updated</param>
        /// <param name="rackID">The rack where the pallet is located</param>
        /// <param name="slotID">The slot that the pallet is located in</param>
        /// <exception cref="Exception">This will fire when there is problems in loading files and when there is null lines in the file</exception>
        private void CreatePalletLocation(ProductPallet productPallet, int rackID, int slotID)
        {
           // the pallets new location
            string palletLocation = $"Storage Rack {rackID} and Storage Slot {slotID}";
            productPallet.StorageLocation = palletLocation; // update location
            // create the line to update old line
            string newline = $"{productPallet.Id},{productPallet.StorageLocation},{productPallet.PalletHeight},{productPallet.ProductType}";

            string? tempFile = Path.GetTempFileName(); // create temp file
            StreamWriter streamWriter = new StreamWriter(tempFile); // create steamwriter for temp file
            csvTextFieldParser = new TextFieldParser(path + _productPalletFile); // read the current pallet file with the csvTextfieldParser
            // null check for csvTextFieldParser
            if (csvTextFieldParser == null)
            {
                streamWriter.Close();
                throw new Exception("An error occured with csvTextFieldParser");
            }
            // set Delimiters for textFieldParser
            csvTextFieldParser.SetDelimiters(new string[] { "," });
            int currentLine = 0; // stores current line for errors
            while (!csvTextFieldParser.EndOfData)
            {
                string? line = csvTextFieldParser.ReadLine(); // create line string from readline
                // null check on line before progressing as no null lines are permitted
                if (line == null)
                {
                    csvTextFieldParser.Close();
                    streamWriter.Close();
                    string message = "There has been a problem in reading the CSV file in regards to the Product Pallet File, t" +
                        "here appears to be an ilegal entry at line: " + currentLine.ToString();
                    string caption = "ERROR";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("An error occured with csvTextFieldParser"); ;
                }
                // see if current line is where the pallet is located?
                string[] values = line.Split(",");
                if (int.Parse(values[0]) == productPallet.Id)
                {
                    streamWriter.WriteLine(newline);// found
                }
                else
                {
                    streamWriter.WriteLine(line); // not found
                }
                currentLine++;
            }
            // close reader and writer
            csvTextFieldParser.Close();
            streamWriter.Close();
            // delete old path
            File.Delete(path + _productPalletFile);
            // move temp file and name it to the previous file
            File.Move(tempFile, path + _productPalletFile);
            MessageBox.Show("Product Pallet Successfully Allocated", "Success");
            tabControl2.SelectedIndex = 0; // return to the current pallets tab
            
            // clear allocation tab grids
            palletSlotResultDataGridView.Columns.Clear();
            palletSlotResultDataGridView.Rows.Clear();
            palletBeingAllocatedDataGridView.Columns.Clear();
            palletBeingAllocatedDataGridView.Rows.Clear();
        }
    }
}
