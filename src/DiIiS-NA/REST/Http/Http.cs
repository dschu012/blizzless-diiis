﻿//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.IO;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Reflection;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Http
{
    public class HttpHeader
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public string DeviceId { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
        public string AcceptLanguage { get; set; }
        public string Accept { get; set; }
        public string UserAgent { get; set; }
        public string Content { get; set; }
    }

    public enum HttpCode
    {
        OK = 200,
        Found = 302,
        BadRequest = 400,
        NotFound = 404,
        InternalServerError = 500
    }

    public class HttpHelper
    {
        public static byte[] CreateResponse2(HttpCode httpCode, bool closeConnection = false)
        {
            var sb = new StringBuilder();

            //Blizzless Project 2022 
using (var sw = new StringWriter(sb))
            {
                sw.WriteLine($"HTTP/1.1 {(int)httpCode} {httpCode}");

                sw.WriteLine($"Date: {DateTime.Now.ToUniversalTime():r}");
                sw.WriteLine("Server: Apache");
                //sw.WriteLine("Retry-After: 600");
                sw.WriteLine($"Content-Length: 0");
                //sw.WriteLine("Vary: Accept-Encoding");
                sw.WriteLine("[Request URI: https://cdn.discordapp.com/attachments/826902540460490753/830193471871647804/bgs-key-fingerprint");
                if (closeConnection)
                    sw.WriteLine("Connection: close");

                sw.WriteLine("Content-Type: text/plain;charset=UTF-8");
                //sw.WriteLine("[Request URI: http://eu.depot.battle.net:1119/adff75d57de90974f8e383c2a54ebd3d83838899d938fe33369a4e305f224fa9.bpk]");
                //System.IO.File.WriteAllLines(@"C:\WriteLines.txt", lines);

                //sw.WriteLine(System.IO.File.ReadAllText("bgs-key-fingerprint"));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static byte[] CreateResponse1(HttpCode httpCode, byte[] content, bool closeConnection = false)
        {
            var sb = new StringBuilder();

            //Blizzless Project 2022 
using (var sw = new StringWriter(sb))
            {
                sw.WriteLine($"HTTP/1.1 {(int)httpCode} {httpCode}");

                //sw.WriteLine($"Date: {DateTime.Now.ToUniversalTime():r}");
                sw.WriteLine("Server: Apache/2.2.15 (CentOS)");
                //sw.WriteLine("Retry-After: 600");
                sw.WriteLine($"Content-Length: {content.Length}");
                //sw.WriteLine("Vary: Accept-Encoding");

                if (closeConnection)
                    sw.WriteLine("Connection: close");

                sw.WriteLine("Content-Type: text/plain;charset=UTF-8");
                //sw.WriteLine("[Request URI: http://eu.depot.battle.net:1119/adff75d57de90974f8e383c2a54ebd3d83838899d938fe33369a4e305f224fa9.bpk]");

                sw.WriteLine(content);
            }

            return Encoding.UTF8.GetBytes(sb.ToString() + content);
        }

        public static byte[] CreateResponseAlt(HttpCode httpCode, string content, bool closeConnection = false)
        {
            var sb = new StringBuilder();

            //Blizzless Project 2022 
using (var sw = new StringWriter(sb))
            {
                //sw.WriteLine($"HTTP/1.1 404 Not Found");
                //sw.WriteLine("Connection: close");
                //*
                sw.WriteLine($"HTTP/1.1 {(int)httpCode} {httpCode}");
                sw.WriteLine("Connection: close"); ;
                sw.WriteLine($"Content-Length: 0\r\n");
                //*/
                //sw.WriteLine();
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static byte[] CreateResponse(HttpCode httpCode, string content, bool closeConnection = false)
        {
            var sb = new StringBuilder();

            //Blizzless Project 2022 
using (var sw = new StringWriter(sb))
            {
                sw.WriteLine($"HTTP/1.1 {(int)httpCode} {httpCode}");

                //sw.WriteLine($"Date: {DateTime.Now.ToUniversalTime():r}");
                //sw.WriteLine("Server: Arctium-Emulation");
                //sw.WriteLine("Retry-After: 600");
                sw.WriteLine($"Content-Length: {content.Length}");
                //sw.WriteLine("Vary: Accept-Encoding");

                if (closeConnection)
                    sw.WriteLine("Connection: close");

                sw.WriteLine("Content-Type: application/json;charset=UTF-8");
                sw.WriteLine();

                sw.WriteLine(content);
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static HttpHeader ParseRequest(byte[] data, int length)
        {
            var headerValues = new Dictionary<string, object>();
            var header = new HttpHeader();

            //Blizzless Project 2022 
using (var sr = new StreamReader(new MemoryStream(data, 0, length)))
            {
                var info = sr.ReadLine().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (info.Length != 3)
                    return null;

                headerValues.Add("method", info[0]);
                headerValues.Add("path", info[1]);
                headerValues.Add("type", info[2]);

                while (!sr.EndOfStream)
                {
                    info = sr.ReadLine().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (info.Length == 2)
                        headerValues.Add(info[0].Replace("-", "").ToLower(), info[1]);
                    else if (info.Length > 2)
                    {
                        var val = "";

                        info.Skip(1);

                        headerValues.Add(info[0].Replace("-", "").ToLower(), val);
                    }
                    else
                    {
                        // We are at content here.
                        var content = sr.ReadLine();

                        headerValues.Add("content", content);

                        // There shouldn't be anything after the content!
                        break;
                    }
                }
            }

            var httpFields = typeof(HttpHeader).GetTypeInfo().GetProperties();

            foreach (var f in httpFields)
            {
                object val;

                if (headerValues.TryGetValue(f.Name.ToLower(), out val))
                {
                    if (f.PropertyType == typeof(int))
                        f.SetValue(header, Convert.ChangeType(Convert.ToInt32(val), f.PropertyType));
                    else
                        f.SetValue(header, Convert.ChangeType(val, f.PropertyType));
                }
            }

            return header;
        }
    }
}
