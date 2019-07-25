using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

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
        String Select = "SELECT Cantidad,QuimicoOirsaDescripcion,DosisOirsaDescripcion,CONVERT(VARCHAR, Tiempo)+' '+UnidadTiempo Tiempo,UnidadTiempo,Origen,Destino,Procedencia,producto,ListaProductosOrigen ";
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

    public List<Dictionary<string, object>> GetDataToBandeja(String Query)
    {
        Funciones fn = new Funciones();
        DataTable dt = new DataTable();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        String Clave = "BandejaOrden";
        SqlConnection Conn = fn.ConnectionSql();        
        SqlCommand cmSQL = new SqlCommand(Query, Conn);
        SqlDataAdapter da = new SqlDataAdapter(cmSQL);
        da.Fill(dt);

        Dictionary<string, object> row;
        foreach (DataRow dr in dt.Rows)
        {
            row = new Dictionary<string, object>();
            foreach (DataColumn col in dt.Columns)
            {
                row.Add(col.ColumnName, dr[col]);

                if ("Puesto".Equals(col.ColumnName))
                {
                    object NoOrden = dr[col];
                    row.Add("PuestoEncryp", EncryptToBase64String(NoOrden.ToString(), Clave));
                }

                if ("NoOrdenMag".Equals(col.ColumnName)) {
                    object NoOrden = dr[col];
                    row.Add("NoOrdenEncryp", EncryptToBase64String(NoOrden.ToString(), Clave));                    
                }
                
            }
            rows.Add(row);
        }
        Conn.Close();
        return rows;
    }

    public List<Dictionary<string, object>> GetDataToGQGrid(String Query)
    {
        Funciones fn = new Funciones();
        DataTable dt = new DataTable();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        SqlConnection Conn = fn.ConnectionSql();
        SqlCommand cmSQL = new SqlCommand(Query, Conn);
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

    public Dictionary<string, object> GetDataToGQGridWithPagination(String Query, int Page, int TotalRegistros, int Limit)
    {
        Funciones fn = new Funciones();
        DataTable dt = new DataTable();
        Dictionary<string, object> rows = new Dictionary<string, object>();
        List<Dictionary<string, object>> rows2 = new List<Dictionary<string, object>>();
        SqlConnection Conn = fn.ConnectionSql();
        SqlCommand cmSQL = new SqlCommand(Query, Conn);
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
            rows2.Add(row);
        }
        Conn.Close();

        row = new Dictionary<string, object>();

        double TotalPag = ((double)TotalRegistros / (double)Limit);
        int TotalPaginas = Convert.ToInt32(Math.Ceiling((TotalPag)));

        if (Page > TotalPaginas) Page = TotalPaginas;

        row.Add("records", TotalRegistros);
        row.Add("page", Page);
        row.Add("total", TotalPaginas);
        row.Add("rows", rows2);

        return row;
    }

    public string GuardarOrdenCIEX(object Objeto) {
        JavaScriptSerializer obj = new JavaScriptSerializer();
        Servicio Servicio = obj.ConvertToType<Servicio>(Objeto);

        List<DetalleServicio> Detalle = Servicio.Detalle;

        foreach (DetalleServicio Det in Detalle)
        {
            DetalleServicio DS = Det;
            Console.WriteLine(Det.Destino);
        }

        InsertarDb(Servicio);

        return Servicio.Enca.Puesto;
    }

    public string InsertarDb(Servicio Servicio) {
        Funciones fn = new Funciones();        
        
        SqlCommand Command = new SqlCommand();
        SqlCommand CommandSelect = new SqlCommand();
        //SqlCommand CommandCambiarEstado = new SqlCommand();
        SqlTransaction Transaction;
        SqlConnection Conn = fn.ConnectionSql();
        Servicio.Enca.Puesto = HttpContext.Current.Session["idPuesto"].ToString();
        Servicio.Enca.idpais = HttpContext.Current.Session["idPais"].ToString();
        Servicio.Enca.Responsable = HttpContext.Current.Session["login"].ToString();
        //Servicio.Enca.NordenCIEX = ObtenerValorVariable(Servicio.Enca.NordenCIEX);

        decimal Total = 0;
        List<DetalleServicio> Detalle = Servicio.Detalle;
        foreach (DetalleServicio det in Detalle)
        {
            Total = Total + det.Total;
        }
        Servicio.Enca.Total = Total;

        Transaction = Conn.BeginTransaction();
        try
        {            
            Command.Connection = Conn;
            Command.Transaction = Transaction;

            CommandSelect.Connection = Conn;
            CommandSelect.Transaction = Transaction;

            //CommandCambiarEstado.Connection = Conn;
            //CommandCambiarEstado.Transaction = Transaction;
            GenerarNoOrdenCIEX(Conn, Transaction, Servicio);

            int IdEncabezado = GuardarEncabezado(Command, Servicio, CommandSelect);

            GuardarDetalle(Conn, Transaction, Servicio, IdEncabezado);

            CambiarEstadoOrdenMAG(Conn, Transaction, Servicio);

            Transaction.Commit();
        }
        catch (SqlException sqlex)
        {
            Transaction.Rollback();
            throw new Exception("Error: "+ sqlex, sqlex);
        }
        catch (Exception e) {
            throw new Exception("Error: " + e, e);
        }
        finally
        {
            Conn.Close();
        }

        return null;
    }

    private int GuardarEncabezado(SqlCommand Command, Servicio Servicio, SqlCommand CommandSelect) {
        int response = 0;
        String queryInsertEncabezado = " insert into tblOrdenPagoCiex( id, Puesto, Cortesia, Local_, Anulado, Remesado, Replicado, NEnd, idPais, Fecha, Cambio, Total, TotalString, Observacion, Responsable, TipoCertificado, TipoCliente, ClienteExtra, Cliente, Vapor, NAduana, Placa, Impuesto, FechaTrat, FechaTrat_Fin, FOrden, FechaAtraque, Credito, NOrden, Estado, AOrden, Cuarentena, IDFactura, NViaje, Ingles, NOrdenCiex) ";
        queryInsertEncabezado += "VALUES (Next value FOR sq_OrdenPagoCiex,@Puesto,@Cortesia,@Local_,@Anulado,@Remesado,@Replicado,@NEnd,@idPais,@Fecha,@Cambio,@Total,@TotalString,@Observacion,@Responsable,@TipoCertificado,@TipoCliente,@ClienteExtra,@Cliente,@Vapor,@NAduana,@Placa,@Impuesto,@FechaTrat,@FechaTrat_Fin,@FOrden,@FechaAtraque,@Credito,@NOrden,@Estado,@AOrden,@Cuarentena,@IDFactura,@NViaje,@Ingles,@NOrdenCiex)";
        
        Command.CommandText = queryInsertEncabezado;

        Command.Parameters.Add("@Puesto", SqlDbType.VarChar);
        Command.Parameters["@Puesto"].Value = Servicio.Enca.Puesto;
        Command.Parameters.Add("@Cortesia", SqlDbType.Bit);
        Command.Parameters["@Cortesia"].Value = Servicio.Enca.Cortesia;
        Command.Parameters.Add("@Local_", SqlDbType.Bit);
        Command.Parameters["@Local_"].Value = Servicio.Enca.Local;
        Command.Parameters.Add("@Anulado", SqlDbType.Bit);
        Command.Parameters["@Anulado"].Value = Servicio.Enca.Anulado;
        Command.Parameters.Add("@Remesado", SqlDbType.Bit);
        Command.Parameters["@Remesado"].Value = Servicio.Enca.Remesado;
        Command.Parameters.Add("@Replicado", SqlDbType.Bit);
        Command.Parameters["@Replicado"].Value = Servicio.Enca.Replicado;
        Command.Parameters.Add("@NEnd", SqlDbType.Bit);
        Command.Parameters["@NEnd"].Value = Servicio.Enca.Nend;
        Command.Parameters.Add("@idPais", SqlDbType.VarChar);
        Command.Parameters["@idPais"].Value = Servicio.Enca.idpais;
        Command.Parameters.Add("@Fecha", SqlDbType.DateTime);
        Command.Parameters["@Fecha"].Value = DateTime.Now;
        Command.Parameters.Add("@Cambio", SqlDbType.Real);
        Command.Parameters["@Cambio"].Value = Servicio.Enca.Cambio;
        Command.Parameters.Add("@Total", SqlDbType.Decimal);
        Command.Parameters["@Total"].Value = Servicio.Enca.Total;
        Command.Parameters.Add("@TotalString", SqlDbType.VarChar);
        Command.Parameters["@TotalString"].Value = Servicio.Enca.Totalstring;
        Command.Parameters.Add("@Observacion", SqlDbType.NVarChar);
        Command.Parameters["@Observacion"].Value = Servicio.Enca.Observacion;
        Command.Parameters.Add("@Responsable", SqlDbType.VarChar);
        Command.Parameters["@Responsable"].Value = Servicio.Enca.Responsable;        
        Command.Parameters.Add("@TipoCertificado", SqlDbType.VarChar);
        Command.Parameters["@TipoCertificado"].Value = Servicio.Enca.Tipocertificado;
        Command.Parameters.Add("@TipoCliente", SqlDbType.Char);
        Command.Parameters["@TipoCliente"].Value = Servicio.Enca.Tipocliente;
        Command.Parameters.Add("@ClienteExtra", SqlDbType.VarChar);
        Command.Parameters["@ClienteExtra"].Value = Servicio.Enca.Clienteextra;
        Command.Parameters.Add("@Cliente", SqlDbType.VarChar);
        Command.Parameters["@Cliente"].Value = Servicio.Enca.Cliente;
        Command.Parameters.Add("@Vapor", SqlDbType.VarChar);
        Command.Parameters["@Vapor"].Value = Servicio.Enca.Vapor;
        Command.Parameters.Add("@NAduana", SqlDbType.NVarChar);
        Command.Parameters["@NAduana"].Value = Servicio.Enca.Naduana;
        Command.Parameters.Add("@Placa", SqlDbType.Char);
        Command.Parameters["@Placa"].Value = Servicio.Enca.Placa;
        Command.Parameters.Add("@Impuesto", SqlDbType.Real);
        Command.Parameters["@Impuesto"].Value = Servicio.Enca.Impuesto;
        Command.Parameters.Add("@FechaTrat", SqlDbType.DateTime);
        Command.Parameters["@FechaTrat"].Value = Servicio.Enca.Fechatrat;
        //Command.Parameters.Add("@FechaTrat_Fin", SqlDbType.DateTime);
        //Command.Parameters["@FechaTrat_Fin"].Value = Servicio.Enca.FechatratFin;        
        if (!Servicio.Enca.FechatratFin.Trim().Equals(""))
        {
            Command.Parameters.AddWithValue("@FechaTrat_Fin", Servicio.Enca.FechatratFin);
        }
        else
        {
            Command.Parameters.AddWithValue("@FechaTrat_Fin", DBNull.Value);
        }
        Command.Parameters.Add("@FOrden", SqlDbType.DateTime);
        Command.Parameters["@FOrden"].Value = DateTime.Now;        
        
        if (!Servicio.Enca.Fechaatraque.Trim().Equals(""))   
        {
            Command.Parameters.AddWithValue("@FechaAtraque", Servicio.Enca.Fechaatraque);
        }
        else
        {
            Command.Parameters.AddWithValue("@FechaAtraque", DBNull.Value);
        }
        //Command.Parameters.Add("@FechaAtraque", SqlDbType.DateTime);
        //Command.Parameters["@FechaAtraque"].Value = DateTime.Now;//buscar de donde viene
        Command.Parameters.Add("@Credito", SqlDbType.Bit);
        Command.Parameters["@Credito"].Value = Servicio.Enca.Credito;
        Command.Parameters.Add("@NOrden", SqlDbType.VarChar);
        Command.Parameters["@NOrden"].Value = Servicio.Enca.Norden;
        Command.Parameters.Add("@Estado", SqlDbType.VarChar);
        Command.Parameters["@Estado"].Value = "PENDIENTE";
        Command.Parameters.Add("@AOrden", SqlDbType.VarChar);
        Command.Parameters["@AOrden"].Value = Servicio.Enca.Aorden;
        Command.Parameters.Add("@Cuarentena", SqlDbType.VarChar);
        Command.Parameters["@Cuarentena"].Value = Servicio.Enca.Cuarentena;
        Command.Parameters.Add("@IDFactura", SqlDbType.VarChar);
        Command.Parameters["@IDFactura"].Value = Servicio.Enca.Idfactura;
        Command.Parameters.Add("@NViaje", SqlDbType.VarChar);
        Command.Parameters["@NViaje"].Value = Servicio.Enca.Nviaje;
        Command.Parameters.Add("@Ingles", SqlDbType.Bit);
        Command.Parameters["@Ingles"].Value = Servicio.Enca.Ingles;
        Command.Parameters.AddWithValue("@NOrdenCiex", Servicio.Enca.NordenCIEX);

        Command.ExecuteNonQuery();

        String querySelect = "SELECT id FROM tblOrdenPagoCiex WHERE Puesto='"+ Servicio.Enca.Puesto + "' AND NOrden='"+ Servicio.Enca.Norden + "' AND idPais='"+ Servicio.Enca.idpais + "'";
        CommandSelect.CommandText = querySelect;
        SqlDataReader reader = CommandSelect.ExecuteReader();
        while (reader.Read())
        {            
            response = Convert.ToInt32(reader["id"].ToString());            
        }
        return response;
    }

    private void GuardarDetalle(SqlConnection Conn, SqlTransaction Transaction, Servicio Servicio, int IdEncabezado) {

        List<DetalleServicio> Detalle = Servicio.Detalle;
        
        foreach (DetalleServicio det in Detalle)
        {
            SqlCommand Command = new SqlCommand();
            Command.Connection = Conn;
            Command.Transaction = Transaction;

            det.Puesto = Servicio.Enca.Puesto;
            det.IdPais = Servicio.Enca.idpais;
            det.Session = HttpContext.Current.Session["session"].ToString();
            if (det.UC == null) {
                det.UC = "";
            }
            if (det.Densidad == null)
            {
                det.Densidad = "";
            }
            if (det.Contenedor == null)
            {
                det.Contenedor = "";
            }
            if (det.Concentracion == null)
            {
                det.Concentracion = "";
            }
            if (det.Silo == null)
            {
                det.Silo = "";
            }
            if (det.UtAereacion == null)
            {
                det.UtAereacion = "";
            }

            String QueryInsertDet = "  INSERT INTO tblOrdenPagoCiexDetalle(id, Puesto, idPais, idOrdenCiex, DB, Servicio, Cantidad, SubTotal, US, Local, Plaguicida, Dosis, UD, Real, Producto, Ruta, Procedencia, Destino, TiempoExposicion, UT, Session, CantVol, UC, EnviaPlat, CantidadCubicada, Teorico, Densidad, Contenedor, Silo, LugTrat, Concentracion, Temperatura, TiempoAereacion, UT_Aereacion, Origen, TipoAvion, NVuelo, Razon, NActa) ";
            QueryInsertDet += "VALUES (Next value FOR sq_OrdenPagoCiexDetalle,@Puesto,@idPais,@idOrdenCiex,@DB,@Servicio,@Cantidad,@SubTotal,@US,@Local,@Plaguicida,@Dosis,@UD,@Real,@Producto,@Ruta,@Procedencia,@Destino,@TiempoExposicion,@UT,@Session,@CantVol,@UC,@EnviaPlat,@CantidadCubicada,@Teorico,@Densidad,@Contenedor,@Silo,@LugTrat,@Concentracion,@Temperatura,@TiempoAereacion,@UT_Aereacion,@Origen,@TipoAvion,@NVuelo,@Razon,@NActa)";
            
            Command.CommandText = QueryInsertDet;
                        
            Command.Parameters.Add("@Puesto", SqlDbType.VarChar);
            Command.Parameters["@Puesto"].Value = det.Puesto;
            //Command.Parameters.Add("@NOrden", SqlDbType.VarChar);
            //Command.Parameters["@NOrden"].Value = 123;
            Command.Parameters.Add("@idPais", SqlDbType.VarChar);
            Command.Parameters["@idPais"].Value = det.IdPais;
            Command.Parameters.Add("@idOrdenCiex", SqlDbType.Int);
            Command.Parameters["@idOrdenCiex"].Value = IdEncabezado;
            Command.Parameters.Add("@DB", SqlDbType.Bit);
            Command.Parameters["@DB"].Value = det.Db;
            Command.Parameters.Add("@Servicio", SqlDbType.VarChar);
            Command.Parameters["@Servicio"].Value = det.IdServicio;
            Command.Parameters.Add("@Cantidad", SqlDbType.Float);
            Command.Parameters["@Cantidad"].Value = det.Cantidad;
            Command.Parameters.Add("@SubTotal", SqlDbType.Decimal);
            Command.Parameters["@SubTotal"].Value = det.SubTotal;
            Command.Parameters.Add("@US", SqlDbType.Real);
            Command.Parameters["@US"].Value = det.US;
            Command.Parameters.Add("@Local", SqlDbType.Real);
            Command.Parameters["@Local"].Value = det.Local;
            Command.Parameters.Add("@Plaguicida", SqlDbType.VarChar);
            Command.Parameters["@Plaguicida"].Value = det.Plaguicida;
            Command.Parameters.Add("@Dosis", SqlDbType.Real);
            Command.Parameters["@Dosis"].Value = det.Dosis;
            Command.Parameters.Add("@UD", SqlDbType.VarChar);
            Command.Parameters["@UD"].Value = det.IdDosis;
            Command.Parameters.Add("@Real", SqlDbType.Real);
            Command.Parameters["@Real"].Value = det.Real;
            Command.Parameters.Add("@Producto", SqlDbType.SmallInt);
            Command.Parameters["@Producto"].Value = det.Producto;
            Command.Parameters.Add("@Ruta", SqlDbType.VarChar);
            Command.Parameters["@Ruta"].Value = det.Ruta;
            Command.Parameters.Add("@Procedencia", SqlDbType.VarChar);
            Command.Parameters["@Procedencia"].Value = det.Procedencia;
            Command.Parameters.Add("@Destino", SqlDbType.VarChar);
            Command.Parameters["@Destino"].Value = det.Destino;
            Command.Parameters.Add("@TiempoExposicion", SqlDbType.Real);
            Command.Parameters["@TiempoExposicion"].Value = det.TiempoExposicion;
            Command.Parameters.Add("@UT", SqlDbType.VarChar);
            Command.Parameters["@UT"].Value = det.UTiempoD;
            Command.Parameters.Add("@Session", SqlDbType.Char);
            Command.Parameters["@Session"].Value = det.Session;
            Command.Parameters.Add("@CantVol", SqlDbType.Real);
            Command.Parameters["@CantVol"].Value = det.CantVol;
            Command.Parameters.Add("@UC", SqlDbType.VarChar);
            Command.Parameters["@UC"].Value = det.UC;
            Command.Parameters.Add("@EnviaPlat", SqlDbType.Bit);
            Command.Parameters["@EnviaPlat"].Value = det.Enviaplat;
            Command.Parameters.Add("@CantidadCubicada", SqlDbType.Real);
            Command.Parameters["@CantidadCubicada"].Value = det.Cantidadcubicad;
            Command.Parameters.Add("@Teorico", SqlDbType.Real);
            Command.Parameters["@Teorico"].Value = det.Teorico;
            Command.Parameters.Add("@Densidad", SqlDbType.VarChar);
            Command.Parameters["@Densidad"].Value = det.Densidad;
            Command.Parameters.Add("@Contenedor", SqlDbType.VarChar);
            Command.Parameters["@Contenedor"].Value = det.Contenedor;
            Command.Parameters.Add("@Silo", SqlDbType.VarChar);
            Command.Parameters["@Silo"].Value = det.Silo;
            Command.Parameters.Add("@LugTrat", SqlDbType.VarChar);
            Command.Parameters["@LugTrat"].Value = det.LugarTra;
            Command.Parameters.Add("@Concentracion", SqlDbType.NVarChar);
            Command.Parameters["@Concentracion"].Value = det.Concentracion;
            Command.Parameters.Add("@Temperatura", SqlDbType.Float);
            Command.Parameters["@Temperatura"].Value = det.Temperatura;
            Command.Parameters.Add("@TiempoAereacion", SqlDbType.Real);
            Command.Parameters["@TiempoAereacion"].Value = det.Tiempoaereacion;
            Command.Parameters.Add("@UT_Aereacion", SqlDbType.VarChar);
            Command.Parameters["@UT_Aereacion"].Value = det.UtAereacion;
            Command.Parameters.Add("@Origen", SqlDbType.VarChar);
            Command.Parameters["@Origen"].Value = det.Origen;
            Command.Parameters.Add("@TipoAvion", SqlDbType.VarChar);
            Command.Parameters["@TipoAvion"].Value = det.TipoAvion;
            Command.Parameters.Add("@NVuelo", SqlDbType.VarChar);
            Command.Parameters["@NVuelo"].Value = det.NVuelo;
            Command.Parameters.Add("@Razon", SqlDbType.VarChar);
            Command.Parameters["@Razon"].Value = det.Razon;
            Command.Parameters.Add("@NActa", SqlDbType.VarChar);
            Command.Parameters["@NActa"].Value = det.Nacta;

            Command.ExecuteNonQuery();            
        }

    }

    private void GenerarNoOrdenCIEX(SqlConnection Conn, SqlTransaction Transaction, Servicio Servicio) {
        
        SqlCommand CommandSelect = new SqlCommand();
        CommandSelect.Connection = Conn;
        CommandSelect.Transaction = Transaction;
        String puesto = Servicio.Enca.Puesto;
        CommandSelect.CommandText = "SELECT max(id) ultimoRegistro,(SELECT cod_dga FROM tblPuestoCodDga WHERE puesto='" + puesto + "') cod_dga FROM tblOrdenPagoCiex WHERE Puesto='"+ puesto + "'";
        String Correlativo = "";
        String CodigoDGA = "";
        String UltimoRegistroPagoCiex = "";
        SqlDataReader reader = CommandSelect.ExecuteReader();
        while (reader.Read())
        {
            UltimoRegistroPagoCiex = reader["ultimoRegistro"].ToString();
            CodigoDGA = reader["cod_dga"].ToString();
        }
        
        if (CodigoDGA == null || "".Equals(CodigoDGA))
        {
            throw new Exception("No se ha encontrado codigo DGA.");
        }

        if (UltimoRegistroPagoCiex != null && !"".Equals(UltimoRegistroPagoCiex))
        {
            SqlCommand CommandSelectById = new SqlCommand();
            CommandSelectById.Connection = Conn;
            CommandSelectById.Transaction = Transaction;
            int id = Convert.ToInt32(UltimoRegistroPagoCiex);
            CommandSelectById.CommandText = "SELECT NOrdenCiex FROM tblOrdenPagoCiex WHERE id = " + id;

            SqlDataReader readerId = CommandSelectById.ExecuteReader();

            while (readerId.Read())
            {
                String NoOrdenPagoCiex = readerId["NOrdenCiex"].ToString();
                Correlativo = (Convert.ToInt32(NoOrdenPagoCiex.Split('-')[1]) + 1).ToString();
            }
        } else {
            Correlativo = "1";
        }


        String Mes = DateTime.Now.Month.ToString();
        if (Mes.Length == 1) {
            Mes = "0" + Mes;
        }
        String Anio = DateTime.Now.Year.ToString();

        String NoOrdenCiex = Anio + Mes + "-" + FormatoCorrelativo(Correlativo)+ "-" + CodigoDGA;
        Servicio.Enca.NordenCIEX = NoOrdenCiex;

    }

    private string FormatoCorrelativo(String corr) {
        string response = "";
        switch (corr.Length)
        {
            case 1:
                response = "0000" + corr;
                break;
            case 2:
                response = "000" + corr;
                break;
            case 3:
                response = "00" + corr;
                break;
            case 4:
                response = "0" + corr;
                break;
            case 5:
                response = corr;
                break;
        }
        return response;
    }

    private void CambiarEstadoOrdenMAG(SqlConnection Conn, SqlTransaction Transaction, Servicio Servicio) {
        SqlCommand CommandUpdate = new SqlCommand();
        //SqlCommand CommandSelect = new SqlCommand();
        String estado = "Procesado";
        CommandUpdate.Connection = Conn;
        CommandUpdate.Transaction = Transaction;
        
        String QueryUpdate = "UPDATE tblOrdenMAG SET Estado = '"+ estado + "' WHERE Puesto='" + Servicio.Enca.Puesto + "' AND NoOrdenMAG = '" + Servicio.Enca.Norden + "'";
        CommandUpdate.CommandText = QueryUpdate;

        int Update = CommandUpdate.ExecuteNonQuery();
        if (Update == 0) {            
            throw new Exception("No se actualizo el estado de la orden MAG Puesto: " + Servicio.Enca.Puesto +" No. Orden: " + Servicio.Enca.Norden);
        }
    }

    public string GenerarCertificado(int IdPagoOrdenCIEX)
    {
        Funciones fn = new Funciones();
        String Response = "Testing";
        String QueryEncabezado = "SELECT id,Puesto,NCertificado,Fecha,Cambio,Cortesia,Local_,Total,TotalString,Observacion,Responsable,Anulado,Remesado,Replicado,NEnd,TipoCertificado,TipoCliente,ClienteExtra,Cliente,Vapor,NAduana,Placa,Impuesto,IDCCaja,ADelDia,EnviaPlat,FechaTrat,FechaTrat_Fin,Credito,NOrden,FOrden,fPagoCiex,Estado,AOrden,Cuarentena,IDFactura,NViaje,FechaAtraque,idPais,Ingles,Operador,CompFiscal,NoCompFiscal,PlacaDos,NumeroBL,MigradoGP,NOrdenCiex,NCompranteCiex FROM tblOrdenPagoCiex where id = " + IdPagoOrdenCIEX;
        String QueryDetalle = "SELECT id,idOrdenCiex,Puesto,NCertificado,Servicio,Cantidad,US,Local,SubTotal,Plaguicida,Dosis,UD,Plaguicida2,Dosis2,UD2,Real,Producto,Ruta,TipoAvion,Procedencia,Destino,NVuelo,TiempoExposicion,UT,Razon,NActa,DB,Session,CantVol,UC,EnviaPlat,CantidadCubicada,Teorico,Densidad,Contenedor,Silo,LugTrat,Concentracion,Temperatura,TiempoAereacion,UT_Aereacion,Origen,idPais,MatriculaAvion,Descripcion FROM tblOrdenPagoCiexDetalle WHERE idOrdenCiex = " + IdPagoOrdenCIEX;
        SqlConnection Conn = fn.ConnectionSql();
        SITC.Certificado.CertificadosTransaction tr = new SITC.Certificado.CertificadosTransaction();
        SITC.Certificado.tblcertif Encabezado = new SITC.Certificado.tblcertif();
        String ncertificado = "";
        try
        {
            
            SqlCommand cmSQL = new SqlCommand(QueryEncabezado, Conn);
            SqlDataReader reader = cmSQL.ExecuteReader();
            while (reader.Read())
            {
                String idPais = reader["idPais"].ToString();
                String idPuesto = reader["Puesto"].ToString();
                ncertificado = (fn.Recuperar_CorrelativoC(idPais, idPuesto, "NCertificado"));
              
                if (ncertificado == "0")
                {
                    throw new Exception("Correlativo no definido! Informar al Depto. de Informatica de OIRSA Sede");
                }

                Encabezado.Ncertificado = ncertificado;
                Encabezado.Puesto = idPuesto;
                Encabezado.idpais = idPais;
                if (!"".Equals(reader["Fecha"].ToString()))
                {
                    Encabezado.Fecha = Convert.ToDateTime(reader["Fecha"].ToString());
                }
                Encabezado.Responsable = reader["Responsable"].ToString();
                Encabezado.Cambio= Convert.ToDecimal(reader["Cambio"].ToString());
                Encabezado.Cortesia = Convert.ToBoolean(reader["Cortesia"].ToString());
                Encabezado.Local = Convert.ToBoolean(reader["Local_"].ToString());
                Encabezado.Total = Convert.ToDecimal(reader["Total"].ToString());
                Encabezado.Totalstring = reader["TotalString"].ToString();
                Encabezado.Observacion = reader["Observacion"].ToString();
                Encabezado.Anulado = Convert.ToBoolean(reader["Anulado"].ToString());
                Encabezado.Remesado = Convert.ToBoolean(reader["Remesado"].ToString());
                Encabezado.Replicado = Convert.ToBoolean(reader["Replicado"].ToString());
                Encabezado.Nend = Convert.ToBoolean(reader["NEnd"].ToString());
                Encabezado.Tipocertificado = reader["TipoCertificado"].ToString();
                Encabezado.Tipocliente = reader["TipoCliente"].ToString();
                Encabezado.Clienteextra = reader["ClienteExtra"].ToString();
                Encabezado.Cliente = reader["Cliente"].ToString();
                Encabezado.Vapor = reader["Vapor"].ToString();               
                Encabezado.Naduana = reader["NAduana"].ToString();
                Encabezado.Placa = reader["Placa"].ToString();
                Encabezado.Impuesto = Convert.ToDecimal(reader["Impuesto"].ToString());
                if (!"".Equals(reader["IDCCaja"].ToString()))
                {
                    Encabezado.Idccaja = Convert.ToInt32(reader["IDCCaja"].ToString());
                }
                Encabezado.Adeldia = Convert.ToBoolean(reader["ADelDia"].ToString());
                Encabezado.Enviaplat = Convert.ToBoolean(reader["EnviaPlat"].ToString());
                if (!"".Equals(reader["FechaTrat"].ToString()))
                {
                    Encabezado.Fechatrat = Convert.ToDateTime(reader["FechaTrat"].ToString());
                }
                if (!"".Equals(reader["FechaTrat_Fin"].ToString()))
                {
                    Encabezado.FechatratFin = Convert.ToDateTime(reader["FechaTrat_Fin"].ToString());
                }             
                Encabezado.Credito = Convert.ToBoolean(reader["Credito"].ToString());
                Encabezado.Norden = reader["NOrden"].ToString();
                if (!"".Equals(reader["FOrden"].ToString()))
                {
                    Encabezado.Forden = Convert.ToDateTime(reader["FOrden"].ToString());
                }
                Encabezado.Aorden = reader["AOrden"].ToString();
                Encabezado.Cuarentena = reader["Cuarentena"].ToString();
                //
                Encabezado.Idfactura = reader["IDFactura"].ToString();
                Encabezado.Nviaje = reader["NViaje"].ToString();
                if (!"".Equals(reader["FechaAtraque"].ToString()))
                {
                    Encabezado.Fechaatraque = Convert.ToDateTime(reader["FechaAtraque"].ToString());
                }
                Encabezado.Ingles = Convert.ToBoolean(reader["Ingles"].ToString());
                Encabezado.PlacaDos = reader["PlacaDos"].ToString();
                Encabezado.NumeroBL = reader["NumeroBL"].ToString();

                //id,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,fPagoCiex,Estado,,,,,,,,Operador,CompFiscal,NoCompFiscal,,,MigradoGP,NOrdenCiex,NCompranteCiex FROM tblOrdenPagoCiex
                tr.Encabezado = Encabezado;
            }

            SqlCommand cmDetSQL = new SqlCommand(QueryDetalle, Conn);            
            SqlDataAdapter da = new SqlDataAdapter(cmDetSQL);
            DataTable dt = new DataTable();
            da.Fill(dt);

            //se crea vector con la cantidad de registros a guardar.
            int CantidadRegistros = dt.Rows.Count;            
            SITC.Certificado.tblcertifdet[] dets= new SITC.Certificado.tblcertifdet[CantidadRegistros];

            int Index = 0;
            foreach (DataRow row in dt.Rows)
            {
                SITC.Certificado.tblcertifdet Det = new SITC.Certificado.tblcertifdet();
                //Det.Contenedor = "Contenedor de Prueb c#";
                
                Det.Puesto= row["Puesto"].ToString();
                Det.Ncertificado = ncertificado;
                Det.Servicio = row["Servicio"].ToString();
                Det.Cantidad = (float) Convert.ToDouble(row["Cantidad"].ToString());
                Det.Us = Convert.ToDecimal(row["US"].ToString());
                Det.Local = Convert.ToDecimal(row["Local"].ToString());
                Det.Subtotal = Convert.ToDecimal(row["SubTotal"].ToString());
                Det.Plaguicida = row["Plaguicida"].ToString();
                Det.Dosis = Convert.ToDecimal(row["Dosis"].ToString());
                Det.Ud = row["UD"].ToString();
                Det.Plaguicida2 = row["Plaguicida2"].ToString();
                if (!"".Equals(row["Dosis2"].ToString()))
                {
                    Det.Dosis2 = Convert.ToDecimal(row["Dosis2"].ToString());
                }
                Det.Ud2 = row["UD2"].ToString();
                Det.Real = Convert.ToDecimal(row["Real"].ToString());
                Det.Producto = Convert.ToInt32(row["Producto"].ToString());
                Det.Ruta = row["Ruta"].ToString();
                Det.Tipoavion = row["TipoAvion"].ToString();
                Det.Procedencia = row["Procedencia"].ToString();
                Det.Destino = row["Destino"].ToString();
                Det.Nvuelo = row["NVuelo"].ToString();
                Det.Tiempoexposicion = Convert.ToDecimal(row["TiempoExposicion"].ToString());
                Det.Ut = row["UT"].ToString();
                Det.Razon = row["Razon"].ToString();
                Det.Nacta = row["NActa"].ToString();
                Det.Db = Convert.ToBoolean(row["DB"].ToString());
                Det.Session = row["Session"].ToString();
                Det.Cantvol = Convert.ToDecimal(row["CantVol"].ToString());
                Det.Uc = row["UC"].ToString();
                Det.Enviaplat = Convert.ToBoolean(row["EnviaPlat"].ToString());
                Det.Cantidadcubicada = Convert.ToDecimal(row["CantidadCubicada"].ToString());
                Det.Teorico = Convert.ToDecimal(row["Teorico"].ToString());
                Det.Densidad = row["Densidad"].ToString();
                Det.Contenedor = row["Contenedor"].ToString();
                Det.Silo = row["Silo"].ToString();
                Det.Lugtrat = row["LugTrat"].ToString();
                //
                Det.Concentracion = row["Concentracion"].ToString();
                Det.Temperatura = (float)Convert.ToDouble(row["Temperatura"].ToString());
                Det.Tiempoaereacion = Convert.ToDecimal(row["TiempoAereacion"].ToString());
                Det.UtAereacion = row["UT_Aereacion"].ToString();
                Det.Origen = row["Origen"].ToString();
                Det.idpais = row["idPais"].ToString();
                Det.MatriculaAvion = row["MatriculaAvion"].ToString();
                //id,idOrdenCiex,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,Descripcion

                dets[Index] = Det;
                Index++;
            }
            
            tr.Detalle = dets;

            String UpdateEncabezado = "UPDATE tblOrdenPagoCiex SET NCertificado='"+ ncertificado + "',Estado='PROCESADO' WHERE ID = " + IdPagoOrdenCIEX;
            String UpdateDetalle = "UPDATE tblOrdenPagoCiexDetalle SET NCertificado='"+ ncertificado +"' WHERE idOrdenCiex = " + IdPagoOrdenCIEX;

            tr.GuardarCIEX(UpdateEncabezado, UpdateDetalle);

        }
        catch (SqlException sqlex)
        {
            
            throw new Exception("Error: " + sqlex, sqlex);
        }
        catch (Exception e)
        {
            throw new Exception("Error: " + e, e);
        }
        finally
        {
            Conn.Close();
        }

        return Response;
    }
    
}



public class Servicio
{
    public Enca Enca;
    public List<DetalleServicio> Detalle;
}

public class Enca
{
    public string Puesto;//1
    public string Fecha;//2
    public Single Cambio;//3
    public string Cortesia;//4
    public string Local;//5
    public decimal Total;//6
    public string Totalstring;//7
    public string Observacion;//8
    public string Responsable;//9
    public bool Anulado;//10
    public bool Remesado;//11
    public bool Replicado;//12
    public bool Nend;//13
    public string Tipocertificado;//14
    public string Tipocliente;//15
    public string Clienteextra;//16
    public string Cliente;//17
    public string Vapor;//18
    public string Naduana;//19
    public string Placa;//20
    public double Impuesto;//21
    public int Idccaja;//22
    public bool Adeldia;//23
    public bool Enviaplat;//24
    public string Fechatrat;//25
    public string FechatratFin;//26
    public string Credito;//27
    public string Norden;//28
    public string Forden;//29, fPagoCiex, Estado
    public string Aorden;//32
    public string Cuarentena;//33
    public string Idfactura;//34
    public string Nviaje;//35
    public string Fechaatraque;//36
    public string idpais;//37
    public bool Ingles;//38
    public string NombreCliente;
    public string FormaPago;
    public string PlacaDos;
    public string NumeroBL;
    public string NordenCIEX;

}

public class DetalleServicio
{
    public string Servicio;
    public float Cantidad;
    public double US;
    public double Local;
    public decimal Total;
    public string PlaguicidaN;
    public string Plaguicida;
    public string IdServicio;//=Servicio
    public string IdPlaguicida;
    public decimal SubTotal;
    public double Dosis;
    public string IdDosis;//=UD
    public int Producto;
    public string Ruta;
    public string TipoAvion;
    public string Procedencia;
    public string Destino;
    public string NVuelo;
    public double TiempoExposicion;
    public string UTiempo;
    public string Matricula;
    public string Razon;
    public string LugarTra;
    public string Origen;
    public string IdPais;
    public string UTiempoD;//=Ut
    public double Cantidadcubicad;
    public float Temperatura;
    public double Tiempoaereacion;
    public string UtAereacion;
    public string Densidad;
    public double Teorico;
    public double CantVol;
    public string UC;
    public string Contenedor;
    public string Silo;
    public string Concentracion;
    public double Real;
    public string MensajeJC;
    public string Puesto;    
    public string Session;
    public bool Db;
    public bool Enviaplat;
    public string Nacta;
}