using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using SolTechnology.Avro;
using System.Text;

namespace Akot.PeanutButter.API.Web
{
    public class AvroMiddleware
    {
    }

    public class AvroInputFormatter : InputFormatter
    {
        public AvroInputFormatter()
        {
            this.SupportedMediaTypes.Clear();
            this.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/avro"));
            this.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/avro+schema"));
        }
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            await using MemoryStream ms = new();
            await context.HttpContext.Request.Body.CopyToAsync(ms);
            var type = context.ModelType;
            object result = AvroConvert.Deserialize(ms.ToArray(), type);
            return await InputFormatterResult.SuccessAsync(result);
        }
    }

    public class AvroWithSchemaOutputFormatter : OutputFormatter
    {
        private readonly CodecType _codec;
        public AvroWithSchemaOutputFormatter(CodecType codec = CodecType.Null)
        {
            _codec = codec;
            this.SupportedMediaTypes.Clear();
            this.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/avro+schema"));
        }
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var schema = AvroConvert.GenerateSchema(context.Object?.GetType());
            var avroBody = AvroConvert.Serialize(context.Object, _codec);
            var response = context.HttpContext.Response;
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(new object[]
            //{
            //    schema as object,
            //    avroBody as object,
            //});
            var json = $"[{schema},\"{Convert.ToBase64String(avroBody)}\"]";
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            response.ContentLength = jsonBytes.Length;
            await response.Body.WriteAsync(jsonBytes);
        }
    }

    public class AvroOutputFormatter : OutputFormatter
    {
        private readonly CodecType _codec;
        public AvroOutputFormatter(CodecType codec = CodecType.Null)
        {
            _codec = codec;
            this.SupportedMediaTypes.Clear();
            this.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/avro"));
        }
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var avroBody = AvroConvert.Serialize(context.Object, _codec);
            var response = context.HttpContext.Response;
            response.ContentLength = avroBody.Length;
            await response.Body.WriteAsync(avroBody);
        }
    }

    public static class HttpClientAvroExtensions
    {
        public static async Task<HttpResponseMessage> PostAsAvro(this HttpClient httpClient, string requestUri, object content)
        {
            var body = new ByteArrayContent(AvroConvert.Serialize(content));
            body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/avro");
            return await httpClient.PostAsync(requestUri, body);
        }
        public static async Task<T> GetAsAvro<T>(this HttpClient httpClient, string requestUri)
        {
            var response = await httpClient.GetByteArrayAsync(requestUri);
            T result = AvroConvert.Deserialize<T>(response);
            return result;
        }
    }
}
