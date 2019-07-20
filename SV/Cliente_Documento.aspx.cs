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

public partial class Cliente_Documento : System.Web.UI.Page
{

    public class Client
    {
        public string Puesto;
        public string Cliente;
        public string Nombre;
        public string Dui;
        public string Nit;
        public string Pasaporte;
        public string IdPais;
    }

    public class PuestoPojo {
        public string Puesto;
        public string Nombre;
    }

    public class Pagina {
        public int TotalPaginas;
        public int TotalRegistros;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (Session["IDUsuario"] == null)
        {
            Response.Redirect("Login.aspx");
        }
    }


    [WebMethod()]
    public static List<Client> GetClientList(string busqueda, int currentpage, string puesto)
    {
        ValidarSession();
        busqueda = busqueda.ToUpper().Trim();
        return GetList(busqueda, currentpage, puesto);
    }

    public static List<Client> GetList(string busqueda, int currentpage, string puesto)
    {
        int Limit = 10;
        int Start = (currentpage - 1) * Limit;
        String IdPais = "SV";
        List<Client> list = listaClientesDb(Start, Limit, IdPais, busqueda, puesto);
       
        return list;
    }
    public static List<Client> listaClientesDb(int Start, int Limit, String IdPais, string busqueda, string puesto)
    {
        Funciones fn = new Funciones();
        String LikeNombreOrCliente = " ";     
        if (!busqueda.Equals(""))
        {
            LikeNombreOrCliente = " and (upper(Nombre) like '%" + busqueda + "%' or upper(cli.Cliente) like '%"+ busqueda + "%') ";
        }

        String LikePuesto = " ";
        if (!puesto.Equals("[Puesto]"))
        {
            LikePuesto = " and cli.Puesto like '%"+puesto+"%' ";
        }

        List<Client> lista = new List<Client>();
        
        SqlConnection Conn = fn.ConnectionSql();

        String sQuery = "select cli.Puesto,cli.Cliente,Nombre,dui,nit,pasaporte,cli.idPais from tblCliente cli left join dbo.tblClienteDocumento clidoc ";
        sQuery = sQuery + "on cli.Puesto = clidoc.puesto and clidoc.cliente = cli.cliente and clidoc.idPais=cli.idPais ";
        sQuery = sQuery + "where cli.idPais='" + IdPais + "'" + LikeNombreOrCliente + LikePuesto;
        sQuery = sQuery + "ORDER BY cli.Nombre OFFSET " + Start + " ROWS FETCH NEXT " + Limit + " ROWS ONLY";
        
        SqlCommand cmSQL = new SqlCommand(sQuery, Conn);

        SqlDataReader reader = cmSQL.ExecuteReader();
        while (reader.Read())
        {
            Client c1 = new Client();            
            c1.Puesto = reader["Puesto"].ToString();
            c1.Cliente = reader["Cliente"].ToString();
            c1.Nombre = reader["Nombre"].ToString();
            c1.Dui = reader["dui"].ToString();
            c1.Nit = reader["nit"].ToString();
            c1.Pasaporte = reader["pasaporte"].ToString();
            c1.IdPais = reader["idPais"].ToString();
            lista.Add(c1);            
        }
        Conn.Close();
        return lista;
    }

    [WebMethod()]
    public static string ActualizarClienteDoc(string Cliente, string Nombre, string Puesto, string Dui, string Nit, string Pasaporte, string IdPais)
    {
       ValidarSession();
       String a= ejecutarUpdate(Cliente, Puesto, Dui, Nit, Pasaporte, IdPais);
       return null;
       
    }

    public static string ejecutarUpdate(string Cliente, string Puesto, string Dui, string Nit, string Pasaporte, string IdPais)
    {
        Funciones fn = new Funciones();
        String IdClienteDocumento = "";
        String queryBusqueda = "select id from tblClienteDocumento where puesto='" + Puesto + "' and cliente='" + Cliente + "' and idPais='" + IdPais + "'";
       
        SqlConnection Conn = fn.ConnectionSql();
       
        SqlCommand cmSQL = new SqlCommand(queryBusqueda, Conn);
        SqlDataReader reader = cmSQL.ExecuteReader();
        while (reader.Read())
        {
            IdClienteDocumento = reader["id"].ToString();
        }

        if (!IdClienteDocumento.Equals(""))
        {
            int id = Convert.ToInt32(IdClienteDocumento);
            String queryUpdate = "update tblClienteDocumento set dui='"+Dui+"',nit='"+Nit+"',pasaporte='"+Pasaporte+"' where id="+id+"";
            //Ejecuta una transaccion. (insert,update,delete)
            fn.EjecutarNonQuery(queryUpdate, Conn);
        }
        else
        {
            String queryAdd = "insert into tblClienteDocumento(puesto,cliente,idPais,dui,nit,pasaporte) values('"+ Puesto + "','"+ Cliente + "','"+ IdPais + "','"+Dui+"','"+Nit+"','"+Pasaporte+"')";            
            fn.EjecutarNonQuery(queryAdd, Conn);
        }

        Conn.Close();
        return null;
    }

    [WebMethod()]
    public static List<PuestoPojo> ObtenerPuestos(string puesto) {
        Funciones fn = new Funciones();
        String IdPais = "SV";
        List<PuestoPojo> puestoList = new List<PuestoPojo>();
        String queryBusqueda = "select Puesto,Nombre from tblPuesto where idPais='"+ IdPais + "'";
        
        SqlConnection Conn = fn.ConnectionSql();
        
        SqlCommand cmSQL = new SqlCommand(queryBusqueda, Conn);
        SqlDataReader reader = cmSQL.ExecuteReader();
        while (reader.Read())
        {
            PuestoPojo p = new PuestoPojo();
            p.Puesto = reader["Puesto"].ToString();
            p.Nombre = reader["Nombre"].ToString();
            puestoList.Add(p);
        }
        Conn.Close();
        return puestoList;
    }

    [WebMethod()]
    public static Pagina GetPaginas(string busqueda, int currentpage, string puesto)
    {
        Funciones fn = new Funciones();
        Pagina pagina = new Pagina();
        pagina.TotalPaginas = 0;
        pagina.TotalRegistros = 0;
        busqueda = busqueda.ToUpper().Trim();
        int Limit = 10;
        
        String IdPais = "SV";

        String LikeNombreOrCliente = " ";
        if (!busqueda.Equals(""))
        {
            LikeNombreOrCliente = " and (upper(Nombre) like '%" + busqueda + "%' or upper(cli.Cliente) like '%" + busqueda + "%') ";
        }

        String LikePuesto = " ";
        if (!puesto.Equals("[Puesto]"))
        {
            LikePuesto = " and cli.Puesto like '%" + puesto + "%' ";
        }

        List<Client> lista = new List<Client>();
       
        SqlConnection Conn = fn.ConnectionSql();
       

        String sQuery = "select count(cli.Puesto) totalRegistros from tblCliente cli left join dbo.tblClienteDocumento clidoc ";
        sQuery = sQuery + "on cli.Puesto = clidoc.puesto and clidoc.cliente = cli.cliente and clidoc.idPais=cli.idPais ";
        sQuery = sQuery + "where cli.idPais='" + IdPais + "'" + LikeNombreOrCliente + LikePuesto;
        
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
