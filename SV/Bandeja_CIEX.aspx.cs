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
    public static List<Dictionary<string, object>> GetBandejaData(string Busqueda)
    {
        Utilidades Util = new Utilidades();
        //String Query = "SELECT FechaOrden,NoOrdenMag,OperacionMAG,Tratamiento,Puesto,Cliente,InspectorMAG,Placa,Vapor,Naturaleza Producto  FROM tblOrdenMAG";

        return Util.GetDataToGQGrid(getQuery(Busqueda));
    }

    public static string getQuery(String busqueda)
    {         
        String Puesto = HttpContext.Current.Session["idPuesto"].ToString();
        String sQuery = "";
        String where = " WHERE tOr.Puesto='" + Puesto + "' AND UPPER(tOr.Estado) = UPPER('Pendiente') AND tOr.idPais = 'SV' ";
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

        //sQuery = "SELECT FechaOrden,NoOrdenMag,OperacionMAG,Tratamiento,Puesto,Cliente,InspectorMAG,Placa+' '+Vapor PlacaVapor,Naturaleza Producto  FROM tblOrdenMAG ";
        // sQuery = "SELECT Fecha,NOrdenCiex,TipoCertificado,(SELECT Nombre FROM tblCliente WHERE Cliente=tOr.Cliente AND Puesto=tOr.Puesto AND idPais=tOr.idPais) cliente, (SELECT InspectorMAG FROM tblOrdenMAG WHERE Puesto=tOr.Puesto AND NoOrdenMAG=tOr.NOrden) responsableMag,Placa+' '+Vapor PlacaVapor,Estado,fPagoCiex fechaHoraPago FROM tblOrdenPagoCiex tOr ";
        sQuery = " SELECT tOr.Fecha,NOrdenCiex,TipoCertificado,Nombre cliente,InspectorMAG responsableMag,tOr.Placa+' '+tOr.Vapor PlacaVapor,tOr.Estado,fPagoCiex fechaHoraPago ";
        sQuery += " FROM tblOrdenPagoCiex tOr INNER JOIN tblCliente tbCli ON tOr.Puesto=tbCli.Puesto AND tOr.idPais=tbCli.idPais AND tOr.Cliente=tbCli.Cliente ";
        sQuery += " INNER JOIN tblOrdenMAG tbMag ON tOr.Puesto=tbMag.Puesto AND tOr.NOrden=tbMag.NoOrdenMAG ";
        sQuery += where;

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
