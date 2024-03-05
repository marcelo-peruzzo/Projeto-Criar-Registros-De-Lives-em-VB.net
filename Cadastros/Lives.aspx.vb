Imports Newtonsoft.Json

Public Class Lives
    Inherits System.Web.UI.Page

    Private oLive As Live

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            CarregaGrid()
            CarregaCombo()
        End If
    End Sub

    Private Sub SalvarNoViewState()
        Dim json = JsonConvert.SerializeObject(oLive)
        ViewState.Add("oLive", json)
    End Sub

    Private Function RecuperaDoViewState() As Live
        Dim json As String = ViewState("oLive")
        Return JsonConvert.DeserializeObject(Of Live)(json)
    End Function

#Region "Funções"
    Private Sub CarregaGrid()
        grd.DataSource = Live.RetDataTableLivesComInstrutores(CarregaFiltro())
        grd.DataBind()
        updGrid.Update()
    End Sub

    Private Function CarregaFiltro() As String
        Dim sFiltro As String = ""

        If Not String.IsNullOrEmpty(txtCodigoFiltro.Text) Then
            sFiltro &= $" And Lives.LiveId = {txtCodigoFiltro.Text} "
        End If

        If Not String.IsNullOrEmpty(txtDescricaoFiltro.Text) Then
            sFiltro &= $" And Lives.Descricao like '%{txtDescricaoFiltro.Text}%' "
        End If

        If Not String.IsNullOrEmpty(txtDuracaoFiltro.Text) Then
            sFiltro &= $" And Lives.LiveId = {txtDuracaoFiltro.Text} "
        End If

        If Not String.IsNullOrEmpty(txtDataInicioInicialFiltro.Text) AndAlso Not String.IsNullOrEmpty(txtDataInicioFinalFiltro.Text) Then
            sFiltro &= $" And Lives.DataInicio Between {txtDataInicioInicialFiltro.Text} and {txtDataInicioFinalFiltro.Text} "
        End If

        Return sFiltro
    End Function

    Private Sub CarregaCombo()
        cmbInstrutor.DataSource = Instrutor.RetDataTableInstrutor()
        cmbInstrutor.DataTextField = "Nome"
        cmbInstrutor.DataValueField = "Id"
        cmbInstrutor.DataBind()
        cmbInstrutor.Items.Insert(0, New ListItem("(Selecione)", 0))
    End Sub

    Private Sub CarregaModalDetalhes(Optional ByVal id As Integer = 0)
        If id = 0 Then
            oLive = New Live()
            oLive.Instrutor = New Instrutor()
        Else
            oLive = New Live(id)
            oLive.Instrutor = New Instrutor(oLive.Instrutor.InstrutorId)
        End If


        lblID.Text = oLive.LiveId
        txtNome.Text = oLive.Nome
        txtDescricao.Text = oLive.Descricao
        txtDuracao.Text = IIf(oLive.Duracao = 0, "", oLive.Duracao)
        txtValorInscricao.Text = IIf(oLive.ValorInscricao = 0, "", FormatNumber(oLive.ValorInscricao, 2))

        cmbInstrutor.SelectedValue = oLive.Instrutor.InstrutorId

        If oLive.LiveId <> 0 Then
            txtDataInicio.Text = oLive.DataInicio.ToString("yyyy-MM-dd hh:mm")
        Else
            txtDataInicio.Text = ""
        End If

        SalvarNoViewState()

        updModal.Update()
        ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "openModalCadastro", "$('#modalCadastro').modal('show');", True)
    End Sub

    Private Sub ExcluirLive(ByVal id As Integer)
        Dim transaction As SqlClient.SqlTransaction = BDLives.BeginTransaction

        Try
            transaction = BDLives.BeginTransaction

            oLive = New Live(id)
            oLive.Delete(transaction)

            transaction.Commit()

            CarregaGrid()
        Catch ex As Exception
            transaction.Rollback()
            ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "msgSucessoSalva", $"mostraAviso('erro','Não foi possivel excluir a live/n{ex.Message}');", True)
        Finally
            transaction.Dispose()
        End Try
    End Sub

    Private Sub SalvarLive()
        Dim transaction As SqlClient.SqlTransaction
        Try
            oLive = RecuperaDoViewState()

            oLive.Nome = txtNome.Text
            oLive.Descricao = txtDescricao.Text
            oLive.Duracao = txtDuracao.Text
            oLive.ValorInscricao = txtValorInscricao.Text

            oLive.Instrutor.InstrutorId = cmbInstrutor.SelectedValue

            If IsDate(txtDataInicio.Text) Then
                oLive.DataInicio = CType(txtDataInicio.Text, DateTime)
            End If

            transaction = BDLives.BeginTransaction

            If oLive.Salvar(transaction) Then
                ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "msgSucessoSalva", "mostraAviso('sucesso','Resgitro salvo com sucesso.');", True)
            End If

            transaction.Commit()

            CarregaGrid()
            ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "fecharModalCadastro", "$('#modalCadastro').modal('hide');", True)
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "msgSucessoSalva", $"mostraAviso('erro','{ex.Message}');", True)
            transaction.Rollback()
        Finally
            transaction.Dispose()
        End Try
    End Sub
#End Region

#Region "Ações"
    Private Sub btnNovo_Click(sender As Object, e As EventArgs) Handles btnNovo.Click
        CarregaModalDetalhes()
    End Sub

    Private Sub btnFiltrar_Click(sender As Object, e As EventArgs) Handles btnFiltrar.Click
        CarregaGrid()
    End Sub

    Private Sub grd_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grd.PageIndexChanging
        grd.PageIndex = e.NewPageIndex
        CarregaGrid()
    End Sub

    Private Sub grd_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grd.RowCommand
        If e.CommandName.Equals("Detalhes") Then
            CarregaModalDetalhes(e.CommandArgument)
        ElseIf e.CommandName.Equals("Excluir") Then
            ExcluirLive(e.CommandArgument)
        End If
    End Sub

    Private Sub btnSalvar_Click(sender As Object, e As EventArgs) Handles btnSalvar.Click
        SalvarLive()
    End Sub

#End Region
End Class