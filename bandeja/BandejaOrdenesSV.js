var page = 1;
var criterioDeBusquedaUsado = "";
var rangoPaginas = 5;
var totalRegistros = 0;
var totalPaginas = 0;
$(document).ready(function () {    
    getTotalesRegistro("", page);
    getData("", page);
    prepararBotones();
    ocultarMientrasPageCarga();
});

function getData(valorBusqueda, page) {
    criterioDeBusquedaUsado = valorBusqueda;    
    var params = new Object();
    params.busqueda = valorBusqueda;
    params.currentpage = page;
    
    var cadena = "";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "Bandeja_Ordenes.aspx/GetOrdenesList",
        data: JSON.stringify(params),
        dataType: "json",
        async: false,
        beforeSend: function () {            
            //mensajeDeEspera();            
        },
        success: function (data, textStatus) {
            if (textStatus == "success") {                
                jQuery.each(data, function (index, itemData) {

                    var objeto = new Object();
                    objeto.PuestoEncryp = itemData.PuestoEncryp;
                    objeto.NoOrdenEncryp = itemData.NoOrdenEncryp;
                    objeto.TipoTratamiento = itemData.TipoTratamiento;
                    
                    cadena += "<tr><td>" + itemData.Fecha + "</td><td>" + itemData.NoOrden + "</td><td>" + itemData.OperacionMag + "</td><td>" + itemData.TipoTratamiento + "</td><td>" + itemData.Puesto + "</td><td>" + itemData.Cliente + "</td>" + "</td><td>" + itemData.ResponsableMag + "</td>" + "</td><td>" + itemData.PlacaVapor + "</td>" + "</td><td>" + itemData.Producto + "</td>";
                    cadena += "<td><button type='button' class='btn btn-primary btn-sm' onClick='goTratamiento(" + JSON.stringify(objeto) + ")'><span class='oi oi-folder' style='font-size:75%' title='Ver'></span></button> </td></tr>";
                });                
                $("#tbody").html(cadena);
                pagineo();
            }
        },
        complete: function () {
            //quitarMensajeEspera();
        },
        error: function (request, status, error) {
            mensajeErrorDialog(jQuery.parseJSON(request.responseText).Message);
            //alert(jQuery.parseJSON(request.responseText).Message);
        }
    });

}

function pagineo() {
    
    var totalRegistroPorPagina = (page * 10);
    if (totalRegistroPorPagina > totalRegistros) {
        totalRegistroPorPagina = totalRegistros;
    }
    $("#caption").text((page * 10) - 9 + " - " + totalRegistroPorPagina + " de " + totalRegistros + " registros, páginas en total: " + totalPaginas);
    if (totalRegistros == 0) {
        $("#caption").text("No se encontraron registros");
    }
    var pageAtrasIfDisabled = "<li class='page-item'>";
    if (page == 1 || totalRegistros == 0) {
        pageAtrasIfDisabled = "<li class='page-item disabled'>";
    }
    var pagineo = pageAtrasIfDisabled + "<a class='page-link' href='javascript:retroceder()'>Anterior</a></li>";    

    var valori = 0;
    var valorleng = 0;
    if (page > rangoPaginas) {
        valori = page - rangoPaginas;
        valorleng = page + rangoPaginas;
        if (valorleng > totalPaginas) {
            valorleng = totalPaginas;
        }
        for (var i = valori; i <= valorleng; i++) {
            if (i == page) {
                pagineo += "<li class='page-item active'><a class='page-link' href='javascript:llamarModalPaginas()'>" + i + "</a></li>";
            } else {
                pagineo += "<li class='page-item'><a class='page-link' href='javascript:paginaSeleccionada(\"" + i + "\")'>" + i + "</a></li>";
            }
        }
    } else {
        var paginasNext = (rangoPaginas * 2) + 1;
        if (paginasNext > totalPaginas) {
            paginasNext = totalPaginas;
        }
        for (var i = 1; i <= paginasNext; i++) {
            if (i == page) {
                pagineo += "<li class='page-item active'><a class='page-link' href='javascript:llamarModalPaginas()'>" + i + "</a></li>";
            } else {
                pagineo += "<li class='page-item'><a class='page-link' href='javascript:paginaSeleccionada(\"" + i + "\")'>" + i + "</a></li>";
            }
        }
    }

    var pageSiguienteIfDisabled = "<li class='page-item'>";
    if (page == totalPaginas || totalRegistros == 0) {
        pageSiguienteIfDisabled = "<li class='page-item disabled'>";
    }    
    pagineo += pageSiguienteIfDisabled + "<a class='page-link' href='javascript:adelante()'>Siguiente</a></li>";    
    $("#pagineo").html(pagineo);
}

function pagineo2() {
    //var paginasMostrar = rangoPaginas + page;
    var pagineo = "<li class='page-item'><a class='page-link' href='javascript:retroceder()'>Atras</a></li>";
    
    pagineo += "<li class='page-item'><a class='page-link' href='javascript:adelante()'>Siguiente</a></li>";
    $("#pagineo").html(pagineo);
}

function adelante() {
    page += 1;
    getData(criterioDeBusquedaUsado, page);
}

function retroceder() {
    if (page > 1) {
        page = page - 1;
    }
    getData(criterioDeBusquedaUsado, page);
}

function paginaSeleccionada(pagina) {
    page = Number(pagina);    
    getData(criterioDeBusquedaUsado, page);
}

function llamarModalPaginas() {
    $("#paginaValue").val("");    
    $('#modalPaginas').modal('show');
}

function goTratamiento(objeto) {
    var aspxPage = "";
    var tipoTratamiento = objeto.TipoTratamiento;
    switch (tipoTratamiento.toUpperCase()) {
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
            break;
        case "NEBULIZACION TERRESTRE":
            aspxPage = "Cert_Nebulizacion_Terrestre.aspx?select=0";
            break;
        default:
            aspxPage = "";
            break;
    }
    if (aspxPage != "") {
        window.location.href = aspxPage + "&pto=" + objeto.PuestoEncryp + "&ord=" + objeto.NoOrdenEncryp;
        mensajeDeSalidaPagina();
    } else {
        mensajeErrorDialog("El tipo de tratamiento no es valido.");
    }
}

function prepararBotones() {

    $("#buscar").click(function () {
        page = 1;
        getTotalesRegistro(valorDebusqueda(), page);        
        getData(valorDebusqueda(), page);       
    });

    $("#ir").click(function () {
        irAPagina();
    });

}

function irAPagina() {
    var paginaIr = $("#paginaValue").val();
    if (paginaIr == "" || paginaIr < 1) {
        paginaIr = 1;
    }
    page = Number(paginaIr);
    if (page > totalPaginas) {
        page = totalPaginas;
    }
    getData(criterioDeBusquedaUsado, page);
    $('#modalPaginas').modal('hide');
}

function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) {
            return pair[1];
        }
    }
    return false;
}


function valorDebusqueda() {
    return valorBusqueda = $("#busquedaValue").val();
}


function getTotalesRegistro(valorBusqueda, page) {
     
    var params = new Object();
    params.busqueda = valorBusqueda;
    params.currentpage = page;
    
    var cadena = "";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "Bandeja_Ordenes.aspx/GetPaginas",
        data: JSON.stringify(params),
        dataType: "json",
        async: false,
        success: function (data, textStatus) {
            if (textStatus == "success") {                  
                totalRegistros = data.TotalRegistros;
                totalPaginas = data.TotalPaginas;
                            
            }
        },
        error: function (request, status, error) {
            mensajeErrorDialog(jQuery.parseJSON(request.responseText).Message);
            //alert(jQuery.parseJSON(request.responseText).Message);
        }
    });

}

function mensajeDeEspera() {
    //$.blockUI({ message: "prueba" });
    //setTimeout($.unblockUI, 2000); 
    $("#contenidoTabla").block({ message: 'Cargando...' });
    //alert("beforeSend");
}

function quitarMensajeEspera() {
    $("#contenidoTabla").unblock();
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

function ocultarMientrasPageCarga() {
    $("#cargandoContenido").hide();
    $("#contenidoPagina").show();
}

function mensajeDeSalidaPagina() {
    $.blockUI({ message: "Cargando..." });
}