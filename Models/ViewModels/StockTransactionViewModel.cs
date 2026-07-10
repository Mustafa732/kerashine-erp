using KerashineERP.Models.Inventory;
using KerashineERP.Models.Master;
using System.ComponentModel.DataAnnotations;

namespace KerashineERP.Models.ViewModels
{
    public class StockTransactionViewModel
    {
        public INV_StockTransactionHeader Header { get; set; } = new();
        public List<INV_StockTransactionDetail> Details { get; set; } = new() { new INV_StockTransactionDetail() };

        // Dropdowns
        public List<SET_DocumentType> DocumentTypes { get; set; } = new();
        public List<SET_Location> Locations { get; set; } = new();
        public List<INV_Item> Items { get; set; } = new();
        public List<INV_SET_UOM> UOMs { get; set; } = new();

        [Display(Name = "Transaction Type")]
        public int SelectedDocTypeID { get; set; } // DocType dropdown ke liye
    }
}