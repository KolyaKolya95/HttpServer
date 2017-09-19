using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerNet
{
    public class Response
    {
        private Byte[] data = null;
        private String status;
        private String mime;

        private Response(String status, String mime, Byte[] data)
        {
            this.status = status;
            this.data = data;
            this.mime = mime;
        }

        public static Response From(Request request)
        {
            if (request == null)
                return MakeNullRequest();

            if (request.Type == "GET")
            {
                String file = Environment.CurrentDirectory + HttpServer.WEB_DIR + request.URL;
                if (File.Exists(file))
                {
                    return null;
                }
                else
                {
                    return null;
                }
            }
            else
                return MakeMathodNotAllowed();

        }

        private static Response MakeNullRequest()
        {
            String file = Environment.CurrentDirectory + HttpServer.MSG_DIR + "400.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] d = new Byte[fileStream.Length];
            reader.Read(d, 0, d.Length);


            return new Response("400 bad request", "html/text", d);
        }

        private static Response MakeMathodNotAllowed()
        {
            String file = Environment.CurrentDirectory + HttpServer.MSG_DIR + "405.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] d = new Byte[fileStream.Length];
            reader.Read(d, 0, d.Length);


            return new Response("405 bad request", "html/text", d);
        }

        private static Response MakeNotFound()
        {
            String file = Environment.CurrentDirectory + HttpServer.MSG_DIR + "404.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] d = new Byte[fileStream.Length];
            reader.Read(d, 0, d.Length);


            return new Response("404 page not found", "html/text", d);
        }

        public void Post(NetworkStream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(String.Format("{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Range: bytes\r\nContetn-Length: {4}\r\n",
                 HttpServer.VERSION, status, HttpServer.NAME, mime));
            stream.Write(data, 0, data.Length);
        }
    }
}
