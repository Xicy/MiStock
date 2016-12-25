using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class Response
        {

            private static readonly Dictionary<short, string> statusCodeData = new Dictionary<short, string> {
                #region StatusCodes
                { 100,"100 Continue"},
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

            private static IDictionary<string, string> mimeTypeMappings = new Dictionary<string, string> { 
                #region MIME type list
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        #endregion
            };

            public string Content;
            public string ContentFileExtention;
            public short StatusCode;
            private Dictionary<string, string> ResponseHeader;

            public static Response ResponseFromFile(FileStream fileStream)
            {
                throw new Exception("Not Workink Now");
                return new Response(200, new StreamReader(fileStream, Encoding.UTF8).ReadToEnd(), Path.GetExtension(fileStream.Name));
            }

            public Response(short statusCode = 200, string content = "", string _contentFileExtention = ".html")
            {
                StatusCode = statusCode;
                Content = content;
                ContentFileExtention = _contentFileExtention;
                ResponseHeader = new Dictionary<string, string>
                {
                    {$"HTTP/1.1 {statusCodeData[StatusCode]}", null },
                    {"Date", DateTime.Now.ToString("r")},
                    {"Last-Modified", DateTime.Now.ToString("r")},
                    {"Server",  typeof(Bootstrap).Namespace},
                };
            }

            //TODO:Response
            public byte[] ResponseData()
            {
                if (Content.Length > 0)
                {
                    ResponseHeader.Add("Content-Type", mimeTypeMappings.ContainsKey(ContentFileExtention) ? mimeTypeMappings[ContentFileExtention] : "application/octet-stream");
                    ResponseHeader.Add("Content-Length", Content.Length.ToString());
                    ResponseHeader.Add("Connection", "Closed");
                    ResponseHeader.Add($"\r\n{Content}", null);
                }

                return Encoding.UTF8.GetBytes(ResponseHeader.Aggregate("",
                        (current, header) =>
                            current + header.Key + (header.Value != null ? $":{header.Value}" : "") + "\r\n"));
            }
        }
    }
}