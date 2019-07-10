using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for Utilidades
/// </summary>
public class Utilidades
{
    public Utilidades()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string EncryptToBase64String(string StrCadena,string Clave)
    {
        try
        {
            byte[] claveBytes = Convert.FromBase64String(Clave);
            string strKey = Convert.ToBase64String(claveBytes);

            RijndaelManaged objProvider = GetInstance(strKey);

            ICryptoTransform objCrypto = objProvider.CreateEncryptor();
            byte[] arrBytBuffer = Encoding.Unicode.GetBytes(StrCadena);

            // Devuelve el array de bytes encriptado
            byte[] EncriptadoBytes = objCrypto.TransformFinalBlock(arrBytBuffer, 0, arrBytBuffer.Length);
            return Convert.ToBase64String(EncriptadoBytes);
        }
        catch (Exception e)
        {            
            return "Error: "+ e;
        }
    }

    public string DecryptFromBase64String(string StrCadena, string Clave)
    {
        try
        {
            byte[] arrBytBuffer = Convert.FromBase64String(StrCadena);

            byte[] claveBytes = Convert.FromBase64String(Clave);
            string strKey = Convert.ToBase64String(claveBytes);

            RijndaelManaged objProvider = GetInstance(strKey);
            ICryptoTransform objCrypto = objProvider.CreateDecryptor();

            // Devuelve la cadena desencriptada
            return Encoding.Unicode.GetString(objCrypto.TransformFinalBlock(arrBytBuffer, 0,
                                              arrBytBuffer.Length));
        }
        catch (Exception e) {
            return "Error desencriptando: " + e;
        }
    }

    private RijndaelManaged GetInstance(string Clave)
    {
        RijndaelManaged objProvider = new RijndaelManaged();

        // Inicializa el proveedor
        objProvider.Key = Encoding.Unicode.GetBytes(Clave);
        objProvider.IV = new byte[objProvider.BlockSize / 8];
        // Devuelve el proveedor
        return objProvider;
    }

    public String ObtenerValorVariable(String Variable) {
       
            if (Variable == null) {
                return "variableNoExiste";
            }
            Variable = Variable.Replace(" ", "+");
            String variableValue = DecryptFromBase64String(Variable, "BandejaOrden");
        
        if (variableValue.StartsWith("Error")) {
            return "variableAlterada";
        }
            return variableValue;
    }

    public String Acciones(string Puesto, string Orden) {
        if (Puesto.Equals("variableNoExiste") && Orden.Equals("variableNoExiste"))
        {
            return "ProcesoNormal";
        }
        if (Puesto.Equals("variableAlterada") 
          || Orden.Equals("variableAlterada")
         || Puesto.Equals("variableNoExiste")
          || Orden.Equals("variableNoExiste")) {
            return "HacerRedirect";
        }
        
        return "ProcesoBandeja";
    }

    public List<Dictionary<string, object>> GetDataTblOrdenMAGDet(String Puesto, String NoOrden)
    {
        Funciones fn = new Funciones();
        DataTable dt = new DataTable();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        SqlConnection Conn = fn.ConnectionSql();       
        String Select = "SELECT Cantidad,QuimicoOirsaDescripcion,DosisOirsaDescripcion,CONVERT(VARCHAR, Tiempo)+' '+UnidadTiempo Tiempo,UnidadTiempo,Origen,Destino,Procedencia,producto,ListaProductosOrigen,Estado ";
        String sQuery = Select+ " FROM tblOrdenMAGDetalle WHERE PuestoFk='"+ Puesto + "' AND NoOrdenMAGFk='"+ NoOrden + "'";        
        SqlCommand cmSQL = new SqlCommand(sQuery, Conn);
        SqlDataAdapter da = new SqlDataAdapter(cmSQL);
        da.Fill(dt);

        Dictionary<string, object> row;
        foreach (DataRow dr in dt.Rows)
        {
            row = new Dictionary<string, object>();
            foreach (DataColumn col in dt.Columns)
            {
                row.Add(col.ColumnName, dr[col]);
            }
            rows.Add(row);
        }
        Conn.Close();
        return rows;
    }
}



public class DetalleBandeja {
    public int IdDetalle;
    public int Cantidad;    
}