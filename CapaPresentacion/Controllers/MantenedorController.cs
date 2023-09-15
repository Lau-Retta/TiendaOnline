using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CapaNegocio;
using CapaEntidad;
using Newtonsoft.Json;
using System.Globalization;
using System.Configuration;
using System.IO;

namespace CapaPresentacion.Controllers
{
    public class MantenedorController : Controller
    {
        public string ConfigurationManger { get; private set; }

        // GET: Mantenedor
        public ActionResult Categoria()
        {
            return View();
        }

        public ActionResult Marca()
        {
            return View();
        }

        public ActionResult Producto()
        {
            return View();
        }

        //CATEGORIA
        #region categoria
        [HttpGet]
        public JsonResult ListarCategorias()
        {

            List<Categoria> oLista = new List<Categoria>();

            oLista = new CN_Categoria().listar();

            //que retorne los elementos de la lista como un Json
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GuardarCategoria(Categoria objeto)
        {

            object resultado;//las variables object permiten guardar de cualquier tipo de valor sea objeto,string o numero
            string mensaje = string.Empty;

            if (objeto.IdCategoria == 0)// si el id del usuario que entra como parametro es 0 se registra sino se edita
            {

                resultado = new CN_Categoria().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Categoria().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult Eliminar(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Categoria().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
#endregion

        //MARCA
        #region Marca
        [HttpGet]
        public JsonResult ListarMarca()
        {

            List<Marca> oLista = new List<Marca>();

            oLista = new CN_Marca().listar();

            //que retorne los elementos de la lista como un Json
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GuardarMarca(Marca objeto)
        {

            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdMarca == 0)
            {

                resultado = new CN_Marca ().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Marca().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult EliminarMarca(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Marca().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //PRODUCTO
        #region Producto
        [HttpGet]
        public JsonResult ListarProducto()
        {

            List<Producto> oLista = new List<Producto>();

            oLista = new CN_Producto().listar();

            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GuardarProducto(string objeto, HttpPostedFileBase archivo)
        {

            
            string mensaje = string.Empty;
            bool GuardadoImagen = true;
            bool operacion_exitosa = true;

            Producto oProducto = new Producto();
            oProducto = JsonConvert.DeserializeObject<Producto>(objeto); 

            decimal precio;
            // valida el precio que sea en deci mal, contemplando que el tipo de los decimales se marcan con punto
            // y la informacion cultural es de Peru, el resultado lo guarda en la variable precio
            //Si la validacion es correcta se almacena el precio sino se indica que la operacion fallo y se envia el mensaje           

           if(decimal.TryParse(oProducto.PrecioTexto, NumberStyles.AllowDecimalPoint, new CultureInfo("es-PE"), out precio))
            {
                oProducto.Precio = precio;
            }
            else {
                return Json(new { operacion_exitosa = false, mensaje = "+El formato del precio debe ser ##.##" }, JsonRequestBehavior.AllowGet);
            }

            if (oProducto.IdProducto == 0)
            {

                int idProductoGenerado = new CN_Producto().Registrar(oProducto, out mensaje);

                if (idProductoGenerado != 0)
                {
                    oProducto.IdProducto = idProductoGenerado;
                }
                else {
                    operacion_exitosa = false;
                }
            }
            else
            {
                operacion_exitosa = new CN_Producto().Editar(oProducto, out mensaje);
            }


            if (operacion_exitosa) {

                if (archivo != null) {
                    string ruta_guardar = ConfigurationManager.AppSettings["ServidorFotos"];// obtengo la ruta de guradado
                    string extension = Path.GetExtension(archivo.FileName); // obtengo la extension de la imagen
                    string nombreImg = string.Concat(oProducto.IdProducto.ToString(), extension); // le doy de nombre  a la imagen el id del producto


                    try
                    {
                        archivo.SaveAs(Path.Combine(ruta_guardar, nombreImg));// guardamos el archivo renombrado en la rutra especificada 
                    }
                    catch (Exception ex)
                    {

                        string msg = ex.Message;
                        GuardadoImagen = false;
                    }

                    if (GuardadoImagen)
                    {
                        oProducto.RutaImagen = ruta_guardar;
                        oProducto.NombreImagen = nombreImg;
                        bool respuesta = new CN_Producto().GuardarImagen(oProducto, out mensaje);
                    }
                    else {
                        mensaje = "El producto se ha guradado pero sin los datos de la imagen";
                    }
                }
            }

            return Json(new { operacion_exitosa = operacion_exitosa, idGenerado = oProducto.IdProducto ,mensaje = mensaje }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult ImagenPrducto(int id) {

            bool conversion;

            Producto oProducto = new CN_Producto().listar().Where(p => p.IdProducto == id).FirstOrDefault();

            string txtBase64 = Cn_Recursos.ConvertirBase64(Path.Combine(oProducto.RutaImagen, oProducto.NombreImagen), out conversion);


            return Json(new { 
                conversion = conversion, 
                txtBase64 = txtBase64, 
                extension = Path.GetExtension(oProducto.NombreImagen)},
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarProducto(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Producto().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}