﻿Imports Monkeyspeak
Imports SilverMonkey.ErrorLogging
Imports SilverMonkey.TextBoxWriter

Imports System.Diagnostics
Imports System.Collections
Imports System.Collections.Generic

Public Class MS_Time
    Inherits Monkeyspeak.Libraries.AbstractBaseLibrary
    Private writer As TextBoxWriter = Nothing
    Dim lock As New Object
    Public Sub New()
        writer = New TextBoxWriter(Variables.TextBox1)


        '(0:299) When the time is {...} hh:mm:ss am/pm FST,
        Add(Monkeyspeak.TriggerCategory.Cause, 299,
Function(reader As TriggerReader)
    Dim Time As String = reader.ReadString
    Dim str As String = ""
    SyncLock lock
        str = Main.FurcTime.ToLongTimeString
    End SyncLock
    Return str.ToUpper = Time.ToUpper
End Function,
"(0:299) When the time is {...} hh:mm:ss am/pm FST,")

        '(5:30) set variable %Variable to the current local time.
        Add(Monkeyspeak.TriggerCategory.Effect, 30,
AddressOf CurrentTime, "(5:30) set variable %Variable to the current local time.")

        '(5:31) set variable %Variable to the current Furcadia Standard time
        Add(Monkeyspeak.TriggerCategory.Effect, 31,
AddressOf FurcTime, "(5:31) set variable %Variable to the current Furcadia Standard time.")
        '(5:32) set variable %Variable to current DateTime
        Add(Monkeyspeak.TriggerCategory.Effect, 32,
AddressOf LocalDateTimeVar, "(5:32) set variable %Variable to current local DateTime.")
        '(5:33) set variable %Variable to current Furcadia DateTime
        Add(Monkeyspeak.TriggerCategory.Effect, 33,
AddressOf FurcDateTimeVar, "(5:33) set variable %Variable to current Furcadia DateTime.")
        '(5:34) use variable %Variable as a DateTime string and subtract Date Time string {...} and put it into variable %Variable
        Add(Monkeyspeak.TriggerCategory.Effect, 34,
AddressOf SubsractDateTimeStr, "(5:34) use variable %Variable as a DateTime string and subtract Date Time string {...} and put it into variable %Variable.")
        '(5:35) use variable %Variable as a DateTime string and subtract Date Time variable %Variable and put it into variable %Variable
        Add(Monkeyspeak.TriggerCategory.Effect, 35,
AddressOf SubsractDateTimeVar, "(5:35) use variable %Variable as a DateTime string and subtract Date Time variable %Variable and put it into variable %Variable.")
        '(5:36) use variable %Variable as a DateTime string and add Date Time string {...} and put it into variable %Variable
        '        Add(Monkeyspeak.TriggerCategory.Effect, 36,
        'AddressOf AddDateTimeStr, "(5:36) use variable %Variable as a DateTime string and add Date Time string {...} and put it into variable %Variable")
        '        '(5:37) use variable %Variable as a DateTime string and add Date Time variable %Variable and put it into variable %Variable
        '        Add(Monkeyspeak.TriggerCategory.Effect, 37,
        'AddressOf AddDateTimeVar, "(5:37) use variable %Variable as a DateTime string and add Date Time variable %Variable and put it into variable %Variable")
        '   
    End Sub

    Function CurrentTime(reader As TriggerReader) As Boolean

        Try
            Dim Var As Monkeyspeak.Variable = reader.ReadVariable(True)
            Dim Str As String = DateTime.Now.ToLongTimeString.ToLower
            Var.Value = Str
            Return True
        Catch ex As Exception
            Dim tID As String = reader.TriggerId.ToString
            Dim tCat As String = reader.TriggerCategory.ToString
            Console.WriteLine(MS_ErrWarning)
            Dim ErrorString As String = "Error: (" & tCat & ":" & tID & ") " & ex.Message
            writer.WriteLine(ErrorString)
            Debug.Print(ErrorString)
            Return False
        End Try
    End Function

    Function FurcTime(reader As TriggerReader) As Boolean

        Try
            Dim Var As Monkeyspeak.Variable = reader.ReadVariable(True)
            Dim Str As String = ""
            SyncLock lock
                Str = Main.FurcTime.ToLongTimeString.ToLower
            End SyncLock
            Var.Value = Str
            Return True
        Catch ex As Exception
            Dim tID As String = reader.TriggerId.ToString
            Dim tCat As String = reader.TriggerCategory.ToString
            Console.WriteLine(MS_ErrWarning)
            Dim ErrorString As String = "Error: (" & tCat & ":" & tID & ") " & ex.Message
            writer.WriteLine(ErrorString)
            Debug.Print(ErrorString)
            Return False
        End Try
    End Function

    '(5: ) set variable %Variable to current DateTime
    Public Function LocalDateTimeVar(reader As Monkeyspeak.TriggerReader) As Boolean
        Try
            Dim var As Monkeyspeak.Variable = reader.ReadVariable(True)
            var.Value = Date.Now.ToString("yyyy-MM-dd HH:mm:ss tt")
        Catch ex As Exception
            Dim tID As String = reader.TriggerId.ToString
            Dim tCat As String = reader.TriggerCategory.ToString
            Console.WriteLine(MS_ErrWarning)
            Dim ErrorString As String = "Error: (" & tCat & ":" & tID & ") " & ex.Message
            writer.WriteLine(ErrorString)
            Debug.Print(ErrorString)
            Return False
        End Try
    End Function
    '(5: ) set variable %Variable to current Furcadia 
    Public Function FurcDateTimeVar(reader As Monkeyspeak.TriggerReader) As Boolean
        Try
            Dim var As Monkeyspeak.Variable = reader.ReadVariable(True)
            SyncLock lock
                var.Value = Main.FurcTime.ToString("yyyy-MM-dd HH:mm:ss tt")
            End SyncLock
            Return True
        Catch ex As Exception
            Dim tID As String = reader.TriggerId.ToString
            Dim tCat As String = reader.TriggerCategory.ToString
            Console.WriteLine(MS_ErrWarning)
            Dim ErrorString As String = "Error: (" & tCat & ":" & tID & ") " & ex.Message
            writer.WriteLine(ErrorString)
            Debug.Print(ErrorString)
            Return False
        End Try
    End Function
    '(5: ) use variable %Variable as a DateTime string and subtract Date Time string {...} and put it into variable %Variable
    Public Function SubsractDateTimeStr(reader As Monkeyspeak.TriggerReader) As Boolean
        Try
            Dim var As Monkeyspeak.Variable = reader.ReadVariable(True)
            Dim str As String = reader.ReadString
            Dim optVar As Monkeyspeak.Variable = reader.ReadVariable(True)
            Dim time As DateTime
            Dim time2 As DateTime
            If DateTime.TryParse(var.Value.ToString, time) And DateTime.TryParse(str, time2) Then
                optVar.Value = time.Subtract(time2).ToString
                Return True
            End If
            Return False
        Catch ex As Exception
            Dim tID As String = reader.TriggerId.ToString
            Dim tCat As String = reader.TriggerCategory.ToString
            Console.WriteLine(MS_ErrWarning)
            Dim ErrorString As String = "Error: (" & tCat & ":" & tID & ") " & ex.Message
            writer.WriteLine(ErrorString)
            Debug.Print(ErrorString)
            Return False
        End Try
    End Function
    '(5: ) use variable %Variable as a DateTime string and subtract Date Time variable %Variable and put it into variable %Variable
    Public Function SubsractDateTimeVar(reader As Monkeyspeak.TriggerReader) As Boolean
        Try
            Dim var As Monkeyspeak.Variable = reader.ReadVariable(True)
            Dim str As Monkeyspeak.Variable = reader.ReadVariable
            Dim optVar As Monkeyspeak.Variable = reader.ReadVariable(True)
            Dim time As DateTime
            Dim time2 As DateTime
            If DateTime.TryParse(var.Value.ToString, time) And DateTime.TryParse(str.Value.ToString, time2) Then
                optVar.Value = time.Subtract(time2).ToString
            End If
            Return True
        Catch ex As Exception
            Dim tID As String = reader.TriggerId.ToString
            Dim tCat As String = reader.TriggerCategory.ToString
            Console.WriteLine(MS_ErrWarning)
            Dim ErrorString As String = "Error: (" & tCat & ":" & tID & ") " & ex.Message
            writer.WriteLine(ErrorString)
            Debug.Print(ErrorString)
            Return False
        End Try
    End Function

    '(5: ) use variable %Variable as a DateTime string and add Date Time string {...} and put it into variable %Variable

    '(5: ) use variable %Variable as a DateTime string and add Date Time variable %Variable and put it into variable %Variable
    'Public Function AddDateTimeVar(reader As Monkeyspeak.TriggerReader) As Boolean
    '    Try
    '        Dim var As Monkeyspeak.Variable = reader.ReadVariable(True)
    '        Dim str As Monkeyspeak.Variable = reader.ReadVariable
    '        Dim optVar As Monkeyspeak.Variable = reader.ReadVariable(True)
    '        Dim time As DateTime
    '        Dim time2 As DateTime
    '        If DateTime.TryParse(var.Value.ToString, time) And DateTime.TryParse(str.Value.ToString, time2) Then
    '            optVar.Value(time.Add(time2).ToString)
    '            Return True
    '        End If
    '        Return False
    '    Catch ex As Exception
    '        Dim tID As String = reader.TriggerId.ToString
    '        Dim tCat As String = reader.TriggerCategory.ToString
    '        Console.WriteLine(MS_ErrWarning)
    '        Dim ErrorString As String = "Error: (" & tCat & ":" & tID & ") " & ex.Message
    '        writer.WriteLine(ErrorString)
    '        Debug.Print(ErrorString)
    '        Return False
    '    End Try
    'End Function

End Class
