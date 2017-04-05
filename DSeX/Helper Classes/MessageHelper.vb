﻿Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Runtime.InteropServices
Imports System.Diagnostics

Public Class MessageHelper
    <DllImport("User32.dll")>

#Region "Public Methods"

    Public Shared Function FindWindow(lpClassName As [String], lpWindowName As [String]) As Int32
    End Function

    Public Shared Function PostMessage(hWnd As IntPtr, Msg As Integer, wParam As IntPtr, ByRef lParam As COPYDATASTRUCT) As IntPtr
    End Function

    Public Shared Function PostMessage(hWnd As Integer, Msg As Integer, wParam As IntPtr, lParam As Integer) As IntPtr
    End Function

    Public Shared Function SendMessage(hWnd As IntPtr, Msg As Integer, wParam As Integer, lParam As Integer) As IntPtr
    End Function

    Public Shared Function SetForegroundWindow(hWnd As Integer) As Boolean
    End Function

    Public Function bringAppToFront(hWnd As Integer) As Boolean
        Return SetForegroundWindow(hWnd)
    End Function

    Public Function getWindowId(className As String, windowName As String) As Integer
        Return FindWindow(className, windowName)
    End Function

    Public Function sendWindowsMessage(hWnd As IntPtr, Msg As Integer, wParam As Integer, lParam As Integer) As IntPtr
        Dim result As IntPtr = IntPtr.Zero

        If hWnd <> IntPtr.Zero Then
            result = SendMessage(hWnd, Msg, wParam, lParam)
        End If

        Return result
    End Function

    Public Function sendWindowsStringMessage(hWnd As IntPtr, wParam As IntPtr, Name As String, fID As UInteger, Tag As String, msg As String) As IntPtr
        Dim result As IntPtr = IntPtr.Zero

        If hWnd <> IntPtr.Zero Then

            Dim cds As New MyData

            cds.lpName = Name
            cds.fID = fID
            cds.lpTag = Tag
            cds.lpMsg = msg

            Dim pData As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(cds))
            Marshal.StructureToPtr(cds, pData, True)

            ' Create the COPYDATASTRUCT you'll use to shuttle the data.
            Dim copy As New COPYDATASTRUCT
            copy.dwData = IntPtr.Zero
            copy.lpData = pData
            copy.cdData = Marshal.SizeOf(cds)
            Dim pCopy As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(copy))
            Marshal.StructureToPtr(copy, pCopy, True)

            ' Send the message to the other application.

            result = SendMessage(hWnd, WM_COPYDATA, wParam, copy)
            Marshal.FreeHGlobal(pData)
            Marshal.FreeHGlobal(pCopy)

        End If

        Return result
    End Function

#End Region

#Region "Private Methods"

    Private Shared Function RegisterWindowMessage(lpString As String) As Integer
    End Function

    <DllImport("User32.dll", EntryPoint:="FindWindow")>
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(hWnd As IntPtr, Msg As Integer, ByVal wParam As IntPtr, ByRef lParam As COPYDATASTRUCT) As IntPtr
    End Function

#End Region

    'For use with WM_COPYDATA and COPYDATASTRUCT
    <DllImport("User32.dll", EntryPoint:="PostMessage")>
    <DllImport("User32.dll", EntryPoint:="SendMessage")>
    <DllImport("User32.dll", EntryPoint:="PostMessage")>
    <DllImport("User32.dll", EntryPoint:="SetForegroundWindow")>
End Class