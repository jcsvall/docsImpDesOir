<%@ Page Language="C#" MasterPageFile="~/Sitc_Sv.master" AutoEventWireup="true" CodeFile="Bandeja_CIEX.aspx.cs"
    Inherits="Bandeja_CIEX" Title="OIRSA - SITC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SampleContent" runat="Server">

<div id="cargandoContenido">
    <em>Cargando contenido...</em>
</div>
<div id="contenidoPagina" style="display:none">

    <div class="form-group row">
        <div class="col-sm-10">
            <input type="text" class="form-control form-control-sm" id="busquedaValue" placeholder="Busqueda por No. Orden, Tratamiento, Placa, Cliente, Fecha dd-MM-yyyy o dd/MM/yyyy" />
        </div>
        <button type="button" class="btn btn-primary btn-sm" id="buscar"><span class="oi oi-magnifying-glass" title="Buscar"></span>Buscar</button>
    </div>

   <table id="dataGrid" style="text-align: center;"></table>
   <div id="pagingGrid"></div>

</div>    
    
    <link rel="stylesheet" href="<%= Page.ResolveUrl("~/css/Jquery/base/jquery.ui.all.css")%>"/>
    <link href="../css/jqgrid/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <script src="../JS/fomi/js/jquery-1.9.0.min.js" type="text/javascript"></script> 
    <script src="../JS/fomi/js/popper.min.js" type="text/javascript"></script>
    <script src="../JS/fomi/js/bootstrap.min.js" type="text/javascript"></script>  
    <link rel="stylesheet" href="../JS/fomi/css/bootstrap.min.css" />
    <script src="<%= Page.ResolveUrl("~/JS/Personalizados/JQuery/custom/jquery-ui-1.10.4.custom.min.js") %>"></script>   
    <script src="<%= Page.ResolveUrl("~/JS/jQridjs/js/i18n/grid.locale-en.js") %>"></script>
    <script src="<%= Page.ResolveUrl("~/JS/jQridjs/js/jquery.jqGrid.min.js") %>"></script>
    
    
    <script src="../JS/fomi/js/alertify.min.js" type="text/javascript"></script>
    <link rel="stylesheet" href="../JS/fomi/css/alertify.min.css" />
    <link rel="stylesheet" href="../JS/fomi/css/default.min.css" />
    <script src="../JS/fomi/js/jquery.inputmask.js" type="text/javascript"></script>
    <script src="../JS/fomi/js/jquery.blockUI.js" type="text/javascript"></script>   
    <link href="../JS/fomi/css/icon-iconic/css/open-iconic-bootstrap.css" rel="stylesheet">     
    <script src="../JS/Core/BandejaCIEX.js" type="text/javascript"></script>
    <link rel="stylesheet" href="../JS/fomi/css/defaultStyle.css" />
    
</asp:Content>
