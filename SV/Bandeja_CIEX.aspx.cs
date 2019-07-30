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
using System.Web.Script.Serialization;

public partial class Bandeja_CIEX : System.Web.UI.Page
{  

    
    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (Session["IDUsuario"] == null)
        {
            Response.Redirect("Login.aspx");
        }
    }

    [WebMethod()]
   // public static List<Dictionary<string, object>> GetBandejaData(string Busqueda,string Estado,int page,int rows)
    public static Dictionary<string, object> GetBandejaData(string Busqueda, string Estado, int page, int rows,string sidx, string sord)
    {
        Utilidades Util = new Utilidades();
        Funciones fn = new Funciones();
        int Limit = rows;
        int Start = (page - 1) * Limit;
        int totalRegistros = 0;

        String limite = " ORDER BY "+ sidx +" "+ sord +" OFFSET " + Start + " ROWS FETCH NEXT " + Limit + " ROWS ONLY";
        String QuerySelect = getQuery(Busqueda, Estado,"select") + limite;
        String QueryCount = getQuery(Busqueda, Estado, "count");

        SqlConnection Conn = fn.ConnectionSql();
        SqlCommand cmSQL = new SqlCommand(QueryCount, Conn);

        SqlDataReader reader = cmSQL.ExecuteReader();
        while (reader.Read())
        {
            totalRegistros = Convert.ToInt32(reader["totalRegistros"].ToString());
        }
        Conn.Close();

        return Util.GetDataToGQGridWithPagination(QuerySelect, page, totalRegistros, Limit);
    }

    public static string getQuery(String busqueda,String Estado,String tipo)
    {            
        String Puesto = HttpContext.Current.Session["idPuesto"].ToString();
        String sQuery = "";
        String ByEstado = "";
        if (!Estado.Equals("")) {
            ByEstado = " AND UPPER(tOr.Estado) = UPPER('"+ Estado + "') ";
        }
        
       // String where = " WHERE tOr.Puesto='" + Puesto + "' AND UPPER(tOr.Estado) = UPPER('Pendiente') AND tOr.idPais = 'SV' ";
        String where = " WHERE tOr.Puesto='" + Puesto + "' AND tOr.idPais = 'SV' "+ ByEstado;
        if (!busqueda.Equals(""))
        {
            List<String> EsFechaValida = FechaValida(busqueda);
            if (EsFechaValida[0].Equals("true"))
            {
                String separador = EsFechaValida[1];
                where += " AND format(tOr.Fecha,'dd" + separador + "MM" + separador + "yyyy') = '" + busqueda + "' ";
            }
            else
            {
                where += " AND (upper(tOr.NOrdenCiex) like '%" + busqueda + "%' or upper(tbCli.Nombre) like '%" + busqueda + "%' ";
                where += " or upper(tbMag.Tratamiento) like '%" + busqueda + "%' or upper(tOr.TipoCertificado) like '%" + busqueda + "%' ";
                where += " or upper(tOr.Placa) like '%" + busqueda + "%' or upper(tOr.Vapor) like '%" + busqueda + "%') ";
            }
        }

        if (tipo.Equals("select"))
        {
            sQuery = " SELECT tOr.Fecha,NOrdenCiex,TipoCertificado,Nombre cliente,InspectorMAG responsableMag,LTRIM(tOr.Placa)+' '+LTRIM(tOr.Vapor) PlacaVapor,tOr.Estado,fPagoCiex fechaHoraPago,tOr.Total,tOr.NCompranteCiex,tOr.id,tOr.NCertificado ";
        }
        if (tipo.Equals("count")) {
            sQuery = " SELECT COUNT(tOr.id) totalRegistros ";
        }
        sQuery += " FROM tblOrdenPagoCiex tOr INNER JOIN tblCliente tbCli ON tOr.Puesto=tbCli.Puesto AND tOr.idPais=tbCli.idPais AND tOr.Cliente=tbCli.Cliente ";
        sQuery += " INNER JOIN tblOrdenMAG tbMag ON tOr.Puesto=tbMag.Puesto AND tOr.NOrden=tbMag.NoOrdenMAG ";
        sQuery += where;

        return sQuery;
    }

    [WebMethod()]
    public static string GenerarCertificado(int IdPagoOrdenCIEX)
    {
        Utilidades Util = new Utilidades();
        return Util.GenerarCertificado(IdPagoOrdenCIEX);
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

    public static string GetPuesto()
    {
        Utilidades Util = new Utilidades();
        string Puesto = Util.ObtenerValorVariable(HttpContext.Current.Request.QueryString["pto"]);
        return Puesto;
    }

    //Metodos que sirven para los diferentes tipos de certificados.
    public static string GetNoOrden()
    {
        Utilidades Util = new Utilidades();
        string NoOrden = Util.ObtenerValorVariable(HttpContext.Current.Request.QueryString["ord"]);
        return NoOrden;
    }

    [WebMethod()]
    public static List<Dictionary<string, object>> GetServiciosCIEX(string Pto, string Ord)
    {
        Utilidades Util = new Utilidades();
        string Puesto = Util.ObtenerValorVariable(Pto);
        string NoOrden = Util.ObtenerValorVariable(Ord);
        return Util.GetDataTblOrdenMAGDet(Puesto, NoOrden);
    }

    [WebMethod()]
    public static string AccionBandejaCIEX(string Pto, string Ord)
    {
        if (Pto.ToLower().Equals("false") && Ord.ToLower().Equals("false"))
        {
            return "ProcesoNormal";
        }
        Utilidades Util = new Utilidades();
        string Puesto = Util.ObtenerValorVariable(Pto);
        string NoOrden = Util.ObtenerValorVariable(Ord);
        return Util.Acciones(Puesto, NoOrden);
    }

    [WebMethod()]
    public static string Prueba(object Arr)
    {
        //JavaScriptSerializer obj = new JavaScriptSerializer();
        //Pojo PObject = obj.ConvertToType<Pojo>(Arr);
        //return PObject.Encabezado.Nombre;
        return "OK";
    }

    [WebMethod()]
    public static string GuardarCIEX(object Arr)
    {
        Utilidades Util = new Utilidades();
        return Util.GuardarOrdenCIEX(Arr);
    }

    [WebMethod()]
    public static string ObtenerOrdenMAG(string Ord)
    {
        Utilidades Util = new Utilidades();
        return Util.ObtenerValorVariable(Ord);
    }

}

//public class Pojo
//{
//    public Encabezado Encabezado;
//}

//public class Encabezado
//{
//    public string Nombre;
//}
