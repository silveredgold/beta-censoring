using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BetaCensor.Server
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PreferYamlFilter : Attribute, IResultFilter {

        public void OnResultExecuted(ResultExecutedContext context) {
            
        }

        public void OnResultExecuting(ResultExecutingContext context) {
            if (context.Result is ObjectResult objResult) {
                objResult.Formatters.Insert(0, new YamlFormatter());
            }
        }
    }
    public class YamlFormatter : TextOutputFormatter {
        private readonly ISerializer _serializer;

        public YamlFormatter()
        {
            SupportedMediaTypes.Add("text/yaml");
            SupportedMediaTypes.Add("application/yaml");
            SupportedMediaTypes.Add("application/x-yaml");
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            var serializer = new SerializerBuilder()
                .WithNamingConvention(new PascalCaseNamingConvention())
                .Build();
            _serializer = serializer;
        }

        

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding) {
            if (context.Object != null) {
                var httpContext = context.HttpContext;
                var serviceProvider = httpContext.RequestServices;
                var yaml = _serializer.Serialize(context.Object);
                await httpContext.Response.WriteAsync(yaml, selectedEncoding);
            }
        }
    }

    public static class YamlFormatterExtensions {
        public static MvcOptions AddYamlFormatter(this MvcOptions builder) {
            builder.OutputFormatters.Add(new YamlFormatter());
            return builder;
        }
    }
}