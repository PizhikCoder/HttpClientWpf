using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.EntryPoints;

namespace NNLibrary
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
    class OutPutValue
    {
        [ColumnName("PredictedLabel")]
        public float PredictedLabels;
    }
    public class Class1
    {
        static MLContext ml = new MLContext(); //создаем среду ML.Net
        private static TransformerChain<Microsoft.ML.Transforms.Conversions.KeyToValueMappingTransformer> Training()
        {
            var reader = ml.Data.CreateTextReader<InputData>(separatorChar: ',', hasHeader: false); /* Создаем записыватель, котрый выгружает
            данные по формау IrisData, рзделяя значения по запятым и указывает, что заголовок у значений отсутствует*/
            Microsoft.ML.Data.IDataView data = reader.Read(Environment.CurrentDirectory + @"\NN-Data.txt");//Загружаем данные из текстового файла

            var pipeline = ml.Transforms.Conversion.MapValueToKey("Label")// создаем первый столбец(ключ)(это данные результаты для обучения)
                .Append(ml.Transforms.Concatenate("Features", "MouseInfo", "KeyBoardInfo", "ProcessesInfo"))//объединяем столбцы "Mouse", "KeyBoard", "Processes" в столбец Features(особенности) (если правилльно понимаю, все эти данные под столбцом CorrectOutput)
                .Append(ml.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(labelColumn: "Label", featureColumn: "Features"))// задаем алгоритм тренировки, в нашем случае "Стохастический двухкоординатный подъем", на основе тех значений, ктоторые должны получиться и особенностей(заданных значений)(тем самым заканчиванием формирование столбцов с вводными данными)
                .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));//задаем выходной столбец
            var model = pipeline.Fit(data);//Обучаем модель на основе наших данных
            return model;
        }
        public static float LaunchNN(float MouseInfoNew, float KeyBoardInfoNew, float ProcessesInfoNew)
        {
            var model = Training();
            var prediction = model.CreatePredictionEngine<InputData, OutPutValue>(ml).Predict/*Где InputData- класс определяющий входные данные, OutPutValue - класс определяющий выходные  */
                (
                new InputData
                {
                    MouseInfo = MouseInfoNew,
                    KeyBoardInfo = KeyBoardInfoNew,
                    ProcessesInfo = ProcessesInfoNew
                }
                );
            return prediction.PredictedLabels;
        }
    }
}
