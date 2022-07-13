using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT7536_FinalAssessment_20200344
{
    public partial class Form1 : Form
    {
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

            CreateViewStorageRackDataGrid();
            if (storageRack.AllocatedSlots != null)
            {
                foreach (var item in storageRack.AllocatedSlots)
                {
                    List<ProductPallet>? slotsPallets = GetAllocatedSlotPallets(storageRack.Id, item.Id);
                    CreateAllocatedSlotRow(viewRackAllocatedSlotsDataGridView, item, slotsPallets);
                }
            }
        }

        private void CreateViewStorageRackDataGrid()
        {
            DataGridView dataGridView = viewRackAllocatedSlotsDataGridView;
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();

            dataGridView.Columns.Clear();
            dataGridView.Rows.Clear();

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
        }

        private void CreateAllocatedSlotRow(DataGridView dataGridView, StorageSlot storageSlot, List<ProductPallet>? pallets)
        {
            int rowId = dataGridView.Rows.Add(); // create new row and add it, store new row id
            double availableHeight = storageSlot.RackHeight;
            string productType = storageSlot.AllocatedProductType;

            DataGridViewRow row = dataGridView.Rows[rowId];// store the new row to a DataGridViewRow variable
            DataGridViewComboBoxCell dropBoxComboCell = new(); // create ComboBoxCell for holding the Allocated slots

            // create custom drop box for pallets on the slot
            if(pallets != null && pallets.Count > 0)
            {
                int i = 0;
                foreach (var item in pallets)
                {
                    i++;
                    string displayString = $"ID: {item.Id}, Pallet Height: {item.PalletHeight}, Product Type: {item.ProductType}";
                    availableHeight -= item.PalletHeight;
                    // set the inital value to the first index
                    dropBoxComboCell.Items.Add(displayString);
                    if(i == 1)
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
        }

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
