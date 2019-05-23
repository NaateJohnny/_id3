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
        private DataTable Amostras { get; set; }
        private int TotalHigh { get; set; }
        private int TotalLow { get; set; }
        private int TotalModerate { get; set; }
        private int Total { get; set; }
        private string AttributeDescription { get; set; }
        private double Entropy { get; set; }

        /// <summary>
        ///     Retorna o risco toral
        /// </summary>
        /// <param name="dataDb"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private int RiscTotal(DataTable dataDb, string description)
        {
            int result = 0;
            foreach (DataRow row in dataDb.Rows)
            {
                if ((string)row[AttributeDescription] == description)
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

        /// <summary>
        ///     Busca os valores do atributo
        /// </summary>
        /// <param name="dataDb"></param>
        /// <param name="attribure"></param>
        /// <param name="description"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="moderate"></param>
        private void GetAttributeValues(DataTable dataDb, AttributesData attribure, string description, out int high, out int low, out int moderate)
        {
            high = 0;
            low = 0;
            moderate = 0;

            foreach (DataRow row in dataDb.Rows)
            {
                if ((string)row[attribure.Name] == description)
                {
                    if ((string)row[AttributeDescription] == "alto")
                        high++;
                    else if ((string)row[AttributeDescription] == "baixo")
                        low++;
                    else if ((string)row[AttributeDescription] == "moderado")
                        moderate++;
                }
            }

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
        ///     Verifica se todos pertecem a mesma classe
        /// </summary>
        /// <param name="dataDb"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private ArrayList ChecksAllBelongsClassSame(DataTable dataDb, string description)
        {
            return GetDistinctValues(dataDb, description);
        }

        /// retorna uma lista com todos os values distintos
        private ArrayList GetDistinctValues(DataTable dataDb, string description)
        {
            ArrayList DistinctValues = new ArrayList(dataDb.Rows.Count);

            foreach (DataRow row in dataDb.Rows)
            {
                if (DistinctValues.IndexOf(row[description]) == -1)
                    DistinctValues.Add(row[description]);
            }

            // é feito o retorno, só retorna se for positivo
            return DistinctValues;
        }

        //retorna o valor mais comum dentro do DataDB
        private object GetMostCommonValue(DataTable dataDb, string description)
        {
            ArrayList DistinctValues = GetDistinctValues(dataDb, description);
            int[] contador = new int[DistinctValues.Count];

            foreach (DataRow row in dataDb.Rows)
            {
                int index = DistinctValues.IndexOf(row[description]);
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

        private Node BuildInternalTee(DataTable dataDb, string description, AttributesData[] attributesDb)
        {

            if (ChecksAllBelongsClassSame(dataDb, description).Count == 1)
                return new Node(new AttributesData(dataDb.Rows[0]));

            if (attributesDb.Length == 0)
                return new Node(new AttributesData(GetMostCommonValue(dataDb, description)));

            this.Total = dataDb.Rows.Count;
            this.AttributeDescription = description;
            this.TotalHigh = RiscTotal(dataDb, "alto");
            this.TotalLow = RiscTotal(dataDb, "baixo");
            this.TotalModerate = RiscTotal(dataDb, "moderado");

            this.Entropy = EntropyCalc(this.TotalHigh, this.TotalLow, this.TotalModerate);
            AttributesData melhorAtributo = GetAttributeBest(dataDb, attributesDb);

            Node raiz = new Node(melhorAtributo);

            DataTable data = dataDb.Clone();

            foreach (var item in melhorAtributo.PropetiesValues)
            {

                /// Seleciona todas os elementos com o valor deste attribure
                data.Rows.Clear();
                DataRow[] rows = dataDb.Select(melhorAtributo.Name + " = " + "'" + item.ToString() + "'");

                foreach (DataRow row in rows)
                {
                    data.Rows.Add(row.ItemArray);

                }

                /// Cria uma nova lista/patição de attributesDb menos o attribure corrente que é o melhor attribure
                ArrayList atrbts = new ArrayList(attributesDb.Length - 1);
                for (int _item = 0; _item < attributesDb.Length; _item++)
                {
                    if (attributesDb[_item].Name != melhorAtributo.Name)
                        atrbts.Add(attributesDb[_item]);
                }

                if (data.Rows.Count == 0)
                {
                    return new Node(new AttributesData(GetMostCommonValue(data, description)));
                }
                else
                {
                    DecisionTree id3 = new DecisionTree();
                    Node noFilho = id3.TreeBuild(data, description, (AttributesData[])atrbts.ToArray(typeof(AttributesData)));
                    raiz.NodeCreate(noFilho, item.ToString());
                }

            }

            return raiz;
        }

        public Node TreeBuild(DataTable dataDb, string description, AttributesData[] attributesDb)
        {
            this.Amostras = dataDb;
            return BuildInternalTee(this.Amostras, description, attributesDb);
        }


    }
}
