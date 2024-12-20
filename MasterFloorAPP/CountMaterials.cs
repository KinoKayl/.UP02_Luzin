using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterFloorAPP
{
    internal class CountMaterials
    {
        public int CountMaterial(int prodcutyTypeID, int materialTypeID, int productCount, double par1, double par2)
        {
            if (prodcutyTypeID <= 0 || materialTypeID <= 0 || productCount <= 0 || par1 <= 0 || par2 <= 0)
            {
                return -1;
            }

            var productType = GetProductType(prodcutyTypeID);
            if (productType == null)
            {
                return -1;
            }

            var materialType = GetMaterial_Type(materialTypeID);
            if (materialType == null)
            {
                return -1;
            }

            try
            {
                double materialUnit = par1 * par2 * Convert.ToDouble(productType.RatioTypeProduct);
                double totalMaterial = materialUnit * productCount;
                double defectMultiplier = 1 + ((double)materialType.ScrapRate / 100);
                totalMaterial += defectMultiplier;
                return (int)Math.Ceiling(totalMaterial);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private Product_type GetProductType(int id)
        {
            using (var context = new Entities())
            {
                return context.Product_type.FirstOrDefault(pt => pt.ID == id);
            }
        }

        private Material_type GetMaterial_Type(int id)
        {
            using (var context = new Entities())
            {
                return context.Material_type.FirstOrDefault(mt => mt.ID == id);
            }
        }
    }
}
