Public Class Live

#Region "Variaveis"
    Protected nLiveId As Integer
    Protected sNome As String
    Protected sDescricao As String
    Protected nDataInicio As DateTime
    Protected nDuracao As Integer
    Protected nValorInscricao As Decimal

    Protected oInstrutor As Instrutor
#End Region

#Region "Propriedades"
    Public Property LiveId As Integer
        Get
            Return nLiveId
        End Get
        Set(ByVal value As Integer)
            nLiveId = value
        End Set
    End Property

    Public Property Nome As String
        Get
            Return sNome
        End Get
        Set(ByVal value As String)
            sNome = value
        End Set
    End Property

    Public Property Descricao As String
        Get
            Return sDescricao
        End Get
        Set(ByVal value As String)
            sDescricao = value
        End Set
    End Property

    Public Property DataInicio As Date
        Get
            Return nDataInicio
        End Get
        Set(ByVal value As Date)
            nDataInicio = value
        End Set
    End Property

    Public Property Duracao As Integer
        Get
            Return nDuracao
        End Get
        Set(ByVal value As Integer)
            nDuracao = value
        End Set
    End Property

    Public Property ValorInscricao As Decimal
        Get
            Return nValorInscricao
        End Get
        Set(ByVal value As Decimal)
            nValorInscricao = value
        End Set
    End Property

    Public Property Instrutor As Instrutor
        Get
            Return oInstrutor
        End Get
        Set(ByVal value As Instrutor)
            oInstrutor = value
        End Set
    End Property
#End Region

#Region "Contrutores"
    Public Sub New()
        nLiveId = 0
        sNome = ""
        sDescricao = ""
        nDataInicio = New Date()
        nDuracao = 0
        nValorInscricao = 0
        oInstrutor = New Instrutor()
    End Sub

    Public Sub New(ByVal id As Integer, Optional ByRef transaction As SqlClient.SqlTransaction = Nothing)
        Me.New()

        Dim dt As DataTable = RetDataTableLives($" And LiveId = {id}", transaction)

        If dt.Rows.Count > 0 Then
            Me.LiveId = dt.Rows(0)("Id")
            Me.Nome = dt.Rows(0)("Nome")
            Me.Descricao = dt.Rows(0)("Descricao")
            Me.DataInicio = CDate(dt.Rows(0)("DataInicio"))
            Me.Duracao = dt.Rows(0)("Duracao")
            Me.ValorInscricao = dt.Rows(0)("ValorInscricao")

            If Val(dt.Rows(0)("InstrutorId")) <> 0 Then
                Me.Instrutor = New Instrutor(Val(dt.Rows(0)("InstrutorId")))
            Else
                Me.Instrutor = New Instrutor()
            End If
        End If
    End Sub
#End Region

#Region "Funções"
    Public Shared Function RetDataTableLives(Optional ByVal sFiltro As String = "", Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As DataTable
        Return BDLives.RetDataTable($"Select LiveID as Id, Nome, Descricao, DataInicio, Duracao, ValorInscricao, InstrutorID
                                      From Lives with(nolock) Where 1=1 {sFiltro} ", transaction)
    End Function

    Public Shared Function RetDataTableLivesComInstrutores(Optional ByVal sFiltro As String = "", Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As DataTable
        Return BDLives.RetDataTable($"Select LiveID as Id, Lives.Nome, Descricao, DataInicio, Duracao, ValorInscricao, Lives.InstrutorID, Instrutores.Nome as Instrutor
                                      From Lives with(Nolock) Inner Join Instrutores with(Nolock) on Instrutores.InstrutorID = Lives.InstrutorID
                                      Where 1=1 {sFiltro} ", transaction)
    End Function

    Public Function ValidaSalvar() As Boolean
        If String.IsNullOrEmpty(Me.Nome) Then
            Throw New Exception($"Informe o nome da live.")
            Return False
        End If

        If String.IsNullOrEmpty(Me.Duracao) Then
            Throw New Exception($"Informe a duração da live.")
            Return False
        End If

        If String.IsNullOrEmpty(Me.ValorInscricao) Then
            Throw New Exception($"Informe o valor da inscrição.")
            Return False
        End If

        If Not IsDate(Me.DataInicio) Then
            Throw New Exception($"Informe a data de inicio da live.")
            Return False
        End If

        Return True
    End Function

    Public Function ValidaDelete() As Boolean
        Dim dt As DataTable = BDLives.RetDataTable($"Select LiveId From InscricoesLives Where LiveId = {Me.LiveId}")

        If dt.Rows.Count > 0 Then
            Throw New Exception($"Não foi possivel excluir a live pois a mesma possui inscritos vinculados.")
            Return False
        End If

        Return True
    End Function

    Public Function Salvar(Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As Boolean
        If Not ValidaSalvar() Then
            Return False
        End If

        Dim sSQL As String = ""

        'Utilizado o CONVERT(datetime 'parametro Me.DataInicio' 121), o terceiro parametro 121 é um código de estilo que especifica o formato da data e corresponde ao formato “yyyy-MM-dd HH:mm:ss.fff” no SQL Server
        If Me.LiveId = 0 Then
            sSQL = $"INSERT INTO Lives (Nome,Descricao,DataInicio,Duracao,ValorInscricao,InstrutorID)
             VALUES ('{Me.Nome}', '{Me.Descricao}', CONVERT(datetime, '{Me.DataInicio.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121), {Me.Duracao}, {Me.ValorInscricao}, {Me.Instrutor.InstrutorId} ) "
        Else
            sSQL = $"UPDATE Lives SET Nome = '{Me.Nome}', Descricao = '{Me.Descricao}', DataInicio = CONVERT(datetime, '{Me.DataInicio.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121), Duracao = {Me.Duracao},
            ValorInscricao = {Me.ValorInscricao.ToString().Replace(",", ".")}, InstrutorID = {Me.Instrutor.InstrutorId} WHERE LiveId = {Me.LiveId} "
        End If

        BDLives.ExecutaNonQuery(sSQL, transaction)
        Return True
    End Function

    Public Function Delete(Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As Boolean
        If ValidaDelete() Then
            BDLives.ExecutaNonQuery($"Delete Lives Where LiveId = {Me.LiveId}", transaction)
            Return True
        End If

        Return False
    End Function
#End Region

End Class
