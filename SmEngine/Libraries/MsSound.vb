﻿Imports System.Media
Imports MonkeyCore
Imports Monkeyspeak

Namespace Engine.Libraries

    ''' <summary>
    ''' Effects: (5:2010) - (5:2012)
    ''' <para>
    ''' Simple way to play wave files
    ''' </para>
    ''' <para>
    ''' Default wave folder: <see cref="Paths.SilverMonkeyBotPath"/>
    ''' </para>
    ''' </summary>
    ''' <remarks>
    ''' This lib contains the following unnamed delegates
    ''' <para>
    ''' (5:2010) play the wave file {...}.
    ''' </para>
    ''' <para>
    ''' (5:2011) play the wave file {...} in a loop. if theres not one playing
    ''' </para>
    ''' </remarks>
    Public NotInheritable Class MsSound
        Inherits MonkeySpeakLibrary
        Implements IDisposable

#Region "Public Constructors"

        Private simpleSound As SoundPlayer

        Public Sub New(Session As BotSession)
            MyBase.New(Session)
            Add(New Trigger(TriggerCategory.Effect, 2010),
                Function(reader As TriggerReader) As Boolean
                    Dim SoundFile As String = Paths.CheckBotFolder(reader.ReadString(True))
                    Using simpleSound = New SoundPlayer(SoundFile)
                        simpleSound.Play()

                    End Using
                    Return True
                End Function, "(5:2010) play the wave file {...}.")

            Add(New Trigger(TriggerCategory.Effect, 2011),
                Function(reader As TriggerReader) As Boolean
                    If Not simpleSound Is Nothing Then
                        Dim SoundFile As String = Paths.CheckBotFolder(reader.ReadString(True))
                        simpleSound = New SoundPlayer(SoundFile)
                        simpleSound.PlayLooping()
                    End If
                    Return simpleSound Is Nothing

                End Function, "(5:2011) play the wave file {...} in a loop. if theres not one playing")
            Add(New Trigger(TriggerCategory.Effect, 2012),
                 AddressOf StopSound, "(5:2012) stop playing the sound file.")
        End Sub

        ''' <summary>
        ''' (5:2012) stop playing the sound file.
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' True on Success
        ''' </returns>
        Function StopSound(reader As TriggerReader) As Boolean
            If Not simpleSound Is Nothing Then
                Dim SoundFile As String = Paths.CheckBotFolder(reader.ReadString(True))
                simpleSound = New SoundPlayer(SoundFile)
                simpleSound.[Stop]()
                simpleSound.Dispose()
            End If
            Return True

        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ''' <summary>
        ''' Override Dispose method
        ''' </summary>
        ''' <param name="page"></param>
        Public Overrides Sub Unload(page As Page)
            Dispose(True)
        End Sub

        ' IDisposable
        Protected Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If simpleSound IsNot Nothing Then
                        simpleSound.Dispose()
                        simpleSound = Nothing
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                End If
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

#End Region

    End Class

End Namespace