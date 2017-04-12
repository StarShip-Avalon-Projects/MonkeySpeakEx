﻿Imports Monkeyspeak
Imports Furcadia.Net
Imports Furcadia.Text.Base95

Public Class BaseClass
    Implements Interfaces.msPlugin

#Region "Private Fields"

    Private msHost As Interfaces.msHost

#End Region

#Region "Public Properties"

    Public ReadOnly Property Description() As String Implements Interfaces.msPlugin.Description
        Get
            Return "Base Class for building Modules"
        End Get
    End Property

    Public Property Enabled As Boolean Implements Interfaces.msPlugin.enabled

    Public ReadOnly Property Name() As String Implements Interfaces.msPlugin.Name
        Get
            Return "Base Module"
        End Get
    End Property

    Public ReadOnly Property Version() As String Implements Interfaces.msPlugin.Version
        Get
            Dim VersionInfo As Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
            Return VersionInfo.Major & "." & VersionInfo.Minor & "." & VersionInfo.Build & "." & VersionInfo.Revision
        End Get
    End Property

#End Region

#Region "Public Methods"

    Public Sub Initialize(ByVal Host As Interfaces.msHost) Implements Interfaces.msPlugin.Initialize
        msHost = Host
    End Sub

#End Region

#Region "Global Properties"

    Public Player As FURRE

    Private _MSpage As Page
    Private msDream As DREAM
    Public Property Dream As DREAM
        Get
            Return msHost.Dream
        End Get
        Set(value As DREAM)
            msHost.Dream = value
        End Set
    End Property

    Public Property MsPage As Monkeyspeak.Page Implements SilverMonkey.Interfaces.msPlugin.MsPage
        Get
            Return _MSpage
        End Get
        Set(value As Monkeyspeak.Page)

            _MSpage = value
            msHost.MsPage = _MSpage
        End Set
    End Property
#End Region

    Function MessagePump(ByRef ServerInstruction As String) As Boolean Implements Interfaces.msPlugin.MessagePump
        'Set Object At Feet
        If ServerInstruction.StartsWith("%") Then
            Player = NameToFurre(msHost.BotName, True)
            Player.FloorObjectCurrent = ConvertFromBase95(ServerInstruction.Substring(1))
            MsPage.Execute(2000, 2001)
            msHost.Player = Player
            Dream.FurreList(Player) = Player
            Return True
            'Set Object In Paws
        ElseIf ServerInstruction.StartsWith("^") Then
            Player = NameToFurre(msHost.BotName, True)
            Player.PawObjectCurrent = ConvertFromBase95(ServerInstruction.Substring(1))
            MsPage.Execute(2000, 2001)
            msHost.Player = Player
            Dream.FurreList(Player) = Player
            Return True
        End If
        Return False
    End Function

    Public Sub Start() Implements Interfaces.msPlugin.Start
        '(0:x) When the bot picks up or drops an object
        'Page.SetTriggerHandler(Monkeyspeak.TriggerCategory.Cause, 2000,
        '    Function()
        '        Return Player.FloorObjectOld = Player.PawObjectCurrent And Player.PawObjectOld = Player.FloorObjectCurrent
        '    End Function, "(0:2000) When the bot picks up or drops an object")

        ''(0:2001) When the bot picks up or drops the object #,
        'Page.SetTriggerHandler(Monkeyspeak.TriggerCategory.Cause, 2001,
        '    AddressOf PickUpObjectNumber, "(0:2001) When the bot picks up or drops the object #,")

    End Sub
#Region "Helper Functions"
    Public Function IsBot(ByRef p As FURRE) As Boolean
        Return p.ShortName = msHost.BotName.ToFurcShortName
    End Function

    Public Function NameToFurre(ByRef sname As String, ByRef UbdateMSVariableName As Boolean) As FURRE
        Dim p As New FURRE
        p.Name = sname
        For Each Character As FURRE In Dream.FurreList
            If Character.ShortName = sname.ToFurcShortName Then
                p = Character
                Exit For
            End If
        Next
        If UbdateMSVariableName Then MsPage.SetVariable(VarPrefix & "NAME", sname, True)
        Return p
    End Function

    Public Function ReadVariableOrNumber(ByVal reader As Monkeyspeak.TriggerReader, Optional addIfNotExist As Boolean = False) As Double
        Dim result As Double = 0
        If reader.PeekVariable Then
            Dim value As String = reader.ReadVariable(addIfNotExist).Value.ToString
            Double.TryParse(value, result)
        ElseIf reader.PeekNumber Then
            result = reader.ReadNumber
        End If
        Return result
    End Function
    Private Function fIDtoFurre(ByRef ID As UInteger) As FURRE

        For Each Character As FURRE In Dream.FurreList
            If Character.ID = ID Then
                Return Character
            End If
        Next
    End Function
#End Region
End Class