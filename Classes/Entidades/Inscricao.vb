Public Class Inscricao
#Region "Variaveis"
    Protected nInscricaoId As Integer
    Protected sInscrito As String
    Protected nValorInscricao As Decimal
    Protected nDataVencimento As Date
    Protected nStatusPagamento As Integer


    Protected oLive As Live
    Protected oInscrito As Inscrito

#End Region

#Region "Propriedades"

    Public Property InscricaoId() As String
        Get
            Return nInscricaoId
        End Get
        Set(ByVal value As String)
            nInscricaoId = value
        End Set
    End Property
    Public Property Live As Live
        Get
            Return oLive
        End Get
        Set(ByVal value As Live)
            oLive = value
        End Set
    End Property

    Public Property Inscrito() As Inscrito
        Get
            Return oInscrito
        End Get
        Set(ByVal value As Inscrito)
            oInscrito = value
        End Set
    End Property

    Public Property ValorInscricao() As String
        Get
            Return nValorInscricao
        End Get
        Set(ByVal value As String)
            nValorInscricao = value
        End Set
    End Property

    Public Property DataVencimento() As Date
        Get
            Return nDataVencimento
        End Get
        Set(ByVal value As Date)
            nDataVencimento = value
        End Set
    End Property

    Public Property StatusPagamento() As Integer
        Get
            Return nStatusPagamento
        End Get
        Set(ByVal value As Integer)
            nStatusPagamento = value
        End Set
    End Property
#End Region

#Region "Construtores"
    Public Sub New()
        nInscricaoId = 0
        sInscrito = ""
        nValorInscricao = 0
        nDataVencimento = New Date()
        nStatusPagamento = 0

        oLive = New Live()
        oInscrito = New Inscrito()
    End Sub

    Public Sub New(ByVal id As Integer, Optional ByRef transaction As SqlClient.SqlTransaction = Nothing)
        Me.New()

        Dim dt As DataTable = RetDataTableInscricao($" And InscricaoLiveID = {id}", transaction)

        If dt.Rows.Count > 0 Then
            Me.InscricaoId = dt.Rows(0)("Id")
            Me.ValorInscricao = dt.Rows(0)("ValorInscricao")
            Me.DataVencimento = dt.Rows(0)("DataVencimento")
            Me.StatusPagamento = dt.Rows(0)("StatusPagamento")

            If Val(dt.Rows(0)("LiveID") <> 0) Then
                Me.Live = New Live(Val(dt.Rows(0)("LiveID")))
            Else
                Me.Live = New Live()
            End If
            If Val(dt.Rows(0)("InscritoID") <> 0) Then
                Me.Inscrito = New Inscrito(Val(dt.Rows(0)("InscritoID")))
            Else
                Me.Live = New Live()
            End If
        End If

    End Sub
#End Region

#Region "Funções"
    Public Shared Function RetDataTableInscricao(Optional ByVal sFiltro As String = "", Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As DataTable
        Return BDLives.RetDataTable($"Select InscricaoLiveID as Id, LiveID, InscritoID, inscricoesLives.ValorInscricao, DataVencimento, StatusPagamento
                                      From InscricoesLives with(nolock) Where 1=1 {sFiltro} ", transaction)
    End Function

    Public Shared Function RetDataTableInscricaoComLivesInstrutores(Optional ByVal sFiltro As String = "", Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As DataTable
        Return BDLives.RetDataTable($"SELECT InscricaoLiveID as Id, inscricoesLives.ValorInscricao, DataVencimento,
                                CASE StatusPagamento
                                    WHEN 0 THEN 'Pendente'
                                    WHEN 1 THEN 'Pago'
                                    ELSE 'Desconhecido'
                                END AS StatusPagamento,
                                inscricoesLives.LiveID, Lives.Nome as Lives, inscricoesLives.InscritoID,
                                Inscritos.Nome as Inscritos
                                FROM inscricoesLives with(Nolock)
                                INNER JOIN Lives with(Nolock) ON Lives.LiveID = inscricoesLives.LiveID
                                INNER JOIN Inscritos with(Nolock) ON Inscritos.InscritoID = inscricoesLives.InscritoID
                                WHERE 1=1 {sFiltro}", transaction)
    End Function


    Public Function ValidaSalvar(Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As Boolean
        Dim dt As DataTable = RetDataTableInscricao($" And LiveID = {Me.Live.LiveId} AND InscritoID = {Me.Inscrito.InscritoId}", transaction)

        If Me.Live Is Nothing Then
            Throw New Exception($"Informe uma live.")
            Return False
        End If

        If Me.Inscrito Is Nothing Then
            Throw New Exception($"Informe um inscrito")
        End If

        If String.IsNullOrEmpty(Me.ValorInscricao) Then
            Throw New Exception($"Informe o valor da inscrição.")
            Return False
        End If

        If Not IsDate(Me.DataVencimento) Then
            Throw New Exception($"Informe a data de inicio da live.")
            Return False
        End If

        If dt.Rows.Count > 0 And Me.InscricaoId = 0 Then
            Throw New Exception($"Inscrito já cadastrado para essa live.")
            Return False
        End If

        Return True
    End Function

    Public Function Salvar(Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As Boolean
        If Not ValidaSalvar() Then
            Return False
        End If

        Dim sSql As String = ""

        If Me.InscricaoId = 0 Then
            sSql = $"INSERT INTO inscricoesLives (LiveID, InscritoID, ValorInscricao, DataVencimento, StatusPagamento)
                 VALUES ('{Me.Live.LiveId}', '{Me.Inscrito.InscritoId}', {Me.ValorInscricao.ToString().Replace(",", ".")}, CONVERT(datetime, '{Me.DataVencimento.ToString("yyyy-MM-dd")}', 121), {Me.StatusPagamento}) "
        Else
            sSql = $"UPDATE inscricoesLives SET LiveID = '{Me.Live.LiveId}', InscritoID = '{Me.Inscrito.InscritoId}', ValorInscricao = {Me.ValorInscricao.ToString().Replace(",", ".")}, DataVencimento = CONVERT(datetime, '{Me.DataVencimento.ToString("yyyy-MM-dd")}', 121), StatusPagamento = {Me.StatusPagamento} WHERE InscricaoLiveID = {Me.InscricaoId}"
        End If

        BDLives.ExecutaNonQuery(sSql, transaction)
        Return True
    End Function

    Public Function ValidaDelete() As Boolean
        'Dim dt As DataTable = BDLives.RetDataTable($"Select LiveId From InscricoesLives Where LiveId = {Me.InscricaoId}")

        If Me.StatusPagamento = 1 Then
            Throw New Exception($"Não foi possivel excluir a inscrição pois a mesma já foi paga.")
            Return False
        End If

        Return True
    End Function

    Public Function Delete(Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As Boolean
        If ValidaDelete() Then
            BDLives.ExecutaNonQuery($"Delete InscricoesLives Where InscricaoLiveID = {Me.InscricaoId}", transaction)
            Return True
        End If

        Return False
    End Function
#End Region
End Class
