using CandaWebUtility.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CandaWebUtility.Models
{
    public class HighJumpMoveBinLabel
    {
        internal void Init(Guid rowID)
        {
            this.RowID = rowID;
            //this.LotList = new List<string>();
            this.Warning = string.Empty;

            using (HJWarehouseEntities db = new HJWarehouseEntities())
            {
                var binlabel = db.BINLOCAT.AsNoTracking().Where(m => m.ROWID == rowID).FirstOrDefault();
                if (binlabel != null)
                {
                    this.Product = binlabel.PRODUCT;
                    this.Lot = binlabel.PO_NUM;
                    this.NewLot = binlabel.PO_NUM;
                    this.BinLabel = binlabel.BINLABEL;
                    this.Quantity = binlabel.QUANTITY.Value;
                    this.ExpiryDate = binlabel.DATECREATE.Value;
                }
            }

            this.NewQuantity = this.Quantity;
        }

        internal void Save(string userID)
        {
            try
            {
                using (HJWarehouseEntities db = new HJWarehouseEntities())
                {
                    this.Warning = string.Empty;

                    // find existing record
                    var existingRecord = db.BINLOCAT.Where(m => m.ROWID == this.RowID).FirstOrDefault();
                    if (existingRecord == null)
                    {
                        this.Warning = "Location record not found";
                        return;
                    }

                    int newqty = 0;
                    if (!Int32.TryParse(this.NewQuantity.ToString(), out newqty))
                    {
                        this.Warning = "Quantity must be an integer";
                        return;
                    }

                    if (newqty <= 0)
                    {
                        this.Warning = "Quantity to Move cannot be less than or equal to 0";
                        return;
                    }

                    if (newqty > existingRecord.QUANTITY)
                    {
                        this.Warning = "Quantity to move exceeds existing (source) record";

                        return;
                    }

                    // find new item record
                    var binlocatITEMRecord = db.BINLOCAT.Where(m => m.PRODUCT == this.Product && m.PO_NUM == this.NewLot && m.BINLABEL == this.BinLabel).FirstOrDefault();
                    if (binlocatITEMRecord != null)
                    {
                        // Destination Record Found.  change item quantities only

                        // if new record has a different expiry then return with a warning.
                        if (binlocatITEMRecord.DATECREATE != this.ExpiryDate)
                        {
                            this.Warning = string.Format(CultureInfo.InvariantCulture,
                                "Unable to Move Bin Label to Lot {0} because it has an expiry of {1}"
                                , this.NewLot
                                , binlocatITEMRecord.DATECREATE.Value.ToString("ddd MMM d yyyy")
                                );

                            return;
                        }

                        // decrease quantity of existing record
                        existingRecord.QUANTITY -= newqty;
                        existingRecord.UNALLOC -= newqty;

                        // increase the quantity on new record
                        binlocatITEMRecord.QUANTITY += newqty;
                        binlocatITEMRecord.UNALLOC += newqty;

                        binlocatITEMRecord.DATE_TIME = DateTime.Now.ToString("yyyyMMdd hh:mm:ss.00");
                        binlocatITEMRecord.USER_ID = userID;
                    }
                    else
                    {
                        // no destination item record found.  make a new BINLOCAT record

                        var binlocatLOTRecord = db.BINLOCAT.AsNoTracking().Where(m => m.PRODUCT == this.Product && m.PO_NUM == this.NewLot).FirstOrDefault();
                        if (binlocatLOTRecord != null)
                        {
                            // existing record with product and lot found.

                            if (binlocatLOTRecord.DATECREATE != this.ExpiryDate)
                            {
                                this.Warning = string.Format(CultureInfo.InvariantCulture,
                                    "Unable to Move Bin Label to Lot {0} because it has an expiry of {1}"
                                    , this.NewLot
                                    , binlocatLOTRecord.DATECREATE.Value.ToString("ddd MMM d yyyy")
                                    );

                                return;
                            }

                            // if quantities identical (all quantities in play), then just change the lot number (PO_NUM)
                            if (this.Quantity == newqty)
                            {
                                existingRecord.PO_NUM = this.NewLot;
                                existingRecord.EXTENDED = existingRecord.EXTENDED.Replace(this.Lot, this.NewLot);

                                existingRecord.DATE_TIME = DateTime.Now.ToString("yyyyMMdd hh:mm:ss.00");
                                existingRecord.USER_ID = userID;
                            }
                            else
                            {
                                // create new BINLOCAT record (using AsNoTracking)
                                binlocatITEMRecord = db.BINLOCAT.AsNoTracking().Where(m => m.ROWID == this.RowID).FirstOrDefault();

                                binlocatITEMRecord.ROWID = Guid.NewGuid();
                                binlocatITEMRecord.PO_NUM = this.NewLot;
                                binlocatITEMRecord.DATECREATE = this.ExpiryDate;
                                binlocatITEMRecord.QUANTITY = newqty;
                                binlocatITEMRecord.UNALLOC = newqty;
                                binlocatITEMRecord.EXTENDED = binlocatITEMRecord.EXTENDED.Replace(this.Lot, this.NewLot);

                                binlocatITEMRecord.DATE_TIME = DateTime.Now.ToString("yyyyMMdd hh:mm:ss.00");
                                binlocatITEMRecord.USER_ID = userID;

                                db.BINLOCAT.Add(binlocatITEMRecord);

                                // decrease quantity of existing record
                                existingRecord.QUANTITY -= newqty;
                                existingRecord.UNALLOC -= newqty;
                            }
                        }
                        else
                        {
                            // create new product master record
                            var newProductRecord = db.PRODMSTR.AsNoTracking().Where(m => m.EXTENDED == existingRecord.EXTENDED).FirstOrDefault();
                            if (newProductRecord == null)
                            {
                                this.Warning = "Product Record Not Found";

                                return;
                            }

                            newProductRecord.ROWID = Guid.NewGuid();
                            newProductRecord.EXTENDED = newProductRecord.EXTENDED.Replace(this.Lot, this.NewLot);
                            newProductRecord.ATTRIBUTE2 = this.NewLot;

                            newProductRecord.DATE_MODFY = DateTime.Now;

                            // check if product master exists with new info
                            var existingProductRecord = db.PRODMSTR.AsNoTracking().Where(m => m.EXTENDED == newProductRecord.EXTENDED).FirstOrDefault();
                            if (existingProductRecord == null)
                            {
                                db.PRODMSTR.Add(newProductRecord);
                            }

                            // create new BINLOCAT record (using AsNoTracking)
                            binlocatITEMRecord = db.BINLOCAT.AsNoTracking().Where(m => m.ROWID == this.RowID).FirstOrDefault();

                            binlocatITEMRecord.ROWID = Guid.NewGuid();
                            binlocatITEMRecord.PO_NUM = this.NewLot;
                            binlocatITEMRecord.DATECREATE = this.ExpiryDate;
                            binlocatITEMRecord.QUANTITY = newqty;
                            binlocatITEMRecord.UNALLOC = newqty;
                            binlocatITEMRecord.EXTENDED = binlocatITEMRecord.EXTENDED.Replace(this.Lot, this.NewLot);

                            binlocatITEMRecord.DATE_TIME = DateTime.Now.ToString("yyyyMMdd hh:mm:ss.00");
                            binlocatITEMRecord.USER_ID = userID;

                            db.BINLOCAT.Add(binlocatITEMRecord);

                            // decrease quantity of existing record
                            existingRecord.QUANTITY -= newqty;
                            existingRecord.UNALLOC -= newqty;
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public string BinLabel { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string Lot { get; set; }

        public List<string> LotList { get; set; }

        public string NewLot { get; set; }

        public decimal NewQuantity { get; set; }

        public string Product { get; set; }

        public decimal Quantity { get; set; }

        public Guid RowID { get; set; }

        public string Warning { get; set; }
    }

    public class HighJumpUtilityBinLabel
    {
        public HighJumpUtilityBinLabel()
        {
            this.BinLabel = string.Empty;
            this.Quantity = 0;
            this.ExpiryDate = DateTime.Now;
            this.RowID = Guid.Empty;
        }

        public string BinLabel { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string Lot { get; set; }

        public decimal PackSize { get; set; }

        public decimal Quantity { get; set; }

        public string QuantityDisplay { get; set; }

        public Guid RowID { get; set; }
    }

    public class HighJumpUtilitySearch
    {
        private void getResultList(string productSearchText, string lotSearchText)
        {
            using (HJWarehouseEntities db = new HJWarehouseEntities())
            {
                var prodmasterlist = new List<PRODMSTR>();

                if (string.IsNullOrWhiteSpace(this.ProductSearchText))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.LotSearchText))
                {
                    prodmasterlist = db.PRODMSTR.AsNoTracking().Where(m => m.PRODUCT == this.ProductSearchText).OrderBy(m => m.ATTRIBUTE2).ToList();
                }
                else
                {
                    prodmasterlist = db.PRODMSTR.AsNoTracking().Where(m => m.PRODUCT == this.ProductSearchText && m.ATTRIBUTE2 == this.LotSearchText).OrderBy(m => m.ATTRIBUTE2).ToList();
                }

                this.ResultList = new List<HighJumpUtilityBinLabel>();

                foreach (var prodmaster in prodmasterlist)
                {
                    var binlocatlist = db.BINLOCAT.AsNoTracking().Where(m => m.EXTENDED == prodmaster.EXTENDED).OrderBy(m => m.PO_NUM).ThenBy(m => m.BINLABEL).ToList();
                    foreach (var binlocat in binlocatlist)
                    {
                        HighJumpUtilityBinLabel entry = new HighJumpUtilityBinLabel();

                        entry.RowID = binlocat.ROWID;
                        entry.BinLabel = binlocat.BINLABEL;

                        if (binlocat.EXTENDED.Contains('/'))
                        {
                            string[] extendedsplit = binlocat.EXTENDED.Split('/');
                            if (extendedsplit.Length == 2)
                            {
                                entry.Lot = extendedsplit[1].Trim();
                            }
                            else
                            {
                                entry.Lot = "No Lot";
                            }
                        }
                        else
                        {
                            entry.Lot = "No Lot";
                        }

                        if (binlocat.QUANTITY == null)
                        {
                            entry.Quantity = 0m;
                        }
                        else
                        {
                            decimal qty = 0;
                            if (!Decimal.TryParse(binlocat.QUANTITY.Value.ToString(), out qty))
                            {
                                entry.Quantity = 0;
                            }
                            else
                            {
                                entry.Quantity = qty;
                            }
                        }

                        if (binlocat.PACKSIZE == null)
                        {
                            entry.PackSize = 1m;
                        }
                        else
                        {
                            decimal packsize = 1;
                            if (!Decimal.TryParse(binlocat.PACKSIZE.Value.ToString(), out packsize))
                            {
                                entry.PackSize = 1;
                            }
                            else
                            {
                                entry.PackSize = packsize;
                            }
                        }

                        entry.QuantityDisplay = string.Format(CultureInfo.InvariantCulture, "{0}*{1}", Math.Round(entry.Quantity, 0), Math.Round(entry.PackSize, 0));

                        if (binlocat.DATECREATE == null)
                        {
                            entry.ExpiryDate = DateTime.Now;
                        }
                        else
                        {
                            DateTime dt;
                            if (!DateTime.TryParse(binlocat.DATECREATE.ToString(), out dt))
                            {
                                entry.ExpiryDate = DateTime.Now;
                            }
                            else
                            {
                                entry.ExpiryDate = dt;
                            }
                        }

                        this.ResultList.Add(entry);
                    }
                }
            }
        }

        private void setDefaultExpiryDate()
        {
            int iterator = 0;

            foreach (var entry in this.ResultList)
            {
                if (iterator++ == 0)
                {
                    this.ExpiryDate = entry.ExpiryDate;
                }
                else
                {
                    if (this.ExpiryDate < entry.ExpiryDate)
                    {
                        this.ExpiryDate = entry.ExpiryDate;
                    }
                }
            }
        }

        internal void ChangeAllExpiryDates(string userID)
        {
            getResultList(this.ProductSearchText, this.LotSearchText);

            using (HJWarehouseEntities db = new HJWarehouseEntities())
            {
                foreach (var entry in this.ResultList)
                {
                    var dbrow = db.BINLOCAT.Where(m => m.ROWID == entry.RowID).FirstOrDefault();
                    if (dbrow != null)
                    {
                        dbrow.DATECREATE = this.ExpiryDate;
                        dbrow.USER_ID = userID;

                        entry.ExpiryDate = this.ExpiryDate;
                    }
                }

                db.SaveChanges();
            }
        }

        internal void Init()
        {
            this.ProductSearchText = string.Empty;
            this.LotSearchText = string.Empty;
            this.ExpiryDate = DateTime.Now;
            this.ResultList = new List<HighJumpUtilityBinLabel>();

            using (HJWarehouseEntities db = new HJWarehouseEntities())
            {
                var refreshRecord = db.PRODMSTR.Where(m => m.ROWID == null).FirstOrDefault();
            }
        }

        internal void Search()
        {
            getResultList(this.ProductSearchText, this.LotSearchText);

            this.setDefaultExpiryDate();
        }

        public HighJumpUtilitySearch()
        {
            this.ProductSearchText = string.Empty;
            this.LotSearchText = string.Empty;
            this.ResultList = new List<HighJumpUtilityBinLabel>();
        }

        public DateTime ExpiryDate { get; set; }

        public string LotSearchText { get; set; }

        public string ProductSearchText { get; set; }

        public List<HighJumpUtilityBinLabel> ResultList { get; set; }
    }
}