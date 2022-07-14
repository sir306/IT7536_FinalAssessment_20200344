using Microsoft.VisualBasic.FileIO;
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
        /// Const variable for create rack form for the location default
        /// </summary>
        private const string _newRackLocationDefault = "Enter the Storage Rack's Location..";
        /// <summary>
        /// const variable for the create rack form for the product type default
        /// </summary>
        private const string _newRackProductTypeDefault = "Select or Enter a new Product Type";

        /// <summary>
        /// This method fires whenever the horizontal or vertical allocation NumericUpDown changes and recaluclates the currently allocated slots
        /// </summary>
        /// <param name="sender">the object that called the method</param>
        /// <param name="e">the event that fired the method</param>
        private void AllocationRecalculate(object sender, EventArgs e)
        {
            if (newAllocatedStorageSlotsNumericUpDown.Maximum <= newHorizontalAllocationNumericUpDown.Value * newVerticalAllocationNumericUpDown.Value)
            {
                newAllocatedStorageSlotsNumericUpDown.Maximum = newHorizontalAllocationNumericUpDown.Value * newVerticalAllocationNumericUpDown.Value;
            }
            newAllocatedStorageSlotsNumericUpDown.Value = newHorizontalAllocationNumericUpDown.Value * newVerticalAllocationNumericUpDown.Value;
        }

        /// <summary>
        /// This method fires when a user attempts to create a new storage rack it will attempt to check all the  inputs are valid 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewStorageRackButton_Click(object sender, EventArgs e)
        {
            // validate form
            bool validated = ValidateStorageForm();
            if (validated)
            {
                // declare temp variables to hold form data
                int id;
                List<StorageSlot>? allocatedSlots = new();
                string locatation;
                int horizontalAllocation;
                int verticalAllocation;
                bool full = false;
                string allocatedProductType;
                double rackHeight;

                // allocate form data to variables
                id = int.Parse(newStorageRackIDTextBox.Text);

                locatation = newStorageRackLocationTextBox.Text;
                horizontalAllocation = (int)newHorizontalAllocationNumericUpDown.Value;
                verticalAllocation = (int)newVerticalAllocationNumericUpDown.Value;
                allocatedProductType = newStorageRackProductTypecomboBox.Text;
                rackHeight = (double)newRackHeightNumericUpDown.Value;

                int amount = verticalAllocation * horizontalAllocation;
                int storageSlotsId = CreateStorageSlots(amount, rackHeight, locatation, allocatedProductType);
                if (storageSlotsId != -1)
                {
                    allocatedSlots = GetStorageRackSlots(storageSlotsId);
                    if (allocatedSlots == null)
                    {
                        MessageBox.Show("There appears to have been an error when creating the allocated slots please try again", "Unexpected Error");
                        return;
                    }
                    // create storage obj to pass to save storage rack and add it to the grid and storageRacks
                    var storageRack = StorageRack.CreateStorage(id,
                                                            allocatedSlots,
                                                            locatation,
                                                            horizontalAllocation,
                                                            verticalAllocation,
                                                            full,
                                                            allocatedProductType,
                                                            rackHeight);

                    SaveStorageRack(storageRack, storageSlotsId);
                    storageRacks.Add(storageRack);
                    CreateStorageRackDataGrid();
                }

            }
        }

        /// <summary>
        /// Clear the storage rack create form and set to defaults
        /// </summary>
        /// <param name="sender">the object that called the method</param>
        /// <param name="e">the event that fired the method</param>
        private void ClearNewStorageRackButton_Click(object sender, EventArgs e)
        {
            string racksLocation = "Enter the Storage Rack's Location..";
            string rackProductType = "Select or Enter a new Product Type";
            newStorageRackLocationTextBox.Text = racksLocation;
            newHorizontalAllocationNumericUpDown.Value = 1;
            newVerticalAllocationNumericUpDown.Value = 1;
            newStorageRackProductTypecomboBox.SelectedIndex = -1;
            newStorageRackProductTypecomboBox.Text = rackProductType;
            newRackHeightNumericUpDown.Value = (decimal)_rackMinHeight;
        }

        /// <summary>
        /// Validates the new storage rack creation form and returns true if valid or false if not and displays message boxes where the error was
        /// </summary>
        /// <returns>True if valid or False if invalid</returns>
        private bool ValidateStorageForm()
        {
            // create string variables to hold temp form data
            string? text = null;
            text = newStorageRackLocationTextBox.Text;
            text = text.Trim();
            string? msg = null;
            // check location has been set
            if (text == null || text == _newRackLocationDefault)
            {
                msg = "You Need to enter a valid location";
                MessageBox.Show(msg);
                return false;
            }
            // check horizontal and vertical allocation has been set correctly
            if (newHorizontalAllocationNumericUpDown.Value == 0 || newVerticalAllocationNumericUpDown.Value == 0)
            {
                msg = "You have Entered 0 as a horizontal or vertical allocation value which is not allowed";
                MessageBox.Show(msg);
                return false;
            }
            // reallocate text to next validation
            text = newStorageRackProductTypecomboBox.Text;
            text = text.Trim();
            // check the product type has been given a value
            if (text == null || text == _newRackProductTypeDefault || text == "")
            {
                msg = "You have not entered a value into the product type which is not allowed";
                MessageBox.Show(msg);
                return false;
            }
            // cast rackheight numeric up down to a double
            double? rackHeight = (double)newRackHeightNumericUpDown.Value;
            // check rack height against minimum
            if (rackHeight < _rackMinHeight)
            {
                msg = "The Rack height you have entered is not of the minimum height which is: " + _rackMinHeight.ToString();
                MessageBox.Show(msg);
                return false;
            }
            // passed all checks return true
            return true;
        }

        /// <summary>
        /// Save the new storage rack and the associated slot id to the file
        /// </summary>
        /// <param name="storageRack">the storage rack being saved</param>
        /// <param name="storageRackSlotsId">the id associated to the slot id so it can found when file is loaded</param>
        /// <exception cref="Exception">if file fails to load then theprogram will throw an error</exception>
        private void SaveStorageRack(StorageRack storageRack, int storageRackSlotsId)
        {
            string newline = $"{storageRack.Id},{storageRackSlotsId},{storageRack.Location},{storageRack.HorizontalSlots},{storageRack.VerticalSlots},{storageRack.Full},{storageRack.AllocatedProductType},{storageRack.RackHeight}";

            TextFieldParser textFieldParser = new TextFieldParser(path + _storageRackFile);

            if (textFieldParser == null)
            {
                throw new Exception("An error occured with storageSlotTextFieldParser");
            }
            textFieldParser.SetDelimiters(new string[] { "," }); // set Delimiter params for TextFieldParser

            string? tempFile = Path.GetTempFileName(); // create temp file
            StreamWriter streamWriter = new StreamWriter(tempFile); // create steamwriter for temp file

            while (!textFieldParser.EndOfData)
            {
                string? line = textFieldParser.ReadLine();// read and create line
                if (line != null)
                {
                    streamWriter.WriteLine(line);
                }
            }
            // insert new line
            streamWriter.Write(newline);
            // close parser and writer
            streamWriter.Close();
            textFieldParser.Close();
            // delete old file
            File.Delete(path + _storageRackFile);
            // move temp file and name it to the previous file
            File.Move(tempFile, path + _storageRackFile);
        }
    }
}
