using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT7536_FinalAssessment_20200344
{
    /// <summary>
    /// This partial class is for the storage rack tab or where the method is mainly for storage racks
    /// </summary>
    public partial class Form1 : Form
    {
        private double _rackMinHeight = 100.0; // TODO mend height to a realistic height
        private int _newStorageRackID = 1;// This holds current new rack id its used for creating new racks

        private void ReadStorageRackFile()
        {
            csvTextFieldParser = new TextFieldParser(path + _storageRackFile);
            if (csvTextFieldParser == null)
            {
                throw new Exception("An error occured with csvTextFieldParser");
            }
            csvTextFieldParser.SetDelimiters(new string[] { "," }); // set Delimiter params for TextFieldParser
            int currentFileLine = 0;// declared for tracking current file line starts at 0 as increments every loop at the start
            while (!csvTextFieldParser.EndOfData)
            {
                currentFileLine++;// increment file line to show where error or invalid data exists
                string[]? values = csvTextFieldParser.ReadFields();// convert csv line into string array
                if (values == null || values.Length == 0) // null check
                {
                    MessageBox.Show("There appears to be a line in the file where no data exists  which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Empty FileLine");
                    continue;
                }
                int id = int.Parse(values[0]);
                if (id <= 0)// check for ID 0 or less values which is not allowed
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid ID which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid ID");
                    continue;
                }
                // values[1] is the Id of the slots in regards to the file StorageRackSlots which holds a list of ids of the slots
                int slotId = int.Parse(values[1]);
                List<StorageSlot>? storageSlots = null;
                if (slotId > 0)
                {
                    storageSlots = GetStorageRackSlots(int.Parse(values[1]));
                    if (storageSlots == null)
                    {
                        MessageBox.Show("There appears to be a line in the file where the data holds an invalid SlotsID which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Slot ID");
                        continue;
                    }
                }

                // values[2] is the racks location and is either "No Location" or an actual location, this can not be null or empty
                if (values[2] == "" || values[2] == null)
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid Rack Location which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Location");
                    continue;
                }
                string location = values[2];
                // value[3] is the storage racks horizontal allocation and requires to be at least 1
                if (values[3] == null || int.Parse(values[3]) < 1)
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid horizontalAllocation which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Horizontal Allocation");
                    continue;
                }
                int horizontalAllocation = int.Parse(values[3]);
                // values[4] is the storage racks vertical allocation and requires to be at least 1
                if (values[4] == null || int.Parse(values[4]) < 1)
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid verticalAllocation which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Vertical Allocation");
                    continue;
                }
                int verticalAllocation = int.Parse(values[4]);
                // values[5] is the storage racks current state of full or not as it uses a boolean it will check to see if the value is null
                // and a gettype
                if (values[5] == null || !(bool.Parse(values[5]) == true || bool.Parse(values[5]) == false))
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid full storage value which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Full allotment");
                    continue;
                }
                bool full = bool.Parse(values[5]);
                // values[6] is the storage racks _allotedProductType and should hold a string value
                if (values[6] == null || values[6] == "")
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid Product Type which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Product Type");
                    continue;
                }
                string productType = values[6];
                // values[7] is a double and will require to be larger than a min height
                if (!(double.TryParse(values[7], out double height)) || height < _rackMinHeight)
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid Rack height which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Rack Height");
                    continue;
                }
                if (id >= _newStorageRackID) // increment new id if needed
                {
                    _newStorageRackID = id + 1;
                }

                var storageRack = StorageRack.CreateStorage(id, storageSlots, location, horizontalAllocation, verticalAllocation, full, productType, height);
                storageRacks.Add(storageRack); // add the rack to the list
            }
            newStorageRackIDTextBox.Text = _newStorageRackID.ToString(); // data is all loaded this is the current new id set it on form
            csvTextFieldParser.Close();
        }

        /// <summary>
        /// This method is used to find all the slots that are linked to the ID for that line of slots from the Storage Rack
        /// </summary>
        /// <param name="id">This ID has to be the identifying ID that belongs to the slots of that line</param>
        /// <returns>If slots have been found they will be returned as a list, if they have not this will return null</returns>
        /// <exception cref="Exception">An exception will be thrown if the file holding the storageRackSlotFile can't be opened</exception>
        private List<StorageSlot>? GetStorageRackSlots(int id)
        {
            List<StorageSlot> storageSlots = new();

            TextFieldParser storageSlotTextFieldParser = new TextFieldParser(path + _storageRackSlotFile);

            ArrayList slotList = new ArrayList();

            if (storageSlotTextFieldParser == null)
            {
                throw new Exception("An error occured with storageSlotTextFieldParser");
            }
            storageSlotTextFieldParser.SetDelimiters(new string[] { "," }); // set Delimiter params for TextFieldParser
            int currentFileLine = 0;// declared for tracking current file line starts at 0 as increments every loop at the start

            while (!storageSlotTextFieldParser.EndOfData)
            {
                currentFileLine++;// increment value
                string[]? values = storageSlotTextFieldParser.ReadFields();

                if (values != null)
                {
                    if (int.Parse(values[0]) == id) // found the row looking for
                    {
                        foreach (var item in values) // add all the variable to the arraylist note the first one is an id for the row and not a slot id
                        {
                            slotList.Add(int.Parse(item));
                        }
                        slotList.RemoveAt(0); // remove the first item from the array as it is not needed for this
                        break;
                    }
                    else // not value looking for continue to next line
                    {
                        continue;
                    }
                }
                else
                {
                    // values are null and shouldn't be throw error and notify the user where
                    string msg = "There invalid data in the storageSlot.csv file at line: " + currentFileLine.ToString();
                    MessageBox.Show(msg, "Invalid Data Entry");
                }
            }
            // file is finished close
            storageSlotTextFieldParser.Close();

            // check if the slotList holds a value
            if (slotList.Count > 0)
            {
                // find the slot associated and add them to the list
                foreach (int item in slotList)
                {
                    StorageSlot? slot = GetCompleteSlot(item);
                    if (slot != null)
                    {
                        storageSlots.Add(slot);
                    }
                }
            }
            // return storageSlots with or without values
            return storageSlots;
        }

        /// <summary>
        /// Creates the data grid associated with the current Storage Rack Grid and calls CreateStorageRackRow() to add the data to the grid 
        /// </summary>
        private void CreateStorageRackDataGrid()
        {
            DataGridView dataGridView = currentStorageRackDataGridView;
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();
            // clear any existing data
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            comboBoxColumn.HeaderText = "Allocated Slots";
            comboBoxColumn.Name = "Allocated Slots";

            // create table heads
            dataGridView.Columns.Add("ID", "ID");
            dataGridView.Columns.Add(comboBoxColumn);
            //dataGridView.Columns.Add("Allocated Slots", "Allocated Slots");
            dataGridView.Columns.Add("Location", "Location");
            dataGridView.Columns.Add("Horizontal Allocation", "Horizontal Allocation");
            dataGridView.Columns.Add("Vertical Allocation", "Vertical Allocation");
            dataGridView.Columns.Add("Full", "Full");
            dataGridView.Columns.Add("Product Type", "Product Type");
            dataGridView.Columns.Add("Rack Height", "Rack Height");

            // Resize the datagrid dynamically
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;



            //row.Cells["ID"].Value = new DataGridViewComboBoxCell()
            // loop through storage rack list and allocate them depending on requirement
            foreach (var storage in storageRacks)
            {
                CreateStorageRackRow(dataGridView, storage);
            }
        }

        /// <summary>
        /// Creates the storage rack row for the specified dataGridview and adds the specified storage rack
        /// </summary>
        /// <param name="dataGridView">The dataGridView is the DataGridView you wish to add this Storage rack to</param>
        /// <param name="storageRack">The storageRack should be the storage rack you wish to add to the row</param>
        private void CreateStorageRackRow(DataGridView dataGridView, StorageRack storageRack)
        {
            int rowId = dataGridView.Rows.Add(); // create new row and add it, store new row id

            DataGridViewRow row = dataGridView.Rows[rowId];// store the new row to a DataGridViewRow variable
            DataGridViewComboBoxCell dropBoxComboCell = new(); // create ComboBoxCell for holding the Allocated slots

            // Create Custom ComboBox items from Allocated slots
            if (storageRack.AllocatedSlots != null && storageRack.AllocatedSlots.Count >0)
            {
                int i = 0;
                foreach (var item in storageRack.AllocatedSlots)
                {
                    i++;
                    string displayString = "ID: " + item.Id.ToString() + ", Full: " + item.Full;
                    dropBoxComboCell.Items.Add(displayString);
                    // set the inital value to the first index
                    if (i == 1)
                    {
                        dropBoxComboCell.Value = displayString;
                    }
                }
            }
            else
            {
                // adds a value to combo box incase no slots exist
                dropBoxComboCell.Items.Add("No Allocated Pallets");
                dropBoxComboCell.Value = "No Allocated Slots";
            }

            // append the pallet data to the row in the specified cell
            row.Cells["ID"].Value = storageRack.Id;
            row.Cells["Allocated Slots"] = dropBoxComboCell; // create drop box to hold the slots
            row.Cells["Location"].Value = storageRack.Location;
            row.Cells["Horizontal Allocation"].Value = storageRack.HorizontalSlots;
            row.Cells["Vertical Allocation"].Value = storageRack.VerticalSlots;
            row.Cells["Full"].Value = storageRack.Full;
            row.Cells["Product Type"].Value = storageRack.AllocatedProductType;
            row.Cells["Rack Height"].Value = storageRack.RackHeight;

            // Set the cells readability - as only the allocated slots should be editable to allow drop down
            row.Cells["ID"].ReadOnly = true;
            row.Cells["Allocated Slots"].ReadOnly = false;
            row.Cells["Location"].ReadOnly = true;
            row.Cells["Horizontal Allocation"].ReadOnly = true;
            row.Cells["Vertical Allocation"].ReadOnly = true;
            row.Cells["Full"].ReadOnly = true;
            row.Cells["Product Type"].ReadOnly = true;
            row.Cells["Rack Height"].ReadOnly = true;

        }

        /// <summary>
        /// Click evenet for the current storage data grid view to highlight and select the data, used for the delete and view buttons on the tab
        /// </summary>
        /// <param name="sender">the object that called the method</param>
        /// <param name="e">the event that fired the method</param>
        private void CurrentStorageRacksDataGridView_Click(object sender, EventArgs e)
        {
            if (currentStorageRackDataGridView.SelectedCells.Count > 0) // check there is selected cells meaning there must be some form of data there
            {
                int index = currentStorageRackDataGridView.SelectedCells[0].RowIndex;
                if (index != -1)
                {
                    currentStorageRackDataGridView.Rows[index].Selected = true;
                }
            }
        }

        private void ViewRackButton_Click(object sender, EventArgs e)
        {
            if(currentStorageRackDataGridView.SelectedCells.Count > 0) // if a cell is selected then a row can be or is selected and data exists
            {
                int rowIndex = currentStorageRackDataGridView.SelectedCells[0].RowIndex;
                CreateViewStorageRackPage(storageRacks[rowIndex]);
                tabControl3.SelectedIndex = 2;
            }

        }
    }
}