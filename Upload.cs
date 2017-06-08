using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace NetworkListening
{
    public class Upload
    {
        public string UploadFile(string uploadfile, string url, NameValueCollection querystring, CookieContainer cookies, string fileFormName = "file", string contenttype = "multipart/form-data")
        {
            if ((fileFormName == null) ||(fileFormName.Length == 0))
            {
                fileFormName = "file";
            }
            if ((contenttype == null) ||(contenttype.Length == 0))
            {
                contenttype = "application/octet-stream";
            }
            Uri uri = new Uri(url);
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.CookieContainer = cookies;
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append(fileFormName);
            sb.Append("\"; filename=\"");
            sb.Append(uploadfile);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append(contenttype);
            sb.Append("\r\n");
            sb.Append("\r\n");

            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            byte[] br = Encoding.ASCII.GetBytes("\r\n");
            FileStream fileStream = new FileStream(uploadfile, FileMode.Open, FileAccess.Read);
            long length = postHeaderBytes.Length + fileStream.Length + br.Length;
            if (querystring != null)
            {

                StringBuilder sub = new StringBuilder();
                foreach (string key in querystring.Keys)
                {
                    sub.Append("--");
                    sub.Append(boundary);
                    sub.Append("\r\n");
                    sub.Append("Content-Disposition: form-data; name=\"");
                    sub.Append(key);
                    sub.Append("\"");
                    sub.Append("\r\n");
                    sub.Append("\r\n");
                    sub.Append(querystring[key]);
                    sub.Append("\r\n");
                    byte[] formitembytes = Encoding.UTF8.GetBytes(sub.ToString());
                    length += formitembytes.Length;
                }
            }
            length += boundaryBytes.Length;
            webrequest.ContentLength = length;
            Stream requestStream = webrequest.GetRequestStream();
            // Write out our post header 
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

            // Write out the file contents 
            byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                requestStream.Write(buffer, 0, bytesRead);
            requestStream.Write(br, 0, br.Length);
            if (querystring != null)
            {
                StringBuilder sub = new StringBuilder();
                foreach (string key in querystring.Keys)
                {
                    sub.Append("--");
                    sub.Append(boundary);
                    sub.Append("\r\n");
                    sub.Append("Content-Disposition: form-data; name=\"");
                    sub.Append(key);
                    sub.Append("\"");
                    sub.Append("\r\n");
                    sub.Append("\r\n");
                    sub.Append(querystring[key]);
                    sub.Append("\r\n");
                    byte[] formitembytes = Encoding.UTF8.GetBytes(sub.ToString());
                    requestStream.Write(formitembytes, 0, formitembytes.Length);
                }
            }
            // Write out the trailing boundary   
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            webrequest.Timeout = 1000000;

            WebResponse responce = webrequest.GetResponse();
            Stream s = responce.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string str = sr.ReadToEnd();
            fileStream.Close();
            requestStream.Close();
            sr.Close();
            s.Close();
            responce.Close();

            return str;

        }

    }
}
