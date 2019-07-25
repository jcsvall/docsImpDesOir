var busqueda = "";
var estado = "";
var baseURL = "Bandeja_CIEX.aspx";
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
    llenarEstados();
}

function llenarEstados() {
    var cadena = "<option value='' selected>[Estados]</option>";
    for (var i = 1; i < 4; i++) {
        var estado = "Pagado";
        if (i == 2) {
            estado = "Pendiente";
        }
        if (i == 3) {
            estado = "Procesado";
        }
        cadena += "<option value='" + estado + "'>" + estado + "</option>";
    }
    $("#estados").html(cadena);
}

function cargarGrid() {
    jQuery("#dataGrid").jqGrid({
        url: baseURL+'/GetBandejaData',
        datatype: "json",
        mtype: 'POST',
        postData: { Busqueda: busqueda, Estado: estado },
        height: 'auto',
        width: '690',
        loadError: function (jqXHR, textStatus, errorThrown) {
            alert('HTTP status code: ' + jqXHR.status + '\n' +
                  'textStatus: ' + textStatus + '\n' +
                  'errorThrown: ' + errorThrown);
            alert('HTTP message body (jqXHR.responseText): ' + '\n' + jqXHR.responseText);
        },
        serializeGridData: function (postData) {
            return JSON.stringify(postData);
        },
        ajaxGridOptions: { contentType: "application/json" },       
        colNames: ['Fecha', 'No. Orden', 'Tipo Tratamiento', 'Cliente', 'Responsable MAG', 'Placa/Vapor', 'Valor','Estado', 'Fecha-Hora Pago','Comprobante','Id', 'Acci&oacute;n'],
        colModel: [            
            { name: 'Fecha', index: 'Fecha', width: 130, sorttype: "date", formatter: "date", formatoptions: { srcformat: "ISO8601Long", newformat: "d/m/Y h:i A" } },
            { name: 'NOrdenCiex', index: 'NOrdenCiex', width: 120 },
            { name: 'TipoCertificado', index: 'TipoCertificado', width: 160 },
            { name: 'cliente', index: 'cliente', width: 170, align: "center" },
            { name: 'responsableMag', index: 'responsableMag', width: 120, align: "center" },
            { name: 'PlacaVapor', index: 'PlacaVapor', width: 80, align: "center" },
            { name: 'Total', index: 'Total', width: 90, align: "center", formatter: 'currency', formatoptions: { prefix: '($', suffix: ')', thousandsSeparator: ',' } },
            { name: 'Estado', index: 'Estado', width: 90, align: "center" },
            { name: 'fechaHoraPago', index: 'fechaHoraPago', width: 130, sorttype: "date", formatter: "date", formatoptions: { srcformat: "ISO8601Long", newformat: "d/m/Y h:i A" } },
            { name: 'NCompranteCiex', index: 'NCompranteCiex', width: 90, align: "center" },
            { name: 'id', index: 'id', width: 80, hidden: true },
            { name: 'act', index: 'act', width: 70, sortable: false, align: "center" },
        ],
        rowNum: 10,
        rowList: [10, 20, 30],
        pager: '#pagingGrid',
        sortname: 'Fecha',
        viewrecords: true,
        sortorder: "desc",
        shrinkToFit: false,        
        gridComplete: function(){
            var ids = jQuery("#dataGrid").jqGrid('getDataIDs');
            for(var i=0;i < ids.length;i++){
                var cl = ids[i];                                
                var rowSelected = jQuery("#dataGrid").jqGrid('getRowData', cl);                
                if (rowSelected.Estado.trim().toUpperCase() == 'PAGADO') {
                    //be = "<button type='button' class='btn btn-primary btn-sm' onClick='generarCertificado(" + JSON.stringify(rowSelected) + ")'><span class='oi oi-file' style='font-size:90%' title='Generar Certificado'></span></button> ";
                    //jQuery("#dataGrid").jqGrid('setRowData', ids[i], { act: be });
                    be = "<button type='button' class='btn btn-primary btn-sm' onClick='confirmDialog(" + JSON.stringify(rowSelected) + ")'><span class='oi oi-file' style='font-size:90%' title='Generar Certificado'></span></button> ";
                }else{
                    be = "<button type='button' class='btn btn-primary btn-sm' disabled><span class='oi oi-file' style='font-size:90%' title='No valido para esta opci&oacute;n'></span></button> ";
                }
                jQuery("#dataGrid").jqGrid('setRowData', ids[i], { act: be });
            }	
        },
        caption: "Bandeja de ordenes CIEX"
    });
    jQuery("#dataGrid").jqGrid('navGrid', '#pagingGrid', { search: false, edit: false, add: false, del: false });
}


function generarCertificado(objeto) {    
    var NOrden = objeto.NOrdenCiex;
    //alert(NOrden + " Id: " + objeto.id + " Valor: " + objeto.Total);
    var params = new Object();
    params.IdPagoOrdenCIEX = objeto.id;
    //var respuesta = "";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: baseURL + "/GenerarCertificado",
        data: JSON.stringify(params),
        dataType: "json",
        async: false,
        success: function (data, textStatus) {
            if (textStatus == "success") {
                alert(data);
                alertify.success('Certificado generado correctamente');
            }
        },
        error: function (request, status, error) {
            mensajeErrorDialog(jQuery.parseJSON(request.responseText).Message);            
        }
    });
}

function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, 'g'), replace);
}

function mensajeDeSalidaPagina() {
    $.blockUI({ message: "Cargando..." });
}

function buscarEnGrid() {
    busqueda = $("#busquedaValue").val();
    estado = $("#estados").val();

    jQuery('#dataGrid').jqGrid('clearGridData');
    jQuery('#dataGrid').jqGrid('setGridParam', { postData: { Busqueda: busqueda, Estado: estado } });
    jQuery('#dataGrid').trigger('reloadGrid');
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

function confirmDialog(objeto) {
    alertify.confirm('Confirmaci&oacute;n', '<div style="font-weight:bold">Se generar&aacute; certificado de tipo ' + objeto.TipoCertificado + '</div><div style="color:red;font-weight:bold">¿Desea continuar?</div>',
        function () {
            generarCertificado(objeto);
            //alertify.success('Ok');
        },
        function () {
           // alertify.error('Cancel')
        }).set('labels', { ok: 'Aceptar', cancel: 'Cancelar' });
}