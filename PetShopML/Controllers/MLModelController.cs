using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using PetShopML.Model;

namespace PetShopML.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    //[Authorize]
    public class MLModelController : ControllerBase
    {
        private readonly ILogger<MLModelController> _logger;
        private readonly PredictionEnginePool<SaleData, SalesPrediction> _predictionEnginePool;
        private readonly PredictionEnginePool<SaleData, ProductUnitTimeSeriesPrediction> _predictionTimeSeriesEnginePool;

        /// <summary>
        /// Initializes a new instance of the <see cref="MLModelController"/> class.
        /// </summary>
        /// <param name="logger">Logger for logging information, warnings, and errors.</param>
        /// <param name="predictionEnginePool">Prediction engine pool for sales data regression predictions.</param>
        /// <param name="predictionTimeSeriesEnginePool">Prediction engine pool for product time series predictions.</param>
        public MLModelController(
            ILogger<MLModelController> logger,
            PredictionEnginePool<SaleData, SalesPrediction> predictionEnginePool,
            PredictionEnginePool<SaleData, ProductUnitTimeSeriesPrediction> predictionTimeSeriesEnginePool
        )
        {

            _logger = logger;

            _predictionEnginePool = predictionEnginePool;

            _predictionTimeSeriesEnginePool = predictionTimeSeriesEnginePool;

        }


        /// <summary>
        /// Health check endpoint to verify the service is running.
        /// </summary>
        /// <returns>An Ok result with a health message.</returns>
        [HttpGet("ping")]
        public async Task<IActionResult> Ping()
        {

            await Task.FromResult(0);

            _logger.LogWarning("Ping completed");

            return Ok(new { message = "Health" });

        }


        /// <summary>
        /// Predicts the next sales value for a default SaleData object using the regression model.
        /// </summary>
        /// <returns>
        /// An Ok result containing the predicted sales value and the input SaleData object.
        /// </returns>
        /// <remarks>
        /// This method uses the "PershopSalesModel_V1" model from the PredictionEnginePool.
        /// </remarks>
        /// <exception cref="Exception">Thrown when an unexpected error occurs during prediction.</exception>
        [HttpGet("PredictDefault")]
        public async Task<IActionResult> Predict()
        {



            try

            {

                var saledata = new SaleData()
                {

                    productId = 988,

                    month = 11,

                    year = 2017,

                    avg = 41,

                    max = 225,

                    min = 4,

                    count = 26,

                    prev = 1094,

                    units = 1076,

                    //next = float.NaN

                };



                var engine = _predictionEnginePool.GetPredictionEngine("PershopSalesModel_V1");

                var prediction = engine.Predict(saledata);



                return Ok(new
                {
                    next = prediction.Score,

                    saledata = new
                    {

                        productId = saledata.productId,

                        month = saledata.month,

                        year = saledata.year,

                        avg = saledata.avg,

                        max = saledata.max,

                        min = saledata.min,

                        count = saledata.count,

                        prev = saledata.prev,

                        units = saledata.units,

                    }
                });

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");

                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });

            }

        }



        [HttpGet("PredictProduct988")]
        public async Task<IActionResult> PredictProduct988() 
        {
            try
            {
                var saledata = new SaleData()
                {
                    productId = 988,
                    month = 11,
                    year = 2017,
                    avg = 41,
                    max = 225,
                    min = 4,
                    count = 26,
                    prev = 1094,
                    units = 1076,
                    //next = float.NaN
                };

                //Invalid usecase for TimeSeries prediction engine
                //TimeSeriesPredictionEngine?
                //Fails here
                var engine = _predictionTimeSeriesEnginePool.GetPredictionEngine("Product988TimeSeries");
                var prediction = engine.Predict(saledata);

                return Ok(new
                {
                    prediction.ForecastedProductUnits,
                    prediction.ConfidenceLowerBound,
                    prediction.ConfidenceUpperBound,
                    saledata = new
                    {
                        productId = saledata.productId,
                        month = saledata.month,
                        year = saledata.year,
                        avg = saledata.avg,
                        max = saledata.max,
                        min = saledata.min,
                        count = saledata.count,
                        prev = saledata.prev,
                        units = saledata.units,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected internal error: {ex.Message}");
                return StatusCode(500, new { Error = new[] { ex.Message }, StatusCode = StatusCodes.Status500InternalServerError });
            }
        }
    }
}
