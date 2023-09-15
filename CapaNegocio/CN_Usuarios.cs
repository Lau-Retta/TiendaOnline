using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Usuarios
    {
        //acceso a los metos de la clase CD_usuarios
        private CD_Ususarios objCapaDatos = new CD_Ususarios();

        public List<Usuario> listar() {

            return objCapaDatos.listar();

        }



        public int Registrar(Usuario obj, out string Mensaje) {

            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Nombres) || string.IsNullOrWhiteSpace(obj.Nombres)) {
                Mensaje = "El nombre del usuario no puede ser vacio";
            }
            else if(string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos)) {
                Mensaje = "El apellido del usuario no puede ser vacio";
            }
            else if(string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "El correo del usuario no puede ser vacio";
            }


            if (string.IsNullOrEmpty(Mensaje))
            {
                //enviamos un correo al usuario con el que se registro 

                string clave = Cn_Recursos.GenerearClave();
                string asunto = "Registro de Carrito de compras";
                string mensaje_correo = "<h3>Su cuenta fue creada correctamente</h3></br><p>Su contraseña para acceder es: !clave!</p>";
                mensaje_correo = mensaje_correo.Replace("!clave!",clave); // funcion q remplaza el 1° parametro x el 2
                
                bool repuesta = Cn_Recursos.EnviarCorreo(obj.Correo, asunto, mensaje_correo);


                if (repuesta)
                {
                    obj.Clave = Cn_Recursos.ConvertirSha256(clave);//encriptamos la clave en formato HASA SHA256
                    return objCapaDatos.Registrar(obj, out Mensaje);
                }
                else {
                    Mensaje = "No se puede enviar el correo";
                    return 0;
                }

             
            }
            else {
                return 0;
            }
        }

        public bool Editar(Usuario obj, out string Mensaje)
        {


            Mensaje = string.Empty;
            if (string.IsNullOrEmpty(obj.Nombres) || string.IsNullOrWhiteSpace(obj.Nombres))
            {
                Mensaje = "El nombre del ususario no puede ser vacio";
            }
            else if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "El apellido del ususario no puede ser vacio";
            }
            else if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "El correo del ususario no puede ser vacio";
            }


            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDatos.Editar(obj, out Mensaje);
            }
            else {
                return false;
            }
        }

        public bool Eliminar(int id, out string Mensaje) {
            return objCapaDatos.Eliminar(id, out Mensaje);
        }
            
    }
}
