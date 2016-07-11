﻿
Imports System.Data
Imports System.Data.SQLite
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports MonkeyCore
Imports Monkeyspeak
Imports MonkeyCore.IO

Public Class MSPK_MDB
    Inherits Libraries.AbstractBaseLibrary

    Private writer As TextBoxWriter = Nothing
    Public Shared SQLreader As SQLiteDataReader = Nothing
    Private QueryRun As Boolean = False
    Private Shared _SQLitefile As String
    Public Shared Property SQLitefile As String
        Get
            If String.IsNullOrEmpty(_SQLitefile) Then
                _SQLitefile = Path.Combine(Paths.SilverMonkeyBotPath, "SilverMonkey.db")
            End If
            Return _SQLitefile
        End Get
        Set(value As String)
            _SQLitefile = value
        End Set
    End Property

    Dim lock As New Object
    Dim cache As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

    Public Sub New()

        Try
            writer = New TextBoxWriter(Variables.TextBox1)
        Catch eX As Exception
            Dim logError As New ErrorLogging(eX, Me)
        End Try
        Try
            'Set the default file the first time this MonkeySpeak Library is run


        Catch eX As Exception
            Dim logError As New ErrorLogging(eX, Me)
        End Try
        Try
            '(0:500) When the bot starts backing up the character Phoenix Speak,
            Add(TriggerCategory.Cause, 500,
                Function()
                    Return True
                End Function, "(0:500) When the bot starts backing up the character Phoenix Speak,")
            '(0:501) When the bot completes backing up the characters Phoenix Speak,
            Add(TriggerCategory.Cause, 501,
                Function()
                    Return True
                End Function, "(0:501) When the bot completes backing up the characters Phoenix Speak,")
            '(0:502) When the bot starts restoring the Dreams Character Phoenix Speak,
            Add(TriggerCategory.Cause, 502,
                Function()
                    Return True
                End Function, "(0:502) When the bot starts restoring the Dreams Character Phoenix Speak,")
            '(0:503) When the bot finishes restoring the dreams character Phoenix Speak,
            Add(TriggerCategory.Cause, 503,
                Function()
                    Return True
                End Function, "(0:503) When the bot finishes restoring the dreams character Phoenix Speak,")
            Add(TriggerCategory.Cause, 504,
                Function()
                    Return True
                End Function, "(0:504) When the bot backsup Phoenix Speak for any Furre.")
            Add(TriggerCategory.Cause, 505,
                AddressOf BackUpCharacterNamed, "(0:505) When the bot backsup Phoenix Speak for the furre named {...}.")
            Add(TriggerCategory.Cause, 506,
                Function()
                    Return True
                End Function, "(0:506) When the bot restores any furre's Phoenix Speak.")
            Add(TriggerCategory.Cause, 507,
                AddressOf BackUpCharacterNamed, "(0:507) When the bot restores the  Phoenix Speak for the furre named {...}.")

            '(1:500) and the Database info {...} about the triggering furre is equal to #,
            Add(New Trigger(TriggerCategory.Condition, 500),
                AddressOf TriggeringFurreinfoEqualToNumber, "(1:500) and the Database info {...} about the triggering furre is equal to #,")
            '(1:501) and the Database info {...} about the triggering furre is not equal to #,
            Add(New Trigger(TriggerCategory.Condition, 501),
                AddressOf TriggeringFurreinfoNotEqualToNumber, "(1:501) and the Database info {...} about the triggering furre is not equal to #,")
            '(1:502) and the Database info {...} about the triggering furre is greater than #,
            Add(New Trigger(TriggerCategory.Condition, 502),
                AddressOf TriggeringFurreinfoGreaterThanNumber, "(1:502) and the Database info {...} about the triggering furre is greater than #,")
            '(1:503) and the Database info {...} about the triggering furre is less than #,
            Add(New Trigger(TriggerCategory.Condition, 503),
                AddressOf TriggeringFurreinfoLessThanNumber, "(1:503) and the Database info {...} about the triggering furre is less than #,")

            '(1:504) and the Database info {...} about the triggering furre is greater than or equal to #,
            Add(New Trigger(TriggerCategory.Condition, 504),
                AddressOf TriggeringFurreinfoGreaterThanOrEqualToNumber, "(1:504) and the Database info {...} about the triggering furre is greater than or equal to #,")
            '(1:505) and the Database info {...} about the triggering furre is less than or equal to#,
            Add(New Trigger(TriggerCategory.Condition, 505),
                AddressOf TriggeringFurreinfoLessThanOrEqualToNumber, "(1:505) and the Database info {...} about the triggering furre is less than or equal to #,")


            '(1:508) and the Database info {...} about the furre named {...} is equal to #,
            Add(New Trigger(TriggerCategory.Condition, 508),
                AddressOf FurreNamedinfoEqualToNumber, "(1:508) and the Database info {...} about the furre named {...} is equal to #,")
            '(1:509) and the Database info {...} about the furre named {...} is not equal to #,
            Add(New Trigger(TriggerCategory.Condition, 509),
                AddressOf FurreNamedinfoNotEqualToNumber, "(1:509) and the Database info {...} about the furre named {...} is not equal to #,")
            '(1:510) and the Database info {...} about the furre named {...} is greater than #,
            Add(New Trigger(TriggerCategory.Condition, 510),
                AddressOf FurreNamedinfoGreaterThanNumber, "(1:510) and the Database info {...} about the furre named {...} is greater than #,")
            '(1:511) and the Database info {...} about the furre named {...} is less than #,
            Add(New Trigger(TriggerCategory.Condition, 511),
                AddressOf FurreNamedinfoLessThanNumber, "(1:511) and the Database info {...} about the furre named {...} is less than #,")

            '(1:510) and the Database info {...} about the furre named {...} is greater than or equal to #,
            Add(New Trigger(TriggerCategory.Condition, 512),
            AddressOf FurreNamedinfoGreaterThanOrEqualToNumber, "(1:512) and the Database info {...} about the furre named {...} is greater than or equal to #,")
            '(1:511) and the Database info {...} about the furre named {...} is less than or equal to #,
            Add(New Trigger(TriggerCategory.Condition, 513),
            AddressOf FurreNamedinfoLessThanOrEqualToNumber, "(1:513) and the Database info {...} about the furre named {...} is less than or equal to #,")


            '(1:516) and the Database info {...} about the furre named {...} is equal to {...},
            Add(New Trigger(TriggerCategory.Condition, 516),
           AddressOf FurreNamedinfoEqualToSTR, "(1:516) and the Database info {...} about the furre named {...} is equal to string {...},")
            '(1:517) and the Database info {...} about the furre named {...} is not equal to {...},
            Add(New Trigger(TriggerCategory.Condition, 517),
            AddressOf FurreNamedinfoNotEqualToSTR, "(1:517) and the Database info {...} about the furre named {...} is not equal to string {...},")
            '(1:518) and the Database info {...} about the triggering furre is equal to {...},
            Add(New Trigger(TriggerCategory.Condition, 518),
                AddressOf TriggeringFurreinfoEqualToSTR, "(1:518) and the Database info {...} about the triggering furre is equal to string {...},")
            '(1:519) and the Database info {...} about the triggering furre is not equal to {...},
            Add(New Trigger(TriggerCategory.Condition, 519),
                AddressOf TriggeringFurreinfoNotEqualToSTR, "(1:519) and the Database info {...} about the triggering furre is not equal to string {...},")

            '(1:520) and the bot is not in the middle of a PS Backup Process
            Add(New Trigger(TriggerCategory.Condition, 520),
                  AddressOf BotBackup, "(1:520) and the bot is not in the middle of a PS Backup Process,")

            '(1:521) and the bot is in the middle of a PS Backup Process.
            Add(New Trigger(TriggerCategory.Condition, 521),
                     AddressOf NotBotBackup, "(1:521) and the bot is in the middle of a PS Backup Process,")

            '(1:522) and the bot is not in the middle of a PS Restore Process,
            Add(New Trigger(TriggerCategory.Condition, 522),
                 AddressOf BotRestore, "(1:522) and the bot is not in the middle of a PS Restore Process,")
            '(1:523) and the bot is in the middle of a PS Restore Process,
            Add(New Trigger(TriggerCategory.Condition, 523),
                 AddressOf NotBotRestore, "(1:523) and the bot is in the middle of a PS Restore Process,")


            '(5:500) use SQLite database file {...} or create file if it does not exist.
            Add(New Trigger(TriggerCategory.Effect, 500), AddressOf createMDB, "(5:500) use SQLite database file {...} or create file if it does not exist.")

            '(5:505 ) Add the triggering furre with the default access level 0 to the Furre Table in the database if he, she or it don't exist.
            Add(New Trigger(TriggerCategory.Effect, 505), AddressOf insertTriggeringFurreRecord, "(5:505) add the triggering furre with the default access level ""0"" to the Furre Table in the database if he, she, or it doesn't exist.")
            '(5:506) Add furre named {...} with the default access level 0 to the Furre Table in the database if he, she or it don't exist.
            Add(New Trigger(TriggerCategory.Effect, 506), AddressOf InsertFurreNamed, "(5:506) add furre named {...} with the default access level ""0"" to the Furre Table in the database if he, she, or it doesn't exist.")

            '(5:507) update Database info {...} about the triggering furre will now be #.
            Add(New Trigger(TriggerCategory.Effect, 507), AddressOf UpdateTriggeringFurreField, "(5:507) update Database info {...} about the triggering furre will now be #.")
            '(5:508) update Database info {...} about the furre named {...} will now be #.
            Add(New Trigger(TriggerCategory.Effect, 508), AddressOf UpdateFurreNamed_Field, "(5:508) update Database info {...} about the furre named {...} will now be #.")
            '(5:509) update Database info {...} about the triggering furre will now be {...}.
            Add(New Trigger(TriggerCategory.Effect, 509), AddressOf UpdateTriggeringFurreFieldSTR, "(5:509) update Database info {...} about the triggering furre will now be {...}.")
            '(5:510) update Database info {...} about the furre named {...} will now be {...}.
            Add(New Trigger(TriggerCategory.Effect, 510), AddressOf UpdateFurreNamed_FieldSTR, "(5:510) update Database info {...} about the furre named {...} will now be {...}.")

            '(5:511) select Database info {...} about the triggering furre, and put it in variable %.
            Add(New Trigger(TriggerCategory.Effect, 511), AddressOf ReadDatabaseInfo, "(5:511) select Database info {...} about the triggering furre, and put it in variable %.")
            '(5:512) select Database info {...} about the furre named {...}, and put it in variable %.
            Add(New Trigger(TriggerCategory.Effect, 512), AddressOf ReadDatabaseInfoName, "(5:512) select Database info {...} about the furre named {...}, and put it in variable %.")

            '(5:513) add column {...} with type {...} to the Furre table.
            Add(New Trigger(TriggerCategory.Effect, 513), AddressOf AddColumn, "(5:513) add column {...} with type {...} to the Furre table.")


            '(5:518) delete all Database info about the triggering furre.
            Add(New Trigger(TriggerCategory.Effect, 518), AddressOf DeleteTriggeringFurre, "(5:518) delete all Database info about the triggering furre.")
            '(5:519) delete all Database info about the furre named {...}.
            Add(New Trigger(TriggerCategory.Effect, 519), AddressOf DeleteFurreNamed, "(5:519) delete all Database info about the furre named {...}.")

            '(5:522) get the total of records from table {...} and put it into variable %.
            Add(New Trigger(TriggerCategory.Effect, 522), AddressOf GetTotalRecords, "(5:522) get the total number of records from table {...} and put it into variable %Variable.")

            '(5:523) take the sum of column{...} in table {...} and put it into variable %
            Add(New Trigger(TriggerCategory.Effect, 523), AddressOf ColumnSum, "(5:523) take the sum of column{...} in table {...} and put it into variable %Variable.")

            '(5:550) take variable %Variable , prepare it for a query, and put it in variable %Variable .   (this is your escaping call, which would depend on however you have to do it internally)
            Add(New Trigger(TriggerCategory.Effect, 550), AddressOf PrepQuery,
                "(5:550) take variable %Variable , prepare it for a SQLite Database query, and put it in variable %Variable.")

            '(5:551) execute SQLite Database query {...} Select * from table where name=%2
            Add(New Trigger(TriggerCategory.Effect, 551), AddressOf ExecuteQuery,
                 "(5:551) execute SQLite Database query {...}.")

            '(5:552) retrieve field {...} from SQLite Database query and put it into variable %Variable .
            Add(New Trigger(TriggerCategory.Effect, 552), AddressOf RetrieveQuery,
                "(5:552) retrieve field {...} from SQLite Database query and put it into variable %Variable.")
            '(5:553) Backup All Character phoenixspeak for the dream
            Add(New Trigger(TriggerCategory.Effect, 553), AddressOf BackupAllPS,
               "(5:553) backup All Phoenix Speak for the dream")
            '(5:554) backup Character named {...} phoenix speak 
            Add(New Trigger(TriggerCategory.Effect, 554), AddressOf BackupSingleCharacterPS,
                   "(5:554) backup character named {...} Phoenix Speak. (use ""[DREAM]"" to restore information specific to the dream)")
            '(5:555) restore phoenix speak for character {...}
            Add(New Trigger(TriggerCategory.Effect, 555), AddressOf RestoreCharacterPS,
                   "(5:555) restore Phoenix Speak for character {...}. (use ""[DREAM]"" to restore information specific to the dream)")
            '(5:556) restore all phoenxi speak characters for this dream.
            Add(New Trigger(TriggerCategory.Effect, 556), AddressOf restoreAllPSData,
                 "(5:556) restore all Phoenix Speak records for this dream.")
            '(5:557) remove Entries older then # days from Phoenix Speak Character backup.
            Add(New Trigger(TriggerCategory.Effect, 557), AddressOf PruneCharacterBackup,
                "(5:557) remove Entries older than # days from Phoenix Speak backup.")
            Add(New Trigger(TriggerCategory.Effect, 558), AddressOf restorePS_DataOldrThanDays,
                "(5:558) restore Phoenix Speak records newer then # days.")
            '(5:559) execute VACUUM on the database to rebuild and reclaim wasted space.
            Add(New Trigger(TriggerCategory.Effect, 559), AddressOf VACUUM,
                "(5:559) execute ""VACUUM"" to rebuild the database and reclaim wasted space.")
            Add(New Trigger(TriggerCategory.Effect, 560), AddressOf AbortPS,
                "(5:560) abort Phoenix Speak backup or restore process")
        Catch eX As Exception
            Dim logError As New ErrorLogging(eX, Me)
        End Try

    End Sub


#Region "Condition Functions"

    Public Function BackUpCharacterNamed(reader As TriggerReader) As Boolean
        Dim furre As String = reader.ReadString
        Return callbk.Player.ShortName = furre.ToFurcShortName
    End Function

    '(1: ) and the Database info {...} about the triggering furre is equal to #,
    Public Function TriggeringFurreinfoEqualToNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim number As Double = 0
        Dim Furre As String = Nothing
        Dim Num As Double = 0

        Try
            info = reader.ReadString
            number = ReadVariableOrNumber(reader, False)
            Furre = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString
            Dim Value As Double = 0
            Double.TryParse(GetValueFromTable(info, Furre.ToFurcShortName).ToString, Value)

            Return number = Value
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function

    '(1: ) and the Database info {...} about the triggering furre is not equal to #,
    Public Function TriggeringFurreinfoNotEqualToNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim number As Double = 0
        Dim Furre As String = Nothing
        Try
            info = reader.ReadString
            number = ReadVariableOrNumber(reader, False)
            Furre = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString
            Furre = Furre.ToFurcShortName
            Dim val As String = GetValueFromTable(info, Furre).ToString
            Dim Value As Double = 0
            Double.TryParse(val, Value)
            Return Value <> number
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function
    '(1: ) and the Database info {...} about the triggering furre is greater than #,
    Public Function TriggeringFurreinfoGreaterThanNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim number As Double = 0
        Dim Furre As String = Nothing
        Try
            info = reader.ReadString
            number = ReadVariableOrNumber(reader, False)
            Furre = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString
            Furre = Furre.ToFurcShortName
            Dim check As Object = GetValueFromTable(info, Furre)
            Dim Value As Double = 0
            Double.TryParse(check.ToString, Value)
            Return Value > number

        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function
    '(1: ) and the Database info {...} about the triggering furre is less than #,
    Public Function TriggeringFurreinfoLessThanNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim number As Double = 0
        Dim Furre As String = Nothing
        Try
            info = reader.ReadString
            number = ReadVariableOrNumber(reader, False)
            Furre = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            Dim Num As Double = 0
            Dim check As Object = GetValueFromTable(info, Furre)
            Double.TryParse(check.ToString, Num)

            Return Num < number
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function

    '(1: ) and the Database info {...} about the triggering furre is greater than or equal to #,
    Public Function TriggeringFurreinfoGreaterThanOrEqualToNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim number As Double = 0
        Dim Furre As String = Nothing
        Try
            info = reader.ReadString
            number = ReadVariableOrNumber(reader, False)
            Furre = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            Dim Num As Double = 0
            Dim check As Object = GetValueFromTable(info, Furre)
            Double.TryParse(check.ToString, Num)
            Return Num >= number

        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function
    '(1: ) and the Database info {...} about the triggering furre is less than #,
    Public Function TriggeringFurreinfoLessThanOrEqualToNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim number As Double = 0
        Dim Furre As String = Nothing
        Try
            info = reader.ReadString
            number = ReadVariableOrNumber(reader, False)
            Furre = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            Dim Num As Double = 0
            Dim check As Object = GetValueFromTable(info, Furre)
            Double.TryParse(check.ToString, Num)
            Return Num <= number
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function

    '(1: ) and the Database info {...} about the furre named {...} is equal to #,
    Public Function FurreNamedinfoEqualToNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim Variable As Double = 0
        Dim Furre As String = Nothing

        Try
            info = reader.ReadString
            Furre = reader.ReadString
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            Variable = ReadVariableOrNumber(reader, False)
            Dim Value As Double = 0
            Double.TryParse(GetValueFromTable(info, Furre).ToString, Value)
            Return Variable = Value
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function
    '(1: ) and the Database info {...} about the furre named {...} is not equal to #,
    Public Function FurreNamedinfoNotEqualToNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim Variable As Double = 0
        Dim Furre As String = Nothing
        Try
            info = reader.ReadString
            Furre = reader.ReadString
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            Variable = ReadVariableOrNumber(reader, False)
            Dim check As Object = GetValueFromTable(info, Furre)
            Dim Value As Double = 0
            Double.TryParse(check.ToString, Value)
            Return Value <> Variable
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function
    '(1: ) and the Database info {...} about the furre named {...} is greater than #,
    Public Function FurreNamedinfoGreaterThanNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim Variable As Double = 0
        Dim Furre As String = Nothing
        Try
            info = reader.ReadString
            Furre = reader.ReadString
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            Variable = ReadVariableOrNumber(reader, False)
            Dim check As Object = GetValueFromTable(info, Furre)
            Dim Value As Double = 0
            Double.TryParse(check.ToString, Value)
            Return Value > Variable
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function
    '(1: ) and the Database info {...} about the furre named {...} is less than #,
    Public Function FurreNamedinfoLessThanNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim Variable As Double = 0
        Dim Furre As String = Nothing
        Try
            info = reader.ReadString
            Furre = reader.ReadString
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            Variable = ReadVariableOrNumber(reader, False)
            Dim Value As Double = 0
            Double.TryParse(GetValueFromTable(info, Furre).ToString, Value)
            Return Value < Variable
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function

    '(1: ) and the Database info {...} about the furre named {...} is greater than #,
    Public Function FurreNamedinfoGreaterThanOrEqualToNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim Variable As Double = 0
        Dim Furre As String = Nothing
        Try
            info = reader.ReadString
            Furre = reader.ReadString
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            Variable = ReadVariableOrNumber(reader, False)
            Dim Value As Double = 0
            Double.TryParse(GetValueFromTable(info, Furre).ToString, Value)
            Return Value >= Variable
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function
    '(1: ) and the Database info {...} about the furre named {...} is less than #,
    Public Function FurreNamedinfoLessThanOrEqualToNumber(reader As TriggerReader) As Boolean
        Dim info As String = Nothing
        Dim Variable As Double = 0
        Dim Furre As String = Nothing
        Try
            info = reader.ReadString
            Furre = reader.ReadString
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            Variable = ReadVariableOrNumber(reader, False)
            Dim check As Object = GetValueFromTable(info, Furre)
            Dim Value As Double = 0
            Double.TryParse(check.ToString, Value)
            Return Value <= Variable
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

        Return False
    End Function

    '(1: ) and the Database info {...} about the furre named {...} is equal to {...},
    Public Function FurreNamedinfoEqualToSTR(reader As TriggerReader) As Boolean
        Dim Info As String = reader.ReadString
        Dim Furre As String = reader.ReadString()
        Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
        Dim str As String = reader.ReadString
        Try
            Return str = GetValueFromTable(Info, Furre).ToString
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return False
    End Function
    '(1: ) and the Database info {...} about the furre named {...} is not equal to {...},
    Public Function FurreNamedinfoNotEqualToSTR(reader As TriggerReader) As Boolean
        Dim Info As String = reader.ReadString
        Dim Furre As String = reader.ReadString
        Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
        Dim str As String = reader.ReadString

        Try
            Return str <> GetValueFromTable(Info, Furre).ToString
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return False
    End Function
    '(1: ) and the Database info {...} about the triggering furre is equal to {...},
    Public Function TriggeringFurreinfoEqualToSTR(reader As TriggerReader) As Boolean
        Dim Info As String = reader.ReadString
        Dim Furre As String = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString()
        Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
        Dim str As String = reader.ReadString
        Try
            If str = GetValueFromTable(Info, Furre).ToString Then Return True
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return False
    End Function
    '(1: ) and the Database info {...} about the triggering furre is not equal to {...},
    Public Function TriggeringFurreinfoNotEqualToSTR(reader As TriggerReader) As Boolean
        Dim Info As String = reader.ReadString
        Dim Furre As String = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString()
        Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
        Dim str As String = reader.ReadString
        Try
            If str <> GetValueFromTable(Info, Furre).ToString Then Return True
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return False
    End Function

    '(1:520) and the bot is not in the middle of a PS Backup Process
    Public Function BotBackup(reader As TriggerReader) As Boolean

        Try
            Return Not callbk.PSBackupRunning
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return False
    End Function
    '(1:521) and the bot is in the middle of a PS Backup Process
    Public Function NotBotBackup(reader As TriggerReader) As Boolean
        Try
            Return callbk.PSBackupRunning
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return False
    End Function
    '(1:522) and the bot is not in the middle of a PS Restore Process
    Public Function BotRestore(reader As TriggerReader) As Boolean

        Try
            Return Not callbk.PSRestoreRunning
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return False
    End Function
    '(1:523) and the bot is in the middle of a PS Restore Process
    Public Function NotBotRestore(reader As TriggerReader) As Boolean

        Try
            Return callbk.PSRestoreRunning
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return False
    End Function
#End Region

#Region "Condition Helper Functions"
    Private Function GetValueFromTable(ByRef Column As String, ByRef Name As String) As Object
        Dim db As SQLiteDatabase = New SQLiteDatabase(Main.SQLitefile)
        Dim str As String = "SELECT * FROM FURRE WHERE Name='" & Name & "';"
        SyncLock lock
            cache = db.GetValueFromTable(str)
        End SyncLock
        QueryRun = True
        Try
            If Not IsNothing(cache) Then
                If cache.ContainsKey(Column) Then
                    Return cache.Item(Column)
                End If
            End If
        Catch ex As Exception
            Dim err As New ErrorLogging(ex, Me)
        End Try
        Return ""
    End Function
#End Region


#Region "Effect Functions"


    Public Function createMDB(reader As TriggerReader) As Boolean
        SQLitefile = CheckBotFolder(reader.ReadString())
        Console.WriteLine("NOTICE: SQLite Database file has changed to" + SQLitefile)
        Dim db As New SQLiteDatabase(SQLitefile)
        'db.CreateTbl("FURRE", FurreTable)
        Return True
    End Function


    '(5:405) Add the triggering furre with default access level to the Furre Table in the database if he, she or it don't exist.
    Public Function insertTriggeringFurreRecord(reader As TriggerReader) As Boolean
        Dim Furre As String = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString()
        Dim info As String = reader.ReadString
        'Dim value As String = reader.ReadVariable.Value.ToString

        Dim db As SQLiteDatabase = New SQLiteDatabase(Main.SQLitefile)
        Dim data As New Dictionary(Of [String], [String])()
        Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
        data.Add("[Name]", Furre)
        data.Add("[date added]", Date.Now.ToString)
        data.Add("[date modified]", Date.Now.ToString)
        data.Add("[Access Level]", "0")
        Try
            Return db.Insert("FURRE", data)
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
    End Function

    '(5:506) add furre named {%NewMember} with the default access level "1" to the Furre Table in the database if he, she, or it doesn't exist.
    Public Function InsertFurreNamed(reader As TriggerReader) As Boolean
        Dim Furre As String = reader.ReadString
        Dim info As String
        If reader.PeekString Then
            info = reader.ReadString
        Else
            info = reader.ReadVariableOrNumber.ToString
        End If
        'Dim value As String = reader.ReadVariable.Value.ToString
        Dim db As SQLiteDatabase = New SQLiteDatabase(SQLitefile)
        Dim data As New Dictionary(Of [String], [String])()
        Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
        data.Add(MS_Name, Furre)
        data.Add("[date added]", Date.Now.ToString)
        data.Add("[date modified]", Date.Now.ToString)
        data.Add("[Access Level]", info)
        Try
            Return db.Insert("FURRE", data)
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
    End Function
    '(5407) update Database info {...} about the triggering furre will now be #.
    Public Function UpdateTriggeringFurreField(reader As TriggerReader) As Boolean
        Dim info As String = reader.ReadString
        'Dim Furre As String = reader.ReadString
        Dim Furre As String = ""
        Furre = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString.ToFurcShortName
        Dim value As Double = ReadVariableOrNumber(reader)
        Dim db As SQLiteDatabase = New SQLiteDatabase(SQLitefile)
        Dim data As New Dictionary(Of [String], [String])()
        data.Add(MS_Name, Furre)
        data.Add("[" & info & "]", value.ToString)
        data.Add("[date modified]", Date.Now.ToString)
        Try
            Return db.Update("FURRE", data, "Name='" & Furre & "'")
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
    End Function

    '(5:408) update Database info {...} about the furre named {...} will now be #.
    Public Function UpdateFurreNamed_Field(reader As TriggerReader) As Boolean
        Dim info As String = reader.ReadString
        Dim Furre As String = reader.ReadString
        'Dim Furre As String = MainEngine.MSpage.GetVariable("~Name").Value.ToString
        Dim value As String = ReadVariableOrNumber(reader, False).ToString
        Dim db As New SQLiteDatabase(SQLitefile)
        Dim data As New Dictionary(Of [String], [String])()
        data.Add(MS_Name, Furre.ToFurcShortName)
        data.Add("[" & info & "]", value)
        data.Add("[date modified]", Date.Now.ToString)
        Try
            Return db.Update("FURRE", data, "Name='" & Furre & "'")
        Catch crap As Exception
            Dim e As New ErrorLogging(crap, Me)
            Return False
        End Try
    End Function

    '(5:409) update Database info {...} about the triggering furre will now be {...}.
    Public Function UpdateTriggeringFurreFieldSTR(reader As TriggerReader) As Boolean
        Dim info As String = reader.ReadString
        'Dim Furre As String = reader.ReadString
        Dim Furre As String = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString()
        Dim value As String = reader.ReadString
        Dim db As SQLiteDatabase = New SQLiteDatabase(SQLitefile)
        Dim data As New Dictionary(Of [String], [String])()
        Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
        data.Add(MS_Name, Furre)
        data.Add("[" & info & "]", value)
        data.Add("[date modified]", Date.Now.ToString)
        Try
            db.Update("FURRE", data, "Name='" & Furre & "'")
            Return True
        Catch crap As Exception
            Dim e As New ErrorLogging(crap, Me)
            Return False
        End Try
    End Function
    '(5:410) update Database info {...} about the furre named {...} will now be {...}.
    Public Function UpdateFurreNamed_FieldSTR(reader As TriggerReader) As Boolean
        Dim info As String = reader.ReadString
        Dim Furre As String = reader.ReadString
        'Dim Furre As String = MainEngine.MSpage.GetVariable("~Name").Value.ToString
        Dim value As String = reader.ReadString
        Dim db As SQLiteDatabase = New SQLiteDatabase(SQLitefile)
        Dim data As New Dictionary(Of [String], [String])()
        Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
        data.Add(MS_Name, Furre)
        data.Add("[" & info & "]", value)
        data.Add("[date modified]", Date.Now.ToString)
        Try
            db.Update("FURRE", data, "Name='" & Furre & "'")
            Return True
        Catch crap As Exception
            Dim e As New ErrorLogging(crap, Me)
            Return False
        End Try
    End Function

    '(5:411) select Database info {...} about the triggering furre, and put it in variable %Variable.
    Public Function ReadDatabaseInfo(reader As TriggerReader) As Boolean
        Try
            Dim Info As String = reader.ReadString
            Dim Variable As Variable = reader.ReadVariable(True)
            Dim Furre As String = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString()
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            'Dim db As SQLiteDatabase = New SQLiteDatabase(file)
            Dim cmd As String = "SELECT [" & Info & "] FROM FURRE Where Name ='" & Furre & "'"
            Variable.Value = SQLiteDatabase.ExecuteScalar1(cmd)
            Return True
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
    End Function

    '(5:412) select Database info {...} about the furre named {...}, and put it in variable %Variable.
    Public Function ReadDatabaseInfoName(reader As TriggerReader) As Boolean
        Try
            Dim Info As String = reader.ReadString
            Dim Furre As String = reader.ReadString
            Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
            Dim Variable As Variable = reader.ReadVariable(True)
            ' Dim db As SQLiteDatabase = New SQLiteDatabase(file)
            Dim cmd As String = "SELECT [" & Info & "] FROM FURRE Where Name ='" & Furre & "'"
            Variable.Value = SQLiteDatabase.ExecuteScalar1(cmd)
            Return True
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
    End Function

    '(5:513) add column {...} with type {...} to the Furre table.
    Public Function AddColumn(reader As TriggerReader) As Boolean
        Dim Column As String = reader.ReadString
        Dim Type As String = reader.ReadString
        Dim db As SQLiteDatabase = New SQLiteDatabase(SQLitefile)
        db.addColumn("FURRE", "[" & Column & "]", Type)
        Return True
    End Function
    '(5:418) delete all Database info about the triggering furre.
    Public Function DeleteTriggeringFurre(reader As TriggerReader) As Boolean
        Dim Furre As String = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString()
        Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
        Dim db As SQLiteDatabase = New SQLiteDatabase(SQLitefile)
        Return 0 < SQLiteDatabase.ExecuteNonQuery("Delete from FURRE where Name='" & Furre & "'")

    End Function
    '(5:419) delete all Database info about the furre named {...}.
    Public Function DeleteFurreNamed(reader As TriggerReader) As Boolean
        Dim Furre As String = reader.ReadString
        Furre = Regex.Replace(Furre.ToLower(), REGEX_NameFilter, "")
        Dim db As SQLiteDatabase = New SQLiteDatabase(SQLitefile)
        Return 0 < SQLiteDatabase.ExecuteNonQuery("Delete from FURRE where Name='" & Furre & "'")

    End Function

    '(5:422) get the total number of records from table {...} and put it into variable %.
    Public Function GetTotalRecords(reader As TriggerReader) As Boolean
        Dim Table As String = ""
        Dim Total As Variable
        Dim num As Double = 0

        Try
            Table = reader.ReadString().Replace("[", "").Replace("]", "").Replace("'", "''")
            Total = reader.ReadVariable(True)
            Dim count As String = SQLiteDatabase.ExecuteScalar1("select count(*) from [" & Table & "]")
            Total.Value = count
            Return True
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try


    End Function
    '(5:423) take the sum of column{...} in table {...} and put it into variable %
    Public Function ColumnSum(reader As TriggerReader) As Boolean
        Dim Table As String = ""
        Dim Column As String = ""
        Dim Total As Variable
        Dim TotalSum As Double = 0

        Try
            Column = reader.ReadString
            Table = reader.ReadString
            Total = reader.ReadVariable(True)
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Dim sql As String = "SELECT " & Column & " FROM " & Table & " ;"
        Dim dt As DataTable = SQLiteDatabase.GetDataTable(sql)
        Column = Column.Replace("[", "")
        Column = Column.Replace("]", "")
        For Each row As DataRow In dt.Rows
            Try
                Dim num As Double = 0
                Double.TryParse(row(Column).ToString, num)
                TotalSum += num
                'Console.WriteLine("Calculating TotalSum {0}", TotalSum.ToString)
            Catch
            End Try
        Next row
        Total.Value = TotalSum.ToString
        Return True
    End Function

    '(5:424) in table {...} take info {...} from record index % and and put it into variable %
    Public Function RecordIndex(reader As TriggerReader) As Boolean
        Dim info As String = ""
        Dim Idx As Variable
        Dim OutVar As Variable
        Dim Table As String = ""
        Try
            Table = reader.ReadString(True).Replace("[", "").Replace("]", "").Replace("'", "''")
            info = reader.ReadString(True)
            Idx = reader.ReadVariable(True)
            OutVar = reader.ReadVariable(True)

        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Dim sql As String = "SELECT " & info & " FROM [" & Table & "] ;"
        Dim dt As DataTable = SQLiteDatabase.GetDataTable(sql)
        info = info.Replace("[", "").Replace("]", "")
        Dim i As Double = 0
        For Each row As DataRow In dt.Rows
            Try
                If i = Double.Parse(Idx.Value.ToString) Then
                    OutVar.Value = row(info)
                    Return True
                End If
                i += 1
            Catch
            End Try
        Next row
        Return True
    End Function

    '(5:550) take variable %Variable , prepare it for a query, and put it in variable %Variable   (this is your escaping call, which would depend on however you have to do it internally)
    Public Function PrepQuery(reader As TriggerReader) As Boolean
        Dim var1 As Variable
        Dim var2 As Variable
        Try
            var1 = reader.ReadVariable(True)
            var2 = reader.ReadVariable(True)
            Dim str As String = var1.Value.ToString
            str = str.Replace("'", "''")
            var2.Value = str
            Return True
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
    End Function
    '(5:551) execute query {...}. Select * from table where name=%2
    ' "Has a query been run since the last time someone asked for a result? If so, if read() then export one row.
    Public Function ExecuteQuery(reader As TriggerReader) As Boolean
        Dim str As String = ""

        Try
            str = reader.ReadString
            str = str.Trim
            SyncLock (lock)
                cache.Clear()
                QueryRun = False
                If str.ToUpper.StartsWith("SELECT") Then
                    Dim db As SQLiteDatabase = New SQLiteDatabase(Main.SQLitefile)

                    cache = db.GetValueFromTable(str)
                    QueryRun = True

                    Return cache.Count > 0
                End If
                SQLiteDatabase.ExecuteNonQuery(str)
                Return True
            End SyncLock
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

    End Function

    '(5:552) retrieve field {...} from query and put it into variable %Variable
    Public Function RetrieveQuery(reader As TriggerReader) As Boolean
        Dim Field As String
        Dim Var As Variable

        Try
            Field = reader.ReadString
            Var = reader.ReadVariable(True)

            If QueryRun Then
                SyncLock (lock)
                    If cache.Count > 0 Then

                        For Each key As KeyValuePair(Of String, Object) In cache
                            If key.Key = Field Then
                                Var.Value = cache.Item(Field)
                                Return True
                            End If
                        Next

                    End If
                End SyncLock
            End If
            Return True
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
    End Function

    '(5:553) Backup All Character phoenixspeak for the dream
    Function BackupAllPS(reader As TriggerReader) As Boolean
        Try
            If Not callbk.PSBackupRunning And Not callbk.PSRestoreRunning Then
                callbk.CurrentPS_Stage = Main.PS_BackupStage.GetList
                sendServer("ps get character.*")
            End If
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return True
    End Function

    '(5:554) backup Character named {...} phoenix speak 
    Function BackupSingleCharacterPS(reader As TriggerReader) As Boolean
        Try
            If Not callbk.PSBackupRunning And Not callbk.PSRestoreRunning And Not callbk.PSPruneRunning Then

                Dim str As String = reader.ReadString
                If str.ToUpper <> "[DREAM]" Then
                    str = str.ToFurcShortName
                Else
                    str = str.ToUpper
                End If

                Dim f As New Main.PSInfo_Struct
                f.name = str
                f.PS_ID = callbk.CharacterList.Count + 1
                callbk.CharacterList.Add(f)
                If callbk.CurrentPS_Stage <> Main.PS_BackupStage.GetSingle Then
                    callbk.CurrentPS_Stage = Main.PS_BackupStage.GetSingle
                    callbk.psReceiveCounter = 0
                    callbk.psSendCounter = 1
                    callbk.PSBackupRunning = True
                    If str <> "[DREAM]" Then
                        callbk.ServerStack.Enqueue("ps " + callbk.CharacterList.Count.ToString + " get character." + str + ".*")
                    Else
                        callbk.ServerStack.Enqueue("ps " + callbk.CharacterList.Count.ToString + " get dream.*")
                    End If
                End If
            End If
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return True
    End Function
    '(5:555) restore phoenix speak for character {...}
    Public Function RestoreCharacterPS(reader As TriggerReader) As Boolean

        Try
            Dim furre As String = reader.ReadString()
            callbk.Build_PS_CMD(furre, True)
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return True
    End Function
    '(5:556) restore all phoenxi speak characters for this dream.
    Public Function restoreAllPSData(reader As TriggerReader) As Boolean

        Dim str As String = ""

        Try
            If Not callbk.PSBackupRunning And Not callbk.PSRestoreRunning Then
                callbk.RestorePS()
            End If
            Return True
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

    End Function
    '(5:557) remove Entries older then # days from Phoenix Speak Character backup.

    Public Function PruneCharacterBackup(reader As TriggerReader) As Boolean

        Try
            Dim age As Double = ReadVariableOrNumber(reader)
            If Not callbk.PSBackupRunning And Not callbk.PSRestoreRunning Then
                callbk.PrunePS(age)
            End If
            Return True
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

    End Function

    '(5:558) restore phoenix speak characters newer then # days.
    Public Function restorePS_DataOldrThanDays(reader As TriggerReader) As Boolean

        Try
            Dim days As Double = ReadVariableOrNumber(reader)
            If Not callbk.PSBackupRunning And Not callbk.PSRestoreRunning Then
                callbk.RestorePS(days)
            End If
            Return True
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

    End Function

    Public Function VACUUM(reader As TriggerReader) As Boolean
        Dim start As Date = Date.Now
        SQLiteDatabase.ExecuteNonQuery("VACUUM")
        Dim ts As TimeSpan = Date.Now.Subtract(start)
        callbk.SendClientMessage("SYSTEM:", "Executed Vacum in " + ts.Seconds.ToString + " seconds")
        Return True
    End Function

    Public Function AbortPS(reader As TriggerReader) As Boolean
        If callbk.PSBackupRunning Or callbk.PSRestoreRunning Then
            callbk.PS_Abort()
            callbk.SendClientMessage("SYSTEM:", "Aborted PS Backup/Restore process")
        End If
        Return True
    End Function
#End Region
#Region "Effects Helper Functions"



    Sub sendServer(ByRef var As String)
        Try
            callbk.sndServer(var)
        Catch ex As Exception
            Dim log As New ErrorLogging(ex, Me)
        End Try
    End Sub

#End Region

End Class
