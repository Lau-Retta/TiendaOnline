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
    public class CD_Ususarios
    {
        // Lista los usuarios en la base de datos en tabla "usuario"
        public List<Usuario> listar() {
            List < Usuario > lista = new List<Usuario>();

            try
            { // declaramos la conexion con la db  y en donde
              // va la cadena de conexion colocamos "Conexion.cn" de la clase conexion
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn)) {
                    string query = "Select IdUsuario,Nombres,Apellidos,Correo,Clave,Restablecer,Activo from usuario";

                    MySqlCommand comando = new MySqlCommand(query,oconexion);

                    //le indicamos que es una comando de ttipo text, asio agilizamos la ejecucion
                    comando.CommandType = System.Data.CommandType.Text;

                    oconexion.Open();

                    Console.WriteLine("Conexion establecida");

                    using (MySqlDataReader dr = comando.ExecuteReader()) {
                        //mientras que estes leyendo dr
                        while (dr.Read()) {
                            //indentamos un nuevo usuario para agrgar a la lista
                            lista.Add(new Usuario() { 

                                IdUsuario= Convert.ToInt32(dr["IdUsuario"]),
                                Nombres= dr["Nombres"].ToString(),
                                Apellidos = dr["Apellidos"].ToString(),
                                Correo = dr["Correo"].ToString(),
                                Clave = dr["Clave"].ToString(),
                                Restablecer = Convert.ToBoolean(dr["Restablecer"]),
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

                lista = new List<Usuario>();
            }

            return lista;
        }

        public int Registrar(Usuario obj, out string Mensaje) {

            int idautogenerado = 0;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {

                    oconexion.Open();
                    using (MySqlCommand command = new MySqlCommand("sp_RegistroUsuario", oconexion)) { 
                    
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.AddWithValue("p_Nombres",obj.Nombres);
                        command.Parameters.AddWithValue("p_Apellidos", obj.Apellidos);
                        command.Parameters.AddWithValue("p_Correo", obj.Correo);
                        command.Parameters.AddWithValue("p_Clave", obj.Clave);
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


        public bool Editar(Usuario obj, out string Mensaje)
        {

            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    oconexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("sp_EditarUsuario", oconexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("p_IdUsuario", obj.IdUsuario);
                        cmd.Parameters.AddWithValue("p_Nombres", obj.Nombres);
                        cmd.Parameters.AddWithValue("p_Apellidos", obj.Apellidos);
                        cmd.Parameters.AddWithValue("p_Correo", obj.Correo);
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

                resultado= false;
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
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM usuario WHERE IdUsuario = @id LIMIT 1", oconexion); //elimine como maximo a un usuario que tenga esa id
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false;

                   

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
