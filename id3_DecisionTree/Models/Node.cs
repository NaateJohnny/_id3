using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace id3_DecisionTree.Models
{
    public class Node
    {
        public ArrayList Children { get; set; }
        public AttributesData AttributeData { get; set; }

        public Node(AttributesData dataPropeties)
        {
            if (dataPropeties.PropetiesValues != null)
            {
                Children = new ArrayList(dataPropeties.PropetiesValues.Count);
                for (int i = 0; i < dataPropeties.PropetiesValues.Count; i++)
                    Children.Add(null);
            }
            else
            {
                ArrayList arrayList = new ArrayList(1);
                Children = arrayList;
                Children.Add(null);
            }

            this.AttributeData = dataPropeties;
        }

        public void NodeCreate(Node nodeParam, string nameParam)
        {
            int index = AttributeData.IndexNumberValue(nameParam);
            Children[index] = nodeParam;
        }

        public Node GetChildren(int index)
        {
            return (Node)Children[index];
        }


        public Node GetBranchChildren(string branch)
        {
            return (Node)Children[AttributeData.IndexNumberValue(branch)];
        }
    }
}
