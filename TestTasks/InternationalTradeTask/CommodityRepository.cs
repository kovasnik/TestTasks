using System;
using System.Collections.Generic;
using System.Linq;
using TestTasks.InternationalTradeTask.Models;

namespace TestTasks.InternationalTradeTask
{
    public class CommodityRepository
    {
        public double GetImportTariff(string commodityName)
        {
            var commodity = FindCommodity(commodityName, _allCommodityGroups);
            if (commodity == null)
            {
                throw new ArgumentException($"Commodity '{commodityName}' not found.");
            }

            while (commodity != null)
            {
                var tariff = commodity.ImportTarif;

                if (tariff.HasValue) return tariff.Value;

                commodity = FindParent(_allCommodityGroups, commodity);
            }
            return 0;
        }

        public double GetExportTariff(string commodityName)
        {
            var commodity = FindCommodity(commodityName, _allCommodityGroups);
            if (commodity == null)
            {
                throw new ArgumentException($"Commodity '{commodityName}' not found.");
            }

            while (commodity != null)
            {
                var tariff = commodity.ExportTarif;

                if (tariff.HasValue) return tariff.Value;

                commodity = FindParent(_allCommodityGroups, commodity);
            }

            return 0;
        }

        private ICommodityGroup FindParent(IEnumerable<ICommodityGroup> commodityGroups, ICommodityGroup commodity)
        {
            foreach(var group in commodityGroups)
            {
                if (group.SubGroups?.Contains(commodity) == true)
                {
                    return group;
                }

                var parentCommunity = FindParent(group.SubGroups ?? Array.Empty<ICommodityGroup>(), commodity);

                if (parentCommunity != null) return parentCommunity;
            }
            return null;
        }

        private ICommodityGroup FindCommodity(string name, IEnumerable<ICommodityGroup> groups)
        {
            foreach (var group in groups)
            {
                if (group.Name == name) return group;
                if (group.SubGroups != null)
                {
                    var found = FindCommodity(name, group.SubGroups);
                    if (found != null) return found;
                }
            }
            return null;
        }

        private FullySpecifiedCommodityGroup[] _allCommodityGroups = new FullySpecifiedCommodityGroup[]
        {
            new FullySpecifiedCommodityGroup("06", "Sugar, sugar preparations and honey", 0.05, 0)
            {
                SubGroups = new CommodityGroup[]
                {
                    new CommodityGroup("061", "Sugar and honey")
                    {
                        SubGroups = new CommodityGroup[]
                        {
                            new CommodityGroup("0611", "Raw sugar,beet & cane"),
                            new CommodityGroup("0612", "Refined sugar & other prod.of refining,no syrup"),
                            new CommodityGroup("0615", "Molasses", 0, 0),
                            new CommodityGroup("0616", "Natural honey", 0, 0),
                            new CommodityGroup("0619", "Sugars & syrups nes incl.art.honey & caramel"),
                        }
                    },
                    new CommodityGroup("062", "Sugar confy, sugar preps. Ex chocolate confy", 0, 0)
                }
            },
            new FullySpecifiedCommodityGroup("282", "Iron and steel scrap", 0, 0.1)
            {
                SubGroups = new CommodityGroup[]
                {
                    new CommodityGroup("28201", "Iron/steel scrap not sorted or graded"),
                    new CommodityGroup("28202", "Iron/steel scrap sorted or graded/cast iron"),
                    new CommodityGroup("28203", "Iron/steel scrap sort.or graded/tinned iron"),
                    new CommodityGroup("28204", "Rest of 282.0")
                }
            }
        };
    }
}
