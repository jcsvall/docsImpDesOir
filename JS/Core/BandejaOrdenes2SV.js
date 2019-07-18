var busqueda = "";
$(document).ready(function () {
    init();
    cargarGrid();
});

function init() {
    $("#cargandoContenido").hide();
    $("#contenidoPagina").show();
    $("#buscar").click(function(){
        buscarEnGrid();
    });
    $("#busquedaValue").keypress(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 13) {
            buscarEnGrid();
        }
    });
}

function cargarGrid() {
    jQuery("#dataGrid").jqGrid({
        url: 'Bandeja_Orden.aspx/GetBandejaData',
        datatype: "json",
        mtype: 'POST',
        postData: { Busqueda: busqueda},
        height: 'auto',
        width: '690',
        serializeGridData: function (postData) {
            return JSON.stringify(postData);
        },
        ajaxGridOptions: { contentType: "application/json" },
        loadonce: true,
        colNames: ['Fecha', 'No. de Orden', 'Operacion MAG', 'Tipo Tratamiento', 'Puesto', 'Cliente', 'Responsable MAG', 'Placa/Vapor', 'Producto', 'NoOrdenEncryp', 'PuestoEncryp', 'Acci&oacute;n'],
        colModel: [            
            { name: 'FechaOrden', index: 'FechaOrden', width: 130,sorttype: "date", formatter: "date", formatoptions: { srcformat: "ISO8601Long", newformat: "d/m/Y h:i A" } },
            { name: 'NoOrdenMag', index: 'NoOrdenMag', width: 100 },
            { name: 'OperacionMAG', index: 'OperacionMAG', width: 150 },
            { name: 'Tratamiento', index: 'Tratamiento', width: 150, align: "center" },
            { name: 'Puesto', index: 'Puesto', width: 70, align: "center" },
            { name: 'Cliente', index: 'Cliente', width: 70, align: "center" },
            { name: 'InspectorMAG', index: 'InspectorMAG', width: 90, align: "center" },
            { name: 'PlacaVapor', index: 'PlacaVapor', width: 75, align: "center" },
            { name: 'Producto', index: 'Producto', width: 90, align: "center" },
            { name: 'NoOrdenEncryp', index: 'NoOrdenEncryp', width: 10, hidden: true },
            { name: 'PuestoEncryp', index: 'PuestoEncryp', width: 10, hidden: true },
            { name: 'act', index: 'act', width: 70, sortable: false, align: "center" },
        ],
        rowNum: 10,
        rowList: [10, 20, 30],
        pager: '#pagingGrid',
        sortname: 'FechaOrden',
        viewrecords: true,
        sortorder: "desc",
        shrinkToFit: false,
        gridComplete: function(){
            var ids = jQuery("#dataGrid").jqGrid('getDataIDs');
            for(var i=0;i < ids.length;i++){
                var cl = ids[i];                                
                var rowSelected = jQuery("#dataGrid").jqGrid('getRowData', cl);                
                be = "<button type='button' class='btn btn-primary btn-sm' onClick='goTratamiento(" + JSON.stringify(rowSelected) + ")'><span class='oi oi-folder' style='font-size:90%' title='Abrir'></span></button> ";
                jQuery("#dataGrid").jqGrid('setRowData', ids[i], { act: be });
            }	
        },
        caption: "Bandeja de ordenes CIEX"
    });//.jqGrid("filterToolbar");
    jQuery("#dataGrid").jqGrid('navGrid', '#pagingGrid', { search: false, edit: false, add: false, del: false });
}


function goTratamiento(objeto) {
    var aspxPage = "";
    var tipoTratamiento = objeto.Tratamiento;
    //alert(tipoTratamiento);
    
    switch (tipoTratamiento.toUpperCase().trim()) {
        case "ASPERSION MARITIMA":
            aspxPage = "Cert_Aspersion_TerrestreJ.aspx?select=0&tp=M";
            break;
        case "ASPERSION TERRESTRE":
            aspxPage = "Cert_Aspersion_TerrestreJ.aspx?select=0";
            break;
        case "FUMIGACION MARITIMA":
            aspxPage = "Cert_Fumigacion_TerrestreJ.aspx?tp=M&select=0";
            break;
        case "FUMIGACION TERRESTRE":
            aspxPage = "Cert_Fumigacion_TerrestreJ.aspx?select=0";
            break;
        case "NEBULIZACION MARITIMA":
            aspxPage = "Cert_Nebulizacion_Terrestre.aspx?select=0&tp=M";
            objeto.PuestoEncryp = replaceAll(objeto.PuestoEncryp, "/", "_");
            objeto.NoOrdenEncryp = replaceAll(objeto.NoOrdenEncryp, "/", "_");
            break;
        case "NEBULIZACION TERRESTRE":
            aspxPage = "Cert_Nebulizacion_Terrestre.aspx?select=0";
            objeto.PuestoEncryp = replaceAll(objeto.PuestoEncryp, "/", "_");
            objeto.NoOrdenEncryp = replaceAll(objeto.NoOrdenEncryp, "/", "_");
            break;
        default:
            aspxPage = "";
            break;
    }
    if (aspxPage != "") {
        window.location.href = aspxPage + "&pto=" + objeto.PuestoEncryp + "&ord=" + objeto.NoOrdenEncryp;
        //alert("redir: " + aspxPage + "&pto=" + objeto.PuestoEncryp + "&ord=" + objeto.NoOrdenEncryp);
        mensajeDeSalidaPagina();
    } else {
        mensajeErrorDialog("El tipo de tratamiento no es valido.");
    }
}

function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, 'g'), replace);
}

function mensajeDeSalidaPagina() {
    $.blockUI({ message: "Cargando..." });
}

function buscarEnGrid() {
    busqueda = $("#busquedaValue").val();    
    var datos = getDataSearch();    
    jQuery('#dataGrid').jqGrid('clearGridData');
    jQuery('#dataGrid').jqGrid('setGridParam', { data: datos });
    jQuery('#dataGrid').trigger('reloadGrid');
}

function getDataSearch() {
    var params = new Object();
    params.Busqueda = busqueda;    
    var respuesta = "";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "Bandeja_Orden.aspx/GetBandejaData",
        data: JSON.stringify(params),
        dataType: "json",
        async: false,
        success: function (data, textStatus) {
            if (textStatus == "success") {                
                respuesta = data;
            }
        },
        error: function (request, status, error) {
            mensajeErrorDialog(jQuery.parseJSON(request.responseText).Message);
            //alert(jQuery.parseJSON(request.responseText).Message);
        }
    });
    return respuesta;
}

function mensajeErrorDialog(mensajeDeError) {
    var sessionExpirada = "Session de usuario expirada";
    if (mensajeDeError == "Subproceso anulado.") {
        mensajeDeError = sessionExpirada;
    }
    if (mensajeDeError == sessionExpirada) {
        alertify.alert()
           .setting({
               'label': 'Aceptar',
               'message': mensajeDeError,
               'onok': function () { window.location.href = "Login.aspx"; }
           }).show().setHeader('<em> Sin session </em> ');
    } else {
        alertify.alert()
          .setting({
              'label': 'Aceptar',
              'message': mensajeDeError,
              'onok': function () { }
          }).show().setHeader('<em> Ocurrio un error </em> ');
    }
}