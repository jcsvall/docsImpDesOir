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
        $("#lsFormasPago").html("<option value=0>Pago CIEX</option>");
        $("#ctl00_SampleContent_tb_numero_orden").attr('readonly', true);
        llenarCertificadoMAG();
    } else {
        $("#NoOrden").remove();
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
            { name: 'producto', index: 'producto', width: 75, align: "center" },
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
            Norden: $("#ctl00_SampleContent_tb_numero_orden").val(),
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

function onRowSelect(ids) {
    if (ids != null) {
        var costoLocal = $("#ctl00_SampleContent_tb_costo_local").val();
        var obj = jQuery("#dataGrid").jqGrid('getRowData', ids);

        $("#ctl00_SampleContent_tb_cantidad").val(obj.Cantidad);
        $("#ctl00_SampleContent_tb_subtotal").val(obj.Cantidad * costoLocal);

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

function buscarYsetearSelects(selectList, valorBusqueda) {
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

function quitarAcento(str) {
    var map = { 'À': 'A', 'Á': 'A', 'Â': 'A', 'Ã': 'A', 'Ä': 'A', 'Å': 'A', 'Æ': 'AE', 'Ç': 'C', 'È': 'E', 'É': 'E', 'Ê': 'E', 'Ë': 'E', 'Ì': 'I', 'Í': 'I', 'Î': 'I', 'Ï': 'I', 'Ð': 'D', 'Ñ': 'N', 'Ò': 'O', 'Ó': 'O', 'Ô': 'O', 'Õ': 'O', 'Ö': 'O', 'Ø': 'O', 'Ù': 'U', 'Ú': 'U', 'Û': 'U', 'Ü': 'U', 'Ý': 'Y', 'ß': 's', 'à': 'a', 'á': 'a', 'â': 'a', 'ã': 'a', 'ä': 'a', 'å': 'a', 'æ': 'ae', 'ç': 'c', 'è': 'e', 'é': 'e', 'ê': 'e', 'ë': 'e', 'ì': 'i', 'í': 'i', 'î': 'i', 'ï': 'i', 'ñ': 'n', 'ò': 'o', 'ó': 'o', 'ô': 'o', 'õ': 'o', 'ö': 'o', 'ø': 'o', 'ù': 'u', 'ú': 'u', 'û': 'u', 'ü': 'u', 'ý': 'y', 'ÿ': 'y', 'Ā': 'A', 'ā': 'a', 'Ă': 'A', 'ă': 'a', 'Ą': 'A', 'ą': 'a', 'Ć': 'C', 'ć': 'c', 'Ĉ': 'C', 'ĉ': 'c', 'Ċ': 'C', 'ċ': 'c', 'Č': 'C', 'č': 'c', 'Ď': 'D', 'ď': 'd', 'Đ': 'D', 'đ': 'd', 'Ē': 'E', 'ē': 'e', 'Ĕ': 'E', 'ĕ': 'e', 'Ė': 'E', 'ė': 'e', 'Ę': 'E', 'ę': 'e', 'Ě': 'E', 'ě': 'e', 'Ĝ': 'G', 'ĝ': 'g', 'Ğ': 'G', 'ğ': 'g', 'Ġ': 'G', 'ġ': 'g', 'Ģ': 'G', 'ģ': 'g', 'Ĥ': 'H', 'ĥ': 'h', 'Ħ': 'H', 'ħ': 'h', 'Ĩ': 'I', 'ĩ': 'i', 'Ī': 'I', 'ī': 'i', 'Ĭ': 'I', 'ĭ': 'i', 'Į': 'I', 'į': 'i', 'İ': 'I', 'ı': 'i', 'Ĳ': 'IJ', 'ĳ': 'ij', 'Ĵ': 'J', 'ĵ': 'j', 'Ķ': 'K', 'ķ': 'k', 'Ĺ': 'L', 'ĺ': 'l', 'Ļ': 'L', 'ļ': 'l', 'Ľ': 'L', 'ľ': 'l', 'Ŀ': 'L', 'ŀ': 'l', 'Ł': 'L', 'ł': 'l', 'Ń': 'N', 'ń': 'n', 'Ņ': 'N', 'ņ': 'n', 'Ň': 'N', 'ň': 'n', 'ŉ': 'n', 'Ō': 'O', 'ō': 'o', 'Ŏ': 'O', 'ŏ': 'o', 'Ő': 'O', 'ő': 'o', 'Œ': 'OE', 'œ': 'oe', 'Ŕ': 'R', 'ŕ': 'r', 'Ŗ': 'R', 'ŗ': 'r', 'Ř': 'R', 'ř': 'r', 'Ś': 'S', 'ś': 's', 'Ŝ': 'S', 'ŝ': 's', 'Ş': 'S', 'ş': 's', 'Š': 'S', 'š': 's', 'Ţ': 'T', 'ţ': 't', 'Ť': 'T', 'ť': 't', 'Ŧ': 'T', 'ŧ': 't', 'Ũ': 'U', 'ũ': 'u', 'Ū': 'U', 'ū': 'u', 'Ŭ': 'U', 'ŭ': 'u', 'Ů': 'U', 'ů': 'u', 'Ű': 'U', 'ű': 'u', 'Ų': 'U', 'ų': 'u', 'Ŵ': 'W', 'ŵ': 'w', 'Ŷ': 'Y', 'ŷ': 'y', 'Ÿ': 'Y', 'Ź': 'Z', 'ź': 'z', 'Ż': 'Z', 'ż': 'z', 'Ž': 'Z', 'ž': 'z', 'ſ': 's', 'ƒ': 'f', 'Ơ': 'O', 'ơ': 'o', 'Ư': 'U', 'ư': 'u', 'Ǎ': 'A', 'ǎ': 'a', 'Ǐ': 'I', 'ǐ': 'i', 'Ǒ': 'O', 'ǒ': 'o', 'Ǔ': 'U', 'ǔ': 'u', 'Ǖ': 'U', 'ǖ': 'u', 'Ǘ': 'U', 'ǘ': 'u', 'Ǚ': 'U', 'ǚ': 'u', 'Ǜ': 'U', 'ǜ': 'u', 'Ǻ': 'A', 'ǻ': 'a', 'Ǽ': 'AE', 'ǽ': 'ae', 'Ǿ': 'O', 'ǿ': 'o' };
    var res = '';
    for (var i = 0; i < str.length; i++) {
        c = str.charAt(i);
        res += map[c] || c;
    }
    return res;
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