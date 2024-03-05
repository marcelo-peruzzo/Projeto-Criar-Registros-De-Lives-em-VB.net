Public Class Inscrito

#Region "Variaveis"
    Protected nInscritoId As Integer
    Protected sNome As String
    Protected nDataNascimento As Date
    Protected sEnderecoInstagram As String
#End Region

#Region "Propriedades"
    Public Property InscritoId() As Integer
        Get
            Return nInscritoId
        End Get
        Set(ByVal value As Integer)
            nInscritoId = value
        End Set
    End Property

    Public Property Nome() As String
        Get
            Return sNome
        End Get
        Set(ByVal value As String)
            sNome = value
        End Set
    End Property

    Public Property DataNascimento() As Date
        Get
            Return nDataNascimento
        End Get
        Set(ByVal value As Date)
            nDataNascimento = value
        End Set
    End Property

    Public Property EnderecoInstagram() As String
        Get
            Return sEnderecoInstagram
        End Get
        Set(ByVal value As String)
            sEnderecoInstagram = value
        End Set
    End Property
#End Region

#Region "Contrutores"
    Public Sub New()
        nInscritoId = 0
        sNome = ""
        nDataNascimento = New Date()
        sEnderecoInstagram = ""
    End Sub

    Public Sub New(ByVal id As Integer, Optional ByRef transaction As SqlClient.SqlTransaction = Nothing)
        Me.New()

        Dim dt As DataTable = RetDataTableInscritos($" And InscritoId = {id}", transaction)

        If dt.Rows.Count > 0 Then
            Me.nInscritoId = dt.Rows(0)("Id")
            Me.Nome = dt.Rows(0)("Nome")
            Me.EnderecoInstagram = dt.Rows(0)("EnderecoInstagram")
            Me.DataNascimento = CDate(dt.Rows(0)("DataNascimento"))
        End If

    End Sub
#End Region

#Region "Funções"
    Protected Function ValidaSalvar() As Boolean
        If String.IsNullOrEmpty(Me.Nome) Then
            Throw New Exception("Informe o nome do Inscrito.")
            Return False
        End If

        Return True
    End Function

    Public Shared Function RetDataTableInscritos(Optional ByVal sFiltro As String = "",
                                                    Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As DataTable
        Return BDLives.RetDataTable($"SELECT  InscritoId AS ID,  
                                              Nome,  
                                              DataNascimento, 
                                              EnderecoInstagram 
                                      FROM Inscritos WITH(NOLOCK) Where 1=1 {sFiltro}", transaction)
    End Function

    Protected Function ValidaDelete(Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As Boolean
        Dim dt As DataTable = BDLives.RetDataTable($"Select InscritoId 
               From InscricoesLives with(nolock) Where InscritoId = {Me.nInscritoId};")

        If dt.Rows.Count > 0 Then
            Throw New Exception($"Inscrito esta vinculado a {dt.Rows.Count} lives.")
            Return False
        End If

        Return True
    End Function

    Public Function Salvar(Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As Boolean
        If Not ValidaSalvar() Then
            Return False
        End If

        Dim sSQL As String = ""

        If Me.nInscritoId = 0 Then
            sSQL = $"Insert Into Inscritos (Nome, DataNascimento, EnderecoInstagram) Values ('{Me.Nome}', CONVERT(datetime, '{Me.DataNascimento.ToString("yyyy-MM-dd")}', 121), '{Me.EnderecoInstagram}')"
        Else
            sSQL = $"Update Inscritos Set Nome = '{Me.Nome}', DataNascimento = '{Me.DataNascimento.ToString("yyyy-MM-dd")}', EnderecoInstagram = '{Me.EnderecoInstagram}' Where InscritoId = {Me.InscritoId}"
        End If

        BDLives.ExecutaNonQuery(sSQL, transaction)
        Return True
    End Function

    Public Function Delete(Optional ByRef transaction As SqlClient.SqlTransaction = Nothing) As Boolean
        If ValidaDelete() Then
            BDLives.ExecutaNonQuery($"Delete Inscritos Where InscritoId = {Me.nInscritoId}", transaction)
            Return True
        End If

        Return False
    End Function
#End Region
End Class
