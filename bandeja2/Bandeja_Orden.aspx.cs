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

public partial class Bandeja_Orden : System.Web.UI.Page
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

        return Util.GetDataToBandeja(getQuery(Busqueda));
    }

    public static string getQuery(String busqueda)
    {         
        String Puesto = HttpContext.Current.Session["idPuesto"].ToString();
        String sQuery = "";
        String where = " WHERE Puesto='"+ Puesto + "' ";
        if (!busqueda.Equals(""))
        {
            List<String> EsFechaValida = FechaValida(busqueda);
            if (EsFechaValida[0].Equals("true"))
            {
                String separador = EsFechaValida[1];
                where += " AND format(FechaOrden,'dd" + separador + "MM" + separador + "yyyy') = '" + busqueda + "' ";
            }
            else
            {
                where += " AND (upper(NoOrdenMag) like '%" + busqueda + "%' or upper(Cliente) like '%" + busqueda + "%' ";
                where += " or upper(Tratamiento) like '%" + busqueda + "%' ";
                where += " or upper(Placa) like '%" + busqueda + "%' or upper(Vapor) like '%" + busqueda + "%') ";
            }
        }
       
            sQuery = "SELECT FechaOrden,NoOrdenMag,OperacionMAG,Tratamiento,Puesto,Cliente,InspectorMAG,Placa+' '+Vapor PlacaVapor,Naturaleza Producto  FROM tblOrdenMAG ";
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

}
