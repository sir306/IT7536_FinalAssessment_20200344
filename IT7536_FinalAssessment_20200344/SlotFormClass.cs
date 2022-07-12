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
        private int _newSlotID = 0;

        private int CreateStorageSlots(int amount, double height, string location, string productType)
        {
            int slotID = -1;
            if (amount <= 0) return slotID;
            if (height < _rackMinHeight) return slotID;
            if (location == null || location == "") return slotID;
            if (productType == null || productType == "") return slotID;

            List<StorageSlot>? storageSlots = new();
            List<int> slotIDs = new();
            int i = 0;
            do
            {
                i++;
                StorageSlot storageSlot = new StorageSlot(_newSlotID, location, false, productType, height); // create new slot
                storageSlots.Add(storageSlot);// add to return list
                slotIDs.Add(_newSlotID);// add to slotID list for storageRack
                // increment newSlotID
                _newSlotID++;
            } while (amount != i);

            foreach (StorageSlot storageSlot in storageSlots)
            {
                CommitNewSlotsToFile(storageSlot); // save new slot
            }
            slotID = SaveSlotList(slotIDs);

            return slotID;
        }

        private void CommitNewSlotsToFile(StorageSlot storageSlot)
        {
            TextFieldParser textFieldParser = new TextFieldParser(path + _slots);

            if (textFieldParser == null)
            {
                throw new Exception("An error occured with storageSlotTextFieldParser");
            }
            textFieldParser.SetDelimiters(new string[] { "," }); // set Delimiter params for TextFieldParser
            string insertLine = $"{storageSlot.Id},{storageSlot.Location},{storageSlot.Full},{storageSlot.AllocatedProductType},{storageSlot.RackHeight}";

            string? tempFile = Path.GetTempFileName(); // create temp file
            StreamWriter streamWriter = new StreamWriter(tempFile); // create steamwriter for temp file

            while(!textFieldParser.EndOfData)
            {
                string? line = textFieldParser.ReadLine();// read and create line
                if(line != null)
                {
                    streamWriter.WriteLine(line);
                }
            }
            // insert new line
            streamWriter.Write(insertLine);
            // close parser and writer
            streamWriter.Close();
            textFieldParser.Close();
            // delete old file
            File.Delete(path + _slots);
            // move temp file and name it to the previous file
            File.Move(tempFile, path + _slots);
        }

        private int SaveSlotList(List<int> slotList)
        {
            int slotID = 1;
            // create insert line
            string insertLine = "";
            int i = 0;
            // loop through list to create insert string
            while(i < slotList.Count)
            {
                // add slot id
                insertLine += slotList[i].ToString();
                // if not last line add comma
                if (i < slotList.Count - 1)
                {
                    insertLine += ",";
                }
                i++;
            }

            TextFieldParser textFieldParser = new TextFieldParser(path + _storageRackSlotFile);

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
                    slotID++;
                    streamWriter.WriteLine(line);
                }
            }
            // create final of the insert line
            insertLine = $"{slotID},{insertLine}"; 
            // insert new line
            streamWriter.Write(insertLine);
            // closer parser and writer
            streamWriter.Close();
            textFieldParser.Close();

            // delete old file
            File.Delete(path + _storageRackSlotFile);
            // move temp file and name it to the previous file
            File.Move(tempFile, path + _storageRackSlotFile);

            return slotID;
        }

        /// <summary>
        /// Find the complete Slot entry with the specified ID in the file slots.csv
        /// </summary>
        /// <param name="id">The Id of the storage slot</param>
        /// <returns>A slot object if found and complete else returns null</returns>
        /// <exception cref="Exception">If errors that shouldn't happen such as reading the file at the start an error will be thrown</exception>
        private StorageSlot? GetCompleteSlot(int id)
        {
            StorageSlot? storageSlot = null;

            TextFieldParser slotTextFieldParser = new TextFieldParser(path + _slots);

            if (slotTextFieldParser == null)
            {
                throw new Exception("An error occured with storageSlotTextFieldParser");
            }
            slotTextFieldParser.SetDelimiters(new string[] { "," }); // set Delimiter params for TextFieldParser
            int currentFileLine = 0;// declared for tracking current file line starts at 0 as increments every loop at the start

            while (!slotTextFieldParser.EndOfData)
            {
                currentFileLine++;
                string[]? values = slotTextFieldParser.ReadFields();

                if (values != null) // holds data
                {
                    // update new slot id
                    if(_newSlotID <= int.Parse(values[0]))
                    {
                        _newSlotID = int.Parse(values[0]) + 1;
                    }
                    if (int.Parse(values[0]) == id) // matches id
                    {
                        if (values.Count() == 5) // there is the correct amount of elements
                        {
                            string location = values[1];
                            if (location == "" || location == null)
                            {
                                // values are null and shouldn't be throw error and notify the user where
                                string msg = "The data in slot.csv was found but has an invalid location assigned to it please fix it at line: " + currentFileLine.ToString();
                                MessageBox.Show(msg, "Invalid Data Entry");
                                slotTextFieldParser.Close();
                                return null;
                            }
                            bool full = bool.Parse(values[2]);
                            string productType = values[3];
                            if (productType == "" || productType == null)
                            {
                                // values are null and shouldn't be throw error and notify the user where
                                string msg = "The data in slot.csv was found but has an invalid productType assigned to it please fix it at line: " + currentFileLine.ToString();
                                MessageBox.Show(msg, "Invalid Data Entry");
                                slotTextFieldParser.Close();
                                return null;
                            }
                            double height = double.Parse(values[4]);
                            if (height < _rackMinHeight)
                            {
                                // values are null and shouldn't be throw error and notify the user where
                                string msg = "The data in slot.csv was found but has an invalid Rack height assigned to it please fix it at line: " + currentFileLine.ToString();
                                MessageBox.Show(msg, "Invalid Data Entry");
                                slotTextFieldParser.Close();
                                return null;
                            }
                            storageSlot = new StorageSlot(id, location, full, productType, height);
                        }
                        else
                        {
                            // there are missing values and shouldn't be throw error and notify the user where
                            string msg = "The data in slot.csv was found but has missing values please fix it at line: " + currentFileLine.ToString();
                            MessageBox.Show(msg, "Invalid Data Entry");
                            slotTextFieldParser.Close();
                            return null;
                        }
                    }
                }
                else
                {
                    // values are null and shouldn't be throw error and notify the user where
                    string msg = "There invalid data in the slot.csv file at line: " + currentFileLine.ToString();
                    MessageBox.Show(msg, "Invalid Data Entry");
                }

            }
            slotTextFieldParser.Close();
            // return slot if found or null if not
            return storageSlot;
        }

    }
}
