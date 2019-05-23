using System;
using System.Data;
using Exec_Id3.Cli.Extensions;
using id3_DecisionTree.Data;
using id3_DecisionTree.Models;
using id3_DecisionTree.Services;

namespace Exec_Id3.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            /// Inicia os atriburos
            AttributesData[] attributes = ContextDataDB.InitAttributes();

            /// Busca a base de dados
            DataTable dataDB = ContextDataDB.GetDataBase();

            DecisionTree id3_decisionTree = new DecisionTree();

            Node root = id3_decisionTree.TreeBuild(dataDB, "risco", attributes);

            Utils.Render(root, "");
            Console.ReadKey();
        }
    }
}
