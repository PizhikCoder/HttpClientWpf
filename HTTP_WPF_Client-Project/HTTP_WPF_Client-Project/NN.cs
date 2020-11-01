using System;
using Microsoft.ML.Data;
using Microsoft.ML;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace NNMethods
{
    class InputData
    {
        [LoadColumn(0)]
        public float MouseInfo;
        [LoadColumn(1)]
        public float KeyBoardInfo;
        [LoadColumn(2)]
        public float ProcessesInfo;
        [LoadColumn(3)]
        public float Label;
    }
    class OutPutData
    {
        [ColumnName("PredictedLabel")]
        public float PredictedLabels;
    }
    class NN
    {
        private static ITransformer modelGlobal = null;
        private static IDataView dataGlobal = null;
        private static DataViewSchema modelSchema;//Нужна для загрузки существующей модели

        static MLContext ml = new MLContext();
        private static void Training() //Получаем натренированную модель
        {
            var reader = ml.Data.CreateTextLoader<InputData>(separatorChar:',', hasHeader: false);
            IDataView data = reader.Load(Environment.CurrentDirectory + @"\NN-Data.txt");
            dataGlobal = data;


            var pipeline = ml.Transforms.Conversion.MapValueToKey("Label")
                .Append(ml.Transforms.Concatenate("Features", "MouseInfo", "KeyBoardInfo", "ProcessesInfo"))
                .Append(ml.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName:"Label", featureColumnName: "Features"))
                .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
             modelGlobal = pipeline.Fit(data);
        }
        public static int GetDataFromNN(float MouseInfoNew, float KeyBoardInfoNew, float ProcessesInfoNew)
        {
            float result;
            result = ml.Model.CreatePredictionEngine<InputData, OutPutData>(modelGlobal).Predict
                (
                new InputData
                {
                    MouseInfo = MouseInfoNew,
                    KeyBoardInfo = KeyBoardInfoNew,
                    ProcessesInfo = ProcessesInfoNew
                }
                ).PredictedLabels;
            return Convert.ToInt32(result);
        }

        public static void StartNNTraining()
        {
            if (File.Exists(Environment.CurrentDirectory + @"\model.zip"))
            {
                modelGlobal = ml.Model.Load(Environment.CurrentDirectory + @"\model.zip", out modelSchema);
            }
            else
            {
                Training();
                ml.Model.Save(modelGlobal, dataGlobal.Schema, Environment.CurrentDirectory + @"\model.zip");
            }
        }
    }
}
