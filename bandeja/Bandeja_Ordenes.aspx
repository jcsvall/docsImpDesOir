<%@ Page Language="C#" MasterPageFile="~/Sitc_Sv.master" AutoEventWireup="true" CodeFile="Bandeja_Ordenes.aspx.cs"
    Inherits="Bandeja_Ordenes" Title="OIRSA - SITC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SampleContent" runat="Server">

<div id="cargandoContenido">
    <em>Cargando contenido...</em>
</div>
<div id="contenidoPagina" style="display:none">
    <form>
                        
        <div class="form-group row">
            
            <div class="col-sm-7">
                <input type="text" class="form-control form-control-sm" id="busquedaValue" placeholder="Busqueda por nombre o c&oacute;digo de cliente" />
            </div>
             <button type="button" class="btn btn-primary btn-sm" id="buscar"><span class="oi oi-magnifying-glass" title="Buscar"></span> Buscar</button>
            
            
        </div>        
    </form>

    <div id="contenidoTabla">
    <table class="table table-striped table-sm table-bordered" id="tbl">
        <caption id="caption"></caption>
            <thead>
                <tr>
                    <th scope="col">Fecha</th>
                    <th scope="col">No. Orden</th>
                    <th scope="col">Operacion MAG</th>
                    <th scope="col">Tipo Tratamiento</th>
                    <th scope="col">Puesto</th> 
                    <th scope="col">Cliente</th> 
                    <th scope="col">Responsable MAG</th>
                    <th scope="col">Placa/Vapor</th> 
                    <th scope="col">Producto</th> 
                    <th scope="col">Acciones</th>                    
                </tr>
            </thead>
            <tbody id="tbody">
                             
            </tbody>
        </table>          

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
