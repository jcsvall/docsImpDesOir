<%@ Page Language="C#" EnableEventValidation="false" MasterPageFile="~/Sitc_sv.master" AutoEventWireup="true" CodeFile="Cert_Fumigacion_TerrestreJ.aspx.cs" Inherits="Cert_Fumigacion_TerrestreJ" Title="OIRSA - SITC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SampleContent" Runat="Server">
     <link rel="stylesheet" href="<%= Page.ResolveUrl("~/css/Jquery/base/jquery.ui.all.css")%>"/>
    <link href="../css/jqgrid/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <script src="<%= Page.ResolveUrl("~/JS/Personalizados/JQuery/custom/jquery-1.10.2.js")%>"></script>
    <script src="<%= Page.ResolveUrl("~/JS/Personalizados/JQuery/custom/jquery-ui-1.10.4.custom.min.js") %>"></script>
    <script src="<%= Page.ResolveUrl("~/JS/Personalizados/jquery.json.min.js") %>"></script>
    <script src="<%= Page.ResolveUrl("~/JS/Personalizados/jquery.funciones.js") %>"></script>
    

    <script src="<%= Page.ResolveUrl("~/JS/jQridjs/js/i18n/grid.locale-en.js") %>"></script>
    <script src="<%= Page.ResolveUrl("~/JS/jQridjs/js/jquery.jqGrid.min.js") %>"></script>
    
    <script src="<%= Page.ResolveUrl("~/JS/Personalizados/jquery.autoNumeric.js") %>"></script>
    <script src="<%= Page.ResolveUrl("~/JS/Personalizados/UploadV2.js") %>"></script>
    <script src="<%= Page.ResolveUrl("~/JS/Core/formasPago.js") %>"></script>
    <script src="<%= Page.ResolveUrl("~/JS/Core/Fumigacion.SV.js") + "?id=" + DateTime.Now.ToString() %>"></script>
    <script src="<%= Page.ResolveUrl("~/JS/Core/Fumigacion.SV.CIEX.js") %>"></script>
    <script type='text/javascript'>
        function cancelClick() {
            var label = $get('ctl00_SampleContent_Label1');
            label.innerHTML = 'Cancelado a las:' + (new Date()).localeFormat("T") + '.';
        }
        
        function Cover(bottom, top, ignoreSize) {
            var location = Sys.UI.DomElement.getLocation(bottom);
            top.style.position = 'absolute';
            top.style.top = location.y + 'px';
            top.style.left = location.x + 'px';
            if (!ignoreSize) {
                top.style.height = bottom.offsetHeight + 'px';
                top.style.width = bottom.offsetWidth + 'px';
            }
        }
        
        function updateTime()
        {
            var label = document.getElementById('ctl00_SampleContent_currentTime');
            if (label) {
                var time = (new Date()).localeFormat("T");
                label.innerHTML = time;
            }
        }
        updateTime();
        window.setInterval(updateTime, 1000);
    </script>
      <asp:HiddenField  id="hdTipo" runat="server" />

    <div align="center">
        <table width="100%" border="0">
            <tr>
                <td colspan="2">
                    <table width="100%" border="0">
                        <tr>
                            <td valign="top" align="left"><div class="demoheading" id="dvTitle">FUMIGACIÓN TERRESTRE</div></td>
                            <td align="right" style="display:none">
                                <asp:Panel ID="n_certificado" runat="server" Width="200px" BackColor="White" ForeColor="DarkBlue" BorderWidth="2" BorderStyle="solid" BorderColor="DarkBlue" style="z-index: 1;">
                                    <div style="width: 100%; height: 100%; vertical-align:middle; text-align: center;" align="right">
                                        <p>Certificado No:</p>
                                        <asp:Label ID="l_n_certificado" runat="server" Text="325658" Font-Size="Large" Font-Bold="true"></asp:Label>
                                    </div>
                                </asp:Panel>    
                                <div style="width:230px;height:20px;">
                                    <asp:Panel ID="timer" runat="server" Width="120px" BackColor="White" ForeColor="DarkBlue" BorderWidth="1" BorderStyle="solid" BorderColor="DarkBlue" style="z-index: 1;">
                                        <span id="currentTime" runat="server" style="font-size:large;font-weight:bold;line-height:30px;"/>
                                        <ajaxToolkit:AlwaysVisibleControlExtender ID="avce" runat="server" TargetControlID="timer" VerticalSide="Top" VerticalOffset="10" HorizontalSide="Right" HorizontalOffset="10" ScrollEffectDuration=".1" />
                                    </asp:Panel>
                                </div>    
                            </td>
                        </tr>   
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width:150px" align="left">Fecha</td>
                <td align="left" style="width: 545px">
                    <asp:TextBox runat="server" ID="tb_fecha" Width="70px" ReadOnly="True" />
                    <asp:Panel ID="p_fecha" runat="server" Visible="true">
                        <asp:ImageButton runat="Server" ID="ib_fecha" ImageUrl="~/images/Calendar_scheduleHS.png" AlternateText="Click to show calendar" /><br />
                        <ajaxToolkit:MaskedEditExtender ID="mee_fecha" runat="server" TargetControlID="tb_fecha" Mask="99/99/9999" MessageValidatorTip="true" CultureName="en-US" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError" MaskType="Date" DisplayMoney="Left" AcceptNegative="Left" ErrorTooltipEnabled="True" />
                        <ajaxToolkit:MaskedEditValidator ID="mev_fecha" runat="server" ControlExtender="mee_fecha" ControlToValidate="tb_fecha" EmptyValueMessage="Fecha es Requerida" InvalidValueMessage="Fecha Invalida" Display="Dynamic" TooltipMessage="Ingresar Fecha" EmptyValueBlurredText="*" InvalidValueBlurredMessage="Formato de fecha Invalida [mm/dd/aaaa]" />
                        <ajaxToolkit:CalendarExtender ID="ce_fecha" runat="server" TargetControlID="tb_fecha" PopupButtonID="ib_fecha" />
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td align="left">Fecha Inicio Tratamiento</td>
                <td align="left" style="width: 545px">
                    <asp:TextBox runat="server" ID="tb_fecha_tratamiento" Width="70px" />
                    <asp:ImageButton runat="Server" ID="ib_fecha_tratamiento" ImageUrl="~/images/Calendar_scheduleHS.png" AlternateText="Click to show calendar" /><br />
                    <ajaxToolkit:MaskedEditExtender ID="mee_fecha_tratamiento" runat="server" TargetControlID="tb_fecha_tratamiento" Mask="99/99/9999" MessageValidatorTip="true" CultureName="en-US" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError" MaskType="Date" DisplayMoney="Left" AcceptNegative="Left" ErrorTooltipEnabled="True" />
                    <ajaxToolkit:MaskedEditValidator ID="mev_fecha_tratamiento" runat="server" ControlExtender="mee_fecha_tratamiento" ControlToValidate="tb_fecha_tratamiento" EmptyValueMessage="Fecha de Inicio de Tratamiento es Requerida" InvalidValueMessage="Fecha de Inicio de Tratamiento Invalida" Display="Dynamic" TooltipMessage="Ingresar Fecha de Inicio de Tratamiento" EmptyValueBlurredText="*" InvalidValueBlurredMessage="*" />
                    <ajaxToolkit:CalendarExtender ID="ce_fecha_tratamiento" runat="server" TargetControlID="tb_fecha_tratamiento" PopupButtonID="ib_fecha_tratamiento" Format="MM/dd/yyyy" />
                </td>
            </tr>
            <tr>
                <td align="left">Hora Inicio Tratamiento</td>
                <td align="left" style="width: 545px">
                    <asp:TextBox runat="server" ID="tb_hora_ini" Width="80px" Height="16px" ValidationGroup="Calcular" />
                    <ajaxToolkit:MaskedEditExtender ID="mee_hora_ini" runat="server" TargetControlID="tb_hora_ini" Mask="99:99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError" MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True" />
                    <ajaxToolkit:MaskedEditValidator ID="mev_hora_ini" runat="server" ControlExtender="mee_hora_ini" ControlToValidate="tb_hora_ini" IsValidEmpty="False" EmptyValueMessage="Hora es requerida" InvalidValueMessage="Hora invalida" Display="Dynamic" TooltipMessage="Ingresar hora" EmptyValueBlurredText="*" InvalidValueBlurredMessage="*" ValidationGroup="Calcular" />
                </td>
            </tr>
            <tr>
                <td align="left">Fecha Fin Tratamiento</td>
                <td align="left" style="width: 545px">
                    <asp:TextBox runat="server" ID="tb_fecha_tratamiento_fin" Width="70px" />
                    <asp:ImageButton runat="Server" ID="ib_fecha_tratamiento_fin" ImageUrl="~/images/Calendar_scheduleHS.png" AlternateText="Click to show calendar" /><br />
                    <ajaxToolkit:MaskedEditExtender ID="mee_fecha_tratamiento_fin" runat="server" TargetControlID="tb_fecha_tratamiento_fin" Mask="99/99/9999" MessageValidatorTip="true" CultureName="en-US" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError" MaskType="Date" DisplayMoney="Left" AcceptNegative="Left" ErrorTooltipEnabled="True" />
                    <ajaxToolkit:MaskedEditValidator ID="mev_fecha_tratamiento_fin" runat="server" ControlExtender="mee_fecha_tratamiento_fin" ControlToValidate="tb_fecha_tratamiento_fin" EmptyValueMessage="Fecha de Finalización de Tratamiento es Requerida" InvalidValueMessage="Fecha de Finalización de Tratamiento Invalida" Display="Dynamic" TooltipMessage="Ingresar Fecha de Finalización de Tratamiento" EmptyValueBlurredText="*" InvalidValueBlurredMessage="*" />
                    <ajaxToolkit:CalendarExtender ID="ce_fecha_tratamiento_fin" runat="server" TargetControlID="tb_fecha_tratamiento_fin" PopupButtonID="ib_fecha_tratamiento_fin" Format="MM/dd/yyyy" />
                </td>
            </tr>
            <tr>
                <td align="left">Hora Fin Tratamiento</td>
                <td align="left" style="width: 545px">
                    <asp:TextBox runat="server" ID="tb_hora_fin" Width="80px" Height="16px" ValidationGroup="Calcular" />
                <ajaxToolkit:MaskedEditExtender ID="mee_hora_fin" runat="server" TargetControlID="tb_hora_fin" Mask="99:99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError" MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True" />
                <ajaxToolkit:MaskedEditValidator ID="mev_hora_fin" runat="server" ControlExtender="mee_hora_fin" ControlToValidate="tb_hora_fin" IsValidEmpty="False" EmptyValueMessage="Hora es requerida" InvalidValueMessage="Hora invalida" Display="Dynamic" TooltipMessage="Ingresar hora" EmptyValueBlurredText="*" InvalidValueBlurredMessage="*" ValidationGroup="Calcular" />
                </td>
            </tr>
            <tr runat="server" visible="false">
                <td align="left">Cambio</td>
                <td align="left" style="width: 545px"><asp:TextBox runat="server" ID="tb_cambio" Width="50px" Text="1" ReadOnly="True" /></td>
            </tr>
            <tr runat="server" visible="false">
                <td align="left">Cortesía</td>
                <td align="left" style="width: 545px"><asp:CheckBox ID="cb_cortesia" runat="server" /></td>
            </tr>
            <tr runat="server" visible="true">
                <td align="left">Local</td>
                <td align="left" style="width: 545px"><asp:CheckBox ID="cb_local" runat="server" Checked="true" Enabled="False" AutoPostBack="false" OnCheckedChanged="cb_local_CheckedChanged" /></td>
            </tr>
            <tr>
                <td align="left">Tipo de Cliente</td>
                <td align="left">
                    <asp:RadioButton ID="rb_tipo_cliente_i" runat="server" GroupName="cliente" Checked="true" Text="Importador" />&nbsp;
                    <asp:RadioButton ID="rb_tipo_cliente_e" runat="server" GroupName="cliente" Text="Exportador" />
                </td>
            </tr>
            <tr>
                <td align="left"><asp:Label ID="l_cliente" runat="server" Text="Cliente de Crédito"></asp:Label></td>
                <td align="left" style="width: 545px">
                    <asp:DropDownList ID="ddl_cliente" runat="server" AutoPostBack="false" OnSelectedIndexChanged="ddl_cliente_SelectedIndexChanged" />&nbsp;
                    <a href="javascript:void();" id="lkDetalleCliente">Pendiente de pago</a>
                    <ajaxToolkit:ListSearchExtender ID="lse_cliente" runat="server" TargetControlID="ddl_cliente" PromptCssClass="ListSearchExtenderPrompt"></ajaxToolkit:ListSearchExtender><br />
                    <div style="display:none">
                        <asp:CheckBox ID="cb_cliente_contado" runat="server" AutoPostBack="True" OnCheckedChanged="cb_cliente_contado_CheckedChanged" Text=" Contado" />&nbsp;
                    </div>
                    <select id="lsFormasPago">
                        <option value="1">Credito</option>
                        <option value="0">Contado</option>
                        <option value="-1">Tarjeta de credito</option>                        
                    </select>

                </td>
           </tr>
           <tr>
                <td align="left">Cliente Extra</td>
                <td align="left" style="width: 545px">
                    <asp:TextBox ID="tb_cliente" runat="server" Width="250px"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_cliente" runat="server" TargetControlID="tb_cliente" FilterType="Custom" FilterMode="InvalidChars" InvalidChars="'" />
                </td>
            </tr>
           <tr>
                <td>&nbsp;</td>
                <td align="left" style="width: 545px">
                    
                    <div id="flyout" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;">
                        <asp:LinkButton ID="btnInfo" runat="server" OnClientClick="return false;" Font-Size="X-Small">&laquo; Información Cliente &raquo;</asp:LinkButton>
                    </div>
                    <div id="info" style="display: none; width: 250px; z-index: 2; opacity: 0; filter: progid:DXImageTransform.Microsoft.Alpha(opacity=0); font-size: 12px; border: solid 1px #CCCCCC; background-color: #FFFFFF; padding: 5px;">
                        <div id="btnCloseParent" style="float: right; opacity: 0; filter: progid:DXImageTransform.Microsoft.Alpha(opacity=0);">
                            <asp:LinkButton id="btnClose" runat="server" OnClientClick="return false;" Text="X" ToolTip="Close"
                            Style="background-color: #666666; color: #FFFFFF; text-align: center; font-weight: bold; text-decoration: none; border: outset thin #FFFFFF; padding: 5px;" />
                        </div>
                        <div>
                            <p>
                                Cantidad de Certificados Pendientes: <asp:Label ID="l_cantidad" runat="server" Text="" Font-Bold="true" Font-Size="Small"></asp:Label><br />
                                Total Pendiente de Pago: <asp:Label ID="l_total" runat="server" Text="" Font-Bold="true" Font-Size="Small"></asp:Label><br />
                                Moneda: <asp:Label ID="l_moneda" runat="server" Text="" Font-Bold="true" Font-Size="Small"></asp:Label><br />
                            </p>
                            <br />
                            <p>
                                <asp:LinkButton id="lnkShow" OnClientClick="return false;" runat="server" Visible="false">show</asp:LinkButton>
                                <asp:LinkButton OnClientClick="return false;" id="lnkClose" runat="server" Visible="false">close</asp:LinkButton>
                            </p>
                        </div>
                    </div>
                    <ajaxToolkit:AnimationExtender id="OpenAnimation" runat="server" TargetControlID="btnInfo">
                        <Animations>
                            <OnClick>
                                <Sequence>
                                    <%-- Disable the button so it can't be clicked again --%>
                                    <EnableAction Enabled="false" />
                                    
                                    <%-- Position the wire frame on top of the button and show it --%>
                                    <ScriptAction Script="Cover($get('ctl00_SampleContent_btnInfo'), $get('flyout'));" />
                                    <StyleAction AnimationTarget="flyout" Attribute="display" Value="block"/>
                                    
                                    <%-- Move the wire frame from the button's bounds to the info panel's bounds --%>
                                    <Parallel AnimationTarget="flyout" Duration=".3" Fps="25">
                                        <Move Horizontal="150" Vertical="-50" />
                                        <Resize Width="260" Height="280" />
                                        <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                                    </Parallel>
                                    
                                    <%-- Move the info panel on top of the wire frame, fade it in, and hide the frame --%>
                                    <ScriptAction Script="Cover($get('flyout'), $get('info'), true);" />
                                    <StyleAction AnimationTarget="info" Attribute="display" Value="block"/>
                                    <FadeIn AnimationTarget="info" Duration=".2"/>
                                    <StyleAction AnimationTarget="flyout" Attribute="display" Value="none"/>
                                    
                                    <%-- Flash the text/border red and fade in the "close" button --%>
                                    <Parallel AnimationTarget="info" Duration=".5">
                                        <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                                        <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                                    </Parallel>
                                    <Parallel AnimationTarget="info" Duration=".5">
                                        <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                                        <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                                        <FadeIn AnimationTarget="btnCloseParent" MaximumOpacity=".9" />
                                    </Parallel>
                                </Sequence>
                            </OnClick>
                        </Animations>
                    </ajaxToolkit:AnimationExtender>
                    <ajaxToolkit:AnimationExtender id="CloseAnimation" runat="server" TargetControlID="btnClose">
                        <Animations>
                            <OnClick>
                                <Sequence AnimationTarget="info">
                                    <%--  Shrink the info panel out of view --%>
                                    <StyleAction Attribute="overflow" Value="hidden"/>
                                    <Parallel Duration=".3" Fps="15">
                                        <Scale ScaleFactor="0.05" Center="true" ScaleFont="true" FontUnit="px" />
                                        <FadeOut />
                                    </Parallel>
                                    
                                    <%--  Reset the sample so it can be played again --%>
                                    <StyleAction Attribute="display" Value="none"/>
                                    <StyleAction Attribute="width" Value="250px"/>
                                    <StyleAction Attribute="height" Value=""/>
                                    <StyleAction Attribute="fontSize" Value="12px"/>
                                    <OpacityAction AnimationTarget="btnCloseParent" Opacity="0" />
                                    
                                    <%--  Enable the button so it can be played again --%>
                                    <EnableAction AnimationTarget="btnInfo" Enabled="true" />
                                </Sequence>
                            </OnClick>
                            <OnMouseOver>
                                <Color Duration=".2" PropertyKey="color" StartValue="#FFFFFF" EndValue="#FF0000" />
                            </OnMouseOver>
                            <OnMouseOut>
                                <Color Duration=".2" PropertyKey="color" StartValue="#FF0000" EndValue="#FFFFFF" />
                            </OnMouseOut>
                         </Animations>
                    </ajaxToolkit:AnimationExtender>
                    <asp:Panel ID="xmlShow" runat="server" style="display: none; z-index: 3; background-color:#DDD; border: thin solid navy;">
                        <pre style="margin: 5px">
                            Ejemplo1
                        </pre>
                    </asp:Panel>
                    <asp:Panel ID="xmlClose" runat="server" style="display: none; z-index: 3; background-color: #DDD; border: thin solid navy;">
                        <pre style="margin: 5px">
                            Ejemplo2
                        </pre>
                    </asp:Panel>
                    <ajaxToolkit:HoverMenuExtender ID="hm2" runat="server" TargetControlID="lnkShow" PopupControlID="xmlShow" PopupPosition="Bottom" />
                    <ajaxToolkit:HoverMenuExtender ID="hm1" runat="server" TargetControlID="lnkClose" PopupControlID="xmlClose" PopupPosition="Bottom" />
                </td>
            </tr>
            <tr  id="trNPlaca">
                <td align="left" >No. Placa</td>
                <td align="left" style="width: 545px">
                    <asp:TextBox runat="server" ID="tb_nplaca" Width="80px" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_nplaca" runat="server" TargetControlID="tb_nplaca" FilterType="Custom" FilterMode="InvalidChars" InvalidChars="'" />
                </td>
            </tr>
            <tr id="trnViaje">
                <td align="left">No. Viaje</td>
                <td align="left" style="width: 473px">
                    <asp:TextBox runat="server" ID="tb_nviaje" Width="80px" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" TargetControlID="tb_nviaje" FilterType="Custom" FilterMode="InvalidChars" InvalidChars="'" />
                </td>
            </tr>
            <tr id="trnVapor">
                <td align="left">Nombre Vapor</td>
                <td align="left">
                    <asp:TextBox runat="server" ID="tb_nvapor" Width="250px" MaxLength="50" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_nvapor" runat="server" TargetControlID="tb_nvapor" FilterType="Custom" FilterMode="InvalidChars" InvalidChars="'" />
                </td>
            </tr>
            <tr id="trFechaAtraque">
                <td align="left">Fecha Atraque</td>
                <td align="left">
                    <asp:TextBox runat="server" ID="tb_fecha_atraque" Width="70px" />
                    <asp:ImageButton runat="Server" ID="ib_fecha_atraque" ImageUrl="~/images/Calendar_scheduleHS.png" AlternateText="Click to show calendar" /><br />
                    <ajaxToolkit:MaskedEditExtender ID="mee_fecha_atraque" runat="server" TargetControlID="tb_fecha_atraque" Mask="99/99/9999" MessageValidatorTip="true" CultureName="en-US" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError" MaskType="Date" DisplayMoney="Left" AcceptNegative="Left" ErrorTooltipEnabled="True" />
                    <ajaxToolkit:MaskedEditValidator ID="mev_fecha_atraque" runat="server" ControlExtender="mee_fecha_atraque" ControlToValidate="tb_fecha_atraque" EmptyValueMessage="Fecha de Atraque es Requerida" InvalidValueMessage="Fecha de Atraque Invalida" Display="Dynamic" TooltipMessage="Ingresar Fecha de Atraque" EmptyValueBlurredText="*" InvalidValueBlurredMessage="Formato de fecha Invalida [mm/dd/aaaa]" />
                    <ajaxToolkit:CalendarExtender ID="ce_fecha_atraque" runat="server" TargetControlID="tb_fecha_atraque" PopupButtonID="ib_fecha_atraque" />
                </td>
            </tr>
            <tr id="trHoraAtraque">
                <td style="width:150px; background-color:#EFEFEF" align="left">Hora Atraque</td>
                <td align="left">
                    <asp:TextBox runat="server" ID="tb_hora_atraque" Width="80px" Height="16px" ValidationGroup="Calcular" />
                    <ajaxToolkit:MaskedEditExtender ID="mee_hora_atraque" runat="server" TargetControlID="tb_hora_atraque" Mask="99:99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError" MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True" />
                    <ajaxToolkit:MaskedEditValidator ID="mev_hora_atraque" runat="server" ControlExtender="mee_hora_atraque" ControlToValidate="tb_hora_atraque" IsValidEmpty="False" EmptyValueMessage="Hora es requerida" InvalidValueMessage="Hora invalida" Display="Dynamic" TooltipMessage="Ingresar hora" EmptyValueBlurredText="*" InvalidValueBlurredMessage="*" />
                </td>
            </tr>
            <tr>
                <td align="left"><asp:Label ID="l_motivo" runat="server" Text="Motivo de la Fumigación"></asp:Label></td>
                <td align="left" style="width: 545px">
                    <asp:DropDownList ID="ddl_motivo" runat="server" />&nbsp;
                </td>
           </tr>
        </table><br />
    </div>
    <div class="demoarea">
        <table id="dataGrid" style="text-align: center;"></table>
        <div id="pagingGrid"></div>
        <div class="heading">Detalle del Servicio Aplicado</div>
        <table width="95%">
            <tr>
                <td style="width:150px">Servicio</td>
                <td><asp:DropDownList ID="ddl_servicio" runat="server" AutoPostBack="False" OnSelectedIndexChanged="ddl_servicio_SelectedIndexChanged" /></td>
            </tr>
            <tr>
                <td>Cantidad</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_cantidad" Text="1" Width="70px" AutoPostBack="false" OnTextChanged="tb_cantidad_TextChanged" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_cantidad" runat="server" TargetControlID="tb_cantidad" FilterType="Custom, Numbers" ValidChars="." />
                </td>
            </tr>
            <tr>
                <td>Costo (US)</td>
                <td><asp:TextBox runat="server" ID="tb_costo_us" ReadOnly="true" Width="70px" /></td>
            </tr>
            <tr>
                <td>Costo (Local)</td>
                <td><asp:TextBox runat="server" ID="tb_costo_local" ReadOnly="true" Width="70px" /></td>
            </tr>
            <tr>
                <td>SubTotal</td>
                <td><asp:TextBox runat="server" ID="tb_subtotal" ReadOnly="true" Width="70px" /></td>
            </tr>
            <tr>
                <td>Plaguicida</td>
                <td><asp:DropDownList ID="ddl_plaguicida" runat="server" AutoPostBack="false" OnSelectedIndexChanged="ddl_plaguicida_SelectedIndexChanged" /></td>
            </tr>
            <tr>
                <td>Dosis</td>
                <td><asp:DropDownList ID="ddl_unidad_dosis" runat="server" AutoPostBack="false" OnSelectedIndexChanged="ddl_unidad_dosis_SelectedIndexChanged" /></td>
            </tr>
            <tr>
                <td>
                    Humedad Relativa</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_humedad_relativa" Width="50px" />                                        
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" TargetControlID="tb_humedad_relativa"
                        FilterType="Custom, Numbers" ValidChars="." />
                </td>
            </tr>
            <tr>
                <td>Valor Dosis</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_dosis" Width="100px" ReadOnly="true" />
                    <asp:RequiredFieldValidator runat="server" ID="rfv_dosis" ControlToValidate="tb_dosis" Display="None" ErrorMessage="<b>Dosis es requerida.</b>" ValidationGroup="Certificado_Detalle" />
                    <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vce_dosis" TargetControlID="rfv_dosis" HighlightCssClass="validatorCalloutHighlight" Width="250px" />
                </td>
            </tr>
            <tr>
                <td>Lugar Tratado</td>
                <td><asp:DropDownList ID="ddl_lugar_tratado" runat="server" AutoPostBack="false" OnSelectedIndexChanged="ddl_lugar_tratado_SelectedIndexChanged" /></td>
            </tr>
            <tr>
                <td>Producto</td>
                <td>
                    <asp:DropDownList ID="ddl_producto" runat="server" AutoPostBack="false" OnSelectedIndexChanged="ddl_producto_SelectedIndexChanged" />
                    <ajaxToolkit:ListSearchExtender ID="lse_producto" runat="server" TargetControlID="ddl_producto" PromptCssClass="ListSearchExtenderPrompt"></ajaxToolkit:ListSearchExtender>
                </td>
            </tr>
            <tr>
                <td>Peso Producto (Tm)</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_peso_producto_tm" Width="70px" Text="0" AutoPostBack="false" OnTextChanged="tb_peso_producto_tm_TextChanged" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_peso_producto_tm" runat="server" TargetControlID="tb_peso_producto_tm" FilterType="Custom, Numbers" ValidChars="." />
                </td>
            </tr>
            <tr>
                <td>Cantidad de Producto</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_peso_producto" Width="70px" Text="0" AutoPostBack="false" OnTextChanged="tb_peso_producto_TextChanged"  /><asp:DropDownList ID="ddl_peso_producto" runat="server" />
                    <asp:RequiredFieldValidator runat="server" ID="rfv_peso_producto" ControlToValidate="tb_peso_producto" Display="None" ErrorMessage="<b>Peso del Producto es requerido.</b>" ValidationGroup="Certificado_Detalle" />
                    <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vce_peso_producto" TargetControlID="rfv_peso_producto" HighlightCssClass="validatorCalloutHighlight" Width="250px" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_peso_producto" runat="server" TargetControlID="tb_peso_producto" FilterType="Custom, Numbers" ValidChars="." />
                </td>
            </tr>
            
            <tr style="display:none;" id="ctl00_SampleContent_p_tipo">
                <td><asp:Label ID="l_tipo" runat="server" Text="Densidad"></asp:Label></td>
                <td>
                    <asp:DropDownList ID="ddl_tipo" runat="server" AutoPostBack="false" OnSelectedIndexChanged="ddl_tipo_SelectedIndexChanged" />&nbsp;
                    <asp:TextBox ID="tb_tipo" runat="server" Visible="false" Text="0"></asp:TextBox>
                   <div id="dvContendor" style="display:none;"> 
                        <asp:Label ID="l_contenedor" runat="server" Text="Especificar contenedor" Visible="true"></asp:Label>
                        <asp:TextBox ID="tb_contenedor" runat="server"  Text=""></asp:TextBox>
                        <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_contenedor" runat="server" TargetControlID="tb_contenedor" FilterType="Custom" FilterMode="InvalidChars" InvalidChars="'" />
                    </div>
                </td>
            </tr>
            
            <tr>
                <td>Cantidad Cubicada</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_cantidad_cubicada" Text="0" Width="70px" AutoPostBack="false" OnTextChanged="tb_cantidad_cubicada_TextChanged" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_cantidad_cubicada" runat="server" TargetControlID="tb_cantidad_cubicada" FilterType="Custom, Numbers" ValidChars="." />
                </td>
            </tr>
            <tr>
                <td>Consumo Teórico</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_consumo_teorico" Text="0" Width="70px" ReadOnly="true" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_consumo_teorico" runat="server" TargetControlID="tb_consumo_teorico" FilterType="Custom, Numbers" ValidChars="." />
                </td>
            </tr>
            <tr>
                <td>Consumo Real</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_consumo_real" Width="70px" Text="0" AutoPostBack="false" OnTextChanged="tb_consumo_real_TextChanged" />
                    <asp:RequiredFieldValidator runat="server" ID="rfv_consumo_real" ControlToValidate="tb_consumo_real" Display="None" ErrorMessage="<b>Consumo Real es requerido.</b>" ValidationGroup="Certificado_Detalle" />
                    <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vce_consumo_real" TargetControlID="rfv_consumo_real" HighlightCssClass="validatorCalloutHighlight" Width="250px" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_consumo_real" runat="server" TargetControlID="tb_consumo_real" FilterType="Custom, Numbers" ValidChars="." />
                </td>
            </tr>
            <tr>
                <td>Desviación</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_desviacion" Text="0" Width="70px" AutoPostBack="false" ReadOnly="true"/>
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_desviacion" runat="server" TargetControlID="tb_desviacion" FilterType="Custom, Numbers" ValidChars="." />
                </td>
            </tr>
            <tr>
                <td>Tiempo Exposición</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_tiempo_exposicion" Width="70px" /><asp:DropDownList ID="ddl_unidad_tiempo" runat="server" />
                    <asp:RequiredFieldValidator runat="server" ID="rfv_tiempo_exposicion" ControlToValidate="tb_tiempo_exposicion" Display="None" ErrorMessage="<b>Tiempo de Exposición es requerido.</b>" ValidationGroup="Certificado_Detalle" />
                    <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vce_tiempo_exposicion" TargetControlID="rfv_tiempo_exposicion" HighlightCssClass="validatorCalloutHighlight" Width="250px" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_tiempo_exposicion" runat="server" TargetControlID="tb_tiempo_exposicion" FilterType="Custom, Numbers" ValidChars="." />
                </td>
            </tr>
            <tr>
                <td>Concentración</td>
                <td>
                    <asp:TextBox ID="tb_concentracion" runat="server" TextMode="MultiLine" Height="50px" Width="426px"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_concentracion" runat="server" TargetControlID="tb_concentracion" FilterType="Custom" FilterMode="InvalidChars" InvalidChars="'" />
                </td>
            </tr>
            <tr>
                <td>Temperatura Ambiente</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_temperatura" Text="0" Width="70px" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_temperatura" runat="server" TargetControlID="tb_temperatura" FilterType="Custom, Numbers" ValidChars="." />&nbsp;C°
                </td>
            </tr>
            <tr>
                <td>Tiempo de Aereación</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_tiempo_aereacion" Text="0" Width="70px" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_tiempo_aereacion" runat="server" TargetControlID="tb_tiempo_aereacion" FilterType="Custom, Numbers" ValidChars="." /><asp:DropDownList ID="ddl_tiempo_aereacion" runat="server" />
                </td>
            </tr>
            <tr>
                <td>Origen</td>
                <td>
                    <asp:DropDownList ID="ddl_origen" runat="server" />
                    <ajaxToolkit:ListSearchExtender ID="lse_origen" runat="server" TargetControlID="ddl_origen" PromptCssClass="ListSearchExtenderPrompt"></ajaxToolkit:ListSearchExtender>
                </td>
            </tr>
            <tr>
                <td>Procedencia</td>
                <td>
                    <asp:DropDownList ID="ddl_procedencia" runat="server" />
                    <ajaxToolkit:ListSearchExtender ID="lse_procedencia" runat="server" TargetControlID="ddl_procedencia" PromptCssClass="ListSearchExtenderPrompt"></ajaxToolkit:ListSearchExtender>
                </td>
            </tr>
            <tr>
                <td>Destino</td>
                <td>
                    <asp:DropDownList ID="ddl_destino" runat="server" />
                    <ajaxToolkit:ListSearchExtender ID="lse_destino" runat="server" TargetControlID="ddl_destino" PromptCssClass="ListSearchExtenderPrompt"></ajaxToolkit:ListSearchExtender>
                </td>
            </tr>
            <tr>
                <td>Ruta</td>
                <td><asp:DropDownList ID="ddl_ruta" runat="server" /></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <div style="display:none;">
                        <asp:Button ID="b_aceptar" runat="server" Text=" Agregar Servicio " OnClick="b_aceptar_Click" ValidationGroup="Certificado_Detalle" BorderStyle="Dashed"  />
                    </div>
                    <input type="button" id="btAgregarServ" value="Agregar Servicio" />
                    <input type="button" id="btSobreCargo" value="Agregar Recargo" />
                </td>
                <td>&nbsp;</td>
            </tr>
        </table><br />
        <table width="95%">
        <tr>
                <td>
                    <div>
                        <table id="jqAtomizacion"></table>
                        <div id="dvpager"></div>
                    </div>
                </td> 
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="gv_servicios" DataKeyNames="Detalle" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" AutoGenerateColumns="False" OnSelectedIndexChanged="gv_servicios_SelectedIndexChanged">
                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                        <AlternatingRowStyle BackColor="#DCDCDC" />
                    <Columns>
                        <asp:BoundField DataField="Detalle" HeaderText="Id" HeaderStyle-Font-Size="XX-Small" ItemStyle-Font-Size="XX-Small" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Names="Arial" ItemStyle-Font-Names="Arial" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemStyle VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Servicio" HeaderText="Servicio" HeaderStyle-Font-Size="XX-Small" ItemStyle-Font-Size="XX-Small" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Names="Arial" ItemStyle-Font-Names="Arial" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemStyle VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="XX-Small" ItemStyle-Font-Size="XX-Small" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Names="Arial" ItemStyle-Font-Names="Arial" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemStyle VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:BoundField DataField="US" HeaderText="US" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="XX-Small" ItemStyle-Font-Size="XX-Small" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Names="Arial" ItemStyle-Font-Names="Arial" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemStyle VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Local" HeaderText="Local" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="XX-Small" ItemStyle-Font-Size="XX-Small" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Names="Arial" ItemStyle-Font-Names="Arial" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemStyle VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SubTotal" HeaderText="Total" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="XX-Small" ItemStyle-Font-Size="XX-Small" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Names="Arial" ItemStyle-Font-Names="Arial" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemStyle VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Plaguicida" HeaderText="NombrePlaguicida" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="XX-Small" ItemStyle-Font-Size="XX-Small" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Names="Arial" ItemStyle-Font-Names="Arial" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemStyle VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:ButtonField Text="&lt;IMG src=images/delete.png border=0 alt=&quot;Eliminar&quot;&gt;" CommandName="Select" ItemStyle-Font-Size="X-Small" ItemStyle-Font-Bold="true" />
                    </Columns>
                    </asp:GridView>         
                </td>
            </tr>
        </table><br />
        <table width="98%" style="background-color:#EFEFEF;">
            <tr>
                <td style="width:150px; background-color:#EFEFEF">Servicios Agregados</td>
                <td><asp:Label ID="l_servicios" runat="server" Text="0" Font-Bold="true" Font-Size="Medium" Font-Underline="true"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px; background-color:#EFEFEF">Valor en Letras</td>
                <td><asp:Label ID="l_numlet" runat="server" Text="" Font-Bold="true" Font-Size="Medium"></asp:Label></td>
            </tr>
            <tr>
                <td style="background-color:#EFEFEF">SubTotal</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_subtotalg" ReadOnly="true" Width="70px" Text="0" />
                    <asp:RangeValidator ID="rv_total" runat="server" ErrorMessage="<b>Imposible grabar este certificado, tiene que agregar un servicio!</b>" MaximumValue="999999999" MinimumValue="1" Display="None" ControlToValidate="tb_subtotalg"></asp:RangeValidator>
                    <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vce_total" TargetControlID="rv_total" HighlightCssClass="validatorCalloutHighlight" Width="250px" />
                </td>
            </tr>
            <tr style="display:none;">
                <td style="background-color:#EFEFEF">Recargo</td>
                <td style="height: 26px">
                    <asp:TextBox runat="server" ID="tb_recargo" Width="70px" Text="0" ReadOnly="false" />&nbsp;
                    <div style="display:none;">
                        <asp:CheckBox runat="server" ID="cb_recargo" AutoPostBack="true" Text="(25%)" OnCheckedChanged="cb_recargo_CheckedChanged"  />
                    </div>
                </td>
            </tr>
            <tr style="display:none">
                <td style="background-color:#EFEFEF">Impuesto</td>
                <td><asp:TextBox runat="server" ID="tb_impuesto" Width="70px" Text="0" ReadOnly="True" /></td>
            </tr>
            <tr>
                <td style="background-color:#EFEFEF">Total</td>
                <td>
                    <asp:TextBox runat="server" ID="tb_total" ReadOnly="true" Width="70px" Text="0" />
                </td>
            </tr>
        </table>
    </div>
    <div class="demobottom"></div><br />
    <div aling="center">
        <table width="95%" border="0">
            <tr>
                <td style="width:150px">Observaciones</td>
                <td>
                    <asp:TextBox ID="tb_observaciones" runat="server" TextMode="MultiLine" Height="73px" Width="426px"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_observaciones" runat="server" TargetControlID="tb_observaciones" FilterType="Custom" FilterMode="InvalidChars" InvalidChars="'" />
                </td>
            </tr>
            <tr>
                <td>Encargado Cuarentena</td>
                <td><asp:DropDownList ID="ddl_cuarentena" runat="server" /></td>
            </tr>
            <tr>
                <td>Número de Orden</td>
                <td>
                    <asp:TextBox ID="tb_numero_orden" runat="server" Width="70px"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbe_numero_orden" runat="server" TargetControlID="tb_numero_orden" FilterType="Custom" FilterMode="InvalidChars" InvalidChars="'" />
                </td>
            </tr>
            <tr>
                <td align="left">Fecha de Orden</td>
                <td align="left" style="width: 473px">
                    <asp:TextBox runat="server" ID="tb_fecha_orden" Width="70px" />
                    <asp:ImageButton runat="Server" ID="ib_fecha_orden" ImageUrl="~/images/Calendar_scheduleHS.png" AlternateText="Click to show calendar" /><br />
                    <ajaxToolkit:MaskedEditExtender ID="mee_fecha_orden" runat="server" TargetControlID="tb_fecha_orden" Mask="99/99/9999" MessageValidatorTip="true" CultureName="en-US" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError" MaskType="Date" DisplayMoney="Left" AcceptNegative="Left" ErrorTooltipEnabled="True" />
                    <ajaxToolkit:MaskedEditValidator ID="mev_fecha_orden" runat="server" ControlExtender="mee_fecha_orden" ControlToValidate="tb_fecha_orden" EmptyValueMessage="Fecha de Orden es Requerida" InvalidValueMessage="Fecha de Orden Invalida" Display="Dynamic" TooltipMessage="Ingresar Fecha de Orden" EmptyValueBlurredText="*" InvalidValueBlurredMessage="*" />
                    <ajaxToolkit:CalendarExtender ID="ce_fecha_orden" runat="server" TargetControlID="tb_fecha_orden" PopupButtonID="ib_fecha_orden" Format="MM/dd/yyyy" />
                </td>
            </tr>
            <tr>
                <td align="left">Agregar archivo de Orden</td>
                <td align="left" style="width: 473px">
                    <input type="file" id="fu_archivo" runat="server" accept="image/gif, image/jpeg, image/jpg, image/png, image/pdf"/>
                </td>
            </tr>
            <tr><td colspan="2">&nbsp;</td></tr>
            <tr>
                <td colspan="2" align="center">
                   <div style="display:none;"> 
                    <asp:Button ID="b_grabar" runat="server" Text=" Grabar Certificado " BorderColor="AppWorkspace" Height="40px" OnClick="b_grabar_Click" />
                    <ajaxToolkit:ConfirmButtonExtender ID="cfe_grabar" runat="server" TargetControlID="b_grabar" ConfirmText="" OnClientCancel="cancelClick" ConfirmOnFormSubmit="true" /><br />
                    <asp:Label ID="Label1" runat="server" Visible="true" />
                   </div>
                   <input type="button" id="btGuardar" value=" Grabar Certificado " style="height:40px;" />
                   <input type="button" id="btGuardarCIEX" value=" Grabar Orden de pago CIEX" style="height:40px; display:none" />
                </td>
            </tr>
        </table>
    </div>
    <div id="dvRecargo" title="Recargo por cosumo" style="display:none;">
    <input type="hidden" id="prcuniS" />
        <fieldset>
            <legend>
                Indicar detalles del sobrecargo.
            </legend>
            <table>
                <tr>
                    <td><label>Quimico:</label></td>
                    <td><select id="lsQsobrecargo" style="width:450px"></select></td>
                </tr>
                <tr>
                    <td>
                        <label>Cantidad:</label>
                    </td>
                    <td>
                        <asp:TextBox  runat="server" ID="tb_cExtra" Text="0" Width="70px" />
                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="tb_cExtra" FilterType="Custom, Numbers" ValidChars="." />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label style="font-weight:bolder;">Total</label>
                    </td>
                    <td>
                        <label style="font-weight:bolder;" id="lbTotalC"></label>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>



    <div id="dvSaldoCliente" title="Informacion del cliente" style="display:none;">
    
        <fieldset>
            <legend>
                Saldo pendiente de pago.
            </legend>
            <table>
                <tr>
                    <td><label>Total de constancias pendientes:</label></td>
                    <td><label id="lbCantidadConstancias"></label></td>
                </tr>
                <tr>
                    <td>
                        <label>Saldo pendiente de pago:</label>
                    </td>
                    <td>
                        <label id="lbSaldoPendiente"></label>
                    </td>
                </tr>                
            </table>
        </fieldset>
    </div>



    <div id="dvConfirmacionPago" title="Confirmación de pago" style="display:none;">
    
        <fieldset>
            <legend>
                ¿Confirma que desea imprimir esta constancia?
            </legend>
            <table>
                <tr>
                    <td><label>Constancia No.:</label></td>
                    <td><label id="lbNConstancia"></label></td>
                </tr>
                <tr>
                    <td>
                        <label>Cliente:</label>
                    </td>
                    <td>
                        <label id="lbNombreCliente"></label>
                    </td>
                </tr>                
                <tr>
                    <td>
                        <label>Monto:</label>
                    </td>
                    <td>
                        <label id="lbTotalConstancia"></label>
                    </td>
                </tr>                
                <tr id="trAutorizacionTarjeta">
                    <td>
                        <label>Numero autorizacion pago:</label>
                    </td>
                    <td>
                        <input type="text" id="txtAutorizaTarjeta"/>
                    </td>
                </tr>                
            </table>
        </fieldset>
    </div>
</asp:Content>