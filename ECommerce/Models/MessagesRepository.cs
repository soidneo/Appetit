using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ECommerce.Models
{
    public class MessagesRepository
    {
        private ECommerceContext db = new ECommerceContext();
        readonly string _connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<CajaVista> GetAllMessages()
        {

            var cajaVista = new List<CajaVista>();
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"SELECT VentaID, ClienteID, EstadoID,'Domicilio' as tipo, Fecha, comentarios FROM dbo.Ventas", connection))

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
                        int cid = (int)reader["ClienteID"];
                        int eid = (int)reader["EstadoID"];
                        var cliente = db.Clientes.Where(c => c.ClienteID == cid).FirstOrDefault();
                        var estado = db.Estados.Where(e => e.EstadoID == eid).FirstOrDefault();

                        cajaVista.Add(item: new CajaVista { VentaID = (int)reader["VentaID"], Nombre = cliente.FullName, Estado = estado.Descripcion, Fecha = Convert.ToDateTime(reader["Fecha"]), Comentarios = coments, Tipo = (string)reader["Tipo"] });
                    }
                }
            }
            using (var connection2 = new SqlConnection(_connString))
            {
                connection2.Open();

                using (var command2 = new SqlCommand(@"SELECT PedidoMesaID, MesaID, EstadoID,'Mesa' as tipo, Fecha, comentarios FROM dbo.PedidoMesas", connection2))

                {
                    command2.Notification = null;
                    var dependency = new SqlDependency(command2);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange2);

                    if (connection2.State == ConnectionState.Closed)
                        connection2.Open();

                    var reader2 = command2.ExecuteReader();

                    while (reader2.Read())
                    {
                        string coments;
                        //E.Descripcion as Estado, M.Nombre as Nombre, Fecha,Comentarios
                        if (reader2["Comentarios"].ToString().Length < 1)
                        {
                            coments = "";
                        }
                        else
                        {
                            coments = (string)reader2["Comentarios"];
                        }

                        int mid = (int)reader2["MesaID"];
                        int eid = (int)reader2["EstadoID"];
                        var mesa = db.Mesas.Where(m => m.MesaID == mid).FirstOrDefault();
                        var estado = db.Estados.Where(e => e.EstadoID == eid).FirstOrDefault();


                        cajaVista.Add(item: new CajaVista { VentaID = (int)reader2["PedidoMesaID"], Nombre = mesa.Nombre, Estado = estado.Descripcion, Fecha = Convert.ToDateTime(reader2["Fecha"]), Comentarios = coments, Tipo = (string)reader2["Tipo"] });
                    }
                }

            }
            var newList = cajaVista.OrderByDescending(c => c.Fecha).ToList();
            return newList;
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                Hubs.MessagesHub.SendMessages();
            }
        }
        private void dependency_OnChange2(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                Hubs.MessagesHub.SendMessages();
            }
        }
    }
}