using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CapaNegocio;
using CapaEntidad;

namespace CapaPresentacion.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Usuarios()
        {
            return View();

        }
        [HttpGet]
        public JsonResult ListarUsuarios() {

            List<Usuario> oLista = new List<Usuario>();

            oLista = new CN_Usuarios().listar();

            //que retorne los elementos de la lista como un Json
            return Json(new {data = oLista }, JsonRequestBehavior.AllowGet);

        }
         
        [HttpPost]
        public JsonResult GuardarUsuario(Usuario objeto){

            object resultado;//las variables object permiten guardar de cualquier tipo de valor sea objeto,string o numero
            string mensaje = string.Empty;

            if (objeto.IdUsuario == 0)// si el id del usuario que entra como parametro es 0 se registra sino se edita
            {

                resultado = new CN_Usuarios().Registrar(objeto, out mensaje);
            }
            else {
                resultado = new CN_Usuarios().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public JsonResult Eliminar(int id) {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Usuarios().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult VistaDashBoard() {
            DashBoard obj = new CN_Reporte().VerDashBoard();

            return Json(new { resultado =  obj}, JsonRequestBehavior.AllowGet);
        }
    }
}