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
    public class CD_Categoria
    {
        public List<Categoria> listar()
        {
            List<Categoria> lista = new List<Categoria>();

            try
            { // declaramos la conexion con la db  y en donde
              // va la cadena de conexion colocamos "Conexion.cn" de la clase conexion
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdCategoria,Descripcion,Activo FROM categoria";

                    MySqlCommand comando = new MySqlCommand(query, oconexion);

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
                            lista.Add(new Categoria()
                            {

                                IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                                Descripcion = dr["Descripcion"].ToString(),
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

                lista = new List<Categoria>();
            }

            return lista;
        }

        public int Registrar(Categoria obj, out string Mensaje)
        {

            int idautogenerado = 0;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {

                    oconexion.Open();
                    using (MySqlCommand command = new MySqlCommand("sp_RegistrarCAtegoria", oconexion))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.AddWithValue("p_Descripcion", obj.Descripcion);
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

        public bool Editar(Categoria obj, out string Mensaje)
        {

            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("sp_EditarCategoria", oconexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("p_IdCategoria", obj.IdCategoria);
                        cmd.Parameters.AddWithValue("p_Descripcion", obj.Descripcion);
                        cmd.Parameters.AddWithValue("p_Activo", obj.Activo);

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

        public bool Eliminar(int id, out string Mensaje)
        {

            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("sp_EliminarCategoria", oconexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("p_IdCategoria", id);

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
