var accionBandeja = false;
var reDirect = false;
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
        $("#btGuardarCIEX").show();
        $("#btGuardar").hide();
    }
    if (reDirect) {
        redirectBandejaCIEX();
    }
    $("#btGuardarCIEX").click(function () {
        //guardarCIEX();
        validaciones();
    });
}

function redirectBandejaCIEX() {
    window.location.href = "Bandeja_Orden.aspx";
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

function guardarCIEX() {
    var dDet = jQuery("#jqAtomizacion").jqGrid('getDataIDs');
    var obj = {
        Enca: {
            Puesto: "Envio de construccion de objeto",
            Fecha: $("#ctl00_SampleContent_tb_fecha").val(),
            Cambio: $.ConsultarQueryS("Cert_atomizacionJ.aspx/ObtenerValIni", { Val: "Cambio" }),
            Cortesia: false,
            Local: $("#ctl00_SampleContent_cb_local")[0].checked,
            Total: 0.0,
            Totalstring: $("#ctl00_SampleContent_l_numlet").html(),
            Observacion: $("#ctl00_SampleContent_tb_observaciones").val() + " " + $("#ctl00_SampleContent_tb_humedad_relativa").val(),
            Responsable: $("#ctl00_SampleContent_ddl_cuarentena").val(),
            Anulado: false,
            Remesado: false,
            Replicado: false,
            Nend: true,
            Tipocertificado: "FUMIGACIÓN TERRESTRE",
            Tipocliente: $("#ctl00_SampleContent_rb_tipo_cliente_i")[0].checked == true ? "I" : "E",
            Clienteextra: $("#ctl00_SampleContent_tb_cliente").val(),
            Cliente: $("#ctl00_SampleContent_ddl_cliente").val(),
            Vapor: $("#ctl00_SampleContent_tb_nvapor").val(),
            Naduana: $("#ctl00_SampleContent_ddl_motivo").val(),
            Placa: $("#ctl00_SampleContent_tb_nplaca").val(),
            Impuesto: 0.0,
            Idccaja: 0,
            Adeldia: false,
            Enviaplat: false,
            Fechatrat: $("#ctl00_SampleContent_tb_fecha_tratamiento").val() + " " + $("#ctl00_SampleContent_tb_hora_ini").val(),
            FechatratFin: $("#ctl00_SampleContent_tb_fecha_tratamiento_fin").val() + " " + $("#ctl00_SampleContent_tb_hora_fin").val(),
            Credito: $("#lsFormasPago").val() == 1 ? true : false,
            Norden: $("#ctl00_SampleContent_tb_numero_orden").val(),
            Forden: $("#ctl00_SampleContent_tb_fecha_orden").val(),
            Aorden: "",/*$("#ctl00_SampleContent_").val()*/
            Cuarentena: $("#ctl00_SampleContent_ddl_cuarentena").val(),
            Idfactura: "",
            Nviaje: $("#ctl00_SampleContent_tb_nviaje").val(),
            Fechaatraque: $("#ctl00_SampleContent_tb_fecha_atraque").val() + " " + $("#ctl00_SampleContent_tb_hora_atraque").val(),
            idpais: "",
            Ingles: false,
            NombreCliente: $("#ctl00_SampleContent_ddl_cliente :selected").text(),
            FormaPago: $("#lsFormasPago").val(),
            NordenCIEX: varOrd
        },
        Detalle: []
    }
    for (var i = 0; i < dDet.length; i++) {
        var rowId = dDet[i];
        var detObject = jQuery("#jqAtomizacion").jqGrid('getRowData', rowId);
        detObject.MensajeJC = "HOLA";
        detObject.Dosis = String(detObject.Dosis) == "" ? 0 : String(detObject.Dosis);
        detObject.IdDosis = String(detObject.IdDosis).split('-')[0]; //Ud: String(dDet[i].IdDosis).split('-')[0]
        detObject.IdServicio = String(detObject.IdServicio).split('-')[0];
        detObject.Real = detObject.Real == "" ? 0 : detObject.Real;
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
        detObject.Teorico = detObject.Teorico == "" ? 0 : detObject.Teorico;
        detObject.Densidad = detObject.Densidad == "" ? 0 : detObject.Densidad;
        detObject.Contenedor = detObject.Contenedor == "" ? 0 : detObject.Contenedor;
        detObject.Concentracion = detObject.Concentracion == "" ? 0 : detObject.Concentracion;
        detObject.Temperatura = detObject.Temperatura == "" ? 0 : detObject.Temperatura;
        detObject.Tiempoaereacion = detObject.Tiempoaereacion == "" ? 0 : detObject.Tiempoaereacion;
        detObject.IdPais = "";
        if ($("#ctl00_SampleContent_tb_recargo").val() > 0) {
            if (dDet.length - 1 == obj.Detalle.length) {
                detObject.Subtotal = (detObject.Subtotal * 1) + ($("#ctl00_SampleContent_tb_recargo").val() * 1);
                //detObject.Subtotal = Fumigacion.RedDondeoT(det.Subtotal);
            }
        }
        obj.Detalle.push(detObject);
    }

    if ($("#ctl00_SampleContent_hdTipo").val() == "M") {
        obj.Enca.Vapor = $("#ctl00_SampleContent_tb_nvapor").val();
        obj.Enca.Fechaatraque = $("#ctl00_SampleContent_tb_fecha_atraque").val() + " " + $("#ctl00_SampleContent_tb_hora_atraque").val();
        obj.Enca.Tipocertificado = "FUMIGACIÓN MARITIMA";
        obj.Enca.Nviaje = $("#ctl00_SampleContent_tb_nviaje").val();
    }
     
    var Objeto = { Arr: obj };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "Cert_Fumigacion_TerrestreJ.aspx/GuardarCIEX",
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
    var correcto = true;
    var nOrden = $("#ctl00_SampleContent_tb_numero_orden").val();
    if (nOrden.trim().length < 1) {
        correcto = false;
        alert("Ingrese número de Orden");
    }
    
    if (correcto) {
        guardarCIEX();
    }

}