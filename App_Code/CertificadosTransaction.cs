using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
namespace SITC.Certificado {
    /// <summary>
    /// Summary description for CertificadosTransaction
    /// </summary>
    public class CertificadosTransaction {
        tblcertif _Encabezado;
        tblcertifdet[] _Detalle;
        Int16 _PostFecha = 0;
        public Int16 PostFecha { get { return _PostFecha; } set { _PostFecha = value; } }
        public CertificadosTransaction () { }
        public tblcertif Encabezado { get { return _Encabezado; } set { _Encabezado = value; } }
        public tblcertifdet[] Detalle { get { return _Detalle; } set { _Detalle = value; } }
        private void ProcesarInventario (ref Funciones fn, tblcertifdet det) {
            string Kardex = "select ConsumoAutomatico from tblPlaguicida where idPais='" + det.idpais + "' and plaguicida='" + det.Plaguicida + "'";
            string valido = fn.EjecutarScalarString (Kardex);
            if (valido.ToLower () == "true" && det.Plaguicida.Trim () != "") {
                decimal Exitencia = (decimal) fn.Plaguicida_Existencia (det.Plaguicida, det.Puesto, det.idpais);
                if (Exitencia < det.Real) {
                    throw new Exception ("Imposible finalizar operacion, existencias insuficientes.");
                } else {
                    decimal nExistencia = Exitencia - det.Real;
                    string id = this.Encabezado.Fecha.Year.ToString () + this.Encabezado.Fecha.Month.ToString () + this.Encabezado.Fecha.Day.ToString ();
                    id = id + "-" + fn.Recuperar_Correlativo (det.idpais, det.Puesto, "IdKardex").ToString ();
                    string Sql = "select Costo from tblPlaguicida where idPais='" + det.idpais + "' and plaguicida='" + det.Plaguicida + "'";
                    Double costo = fn.EjecutarScalarDouble (Sql);
                    Sql = "insert into tblInvKardex (Puesto,id,Plaguicida,Existencia,Egreso,Ingreso,NuevaExistencia,Responsable,Fecha,Procesado,Replicado,Concepto,Certificados,Costo,FechaDigitacion,idDetFum,[Session],idPais)";
                    Sql = Sql + "values(@Puesto,@id,@Plaguicida,@Existencia,@Egreso,@Ingreso,@NuevaExistencia,@Responsable,@Fecha,@Procesado,@Replicado,@Concepto,@Certificados,@Costo,@FechaDigitacion,@idDetFum,@Session,@idPais)";
                    SqlCommand cm = new SqlCommand (Sql, fn._LocalC, fn._tr);
                    cm.Parameters.AddWithValue ("@Puesto", det.Puesto);
                    cm.Parameters.AddWithValue ("@id", id);
                    cm.Parameters.AddWithValue ("@Plaguicida", det.Plaguicida);
                    cm.Parameters.AddWithValue ("@Existencia", Exitencia);
                    cm.Parameters.AddWithValue ("@Egreso", det.Real);
                    cm.Parameters.AddWithValue ("@Ingreso", 0);
                    cm.Parameters.AddWithValue ("@NuevaExistencia", nExistencia);
                    cm.Parameters.AddWithValue ("@Responsable", HttpContext.Current.Session["login"]);
                    cm.Parameters.AddWithValue ("@Fecha", this.Encabezado.Fecha);
                    cm.Parameters.AddWithValue ("@Procesado", 1);
                    cm.Parameters.AddWithValue ("@Replicado", 0);
                    cm.Parameters.AddWithValue ("@Concepto", string.Format ("Consumo automatico por fumigación [NCertificado:{0}]", det.Ncertificado));
                    cm.Parameters.AddWithValue ("@Certificados", det.Ncertificado);
                    cm.Parameters.AddWithValue ("@Costo", costo);
                    cm.Parameters.AddWithValue ("@FechaDigitacion", DateTime.Now);
                    cm.Parameters.AddWithValue ("@idDetFum", det.Detalle);
                    cm.Parameters.AddWithValue ("@Session", HttpContext.Current.Session["session"]);
                    cm.Parameters.AddWithValue ("@idPais", det.idpais);
                    cm.ExecuteNonQuery ();
                    fn.Incrementar_Correlativo (det.idpais, det.Puesto, "IdKardex");
                    Sql = "update tblInvExistencia set Existencia=@Existencia where Plaguicida=@Plaguicida and Puesto=@Puesto and idPais=@IdPais";
                    SqlCommand cmExistencia = new SqlCommand (Sql, fn._LocalC, fn._tr);
                    cmExistencia.Parameters.AddWithValue ("@Existencia", nExistencia);
                    cmExistencia.Parameters.AddWithValue ("@Plaguicida", det.Plaguicida);
                    cmExistencia.Parameters.AddWithValue ("@Puesto", det.Puesto);
                    cmExistencia.Parameters.AddWithValue ("@IdPais", det.idpais);
                    cmExistencia.ExecuteNonQuery ();
                }
            }
        }
        public void Guardar () {
            Funciones fn = new Funciones ();
            try {
                DataGrid dr = new DataGrid ();
                tblcertifmdp mdp = new tblcertifmdp ();
                mdp.Mdp = Encabezado.Credito == true ? Int16.Parse ("1") : Int16.Parse ("0");
                mdp.Ncertificado = this.Encabezado.Ncertificado;
                mdp.Puesto = Encabezado.Puesto;
                mdp.Idabono = 0;
                mdp.idpais = Encabezado.idpais;
                mdp.Cantidad = Encabezado.Total;
                mdp.Abono = 0;
                mdp.Saldonuevo = Encabezado.Total;
                mdp.Nend = false;
                mdp.Enviaplat = false;
                if (Encabezado.FormaPago == "-1") {
                    mdp.Mdp = 10;
                    mdp.Banco = Encabezado.AutorizacionBanco;
                }
                fn.ConnectionSql ();
                fn.BeginTransaction ();
                SqlCommand con = this.Encabezado.getComandQuery (fn._LocalC, TipoTransaccion.Insert);
                con.Transaction = fn._tr;
                con.ExecuteNonQuery ();
                string cAuto = fn.ValIni (Encabezado.idpais, "ConsumoAutomatico");
                foreach (tblcertifdet det in this.Detalle) {
                    if (det.Servicio != "CuentaRecargo") {
                        SqlCommand cond = det.getComandQuery (fn._LocalC, TipoTransaccion.Insert);
                        cond.Transaction = fn._tr;
                        cond.ExecuteNonQuery ();
                        det.Detalle = det.GetLasIdentity (fn._LocalC, fn._tr);
                        if (cAuto == "1") {
                            this.ProcesarInventario (ref fn, det);
                        }
                    } else {
                        fn.RegistrarRecargo (this.Encabezado.Ncertificado, det.Local.ToString ());
                    }
                }
                SqlCommand cmMdp = mdp.getComandQuery (fn._LocalC, TipoTransaccion.Insert);
                cmMdp.Transaction = fn._tr;
                cmMdp.ExecuteNonQuery ();
                fn.Incrementar_Correlativo (this.Encabezado.idpais, this.Encabezado.Puesto, "NCertificado");
                //Se registra en auditoria
                fn.Auditoria (this.Detalle[0].Session, "Agregar Certificado", this.Encabezado.Ncertificado, this.Encabezado.idpais);
                fn._tr.Commit ();
            } catch (Exception ex) {
                fn._tr.Rollback ();
                fn._LocalC.Close ();
                throw ex;
            } finally {
                fn._tr.Dispose ();
                fn._LocalC.Close ();
            }
        }

        public void GuardarCIEX(String UpdateEncabezado, String UpdateDetalle)
        {
            Funciones fn = new Funciones();
            try
            {
                DataGrid dr = new DataGrid();
                tblcertifmdp mdp = new tblcertifmdp();
                mdp.Mdp = Encabezado.Credito == true ? Int16.Parse("1") : Int16.Parse("0");
                mdp.Ncertificado = this.Encabezado.Ncertificado;
                mdp.Puesto = Encabezado.Puesto;
                mdp.Idabono = 0;
                mdp.idpais = Encabezado.idpais;
                mdp.Cantidad = Encabezado.Total;
                mdp.Abono = 0;
                mdp.Saldonuevo = Encabezado.Total;
                mdp.Nend = false;
                mdp.Enviaplat = false;
                if (Encabezado.FormaPago == "-1")
                {
                    mdp.Mdp = 10;
                    mdp.Banco = Encabezado.AutorizacionBanco;
                }
                fn.ConnectionSql();
                fn.BeginTransaction();
                SqlCommand con = this.Encabezado.getComandQuery(fn._LocalC, TipoTransaccion.Insert);
                con.Transaction = fn._tr;
                con.ExecuteNonQuery();
                string cAuto = fn.ValIni(Encabezado.idpais, "ConsumoAutomatico");
                foreach (tblcertifdet det in this.Detalle)
                {
                    if (det.Servicio != "CuentaRecargo")
                    {
                        SqlCommand cond = det.getComandQuery(fn._LocalC, TipoTransaccion.Insert);
                        cond.Transaction = fn._tr;
                        cond.ExecuteNonQuery();
                        det.Detalle = det.GetLasIdentity(fn._LocalC, fn._tr);
                        if (cAuto == "1")
                        {
                            this.ProcesarInventario(ref fn, det);
                        }
                    }
                    else
                    {
                        fn.RegistrarRecargo(this.Encabezado.Ncertificado, det.Local.ToString());
                    }
                }
                SqlCommand cmMdp = mdp.getComandQuery(fn._LocalC, TipoTransaccion.Insert);
                cmMdp.Transaction = fn._tr;
                cmMdp.ExecuteNonQuery();
                fn.Incrementar_Correlativo(this.Encabezado.idpais, this.Encabezado.Puesto, "NCertificado");
                //Se registra en auditoria
                fn.Auditoria(this.Detalle[0].Session, "Agregar Certificado", this.Encabezado.Ncertificado, this.Encabezado.idpais);
               
                //Se actualizan las tablas espejos.
                //Se actualiza el encabezado
                SqlCommand CommandUpdate = new SqlCommand();
                CommandUpdate.Connection = fn._LocalC;
                CommandUpdate.Transaction = fn._tr;
                CommandUpdate.CommandText = UpdateEncabezado;
                int Update = CommandUpdate.ExecuteNonQuery();
                if (Update == 0)
                {
                    throw new Exception("Ocurrio un error al actualizar el numero de certificado en la tabla espejo tblOrdenPagoCiex");
                }

                //Se actualiza el detalle                
                SqlCommand CommandUpdateDet = new SqlCommand();
                CommandUpdateDet.Connection = fn._LocalC;
                CommandUpdateDet.Transaction = fn._tr;
                CommandUpdateDet.CommandText = UpdateDetalle;
                int UpdateDet = CommandUpdateDet.ExecuteNonQuery();
                if (UpdateDet == 0)
                {
                    throw new Exception("Ocurrio un error al actualizar el numero de certificado en la tabla espejo tblOrdenPagoCiexDetalle");
                }

                fn._tr.Commit();
            }
            catch (Exception ex)
            {
                fn._tr.Rollback();
                fn._LocalC.Close();
                throw ex;
            }
            finally
            {
                fn._tr.Dispose();
                fn._LocalC.Close();
            }
        }

    }
    public class tblcertif : EntidadBase {
        public tblcertif () {
            this.Esquema = "dbo";
            this.NombreSQL = "tblcertif";
        }
        string _Puesto;
        [AtributosBase (VarPorDefecto = "", LLavePrimaria = true, TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Puesto")]
        public string Puesto { get { return _Puesto; } set { _Puesto = value; } }
        string _Ncertificado;
        [AtributosBase (VarPorDefecto = "", LLavePrimaria = true, TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "NCertificado")]
        public string Ncertificado { get { return _Ncertificado; } set { _Ncertificado = value; } }
        DateTime _Fecha;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.DATETIME, Nombre = "Fecha")]
        public DateTime Fecha { get { return _Fecha; } set { _Fecha = value; } }
        decimal _Cambio;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Cambio")]
        public decimal Cambio { get { return _Cambio; } set { _Cambio = value; } }
        bool _Cortesia;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "Cortesia")]
        public bool Cortesia { get { return _Cortesia; } set { _Cortesia = value; } }
        bool _Local;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "Local")]
        public bool Local { get { return _Local; } set { _Local = value; } }
        decimal _Total;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Total")]
        public decimal Total { get { return _Total; } set { _Total = value; } }
        string _Totalstring;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "TotalString")]
        public string Totalstring { get { return _Totalstring; } set { _Totalstring = value; } }
        string _Observacion;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Observacion")]
        public string Observacion { get { return _Observacion; } set { _Observacion = value; } }
        string _Responsable;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Responsable")]
        public string Responsable { get { return _Responsable; } set { _Responsable = value; } }
        bool _Anulado;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "Anulado")]
        public bool Anulado { get { return _Anulado; } set { _Anulado = value; } }
        bool _Remesado;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "Remesado")]
        public bool Remesado { get { return _Remesado; } set { _Remesado = value; } }
        bool _Replicado;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "Replicado")]
        public bool Replicado { get { return _Replicado; } set { _Replicado = value; } }
        bool _Nend;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "NEnd")]
        public bool Nend { get { return _Nend; } set { _Nend = value; } }
        string _Tipocertificado;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "TipoCertificado")]
        public string Tipocertificado { get { return _Tipocertificado; } set { _Tipocertificado = value; } }
        string _Tipocliente;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "TipoCliente")]
        public string Tipocliente { get { return _Tipocliente; } set { _Tipocliente = value; } }
        string _Clienteextra;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "ClienteExtra")]
        public string Clienteextra { get { return _Clienteextra; } set { _Clienteextra = value; } }
        string _Cliente;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Cliente")]
        public string Cliente { get { return _Cliente; } set { _Cliente = value; } }
        string _Vapor;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Vapor")]
        public string Vapor { get { return _Vapor; } set { _Vapor = value; } }
        string _Naduana;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "NAduana")]
        public string Naduana { get { return _Naduana; } set { _Naduana = value; } }
        string _Placa;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Placa")]
        public string Placa { get { return _Placa; } set { _Placa = value; } }
        string _PlacaDos;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "PlacaDos")]
        public string PlacaDos { get { return _PlacaDos; } set { _PlacaDos = value; } }
        string _NumeroBL;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "NumeroBL")]
        public string NumeroBL { get { return _NumeroBL; } set { _NumeroBL = value; } }
        decimal _Impuesto;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Impuesto")]
        public decimal Impuesto { get { return _Impuesto; } set { _Impuesto = value; } }
        int _Idccaja;
        [AtributosBase (VarPorDefecto = "", IntToNull = true, TipoDato = AtributosBase.DataTypes.INT, Nombre = "IDCCaja")]
        public int Idccaja { get { return _Idccaja; } set { _Idccaja = value; } }
        bool _Adeldia;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "ADelDia")]
        public bool Adeldia { get { return _Adeldia; } set { _Adeldia = value; } }
        bool _Enviaplat;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "EnviaPlat")]
        public bool Enviaplat { get { return _Enviaplat; } set { _Enviaplat = value; } }
        DateTime _Fechatrat;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.DATETIME, Nombre = "FechaTrat")]
        public DateTime Fechatrat { get { return _Fechatrat; } set { _Fechatrat = value; } }
        DateTime _FechatratFin;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.DATETIME, Nombre = "FechaTrat_Fin")]
        public DateTime FechatratFin { get { return _FechatratFin; } set { _FechatratFin = value; } }
        bool _Credito;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "Credito")]
        public bool Credito { get { return _Credito; } set { _Credito = value; } }
        string _Norden;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "NOrden")]
        public string Norden { get { return _Norden; } set { _Norden = value; } }
        DateTime _Forden;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.DATETIME, Nombre = "FOrden")]
        public DateTime Forden { get { return _Forden; } set { _Forden = value; } }
        string _Aorden;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "AOrden")]
        public string Aorden { get { return _Aorden; } set { _Aorden = value; } }
        string _Cuarentena;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Cuarentena")]
        public string Cuarentena { get { return _Cuarentena; } set { _Cuarentena = value; } }
        string _Idfactura;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "IDFactura")]
        public string Idfactura { get { return _Idfactura; } set { _Idfactura = value; } }
        string _Nviaje;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "NViaje")]
        public string Nviaje { get { return _Nviaje; } set { _Nviaje = value; } }
        DateTime _Fechaatraque;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.DATETIME, Nombre = "FechaAtraque")]
        public DateTime Fechaatraque { get { return _Fechaatraque; } set { _Fechaatraque = value; } }
        string _idpais;
        [AtributosBase (VarPorDefecto = "", LLavePrimaria = true, TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "idPais")]
        public string idpais { get { return _idpais; } set { _idpais = value; } }
        bool _Ingles;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "Ingles")]
        public bool Ingles { get { return _Ingles; } set { _Ingles = value; } }
        string _AutorizacionBanco;
        public string AutorizacionBanco { get { return _AutorizacionBanco; } set { _AutorizacionBanco = value; } }
        string _FormaPago;
        public string FormaPago { get { return _FormaPago; } set { _FormaPago = value; } }
    }
    public class tblcertifdet : EntidadBase {
        public tblcertifdet () {
            this.Esquema = "dbo";
            this.NombreSQL = "tblcertifdet";
        }
        string _Puesto;
        [AtributosBase (VarPorDefecto = "", LLavePrimaria = true, TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Puesto")]
        public string Puesto { get { return _Puesto; } set { _Puesto = value; } }
        string _Ncertificado;
        [AtributosBase (VarPorDefecto = "", LLavePrimaria = true, TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "NCertificado")]
        public string Ncertificado { get { return _Ncertificado; } set { _Ncertificado = value; } }
        int _Detalle;
        [AtributosBase (VarPorDefecto = "", InsertTable = false, LLavePrimaria = true, TipoDato = AtributosBase.DataTypes.INT, Nombre = "Detalle")]
        public int Detalle { get { return _Detalle; } set { _Detalle = value; } }
        string _Servicio;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Servicio")]
        public string Servicio { get { return _Servicio; } set { _Servicio = value; } }
        float _Cantidad;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Cantidad")]
        public float Cantidad { get { return _Cantidad; } set { _Cantidad = value; } }
        decimal _Us;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "US")]
        public decimal Us { get { return _Us; } set { _Us = value; } }
        decimal _Local;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Local")]
        public decimal Local { get { return _Local; } set { _Local = value; } }
        decimal _Subtotal;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "SubTotal")]
        public decimal Subtotal { get { return _Subtotal; } set { _Subtotal = value; } }
        string _Plaguicida;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Plaguicida")]
        public string Plaguicida { get { return _Plaguicida; } set { _Plaguicida = value; } }
        decimal _Dosis;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Dosis")]
        public decimal Dosis { get { return _Dosis; } set { _Dosis = value; } }
        string _Ud;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "UD")]
        public string Ud { get { return _Ud; } set { _Ud = value; } }
        string _Plaguicida2;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Plaguicida2")]
        public string Plaguicida2 { get { return _Plaguicida2; } set { _Plaguicida2 = value; } }
        decimal _Dosis2;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Dosis2")]
        public decimal Dosis2 { get { return _Dosis2; } set { _Dosis2 = value; } }
        string _Ud2;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "UD2")]
        public string Ud2 { get { return _Ud2; } set { _Ud2 = value; } }
        decimal _Real;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Real")]
        public decimal Real { get { return _Real; } set { _Real = value; } }
        Int32 _Producto;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.SMALLINT, Nombre = "Producto")]
        public Int32 Producto { get { return _Producto; } set { _Producto = value; } }
        string _Ruta;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Ruta")]
        public string Ruta { get { return _Ruta; } set { _Ruta = value; } }
        string _Tipoavion;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "TipoAvion")]
        public string Tipoavion { get { return _Tipoavion; } set { _Tipoavion = value; } }
        string _Procedencia;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Procedencia")]
        public string Procedencia { get { return _Procedencia; } set { _Procedencia = value; } }
        string _Destino;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Destino")]
        public string Destino { get { return _Destino; } set { _Destino = value; } }
        string _Nvuelo;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "NVuelo")]
        public string Nvuelo { get { return _Nvuelo; } set { _Nvuelo = value; } }
        decimal _Tiempoexposicion;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "TiempoExposicion")]
        public decimal Tiempoexposicion { get { return _Tiempoexposicion; } set { _Tiempoexposicion = value; } }
        string _Ut;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "UT")]
        public string Ut { get { return _Ut; } set { _Ut = value; } }
        String _Razon;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Razon")]
        public String Razon { get { return _Razon; } set { _Razon = value; } }
        string _Nacta;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "NActa")]
        public string Nacta { get { return _Nacta; } set { _Nacta = value; } }
        bool _Db;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "DB")]
        public bool Db { get { return _Db; } set { _Db = value; } }
        String _Session;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Session")]
        public String Session { get { return _Session; } set { _Session = value; } }
        decimal _Cantvol;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "CantVol")]
        public decimal Cantvol { get { return _Cantvol; } set { _Cantvol = value; } }
        string _Uc;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "UC")]
        public string Uc { get { return _Uc; } set { _Uc = value; } }
        bool _Enviaplat;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "EnviaPlat")]
        public bool Enviaplat { get { return _Enviaplat; } set { _Enviaplat = value; } }
        decimal _Cantidadcubicada;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "CantidadCubicada")]
        public decimal Cantidadcubicada { get { return _Cantidadcubicada; } set { _Cantidadcubicada = value; } }
        decimal _Teorico;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Teorico")]
        public decimal Teorico { get { return _Teorico; } set { _Teorico = value; } }
        string _Densidad;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Densidad")]
        public string Densidad { get { return _Densidad; } set { _Densidad = value; } }
        string _Contenedor;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Contenedor")]
        public string Contenedor { get { return _Contenedor; } set { _Contenedor = value; } }
        string _Silo;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Silo")]
        public string Silo { get { return _Silo; } set { _Silo = value; } }
        string _Lugtrat;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "LugTrat")]
        public string Lugtrat { get { return _Lugtrat; } set { _Lugtrat = value; } }
        String _Concentracion;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Concentracion")]
        public String Concentracion { get { return _Concentracion; } set { _Concentracion = value; } }
        float _Temperatura;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Temperatura")]
        public float Temperatura { get { return _Temperatura; } set { _Temperatura = value; } }
        decimal _Tiempoaereacion;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "TiempoAereacion")]
        public decimal Tiempoaereacion { get { return _Tiempoaereacion; } set { _Tiempoaereacion = value; } }
        string _UtAereacion;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "UT_Aereacion")]
        public string UtAereacion { get { return _UtAereacion; } set { _UtAereacion = value; } }
        string _Origen;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Origen")]
        public string Origen { get { return _Origen; } set { _Origen = value; } }
        String _idpais;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "idPais")]
        public String idpais { get { return _idpais; } set { _idpais = value; } }
        string _MatriculaAvion;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "MatriculaAvion")]
        public string MatriculaAvion { get { return _MatriculaAvion; } set { _MatriculaAvion = value; } }
    }
    public class tblcertifmdp : EntidadBase {
        public tblcertifmdp () {
            this.Esquema = "dbo";
            this.NombreSQL = "tblcertifmdp";
        }
        string _Puesto;
        [AtributosBase (VarPorDefecto = "", LLavePrimaria = true, TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Puesto")]
        public string Puesto { get { return _Puesto; } set { _Puesto = value; } }
        string _Ncertificado;
        [AtributosBase (VarPorDefecto = "", LLavePrimaria = true, TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "NCertificado")]
        public string Ncertificado { get { return _Ncertificado; } set { _Ncertificado = value; } }
        Int16 _Mdp;
        [AtributosBase (VarPorDefecto = "", LLavePrimaria = true, TipoDato = AtributosBase.DataTypes.SMALLINT, Nombre = "MDP")]
        public Int16 Mdp { get { return _Mdp; } set { _Mdp = value; } }
        Int64 _Detalle;
        [AtributosBase (VarPorDefecto = "", InsertTable = false, TipoDato = AtributosBase.DataTypes.BIGINT, Nombre = "Detalle")]
        public Int64 Detalle { get { return _Detalle; } set { _Detalle = value; } }
        decimal _Cantidad;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Cantidad")]
        public decimal Cantidad { get { return _Cantidad; } set { _Cantidad = value; } }
        float _Abono;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "Abono")]
        public float Abono { get { return _Abono; } set { _Abono = value; } }
        decimal _Saldonuevo;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.FLOAT, Nombre = "SaldoNuevo")]
        public decimal Saldonuevo { get { return _Saldonuevo; } set { _Saldonuevo = value; } }
        int _Idabono;
        [AtributosBase (VarPorDefecto = "", LLavePrimaria = true, TipoDato = AtributosBase.DataTypes.INT, Nombre = "IDABono")]
        public int Idabono { get { return _Idabono; } set { _Idabono = value; } }
        bool _Nend;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "NEnd")]
        public bool Nend { get { return _Nend; } set { _Nend = value; } }
        string _Cheque;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Cheque")]
        public string Cheque { get { return _Cheque; } set { _Cheque = value; } }
        string _Banco;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "Banco")]
        public string Banco { get { return _Banco; } set { _Banco = value; } }
        Int64 _iddeposito;
        //[AtributosBase(VarPorDefecto = "", IntToNull=true, TipoDato = AtributosBase.DataTypes.BIGINT, Nombre = "idDeposito")]
        public Int64 iddeposito { get { return _iddeposito; } set { _iddeposito = value; } }
        bool _Enviaplat;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.BIT, Nombre = "EnviaPlat")]
        public bool Enviaplat { get { return _Enviaplat; } set { _Enviaplat = value; } }
        string _idpais;
        [AtributosBase (VarPorDefecto = "", TipoDato = AtributosBase.DataTypes.VARCHAR, Nombre = "idPais")]
        public string idpais { get { return _idpais; } set { _idpais = value; } }
    }
}