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
        private const string _newRackLocationDefault = "Enter the Storage Rack's Location..";
        private const string _newRackProductTypeDefault = "Select or Enter a new Product Type";

        private void CreateEventBindings()
        {

        }

        private void CreateNewStorageRackButton_Click(object sender, EventArgs e)
        {
            bool validated = ValidateStorageForm();
            if (validated)
            {
                int id;
                List<StorageSlot>? allocatedSlots = new();
                string locatation;
                int horizontalAllocation;
                int verticalAllocation;
                bool full = false;
                string allocatedProductType;
                double rackHeight;

                id = int.Parse(newStorageRackIDTextBox.Text);
                
                locatation = newStorageRackLocationTextBox.Text;
                horizontalAllocation = (int)newHorizontalAllocationNumericUpDown.Value;
                verticalAllocation = (int)newVerticalAllocationNumericUpDown.Value;
                allocatedProductType = newStorageRackProductTypecomboBox.Text;
                rackHeight = (double)newRackHeightNumericUpDown.Value;

                int amount = verticalAllocation * horizontalAllocation;
                int storageSlotsId = CreateStorageSlots(amount, rackHeight, locatation, allocatedProductType);
                if(storageSlotsId != -1)
                {
                    allocatedSlots = GetStorageRackSlots(storageSlotsId);
                    if (allocatedSlots == null)
                    {
                        MessageBox.Show("There appears to have been an error when creating the allocated slots please try again", "Unexpected Error");
                        return;
                    }
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

        private void ClearNewStorageRackButton_Click(object sender, EventArgs e)
        {

        }

        private void NewStorageRackHelpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

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
