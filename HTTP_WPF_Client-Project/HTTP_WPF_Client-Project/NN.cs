using System;
using Microsoft.ML.Data;
using Microsoft.ML;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using HTTP_WPF_Client_Project;

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
            App.CreateJournalLines("*Начат процесс загрузки данных для обучения модели нейронной сети*");
            IDataView data = reader.Load(Environment.CurrentDirectory + @"\NN-Data.txt");
            App.CreateJournalLines("*Завершен процесс загрузки данных для обучения модели нейронной сети*");
            dataGlobal = data;


            var pipeline = ml.Transforms.Conversion.MapValueToKey("Label")
                .Append(ml.Transforms.Concatenate("Features", "MouseInfo", "KeyBoardInfo", "ProcessesInfo"))
                .Append(ml.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName:"Label", featureColumnName: "Features"))
                .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            App.CreateJournalLines("*Идет обучение...*");
            modelGlobal = pipeline.Fit(data);
        }
        public static float GetDataFromNN(float MouseInfoNew, float KeyBoardInfoNew, float ProcessesInfoNew)
        {
            float result;
            App.CreateJournalLines("*Данные загружаются в нейронную сеть...*");
            result = ml.Model.CreatePredictionEngine<InputData, OutPutData>(modelGlobal).Predict
                (
                new InputData
                {
                    MouseInfo = MouseInfoNew,
                    KeyBoardInfo = KeyBoardInfoNew,
                    ProcessesInfo = ProcessesInfoNew
                }
                ).PredictedLabels;
            App.CreateJournalLines("*Результат получен*");
            return result;
        }

        public static void StartNNTraining()
        {
            if (File.Exists(Environment.CurrentDirectory + @"\model.zip"))//Проверка, существует ли обученная модель в корневой папке или нет, если нет, то начинается обучение модели и затем сохраняется готовая модель
            {
                App.CreateJournalLines("*Начата загрузка модели нейронной сети*");
                modelGlobal = ml.Model.Load(Environment.CurrentDirectory + @"\model.zip", out modelSchema);
                App.CreateJournalLines("*Модель нейронной сети загружена*");
            }
            else
            {
                App.CreateJournalLines("*Начат процесс обучения модели нейронной сети*");
                Training();
                App.CreateJournalLines("*Процесс обучения модели нейронной сети завершен*");
                ml.Model.Save(modelGlobal, dataGlobal.Schema, Environment.CurrentDirectory + @"\model.zip");
                App.CreateJournalLines("*Обученная модель сохранена в корневую папку*");
            }
        }
    }
}
