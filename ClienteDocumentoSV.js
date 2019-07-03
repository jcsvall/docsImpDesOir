var page = 1;
var objetoGuardar = new Object();
var criterioDeBusquedaUsado = "";
var puestoBuscando = "";
var rangoPaginas = 5;
var totalRegistros = 0;
var totalPaginas = 0;
$(document).ready(function () {    
    llenarDropDownPuestos();
    getTotalesRegistro("", page, "[Puesto]");
    getData("", page, "[Puesto]");
    prepararBotones();
    ocultarMientrasPageCarga();
});

function getData(valorBusqueda, page, puesto) {
    criterioDeBusquedaUsado = valorBusqueda;
    puestoBuscando = puesto;
    var params = new Object();
    params.busqueda = valorBusqueda;
    params.currentpage = page;
    params.puesto = puesto;
    var cadena = "";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "Cliente_Documento.aspx/GetClientList",
        data: JSON.stringify(params),
        dataType: "json",
        async: false,
        beforeSend: function () {            
            //mensajeDeEspera();            
        },
        success: function (data, textStatus) {
            if (textStatus == "success") {                
                jQuery.each(data, function (index, itemData) {
                    //alert(itemData.Nombre);
                    var objeto = new Object();
                    objeto.Cliente = itemData.Cliente;
                    objeto.Nombre = itemData.Nombre;
                    objeto.Puesto = itemData.Puesto;
                    objeto.Dui = itemData.Dui;
                    objeto.Nit = itemData.Nit;
                    objeto.Pasaporte = itemData.Pasaporte;
                    objeto.IdPais = itemData.IdPais;
                    cadena += "<tr><td>" + itemData.Puesto + "</td><td>" + itemData.Cliente + "</td><td>" + itemData.Nombre + "</td><td>" + itemData.Dui + "</td><td>" + itemData.Nit + "</td><td>" + itemData.Pasaporte + "</td>";
                    cadena += "<td> <button type='button' class='btn btn-primary btn-sm' data-toggle='modal' data-target=\"#exampleModal\" onClick='llenarModal(" + JSON.stringify(objeto) + ")'>Editar</button> </td></tr>";
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
    //var paginasMostrar = rangoPaginas + page;
    //var pagineo = "<li class='page-item'><a class='page-link' href='javascript:retroceder()'>|<</a></li>";
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
    //pagineo += pageSiguienteIfDisabled + "<a class='page-link' href='javascript:paginaSeleccionada(\"" + totalPaginas + "\")'>" + ">|" +"</a></li>";    
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
    getData(criterioDeBusquedaUsado, page, puestoBuscando);
}

function retroceder() {
    if (page > 1) {
        page = page - 1;
    }
    getData(criterioDeBusquedaUsado, page, puestoBuscando);
}

function paginaSeleccionada(pagina) {
    page = Number(pagina);    
    getData(criterioDeBusquedaUsado, page, puestoBuscando);
}

function llamarModalPaginas() {
    $("#paginaValue").val("");    
    $('#modalPaginas').modal('show');
}

function llenarModal(objeto) {
    //alert(objeto.nombre);
    $("#formGroupExampleInput").val(objeto.Cliente);
    $("#formGroupExampleInput2").val(objeto.Nombre);
    $("#puesto").val(objeto.Puesto);
    $("#dui").val(objeto.Dui);
    $("#nit").val(objeto.Nit);
    $("#pasaporte").val(objeto.Pasaporte);
    objetoGuardar = objeto;
}

function prepararBotones() {
    $("#guardar").click(function () {
        editarDatos();
    });

    $("#dui").inputmask({ "mask": "99999999-9" });
    $("#nit").inputmask({ "mask": "9999-999999-999-9" });

    //$("#next").click(function () {
    //    page += 1;
    //    getData(criterioDeBusquedaUsado, page, puestoBuscando);
    //});

    //$("#previous").click(function () {
    //    if (page > 1) {
    //        page = page - 1;
    //    }
    //    getData(criterioDeBusquedaUsado, page, puestoBuscando);
    //});

    $("#buscar").click(function () {
        page = 1;
        getTotalesRegistro(valorDebusqueda(), page, valorPuesto());        
        getData(valorDebusqueda(), page, valorPuesto());       
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
    getData(valorDebusqueda(), page, valorPuesto());
    $('#modalPaginas').modal('hide');
}

function mensajeExito() {
    alertify.success("Guardado con exito");
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

function editarDatos() { 
    objetoGuardar.Dui = $("#dui").val();
    objetoGuardar.Nit = $("#nit").val();
    objetoGuardar.Pasaporte = $("#pasaporte").val();
    
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "Cliente_Documento.aspx/ActualizarClienteDoc",
            data: JSON.stringify(objetoGuardar),
            dataType: "json",
            async: false,
            beforeSend: function () {
                //mensajeDeEspera("Guardando...");
            },
            success: function (data, textStatus) {
                if (textStatus == "success") {
                    getData(valorDebusqueda(), page, puestoBuscando);
                    $('#exampleModal').modal('toggle');
                    //quitarMensajeEspera();
                    mensajeExito();                    
                }
            },
            error: function (request, status, error) {
                mensajeErrorDialog(jQuery.parseJSON(request.responseText).Message);
                //alert(jQuery.parseJSON(request.responseText).Message);
            }
        });
    
}

function valorDebusqueda() {
    return valorBusqueda = $("#busquedaValue").val();
}

function valorPuesto() {
    return $("#puestos").val();
}

function llenarDropDownPuestos() {   
    var params = new Object();
    params.puesto = "test";
    var cadena = "<option selected>[Puesto]</option>";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "Cliente_Documento.aspx/ObtenerPuestos",
        data: JSON.stringify(params),
        dataType: "json",
        async: false,
        success: function (data, textStatus) {
            if (textStatus == "success") {
                jQuery.each(data, function (index, itemData) {
                    cadena += "<option value='" + itemData.Puesto + "'>" + itemData.Puesto + " - " + itemData.Nombre + "</option>";                    
                });
                $("#puestos").html(cadena);
            }
        },
        error: function (request, status, error) {
            mensajeErrorDialog(jQuery.parseJSON(request.responseText).Message);
            //alert(jQuery.parseJSON(request.responseText).Message);
        }
    });

}

function getTotalesRegistro(valorBusqueda, page, puesto) {
    //criterioDeBusquedaUsado = valorBusqueda;
    //puestoBuscando = puesto;
    var params = new Object();
    params.busqueda = valorBusqueda;
    params.currentpage = page;
    params.puesto = puesto;
    var cadena = "";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "Cliente_Documento.aspx/GetPaginas",
        data: JSON.stringify(params),
        dataType: "json",
        async: false,
        success: function (data, textStatus) {
            if (textStatus == "success") {                  
                totalRegistros = data.TotalRegistros;
                totalPaginas = data.TotalPaginas;
                //$("#caption").text("No. de Páginas: " + totalPaginas + ", Registros encontrados: " + totalRegistros);                
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
              'message': mensajeError,
              'onok': function () { }
          }).show().setHeader('<em> Ocurrio un error </em> ');
    }
}

function ocultarMientrasPageCarga() {
    $("#cargandoContenido").hide();
    $("#contenidoPagina").show();
}