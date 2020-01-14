using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using NHibernate;

namespace ConsoleApp
{
    public class MLRecommendation
    {
        private readonly IConfigurationKeys _configuration;

        public MLRecommendation(IConfigurationKeys configuration)
        {
            _configuration = configuration;
        }

        public void Do()
        {
            MLContext mlContext = new MLContext();
            (IDataView trainingDataView, IDataView testDataView) = LoadData(mlContext);
            ITransformer model = BuildAndTrainModel(mlContext, trainingDataView);
            EvaluateModel(mlContext, testDataView, model);
            UseModelForSinglePrediction(mlContext, model);
            SaveModel(mlContext, trainingDataView.Schema, model);
        }

        public  (IDataView training, IDataView test) LoadData(MLContext mlContext)
        {
            //   var trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-train.csv");
            // var testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-test.csv");

            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<CourseData>();
          //  string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=<YOUR-DB-FILEPATH>;Database=<YOUR-DB-NAME>;Integrated Security=True;Connect Timeout=30";

            string sqlCommand = @"Select uc.CourseId as CourseId , uc2.CourseId as CourseId2, 1 as Label from sb.UsersCourses uc 
join sb.UsersCourses uc2 on uc.UserId = uc2.UserId
where uc.CourseId != uc2.CourseId";


            string sqlCommand2 = @"Select uc.CourseId as CourseId , uc2.CourseId as CourseId2 , 1 as Label from sb.UsersCourses uc 
join sb.UsersCourses uc2 on uc.UserId = uc2.UserId where id = 638";

            DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, _configuration.Db.Db, sqlCommand);
            DatabaseSource dbSourceTraining = new DatabaseSource(SqlClientFactory.Instance, _configuration.Db.Db, sqlCommand2);
            IDataView trainingDataView = loader.Load(dbSource);
            IDataView testDataView = loader.Load(dbSourceTraining);

            return (trainingDataView, testDataView);
        }

        public  ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingDataView)
        {
            IEstimator<ITransformer> estimator =
                mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "CourseIdEncoded",
                    inputColumnName: "CourseId")
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "CourseId2Encoded", inputColumnName: "CourseId2"));
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "CourseIdEncoded",
                MatrixRowIndexColumnName = "CourseId2Encoded",
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

            Console.WriteLine("=============== Training the model ===============");
            ITransformer model = trainerEstimator.Fit(trainingDataView);

            return model;
        }

        public  void EvaluateModel(MLContext mlContext, IDataView testDataView, ITransformer model)
        {
            Console.WriteLine("=============== Evaluating the model ===============");
            var prediction = model.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");
            Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
            Console.WriteLine("RSquared: " + metrics.RSquared.ToString());
        }
        public  void UseModelForSinglePrediction(MLContext mlContext, ITransformer model)
        {
            Console.WriteLine("=============== Making a prediction ===============");
            var predictionEngine = mlContext.Model.CreatePredictionEngine<string, MovieRatingPrediction>(model);

           // var testInput = new MovieRating { userId = 6, movieId = 10 };

            var movieRatingPrediction = predictionEngine.Predict("economics");

            if (Math.Round(movieRatingPrediction.Score, 1) > 3.5)
            {
                Console.WriteLine("Movie is recommended for user");
            }
            else
            {
                Console.WriteLine("Movie  is not recommended for user");
            }
        }

        public  void SaveModel(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            var modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "MovieRecommenderModel.zip");

            Console.WriteLine("=============== Saving the model to a file ===============");
            mlContext.Model.Save(model, trainingDataViewSchema, modelPath);
        }
    }


    public class CourseData
    {
        
        public string CourseId;
        public string CourseId2;
        public float Label;
        
    }

    public class MovieRatingPrediction
    {
        public string Label;
        public float Score;
    }
}
