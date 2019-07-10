using System;
using System.IO;
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
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Collections.Generic;

[System.Web.Script.Services.ScriptService]
public partial class Cert_Fumigacion_TerrestreJ : System.Web.UI.Page
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
                    RedirectBandeja();
                    string variable = Request.QueryString["tp"].ToString();                    

                    if (variable == "M")
                    {
                        hdTipo.Value = variable;
                    }
                    else
                    {
                        hdTipo.Value = "T";
                    }
                }
                catch (Exception ex)
                {
                    hdTipo.Value = "T";
                }

                String Sql;
                SqlConnection SqlConn = fn.ConnectionSql();

                //Se verifica si el usuario tiene autorización
                bool permiso = false;
                string npagina = "";

                permiso = fn.PaginaPermiso(fn.PaginaActual(), Session["IDEmpleado"].ToString(), Session["idPais"].ToString(), SqlConn);
                npagina = fn.PaginaNombre("Cert_Fumigacion_Terrestre.aspx", Session["IDEmpleado"].ToString(), Session["idPais"].ToString(), SqlConn);
                if (permiso == false)
                {
                    //Response.Redirect("Autorizacion.aspx?pagina=" + npagina.ToString() + "&select=" + Request["select"]);
                }

                //Mensaje de aviso que el certificado fue grabado
                if (Request["save"] != null)
                {
                    fn.Message(Page, "Certificado agregado con éxito!");

                    string nfile = "ConstanciaTratamiento_" + Session["idPais"].ToString().Trim() + "_" + Session["idPuesto"].ToString().Trim();
                    Response.Write("<script>window.open('" + Funciones.RsReports() + "/SitcRS/Reporte.aspx?nfile=" + nfile + "&formato=PDF&ncertificado=" + Request["ncertif"] + "&idPuesto=" + Session["idPuesto"] + "&idPais=" + Session["idPais"] + "&NPuesto=" + Session["NombrePuesto"] + "&NPais=" + Session["NombrePais"] + "','_new');</script>");
                }
                else
                {
                    //Limpiar temporales de consumo de quimico
                    LimpiarTemporales(SqlConn);
                }

                //Mensaje de aviso si el certificado tiene un error
                if (Request["error"] != null)
                {
                    fn.Eliminar_Certificado(Request["ncertif"].ToString(), Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn);
                    fn.Message(Page, "Ocurrio un error al intentar grabar el Certificado. Ingreselo nuevamente!");
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
                if (!Page.IsPostBack)
                {
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

                    Sql = "Select (Servicio + '-' + Plaguicida) as Servicio, Nombre from tblServicio where Activo=1 and Tipo='FUMIGACIÓN' and idPais='" + Session["idPais"] + "' order by Nombre";
                    SqlCommand SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDataReader SqlDat = SqlComm.ExecuteReader();
                    ddl_servicio.DataValueField = "Servicio";
                    ddl_servicio.DataTextField = "Nombre";
                    ddl_servicio.DataSource = SqlDat;
                    ddl_servicio.DataBind();
                    SqlDat.Close();

                    Sql = "Select b.Plaguicida, a.Nombre from tblPlaguicida a, tblInvExistencia b where a.Plaguicida like '01%'and a.Plaguicida=b.Plaguicida and b.Puesto='" + Session["idPuesto"] + "' and b.Existencia>0 and a.idPais='" + Session["idPais"] + "' and b.idPais='" + Session["idPais"] + "' order by Nombre";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_plaguicida.DataValueField = "Plaguicida";
                    ddl_plaguicida.DataTextField = "Nombre";
                    ddl_plaguicida.DataSource = SqlDat;
                    ddl_plaguicida.DataBind();
                    SqlDat.Close();

                    String[] plaguicida = ddl_servicio.SelectedValue.Split('-');

                    //ddl_plaguicida.Items.FindByValue(plaguicida[1]).Selected = true;

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

                    Sql = "Select Codigo, Nombre from tblLugTra where idPais='" + Session["idPais"] + "' and Servicio='FUMIGACIÓN' order by Nombre";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_lugar_tratado.DataValueField = "Codigo";
                    ddl_lugar_tratado.DataTextField = "Nombre";
                    ddl_lugar_tratado.DataSource = SqlDat;
                    ddl_lugar_tratado.DataBind();
                    SqlDat.Close();

                    Sql = "select Producto,Nombre from tblProducto where idPais='" + Session["idPais"] + "' order by Nombre";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_producto.DataValueField = "Producto";
                    ddl_producto.DataTextField = "Nombre";
                    ddl_producto.DataSource = SqlDat;
                    ddl_producto.DataBind();
                    SqlDat.Close();

                    lugar_tratado(SqlConn);

                    Sql = "select Codigo,Unidad from tblValUnidad where tipo='UT' and idPais='" + Session["idPais"] + "' order by Unidad";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_unidad_tiempo.DataValueField = "Codigo";
                    ddl_unidad_tiempo.DataTextField = "Unidad";
                    ddl_unidad_tiempo.DataSource = SqlDat;
                    ddl_unidad_tiempo.DataBind();
                    SqlDat.Close();

                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_tiempo_aereacion.DataValueField = "Codigo";
                    ddl_tiempo_aereacion.DataTextField = "Unidad";
                    ddl_tiempo_aereacion.DataSource = SqlDat;
                    ddl_tiempo_aereacion.DataBind();
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

                    Sql = "select Codigo,Unidad from tblValUnidad where tipo='MF' and idPais='" + Session["idPais"] + "' order by Unidad";
                    SqlComm = new SqlCommand(Sql, SqlConn);
                    SqlDat = SqlComm.ExecuteReader();
                    ddl_motivo.DataValueField = "Unidad";
                    ddl_motivo.DataTextField = "Unidad";
                    ddl_motivo.DataSource = SqlDat;
                    ddl_motivo.DataBind();
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
                    tb_hora_fin.Text = hora + ":" + Convert.ToString(DateTime.Now.Minute) + ":" + Convert.ToString(DateTime.Now.Second) + " " + ampm;

                    tb_fecha.Text = Convert.ToString(DateTime.Now.Month) + "/" + Convert.ToString(DateTime.Now.Day) + "/" + Convert.ToString(DateTime.Now.Year);
                    tb_fecha_tratamiento.Text = Convert.ToString(DateTime.Now.Month) + "/" + Convert.ToString(DateTime.Now.Day) + "/" + Convert.ToString(DateTime.Now.Year);
                    tb_fecha_tratamiento_fin.Text = Convert.ToString(DateTime.Now.Month) + "/" + Convert.ToString(DateTime.Now.Day) + "/" + Convert.ToString(DateTime.Now.Year);
                    tb_fecha_orden.Text = Convert.ToString(DateTime.Now.Month) + "/" + Convert.ToString(DateTime.Now.Day) + "/" + Convert.ToString(DateTime.Now.Year);

                    SqlConn.Close();
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

        Sql = "Select b.Plaguicida, a.Nombre from tblPlaguicida a, tblInvExistencia b where a.Plaguicida like '01%'and a.Plaguicida=b.Plaguicida and b.Puesto='" + Session["idPuesto"] + "' and b.Existencia>0 and a.idPais='" + Session["idPais"] + "' and b.idPais='" + Session["idPais"] + "' order by Nombre";
        SqlCommand SqlComm = new SqlCommand(Sql, SqlConn);
        SqlDataReader SqlDat = SqlComm.ExecuteReader();
        ddl_plaguicida.DataValueField = "Plaguicida";
        ddl_plaguicida.DataTextField = "Nombre";
        ddl_plaguicida.DataSource = SqlDat;
        ddl_plaguicida.DataBind();
        SqlDat.Close();

        String[] plaguicida = ddl_servicio.SelectedValue.Split('-');

        //ddl_plaguicida.Items.FindByValue(plaguicida[1]).Selected = true;

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
        tb_cantidad.Focus();
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

        if (tb_dosis.Text.ToString().Length == 0)
            tb_dosis.Text = "0";

        if (tb_cantidad_cubicada.Text.ToString().Length == 0)
            tb_cantidad_cubicada.Text = "0";

        if (tb_consumo_real.Text.ToString().Length == 0)
            tb_consumo_real.Text = "0";

        tb_consumo_teorico.Text = Convert.ToString(Math.Round(Convert.ToDouble(tb_dosis.Text) * Convert.ToDouble(tb_cantidad_cubicada.Text), 2));
        tb_desviacion.Text = Convert.ToString(Convert.ToDouble(tb_consumo_real.Text) - Convert.ToDouble(tb_consumo_teorico.Text));

        SqlConn.Close();

        ddl_unidad_dosis.Focus();
    }

    //Cubicaje a utilizar
    void lugar_tratado(SqlConnection cnn)
    {
        bool cproducto = false;
        bool tipo = false;
        string sql, sqlp;

        sqlp = "select Producto,Nombre from tblProducto where idPais='" + Session["idPais"] + "' order by Nombre";

        tb_contenedor.Visible = true;
        l_contenedor.Visible = true;
        ddl_producto.AutoPostBack = false;

        switch (ddl_lugar_tratado.SelectedItem.Text)
        {
            case "Bodega de Cereales":
                sql = "select idDensidad as Codigo,Descripcion from tblDensidad where Producto=" + ddl_producto.SelectedValue + " and idPais='" + Session["idPais"] + "'";
                sqlp = "select Producto,Nombre from tblProducto where Producto in (select distinct Producto from tblDensidad where idPais='" + Session["idPais"] + "') and idPais='" + Session["idPais"] + "' order by Nombre";
                tipo = true;
                l_tipo.Text = "Densidad";
                //p_tipo.Visible = true;
               // ddl_producto.AutoPostBack = true;
                break;
            case "Contenedor":
                sql = "select idContenedor as Codigo,Nombre as Descripcion from tblContenedor where idPais='" + Session["idPais"] + "'";
                sqlp = "select Producto,Nombre from tblProducto where idPais='" + Session["idPais"] + "' order by Nombre";
                tipo = true;
                l_tipo.Text = "Contenedor";
                //p_tipo.Visible = true;
                break;
            case "Silo":
                sql = "select idSilo as Codigo,Descripcion from tblSilo where idPais='" + Session["idPais"] + "'";
                sqlp = "select Producto,Nombre from tblProducto where idPais='" + Session["idPais"] + "' order by Nombre";
                tipo = true;
                l_tipo.Text = "Silo";
                //p_tipo.Visible = true;
                break;
            default:
                sql = "";
                sqlp = "select Producto,Nombre from tblProducto where idPais='" + Session["idPais"] + "' order by Nombre";
                tipo = false;
                //p_tipo.Visible = false;
                break;
        }

        SqlCommand SqlCommp = new SqlCommand(sqlp, cnn);
        SqlDataReader SqlDatp = SqlCommp.ExecuteReader();
        ddl_producto.DataValueField = "Producto";
        ddl_producto.DataTextField = "Nombre";
        ddl_producto.DataSource = SqlDatp;
        ddl_producto.DataBind();
        SqlDatp.Close();

        if (ddl_lugar_tratado.SelectedItem.Text == "Bodega de Cereales")
        {
            sql = "select idDensidad as Codigo,Descripcion from tblDensidad where Producto=" + ddl_producto.SelectedValue + " and idPais='" + Session["idPais"] + "'";
          //  ddl_producto.AutoPostBack = true;
        }

        if (tipo)
        {
            SqlCommp = new SqlCommand(sql, cnn);
            SqlDatp = SqlCommp.ExecuteReader();
            ddl_tipo.DataValueField = "Codigo";
            ddl_tipo.DataTextField = "Descripcion";
            ddl_tipo.DataSource = SqlDatp;
            ddl_tipo.DataBind();
            SqlDatp.Close();

            switch (ddl_lugar_tratado.SelectedItem.Text)
            {
                case "Bodega de Cereales":
                    sql = "select cubicaje from tblDensidad where idDensidad=" + ddl_tipo.SelectedValue + " and idPais='" + Session["idPais"] + "'";
                    tb_tipo.Text = Convert.ToString(fn.EjecutarScalarDouble(sql, cnn));
                    tb_cantidad_cubicada.Text = "0";
                    break;
                case "Contenedor":
                    sql = "select cubicaje from tblContenedor where idContenedor=" + ddl_tipo.SelectedValue + " and idPais='" + Session["idPais"] + "'";
                    tb_cantidad_cubicada.Text = Convert.ToString(fn.EjecutarScalarDouble(sql, cnn));
                    break;
                case "Silo":
                    sql = "select cubicaje from tblSilo where idSilo=" + ddl_tipo.SelectedValue + " and idPais='" + Session["idPais"] + "'";
                    tb_cantidad_cubicada.Text = Convert.ToString(fn.EjecutarScalarDouble(sql, cnn));
                    break;
            }
        }

        consumo_real();
    }
    [WebMethod(EnableSession = true)]
    [System.Web.Script.Services.ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string ObtenerTipoTra(string Tipo, string idProducto)
    {
        Funciones fn = new Funciones();
        SqlConnection SqlConn = fn.ConnectionSql();

        String sql = ""; 
        switch (Tipo)
        {
            case "Bodega de Cereales":
                sql = "select Cubicaje as Codigo,Descripcion from tblDensidad where Producto=" + idProducto + " and idPais='" + HttpContext.Current.Session["idPais"] + "'";
                break;
            case "Contenedor":
                sql = "select Cubicaje as Codigo,Nombre as Descripcion from tblContenedor where idPais='" + HttpContext.Current.Session["idPais"] + "'";
                break;
            case "Silo":
                sql = "select Cubicaje as Codigo,Descripcion from tblSilo where idPais='" + HttpContext.Current.Session["idPais"] + "'";
                break;
            default:

                break;
        }
        JavaScriptSerializer ser = new JavaScriptSerializer();
        if (string.IsNullOrEmpty(sql))
        {
            return "";
        }
        SqlCommand SqlComm = new SqlCommand(sql, SqlConn);
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = SqlComm;
        DataSet DS = new DataSet();
        da.Fill(DS);
        

        return ser.Serialize(Helpers.ToList(DS.Tables[0], false));

        
    }
    [WebMethod(EnableSession = true)]
    [System.Web.Script.Services.ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string ObtenerProductoInfo(string Tipo)
    {
        Funciones fn = new Funciones();
        SqlConnection SqlConn = fn.ConnectionSql();

        String sqlp = "select Producto,Nombre from tblProducto where idPais='" + HttpContext.Current.Session["idPais"] + "' order by Nombre";
        switch (Tipo)
        {
            case "Bodega de Cereales":
                sqlp = "select Producto,Nombre from tblProducto where Producto in (select distinct Producto from tblDensidad where idPais='" + HttpContext.Current.Session["idPais"] + "') and idPais='" + HttpContext.Current.Session["idPais"] + "' order by Nombre";
                break;
            case "Contenedor":
            case "Silo":
                sqlp = "select Producto,Nombre from tblProducto where idPais='" + HttpContext.Current.Session["idPais"] + "' order by Nombre";
                break;
            default:
                sqlp = "select Producto,Nombre from tblProducto where idPais='" + HttpContext.Current.Session["idPais"] + "' order by Nombre";
                break;
        }

        SqlCommand SqlComm = new SqlCommand(sqlp, SqlConn);
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = SqlComm;
        DataSet DS = new DataSet();
        da.Fill(DS);
        JavaScriptSerializer ser = new JavaScriptSerializer();

        return ser.Serialize(Helpers.ToList(DS.Tables[0], false));


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
        String contenedor, densidad, silo;

        densidad = "Null";
        contenedor = "Null";
        silo = "Null";

        //Se borran registros temporales en el Kardex que no sean de esta session
        Sql = "delete from tblInvKardex where Procesado=0 and Cast(Certificados as char(20))='" + l_n_certificado.Text + "' and idPais='" + Session["idPais"] + "' and [Session]<>'" + Session["session"] + "'";
        fn.EjecutarNonQuery(Sql, SqlConn);

        //Se borran detalles del certificado que no sean de esta session
        Sql = "delete from tblCertifDet where NCertificado='" + l_n_certificado.Text + "' and Puesto='" + Session["idPuesto"] + "' and idPais='" + Session["idPais"] + "' and Session<>'" + Session["session"] + "'";
        fn.EjecutarNonQuery(Sql, SqlConn);

        //Se busca si existe consumo temporal para esta session
        Sql = "select sum(egreso) from tblInvKardex where procesado=0 and Cast(Certificados as char(20))='" + l_n_certificado.Text + "' and Plaguicida='" + ddl_plaguicida.Text + "' and Puesto='" + Session["idPuesto"] + "' and idPais='" + Session["idPais"] + "'";
        Double egreso_temporal = fn.EjecutarScalarDouble(Sql, SqlConn);

        //Se busca la existencia actual y se le quita el consumo temporal
        Double existencia;
        if (fn.ValIni(Session["idPais"].ToString(), "ConsumoAutomatico", SqlConn) == "1")
            existencia = fn.Plaguicida_Existencia(ddl_plaguicida.SelectedValue, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn) - egreso_temporal;
        else
            existencia = 100000000;

        if (Convert.ToDouble(tb_consumo_real.Text)<=existencia && fn.Certificado_Existe(l_n_certificado.Text, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn) == 0 && Convert.ToDouble(tb_consumo_real.Text) > 0)
        {
            switch (ddl_lugar_tratado.SelectedItem.Text)
            {
                case "Bodega de Cereales":
                    densidad = "'" + ddl_tipo.SelectedItem.ToString() + "'";
                    break;
                case "Contenedor":
                    if (ddl_tipo.SelectedItem.ToString() == "Otro")
                        contenedor = "'" + tb_contenedor.Text + "'";
                    else
                        contenedor = "'" + ddl_tipo.SelectedItem.ToString() + "'";
                    break;
                case "Silo":
                    silo = "'" + ddl_tipo.SelectedItem.ToString() + "'";
                    break;
            }

            String concentracion;
            if (tb_concentracion.Text.Length > 0)
                concentracion = "'" + tb_concentracion.Text + "'";
            else
                concentracion = "Null";

            String temperatura;
            if (tb_temperatura.Text.Length > 0)
                temperatura = tb_temperatura.Text;
            else
                temperatura = "Null";

            String tiempo_aereacion;
            String ut_aereacion;
            if (tb_tiempo_aereacion.Text.Length > 0)
            {
                tiempo_aereacion = tb_tiempo_aereacion.Text;
                ut_aereacion = "'" + ddl_tiempo_aereacion.SelectedItem.Text + "'";
            }
            else
            {
                tiempo_aereacion = "Null";
                ut_aereacion = "Null";
            }

            Sql = "insert into tblCertifDet (Puesto,NCertificado,Servicio,Cantidad,US,Local,SubTotal,Plaguicida,Dosis,UD,Real,Producto,Ruta,Procedencia,Destino,TiempoExposicion,UT,DB,Session,CantVol,UC,CantidadCubicada,Teorico,Densidad,Contenedor,Silo,LugTrat,Origen,Concentracion,Temperatura,TiempoAereacion,UT_Aereacion,idPais) values ('" + Session["idPuesto"] + "','";
            Sql = Sql + l_n_certificado.Text + "','" + servicio[0] + "'," + tb_cantidad.Text + "," + tb_costo_us.Text + "," + Convert.ToString(Math.Round(Convert.ToDouble(tb_costo_local.Text), 2)) + "," + Convert.ToString(Math.Round(Convert.ToDouble(tb_subtotal.Text), 2)) + ",'";
            Sql = Sql + ddl_plaguicida.SelectedValue + "'," + tb_dosis.Text + ",'" + dosis[0] + "'," + tb_consumo_real.Text + ",'" + ddl_producto.SelectedValue + "','" + ddl_ruta.SelectedItem + "','";
            Sql = Sql + ddl_procedencia.SelectedItem + "','" + ddl_destino.SelectedItem + "'," + tb_tiempo_exposicion.Text + ",'" + ddl_unidad_tiempo.SelectedItem + "',0,'" + Session["session"] + "'," + tb_peso_producto.Text + ",'" + ddl_peso_producto.SelectedItem + "'," + tb_cantidad_cubicada.Text + "," + tb_consumo_teorico.Text + ",";
            Sql = Sql + densidad + "," + contenedor + "," + silo + ",'" + ddl_lugar_tratado.SelectedItem + "','" + ddl_origen.SelectedItem + "'," + concentracion + "," + temperatura + "," + tiempo_aereacion + "," + ut_aereacion + ",'" + Session["idPais"] + "')";

            fn.EjecutarNonQuery(Sql, SqlConn);

            l_servicios.Text = Convert.ToString(Convert.ToDouble(l_servicios.Text) + 1);

            BindData(SqlConn);

            if (fn.ValIni(Session["idPais"].ToString(), "ConsumoAutomatico", SqlConn) == "1")
            {
                //Se recupera el ultimo detalle de la tblCertifDet
                Sql = "select max(detalle) from tblCertifDet where Puesto='" + Session["idPuesto"] + "' and idPais='" + Session["idPais"] + "' and NCertificado='" + l_n_certificado.Text + "'";
                Double ultimoid = fn.EjecutarScalarDouble(Sql, SqlConn);

                //Registro de salida del quimico
                Double nueva_existencia = existencia - Convert.ToDouble(tb_consumo_real.Text);

                String id = Convert.ToDateTime(tb_fecha.Text).Year.ToString() + Convert.ToDateTime(tb_fecha.Text).Month.ToString() + Convert.ToDateTime(tb_fecha.Text).Day.ToString() + "-" + Convert.ToString(fn.Recuperar_Correlativo(Session["idPais"].ToString(), Session["idPuesto"].ToString(), "IdKardex", SqlConn));

                Sql = "select Costo from tblPlaguicida where idPais='" + Session["idPais"].ToString() + "' and plaguicida='" + ddl_plaguicida.SelectedValue + "'";
                Double costo = fn.EjecutarScalarDouble(Sql, SqlConn);

                String fecha = tb_fecha.Text + " " + DateTime.Now.ToLongTimeString();
                String fecha_d = Convert.ToString(DateTime.Now.Month) + "/" + Convert.ToString(DateTime.Now.Day) + "/" + Convert.ToString(DateTime.Now.Year) + " " + DateTime.Now.ToLongTimeString();

                //Se agrega el egreso temporal
                Sql = "insert into tblInvKardex (Puesto,id,Plaguicida,Existencia,Egreso,Ingreso,NuevaExistencia,Responsable,Fecha,Procesado,Replicado,Concepto,Certificados,Costo,FechaDigitacion,idDetFum,[Session],idPais) values ('";
                Sql = Sql + Session["idPuesto"] + "','" + id + "','" + ddl_plaguicida.SelectedValue + "'," + existencia + "," + tb_consumo_real.Text + ",0," + nueva_existencia + ",'";
                Sql = Sql + Session["login"] + "','" + fecha + "',0,0,'Consumo automatico por fumigación [NCertificado:" + l_n_certificado.Text + "]','" + l_n_certificado.Text + "'," + costo.ToString() + ",'" + fecha_d + "','" + Convert.ToString(ultimoid) + "','" + Session["session"] + "','" + Session["idPais"] + "')";

                fn.EjecutarNonQuery(Sql, SqlConn);
            }

            fn.Incrementar_Correlativo(Session["idPais"].ToString(), Session["idPuesto"].ToString(), "IdKardex", SqlConn);

            cfe_grabar.ConfirmText = "Certificado :" + l_n_certificado.Text + " \n Cliente: " + ddl_cliente.SelectedItem + " \n Monto a cobrar: " + tb_total.Text + " \n\n Desea Continuar?";
        }
        else
        {
            if (Convert.ToDouble(tb_consumo_real.Text) == 0)
            {
                fn.Message(Page, "El campo CONSUMO REAL no puede ser igual a CERO!");
                tb_consumo_real.Focus();
            }
            else
            {
                if (Convert.ToDouble(tb_consumo_real.Text) > existencia)
                {
                    fn.Message(Page, "El consumo real sobrepasa la existencia actual de este químico!");
                }
                else
                {
                    fn.Message(Page, "Certificado ya existe! Tiene que definir el Consecutivo");
                    b_aceptar.Visible = false;
                    b_grabar.Visible = false;
                }
            }
        }

        SqlConn.Close();
        tb_recargo.Focus();
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
        ddl_lugar_tratado.Focus();
    }

    //Proceso que elimina los servicios agregados
    protected void gv_servicios_SelectedIndexChanged(object sender, EventArgs e)
    {
        String Sql;
        SqlConnection SqlConn = fn.ConnectionSql();

        Sql = "delete tblCertifDet where Detalle=" + gv_servicios.SelectedDataKey.Value.ToString();
        fn.EjecutarNonQuery(Sql, SqlConn);

        Sql = "delete tblInvKardex where idDetFum='" + gv_servicios.SelectedDataKey.Value.ToString() + "' and Puesto='" + Session["idPuesto"] + "' and idPais='" + Session["idPais"] + "'";
        fn.EjecutarNonQuery(Sql, SqlConn);

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

        //Se verifica si se desea agregar archivo de orden de tratamiento
        String narchivo = "";
        Boolean enarchivo = true;
        int tarchivo = 0;
        if (fu_archivo.PostedFile != null && fu_archivo.PostedFile.ContentLength > 0)
        {
            narchivo = System.IO.Path.GetFileName(fu_archivo.PostedFile.FileName); //Nombre del archivo
            tarchivo = fu_archivo.PostedFile.ContentLength; //Tamaño del archivo

            String[] extension = narchivo.Split('.');
            if (extension[extension.GetUpperBound(0)].ToLower() != "jpg" && extension[extension.GetUpperBound(0)].ToLower() != "gif" && extension[extension.GetUpperBound(0)].ToLower() != "png" && extension[extension.GetUpperBound(0)].ToLower() != "pdf")
                enarchivo = false;
        }

        if (tarchivo < 102400 && enarchivo && Convert.ToDouble(tb_subtotalg.Text) == Math.Round(total, 2) && cert_det == Convert.ToDouble(l_servicios.Text) && cert_det > 0 && ddl_cliente.SelectedValue.Trim() != "" && ddl_cuarentena.SelectedValue.Trim() != "" && tb_fecha_tratamiento.Text.Trim().Length != 0)
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

            String norden;
            if (tb_numero_orden.Text.Length == 0)
            {
                norden = "";
            }
            else
            {
                norden = tb_numero_orden.Text;
            }

            String fecha_orden;
            if (tb_fecha_orden.Text.ToString().Length == 0)
            {
                fecha_orden = DateTime.Now.ToString();
            }
            else
            {
                fecha_orden = tb_fecha_orden.Text;
            }

            String tipo_cliente;
            if (rb_tipo_cliente_i.Checked)
                tipo_cliente = "I";
            else
                tipo_cliente = "E";

            String fecha_trat_ini = tb_fecha_tratamiento.Text + " " + Convert.ToDateTime(tb_hora_ini.Text).ToLongTimeString();
            String fecha_trat_fin = tb_fecha_tratamiento_fin.Text + " " + Convert.ToDateTime(tb_hora_fin.Text).ToLongTimeString(); 

            String fecha = tb_fecha.Text + " " + DateTime.Now.ToLongTimeString();

            SqlCertif = "insert into tblCertif (Puesto,Ncertificado,Fecha,Cambio,Cortesia,Local,Total,TotalString,Observacion,Responsable,Anulado,Remesado,Replicado,NEnd,TipoCertificado,TipoCliente,ClienteExtra,Cliente,NAduana,Placa,FechaTrat,FechaTrat_Fin,NOrden,FOrden,AOrden,Cuarentena,idPais) values ('";
            SqlCertif = SqlCertif + Session["idPuesto"] + "','" + l_n_certificado.Text + "','" + fecha.ToString() + "'," + tb_cambio.Text + "," + Cortesia.ToString() + "," + Local + "," + tb_total.Text + ",'" + l_numlet.Text + "','" + observacion.ToString() + "','";
            SqlCertif = SqlCertif + Session["login"] + "',0,0,0,1,'FUMIGACIÓN TERRESTRE','" + tipo_cliente + "','" + tb_cliente.Text + "','" + ddl_cliente.SelectedValue + "','" + ddl_motivo.SelectedValue + "','" + nplaca.ToString() + "','" + fecha_trat_ini + "','" + fecha_trat_fin + "','" + norden + "','" + fecha_orden + "','" + narchivo + "','" + ddl_cuarentena.SelectedValue + "','" + Session["idPais"] + "')";

            bool error = false;

            try
            {
                fn.EjecutarNonQuery(SqlCertif, SqlConn);
                fn.EjecutarNonQuery(SqlCertifMdp, SqlConn);

                //Se graba el archivo en el servidor
                if (narchivo.Length > 0)
                    fu_archivo.PostedFile.SaveAs(Server.MapPath(".").ToString() + "\\images\\" + narchivo);
            }
            catch
            {
                error = true;
            }
            finally
            {
                if (error)
                {
                    Response.Redirect("Cert_Fumigacion_Terrestre.aspx?error=ok&ncertif=" + l_n_certificado.Text);
                }
                else
                {
                    if (fn.Verificar_Certificado(l_n_certificado.Text, Session["idPuesto"].ToString(), Session["idPais"].ToString(), SqlConn))
                    {
                        //Se actualiza el inventario
                        if (fn.ValIni(Session["idPais"].ToString(), "ConsumoAutomatico", SqlConn) == "1")
                            fn.ActualizarInventarioCertif(Session["idPuesto"].ToString(), Session["idPais"].ToString(), l_n_certificado.Text, Session["session"].ToString(), SqlConn);

                        fn.Incrementar_Correlativo(Session["idPais"].ToString(), Session["idPuesto"].ToString(), "NCertificado", SqlConn);

                        //Se registra en auditoria
                        fn.Auditoria(Session["login"].ToString(), "Agregar Certificado", l_n_certificado.Text, Session["idPais"].ToString(), SqlConn);

                        SqlConn.Close();

                        Response.Redirect("Cert_Fumigacion_Terrestre.aspx?save=ok&ncertif=" + l_n_certificado.Text);
                    }
                    else
                    {
                        Response.Redirect("Cert_Fumigacion_Terrestre.aspx?error=ok&ncertif=" + l_n_certificado.Text);
                    }
                }
            }
        }
        else
        {
            if (cert_det == 0 || cert_det != Convert.ToDouble(l_servicios.Text) || Convert.ToDouble(tb_subtotalg.Text) != Math.Round(total, 2))
            {
                Response.Redirect("Cert_Fumigacion_Terrestre.aspx?error=ok");
            }
            else
            {
                if (enarchivo == false)
                {
                    fn.Message(Page, "Tipo de archivo invalido! Tipo de arhivos validos GIF, JPG, PNG ó PDF.");
                }
                else
                {
                    if (tarchivo > 102400)
                        fn.Message(Page, "No se puede grabar este archivo ya que excede los 100 KB de limite!");
                    else
                        fn.Message(Page, "Verificar! Hay campos que son obligatorios como el CLIENTE, FECHA TRATAMIENTO y ENCARGADO DE CUARENTENA");
                }
            }
        }
    }

    protected void ddl_cliente_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlConnection SqlConn = fn.ConnectionSql();

        cfe_grabar.ConfirmText = "Certificado :" + l_n_certificado.Text + " \n Cliente: " + ddl_cliente.SelectedItem + " \n Monto a cobrar: " + tb_total.Text + " \n\n Desea Continuar?";
        Informacion_Cliente(ddl_cliente.SelectedValue);
        lugar_tratado(SqlConn);
        tb_cliente.Focus();

        SqlConn.Close();
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
            tb_dosis.Text = "0";
        }

        tb_consumo_teorico.Text = Convert.ToString(Math.Round(Convert.ToDouble(tb_dosis.Text) * Convert.ToDouble(tb_cantidad_cubicada.Text), 2));
        tb_desviacion.Text = Convert.ToString(Convert.ToDouble(tb_consumo_real.Text) - Convert.ToDouble(tb_consumo_teorico.Text));

        consumo_real();
        ddl_producto.Focus();
    }
    [WebMethod(EnableSession = true)]
    [System.Web.Script.Services.ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string ServiceSobreCargo(string Servicio)
    {
        Funciones fn = new Funciones();
        SqlConnection SqlConn = fn.ConnectionSql();
        

        //ddl_plaguicida.Items.FindByValue(plaguicida[1]).Selected = true;

        string Sql = "select   Servicio + '-' + CONVERT(char(10), PrcUnit) AS Servicio, Nombre, Plaguicida "
        + " from tblServicio where Activo=1 and Tipo='" + Servicio + "' and idPais='" + HttpContext.Current.Session["idPais"].ToString() + "'";

        SqlCommand SqlComm = new SqlCommand(Sql, SqlConn);
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = SqlComm;
        DataSet DS = new DataSet();
        da.Fill(DS);
        JavaScriptSerializer ser = new JavaScriptSerializer();


        return ser.Serialize(Helpers.ToList(DS.Tables[0], false));
    }
    protected void tb_cantidad_cubicada_TextChanged(object sender, EventArgs e)
    {
        consumo_real();
        tb_consumo_real.Focus();
    }

    protected void tb_consumo_real_TextChanged(object sender, EventArgs e)
    {
        if (tb_dosis.Text.ToString().Length == 0)
            tb_dosis.Text = "0";

        if (tb_cantidad_cubicada.Text.ToString().Length == 0)
            tb_cantidad_cubicada.Text = "0";

        if (tb_consumo_real.Text.ToString().Length == 0)
            tb_consumo_real.Text = "0";

        tb_desviacion.Text = Convert.ToString(Math.Round(Convert.ToDouble(tb_consumo_real.Text) - Convert.ToDouble(tb_consumo_teorico.Text), 2));
        tb_tiempo_exposicion.Focus();
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

    //Se limpian las salidas de insumos temporales sin procesar
    void LimpiarTemporales(SqlConnection cnn)
    {
        String Sql;

        Sql = "delete from tblInvKardex where Procesado=0 and Puesto='" + Session["idPuesto"] + "' and idPais='" + Session["idPais"] + "'";
        fn.EjecutarNonQuery(Sql, cnn);
    }

    protected void ddl_lugar_tratado_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlConnection SqlConn = fn.ConnectionSql();

        lugar_tratado(SqlConn);
        ddl_producto.Focus();

        SqlConn.Close();
    }

    protected void ddl_producto_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_lugar_tratado.SelectedItem.Text == "Bodega de Cereales")
        {
            String sql;
            SqlConnection SqlConn = fn.ConnectionSql();

            sql = "select idDensidad as Codigo,Descripcion from tblDensidad where Producto=" + ddl_producto.SelectedValue + " and idPais='" + Session["idPais"] + "'";

            SqlCommand SqlComm = new SqlCommand(sql, SqlConn);
            SqlDataReader SqlDat = SqlComm.ExecuteReader();
            ddl_tipo.DataValueField = "Codigo";
            ddl_tipo.DataTextField = "Descripcion";
            ddl_tipo.DataSource = SqlDat;
            ddl_tipo.DataBind();
            SqlDat.Close();

            sql = "select cubicaje from tblDensidad where idDensidad=" + ddl_tipo.SelectedValue + " and idPais='" + Session["idPais"] + "'";
            tb_tipo.Text = Convert.ToString(fn.EjecutarScalarDouble(sql, SqlConn));

            tb_cantidad_cubicada.Text = "0";

            consumo_real();

            tb_peso_producto_tm.Focus();

            SqlConn.Close();
        }
    }

    [WebMethod(EnableSession = true)]
    [System.Web.Script.Services.ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string ObtenerCubicajeInfo(string Tipo,String tipodes) {
        String sql="";
        switch (tipodes)
        {
            case "Bodega de Cereales":
                sql = "select Descripcion Nombre, Cubicaje from tblDensidad where idDensidad=" + Tipo + " and idPais='" + HttpContext.Current.Session["idPais"] + "'";
                
                break;
            case "Contenedor":
                sql = "select  Nombre,Cubicaje from tblContenedor where idContenedor=" + Tipo + " and idPais='" + HttpContext.Current.Session["idPais"] + "'";
                
                break;
            case "Silo":
                sql = "select Descripcion Nombre, Cubicaje  from tblSilo where idSilo=" + Tipo + " and idPais='" + HttpContext.Current.Session["idPais"] + "'";                
                break;
        }
        if (sql != "")
        {
            Funciones fn = new Funciones();
            fn.ConnectionSql();
            SqlDataAdapter da = new SqlDataAdapter(sql, fn._LocalC);
            DataSet DS = new DataSet();
            da.Fill(DS);
            fn._LocalC.Close();

            JavaScriptSerializer ser = new JavaScriptSerializer();

            return ser.Serialize(Helpers.ToList(DS.Tables[0], false));
        }
        else {
            return "Otro";
        }

    }
    protected void ddl_tipo_SelectedIndexChanged(object sender, EventArgs e)
    {
        String sql;
        SqlConnection SqlConn = fn.ConnectionSql();
        Boolean contenedor_otro = false;

        switch (ddl_lugar_tratado.SelectedItem.Text)
        {
            case "Bodega de Cereales":
                sql = "select cubicaje from tblDensidad where idDensidad=" + ddl_tipo.SelectedValue + " and idPais='" + Session["idPais"] + "'";
                tb_tipo.Text = Convert.ToString(fn.EjecutarScalarDouble(sql, SqlConn));
                tb_cantidad_cubicada.Text = "0";
                break;
            case "Contenedor":
                sql = "select cubicaje from tblContenedor where idContenedor=" + ddl_tipo.SelectedValue + " and idPais='" + Session["idPais"] + "'";
                tb_cantidad_cubicada.Text = Convert.ToString(fn.EjecutarScalarDouble(sql, SqlConn));
                if (ddl_tipo.SelectedItem.ToString() == "Otro")
                    contenedor_otro = true;
                break;
            case "Silo":
                sql = "select cubicaje from tblSilo where idSilo=" + ddl_tipo.SelectedValue + " and idPais='" + Session["idPais"] + "'";
                tb_cantidad_cubicada.Text = Convert.ToString(fn.EjecutarScalarDouble(sql, SqlConn));
                break;
        }

        consumo_real();

        tb_consumo_real.Text = "0";

        if (contenedor_otro)
        {
            fn.Message(Page, "Debe de ingresar el tipo de contenedor y luego la cantidad cubicada!");
            tb_contenedor.Visible = true;
            l_contenedor.Visible = true;
            tb_contenedor.Focus();
        }
        else
        {
            tb_contenedor.Visible = true;
            l_contenedor.Visible = true;
            tb_consumo_real.Focus();
        }

        SqlConn.Close();
    }

    void consumo_real()
    {
        if (tb_dosis.Text.ToString().Length == 0)
            tb_dosis.Text = "0";

        if (tb_cantidad_cubicada.Text.ToString().Length == 0)
            tb_cantidad_cubicada.Text = "0";

        if (tb_consumo_real.Text.ToString().Length == 0)
            tb_consumo_real.Text = "0";

        if (ddl_lugar_tratado.SelectedItem.Text == "Bodega de Cereales")
        {
            // (peso * dosis[1000 p3/28 m3])/densidad
            tb_cantidad_cubicada.Text = Convert.ToString(Math.Round(Convert.ToDouble(tb_peso_producto.Text) / Convert.ToDouble(tb_tipo.Text), 2));
            tb_consumo_teorico.Text = Convert.ToString(Math.Round((Convert.ToDouble(tb_dosis.Text) * Convert.ToDouble(tb_peso_producto.Text)) / Convert.ToDouble(tb_tipo.Text), 2));
            tb_desviacion.Text = Convert.ToString(Convert.ToDouble(tb_consumo_real.Text) - Convert.ToDouble(tb_consumo_teorico.Text));
        }
        else
        {
            if (ddl_lugar_tratado.SelectedItem.Text == "Silo")
            {
                // cubicaje * dosis[35.3 p3/1 m3]
                tb_consumo_teorico.Text = Convert.ToString(Math.Round(Convert.ToDouble(tb_dosis.Text) * Convert.ToDouble(tb_cantidad_cubicada.Text), 2));
                tb_desviacion.Text = Convert.ToString(Convert.ToDouble(tb_consumo_real.Text) - Convert.ToDouble(tb_consumo_teorico.Text));
            }
            else
            {
                tb_consumo_teorico.Text = Convert.ToString(Math.Round(Convert.ToDouble(tb_dosis.Text) * Convert.ToDouble(tb_cantidad_cubicada.Text), 2));
                tb_desviacion.Text = Convert.ToString(Convert.ToDouble(tb_consumo_real.Text) - Convert.ToDouble(tb_consumo_teorico.Text));
            }

        }
    }

    protected void tb_peso_producto_tm_TextChanged(object sender, EventArgs e)
    {
        if(tb_peso_producto_tm.Text != "0" && tb_peso_producto_tm.Text.Trim().Length > 0)
        {
            tb_peso_producto.Text = Convert.ToString(Math.Round(Convert.ToDouble(tb_peso_producto_tm.Text) * 22.046, 3));
            ddl_peso_producto.Text = "QQ";
        }
        else
        {
            tb_peso_producto_tm.Text = "0";
            tb_peso_producto.Text = "0";
        }

        consumo_real();
        tb_consumo_real.Focus();
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

    protected void tb_peso_producto_TextChanged(object sender, EventArgs e)
    {
        consumo_real();
    }

    public static void RedirectBandeja() {
        Utilidades Util = new Utilidades();
       //Puesto = GetPuesto();
       //NoOrden = GetNoOrden();
        string Accion = Util.Acciones(GetPuesto(), GetNoOrden());
        if (Accion.Equals("HacerRedirect")) {
            HttpContext.Current.Response.Redirect("Bandeja_Ordenes.aspx");
        }
    }
    public static string GetPuesto() {
        Utilidades Util = new Utilidades();
        string Puesto = Util.ObtenerValorVariable(HttpContext.Current.Request.QueryString["pto"]);
        return Puesto;
    }
    public static string GetNoOrden() {
        Utilidades Util = new Utilidades();
        string NoOrden = Util.ObtenerValorVariable(HttpContext.Current.Request.QueryString["ord"]);
        return NoOrden;
    }    

    [WebMethod()]
    public static List<Dictionary<string, object>> GetServiciosCIEX(string Pto, string Ord) {
        Utilidades Util = new Utilidades();
        string Puesto = Util.ObtenerValorVariable(Pto);
        string NoOrden = Util.ObtenerValorVariable(Ord);
        return Util.GetDataTblOrdenMAGDet(Puesto, NoOrden);
    }

    [WebMethod()]
    public static string AccionBandejaCIEX(string Pto, string Ord)
    {
        if (Pto.ToLower().Equals("false") || Ord.ToLower().Equals("false")) {
            return "ProcesoNormal";
        }
        Utilidades Util = new Utilidades();
        string Puesto = Util.ObtenerValorVariable(Pto);
        string NoOrden = Util.ObtenerValorVariable(Ord);
        return Util.Acciones(Puesto, NoOrden);
    }

    [WebMethod()]
    public static string Prueba(object Arr) {
        JavaScriptSerializer obj = new JavaScriptSerializer();
        Pojo PObject = obj.ConvertToType<Pojo>(Arr);
        return PObject.Encabezado.Nombre;
    }

}
public class Pojo {
    public Encabezado Encabezado;
}

public class Encabezado {
    public string Nombre;
}