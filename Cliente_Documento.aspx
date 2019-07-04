<%@ Page Language="C#" MasterPageFile="~/Sitc_Sv.master" AutoEventWireup="true" CodeFile="Cliente_Documento.aspx.cs"
    Inherits="Cliente_Documento" Title="OIRSA - SITC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SampleContent" runat="Server">

<div id="cargandoContenido">
    <em>Cargando contenido...</em>
</div>
<div id="contenidoPagina" style="display:none">
    <form>
                        
        <div class="form-group row">
            <div class="col-sm-3">
                <select class="custom-select custom-select-sm" id="puestos">
                    <option selected>[Puesto]</option>
                    
                </select>
            </div>
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
                    <th scope="col">Puesto</th>
                    <th scope="col">Cliente</th>
                    <th scope="col">Nombre</th>
                    <th scope="col">DUI</th>
                    <th scope="col">NIT</th> 
                    <th scope="col">Pasaporte</th> 
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
        <div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">Editar documento</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                       
                            <div class="form-group">
                                <label for="puesto">Puesto</label>
                                <input type="text" readonly class="form-control" id="puesto" placeholder="puesto" />
                            </div>
                            <div class="form-group">
                                <label for="formGroupExampleInput">Cliente</label>
                                <input type="text" readonly class="form-control" id="formGroupExampleInput" placeholder="Cliente" />
                            </div>
                            <div class="form-group">
                                <label for="formGroupExampleInput2">Nombre</label>
                                <input type="text" readonly class="form-control" id="formGroupExampleInput2" placeholder="Ingrese nombre" />
                            </div>
                            <div class="form-group">
                                <label for="dui">DUI</label>
                                <input type="text" class="form-control" id="dui" placeholder="########-#" />
                            </div>
                            <div class="form-group">
                                <label for="nit">NIT</label>
                                <input type="text" class="form-control" id="nit" placeholder="####-######-###-#" />
                            </div>
                            <div class="form-group">
                                <label for="pasaporte">Pasaporte</label>
                                <input type="text" class="form-control" id="pasaporte" placeholder="Ingrese pasaporte" maxlength="25"/>
                            </div>
                        
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal"><span class="oi oi-x"></span> Cerrar</button>
                        <button type="button" class="btn btn-primary btn-sm" id="guardar"><span class="oi oi-hard-drive"></span> Guardar</button>
                    </div>
                </div>
            </div>
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
    <script src="../JS/Core/ClienteDocumentoSV.js" type="text/javascript"></script>
    <link rel="stylesheet" href="../JS/fomi/css/defaultStyle.css" />
    
</asp:Content>
