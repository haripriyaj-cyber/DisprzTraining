using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Text;

namespace DisprzTraining.Controllers
{
    /// <summary>
    /// Controller for accessing OpenAPI documentation
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OpenApiController : ControllerBase
    {
        private readonly ISwaggerProvider _swaggerProvider;

        /// <summary>
        /// Initializes a new instance of the OpenApiController
        /// </summary>
        /// <param name="swaggerProvider">The Swagger provider</param>
        public OpenApiController(ISwaggerProvider swaggerProvider)
        {
            _swaggerProvider = swaggerProvider;
        }

        /// <summary>
        /// Gets the OpenAPI specification in YAML format
        /// </summary>
        /// <returns>The OpenAPI specification as YAML</returns>
        [HttpGet("yaml")]
        [Produces("text/yaml")]
        public IActionResult GetOpenApiYaml()
        {
            var swagger = _swaggerProvider.GetSwagger("v1");
            
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var yamlWriter = new OpenApiYamlWriter(stringWriter);
            
            swagger.SerializeAsV3(yamlWriter);
            
            return Content(stringBuilder.ToString(), "text/yaml");
        }
        
        /// <summary>
        /// Gets the OpenAPI specification in JSON format
        /// </summary>
        /// <returns>The OpenAPI specification as JSON</returns>
        [HttpGet("json")]
        [Produces("application/json")]
        public IActionResult GetOpenApiJson()
        {
            var swagger = _swaggerProvider.GetSwagger("v1");
            
            return Ok(swagger);
        }
    }
}
