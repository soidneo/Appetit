using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ECommerce.Models
{
    /*public class MessagesRepository
    {
        readonly string aa_connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<CajaVista> GetAllMessagesss()
        {

            var ventas = new List<CajaVista>();
            using (var connection = new SqlConnection(aa_connString))
            {
                connection.Open();
                //using (var command = new SqlCommand(@"SELECT VentaID, ClienteID, EstadoID, Fecha FROM dbo.Ventas", connection))
                using (var command = new SqlCommand(@"SELECT V.VentaID as Venta,E.Descripcion as Estado, C.Nombre+''+C.Apellido as Nombre, Fecha,Comentarios,'Cliente' as Tipo
                FROM Ventas V 
                JOIN Clientes C ON C.ClienteID = v.ClienteID 
                JOIN Estadoes E ON V.EstadoID = E.EstadoID
                WHERE V.EmpresaID = 1
                union
                SELECT PM.PedidoMesaID as Venta,E.Descripcion as Estado, M.Nombre as Nombre, Fecha,Comentarios,'Mesa' as Tipo
                FROM PedidoMesas PM 
                JOIN Mesas M ON M.MesaID = PM.MesaID 
                JOIN Estadoes E ON PM.EstadoID = E.EstadoID
                WHERE PM.EmpresaID = 1order by Fecha", connection))
                {
                    command.Notification = null;
                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string coments;
                        //E.Descripcion as Estado, M.Nombre as Nombre, Fecha,Comentarios
                        if (reader["Comentarios"].ToString().Length < 1)
                        {
                            coments = "";
                        }
                        else
                        {
                            coments = (string)reader["Comentarios"];
                        } 
                        ventas.Add(item: new CajaVista { VentaID = (int)reader["Venta"], Nombre = (string)reader["Nombre"], Estado = (string)reader["Estado"], Fecha = Convert.ToDateTime(reader["Fecha"]), Comentarios = coments, Tipo = (string)reader["Tipo"] });
                    }
                }

            }
            return ventas;
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Type.ToString());
            if (e.Type == SqlNotificationType.Change)
            {
                Hubs.MessagesHub.SendMessages();
            }
        }
    }*/
}