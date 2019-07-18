var accionBandeja = false;
var reDirect = false;
var varPto;
var varOrd;
var webMethodBase = "Bandeja_Orden.aspx";
$(document).ready(function () {
    procesar();
});

function procesar() {
    varPto = getVariableUrl("pto");
    varOrd = getVariableUrl("ord");
    varPto = replaceAll(varPto, "_", "/");
    varOrd = replaceAll(varOrd, "_", "/");
    tipoProceso();
    if (accionBandeja) {
        cargarGridServiciosCIEX();
        $("#btGuardarCIEX").show();
        $("#btGuardar").hide();
    }
    if (reDirect) {
        redirectBandejaCIEX();
    }
    $("#btGuardarCIEX").click(function () {        
        validaciones();
    });
}

function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, 'g'), replace);
}

function redirectBandejaCIEX() {
    window.location.href = "Bandeja_Orden.aspx";
}

function cargarGridServiciosCIEX() {
    
    jQuery("#dataGrid").jqGrid({
        url: webMethodBase + '/GetServiciosCIEX',
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
        url: webMethodBase + "/AccionBandejaCIEX",
        data: JSON.stringify(params),
        dataType: "json",
        async: false,
        
        success: function (data, textStatus) {
            if (textStatus == "success") {
                if (data == "ProcesoBandeja") {                    
                    accionBandeja = true;
                }
                if (data == "HacerRedirect") {
                    reDirect = true;                    
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

function guardarCIEX() {
    var obj = getObjetoServices("jqAtomizacion");     
    var Objeto = { Arr: obj };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: webMethodBase + "/GuardarCIEX",
        data: JSON.stringify(Objeto),
        dataType: "json",
        async: false,
        cache: false,
        success: function (data, textStatus) {
            if (textStatus == "success") {
                alert("Orden de Pago CIEX creada satisfactoriamente");
                redirectBandejaCIEX();
            }
        },
        error: function (request, status, error) {            
            alert(jQuery.parseJSON(request.responseText).Message);
        }
    });

}

function validaciones() {
    //var correcto = false;
        
    //if (correcto) {
        guardarCIEX();
    //}
    //pruebaObjeto();
}

function pruebaObjeto() {
    var obj = getObjetoServices("jqAtomizacion");
       
    var Objeto = { Arr: obj };

    console.log(JSON.stringify(Objeto));
    alert(JSON.stringify(Objeto));
    alert("varOrd: " + varOrd);
}

function getObjetoServices(gridNombre) {
    var idGrid = "#" + gridNombre;
    var dDet = jQuery(idGrid).jqGrid('getDataIDs');
    var obj = {
        Enca: {
            Puesto: "",
            Ncertificado: $("#ctl00_SampleContent_l_n_certificado").html(),
            Fecha: $("#ctl00_SampleContent_tb_fecha").val(),
            Cambio: $.ConsultarQueryS("Cert_atomizacionJ.aspx/ObtenerValIni", {
                Val: "Cambio"
            }),
            Cortesia: false,
            Local: $("#ctl00_SampleContent_cb_local")[0].checked,
            Total: 0.0,
            Totalstring: $("#ctl00_SampleContent_l_numlet").html(),
            Observacion: $("#ctl00_SampleContent_tb_observaciones").val(),
            Responsable: $("#ctl00_SampleContent_ddl_cuarentena").val(),
            Anulado: false,
            Remesado: false,
            Replicado: false,
            Nend: true,
            Tipocertificado: "NEBULIZACION TERRESTRE",
            Tipocliente: "",
            Clienteextra: $("#ctl00_SampleContent_tb_cliente").val(),
            Cliente: $("#ctl00_SampleContent_ddl_cliente").val(),
            Vapor: "",
            Naduana: "",
            Placa: $("#ctl00_SampleContent_tb_nplaca").val(),
            Impuesto: 0.0,
            Idccaja: 0,
            Adeldia: false,
            Enviaplat: false,
            Fechatrat: $("#ctl00_SampleContent_tb_fecha_tratamiento").val() + " " + $("#ctl00_SampleContent_tb_hora_ini").val(),
            FechatratFin: "",
            Credito: $("#lsFormasPago").val() == 1 ? true : false,
            Norden: "",
            Forden: "",
            Aorden: "",
            Cuarentena: $("#ctl00_SampleContent_ddl_cuarentena").val(),
            Idfactura: "",
            Nviaje: "",
            Fechaatraque: "",
            idpais: "",
            Ingles: false,
            NombreCliente: $("#ctl00_SampleContent_ddl_cliente :selected").text(),
            FormaPago: $("#lsFormasPago").val()
        },
        Detalle: []
    }
    for (var i = 0; i < dDet.length; i++) {
        var rowId = dDet[i];
        var detObject = jQuery(idGrid).jqGrid('getRowData', rowId);
        detObject.MensajeJC = "HOLA";
        detObject.Dosis = String(detObject.Dosis) == "" ? 0 : String(detObject.Dosis);
        detObject.IdDosis = String(detObject.IdDosis).split('-')[0]; //Ud: String(dDet[i].IdDosis).split('-')[0]
        detObject.IdServicio = String(detObject.IdServicio).split('-')[0];
        detObject.Real = detObject.ConsumoReal == "" ? 0 : detObject.ConsumoReal;
        detObject.Producto = detObject.Producto == "" ? 0 : detObject.Producto;
        detObject.TiempoExposicion = detObject.TiempoExposicion == "" ? 0 : detObject.TiempoExposicion;
        //Ut: dDet[i].UTiempoD
        detObject.Razon = "";
        detObject.Nacta = "";
        detObject.Db = false;
        detObject.Session = "";
        detObject.CantVol = detObject.CantVol == "" ? 0 : detObject.CantVol;
        detObject.Enviaplat = false;
        detObject.Cantidadcubicad = detObject.Cantidadcubicad == "" ? 0 : detObject.Cantidadcubicad;
        detObject.Teorico = 0.0;
        detObject.Densidad = "";
        detObject.Contenedor = "";
        detObject.Concentracion = "";
        detObject.Temperatura = 0.0;
        detObject.Tiempoaereacion = 0.0;
        detObject.IdPais = "";
        detObject.Silo = "";
        detObject.LugarTra = "";
        detObject.UtAereacion = "";
        if ($("#ctl00_SampleContent_tb_recargo").val() > 0) {
            if (dDet.length - 1 == obj.Detalle.length) {
                detObject.Subtotal = (detObject.Subtotal * 1) + ($("#ctl00_SampleContent_tb_recargo").val() * 1);
            }
        }
        obj.Detalle.push(detObject);
    }

    if ($("#ctl00_SampleContent_hdTipo").val() == "M") {
        obj.Enca.Tipocertificado = "NEBULIZACION MARITIMA";
        obj.Enca.Nviaje = $("#ctl00_SampleContent_tb_nviaje").val();
        obj.Enca.Vapor = $("#ctl00_SampleContent_tb_nvapor").val();
        obj.Enca.Fechaatraque = $("#ctl00_SampleContent_tb_fecha_atraque").val();
        obj.Enca.Placa = "";
    }
    obj.Enca.Tipocliente = $("#ctl00_SampleContent_rb_tipo_cliente_i")[0].checked == true ? 'I' : 'E';
    obj.Enca.Total = $("#ctl00_SampleContent_tb_total").val();

    return obj;
}