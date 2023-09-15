using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using System.Net.Mail;
using System.Net;
using System.IO;

namespace CapaNegocio
{
    public class Cn_Recursos
    {

        //generar clave automatica
        public static string GenerearClave() {

            string clave= Guid.NewGuid().ToString("N").Substring(0,6);// con new guid le estoy designando una clave random N= alfanumerica, y de 6 digitos
            return clave;
        
        }


        //encriptacion DE TEXTO en SHA256
        public static string ConvertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));

            }

            return sb.ToString();
        }


        public static bool EnviarCorreo(string Correo, string Asunto, string Mensaje) {

            bool resultado = false;
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(Correo);//correo destino
                mail.From = new MailAddress("laut55723@gmail.com");//correo de donde mandamos
                mail.Subject = Asunto;
                mail.Body = Mensaje;
                mail.IsBodyHtml = true; //le estamos indicando que el cuerpo del mensaje lo trabajamos en formato HTML

                var smtp = new SmtpClient()//config el servidor para mandar correo
                {
                    Credentials = new NetworkCredential("laut55723@gmail.com", "xnunfdtpsknmxwfr"), //el correo que vamos a usar y su pass de aplicaicones 
                    Host = "smtp.gmail.com", //el host del servidor de gamil
                    Port =587, //el puesto x el que envia los correos
                    EnableSsl = true //sertificado de segurirdad

                };

                smtp.Send(mail);
                resultado = true;

            }
            catch (Exception ex)
            {
                resultado = false;
                
            }
            return resultado;
        }


        public static string ConvertirBase64(string ruta, out bool conv) {

            string txtBase64 = string.Empty;
            conv = true;

            try
            {
                byte[] bytes = File.ReadAllBytes(ruta);
                txtBase64 = Convert.ToBase64String(bytes);
            }
            catch (Exception)
            {

                conv = false;
            }
            return txtBase64;
        }


    }
}
