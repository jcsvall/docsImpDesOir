<%@ Page Language="C#" MasterPageFile="~/Sitc_Sv.master" AutoEventWireup="true" CodeFile="Bandeja_Ordenes.aspx.cs"
    Inherits="Bandeja_Ordenes" Title="OIRSA - SITC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SampleContent" runat="Server">

<div id="cargandoContenido">
    <em>Cargando contenido...</em>
</div>
<div id="contenidoPagina" style="display:none">
    <form>
                        
        <div class="form-group row">
            
            <div class="col-sm-10">
                <input type="text" class="form-control form-control-sm" id="busquedaValue" placeholder="Busqueda por No. Orden, Tratamiento, Placa, Cliente, Fecha dd-MM-yyyy o dd/MM/yyyy" />
            </div>
             <button type="button" class="btn btn-primary btn-sm" id="buscar"><span class="oi oi-magnifying-glass" title="Buscar"></span> Buscar</button>
            
            
        </div>        
    </form>

    <div id="contenidoTabla">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div class="table-responsive">
                        <table class="table table-striped table-sm table-bordered" id="tbl">
                            <caption id="caption"></caption>
                            <thead>
                                <tr>
                                    <th scope="col"><div class="tamanio75">Fecha</div></th>
                                    <th scope="col"><div>No. Orden</div></th>
                                    <th scope="col"><div>Operacion MAG</div></th>
                                    <th scope="col"><div>Tipo Tratamiento</div></th>
                                    <th scope="col"><div>Puesto</div></th>
                                    <th scope="col"><div>Cliente</div></th>
                                    <th scope="col"><div>Responsable MAG</div></th>
                                    <th scope="col"><div>Placa/Vapor</div></th>
                                    <th scope="col"><div>Producto</div></th>
                                    <th scope="col">Acciones</th>
                                </tr>
                            </thead>
                            <tbody id="tbody">
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <nav aria-label="Page navigation example">
            <ul class="pagination pagination-sm" id="pagineo">                
                            
            </ul>
        </nav>
    </div>
               

    <!-- Modal -->
        <div class="modal fade" id="modalPaginas" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered modal-sm" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">Mostrar p&aacute;gina</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="form-group row">
                            <div class="col-sm-2">
                                <label>P&aacute;gina</label>
                            </div>
                            <div class="col-sm-5">
                                <input type="number" class="form-control form-control-sm" id="paginaValue" />
                            </div>                            
                            <button type="button" class="btn btn-primary btn-sm" id="ir"><span class="oi oi-reload" title="Actualizar"></span></button>
                            
                        </div>
                    </div>
                    
                </div>
            </div>
        </div>
</div>    
    
    <script src="../JS/fomi/js/jquery-1.9.0.min.js" type="text/javascript"></script>    
    <script src="../JS/fomi/js/popper.min.js" type="text/javascript"></script>
    <script src="../JS/fomi/js/bootstrap.min.js" type="text/javascript"></script>  
    <link rel="stylesheet" href="../JS/fomi/css/bootstrap.min.css" />
    <script src="../JS/fomi/js/alertify.min.js" type="text/javascript"></script>
    <link rel="stylesheet" href="../JS/fomi/css/alertify.min.css" />
    <link rel="stylesheet" href="../JS/fomi/css/default.min.css" />
    <script src="../JS/fomi/js/jquery.inputmask.js" type="text/javascript"></script>
    <script src="../JS/fomi/js/jquery.blockUI.js" type="text/javascript"></script> 
    <link href="../JS/fomi/css/icon-iconic/css/open-iconic-bootstrap.css" rel="stylesheet">   
    <script src="../JS/Core/BandejaOrdenesSV.js" type="text/javascript"></script>
    <link rel="stylesheet" href="../JS/fomi/css/defaultStyle.css" />
    
</asp:Content>
