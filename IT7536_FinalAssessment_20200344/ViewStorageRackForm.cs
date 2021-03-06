using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT7536_FinalAssessment_20200344
{
    /// <summary>
    /// this partial class is for creating the view storage rack page and filling the form in, it also has a method to clear the tab to defaults
    /// these methods are used elsewhere such as the getallocated slots and clearviewstoragerack
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// This method is to clear the view storage tab as when pallets are added this data will now be incorrect or incomplete
        /// </summary>
        private void ClearViewStorageRack()
        {
            // assign form values
            viewRackIDTextBox.Text = "";
            viewRackFullTextBox.Text = "";
            viewRackLocationTextBox.Text = "";
            viewRackProductTypeTextBox.Text = "";
            viewRackHAllocationTextBox.Text = "";
            viewRackVAllocationTextBox.Text = "";
            viewRackHeightTextBox.Text = "";
            viewRackAllocatedSlotsDataGridView.Columns.Clear();
            viewRackAllocatedSlotsDataGridView.Rows.Clear();
        }

        /// <summary>
        /// Create the view storage rack tab with the supplied storage rack
        /// </summary>
        /// <param name="storageRack"></param>
        private void CreateViewStorageRackPage(StorageRack storageRack)
        {
            if (storageRack == null) return; // this should only fire if the view button manages to send an empty rack object which it shouldn't

            // assign form values
            viewRackIDTextBox.Text = storageRack.Id.ToString();
            viewRackFullTextBox.Text = storageRack.Full ? "Rack Full" : "Space Available";
            viewRackLocationTextBox.Text = storageRack.Location;
            viewRackProductTypeTextBox.Text = storageRack.AllocatedProductType;
            viewRackHAllocationTextBox.Text = storageRack.HorizontalSlots.ToString();
            viewRackVAllocationTextBox.Text = storageRack.VerticalSlots.ToString();
            viewRackHeightTextBox.Text = storageRack.RackHeight.ToString() + "cm";

            // create the grid
            CreateViewStorageRackDataGrid(viewRackAllocatedSlotsDataGridView, false);
            // does it have allocated slots - it should
            if (storageRack.AllocatedSlots != null)
            {
                // loop through all the slots
                foreach (var item in storageRack.AllocatedSlots)
                {
                    List<ProductPallet>? slotsPallets = GetAllocatedSlotPallets(storageRack.Id, item.Id);
                    CreateAllocatedSlotRow(viewRackAllocatedSlotsDataGridView, item, slotsPallets, null);
                }
            }
        }

        /// <summary>
        /// Create the data grid on the view page and fill the form in with all the slots
        /// </summary>
        /// <param name="dataGridView">the grid that will hold the new data table</param>
        /// <param name="includeRackID">A bool to include the rack id associated with slot or not</param>
        private void CreateViewStorageRackDataGrid(DataGridView dataGridView, bool includeRackID)
        {
            // declare the combo box column
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();
            // clear previous data
            dataGridView.Columns.Clear();
            dataGridView.Rows.Clear();

            // create the column headers
            comboBoxColumn.HeaderText = "Allocated Pallets";
            comboBoxColumn.Name = "Allocated Pallets";


            dataGridView.Columns.Add("Slot ID", "Slot ID");
            dataGridView.Columns.Add(comboBoxColumn);
            dataGridView.Columns.Add("Slot Height", "Slot Height");
            dataGridView.Columns.Add("Available Height", "Available Height");
            dataGridView.Columns.Add("Product Type", "Product Type");

            // Resize the datagrid dynamically
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            // add the rack id if includeRackID is true
            if (includeRackID)
            {
                palletSlotResultDataGridView.Columns.Add("Rack ID", "Rack ID");
                palletSlotResultDataGridView.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        /// <summary>
        /// Create the the allocated slot row to go with the method CreateViewStorageRackDataGrid for the correct headings
        /// </summary>
        /// <param name="dataGridView">the datagrid being assigned this values to</param>
        /// <param name="storageSlot">the slot for this particular row</param>
        /// <param name="pallets">if there are pallets then pass them in a pallet list otherwise null if empty</param>
        /// <param name="RackID">if rack id is provided then it will be added to the row other wise null</param>
        private void CreateAllocatedSlotRow(DataGridView dataGridView, StorageSlot storageSlot, List<ProductPallet>? pallets, int? RackID)
        {
            int rowId = dataGridView.Rows.Add(); // create new row and add it, store new row id
            double availableHeight = storageSlot.RackHeight - _minClearance;
            string productType = storageSlot.AllocatedProductType;

            DataGridViewRow row = dataGridView.Rows[rowId];// store the new row to a DataGridViewRow variable
            DataGridViewComboBoxCell dropBoxComboCell = new(); // create ComboBoxCell for holding the Allocated slots

            // create custom drop box for pallets on the slot
            if (pallets != null && pallets.Count > 0)
            {
                int i = 0;
                foreach (var item in pallets)
                {
                    i++;
                    string displayString = $"ID: {item.Id}, Pallet Height: {item.PalletHeight}, Product Type: {item.ProductType}";
                    availableHeight -= item.PalletHeight;
                    // set the inital value to the first index
                    dropBoxComboCell.Items.Add(displayString);
                    if (i == 1)
                    {
                        dropBoxComboCell.Value = displayString;
                    }
                }
            }
            else
            {
                // adds a value to combo box incase no pallets exist
                dropBoxComboCell.Items.Add("No Allocated Pallets");
                dropBoxComboCell.Value = "No Allocated Pallets";
            }


            // append the pallet data to the row in the specified cell
            row.Cells["Slot ID"].Value = storageSlot.Id;
            row.Cells["Allocated Pallets"] = dropBoxComboCell;
            row.Cells["Slot Height"].Value = storageSlot.RackHeight.ToString("#.##");
            row.Cells["Available Height"].Value = availableHeight.ToString("#.##");
            row.Cells["Product Type"].Value = productType;


            row.Cells["Slot ID"].ReadOnly = true;
            row.Cells["Allocated Pallets"].ReadOnly = false;
            row.Cells["Slot Height"].ReadOnly = true;
            row.Cells["Available Height"].ReadOnly = true;
            row.Cells["Product Type"].ReadOnly = true;

            if (RackID != null)
            {
                row.Cells["Rack ID"].Value = RackID;
                row.Cells["Rack ID"].ReadOnly = true;
            }
        }

        /// <summary>
        /// This is to find any pallets that are linked to the provided slot id and rack id
        /// </summary>
        /// <param name="rackID">The id of the rack the parent to the slot</param>
        /// <param name="slotID">the id of the slot the child of rack</param>
        /// <returns>If allocated pallets exist and have been found they will return in a list otherwise returns an empty list</returns>
        private List<ProductPallet>? GetAllocatedSlotPallets(int rackID, int slotID)
        {
            List<ProductPallet>? allocatedPallets = new List<ProductPallet>();
            string searchQuery = $"Storage Rack {rackID} and Storage Slot {slotID}";

            // loop through pallets and find all pallets that match params
            foreach (var pallet in productPallets)
            {
                if (pallet != null)
                {

                    if (pallet.StorageLocation == searchQuery)
                    {
                        allocatedPallets.Add(pallet);
                    }
                }
            }

            return allocatedPallets;
        }
    }
}
