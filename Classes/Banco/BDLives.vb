Imports System.Data.Common
Imports System.Data.SqlClient
Imports Microsoft.Practices.EnterpriseLibrary.Data
Imports Microsoft.Practices.EnterpriseLibrary.Data.Sql

Public Class BDLives
    Protected Const nTempoTimeout As Integer = 300

    Private Shared con As New SqlConnection()
    Private Shared sConncetionString As String = ""

    Public Shared Property ConnectionString() As String
        Get
            Return sConncetionString
        End Get
        Set(value As String)
            If sConncetionString <> value Then
                Try
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                Catch ex As Exception

                End Try
            End If

            sConncetionString = value
        End Set
    End Property

    Public Shared ReadOnly Property Connection() As SqlConnection
        Get
            If con.State <> ConnectionState.Open Then
                con.ConnectionString = sConncetionString
                con.Open()
            End If

            Return con
        End Get
    End Property

#Region "Publico"
    Public Shared Function getDataBase() As SqlDatabase
        Return DirectCast(DatabaseFactory.CreateDatabase("ProjetoLives"), SqlDatabase)
    End Function

    Public Overloads Shared Function RetDataTable(ByVal sSql As String, Optional ByRef transaction As SqlTransaction = Nothing)
        Dim bd As SqlDatabase
        Dim cmd As DbCommand

        Try
            If transaction Is Nothing Then
                bd = getDataBase()
                cmd = bd.GetSqlStringCommand(sSql)
                cmd.CommandTimeout = nTempoTimeout

                Return bd.ExecuteDataSet(cmd).Tables(0)
            Else
                bd = New SqlDatabase(transaction.Connection.ConnectionString)
                cmd = transaction.Connection.CreateCommand
                cmd.CommandType = CommandType.Text
                cmd.CommandText = sSql
                cmd.CommandTimeout = nTempoTimeout

                Return bd.ExecuteDataSet(cmd, transaction).Tables(0)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Overloads Shared Function ExecutaNonQuery(ByVal sSQL As String, Optional ByRef transaction As SqlTransaction = Nothing) As Integer
        Try
            Dim db As SqlDatabase '= getDatabase()
            Dim cmd As DbCommand

            If transaction Is Nothing Then
                db = getDataBase()
                cmd = db.GetSqlStringCommand(sSQL)
                cmd.CommandTimeout = nTempoTimeout

                Return db.ExecuteNonQuery(cmd)
            Else
                db = New Microsoft.Practices.EnterpriseLibrary.Data.Sql.SqlDatabase(transaction.Connection.ConnectionString)
                cmd = transaction.Connection.CreateCommand
                cmd.CommandType = CommandType.Text
                cmd.CommandText = sSQL
                cmd.CommandTimeout = nTempoTimeout

                Return db.ExecuteNonQuery(cmd, transaction)
            End If
        Catch ex As SqlException
            If ex.Number = 1205 Then
                Throw New Exception("Não foi possível concluir a operação. Por favor, tente novamente.")
            Else
                Throw ex
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Shared Function BeginTransaction() As SqlTransaction
        Dim connection As DbConnection = getDataBase.CreateConnection

        connection.Open()

        Return CType(connection.BeginTransaction, SqlTransaction)
    End Function
#End Region
End Class
