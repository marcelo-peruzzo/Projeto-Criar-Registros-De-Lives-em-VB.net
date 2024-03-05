Imports Newtonsoft.Json

Public Class Inscritos
    Inherits System.Web.UI.Page

    Private oInscrito As Inscrito

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            CarregaGrid()
        End If
    End Sub


    Private Sub SalvarNoViewState()
        Dim json = JsonConvert.SerializeObject(oInscrito)
        ViewState.Add("oInscrito", json)
    End Sub

    Private Function RecuperaDoViewState() As Inscrito
        Dim json As String = ViewState("oInscrito")
        Return JsonConvert.DeserializeObject(Of Inscrito)(json)
    End Function

#Region "Funções"
    Public Sub CarregaGrid()
        grd.DataSource = Inscrito.RetDataTableInscritos(CarregaFiltro())
        grd.DataBind()
        updGrid.Update()
    End Sub

    Private Function CarregaFiltro() As String
        Dim sFiltro As String = ""

        If Not String.IsNullOrEmpty(txtCodigoFiltro.Text) Then sFiltro &= $" And InstrutorID = {Val(txtCodigoFiltro.Text)} "

        If Not String.IsNullOrEmpty(txtNomeFiltro.Text) Then sFiltro &= $" And Nome like '%{txtNomeFiltro.Text}%' "

        If Not String.IsNullOrEmpty(txtDataNascimentoInicialFiltro.Text) AndAlso Not String.IsNullOrEmpty(txtDataNascimentoFinalFiltro.Text) Then
            sFiltro &= $" And DataNascimento between '{CDate(txtDataNascimentoInicialFiltro.Text).ToString("yyyy-MM-dd HH:mm:ss")}' And '{CDate(txtDataNascimentoFinalFiltro.Text).ToString("yyyy-MM-dd HH:mm:ss")}'"
        End If

        If Not String.IsNullOrEmpty(txtInstagramFiltro.Text) Then sFiltro &= $" And EnderecoInstagram like '%{txtInstagramFiltro.Text}%' "

        Return sFiltro
    End Function

    Private Sub CarregaModalDetalhes(Optional ByVal Id As Integer = 0)
        If Id = 0 Then
            oInscrito = New Inscrito()
        Else
            oInscrito = New Inscrito(Id)
        End If

        lblID.Text = oInscrito.InscritoId
        txtNome.Text = oInscrito.Nome
        txtInstagram.Text = oInscrito.EnderecoInstagram

        If oInscrito.InscritoId <> 0 Then
            txtDataNascimento.Text = oInscrito.DataNascimento.ToString("yyyy-MM-dd")
        Else
            txtDataNascimento.Text = ""
        End If

        SalvarNoViewState()

        updModal.Update()
        ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "fecharModalCadastro", "$('#modalCadastro').modal('show');", True)
    End Sub

    Private Sub SalvarInstrutor()
        Dim transaction As SqlClient.SqlTransaction
        Try
            oInscrito = RecuperaDoViewState()

            oInscrito.Nome = txtNome.Text
            oInscrito.EnderecoInstagram = txtInstagram.Text

            If IsDate(txtDataNascimento.Text) Then
                oInscrito.DataNascimento = CDate(txtDataNascimento.Text)
            End If

            transaction = BDLives.BeginTransaction

            If oInscrito.Salvar(transaction) Then
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

    Private Sub ExcluirInstrutor(ByVal id As Integer)
        Dim transaction As SqlClient.SqlTransaction = BDLives.BeginTransaction

        Try
            transaction = BDLives.BeginTransaction

            oInscrito = New Inscrito(id)
            oInscrito.Delete(transaction)

            transaction.Commit()

            CarregaGrid()
        Catch ex As Exception
            transaction.Rollback()
            ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "msgSucessoSalva", $"mostraAviso('erro','Não foi possivel salvar o inscrito/n{ex.Message}');", True)
        Finally
            transaction.Dispose()
        End Try
    End Sub
#End Region

#Region "Ações"
    Private Sub btnSalvar_Click(sender As Object, e As EventArgs) Handles btnSalvar.Click
        SalvarInstrutor()
    End Sub

    Private Sub grd_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grd.PageIndexChanging
        grd.PageIndex = e.NewPageIndex
        CarregaGrid()
    End Sub

    Private Sub btnNovo_Click(sender As Object, e As EventArgs) Handles btnNovo.Click
        CarregaModalDetalhes()
    End Sub

    Private Sub grd_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grd.RowCommand
        If e.CommandName.Equals("Detalhes") Then
            CarregaModalDetalhes(e.CommandArgument)
        ElseIf e.CommandName.Equals("Excluir") Then
            ExcluirInstrutor(e.CommandArgument)
        End If
    End Sub

    Private Sub btnFiltrar_Click(sender As Object, e As EventArgs) Handles btnFiltrar.Click
        CarregaGrid()
    End Sub


#End Region

End Class