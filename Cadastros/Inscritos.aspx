<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Inscritos.aspx.vb" Inherits="ProjetoLives.Inscritos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
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
                                    <div class="col-2">
                                        <label>Instagram:</label>
                                        <asp:TextBox ID="txtInstagramFiltro" runat="server" CssClass="form-control mw-100" MaxLength="100"></asp:TextBox>
                                    </div>
                                    <div class="col-4">
                                        <label>Data Nascimento Entre:</label>
                                        <div class="row">
                                            <div class="col-5">
                                                <asp:TextBox ID="txtDataNascimentoInicialFiltro" runat="server" CssClass="form-control mw-100" Type="Date"></asp:TextBox>
                                            </div>                                            
                                            <div class="col-5">
                                                <asp:TextBox ID="txtDataNascimentoFinalFiltro" runat="server" CssClass="form-control" Type="Date"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>                                                                                      
                                    <div class="col-2 pt-3 pb-3 d-flex justify-content-end">
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
                            <asp:BoundField HeaderText="Data Nascimento" DataField="DataNascimento" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField HeaderText="Instagram" DataField="EnderecoInstagram" />
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
                                <label>Data Nascimento:</label>
                                <asp:TextBox ID="txtDataNascimento" runat="server" CssClass="form-control mw-100" Type="Date"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>Instagram:</label>
                                <asp:TextBox ID="txtInstagram" runat="server" CssClass="form-control mw-100" MaxLength="100"></asp:TextBox>
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