﻿Public Interface msHost

#Region "Public Properties"

    ReadOnly Property BotName As String
    Property Data As String
    Property Dream As Furcadia.Net.DREAM
    WriteOnly Property Page As Monkeyspeak.Page
    Property Player As Furcadia.Net.FURRE

#End Region

#Region "Public Methods"

    Sub logError(ByRef Ex As Exception, ByRef ObjectThrowingError As Object)

    Sub SendClientMessage(msg As String, data As String)

    Sub sendServer(ByRef var As String)

    Sub start(ByRef page As Monkeyspeak.Page)

#End Region

End Interface