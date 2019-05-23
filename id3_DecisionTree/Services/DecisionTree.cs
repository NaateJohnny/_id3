using id3_DecisionTree.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace id3_DecisionTree.Services
{
    public class DecisionTree
    {
        private DataTable DataDB { get; set; }
        private int TotalHigh { get; set; }
        private int TotalLow { get; set; }
        private int TotalModerate { get; set; }
        private int Total { get; set; }
        private string AttributeLabel { get; set; }
        private double Entropy { get; set; }



        /// <summary>
        ///     Constrói a árvore
        /// </summary>
        /// <param name="dataDb"></param>
        /// <param name="label"></param>
        /// <param name="attributesDb"></param>
        /// <returns></returns>
        public Node TreeBuild(DataTable dataDb, string label, AttributesData[] attributesDb)
        {
            this.DataDB = dataDb;
            return BuildInternalTee(this.DataDB, label, attributesDb);
        }

        /// <summary>
        ///     Constrói a árvore realizando a indução.
        /// </summary>
        /// <param name="dataDb"></param>
        /// <param name="label"></param>
        /// <param name="attributesDb"></param>
        /// <returns></returns>
        private Node BuildInternalTee(DataTable dataDb, string label, AttributesData[] attributesDb)
        {

            if (ChecksAllBelongsClassSame(dataDb, label).Count == 1)
                return new Node(new AttributesData(dataDb.Rows[0]));

            if (attributesDb.Length == 0)
                return new Node(new AttributesData(GetMostCommonValue(dataDb, label)));

            this.Total = dataDb.Rows.Count;
            this.AttributeLabel = label;
            this.TotalHigh = RiscTotal(dataDb, "alto");
            this.TotalLow = RiscTotal(dataDb, "baixo");
            this.TotalModerate = RiscTotal(dataDb, "moderado");

            this.Entropy = EntropyCalc(this.TotalHigh, this.TotalLow, this.TotalModerate);
            AttributesData melhorAtributo = GetAttributeBest(dataDb, attributesDb);

            Node root = new Node(melhorAtributo);

            DataTable data = dataDb.Clone();

            /// Predição
            foreach (var item in melhorAtributo.PropetiesValues)
            {
                /// Limpa a linha
                data.Rows.Clear();

                /// Seleciona todos os elementos com o valor deste atributo
                DataRow[] rows = dataDb.Select(melhorAtributo.Name + " = " + "'" + item.ToString() + "'");

                foreach (DataRow row in rows)
                {
                    data.Rows.Add(row.ItemArray);

                }

                /// Cria uma nova lista/partição de attributesDb menos o atributo corrente que é o melhor atributo
                ArrayList atrbts = new ArrayList(attributesDb.Length - 1);
                for (int _item = 0; _item < attributesDb.Length; _item++)
                {
                    if (attributesDb[_item].Name != melhorAtributo.Name)
                        atrbts.Add(attributesDb[_item]);
                }

                if (data.Rows.Count == 0)
                {
                    return new Node(new AttributesData(GetMostCommonValue(data, label)));
                }
                else
                {
                    DecisionTree id3 = new DecisionTree();
                    Node noFilho = id3.TreeBuild(data, label, (AttributesData[])atrbts.ToArray(typeof(AttributesData)));
                    root.NodeCreate(noFilho, item.ToString());
                }

            }

            return root;
        }

        /// <summary>
        ///     Verifica se todos pertecem a mesma classe
        /// </summary>
        /// <param name="dataDb"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        private ArrayList ChecksAllBelongsClassSame(DataTable dataDb, string label)
        {
            return GetDistinctValues(dataDb, label);
        }


        /// Retorna uma lista com todos os values distintos
        private ArrayList GetDistinctValues(DataTable dataDb, string label)
        {
            ArrayList DistinctValues = new ArrayList(dataDb.Rows.Count);

            foreach (DataRow row in dataDb.Rows)
            {
                if (DistinctValues.IndexOf(row[label]) == -1)
                    DistinctValues.Add(row[label]);
            }

            /// é feito o retorno, só retorna se for positivo
            return DistinctValues;
        }


        /// Retorna o valor mais comum dentro do DataDB(Base)
        private object GetMostCommonValue(DataTable dataDb, string label)
        {
            ArrayList DistinctValues = GetDistinctValues(dataDb, label);
            int[] contador = new int[DistinctValues.Count];

            foreach (DataRow row in dataDb.Rows)
            {
                int index = DistinctValues.IndexOf(row[label]);
                contador[index]++;
            }

            int maxIndex = 0;
            int maxCount = 0;

            for (int item = 0; item < contador.Length; item++)
            {
                if (contador[item] > maxCount)
                {
                    maxCount = contador[item];
                    maxIndex = item;
                }
            }

            return DistinctValues[maxIndex];

        }


        /// <summary>
        ///     Retorna o risco total
        /// </summary>
        /// <param name="dataDb"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        private int RiscTotal(DataTable dataDb, string label)
        {
            int result = 0;
            foreach (DataRow row in dataDb.Rows)
            {
                if ((string)row[AttributeLabel] == label)
                    result++;
            }

            return result;
        }


        /// <summary>
        ///     Realiza o cálcuro da entropia
        /// </summary>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="moderate"></param>
        /// <returns></returns>
        private double EntropyCalc(int high, int low, int moderate)
        {
            int total = high + low + moderate;
            double highProportion = (double)high / total;
            double lowProportion = (double)low / total;
            double moderateProportion = (double)moderate / total;


            if (highProportion != 0)
                highProportion = -(highProportion) * Math.Log(highProportion, 2);

            if (lowProportion != 0)
                lowProportion = -(lowProportion) * Math.Log(lowProportion, 2);

            if (moderateProportion != 0)
                moderateProportion = -(moderateProportion) * Math.Log(moderateProportion, 2);

            return highProportion + lowProportion + moderateProportion;
        }


        /// Retorna o melhor attribure
        private AttributesData GetAttributeBest(DataTable dataDb, AttributesData[] attributesDb)
        {
            double maximoGanho = 0.0;
            AttributesData result = null;

            foreach (AttributesData attribure in attributesDb)
            {
                double aux = GainCalc(dataDb, attribure);
                if (aux > maximoGanho)
                {
                    maximoGanho = aux;
                    result = attribure;
                }
            }

            return result;
        }


        /// <summary>
        ///     Calcula o ganho
        /// </summary>
        /// <param name="dataDb"></param>
        /// <param name="attribure"></param>
        /// <returns></returns>
        private double GainCalc(DataTable dataDb, AttributesData attribure)
        {
            ArrayList values = attribure.PropetiesValues;
            double soma = 0.0;

            for (int item = 0; item < values.Count; item++)
            {
                int high, low, moderate = 0;

                GetAttributeValues(dataDb, attribure, values[item].ToString(), out high, out low, out moderate);

                double entropia = EntropyCalc(high, low, moderate);
                soma += -(double)(high + low + moderate) / Total * entropia;
            }

            return this.Entropy + soma;
        }


        /// <summary>
        ///     Busca os valores do atributo
        /// </summary>
        /// <param name="dataDb"></param>
        /// <param name="attribure"></param>
        /// <param name="label"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="moderate"></param>
        private void GetAttributeValues(DataTable dataDb, AttributesData attribure, string label, out int high, out int low, out int moderate)
        {
            high = 0;
            low = 0;
            moderate = 0;

            foreach (DataRow row in dataDb.Rows)
            {
                if ((string)row[attribure.Name] == label)
                {
                    if ((string)row[AttributeLabel] == "alto")
                        high++;
                    else if ((string)row[AttributeLabel] == "baixo")
                        low++;
                    else if ((string)row[AttributeLabel] == "moderado")
                        moderate++;
                }
            }

        }

    }
}
