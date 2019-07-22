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

public partial class Cert_Nebulizacion_Terrestre : System.Web.UI.Page
{
    Funciones fn = new Funciones();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["IDUsuario"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        else
        {
            if (true)
            {
				try
                {
                    string variable = Request.QueryString["tp"].ToString();

                    if (variable == "M")
                    {
                        hdTipo.Value = variable;
                    }
                    else {
                        hdTipo.Value = "T";
                    }
                }
                catch (Exception ex) {
                    hdTipo.Value = "T";
                }
				
                String Sql;
                SqlConnection SqlConn = fn.ConnectionSql();

                //Se verifica si el usuario tiene autorización
                bool permiso = false;
                string npagina = "";

                permiso = fn.PaginaPermiso(fn.PaginaActual(), Session["IDEmpleado"].ToString(), Session["idPais"].ToString(), SqlConn);
                npagina = fn.PaginaNombre(fn.PaginaActual(), Session["IDEmpleado"].ToString(), Session["idPais"].ToString(), SqlConn);
                if (permiso == false)
                {
                    Response.Redirect("Autorizacion.aspx?pagina=" + npagina.ToString() + "&select=" + Request["select"]);
                }

                //Se recupera correlativo
                //String ncertificado = Convert.ToString(fn.Recuperar_Correlativo(Session["idPais"].ToString(), Session["idPuesto"].ToString(), "NCertificado", SqlConn));
                String ncertificado = (fn.Recuperar_CorrelativoC(Session["idPais"].ToString(), Session["idPuesto"].ToString(), "NCertificado"));
                if (ncertificado == "0")
                {
                    fn.Message(Page, "Correlativo no definido! Informar al Depto. de Informatica de OIRSA Sede");
                }
                else
                {

                    l_n_certificado.Text = ncertificado.ToString();
                    
                }

                //Se busca el numero de certificado
                Double cuantos;
                Boolean existe = false;
                cuantos = fn.CertificadoExiste(l_n_certificado.Text, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn);

                if (cuantos == 3)
                {
                    fn.Message(Page, "Certificado ya existe! Tiene que definir el Consecutivo");
                    b_aceptar.Visible = false;
                    b_grabar.Visible = false;
                    existe = true;
                }
                else
                {
                    if (cuantos == 2)
                    {
                        fn.Eliminar_Certificado(l_n_certificado.Text, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn);
                    }
                    else
                    {
                        if (cuantos == 1)
                        {
                            Sql = "select Count(NCertificado) from tblCertifDet where NCertificado='" + l_n_certificado.Text + "' and Puesto='" + Session["idPuesto"] + "' and idPais='" + Session["idPais"] + "'";
                            if (fn.EjecutarScalarDouble(Sql, SqlConn) == 0)
                                fn.Eliminar_Certificado(l_n_certificado.Text, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn);
                        }
                    }
                }

                //Se limpian registros en la tblCertifDet
                if (existe == false)
                {
                    bool flag = fn.LimpiarCertifDet(l_n_certificado.Text, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn);
                }

                //Se verifica si tiene mas de una moneda
                string cambio = fn.ValIni(Session["idPais"].ToString(), "Cambio", SqlConn);
                if (cambio == "0")
                {
                    fn.Message(Page, "Variable CAMBIO no encontrada! Informar al Depto. de Informatica de OIRSA Sede");
                }
                else
                {
                    tb_cambio.Text = cambio;
                    if (cambio != "1")
                    {
                        cb_local.Enabled = true;
                    }
                }
                if (!IsPostBack)
                {
                    //Se verifica si existen combos
                    Sql = "Select Count(*) from tblServicio where tipo like '%NEBULIZACIÓN%' and Activo=1 and combo=1 and idPais='" + Session["idPais"] + "'";
                    if (fn.EjecutarScalarDouble(Sql, SqlConn) > 0)
                    {
                        cb_combo.Enabled = true;
                        cb_combo.Visible = true;
                        l_servicio.Text = "Servicio Simple";
                    }
                    else
                    {
                        cb_combo.Enabled = false;
                        cb_combo.Visible = false;
                    }

                    //Se verifica el tipo de pago del puesto
                    String TipoPuesto;
                    TipoPuesto = fn.EjecutarScalarString("select TipoPuesto from tblPuesto where Puesto='" + Session["idPuesto"] + "' and idPais='" + Session["idPais"] + "'", SqlConn);
                    Sql = "select mdp from tblTipoPuesto where Nombre='" + TipoPuesto + "' and idPais='" + Session["idPais"] + "'";
                    if (fn.EjecutarScalarString(Sql, SqlConn).Trim() == "0")
                    {
                        Sql = "Select Cliente, Nombre from tblCliente where Puesto='" + Session["idPuesto"] + "' and SinCredito=1 and idPais='" + Session["idPais"] + "' order by Nombre";
                        l_cliente.Text = "Cliente de Contado";
                        cb_cliente_contado.Checked = true;
                    }
                    else
                    {
                        Sql = "Select Cliente, Nombre from tblCliente where Puesto='" + Session["idPuesto"] + "' and SinCredito=0 and idPais='" + Session["idPais"] + "' order by Nombre";
                        l_cliente.Text = "Cliente de Crédito";
                        cb_cliente_contado.Checked = false;
                    }

                    //Se llenan combos
                    SqlCommand SqlCommCli = new SqlCommand(Sql, SqlConn);
                    SqlDataReader SqlDatCli = SqlCommCli.ExecuteReader();
                    ddl_cliente.DataValueField = "Cliente";
                    ddl_cliente.DataTextField = "Nombre";
                    ddl_cliente.DataSource = SqlDatCli;
                    ddl_cliente.DataBind();
                    SqlDatCli.Close();

                    cfe_grabar.ConfirmText = "Certificado :" + l_n_certificado.Text + " \n Cliente: " + ddl_cliente.SelectedItem + " \n Monto a cobrar: " + tb_total.Text + " \n\n Desea Continuar?";
                    Informacion_Cliente(ddl_cliente.SelectedValue);

                    Sql = "Select (Servicio + '-' + Plaguicida) as Servicio, Nombre from tblServicio where Activo=1 and combo=0 and Tipo='NEBULIZACIÓN' and idPais='" + Session["idPais"] + "' order by Nombre";
                    SqlCommand SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDataReader SqlDat = SqlComm.ExecuteReader();
                    ddl_servicio.DataValueField = "Servicio";
                    ddl_servicio.DataTextField = "Nombre";
                    ddl_servicio.DataSource = SqlDat;
                    ddl_servicio.DataBind();
                    SqlDat.Close();

                    Sql = "SELECT Plaguicida, Nombre from tblPlaguicida where Plaguicida like '01%' and idPais='" + Session["idPais"] + "' order by nombre";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_plaguicida.DataValueField = "Plaguicida";
                    ddl_plaguicida.DataTextField = "Nombre";
                    ddl_plaguicida.DataSource = SqlDat;
                    ddl_plaguicida.DataBind();
                    SqlDat.Close();

                    String[] plaguicida = ddl_servicio.SelectedValue.Split('-');

                    ddl_plaguicida.Items.FindByValue(plaguicida[1]).Selected = true;

                    Sql = "select Descripcion,(Dosis + '-' + cast(valor as char(20))) as Dosis from tblDosis where Activo=1 and Plaguicida='" + ddl_plaguicida.SelectedValue + "' and idPais='" + Session["idPais"] + "' order by Descripcion";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_unidad_dosis.DataValueField = "Dosis";
                    ddl_unidad_dosis.DataTextField = "Descripcion";
                    ddl_unidad_dosis.DataSource = SqlDat;
                    ddl_unidad_dosis.DataBind();
                    SqlDat.Close();

                    if (ddl_unidad_dosis.SelectedValue.Length > 0)
                    {
                        String[] dosis = ddl_unidad_dosis.SelectedValue.Split('-');
                        tb_dosis.Text = dosis[1];
                    }
                    else
                    {
                        tb_dosis.Text = "";
                    }

                    Sql = "select Producto,Nombre from tblProducto where idPais='" + Session["idPais"] + "' order by Nombre";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_producto.DataValueField = "Producto";
                    ddl_producto.DataTextField = "Nombre";
                    ddl_producto.DataSource = SqlDat;
                    ddl_producto.DataBind();
                    SqlDat.Close();

                    Sql = "select Codigo,Unidad from tblValUnidad where tipo='UT' and idPais='" + Session["idPais"] + "' order by Unidad";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_unidad_tiempo.DataValueField = "Codigo";
                    ddl_unidad_tiempo.DataTextField = "Unidad";
                    ddl_unidad_tiempo.DataSource = SqlDat;
                    ddl_unidad_tiempo.DataBind();
                    SqlDat.Close();

                    Sql = "select Codigo,Unidad from tblValUnidad where tipo='UC' and idPais='" + Session["idPais"] + "' order by Unidad";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_peso_producto.DataValueField = "Unidad";
                    ddl_peso_producto.DataTextField = "Unidad";
                    ddl_peso_producto.DataSource = SqlDat;
                    ddl_peso_producto.DataBind();
                    SqlDat.Close();

                    Sql = "select id,Nombre from tblPais where idPais='" + Session["idPais"] + "' order by Nombre";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_origen.DataValueField = "Nombre";
                    ddl_origen.DataTextField = "Nombre";
                    ddl_origen.DataSource = SqlDat;
                    ddl_origen.DataBind();
                    SqlDat.Close();

                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_procedencia.DataValueField = "Nombre";
                    ddl_procedencia.DataTextField = "Nombre";
                    ddl_procedencia.DataSource = SqlDat;
                    ddl_procedencia.DataBind();
                    SqlDat.Close();

                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_destino.DataValueField = "Nombre";
                    ddl_destino.DataTextField = "Nombre";
                    ddl_destino.DataSource = SqlDat;
                    ddl_destino.DataBind();
                    SqlDat.Close();

                    ddl_destino.Items.FindByValue(fn.ValIni(Session["idPais"].ToString(), "PaisName", SqlConn)).Selected = true;

                    Sql = "select Codigo,Unidad from tblValUnidad where tipo='RUTA' and idPais='" + Session["idPais"] + "' order by Unidad";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_ruta.DataValueField = "Codigo";
                    ddl_ruta.DataTextField = "Unidad";
                    ddl_ruta.DataSource = SqlDat;
                    ddl_ruta.DataBind();
                    SqlDat.Close();

                    Sql = "select * from tblEncargadoCuarentena where Puesto='" + Session["idPuesto"] + "' and idPais='" + Session["idPais"] + "' order by Nombre";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_cuarentena.DataValueField = "Codigo";
                    ddl_cuarentena.DataTextField = "Nombre";
                    ddl_cuarentena.DataSource = SqlDat;
                    ddl_cuarentena.DataBind();
                    SqlDat.Close();


                    calculo();

                    //Se verifica si se aplicara la fecha del servidor
                    Sql = "select Valor from tblIni where ID='FechaServidor' and IdPais='" + Session["idPais"].ToString() + "'";
                    string fecha_servidor = fn.EjecutarScalarString(Sql, SqlConn);

                    if (fecha_servidor == "0")
                    {
                        tb_fecha.ReadOnly = false;
                        p_fecha.Visible = true;
                    }
                    else
                    {
                        tb_fecha.ReadOnly = true;
                        p_fecha.Visible = false;
                    }

                    //Solo cuando se implementa un nuevo puesto
                    /*if (Session["idPuesto"].ToString().Trim() == "J")
                    {
                        tb_fecha.ReadOnly = false;
                        p_fecha.Visible = true;
                    }*/

                    tb_fecha.Text = Convert.ToString(DateTime.Now.Month) + "/" + Convert.ToString(DateTime.Now.Day) + "/" + Convert.ToString(DateTime.Now.Year);
                    tb_fecha_tratamiento.Text = Convert.ToString(DateTime.Now.Month) + "/" + Convert.ToString(DateTime.Now.Day) + "/" + Convert.ToString(DateTime.Now.Year);

                    //Mensaje de aviso que el certificado fue grabado
                    if (Request["save"] != null)
                    {
                        fn.Message(Page, "Certificado agregado con éxito!");

                        //string nfile = "ConstanciaTratamiento_" + Session["idPais"].ToString().Trim() + "_" + Session["idPuesto"].ToString().Trim();
                        string nfile = Funciones.ReporteCertifName();
                        Response.Write("<script>window.open('" + Funciones.RsReports() + "/SitcRS/Reporte.aspx?nfile=" + nfile + "&formato=PDF&ncertificado=" + Request["ncertif"] + "&idPuesto=" + Session["idPuesto"] + "&idPais=" + Session["idPais"] + "&NPuesto=" + Session["NombrePuesto"] + "&NPais=" + Session["NombrePais"] + "','_new');</script>");
                    }

                    //Mensaje de aviso si el certificado tiene un error
                    if (Request["error"] != null)
                    {
                        fn.Eliminar_Certificado(Request["ncertif"].ToString(), Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn);
                        fn.Message(Page, "Ocurrio un error al intentar grabar el Certificado. Ingreselo nuevamente!");
                    }
                    String hora;
                    String ampm;
                    if (DateTime.Now.Hour <= 12)
                    {
                        ampm = "AM";
                        if (DateTime.Now.Hour == 12)
                        {
                            if (DateTime.Now.Minute > 0)
                            {
                                ampm = "PM";
                            }
                        }
                        hora = Convert.ToString(DateTime.Now.Hour);
                    }
                    else
                    {
                        hora = Convert.ToString(DateTime.Now.Hour - 12);
                        ampm = "PM";
                    }

                    tb_hora_ini.Text = hora + ":" + Convert.ToString(DateTime.Now.Minute) + ":" + Convert.ToString(DateTime.Now.Second) + " " + ampm;
                }
            }
        }
    }

    //Proceso que cambia el tipo de cliente en el combo (CREDITO ó CONTADO)
    protected void cb_cliente_contado_CheckedChanged(object sender, EventArgs e)
    {
        String Sql;
        SqlConnection SqlConn = fn.ConnectionSql();

        if (cb_cliente_contado.Checked == true)
        {
            Sql = "Select Cliente, Nombre from tblCliente where Puesto='" + Session["idPuesto"] + "' and SinCredito=1 and idPais='" + Session["idPais"] + "' order by Nombre";
            l_cliente.Text = "Cliente de Contado";
        }
        else
        {
            Sql = "Select Cliente, Nombre from tblCliente where Puesto='" + Session["idPuesto"] + "' and SinCredito=0 and idPais='" + Session["idPais"] + "' order by Nombre";
            l_cliente.Text = "Cliente de Crédito";
        }

        SqlCommand SqlCommCli = new SqlCommand(Sql, SqlConn);
        SqlDataReader SqlDatCli = SqlCommCli.ExecuteReader();

        ddl_cliente.DataValueField = "Cliente";
        ddl_cliente.DataTextField = "Nombre";
        ddl_cliente.DataSource = SqlDatCli;
        ddl_cliente.DataBind();

        cfe_grabar.ConfirmText = "Certificado :" + l_n_certificado.Text + " \n Cliente: " + ddl_cliente.SelectedItem + " \n Monto a cobrar: " + tb_total.Text + " \n\n Desea Continuar?";
        Informacion_Cliente(ddl_cliente.SelectedValue);

        SqlDatCli.Close();
        SqlConn.Close();
    }

    protected void ddl_servicio_SelectedIndexChanged(object sender, EventArgs e)
    {
        String Sql;
        SqlConnection SqlConn = fn.ConnectionSql();

        Sql = "SELECT Plaguicida, Nombre from tblPlaguicida where Plaguicida like '01%' and idPais='" + Session["idPais"] + "' order by nombre";
        SqlCommand SqlComm = new SqlCommand(Sql, SqlConn);
        SqlDataReader SqlDat = SqlComm.ExecuteReader();
        ddl_plaguicida.DataValueField = "Plaguicida";
        ddl_plaguicida.DataTextField = "Nombre";
        ddl_plaguicida.DataSource = SqlDat;
        ddl_plaguicida.DataBind();
        SqlDat.Close();

        String[] plaguicida = ddl_servicio.SelectedValue.Split('-');

        ddl_plaguicida.Items.FindByValue(plaguicida[1]).Selected = true;

        Sql = "select Descripcion,(Dosis + '-' + cast(valor as char(20))) as Dosis from tblDosis where Activo=1 and Plaguicida='" + ddl_plaguicida.SelectedValue + "' and idPais='" + Session["idPais"] + "' order by Descripcion";
        SqlComm = new SqlCommand(Sql, SqlConn);
        SqlDat = SqlComm.ExecuteReader();
        ddl_unidad_dosis.DataValueField = "Dosis";
        ddl_unidad_dosis.DataTextField = "Descripcion";
        ddl_unidad_dosis.DataSource = SqlDat;
        ddl_unidad_dosis.DataBind();
        SqlDat.Close();
        SqlConn.Close();

        if (ddl_unidad_dosis.SelectedValue.Length > 0)
        {
            String[] dosis = ddl_unidad_dosis.SelectedValue.Split('-');
            tb_dosis.Text = dosis[1];
        }
        else
        {
            tb_dosis.Text = "";
        }

        calculo();
    }

    protected void ddl_plaguicida_SelectedIndexChanged(object sender, EventArgs e)
    {
        String Sql;
        SqlConnection SqlConn = fn.ConnectionSql();

        Sql = "select Descripcion,(Dosis + '-' + cast(valor as char(20))) as Dosis from tblDosis where Activo=1 and Plaguicida='" + ddl_plaguicida.SelectedValue + "' and idPais='" + Session["idPais"] + "' order by Descripcion";
        SqlCommand SqlComm = new SqlCommand(Sql, SqlConn);
        SqlDataReader SqlDat = SqlComm.ExecuteReader();
        ddl_unidad_dosis.DataValueField = "Dosis";
        ddl_unidad_dosis.DataTextField = "Descripcion";
        ddl_unidad_dosis.DataSource = SqlDat;
        ddl_unidad_dosis.DataBind();
        SqlDat.Close();
        SqlConn.Close();

        if (ddl_unidad_dosis.SelectedValue.Length > 0)
        {
            String[] dosis = ddl_unidad_dosis.SelectedValue.Split('-');
            tb_dosis.Text = dosis[1];
        }
        else
        {
            tb_dosis.Text = "";
        }

    }

    //Calculo de subtotal del servicio seleccionado
    void calculo()
    {
        String Sql;
        String[] value = ddl_servicio.SelectedValue.Split('-');
        SqlConnection SqlConn = fn.ConnectionSql();
        String Local;

        if (cb_local.Checked == true)
        {
            Local = "1";
        }
        else
        {
            Local = "0";
        }

        Sql = "select * from tblServicio where Servicio=" + value[0] + " and idPais='" + Session["idPais"].ToString() + "'";
        SqlCommand SqlComm = new SqlCommand(Sql, SqlConn);
        SqlDataReader SqlDat = SqlComm.ExecuteReader();
        if (SqlDat.Read())
        {
            if (Local == "1")
            {
                tb_costo_us.Text = SqlDat.GetValue(3).ToString();
                tb_costo_local.Text = Convert.ToString(Convert.ToSingle(SqlDat.GetValue(3)) * Convert.ToSingle(tb_cambio.Text));
                tb_subtotal.Text = Convert.ToString(Convert.ToSingle(tb_costo_local.Text) * Convert.ToSingle(tb_cantidad.Text));
            }
            else
            {
                tb_costo_us.Text = SqlDat.GetValue(3).ToString();
                tb_costo_local.Text = Convert.ToString(Convert.ToSingle(SqlDat.GetValue(3)) * Convert.ToSingle(tb_cambio.Text));
                tb_subtotal.Text = Convert.ToString(Convert.ToSingle(tb_costo_us.Text) * Convert.ToSingle(tb_cantidad.Text));
            }
        }

        SqlDat.Close();
        SqlConn.Close();
    }

    //Proceso que llena el GridView
    void BindData(SqlConnection cnn)
    {
        String Sql;
        Double total;
        String Local;
        String moneda;

        Local = "0";
        if (cb_local.Checked == true)
        {
            Local = "1";
            moneda = fn.ValIni(Session["idPais"].ToString(), "MonedaLocalName", cnn);
        }
        else
        {
            Local = "0";
            moneda = "Dolares";
        }

        Sql = "select a.Detalle,b.Nombre as Servicio,a.Cantidad,a.Local,a.Us,a.SubTotal,c.Nombre as Plaguicida from tblCertifDet a,tblServicio b,tblPlaguicida c where a.Servicio=b.Servicio and a.Plaguicida=c.Plaguicida and NCertificado='" + l_n_certificado.Text + "' and Puesto='" + Session["idPuesto"] + "' and a.idPais='" + Session["idPais"] + "' and b.idPais='" + Session["idPais"] + "' and c.idPais='" + Session["idPais"] + "'";

        SqlDataAdapter SdaDet = new SqlDataAdapter(Sql, cnn);
        DataSet DsDet = new DataSet();
        SdaDet.Fill(DsDet, "DetCertificados");
        gv_servicios.DataSource = DsDet;
        gv_servicios.DataBind();

        total = fn.TotalCertificado(l_n_certificado.Text, Session["idPuesto"].ToString(), Session["idPais"].ToString(), cnn);
        //l_numlet.Text = fn.NumeroALetras(Convert.ToString(total)) + " " + moneda;
        tb_subtotalg.Text = Convert.ToString(Math.Round(total, 2));
        tb_total.Text = Convert.ToString(Math.Round(total + Convert.ToSingle(tb_recargo.Text), 2));
        l_numlet.Text = fn.NumeroALetras(tb_total.Text) + " " + moneda;
    }

    protected void b_aceptar_Click(object sender, EventArgs e)
    {
        String[] servicio = ddl_servicio.SelectedValue.Split('-');
        String[] dosis = ddl_unidad_dosis.SelectedValue.Split('-');
        String Sql = "";
        SqlConnection SqlConn = fn.ConnectionSql();

        if (fn.Certificado_Existe(l_n_certificado.Text, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn) == 0)
        {
            Sql = "delete from tblCertifDet where NCertificado='" + l_n_certificado.Text + "' and Puesto='" + Session["idPuesto"] + "' and idPais='" + Session["idPais"] + "' and Session<>'" + Session["session"] + "'";
            fn.EjecutarNonQuery(Sql, SqlConn);
            if(cb_local.Checked==true){
                string subt = Convert.ToString(Math.Round(Convert.ToDouble(tb_subtotal.Text), 2));
                string[] asubt = subt.Split('.');
                if (asubt.Length > 1) {
                    if (int.Parse(asubt[1]) > 4)
                    {
                       // tb_subtotal.Text = Convert.ToString(int.Parse(asubt[0]) + 1);
                    }
                    else {
                        //tb_subtotal.Text = asubt[0];
                    }
                }
            }
            string cantidadCubi = tb_cantidad_cubicada.Text == "" ? "0" : tb_cantidad_cubicada.Text;
            Sql = "insert into tblCertifDet (Puesto,NCertificado,Servicio,Cantidad,US,Local,SubTotal,Plaguicida,Dosis,UD,Real,Producto,Ruta,Procedencia,Destino,TiempoExposicion,UT,DB,[Session],CantVol,UC,Origen,cantidadcubicada,idPais) values ('" + Session["idPuesto"] + "','";
            Sql = Sql + l_n_certificado.Text + "','" + servicio[0] + "'," + tb_cantidad.Text + "," + tb_costo_us.Text + "," + Convert.ToString(Math.Round(Convert.ToDouble(tb_costo_local.Text), 2)) + "," + Convert.ToString(Math.Round(Convert.ToDouble(tb_subtotal.Text), 2)) + ",'";
            Sql = Sql + ddl_plaguicida.SelectedValue + "'," + tb_dosis.Text + ",'" + dosis[0] + "'," + tb_consumo_real.Text + ",'" + ddl_producto.SelectedValue + "','" + ddl_ruta.SelectedItem + "','";
            Sql = Sql + ddl_procedencia.SelectedItem + "','" + ddl_destino.SelectedItem + "'," + tb_tiempo_exposicion.Text + ",'" + ddl_unidad_tiempo.SelectedItem + "',0,'" + Session["session"] + "'," + tb_peso_producto.Text + ",'" + ddl_peso_producto.SelectedItem + "','" + ddl_origen.SelectedItem + "'," + cantidadCubi + ",'" + Session["idPais"] + "')";

            fn.EjecutarNonQuery(Sql, SqlConn);

            l_servicios.Text = Convert.ToString(Convert.ToDouble(l_servicios.Text) + 1);

            BindData(SqlConn);

            cfe_grabar.ConfirmText = "Certificado :" + l_n_certificado.Text + " \n Cliente: " + ddl_cliente.SelectedItem + " \n Monto a cobrar: " + tb_total.Text + " \n\n Desea Continuar?";
        }
        else
        {
            fn.Message(Page, "Certificado ya existe! Tiene que definir el Consecutivo");
            b_aceptar.Visible = false;
            b_grabar.Visible = false;
        }

        SqlConn.Close();
    }

    protected void tb_cantidad_TextChanged(object sender, EventArgs e)
    {
        if (tb_cantidad.Text.Length > 0)
        {
            calculo();
        }
        else
        {
            tb_cantidad.Text = "1";
        }
    }

    //Proceso que elimina los servicios agregados
    protected void gv_servicios_SelectedIndexChanged(object sender, EventArgs e)
    {
        String Sql;
        SqlConnection SqlConn = fn.ConnectionSql();

        Sql = "delete tblCertifDet where Detalle=" + gv_servicios.SelectedDataKey.Value.ToString();
        SqlCommand SqlComm = new SqlCommand(Sql, SqlConn);
        SqlComm.ExecuteNonQuery();

        l_servicios.Text = Convert.ToString(Convert.ToDouble(l_servicios.Text) - 1);

        fn.Message(Page, "Servicio eliminado con éxito!");
        BindData(SqlConn);
        SqlConn.Close();
    }

    protected void b_grabar_Click(object sender, EventArgs e)
    {
        String Sql, SqlCertif, SqlCertifMdp;
        SqlConnection SqlConn = fn.ConnectionSql();
        String MDP, Cortesia, Local;

        //Se verifica que exista detalle para la session actual
        Sql = "select count(ncertificado) from tblCertifDet where Puesto='" + Session["idPuesto"] + "' and idPais='" + Session["idPais"] + "' and ncertificado='" + l_n_certificado.Text + "' and session='" + Session["session"] + "'";
        Double cert_det = fn.EjecutarScalarDouble(Sql, SqlConn);

        Double total = fn.TotalCertificado(l_n_certificado.Text, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn);

        if (Convert.ToDouble(tb_subtotalg.Text) == Math.Round(total, 2) && cert_det == Convert.ToDouble(l_servicios.Text) && cert_det > 0 && ddl_cliente.SelectedValue.Trim() != "" && ddl_cuarentena.SelectedValue.Trim() != "" && tb_fecha_tratamiento.Text.Trim().Length != 0)
        {
            //Se graba en la tblCertifMDP
            if (cb_cliente_contado.Checked == true)
            {
                MDP = "0";
            }
            else
            {
                MDP = "1";
            }

            if (cb_cortesia.Checked == true)
            {
                Cortesia = "1";
            }
            else
            {
                Cortesia = "0";
            }

            if (cb_local.Checked == true)
            {
                Local = "1";
            }
            else
            {
                Local = "0";
            }

            if (Convert.ToDouble(tb_recargo.Text) > 0)
            {
                Sql = "insert into tblCertifDet (Puesto,Ncertificado,Servicio,Cantidad,US,local,subtotal,DB,idPais) values ('" + Session["idPuesto"] + "','" + l_n_certificado.Text + "','";
                Sql = Sql + fn.ValIni(Session["idPais"].ToString(), "CuentaRecargo", SqlConn) + "',1," + tb_recargo.Text + "," + tb_recargo.Text + "," + tb_recargo.Text + ",0,'" + Session["idPais"] + "')";
                fn.EjecutarNonQuery(Sql, SqlConn);
            }

            SqlCertifMdp = "insert into tblCertifMDP (Puesto, NCertificado, MDP, Cantidad, Abono, SaldoNuevo, IDAbono, NEnd, idPais) values ('" + Session["idPuesto"] + "','" + l_n_certificado.Text + "',";
            SqlCertifMdp = SqlCertifMdp + MDP.ToString() + "," + tb_total.Text + ",0," + tb_total.Text + ",0,0,'" + Session["idPais"] + "')";

            //Se graba en la tblCertif
            String nplaca;
            if (tb_nplaca.Text.Length == 0)
            {
                nplaca = "";
            }
            else
            {
                nplaca = tb_nplaca.Text;
            }

            String observacion;
            if (tb_observaciones.Text.Length == 0)
            {
                observacion = "";
            }
            else
            {
                observacion = tb_observaciones.Text;
            }

            String tipo_cliente;
            if (rb_tipo_cliente_i.Checked)
                tipo_cliente = "I";
            else
                tipo_cliente = "E";

            String fecha_trat = tb_fecha_tratamiento.Text + " " + DateTime.Now.ToLongTimeString();
            String fecha = tb_fecha.Text + " " + DateTime.Now.ToLongTimeString();

            SqlCertif = "insert into tblCertif (Puesto,Ncertificado,Fecha,Cambio,Cortesia,Local,Total,TotalString,Observacion,Responsable,Anulado,Remesado,Replicado,NEnd,TipoCertificado,TipoCliente,ClienteExtra,Cliente,Placa,FechaTrat,Cuarentena,idPais) values ('";
            SqlCertif = SqlCertif + Session["idPuesto"] + "','" + l_n_certificado.Text + "','" + fecha.ToString() + "'," + tb_cambio.Text + "," + Cortesia.ToString() + "," + Local + "," + tb_total.Text + ",'" + l_numlet.Text + "','" + observacion.ToString() + "','";
            SqlCertif = SqlCertif + Session["login"] + "',0,0,0,1,'NEBULIZACION TERRESTRE','" + tipo_cliente + "','" + tb_cliente.Text + "','" + ddl_cliente.SelectedValue + "','" + nplaca.ToString() + "','" + fecha_trat.ToString() + "','" + ddl_cuarentena.SelectedValue + "','" + Session["idPais"] + "')";

            bool error = false;

            try
            {
                fn.EjecutarNonQuery(SqlCertif, SqlConn);
                fn.EjecutarNonQuery(SqlCertifMdp, SqlConn);
            }
            catch
            {
                error = true;
            }
            finally
            {
                if (error)
                {
                    Response.Redirect("Cert_Nebulizacion_Terrestre.aspx?error=ok&ncertif=" + l_n_certificado.Text);
                }
                else
                {
                    if (fn.Verificar_Certificado(l_n_certificado.Text, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn))
                    {
                        fn.Incrementar_Correlativo(Session["idPais"].ToString(), Session["idPuesto"].ToString(), "NCertificado");

                        //Se registra en auditoria
                        fn.Auditoria(Session["login"].ToString(), "Agregar Certificado", l_n_certificado.Text, Session["idPais"].ToString(), SqlConn);

                        SqlConn.Close();

                        Response.Redirect("Cert_Nebulizacion_Terrestre.aspx?save=ok&ncertif=" + l_n_certificado.Text);
                    }
                    else
                    {
                        Response.Redirect("Cert_Nebulizacion_Terrestre.aspx?error=ok&ncertif=" + l_n_certificado.Text);
                    }
                }
            }
        }
        else
        {
            if (cert_det == 0 || cert_det != Convert.ToDouble(l_servicios.Text) || Convert.ToDouble(tb_subtotalg.Text) != Math.Round(total, 2))
                Response.Redirect("Cert_Nebulizacion_Terrestre.aspx?error=ok");

            fn.Message(Page, "Verificar! Hay campos que son obligatorios como el CLIENTE, FECHA TRATAMIENTO y ENCARGADO DE CUARENTENA");
        }
    }

    protected void ddl_cliente_SelectedIndexChanged(object sender, EventArgs e)
    {
        cfe_grabar.ConfirmText = "Certificado :" + l_n_certificado.Text + " \n Cliente: " + ddl_cliente.SelectedItem + " \n Monto a cobrar: " + tb_total.Text + " \n\n Desea Continuar?";
        Informacion_Cliente(ddl_cliente.SelectedValue);
    }

    //Buscar información relacionada al cliente
    void Informacion_Cliente(string Cliente)
    {
        String Sql;
        SqlConnection SqlConn = fn.ConnectionSql();
        String Local;
        String moneda;

        if (cb_local.Checked == true)
        {
            Local = "1";
            moneda = fn.ValIni(Session["idPais"].ToString(), "MonedaLocalName", SqlConn);
        }
        else
        {
            Local = "0";
            moneda = "Dolares";
        }

        Sql = "SELECT c.Nombre AS Cliente, a.Fecha AS Fecha, a.NCertificado, a.Total, 'Pendiente ' AS Status, SUM(b.Abono) AS Abono, (a.Total-Sum(b.Abono)) AS Saldo, a.Cliente AS CodCliente, a.Local ";
        Sql = Sql + "FROM tblCertif AS a, tblCertifMDP AS b, tblCliente AS c WHERE a.NCertificado=b.NCertificado AND a.Cliente=c.Cliente AND a.Local=" + Local + " AND a.Cliente='" + Cliente.ToString() + "' AND a.Anulado=0 AND a.Cortesia=0 AND b.mdp NOT IN (0,2,5,8) ";
        Sql = Sql + "AND a.idPais='" + Session["idPais"] + "' AND b.idPais='" + Session["idPais"] + "' AND c.idPais='" + Session["idPais"] + "' AND a.Puesto='" + Session["idPuesto"] + "' AND b.Puesto='" + Session["idPuesto"] + "' AND c.Puesto='" + Session["idPuesto"] + "' GROUP BY c.Nombre, a.Fecha, a.NCertificado, a.Total, a.Cliente, a.Local HAVING (a.total - Sum(b.Abono)) > 0.1";

        SqlCommand SqlComm = new SqlCommand(Sql, SqlConn);
        SqlDataReader SqlDat = SqlComm.ExecuteReader();

        Double cantidad = 0;
        Double total = 0;
        while (SqlDat.Read())
        {
            cantidad = cantidad + 1;
            total = total + Convert.ToDouble(SqlDat.GetValue(6));
        }
        l_cantidad.Text = Convert.ToString(cantidad);
        l_total.Text = Convert.ToString(Math.Round(total, 2));
        l_moneda.Text = moneda;

        if (cantidad == 0)
        {
            btnInfo.Text = "";
        }
        else
        {
            btnInfo.Text = "Tiene certificados pendientes cobro";
        }

        SqlDat.Close();
        SqlConn.Close();
    }
    protected void ddl_unidad_dosis_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_unidad_dosis.SelectedValue.Length > 0)
        {
            String[] dosis = ddl_unidad_dosis.SelectedValue.Split('-');
            tb_dosis.Text = dosis[1];
        }
        else
        {
            tb_dosis.Text = "";
        }
    }
    protected void cb_local_CheckedChanged(object sender, EventArgs e)
    {
        SqlConnection SqlConn = fn.ConnectionSql();
        String Local;

        if (cb_local.Checked == true)
        {
            Local = "1";
        }
        else
        {
            Local = "0";
        }

        calculo();
        Informacion_Cliente(ddl_cliente.SelectedValue);
        fn.ActualizarDetalleMoneda(l_n_certificado.Text, Local, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn);
        BindData(SqlConn);

        SqlConn.Close();
    }

    protected void cb_combo_CheckedChanged(object sender, EventArgs e)
    {
        String Sql;
        SqlConnection SqlConn = fn.ConnectionSql();

        if (cb_combo.Checked == true)
        {
            Sql = "Select (Servicio + '-' + Plaguicida) as Servicio, Nombre from tblServicio where Activo=1 and Combo=1 and tipo like '%ASPERSIÓN%' and idPais='" + Session["idPais"] + "' order by Nombre";
            l_servicio.Text = "Servicio en Combo";
        }
        else
        {
            Sql = "Select (Servicio + '-' + Plaguicida) as Servicio, Nombre from tblServicio where Activo=1 and Combo=0 and tipo = 'ASPERSIÓN' and idPais='" + Session["idPais"] + "' order by Nombre";
            l_servicio.Text = "Servicio Simple";
        }


        SqlCommand SqlComm = new SqlCommand(Sql, SqlConn);
        SqlDataReader SqlDat = SqlComm.ExecuteReader();
        ddl_servicio.DataValueField = "Servicio";
        ddl_servicio.DataTextField = "Nombre";
        ddl_servicio.DataSource = SqlDat;
        ddl_servicio.DataBind();
        SqlDat.Close();

        Sql = "SELECT Plaguicida, Nombre from tblPlaguicida where Plaguicida like '01%' and idPais='" + Session["idPais"] + "' order by nombre";
        SqlComm = new SqlCommand(Sql, SqlConn);
        SqlDat = SqlComm.ExecuteReader();
        ddl_plaguicida.DataValueField = "Plaguicida";
        ddl_plaguicida.DataTextField = "Nombre";
        ddl_plaguicida.DataSource = SqlDat;
        ddl_plaguicida.DataBind();
        SqlDat.Close();

        String[] plaguicida = ddl_servicio.SelectedValue.Split('-');

        ddl_plaguicida.Items.FindByValue(plaguicida[1]).Selected = true;

        Sql = "select Descripcion,(Dosis + '-' + cast(valor as char(20))) as Dosis from tblDosis where Plaguicida='" + ddl_plaguicida.SelectedValue + "' and idPais='" + Session["idPais"] + "' order by Descripcion";
        SqlComm = new SqlCommand(Sql, SqlConn);
        SqlDat = SqlComm.ExecuteReader();
        ddl_unidad_dosis.DataValueField = "Dosis";
        ddl_unidad_dosis.DataTextField = "Descripcion";
        ddl_unidad_dosis.DataSource = SqlDat;
        ddl_unidad_dosis.DataBind();
        SqlDat.Close();

        if (ddl_unidad_dosis.SelectedValue.Length > 0)
        {
            String[] dosis = ddl_unidad_dosis.SelectedValue.Split('-');
            tb_dosis.Text = dosis[1];
        }
        else
        {
            tb_dosis.Text = "";
        }

        calculo();

        SqlConn.Close();
    }

    protected void cb_recargo_CheckedChanged(object sender, EventArgs e)
    {
        String moneda;
        String Local;
        SqlConnection SqlConn = fn.ConnectionSql();

        if (cb_local.Checked == true)
        {
            Local = "1";
            moneda = fn.ValIni(Session["idPais"].ToString(), "MonedaLocalName", SqlConn);
        }
        else
        {
            Local = "0";
            moneda = "Dolares";
        }

        if (cb_recargo.Checked)
            tb_recargo.Text = Convert.ToString(Math.Round(Convert.ToSingle(tb_subtotalg.Text) * 0.25, 2));
        else
            tb_recargo.Text = "0";

        tb_total.Text = Convert.ToString(Math.Round(Convert.ToSingle(tb_subtotalg.Text) + Convert.ToSingle(tb_recargo.Text), 2));
        l_numlet.Text = fn.NumeroALetras(tb_total.Text) + " " + moneda;

        cfe_grabar.ConfirmText = "Certificado :" + l_n_certificado.Text + " \n Cliente: " + ddl_cliente.SelectedItem + " \n Monto a cobrar: " + tb_total.Text + " \n\n Desea Continuar?";

        SqlConn.Close();
    }
}