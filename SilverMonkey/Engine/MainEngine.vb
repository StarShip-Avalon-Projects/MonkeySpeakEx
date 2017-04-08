﻿Imports Monkeyspeak
Imports Furcadia.Util
Imports MonkeyCore
Imports System.Collections.Generic
Imports MonkeyCore.Settings
Imports System.Diagnostics
Imports SilverMonkey.PhoenixSpeak

Imports Microsoft.Win32.SafeHandles
Imports System.Runtime.InteropServices

''' <summary>
''' Silver Monkey's MonkeySpeak Engine with our Customizations
''' </summary>
Public Class MainMsEngine
    Inherits MonkeyspeakEngine
    Implements IDisposable

#Region "Const"
    Private Const MS_Footer As String = "*Endtriggers* 8888 *Endtriggers*"
    Private Const MS_Header As String = "*MSPK V04.00 Silver Monkey"
#End Region

#Region "Public Methods"

    Public Shared Function IsBotControler(ByRef Name As String) As Boolean
        If String.IsNullOrEmpty(cBot.BotController) Then Return False
        Return FurcadiaShortName(cBot.BotController) = FurcadiaShortName(Name)
    End Function

#End Region

#Region "MonkeySpeakEngine"
    Public WithEvents MSpage As Page = Nothing

    Public Shared MS_Stared As Integer = 0

    Public EngineRestart As Boolean = False

    Public MS_Engine_Running As Boolean = False

    Private Const RES_MS_begin As String = "*MSPK V"

    Private Const RES_MS_end As String = "*Endtriggers* 8888 *Endtriggers*"

    Private Shared Writer As TextBoxWriter = New TextBoxWriter(Variables.TextBox1)

    Private msVer As Double = 3.0

    Public Sub New()

        EngineStart(True)
    End Sub

    Public Shared Function MS_Started() As Boolean
        ' 0 = main load
        ' 1 = engine start
        ' 2 = engine running
        Return MS_Stared >= 2
    End Function

    'loads at main load
    Public Sub EngineStart(ByRef LoadPlugins As Boolean)

        MSpage = LoadFromString("")
        LoadLibrary(LoadPlugins)

    End Sub

    Public Sub LoadLibrary(ByRef LoadPlugins As Boolean)
        'Library Loaded?.. Get the Hell out of here
        If MS_Started() Then Exit Sub
        MS_Stared += 1
        MSpage.SetTriggerHandler(TriggerCategory.Cause, 0,
             Function()
                 Return True
             End Function, "(0:0) When the bot starts,")
        Try
            MSpage.LoadSysLibrary()
#If CONFIG = "Release" Then
            '(5:105) raise an error.
            MSpage.RemoveTriggerHandler(TriggerCategory.Effect, 105)
            '(5:110) load library from file {...}.
            MSpage.RemoveTriggerHandler(TriggerCategory.Effect, 110)
#ElseIf CONFIG = "Debug" Then

#End If
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MS_IO())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadTimerLibrary()
            MSpage.LoadStringLibrary()
            MSpage.LoadMathLibrary()
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New StringLibrary()) ' Load our new TestLibrary
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New SayLibrary())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New Banish())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MathLibrary())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MS_Time())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MSPK_MDB())
        Catch ex As FileNotFoundException
            Console.WriteLine(ex.Message)
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MSPK_Web())

        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MS_Cookie())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MsPhoenixSpeak())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New DatabaseSystem())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MS_Dice())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New Description())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try

            MSpage.LoadLibrary(New MonkeySpeakFurreList())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New Warning())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New Movement())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New WmCpyDta())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MS_Pounce())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MS_MemberList())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try
        Try
            MSpage.LoadLibrary(New MS_Verbot())
        Catch ex As Exception
            Dim e As New ErrorLogging(ex, Me)
        End Try

        'Define our Triggers before we use them
        'TODO Check for Duplicate and use that one instead
        'we don't want this to cause a memory leak.. its prefered to run this one time and thats  it except for checking for new plugins
        'Loop through available plugins, creating instances and adding them to listbox
        If Not Plugins Is Nothing And LoadPlugins Then
            Dim objPlugin As Interfaces.msPlugin
            Dim newPlugin As Boolean = False
            For intIndex As Integer = 0 To Plugins.Count - 1
                Try
                    objPlugin = DirectCast(PluginServices.CreateInstance(Plugins(intIndex)), Interfaces.msPlugin)
                    If Not PluginList.ContainsKey(objPlugin.Name.Replace(" ", "")) Then
                        PluginList.Add(objPlugin.Name.Replace(" ", ""), True)
                        newPlugin = True
                    End If

                    If PluginList.Item(objPlugin.Name.Replace(" ", "")) = True Then
                        Console.WriteLine("Loading Plugin: " + objPlugin.Name)
                        objPlugin.Initialize(objHost)
                        objPlugin.Page = MSpage
                        objPlugin.Start()
                    End If
                Catch ex As Exception
                    Dim e As New ErrorLogging(ex, Me)
                End Try
            Next
            'TODO: Add to delegate?
            'If newPlugin Then Main.MainSettings.SaveMainSettings()

        End If
    End Sub

    Public Sub LogError(reader As TriggerReader, ex As Exception)

        Console.WriteLine(MS_ErrWarning)
        Dim ErrorString As String = "Error: (" & reader.TriggerCategory.ToString & ":" & reader.TriggerId.ToString & ") " & ex.Message

        If Not IsNothing(cBot) Then
            If cBot.log Then
                LogStream.WriteLine(ErrorString, ex)
            End If
        End If
        Writer.WriteLine(ErrorString)
    End Sub
    Public Sub LogError(trigger As Trigger, ex As Exception) Handles MSpage.Error

        Console.WriteLine(MS_ErrWarning)
        Dim ErrorString As String = "Error: (" & trigger.Category.ToString & ":" & trigger.Id.ToString & ") " & ex.Message

        If Not IsNothing(cBot) Then
            If cBot.log Then
                '  BotLogStream.WriteLine(ErrorString, ex)
            End If
        End If
        Writer.WriteLine(ErrorString)
    End Sub

    Public Function msReader(ByVal file As String) As String
        file = file.Trim
        Dim Data As String = String.Empty
        Try
            If Not System.IO.File.Exists(file.Trim) Then
                Return ""
            End If
            Dim line As String = ""
            Using objReader As New StreamReader(file)
                ' line = objReader.ReadLine() & Environment.NewLine
                While objReader.Peek <> -1
                    line = objReader.ReadLine()
                    If Not line.StartsWith(RES_MS_begin) Then
                        Data += line + Environment.NewLine
                    End If

                    If line = RES_MS_end Then
                        Exit While
                    End If

                End While
                objReader.Close()
                objReader.Dispose()
            End Using
            Return Data
        Catch eX As Exception
            Dim LogError As New ErrorLogging(eX, Me)
            Return ""
        End Try
    End Function

    Public Sub PageExecute(ParamArray ID() As Integer)
        If Not IsNothing(cBot) Then
            If cBot.MS_Engine_Enable AndAlso MS_Started() Then
                MSpage.Execute(ID)

            End If
        End If

    End Sub

    Public Sub PageSetVariable(ByVal varName As String, ByVal data As Object)
        If cBot.MS_Engine_Enable AndAlso MS_Started() Then
            If data Is Nothing Then data = String.Empty
            Debug.Print("Settingg Variable: " + varName + ":" + data.ToString)
            MSpage.SetVariable(varName.ToUpper, data, True) '

        End If
    End Sub

    Public Sub PageSetVariable(ByVal VariableList As Dictionary(Of String, Object))
        If cBot.MS_Engine_Enable Then

            For Each kv As KeyValuePair(Of String, Object) In VariableList
                PageSetVariable(kv.Key.ToUpper, kv.Value, True)
            Next '

        End If
    End Sub

    Public Sub PageSetVariable(ByVal varName As String, ByVal data As Object, ByVal Constant As Boolean)
        If Not IsNothing(cBot) Then
            If cBot.MS_Engine_Enable AndAlso MS_Started() Then
                Debug.Print("Settingg Variable: " + varName + ":" + data.ToString)
                MSpage.SetVariable(varName.ToUpper, data, Constant) '
            End If
        End If
    End Sub

    'Bot Starts
    Public Function ScriptStart() As Boolean
        Try
            Dim VariableList As New Dictionary(Of String, Object)

            If Not cBot.MS_Engine_Enable Then
                Return False
            End If

            Console.WriteLine("Loading:" & cBot.MS_File)
            Dim start As DateTime = DateTime.Now
            If Not File.Exists(cBot.MS_File) Then
                Directory.Exists(Path.GetDirectoryName(cBot.MS_File))
                cBot.MS_File = Path.Combine(Paths.SilverMonkeyBotPath, cBot.MS_File)
            End If
            cBot.MS_Script = msReader(cBot.MS_File)
            If String.IsNullOrEmpty(cBot.MS_Script) Then
                Console.WriteLine("ERROR: No script loaded! Loading Default MonkeySpeak.")
                MS_Engine_Running = False
                msReader(IO.NewMSFile)
                VariableList.Add("MSPATH", "!!! Not Specified !!!")
            Else
                VariableList.Add("MSPATH", Paths.SilverMonkeyBotPath)
            End If
            Try
                MSpage = LoadFromString(cBot.MS_Script)
            Catch ex As MonkeyspeakException
                Console.WriteLine(ex.Message)
                Return False
            Catch ex As Exception
                Console.WriteLine("There's an error loading the bot script")
                Return False
            End Try
            ' Console.WriteLine("Execute (0:0)")
            MS_Stared = 1
            LoadLibrary(True)

            VariableList.Add("DREAMOWNER", "")
            VariableList.Add("DREAMNAME", "")
            VariableList.Add("BOTNAME", "")
            VariableList.Add("BOTCONTROLLER", cBot.BotController)
            VariableList.Add(MS_Name, "")
            VariableList.Add("MESSAGE", "")
            VariableList.Add("BANISHNAME", "")
            VariableList.Add("BANISHLIST", "")
            PageSetVariable(VariableList)
            '(0:0) When the bot starts,
            PageExecute(0)
            Console.WriteLine("Done! Executed in " & Date.Now.Subtract(start).ToString())
            'Console.ReadKey()
            MS_Engine_Running = True
        Catch eX As Exception
            Dim logError As New ErrorLogging(eX, Me)
            Return False
        End Try
        Return True
    End Function
#End Region

#Region "Protected Methods"

    'TODO Link to Furcadia Proxy Send Client
    Protected Sub SendClientMessage(ByRef System As String, Message As String)

    End Sub

#End Region

#Region "Dispose"
    'need Timer Library disposal here and any other Libs that need to be disposed

    Dim disposed As Boolean = False
    ' Instantiate a SafeHandle instance.
    Dim handle As SafeHandle = New SafeFileHandle(IntPtr.Zero, True)

    ' Public implementation of Dispose pattern callable by consumers.
    Public Sub Dispose() _
               Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    ' Protected implementation of Dispose pattern.
    Protected Overridable Sub Dispose(disposing As Boolean)
        If disposed Then Return

        If disposing Then
            handle.Dispose()

        End If

        ' Free any unmanaged objects here.
        '
        disposed = True
    End Sub
#End Region

End Class