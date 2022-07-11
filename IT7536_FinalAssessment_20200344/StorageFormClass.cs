using Microsoft.VisualBasic.FileIO;
using System;
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
        private double _rackMinHeight = 10.0; // TODO mend height to a realistic height
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
                // values[1] is not checked here as this can be an empty list
                // values[2] is the racks location and is either "No Location" or an actual location, this can not be null or empty
                if (values[2] == "" || values[2] == null)
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid Rack Location which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Location");
                    continue;
                }

                // value[3] is the storage racks horizontal allocation and requires to be at least 1
                if (values[3] == null || int.Parse(values[3]) <= 1)
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid horizontalAllocation which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Horizontal Allocation");
                    continue;
                }

                // values[4] is the storage racks vertical allocation and requires to be at least 1
                if (values[4] == null || int.Parse(values[4]) <= 1)
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid verticalAllocation which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Vertical Allocation");
                    continue;
                }

                // values[5] is the storage racks current state of full or not as it uses a boolean it will check to see if the value is null
                // and a gettype
                if(values[5] == null || !(bool.Parse(values[5]) == true || bool.Parse(values[5]) == false))
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid full storage value which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Full allotment");
                    continue;
                }

                // values[6] is the storage racks _allotedProductType and should hold a string value
                if (values[6] == null || values[6] == "")
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid Product Type which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Product Type");
                    continue;
                }

                // values[7] is a double and will require to be larger than a min height
                if (double.TryParse(values[7], out double height) || height < _rackMinHeight)
                {
                    MessageBox.Show("There appears to be a line in the file where the data holds an invalid Rack height which will not be loaded at line: "
                        + currentFileLine.ToString(), "Warning Invalid Rack Height");
                    continue;
                }
            }
            csvTextFieldParser.Close();
        }
    }
}

// list into csv id*location*allocatedproduct*full*rackheight-id*location*allocatedproduct*full*rackheight etc