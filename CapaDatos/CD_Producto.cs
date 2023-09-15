using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CapaEntidad;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace CapaDatos
{
    public class CD_Producto
    {
        public List<Producto> listar()
        {
            List<Producto> lista = new List<Producto>();

            try
            { // declaramos la conexion con la db  y en donde
              // va la cadena de conexion colocamos "Conexion.cn" de la clase conexion
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("Select p.IdProducto, p.Nombre, p.Descripcion,");
                    query.AppendLine("m.IdMarca, m.Descripcion AS DesMarca,");
                    query.AppendLine("c.IdCategoria,c.Descripcion AS DesCategoria,");
                    query.AppendLine("p.Precio, p.Stock, p.RutaImagen, p.NombreImagen,p.Activo from producto p");
                    query.AppendLine("inner join marca m on m.IdMarca = p.IdMarca");
                    query.AppendLine("inner join categoria c on c.IdCategoria = p.IdCategoria");
 
                    MySqlCommand comando = new MySqlCommand(query.ToString(), oconexion);

                    //le indicamos que es una comando de ttipo text, asio agilizamos la ejecucion
                    comando.CommandType = System.Data.CommandType.Text;

                    oconexion.Open();

                    Console.WriteLine("Conexion establecida");

                    using (MySqlDataReader dr = comando.ExecuteReader())
                    {
                        //mientras que estes leyendo dr
                        while (dr.Read())
                        {
                            //indentamos un nuevo usuario para agrgar a la lista
                            lista.Add(new Producto()
                            {

                                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                Nombre = dr["Nombre"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                oMarca = new Marca() { IdMarca = Convert.ToInt32(dr["IdMarca"]), Descripcion = dr["DesMarca"].ToString()},
                                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(dr["IdCategoria"]), Descripcion = dr["DesCategoria"].ToString()},
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                Stock = Convert.ToInt32(dr["Stock"]),
                                RutaImagen = dr["RutaImagen"].ToString(),
                                NombreImagen = dr["NombreImagen"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"])
                            }

                            );
                        }
                    }
                    oconexion.Close();
                }

            }
            catch (Exception)
            {

                lista = new List<Producto>();
            }

            return lista;
        }

        public int Registrar(Producto obj, out string Mensaje)
        {

            int idautogenerado = 0;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {

                    oconexion.Open();
                    using (MySqlCommand command = new MySqlCommand("sp_RegistrarProducto", oconexion))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.AddWithValue("p_Nombre", obj.Nombre);
                        command.Parameters.AddWithValue("p_Descripcion", obj.Descripcion);
                        command.Parameters.AddWithValue("p_IdMarca", obj.oMarca.IdMarca);
                        command.Parameters.AddWithValue("p_IdCategoria", obj.oCategoria.IdCategoria);
                        command.Parameters.AddWithValue("p_Precio", obj.Precio);
                        command.Parameters.AddWithValue("p_Stock", obj.Stock);
                        command.Parameters.AddWithValue("p_Activo", obj.Activo);

                        // Parámetros de salida
                        command.Parameters.Add(new MySqlParameter("p_Mensaje", MySqlDbType.VarChar, 500));
                        command.Parameters["p_Mensaje"].Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.Add(new MySqlParameter("p_Resultado", MySqlDbType.Int32));
                        command.Parameters["p_Resultado"].Direction = System.Data.ParameterDirection.Output;

                        // Ejecutar el procedimiento almacenado
                        command.ExecuteNonQuery();

                        // Recuperar los valores de los parámetros de salida
                        Mensaje = command.Parameters["p_Mensaje"].Value.ToString();
                        idautogenerado = Convert.ToInt32(command.Parameters["p_Resultado"].Value);

                        Console.WriteLine(Mensaje);
                        Console.WriteLine("p_Resultado: " + idautogenerado);
                    }


                }
            }
            catch (Exception ex)
            {

                idautogenerado = 0;
                Mensaje = ex.Message;
            }
            return idautogenerado;

        }

        public bool Editar(Producto obj, out string Mensaje)
        {

            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    using (MySqlCommand command = new MySqlCommand("sp_EditarProducto", oconexion))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("p_IdProducto", obj.IdProducto);
                        command.Parameters.AddWithValue("p_Nombre", obj.Nombre);
                        command.Parameters.AddWithValue("p_Descripcion", obj.Descripcion);
                        command.Parameters.AddWithValue("p_IdMarca", obj.oMarca.IdMarca);
                        command.Parameters.AddWithValue("p_IdCategoria", obj.oCategoria.IdCategoria);
                        command.Parameters.AddWithValue("p_Precio", obj.Precio);
                        command.Parameters.AddWithValue("p_Stock", obj.Stock);
                        command.Parameters.AddWithValue("p_Activo", obj.Activo);

                        // Parámetros de salida
                        command.Parameters.Add(new MySqlParameter("p_Mensaje", MySqlDbType.VarChar, 500));
                        command.Parameters["p_Mensaje"].Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.Add(new MySqlParameter("p_Resultado", MySqlDbType.Int32));
                        command.Parameters["p_Resultado"].Direction = System.Data.ParameterDirection.Output;


                        command.ExecuteNonQuery();

                        resultado = Convert.ToBoolean(command.Parameters["p_Resultado"].Value);
                        Mensaje = command.Parameters["p_Mensaje"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }

        public bool GuardarImagen(Producto obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {

                    oconexion.Open();
                    string query = "update producto set  RutaImagen = @RutaImagen, NombreImagen = @NombreImagen where IdProducto = @IdProducto";


                    using (MySqlCommand command = new MySqlCommand(query, oconexion))
                    {
                        // Parámetros de entrada
                        command.Parameters.AddWithValue("@IdProducto", obj.IdProducto);
                        command.Parameters.AddWithValue("@RutaImagen", obj.RutaImagen);
                        command.Parameters.AddWithValue("@NombreImagen", obj.NombreImagen);
                        

                        if (command.ExecuteNonQuery() > 0)
                        {
                            resultado = true;
                        }
                        else {
                            Mensaje = "No se pudo actualizar la imagen";
                        }
                      
                    }


                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }

        public bool Eliminar(int id, out string Mensaje)
        {

            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("sp_EliminarProducto", oconexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("p_IdProducto", id);

                        // Parámetros de salida
                        cmd.Parameters.Add(new MySqlParameter("p_Mensaje", MySqlDbType.VarChar, 500));
                        cmd.Parameters["p_Mensaje"].Direction = System.Data.ParameterDirection.Output;

                        cmd.Parameters.Add(new MySqlParameter("p_Resultado", MySqlDbType.Int32));
                        cmd.Parameters["p_Resultado"].Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        resultado = Convert.ToBoolean(cmd.Parameters["p_Resultado"].Value);
                        Mensaje = cmd.Parameters["p_Mensaje"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }


       
    }
}
