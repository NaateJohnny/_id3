using id3_DecisionTree.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace id3_DecisionTree.Data
{
    public class ContextDataDB
    {
        public static AttributesData[] InitAttributes()
        {
            AttributesData risco = new AttributesData("risco", new ArrayList { "alto", "moderado", "baixo" });
            AttributesData historico_credito = new AttributesData("historico_credito", new ArrayList { "ruim", "desconhecida", "boa" });
            AttributesData divida = new AttributesData("divida", new ArrayList { "alta", "baixa" });
            AttributesData garantia = new AttributesData("garantia", new ArrayList { "nenhuma", "adequada" });
            AttributesData renda = new AttributesData("renda", new ArrayList { "$0 a $15 mil", "$15 a $35 mil", "acima de $35 mil" });

            return new AttributesData[] { historico_credito, divida, garantia, renda };
        }

        public static DataTable GetDataBase()
        {
            DataTable baseDB = new DataTable("Base_DB_ID3");

            DataColumn column;

            column = baseDB.Columns.Add("risco");
            column.DataType = typeof(string);

            column = baseDB.Columns.Add("historico_credito");
            column.DataType = typeof(string);

            column = baseDB.Columns.Add("divida");
            column.DataType = typeof(string);

            column = baseDB.Columns.Add("garantia");
            column.DataType = typeof(string);

            column = baseDB.Columns.Add("renda");
            column.DataType = typeof(string);

            baseDB.Rows.Add(new object[] { "alto", "ruim", "alta", "nenhuma", "$0 a $15 mil" });
            baseDB.Rows.Add(new object[] { "alto", "desconhecida", "alta", "nenhuma", "$15 a $35 mil" });
            baseDB.Rows.Add(new object[] { "moderado", "desconhecida", "baixa", "nenhuma", "$15 a $35 mil" });
            baseDB.Rows.Add(new object[] { "alto", "desconhecida", "baixa", "nenhuma", "$0 a $15 mil" });
            baseDB.Rows.Add(new object[] { "baixo", "desconhecida", "baixa", "nenhuma", "acima de $35 mil" });
            baseDB.Rows.Add(new object[] { "baixo", "desconhecida", "baixa", "adequada", "acima de $35 mil" });
            baseDB.Rows.Add(new object[] { "alto", "ruim", "baixa", "nenhuma", "$0 a $15 mil" });
            baseDB.Rows.Add(new object[] { "moderado", "ruim", "baixa", "adequada", "acima de $35 mil" });
            baseDB.Rows.Add(new object[] { "baixo", "boa", "baixa", "nenhuma", "acima de $35 mil" });
            baseDB.Rows.Add(new object[] { "baixo", "boa", "alta", "adequada", "acima de $35 mil" });
            baseDB.Rows.Add(new object[] { "alto", "boa", "alta", "nenhuma", "$0 a $15 mil" });
            baseDB.Rows.Add(new object[] { "moderado", "boa", "alta", "nenhuma", "$15 a $35 mil" });
            baseDB.Rows.Add(new object[] { "baixo", "boa", "alta", "nenhuma", "acima de $35 mil" });
            baseDB.Rows.Add(new object[] { "alto", "ruim", "alta", "nenhuma", "$15 a $35 mil" });

            return baseDB;
        }
    }
}
