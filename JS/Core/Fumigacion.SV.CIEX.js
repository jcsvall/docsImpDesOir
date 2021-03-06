﻿var accionBandeja = false;
var reDirect = false;
var varPto;
var varOrd;
var webMethodBase = "Bandeja_Orden.aspx";
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
        bloquearMostrarOcultar();
        llenarCertificadoMAG();
    }
    if (reDirect) {
        redirectBandejaCIEX();
    }
    $("#btGuardarCIEX").click(function () {
        //guardarCIEX();
        validaciones();
    });
}

function bloquearMostrarOcultar() {
    $("#btGuardarCIEX").show();
    $("#btGuardar").hide();
    $("#ctl00_SampleContent_tb_numero_orden").attr('readonly', true);
    $("#lsFormasPago").html("<option value=0>Pago CIEX</option>");    
    //$("#ctl00_SampleContent_n_certificado").remove();
}

function redirectBandejaCIEX() {
    window.location.href = "Bandeja_Orden.aspx";
}

function cargarGridServiciosCIEX() {
    
    jQuery("#dataGrid").jqGrid({
        url: webMethodBase+'/GetServiciosCIEX',
        datatype: "json",
        mtype: 'POST',
        postData: { Pto: varPto, Ord: varOrd },
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
        loadonce: true,
        colNames: ['Cantidad', 'Quimico', 'Dosis', 'Tiempo', 'Origen', 'Destino', 'Procedencia', 'Producto', 'ListaProductosOrigen'],
        colModel: [
            { name: 'Cantidad', index: 'Cantidad', width: 70 },
            { name: 'QuimicoOirsaDescripcion', index: 'QuimicoOirsaDescripcion', width: 150 },
            { name: 'DosisOirsaDescripcion', index: 'DosisOirsaDescripcion', width: 150 },
            { name: 'Tiempo', index: 'Tiempo', width: 80, align: "center" },
            { name: 'Origen', index: 'Origen', width: 70, align: "center" },
            { name: 'Destino', index: 'Destino', width: 70, align: "center" },
            { name: 'Procedencia', index: 'Procedencia', width: 90, align: "center" },
            { name: 'producto', index: 'producto', width: 75, align: "center", hidden: true },
            { name: 'ListaProductosOrigen', index: 'ListaProductosOrigen', width: 200, align: "center" }
        ],
        rowNum: 5,
        rowList: [10, 20, 30],
        pager: '#pagingGrid',
        sortname: 'Cantidad',
        viewrecords: true,
        sortorder: "desc",
        shrinkToFit: false,
        caption: "Servicios CIEX",
        onSelectRow: function (ids) {
            onRowSelect(ids);
        }
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
        url: webMethodBase+"/AccionBandejaCIEX",
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
        url: webMethodBase+"/GuardarCIEX",
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

function llenarCertificadoMAG() {
    var params = new Object();
    params.Ord = varOrd;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: webMethodBase + "/ObtenerOrdenMAG",
        data: JSON.stringify(params),
        dataType: "json",
        async: false,

        success: function (data, textStatus) {
            if (textStatus == "success") {
                $("#ctl00_SampleContent_tb_numero_orden").val(data);
            }
        },
        error: function (request, status, error) {
            alert(jQuery.parseJSON(request.responseText).Message);
        }
    });
}

function onRowSelect(ids) {
    if (ids != null) {
        var costoLocal = $("#ctl00_SampleContent_tb_costo_local").val();
        var obj = jQuery("#dataGrid").jqGrid('getRowData', ids);

        $("#ctl00_SampleContent_tb_cantidad").val(obj.Cantidad);
        $("#ctl00_SampleContent_tb_subtotal").val(obj.Cantidad * costoLocal);
       // alert(obj.QuimicoOirsaDescripcion);
      //  $("#ctl00_SampleContent_ddl_plaguicida option:contains(" + obj.QuimicoOirsaDescripcion + ")").attr('selected', true);
        
        var selectList = document.getElementById("ctl00_SampleContent_ddl_plaguicida");
        buscarYsetearSelects(selectList, obj.QuimicoOirsaDescripcion);
        obtenerDosis();
        var selectListDosis = document.getElementById("ctl00_SampleContent_ddl_unidad_dosis");
        buscarYsetearSelects(selectListDosis, obj.DosisOirsaDescripcion);
        obtenerValorDosis();

        var tiempoUniadTiempo = obj.Tiempo.split(" ");
        var tiempo = tiempoUniadTiempo[0];
        var unidadTiempo = tiempoUniadTiempo[1];
        $("#ctl00_SampleContent_tb_tiempo_exposicion").val(tiempo);
        var selectListUnidadTiempo = document.getElementById("ctl00_SampleContent_ddl_unidad_tiempo");
        buscarYsetearSelects(selectListUnidadTiempo, unidadTiempo);

        var selectListOrigen = document.getElementById("ctl00_SampleContent_ddl_origen");
        buscarYsetearSelects(selectListOrigen, obj.Origen);

        var selectListProcedencia = document.getElementById("ctl00_SampleContent_ddl_procedencia");
        buscarYsetearSelects(selectListProcedencia, obj.Procedencia);

        var selectListDestino = document.getElementById("ctl00_SampleContent_ddl_destino");
        buscarYsetearSelects(selectListDestino, obj.Destino);

    } else {
        
    }
}

function buscarYsetearSelects(selectList,valorBusqueda) {
    for (var i = 0; i < selectList.length; i++) {
        
        var valorSelect = quitarAcento(selectList.options[i].text);
        var busqueda = quitarAcento(valorBusqueda);
        console.log(selectList.options[i].text + " == " + valorSelect);
        if (valorSelect.trim().toUpperCase().includes(busqueda.trim().toUpperCase())) {
            selectList.selectedIndex = i;            
            break;
        }
    }
}
function obtenerDosis() {
    var Params = {
        Servicion: $("#ctl00_SampleContent_ddl_servicio").val(),
        Plaguicida: $("#ctl00_SampleContent_ddl_plaguicida").val()
    }
    var rt = $.ConsultarQueryS("Cert_atomizacionJ.aspx/ObtenerDosis", Params, false);
    var rtj = $.parseJSON(rt);
    $.SelectListFromJson("ctl00_SampleContent_ddl_unidad_dosis", rtj, "Dosis", "Descripcion", { store: false, emptyOption: false });
    var dosis = $("#ctl00_SampleContent_ddl_unidad_dosis").val();
    if ($.trim(dosis) != "") {
        var arDosis = String(dosis).split('-');
        $("#ctl00_SampleContent_tb_dosis").val(arDosis[1]);
    } else {
        $("#ctl00_SampleContent_tb_dosis").val("");
    }
}
function obtenerValorDosis() {
    var docis = $("#ctl00_SampleContent_ddl_unidad_dosis").val();
    if ($.trim(docis) != "") {
        var arrd = String(docis).split('-');
        $("#ctl00_SampleContent_tb_dosis").val(arrd[1]);
    } else {
        $("#ctl00_SampleContent_tb_dosis").val("");
    }
}

function quitarAcento(str) {
    var map = { 'À': 'A', 'Á': 'A', 'Â': 'A', 'Ã': 'A', 'Ä': 'A', 'Å': 'A', 'Æ': 'AE', 'Ç': 'C', 'È': 'E', 'É': 'E', 'Ê': 'E', 'Ë': 'E', 'Ì': 'I', 'Í': 'I', 'Î': 'I', 'Ï': 'I', 'Ð': 'D', 'Ñ': 'N', 'Ò': 'O', 'Ó': 'O', 'Ô': 'O', 'Õ': 'O', 'Ö': 'O', 'Ø': 'O', 'Ù': 'U', 'Ú': 'U', 'Û': 'U', 'Ü': 'U', 'Ý': 'Y', 'ß': 's', 'à': 'a', 'á': 'a', 'â': 'a', 'ã': 'a', 'ä': 'a', 'å': 'a', 'æ': 'ae', 'ç': 'c', 'è': 'e', 'é': 'e', 'ê': 'e', 'ë': 'e', 'ì': 'i', 'í': 'i', 'î': 'i', 'ï': 'i', 'ñ': 'n', 'ò': 'o', 'ó': 'o', 'ô': 'o', 'õ': 'o', 'ö': 'o', 'ø': 'o', 'ù': 'u', 'ú': 'u', 'û': 'u', 'ü': 'u', 'ý': 'y', 'ÿ': 'y', 'Ā': 'A', 'ā': 'a', 'Ă': 'A', 'ă': 'a', 'Ą': 'A', 'ą': 'a', 'Ć': 'C', 'ć': 'c', 'Ĉ': 'C', 'ĉ': 'c', 'Ċ': 'C', 'ċ': 'c', 'Č': 'C', 'č': 'c', 'Ď': 'D', 'ď': 'd', 'Đ': 'D', 'đ': 'd', 'Ē': 'E', 'ē': 'e', 'Ĕ': 'E', 'ĕ': 'e', 'Ė': 'E', 'ė': 'e', 'Ę': 'E', 'ę': 'e', 'Ě': 'E', 'ě': 'e', 'Ĝ': 'G', 'ĝ': 'g', 'Ğ': 'G', 'ğ': 'g', 'Ġ': 'G', 'ġ': 'g', 'Ģ': 'G', 'ģ': 'g', 'Ĥ': 'H', 'ĥ': 'h', 'Ħ': 'H', 'ħ': 'h', 'Ĩ': 'I', 'ĩ': 'i', 'Ī': 'I', 'ī': 'i', 'Ĭ': 'I', 'ĭ': 'i', 'Į': 'I', 'į': 'i', 'İ': 'I', 'ı': 'i', 'Ĳ': 'IJ', 'ĳ': 'ij', 'Ĵ': 'J', 'ĵ': 'j', 'Ķ': 'K', 'ķ': 'k', 'Ĺ': 'L', 'ĺ': 'l', 'Ļ': 'L', 'ļ': 'l', 'Ľ': 'L', 'ľ': 'l', 'Ŀ': 'L', 'ŀ': 'l', 'Ł': 'L', 'ł': 'l', 'Ń': 'N', 'ń': 'n', 'Ņ': 'N', 'ņ': 'n', 'Ň': 'N', 'ň': 'n', 'ŉ': 'n', 'Ō': 'O', 'ō': 'o', 'Ŏ': 'O', 'ŏ': 'o', 'Ő': 'O', 'ő': 'o', 'Œ': 'OE', 'œ': 'oe', 'Ŕ': 'R', 'ŕ': 'r', 'Ŗ': 'R', 'ŗ': 'r', 'Ř': 'R', 'ř': 'r', 'Ś': 'S', 'ś': 's', 'Ŝ': 'S', 'ŝ': 's', 'Ş': 'S', 'ş': 's', 'Š': 'S', 'š': 's', 'Ţ': 'T', 'ţ': 't', 'Ť': 'T', 'ť': 't', 'Ŧ': 'T', 'ŧ': 't', 'Ũ': 'U', 'ũ': 'u', 'Ū': 'U', 'ū': 'u', 'Ŭ': 'U', 'ŭ': 'u', 'Ů': 'U', 'ů': 'u', 'Ű': 'U', 'ű': 'u', 'Ų': 'U', 'ų': 'u', 'Ŵ': 'W', 'ŵ': 'w', 'Ŷ': 'Y', 'ŷ': 'y', 'Ÿ': 'Y', 'Ź': 'Z', 'ź': 'z', 'Ż': 'Z', 'ż': 'z', 'Ž': 'Z', 'ž': 'z', 'ſ': 's', 'ƒ': 'f', 'Ơ': 'O', 'ơ': 'o', 'Ư': 'U', 'ư': 'u', 'Ǎ': 'A', 'ǎ': 'a', 'Ǐ': 'I', 'ǐ': 'i', 'Ǒ': 'O', 'ǒ': 'o', 'Ǔ': 'U', 'ǔ': 'u', 'Ǖ': 'U', 'ǖ': 'u', 'Ǘ': 'U', 'ǘ': 'u', 'Ǚ': 'U', 'ǚ': 'u', 'Ǜ': 'U', 'ǜ': 'u', 'Ǻ': 'A', 'ǻ': 'a', 'Ǽ': 'AE', 'ǽ': 'ae', 'Ǿ': 'O', 'ǿ': 'o' };
    var res = '';
    for (var i = 0; i < str.length; i++) {
        c = str.charAt(i);
        res += map[c] || c;
    }
    return res;
}
