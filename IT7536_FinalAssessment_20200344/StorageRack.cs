using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT7536_FinalAssessment_20200344
{
    /// <summary>
    /// StorageRack is used for holding StorageSlots which hold ProductPallets, a StorageRack is used to give users a
    /// generalized view of what is being stored and where space is available
    /// </summary>
    internal class StorageRack
    {
        /// <summary>
        /// Private, Protected and Public variables for this Object - see constructor for more information on these variables
        /// </summary>
        private int _id;
        private List<StorageSlot> _allocatedSlots = new();
        private string _locatation = "No Location";
        private int _horizontalAllocation;
        private int _verticalAllocation;
        public bool Full { get; set; }
        private string _allocatedProductType;
        private double _rackHeight;

        /// <summary>
        /// Storage Rack Constructor
        /// </summary>
        /// <param name="id">This ID for this application will just be Interger Values starting at 1 and will be unique to others</param>
        /// <param name="allocatedSlots">This value will hold a list of integers that will be the Ids of the allocated storage 
        /// this value can be initially null as the child class belonging to this rack will not yet exist </param>
        /// <param name="locatation">A string value that will tell the user the location of the rack in regards to the warehouse</param>
        /// <param name="horizontalAllocation">This interger value is for telling how many storage slots horizontally are available</param>
        /// <param name="verticalAllocation">This interger value is for telling how many storage slots vertically are available</param>
        /// horizontalAllocation and verticalAllocation are used to work out the total availalble slots
        /// <param name="full">This boolean is tell that if the current rack is full</param>
        /// <param name="allocatedProductType">This value is a string value to tell what product storage is being kept here and if
        /// any of the storage locations inside this rack differ from this then this will be set to Miscilanious to let 
        /// the user know that this rack stores multiple products and will help with search querys</param>
        /// <param name="rackHeight">The rack height is a fixed value that the child will inheriet and be used to know what the 
        /// maximum height a pallet stack can get too</param>
        private StorageRack(int id,
                           List<StorageSlot>? allocatedSlots,
                           string locatation,
                           int horizontalAllocation,
                           int verticalAllocation,
                           bool full,
                           string allocatedProductType,
                           double rackHeight)
        {
            _id = id;
            if (!(allocatedSlots == null))
            {
                _allocatedSlots = allocatedSlots;
            }
            _locatation = locatation;
            _horizontalAllocation = horizontalAllocation;
            _verticalAllocation = verticalAllocation;
            Full = full;
            _allocatedProductType = allocatedProductType;
            _rackHeight = rackHeight;
        }
        /// <summary>
        /// This constructor is for child classes, by being protected it prevents access externally and creating storage racks with just these params
        /// </summary>
        /// <param name="id">This ID for this application will just be Interger Values starting at 1 and will be unique to others</param>
        /// <param name="locatation">A string value that will tell the user the location of the rack in regards to the warehouse</param>
        /// horizontalAllocation and verticalAllocation are used to work out the total availalble slots
        /// <param name="full">This boolean is tell that if the current rack is full</param>
        /// <param name="allocatedProductType">This value is a string value to tell what product storage is being kept here and if
        /// any of the storage locations inside this rack differ from this then this will be set to Miscilanious to let 
        /// the user know that this rack stores multiple products and will help with search querys</param>
        /// <param name="rackHeight">The rack height is a fixed value that the child will inheriet and be used to know what the 
        /// maximum height a pallet stack can get too</param>
        protected StorageRack(int id, string locatation, bool full, string allocatedProductType, double rackHeight)
        {
            _id = id;
            _locatation = locatation;
            Full = full;
            _allocatedProductType = allocatedProductType;
            _rackHeight = rackHeight;
        }

        /// <summary>
        /// This section is in regards to public property getters, setters and functions to manipulate private member variables,
        /// the getters and setters are written in a way that will be self explanatory the functions will however 
        /// feature details of use and purpose
        /// </summary>

        // Getter for id
        public int Id { get => _id; }
        // Getter for allocatedStorage
        public List<StorageSlot>? AllocatedSlots { get => _allocatedSlots; }

        /// <summary>
        /// This function is used to add storage to the rack this is to be used in initial construction and add the storage slot
        /// to the list once they are created. It is private as will only be used inside its constructor.
        /// </summary>
        /// <param name="slot">StorageSlot obj value that will be added to the list and accessed in other funcs</param>
        private void AddItemToAllocatedSlots(StorageSlot slot)
        {
            if (slot != null)
            {
                _allocatedSlots.Add(slot);
            }
        }
        // TODO ADD FEATURE TO UPDATE ALLOCATED SLOTS IE UPDATE SLOT FUNCTION THAT WILL ITTERATE THROUGH LIST TO SPECIEFIED
        // INDEX AND CHANGE THE PRODUCT STORED TYPE AND THEN CHANGE THIS CLASS STORAGE ALLOCATED SLOT LIKELY TO BE CHANGED TO
        // PRIVATE AND USED INTERNALLY
        public void SetAllocatedSlotsProductType(int slotIndex, string productType)
        {
            throw new Exception("This Function has not been developed yet");
        }

        // Getter for storage location
        public string Location { get => _locatation; }

        // Getter for number of horizontal slots
        public int HorizontalSlots { get => _horizontalAllocation; }

        // Getter for number of veritcal slots ie how many shelfs
        public int VerticalSlots { get => _verticalAllocation; }


        /// <summary>
        /// This function is used to give users the number of total storage slots of the current rack
        /// </summary>
        /// <returns>totalSlots = VerticalSlots * HorizontalSlots</returns>
        public int GetTotalNumberOfSlots()
        {
            int totalSlots = VerticalSlots * HorizontalSlots;
            return totalSlots;
        }

        public string AllocatedProductType { get => _allocatedProductType; set => _allocatedProductType = value; }
        public double RackHeight { get => _rackHeight; set => _rackHeight = value; }

        /// <summary>
        /// This is the public static method to create a new storage rack by using the factory design pattern we are able to perform error checks
        /// and input validation
        /// </summary>
        /// <param name="id">This ID for this application will just be Interger Values starting at 1 and will be unique to others</param>
        /// <param name="allocatedSlots">This value will hold a list of integers that will be the Ids of the allocated storage 
        /// this value can be initially null as the child class belonging to this rack will not yet exist </param>
        /// <param name="locatation">A string value that will tell the user the location of the rack in regards to the warehouse</param>
        /// <param name="horizontalAllocation">This interger value is for telling how many storage slots horizontally are available</param>
        /// <param name="verticalAllocation">This interger value is for telling how many storage slots vertically are available</param>
        /// horizontalAllocation and verticalAllocation are used to work out the total availalble slots
        /// <param name="full">This boolean is tell that if the current rack is full</param>
        /// <param name="allocatedProductType">This value is a string value to tell what product storage is being kept here and if
        /// any of the storage locations inside this rack differ from this then this will be set to Miscilanious to let 
        /// the user know that this rack stores multiple products and will help with search querys</param>
        /// <param name="rackHeight">The rack height is a fixed value that the child will inheriet and be used to know what the 
        /// maximum height a pallet stack can get too</param>
        /// <returns></returns>
        /// <exception cref="Exception"> this error will return the error or data issue</exception>
        public static StorageRack CreateStorage(int id,
                           List<StorageSlot>? allocatedSlots,
                           string locatation,
                           int horizontalAllocation,
                           int verticalAllocation,
                           bool full,
                           string allocatedProductType,
                           double rackHeight)
        {
            if (verticalAllocation < 1 || horizontalAllocation < 1)
            {
                throw new Exception("Vertical and Horizontal Allocation require a value of 1 or greater.");
            }
            return new StorageRack(id,
                            allocatedSlots,
                            locatation,
                            horizontalAllocation,
                            verticalAllocation,
                            full,
                            allocatedProductType,
                            rackHeight);
        }
    }
}
