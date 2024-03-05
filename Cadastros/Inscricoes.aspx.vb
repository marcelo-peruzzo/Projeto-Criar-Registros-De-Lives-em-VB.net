Imports Newtonsoft.Json

Public Class Inscricoes
    Inherits System.Web.UI.Page

    Private oInscricoes As Inscricao

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            CarregaGrid()
            CarregaCombos()
        End If
    End Sub

    Private Sub SalvarNoViewState()
        Dim json = JsonConvert.SerializeObject(oInscricoes)
        ViewState.Add("oInscricoes", json)
    End Sub

    Private Function RecuperaDoViewState() As Inscricao
        Dim json As String = ViewState("oInscricoes")
        Return JsonConvert.DeserializeObject(Of Inscricao)(json)
    End Function
#Region "Funções"


    Private Sub CarregaGrid()
        grd.DataSource = Inscricao.RetDataTableInscricaoComLivesInstrutores(CarregaFiltro())
        grd.DataBind()
        updGrid.Update()
    End Sub

    Private Function CarregaFiltro() As String
        Dim sFiltro As String = ""

        If Not String.IsNullOrEmpty(txtCodigoFiltro.Text) Then
            sFiltro &= $" And inscricoesLives.InscricaoLiveID = {txtCodigoFiltro.Text} "
        End If

        If Not String.IsNullOrEmpty(txtLiveFiltro.Text) Then
            sFiltro &= $" And Lives.Nome LIKE '%{txtLiveFiltro.Text}%' "
        End If

        If Not String.IsNullOrEmpty(txtInscritoFiltro.Text) Then
            sFiltro &= $" And Inscritos.Nome LIKE '%{txtInscritoFiltro.Text}%' "
        End If

        If Not String.IsNullOrEmpty(txtDataVencimentoFiltro.Text) AndAlso Not String.IsNullOrEmpty(txtDataVencimentoFiltro.Text) Then
            sFiltro &= $" And InscricoesLives.DataVencimento = '{CDate(txtDataVencimentoFiltro.Text).ToString("dd-MM-yyy")}'"
        End If

        Return sFiltro
    End Function

    Private Sub CarregaCombos()
        cmbLive.DataSource = Live.RetDataTableLives()
        cmbLive.DataTextField = "Nome"
        cmbLive.DataValueField = "Id"
        cmbLive.DataBind()
        cmbLive.Items.Insert(0, New ListItem("(Selecione)", 0))

        cmbInscrito.DataSource = Inscrito.RetDataTableInscritos()
        cmbInscrito.DataTextField = "Nome"
        cmbInscrito.DataValueField = "Id"
        cmbInscrito.DataBind()
        cmbInscrito.Items.Insert(0, New ListItem("(Selecione)", 0))

    End Sub

    'Metodo para atribuir o valor da inscrição confrome o valor já cadastrado sobre a live selecionada
    Protected Sub RetornaValorComboLive(sender As Object, e As EventArgs)
        'obtem o ID da live selecionada
        Dim liveId As Integer = Convert.ToInt32(cmbLive.SelectedValue)

        'consulta o banco de dados para obter o valor da inscrição da live selecionada
        Dim dt As DataTable = Live.RetDataTableLives($" AND LiveID = {liveId}")
        If dt.Rows.Count > 0 Then
            Dim valorInscricao As Decimal = Convert.ToDecimal(dt.Rows(0)("ValorInscricao"))

            'atualiza o valor da caixa de texto txtValorInscricao com o valor da inscriçao da live selecionad
            txtValorInscricao.Text = valorInscricao.ToString("C")
        End If
    End Sub


    Private Sub CarregaModal(Optional ByVal Id As Integer = 0)

        If Id = 0 Then
            oInscricoes = New Inscricao()
            oInscricoes.Live = New Live()
            oInscricoes.Inscrito = New Inscrito()
        Else
            oInscricoes = New Inscricao(Id)
            oInscricoes.Live = New Live(oInscricoes.Live.LiveId)
            oInscricoes.Inscrito = New Inscrito(oInscricoes.Inscrito.InscritoId)
        End If

        lblID.Text = oInscricoes.InscricaoId
        txtValorInscricao.Text = IIf(oInscricoes.ValorInscricao = 0, "", FormatNumber(oInscricoes.ValorInscricao, 2))

        cmbLive.SelectedValue = oInscricoes.Live.LiveId
        cmbInscrito.SelectedValue = oInscricoes.Inscrito.InscritoId
        rblOpcao.SelectedValue = oInscricoes.StatusPagamento


        If oInscricoes.InscricaoId <> 0 Then
            txtDataVencimento.Text = oInscricoes.DataVencimento.ToString("yyyy-MM-dd")
        Else
            txtDataVencimento.Text = ""
        End If

        SalvarNoViewState()
        updModal.Update()
        ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "openModalCadastro", "$('#modalCadastro').modal('show');", True)
    End Sub

    Private Sub SalvarInscricao()
        Dim transaction As SqlClient.SqlTransaction
        Try
            oInscricoes = RecuperaDoViewState()

            oInscricoes.Live.LiveId = cmbLive.SelectedValue
            oInscricoes.Inscrito.InscritoId = cmbInscrito.SelectedValue
            oInscricoes.ValorInscricao = txtValorInscricao.Text
            oInscricoes.DataVencimento = txtDataVencimento.Text
            oInscricoes.StatusPagamento = rblOpcao.SelectedValue


            If IsDate(txtDataVencimento.Text) Then
                oInscricoes.DataVencimento = CType(txtDataVencimento.Text, Date)
            End If

            transaction = BDLives.BeginTransaction

            If oInscricoes.Salvar(transaction) Then
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

    Private Sub ExcluirLive(ByVal id As Integer)
        Dim transaction As SqlClient.SqlTransaction = BDLives.BeginTransaction

        Try
            transaction = BDLives.BeginTransaction

            oInscricoes = New Inscricao(id)
            oInscricoes.Delete(transaction)

            transaction.Commit()

            CarregaGrid()
        Catch ex As Exception
            transaction.Rollback()
            ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "msgSucessoSalva", $"mostraAviso('erro','Não foi possivel excluir a live {ex.Message}');", True)
        Finally
            transaction.Dispose()
        End Try
    End Sub

#End Region

    Private Sub btnNovo_Click(sender As Object, e As EventArgs) Handles btnNovo.Click
        CarregaModal()
    End Sub

    Private Sub btnSalvar_Click(sender As Object, e As EventArgs) Handles btnSalvar.Click
        SalvarInscricao()
    End Sub

    Private Sub grd_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grd.RowCommand
        If e.CommandName.Equals("Detalhes") Then
            CarregaModal(e.CommandArgument)
        ElseIf e.CommandName.Equals("Excluir") Then
            ExcluirLive(e.CommandArgument)
        End If
    End Sub

    Private Sub btnFiltrar_Click(sender As Object, e As EventArgs) Handles btnFiltrar.Click
        CarregaGrid()
    End Sub

End Class