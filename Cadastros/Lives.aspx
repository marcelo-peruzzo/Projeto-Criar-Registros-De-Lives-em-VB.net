<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Lives.aspx.vb" Inherits="ProjetoLives.Lives" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-maskmoney/3.0.2/jquery.maskMoney.min.js"></script>


    <style>
        .paginacao a {
            text-decoration: none;
            color: #808080;
            padding: .25rem;
        }

        .paginacao a:hover {
            color: black;
        }
    </style>

    <script type="text/javascript">
        function formatCurrency(input) {
            // Remove qualquer caractere que não seja dígito ou ponto
            //var value = input.value.replace(/[^0-9.]/g, '');

            // Formata o valor como moeda
            //var formattedValue = parseFloat(value).toFixed(2).replace('.', ',');

            // Atualiza o valor do campo com a versão formatada
            //input.value = 'R$ ' + value;

            console.log(input)
            $('#'+input.name).mask('R$ 9,99', { reverse: true });            
        }
    </script>

    <div class="row p-3">
        <asp:UpdatePanel ID="updFiltros" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="col-12 pb-3 border-bottom">
                    <div class="row">
                        <div class="col-6">
                            <asp:Button ID="btnNovo" runat="server" Text="Novo" CssClass="btn btn-primary" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12">
                            <div class="panel panel-default">
                                <div class="row mt-2">
                                    <div class="col-1">
                                        <span>Código: </span>
                                        <asp:TextBox ID="txtCodigoFiltro" TextMode="Number" runat="server" CssClass="form-control mw-100" MaxLength="200"></asp:TextBox>
                                    </div>
                                    <div class="col-3">
                                        <label>Nome:</label>
                                        <asp:TextBox ID="txtNomeFiltro" runat="server" CssClass="form-control mw-100" MaxLength="200"></asp:TextBox>
                                    </div>
                                    <div class="col-3">
                                        <label>Descrição:</label>
                                        <asp:TextBox ID="txtDescricaoFiltro" runat="server" CssClass="form-control mw-100" MaxLength="200"></asp:TextBox>
                                    </div>
                                    <div class="col-1">
                                        <label>Duração:</label>
                                        <asp:TextBox ID="txtDuracaoFiltro" runat="server" CssClass="form-control mw-100" MaxLength="100"></asp:TextBox>
                                    </div>
                                    <div class="col-4">
                                        <label>Data de Inicio:</label>
                                        <div class="row">
                                            <div class="col-5">
                                                <asp:TextBox ID="txtDataInicioInicialFiltro" runat="server" CssClass="form-control mw-100" type="datetime-local"></asp:TextBox>
                                            </div>
                                            <div class="col-5">
                                                <asp:TextBox ID="txtDataInicioFinalFiltro" runat="server" CssClass="form-control" Type="datetime-local"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-12 pt-3 pb-3 d-flex justify-content-end">
                                        <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" class="btn btn-outline-secondary" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnNovo" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnFiltrar" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <div class="row">
        <div class="col-12">
            <asp:UpdatePanel ID="updGrid" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:GridView ID="grd" runat="server" AutoGenerateColumns="false"
                        DataKeyNames="" EmptyDataText="Nenhum registro encontrado!"
                        PageSize="5" CssClass="table table-striped table-bordered table-hover thead-light"
                        AllowPaging="True">
                        <EmptyDataTemplate>
                            <div class="alert alert-warning"><span class="glyphicon glyphicon-warning-sign"></span>Nenhum registro foi encontrado.</div>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="" HeaderStyle-ForeColor="#007bff" ItemStyle-Font-Size="Small" HeaderStyle-Font-Size="Small">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDetalhes" CommandName="Detalhes" runat="server" Text="Detalhes" CommandArgument='<%#(Eval("id")) %>' CssClass="btn btn-outline-info btn-sm bs-tooltip-left"><i class="fa fa-pencil-square-o"></i></asp:LinkButton>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Código" DataField="ID" />
                            <asp:BoundField HeaderText="Nome" DataField="Nome" />
                            <asp:BoundField HeaderText="Descrição" DataField="Descricao" />
                            <asp:BoundField HeaderText="Data de Inicio" DataField="dataInicio" />        
                             <asp:BoundField HeaderText="Instrutor" DataField="Instrutor" />  
                            <asp:TemplateField HeaderText="" HeaderStyle-ForeColor="#007bff" ItemStyle-Font-Size="Small" HeaderStyle-Font-Size="Small">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkExcluir" CommandName="Excluir" runat="server" Text="Excluir" CommandArgument='<%#(Eval("id")) %>' CssClass="btn btn-outline-danger btn-sm bs-tooltip-left"><i class="fa fa-trash-o"></i></asp:LinkButton>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                            </asp:TemplateField>
                        </Columns>
                        <PagerSettings FirstPageText="«" LastPageText="»" Mode="NumericFirstLast" />
                        <PagerStyle Width="100%" Font-Bold="true" CssClass="paginacao" />
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="modal fade" id="modalCadastro">
        <div class="modal-dialog">
            <div class="modal-content">
                <asp:UpdatePanel ID="updModal" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h4 class="modal-title">Cadastro</h4>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group">
                                <span>Código: </span>
                                <asp:Label ID="lblID" runat="server" />
                            </div>
                            <div class="form-group">
                                <label>Nome:</label>
                                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control mw-100" MaxLength="200"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-5">
                                        <label>Data de Inicio:</label>
                                        <asp:TextBox ID="txtDataInicio" runat="server" CssClass="form-control mw-100" TextMode="DateTimeLocal"></asp:TextBox>
                                    </div>
                                    <div class="col-3">
                                        <label>Duração:</label>
                                        <asp:TextBox ID="txtDuracao" runat="server" CssClass="form-control mw-100" TextMode="Number" MaxLength="200"></asp:TextBox>
                                    </div>
                                    <div class="col-4">
                                        <label>Valor da Inscrição:</label>
                                        <asp:TextBox ID="txtValorInscricao" runat="server" CssClass="form-control mw-100" MaxLength="200" placeholder="R$ 0,00" onkeyup="formatCurrency(this)"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-12">
                                        <span>Instrutor:</span>
                                        <asp:DropDownList ID="cmbInstrutor" runat="server" Width="100%" CssClass="form-control dropdown-toggle"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label>Descrição:</label>
                                <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control mw-100" TextMode="MultiLine" Rows="4" MaxLength="200"></asp:TextBox>
                            </div>                            
                        </div>
                        <div class="modal-footer">
                            <div class="col-12">
                                <div class="row">
                                    <div class="col-6">
                                        <asp:Button ID="btnSalvar" runat="server" Text="Salvar" CssClass="btn btn-outline-success" />
                                    </div>
                                    <div class="col-6 d-flex justify-content-end">
                                        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Fechar</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>
