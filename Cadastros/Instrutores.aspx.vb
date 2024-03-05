Public Class Instrutores
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            CarregaGrid()
        End If
    End Sub

#Region "Funções"
    Public Sub CarregaGrid()
        Dim dtGrid As DataTable = BDLives.RetDataTable($"SELECT  InstrutorID AS ID,  Nome,  DataNascimento, EnderecoInstagram FROM Instrutores WITH(NOLOCK) Where 1=1 {CarregaFiltro()}")

        grd.DataSource = dtGrid
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

        If Not String.IsNullOrEmpty(txtInstagramFiltro.Text) Then sFiltro &= $" And Instagram like '%{txtInstagramFiltro.Text}%' "

        Return sFiltro
    End Function

    Private Sub CarregaModalDetalhes(Optional ByVal Id As Integer = 0)
        If Id = 0 Then
            lblID.Text = ""
            txtNome.Text = ""
            txtDataNascimento.Text = ""
            txtInstagram.Text = ""
        Else
            Dim dt As DataTable = BDLives.RetDataTable($"SELECT  InstrutorID AS ID,  Nome,  FORMAT(DataNascimento, 'yyyy-MM-dd') as DataNascimento, EnderecoInstagram FROM Instrutores WITH(NOLOCK) Where InstrutorID = {Id}")

            If dt.Rows.Count = 0 Then Exit Sub

            lblID.Text = dt.Rows(0)("ID")
            txtNome.Text = dt.Rows(0)("Nome")
            txtDataNascimento.Text = dt.Rows(0)("DataNascimento")
            txtInstagram.Text = dt.Rows(0)("EnderecoInstagram")
        End If

        updModal.Update()
        ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "fecharModalCadastro", "$('#modalCadastro').modal('show');", True)
    End Sub

    Private Sub SalvarInstrutor()
        If Not ValidaCampos() Then Exit Sub

        Dim transaction As SqlClient.SqlTransaction
        Dim sSQL As String = ""

        If Val(lblID.Text) = 0 Then
            sSQL = $"Insert Into Instrutores (Nome, DataNascimento, EnderecoInstagram) Values ('{txtNome.Text.Trim}', '{CDate(txtDataNascimento.Text)}', '{txtInstagram.Text.Trim}')"
        Else
            sSQL = $"Update Instrutores Set Nome = '{txtNome.Text.Trim}', DataNascimento = '{CDate(txtDataNascimento.Text)}', EnderecoInstagram = '{txtInstagram.Text.Trim}' Where InstrutorID = {Val(lblID.Text)}"
        End If

        Try
            transaction = BDLives.BeginTransaction
            BDLives.ExecutaNonQuery(sSQL, transaction)
            transaction.Commit()

            CarregaGrid()
            ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "fecharModalCadastro", "$('#modalCadastro').modal('hide');", True)
        Catch ex As Exception
            transaction.Rollback()
        Finally
            transaction.Dispose()
        End Try
    End Sub

    Private Function ValidaCampos() As Boolean
        If String.IsNullOrEmpty(txtNome.Text) Then Return False

        If String.IsNullOrEmpty(txtDataNascimento.Text) Then Return False

        If String.IsNullOrEmpty(txtInstagram.Text) Then Return False

        If Val(lblID.Text) = 0 AndAlso BDLives.RetDataTable($"Select InstrutorId From Instrutores with(nolock) Where EnderecoInstagram = '{txtInstagram.Text}'").Rows.Count > 0 Then Return False

        Return True
    End Function

    Private Sub ExcluirInstrutor(ByVal id As Integer)
        Dim transaction As SqlClient.SqlTransaction = BDLives.BeginTransaction

        Dim sSQL As String = $"Delete Instrutores Where InstrutorId = {id}"

        Try
            transaction = BDLives.BeginTransaction
            BDLives.ExecutaNonQuery(sSQL, transaction)
            transaction.Commit()

            CarregaGrid()
        Catch ex As Exception
            transaction.Rollback()
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