using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT7536_FinalAssessment_20200344
{
    partial class Form1 : Form
    {
        /// <summary>
        /// This will allocate the pallet to the rack and update the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void allocatePalletToSlotButton_Click(object sender, EventArgs e)
        {
            if (palletSlotResultDataGridView.SelectedCells.Count > 0)
            {
                // Get the selected rack to alloacte to
                int selectedRow = palletSlotResultDataGridView.SelectedCells[0].RowIndex;
                palletSlotResultDataGridView.Rows[selectedRow].Selected = true;
                int rackId = (int)palletSlotResultDataGridView.SelectedRows[0].Cells[5].Value;
                int slotId = (int)palletSlotResultDataGridView.SelectedRows[0].Cells[0].Value;

                ProductPallet? selectedPallet = null;
                int palletID = (int)palletBeingAllocatedDataGridView.SelectedRows[0].Cells[0].Value;

                foreach (ProductPallet item in productPallets)
                {
                    // pallet found
                    if(item.Id == palletID)
                    {
                        selectedPallet = item;
                        break;
                    }
                }
                foreach (ProductPallet item in nonAllocatedPallets)
                {
                    // pallet found
                    if (item.Id == palletID)
                    {
                        selectedPallet = item;
                        break;
                    }
                }
                if (selectedPallet != null)
                {
                    // create the pallet with the new location
                    CreatePalletLocation(selectedPallet, rackId, slotId);
                    // reload the pallet file
                    ReadProductPallet(path+_productPalletFile);
                    if (productPallets.Count > 0) CreatePalletDataGrid(allocatedPalletsDataGridView, productPallets);
                    if (nonAllocatedPallets.Count > 0) CreatePalletDataGrid(nonAllocatedPalletsDataGridView, nonAllocatedPallets);
                    // clear the view rack tab
                    ClearViewStorageRack();
                }
                else
                {
                    MessageBox.Show("Finding the Pallet to allocate seems to be missing? Has someone recently deleted it?", "Error");
                }

            }
        }

        /// <summary>
        /// This looks for a storage rack that can hold the currently selected pallet
        /// it will also check to see if the checkbox for product type sensitivity has been checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchButton_Click(object sender, EventArgs e)
        {
            if (palletBeingAllocatedDataGridView.SelectedCells.Count > 0)
            {
                // get currently selected pallet information
                palletBeingAllocatedDataGridView.SelectAll();
                int id = (int)palletBeingAllocatedDataGridView.SelectedCells[0].Value;
                string storageLocation = (string)palletBeingAllocatedDataGridView.SelectedCells[1].Value;
                double palletHeight = (double)palletBeingAllocatedDataGridView.SelectedCells[2].Value;
                string productType = (string)palletBeingAllocatedDataGridView.SelectedCells[3].Value;
                ProductPallet pallet = ProductPallet.CreateProductPallet(id, storageLocation, palletHeight, productType);
                bool isCheckboxChecked = allocatePalletAnyRackCheckBox.Checked; // false means no check and isn't locked to product type
                
                List<StorageSlot>? storageSlots = new();

                CreateViewStorageRackDataGrid(palletSlotResultDataGridView, true);

                if (isCheckboxChecked)
                {
                    foreach (StorageRack storage in storageRacks)
                    {
                        if (storage.AllocatedSlots == null) continue; // no allocated slots so continue
                        if (storage.AllocatedProductType != productType) continue; // product types don't match

                        if (storage.Full != false) continue; // this rack is full

                        // check rack height to see if smaller than pallet minus min clearance
                        if (storage.RackHeight - _minClearance <= pallet.PalletHeight) continue;


                        foreach (var item in storage.AllocatedSlots)
                        {
                            if (item == null) continue; // shouldn't be null so skip


                            if (item.Full == true) continue; // if full then not useable
                            List<ProductPallet>? allocatedPallets = GetAllocatedSlotPallets(storage.Id, item.Id);

                            if (allocatedPallets != null && allocatedPallets.Count > 0)
                            {
                                double palletHeightStack = 0;

                                foreach (var allocatedPallet in allocatedPallets)
                                {
                                    palletHeightStack += allocatedPallet.PalletHeight;
                                }
                                if ((pallet.PalletHeight + palletHeightStack + _minClearance) > storage.RackHeight) continue; // current stack with new pallet too high
                                else
                                {
                                    // slot can handle this pallet aswell
                                    storageSlots.Add(item);
                                    CreateAllocatedSlotRow(palletSlotResultDataGridView, item, allocatedPallets, storage.Id);
                                }
                            }
                            // slot is empty and passed height check already so add slot
                            else
                            {
                                storageSlots.Add(item);
                                CreateAllocatedSlotRow(palletSlotResultDataGridView, item, allocatedPallets, storage.Id);
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
                        if (storage.RackHeight - _minClearance <= pallet.PalletHeight) continue;


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
                                if ((pallet.PalletHeight + palletHeightStack + _minClearance) > storage.RackHeight) continue; // current stack with new pallet too high
                                else
                                {
                                    // slot can handle this pallet aswell
                                    storageSlots.Add(item);
                                    CreateAllocatedSlotRow(palletSlotResultDataGridView, item, allocatedPallets, storage.Id);
                                }
                            }
                            // slot is empty and passed height check already so add slot
                            else
                            {
                                storageSlots.Add(item);
                                CreateAllocatedSlotRow(palletSlotResultDataGridView, item, allocatedPallets, storage.Id);
                            }

                        }

                    }
                }
            }
        }


    }
}
