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
            while (true)
            {
                if (request == null)
                    return MakeNullRequest();


                if (request.Type == "GET")
                {
                    String file = Environment.CurrentDirectory + HttpServer.WEB_DIR + request.URL;
                    FileInfo f = new FileInfo(file);
                    if (f.Exists && f.Extension.Contains("."))
                    {
                        return MakeFromFile(f);
                    }
                    else
                    {
                        DirectoryInfo directory = new DirectoryInfo(f + "/");
                        if (!directory.Exists)
                            return MakeNotFound();
                        FileInfo[] files = directory.GetFiles();
                        foreach (FileInfo ff in files)
                        {
                            String n = ff.Name;
                            if (n.Contains("default.html") || n.Contains("default.html") || n.Contains("index.html") || n.Contains("index.html"))
                                return MakeFromFile(ff);
                        }
                    }
                }
                else
                    return MakeMathodNotAllowed();

                return MakeMathodNotAllowed();
            }
        }

        private static Response MakeFromFile(FileInfo f)
        {
            FileStream fileStream = f.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] d = new Byte[fileStream.Length];
            reader.Read(d, 0, d.Length);
            fileStream.Close();
            return new Response("200 ok", "text/html", d);
        }

        private static Response MakeNullRequest()
        {
            String file = Environment.CurrentDirectory + HttpServer.MSG_DIR + "400.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] d = new Byte[fileStream.Length];
            reader.Read(d, 0, d.Length);
            fileStream.Close();
            return new Response("400 bad request", "text/html", d);
        }

        private static Response MakeMathodNotAllowed()
        {
            String file = Environment.CurrentDirectory + HttpServer.MSG_DIR + "405.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] d = new Byte[fileStream.Length];
            fileStream.Close();
            reader.Read(d, 0, d.Length);

 
            return new Response("405 bad request", "text/html", d);
        }

        private static Response MakeNotFound()
        {
            String file = Environment.CurrentDirectory + HttpServer.MSG_DIR + "404.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] d = new Byte[fileStream.Length];
            reader.Read(d, 0, d.Length);

            fileStream.Close();
            return new Response("404 page not found", "text/html", d);
        }

        public void Post(NetworkStream stream)
        {
            
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(String.Format("{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContetn-Length: {4}\r\n",
                HttpServer.VERSION, status, HttpServer.NAME, mime, data.Length));
                writer.Flush();
                stream.Write(data, 0, data.Length);
           
        }
    }
}
