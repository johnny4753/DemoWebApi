using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using LINQtoCSV;

namespace DemoWebApi.Extension
{
    public static class ApiControllerExtension
    {
        public static CsvContentResult<T> Csv<T>(this ApiController controller, IEnumerable<T> entities, string fileName)
        {
            return new CsvContentResult<T>(entities, fileName);
        }
    }

    public class CsvContentResult<T> : IHttpActionResult
    {
        private readonly IEnumerable<T> _content;
        private readonly string _fileName;
        public CsvContentResult(IEnumerable<T> content, string fileName)
        {
            _content = content;
            _fileName = fileName;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms, Encoding.UTF8))
                {
                    var csvContext = new CsvContext(); //[LINQtoCSV]
                    csvContext.Write(_content, sw);
                    sw.Flush();
                    ms.Position = 0;
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ms.ToArray())
                    };
                    resp.Content.Headers.ContentType 
                        = new MediaTypeHeaderValue("application/x-excel");
                    resp.Content.Headers.ContentDisposition 
                        = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = _fileName
                    };
                    return Task.FromResult(resp);
                }
            }
        }
    }
}