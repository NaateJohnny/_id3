using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace id3_DecisionTree.Models
{
    public class AttributesData
    {
        //pagina 355 livro
        public ArrayList PropetiesValues { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }

        public AttributesData(string nameParam, ArrayList propetiesValuesParam)
        {
            this.Name = nameParam;
            this.PropetiesValues = propetiesValuesParam;
            this.PropetiesValues.Sort();
        }

        public AttributesData(object labelParam)
        {
            var labelList = (DataRow)labelParam;
            this.Label = labelList.ItemArray[0].ToString();
            this.Name = null;
            this.PropetiesValues = null;
        }


        public int IndexNumberValue(string nameParam)
        {
            if (PropetiesValues.Count > 0)
                return PropetiesValues.BinarySearch(nameParam);
            else
                return -1;
        }
    }
}
