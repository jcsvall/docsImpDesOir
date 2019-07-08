using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public partial class Bandeja_Ordenes : System.Web.UI.Page
{  

    public class Pagina {
        public int TotalPaginas;
        public int TotalRegistros;
    }

    public class BandejaOrdenes {
        public String PuestoEncryp;
        public String NoOrdenEncryp;
        public String Fecha;
        public String NoOrden;
        public String OperacionMag;
        public String TipoTratamiento;
        public String Puesto;
        public String Cliente;
        public String ResponsableMag;
        public String PlacaVapor;
        public String Producto;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (Session["IDUsuario"] == null)
        {
            Response.Redirect("Login.aspx");
        }
    }


    [WebMethod()]
    public static List<BandejaOrdenes> GetOrdenesList(string busqueda, int currentpage)
    {
        ValidarSession();
        busqueda = busqueda.ToUpper().Trim();        
        return GetList(busqueda, currentpage);
    }

    public static List<BandejaOrdenes> GetList(string busqueda, int currentpage)
    {
        int Limit = 10;
        int Start = (currentpage - 1) * Limit;
        String IdPais = "SV";
        List<BandejaOrdenes> list = listaDb(Start, Limit, IdPais, busqueda);
       
        return list;
    }
    public static List<BandejaOrdenes> listaDb(int Start, int Limit, String IdPais, string busqueda)
    {
        Funciones fn = new Funciones();
        Utilidades Util = new Utilidades();
        String Clave = "BandejaOrden";
       // String LikeNombreOrCliente = " ";     
        //if (!busqueda.Equals(""))
        //{
        //    LikeNombreOrCliente = " and (upper(Nombre) like '%" + busqueda + "%' or upper(cli.Cliente) like '%"+ busqueda + "%') ";
        //}
        
        List<BandejaOrdenes> lista = new List<BandejaOrdenes>();
        
        SqlConnection Conn = fn.ConnectionSql();

        //String sQuery = "select cli.Puesto,cli.Cliente,Nombre,dui,nit,pasaporte,cli.idPais from tblCliente cli left join dbo.tblClienteDocumento clidoc ";
        //sQuery = sQuery + "on cli.Puesto = clidoc.puesto and clidoc.cliente = cli.cliente and clidoc.idPais=cli.idPais ";
        //sQuery = sQuery + "where cli.idPais='" + IdPais + "'" + LikeNombreOrCliente + LikePuesto;
        //String sQuery = "SELECT FechaOrden,NoOrdenMag,OperacionMAG,Tratamiento,Puesto,Cliente,InspectorMAG,Placa,Vapor,'producto nombre' Producto  FROM tblOrdenMAG ";
        //sQuery = sQuery + "ORDER BY NoOrdenMag OFFSET " + Start + " ROWS FETCH NEXT " + Limit + " ROWS ONLY";
        String sQuery = getQuery(Start,Limit, "consulta",busqueda);


        SqlCommand cmSQL = new SqlCommand(sQuery, Conn);

        SqlDataReader reader = cmSQL.ExecuteReader();
        while (reader.Read())
        {
            BandejaOrdenes Bandeja = new BandejaOrdenes();

            String Puesto=reader["Puesto"].ToString();
            String NoOrden = reader["NoOrdenMag"].ToString();

            Bandeja.Fecha = reader["FechaOrden"].ToString();
            Bandeja.NoOrden = NoOrden;
            Bandeja.OperacionMag = reader["OperacionMAG"].ToString();
            Bandeja.TipoTratamiento = reader["Tratamiento"].ToString();
            Bandeja.Puesto = Puesto;
            Bandeja.Cliente = reader["Cliente"].ToString();
            Bandeja.ResponsableMag = reader["InspectorMAG"].ToString();
            Bandeja.PlacaVapor = (reader["Placa"].ToString()+" "+ reader["Vapor"].ToString()).Trim();
            Bandeja.Producto = reader["Producto"].ToString();
            
            Bandeja.PuestoEncryp = Util.EncryptToBase64String(Puesto, Clave);
            Bandeja.NoOrdenEncryp = Util.EncryptToBase64String(NoOrden, Clave);

            lista.Add(Bandeja);            
        }
        Conn.Close();
        return lista;
    }

    public static string getQuery(int Start, int Limit, String tipo, String busqueda) {
        String sQuery = "";
        String where = "";
        if (!busqueda.Equals(""))
        {
            List<String> EsFechaValida = FechaValida(busqueda);
            if (EsFechaValida[0].Equals("true")) {
                String separador = EsFechaValida[1];
                where = " where format(FechaOrden,'dd"+ separador + "MM"+ separador + "yyyy') = '"+busqueda+"' ";
            }
            else { 
                where = " where (upper(NoOrdenMag) like '%" + busqueda + "%' or upper(Cliente) like '%" + busqueda + "%' ";
                where += " or upper(Tratamiento) like '%" + busqueda + "%' ";
                where += " or upper(Placa) like '%" + busqueda + "%' or upper(Vapor) like '%" + busqueda + "%') ";
            }
        }
        if (tipo.Equals("consulta"))
        {
            sQuery = "SELECT format(FechaOrden,'dd-MM-yyyy hh:mm:ss tt') FechaOrden,NoOrdenMag,OperacionMAG,Tratamiento,Puesto,Cliente,InspectorMAG,Placa,Vapor,Naturaleza Producto  FROM tblOrdenMAG ";
            sQuery += where;
            sQuery += "ORDER BY NoOrdenMag OFFSET " + Start + " ROWS FETCH NEXT " + Limit + " ROWS ONLY";
        }
        else {
            sQuery = "SELECT COUNT(NoOrdenMag) totalRegistros FROM tblOrdenMAG ";
            sQuery += where;
        }
        
        return sQuery;
    }

    public static List<String> FechaValida(string inputDate)
    {
        List<String> response = new List<String>();
        
        try
        {
            DateTime dateValue;
            dateValue = DateTime.ParseExact(inputDate, "dd-MM-yyyy", null);
            response.Add("true");
            response.Add("-");
        }
        catch
        {
            try
            {
                DateTime dateValue;
                dateValue = DateTime.ParseExact(inputDate, "dd/MM/yyyy", null);
                response.Add("true");
                response.Add("/");
            }
            catch
            {
                response.Add("false");
            }
        }
        return response;
    }

    [WebMethod()]
    public static Pagina GetPaginas(string busqueda, int currentpage)
    {
        //string puesto = "[Puesto]";
        Funciones fn = new Funciones();
        Pagina pagina = new Pagina();
        pagina.TotalPaginas = 0;
        pagina.TotalRegistros = 0;
        busqueda = busqueda.ToUpper().Trim();
        int Limit = 10;
        
        //String LikeNombreOrCliente = " ";
        //if (!busqueda.Equals(""))
        //{
        //    LikeNombreOrCliente = " and (upper(Nombre) like '%" + busqueda + "%' or upper(cli.Cliente) like '%" + busqueda + "%') ";
        //}

        //String LikePuesto = " ";
        //if (!puesto.Equals("[Puesto]"))
        //{
        //    LikePuesto = " and cli.Puesto like '%" + puesto + "%' ";
        //}

        //List<Client> lista = new List<Client>();
       
        SqlConnection Conn = fn.ConnectionSql();
       

        //String sQuery = "select count(cli.Puesto) totalRegistros from tblCliente cli left join dbo.tblClienteDocumento clidoc ";
        //sQuery = sQuery + "on cli.Puesto = clidoc.puesto and clidoc.cliente = cli.cliente and clidoc.idPais=cli.idPais ";
        //sQuery = sQuery + "where cli.idPais='" + IdPais + "'" + LikeNombreOrCliente + LikePuesto;
        String sQuery = getQuery(0, 0, "count", busqueda);

        SqlCommand cmSQL = new SqlCommand(sQuery, Conn);

        SqlDataReader reader = cmSQL.ExecuteReader();
        while (reader.Read())
        {
            int totalRe = Convert.ToInt32(reader["totalRegistros"].ToString());
            pagina.TotalRegistros = totalRe;

            double TotalPaginas = ((double)totalRe / (double)Limit);
            int TotalHaciaArriba= Convert.ToInt32(Math.Ceiling((TotalPaginas)));
            pagina.TotalPaginas = TotalHaciaArriba;
           
        }
        Conn.Close();

        return pagina;
    }

    public static void ValidarSession() {
        if (HttpContext.Current.Session["IDUsuario"] == null)
        {
            HttpContext.Current.Response.Redirect("Login.aspx");
        }
    }

}
