$(document).ready(function () {
    cargarGridServiciosCIEX();
});

function cargarGridServiciosCIEX() {
    jQuery("#dataGrid").jqGrid({
        url: 'Cert_Fumigacion_TerrestreJ.aspx/GetServiciosCIEX',
        datatype: "json",
        mtype: 'POST',
        height: 'auto',
        serializeGridData: function (postData) {
                        return JSON.stringify(postData);
        },
        ajaxGridOptions: { contentType: "application/json" },
        loadonce: true,
        colNames: ['Cantidad', 'Quimico', 'Dosis', 'Tiempo', 'Unidad de Tiempo', 'Origen', 'Destino'],
        colModel: [
            { name: 'IdDetalle', index: 'IdDetalle', width: 55 },
            { name: 'Cantidad', index: 'Cantidad', width: 90 },
            { name: 'name', index: 'name asc, invdate', width: 100 },
            { name: 'amount', index: 'amount', width: 80, align: "right" },
            { name: 'tax', index: 'tax', width: 80, align: "right" },
            { name: 'total', index: 'total', width: 80, align: "right" },
            { name: 'note', index: 'note', width: 150, sortable: false }
        ],
        rowNum: 5,
        rowList: [10, 20, 30],
        pager: '#pagingGrid',
        sortname: 'IdDetalle',
        viewrecords: true,
        sortorder: "desc",
        caption: "Servicios CIEX"
    });
    jQuery("#dataGrid").jqGrid('navGrid', '#pagingGrid', {search:false, edit: false, add: false, del: false });
}

//$(function () {
//    $("#dataGrid").jqGrid({
//        url: 'Cert_Fumigacion_TerrestreJ.aspx/GetDataFromDB',
//        datatype: 'json',
//        mtype: 'POST',

//        serializeGridData: function (postData) {
//            return JSON.stringify(postData);
//        },

//        ajaxGridOptions: { contentType: "application/json" },
//        loadonce: true,
//        colNames: ['IdDetalle', 'Cantidad'],
//        colModel: [
//                        { name: 'IdDetalle', index: 'IdDetalle', width: 80 },
//                        { name: 'Cantidad', index: 'Cantidad', width: 140 }
//        ],
//        pager: '#pagingGrid',
//        rowNum: 1,
//        rowList: [10, 20, 30],
//        viewrecords: true,
//        gridview: true,
//        jsonReader: {
//            page: function (obj) { return 1; },
//            total: function (obj) { return 1; },
//            records: function (obj) { alert(obj.d); return 2; },
//            root: function (obj) { return obj.d; },
//            repeatitems: false,
//            id: "0"
//        },
//        caption: 'jQ Grid Example'
//    });
//}).pagingGrid("#pager", { edit: true, add: true, del: false });