using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT7536_FinalAssessment_20200344
{
    /// <summary>
    /// StorageSlot is a Child of StorageRack and is used for holding ProductPallets inside the StorageRack
    /// </summary>
    internal class StorageSlot : StorageRack
    {
        /// <summary>
        /// Private, Protected and Public variables for this Object - see constructor for more information on these variables
        /// </summary>
        private int _id;
        private string _locatation = "No Location";
        private string _allocatedProductType;
        private double _rackHeight;

        /// <summary>
        /// Constructor for Storage Slot
        /// </summary>
        /// <param name="id">ID of object</param>
        /// <param name="locatation">The Location of the rack that this slot belongs to</param>
        /// <param name="full">Space available or not boolean check</param>
        /// <param name="allocatedProductType">The products being stored in this rack for this project I am only permitting one prodyct type per slot</param>
        /// <param name="rackHeight">Height of the slot</param>
        public StorageSlot(int id, string locatation, bool full, string allocatedProductType, double rackHeight) : base(id, locatation, full, allocatedProductType, rackHeight)
        {
            _id = id;
            _locatation = locatation;
            Full = full;
            _allocatedProductType = allocatedProductType;
            _rackHeight = rackHeight;
        }
    }
}
