using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace CapaDatos
{
    public class CD_Reporte
    {
        public DashBoard verDashBoard()
        {
            DashBoard objeto = new DashBoard();

            try
            { 
                using (MySqlConnection oconexion = new MySqlConnection(Conexion.cn))
                {
                    

                    MySqlCommand comando = new MySqlCommand("sp_ReporteDashboard", oconexion);
                    
                    comando.CommandType = System.Data.CommandType.StoredProcedure;

                    oconexion.Open();

                    using (MySqlDataReader dr = comando.ExecuteReader())
                    {
                        
                        while (dr.Read())
                        {
                            objeto = new DashBoard()
                            {
                                TotalCliente = Convert.ToInt32(dr["Total_Clientes"]),
                                TotalVenta = Convert.ToInt32(dr["Total_Ventas"]),
                                TotalProducto = Convert.ToInt32(dr["Total_Productos"]),
                            };
                        }
                    }
                    oconexion.Close();
                }

            }
            catch (Exception)
            {

                objeto = new DashBoard();
            }

            return objeto;
        }

    }
}
