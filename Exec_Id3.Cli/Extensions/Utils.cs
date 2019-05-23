using System;
using System.Collections.Generic;
using System.Text;
using id3_DecisionTree.Models;

namespace Exec_Id3.Cli.Extensions

{
    public class Utils
    {
        public static void Render(Node root, string tabs)
        {
            Console.WriteLine(tabs + '|' + root.AttributeData.Name + '|');

            if (root.AttributeData.PropetiesValues != null)
            {
                for (int i = 0; i < root.AttributeData.PropetiesValues.Count; i++)
                {
                    Console.WriteLine(tabs + "\t" + "<" + root.AttributeData.PropetiesValues[i] + ">");
                    Node childrenNode = root.GetBranchChildren(root.AttributeData.PropetiesValues[i].ToString());
                    Render(childrenNode, "\t" + tabs);

                }
            }
            else
            {
                Console.WriteLine(tabs + "\t" + "[" + root.AttributeData.Description + "]");
            }
        }
    }
}
