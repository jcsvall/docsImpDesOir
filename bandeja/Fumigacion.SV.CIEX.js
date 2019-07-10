var accionBandeja = false;
var varPto;
var varOrd;
$(document).ready(function () {
    procesar();
    test();
});

function procesar() {
    varPto = getVariableUrl("pto");
    varOrd = getVariableUrl("ord");
    tipoProceso();
    if (accionBandeja) {
        cargarGridServiciosCIEX();
    }
}

function cargarGridServiciosCIEX() {
    
    jQuery("#dataGrid").jqGrid({
        url: 'Cert_Fumigacion_TerrestreJ.aspx/GetServiciosCIEX',
        datatype: "json",
        mtype: 'POST',
        postData: { Pto: varPto, Ord: varOrd },
        height: 'auto',
        width: '690',
        serializeGridData: function (postData) {
                        return JSON.stringify(postData);
        },
        ajaxGridOptions: { contentType: "application/json" },
        loadonce: true,
        colNames: ['Cantidad', 'Quimico', 'Dosis', 'Tiempo', 'Origen', 'Destino', 'Procedencia', 'Producto', 'ListaProductosOrigen', 'Estado'],
        colModel: [
            { name: 'Cantidad', index: 'Cantidad', width: 70 },
            { name: 'QuimicoOirsaDescripcion', index: 'QuimicoOirsaDescripcion', width: 150 },
            { name: 'DosisOirsaDescripcion', index: 'DosisOirsaDescripcion', width: 150 },
            { name: 'Tiempo', index: 'Tiempo', width: 80, align: "center" },
            { name: 'Origen', index: 'Origen', width: 70, align: "center" },
            { name: 'Destino', index: 'Destino', width: 70, align: "center" },
            { name: 'Procedencia', index: 'Procedencia', width: 90, align: "center" },
            { name: 'producto', index: 'producto', width: 75, align: "center" },
            { name: 'ListaProductosOrigen', index: 'ListaProductosOrigen', width: 200, align: "center" },
            { name: 'Estado', index: 'Estado', width: 90, align: "center" }
        ],
        rowNum: 5,
        rowList: [10, 20, 30],
        pager: '#pagingGrid',
        sortname: 'Cantidad',
        viewrecords: true,
        sortorder: "desc",
        shrinkToFit: false,
        caption: "Servicios CIEX"
    });
    jQuery("#dataGrid").jqGrid('navGrid', '#pagingGrid', {search:false, edit: false, add: false, del: false });
}

function tipoProceso() {
    var params = new Object();
    params.Pto = varPto;
    params.Ord = varOrd;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "Cert_Fumigacion_TerrestreJ.aspx/AccionBandejaCIEX",
        data: JSON.stringify(params),
        dataType: "json",
        async: false,
        
        success: function (data, textStatus) {
            if (textStatus == "success") {
                if (data == "ProcesoBandeja") {                    
                    accionBandeja = true;
                }
            }
        },
        error: function (request, status, error) {
            alert(jQuery.parseJSON(request.responseText).Message);
        }
    });
}

function getVariableUrl(busqueda) {
    var query = window.location.search.substring(1);    
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var variableNombre = vars[i].substring(0, 3);
        if (variableNombre == busqueda) {
            var variableEncontrada = vars[i];
            return variableEncontrada.substring(4, variableEncontrada.length);
        }
    }
    return false;
}

function test() {
    var params = new Object();
    //params.Pto = varPto;
    //params.Ord = varOrd;
    var obj = { Encabezado: {Nombre:'Juan'} }
    //$.toJSON(parameters)
    var Objeto = { Arr: obj };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "Cert_Fumigacion_TerrestreJ.aspx/Prueba",
        data: JSON.stringify(Objeto),
        dataType: "json",
        async: false,
        cache: false,
        success: function (data, textStatus) {
            if (textStatus == "success") {
                if (data == "ProcesoBandeja") {
                    accionBandeja = true;
                }
            }
        },
        error: function (request, status, error) {
            alert(jQuery.parseJSON(request.responseText).Message);
        }
    });
}