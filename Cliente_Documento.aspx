<%@ Page Language="C#" MasterPageFile="~/Sitc_Sv.master" AutoEventWireup="true" CodeFile="Cliente_Documento.aspx.cs"
    Inherits="Cliente_Documento" Title="OIRSA - SITC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SampleContent" runat="Server">

   
    <%--<form id="form1"> --%> 
    
    <form>
                        
        <div class="form-group row">
            <div class="col-sm-3">
                <select class="custom-select custom-select-sm" id="puestos">
                    <option selected>[Puesto]</option>
                    <%--<option value="1">A</option>
                    <option value="2">B</option>
                    <option value="3">C</option>--%>
                </select>
            </div>
            <div class="col-sm-7">
                <input type="text" class="form-control form-control-sm" id="busquedaValue" placeholder="Busqueda por nombre o c&oacute;digo de cliente" />
            </div>
             <button type="button" class="btn btn-primary btn-sm" id="buscar">Buscar</button>
            
            
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
                        <%--<form>--%>
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
                        <%--</form>--%>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal">Cerrar</button>
                        <button type="button" class="btn btn-primary btn-sm" id="guardar">Guardar</button>
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
                            <button type="button" class="btn btn-primary btn-sm" id="ir">Ir</button>
                            
                        </div>
                    </div>
                    
                </div>
            </div>
        </div>

    <%--</form>--%>
    
    <script src="../JS/fomi/js/jquery-1.9.0.min.js" type="text/javascript"></script>
    <%--<script src="../JS/jQridjs/js/jquery-1.9.0.min.js" type="text/javascript"></script>--%>
    <script src="../JS/fomi/js/popper.min.js" type="text/javascript"></script>
    <%--<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js" integrity="sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1" crossorigin="anonymous"></script>--%>
    <script src="../JS/fomi/js/bootstrap.min.js" type="text/javascript"></script>
    <%--<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>--%>  
    <link rel="stylesheet" href="../JS/fomi/css/bootstrap.min.css" />  
    <%--<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous" />  --%>
    <script src="../JS/fomi/js/alertify.min.js" type="text/javascript"></script>
    <%--<script src="//cdn.jsdelivr.net/npm/alertifyjs@1.11.4/build/alertify.min.js"></script>--%> 
    <link rel="stylesheet" href="../JS/fomi/css/alertify.min.css" />  
    <%--<link rel="stylesheet" href="//cdn.jsdelivr.net/npm/alertifyjs@1.11.4/build/css/alertify.min.css"/>--%> 
    <link rel="stylesheet" href="../JS/fomi/css/default.min.css" />    
    <%--<link rel="stylesheet" href="//cdn.jsdelivr.net/npm/alertifyjs@1.11.4/build/css/themes/default.min.css"/>--%>
    <script src="../JS/fomi/js/jquery.inputmask.js" type="text/javascript"></script>
    <script src="../JS/fomi/js/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../JS/Core/ClienteDocumentoSV.js" type="text/javascript"></script>
    <link rel="stylesheet" href="../JS/fomi/css/defaultStyle.css" />
    
</asp:Content>
