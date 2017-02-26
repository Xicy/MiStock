using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class Response
        {

            private static readonly IDictionary<short, string> StatusCodeData = new Dictionary<short, string> {
                #region StatusCodes
                {100,"100 Continue"},
                {101,"101 Switching Protocols"},
                {102,"102 Processing"},
                {200,"200 OK"},
                {201,"201 Created"},
                {202,"202 Accepted"},
                {203,"203 Non-authoritative Information"},
                {204,"204 No Content"},
                {205,"205 Reset Content"},
                {206,"206 Partial Content"},
                {207,"207 Multi-Status"},
                {208,"208 Already Reported"},
                {226,"226 IM Used"},
                {300,"300 Multiple Choices"},
                {301,"301 Moved Permanently"},
                {302,"302 Found"},
                {303,"303 See Other"},
                {304,"304 Not Modified"},
                {305,"305 Use Proxy"},
                {307,"307 Temporary Redirect"},
                {308,"308 Permanent Redirect"},
                {400,"400 Bad Request"},
                {401,"401 Unauthorized"},
                {402,"402 Payment Required"},
                {403,"403 Forbidden"},
                {404,"404 Not Found"},
                {405,"405 Method Not Allowed"},
                {406,"406 Not Acceptable"},
                {407,"407 Proxy Authentication Required"},
                {408,"408 Request Timeout"},
                {409,"409 Conflict"},
                {410,"410 Gone"},
                {411,"411 Length Required"},
                {412,"412 Precondition Failed"},
                {413,"413 Payload Too Large"},
                {414,"414 Request-URI Too Long"},
                {415,"415 Unsupported Media Type"},
                {416,"416 Requested Range Not Satisfiable"},
                {417,"417 Expectation Failed"},
                {418,"418 I'm a teapot"},
                {421,"421 Misdirected Request"},
                {422,"422 Unprocessable Entity"},
                {423,"423 Locked"},
                {424,"424 Failed Dependency"},
                {426,"426 Upgrade Required"},
                {428,"428 Precondition Required"},
                {429,"429 Too Many Requests"},
                {431,"431 Request Header Fields Too Large"},
                {444,"444 Connection Closed Without Response"},
                {451,"451 Unavailable For Legal Reasons"},
                {499,"499 Client Closed Request"},
                {500,"500 Internal Server Error"},
                {501,"501 Not Implemented"},
                {502,"502 Bad Gateway"},
                {503,"503 Service Unavailable"},
                {504,"504 Gateway Timeout"},
                {505,"505 HTTP Version Not Supported"},
                {506,"506 Variant Also Negotiates"},
                {507,"507 Insufficient Storage"},
                {508,"508 Loop Detected"},
                {510,"510 Not Extended"},
                {511,"511 Network Authentication Required"},
                {599,"599 Network Connect Timeout Error"}
                #endregion
            };

            private static readonly IDictionary<string, string> MimeTypeMapData = new Dictionary<string, string> { 
                #region MIME type list
                {"asf", "video/x-ms-asf"},
                {"asx", "video/x-ms-asf"},
                {"avi", "video/x-msvideo"},
                {"cco", "application/x-cocoa"},
                {"crt", "application/x-x509-ca-cert"},
                {"css", "text/css"},
                {"der", "application/x-x509-ca-cert"},
                {"ear", "application/java-archive"},
                {"flv", "video/x-flv"},
                {"gif", "image/gif"},
                {"hqx", "application/mac-binhex40"},
                {"htc", "text/x-component"},
                {"htm", "text/html"},
                {"html", "text/html"},
                {"ico", "image/x-icon"},
                {"jar", "application/java-archive"},
                {"jardiff", "application/x-java-archive-diff"},
                {"jng", "image/x-jng"},
                {"jnlp", "application/x-java-jnlp-file"},
                {"jpeg", "image/jpeg"},
                {"jpg", "image/jpeg"},
                {"js", "application/x-javascript"},
                {"mml", "text/mathml"},
                {"mng", "video/x-mng"},
                {"mov", "video/quicktime"},
                {"mp3", "audio/mpeg"},
                {"mp4", "video/mpeg"},
                {"mpeg", "video/mpeg"},
                {"mpg", "video/mpeg"},
                {"pdb", "application/x-pilot"},
                {"pdf", "application/pdf"},
                {"pem", "application/x-x509-ca-cert"},
                {"pl", "application/x-perl"},
                {"pm", "application/x-perl"},
                {"png", "image/png"},
                {"prc", "application/x-pilot"},
                {"ra", "audio/x-realaudio"},
                {"rar", "application/x-rar-compressed"},
                {"rpm", "application/x-redhat-package-manager"},
                {"rss", "text/xml"},
                {"run", "application/x-makeself"},
                {"sea", "application/x-sea"},
                {"shtml", "text/html"},
                {"sit", "application/x-stuffit"},
                {"swf", "application/x-shockwave-flash"},
                {"tcl", "application/x-tcl"},
                {"tk", "application/x-tcl"},
                {"txt", "text/plain"},
                {"war", "application/java-archive"},
                {"wbmp", "image/vnd.wap.wbmp"},
                {"wmv", "video/x-ms-wmv"},
                {"xml", "text/xml"},
                {"xpi", "application/x-xpinstall"},
                {"zip", "application/zip"},
                #endregion
            };


            public string ContentFileExtention;
            public short StatusCode;

            private IDictionary<string, string> _responseHeader;
            private CookieCollection _cookies;
            private BufferedStream _contentStream;

            public Response()
            {
                _cookies = new CookieCollection();
            }

            public Response SetContent(Stream stream)
            {
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));

                _contentStream = new BufferedStream(stream, BufferSize);
                if (stream is FileStream)
                {
                    SetFileExtention(Path.GetExtension(((FileStream)stream).Name).TrimStart('.'));
                }
                return this;
            }

            public Response SetStatusCode(short statusCode)
            {
                StatusCode = statusCode;
                return this;
            }

            public Response SetFileExtention(string contentFileExtention)
            {
                ContentFileExtention = contentFileExtention;
                return this;
            }

            public Response AddCookie(CookieContainer cookie)
            {
                _cookies.Add(cookie);
                return this;
            }

            public void SendResponseData(Stream writerStream)
            {
                _responseHeader = new Dictionary<string, string>
                {
                    {$"HTTP/1.1 {(StatusCodeData.ContainsKey(StatusCode) ? StatusCodeData[StatusCode] : StatusCode.ToString())}", null },
                    {"Date", DateTime.Now.ToString("r")},
                    {"Last-Modified", DateTime.Now.ToString("r")},
                    {"Server",  typeof(Bootstrap).Namespace},
                    {"Access-Control-Allow-Origin","*" }
                };

                if (_contentStream != null && _contentStream.CanRead && _contentStream.CanSeek && _contentStream.Length > 0)
                {
                    _responseHeader.Add("Content-Type", MimeTypeMapData.ContainsKey(ContentFileExtention) ? MimeTypeMapData[ContentFileExtention] : "application/octet-stream");
                    _responseHeader.Add("Content-Length", _contentStream.Length.ToString());
                }

                if (!_cookies.IsNull())
                {
                    _responseHeader.Add(_cookies.ToResponseData(), null);
                }

                _responseHeader.Add("Connection", "Keep-Alive");
                _responseHeader.Add(String.Empty, null);

                var retBytes = Encoding.UTF8.GetBytes(_responseHeader.Aggregate("", (current, header) => current + header.Key + (header.Value != null ? $":{header.Value}" : "") + Environment.NewLine)).ToArray();

                writerStream.Write(retBytes, 0, retBytes.Length);

                byte[] buffer = new byte[BufferSize];
                while (_contentStream != null && _contentStream.Position < _contentStream.Length)
                {
                    var readedSize = _contentStream.Read(buffer, 0, BufferSize);
                    writerStream.Write(buffer, 0, readedSize);
                    Task.Delay(100).Wait();
                }
            }

            #region Disposing

            protected virtual void Dispose(bool disposing)
            {
                _contentStream?.Dispose();
                ContentFileExtention = null;
                _cookies.Clear();
                _responseHeader.Clear();
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            ~Response()
            {
                Dispose(false);
            }
            #endregion
        }
    }
}