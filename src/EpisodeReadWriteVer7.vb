Imports Microsoft.VisualBasic
Imports System.Math
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Module EpisodeReadWriteVer7
    Friend Structure NewProtocolTypeVer7OldStyle
        Friend ProgramType As Int16 ' 25 for Synapse-generation programs
        Friend ProgramVersion As Int32 ' 7 for this version
        Friend ProtocolBytes As Int32
        Friend SweepWindow As Single ' ms
        Friend TimePerPoint As Single ' in us per chan
        Friend NumPoints As Int32
        Friend WCTime As Single
        Friend DrugTime As Single
        Friend DrugLevel As Single
        Friend SimulatedData As Int32
        Friend GenArray() As Single
        Friend TTLData(,) As Single
        Friend DacData(,) As Single
        Friend StatBox() As Int32
        Friend StatValue() As Single
        Friend StatName() As String
        Friend Comment As String
        Friend AnalysisComment As String
        Friend FileName As String
        Friend TempKeys() As String
        Friend InitValues() As Single
        Friend junk As Byte()
        Friend DacAWFFileNames() As String
        Friend NumBytesInFile As Long
        Friend NumChannels As Long
        Friend Volt() As Double
        Friend Cur() As Double
        Friend Stim() As Double
        Friend ExtraTrace() As Double
        Friend AllTracesDict As Dictionary(Of String, Single())
        Friend InitValuesDict As Dictionary(Of String, Single)
        Friend CellDescription As String ' statName(0)
        Friend ExptDescription As String ' statName(1)
        Friend InternalSolution As String ' statName(2)
        Friend ExternalSolution As String ' statName(3)
        Friend DrugName As String ' statName(4)
        Friend MsPerPoint As Double
        Friend PointsPerMs As Double
    End Structure
    Friend Function ReadEpisodeOldStyle(Optional ByVal FileName As String = "", Optional ByVal ReadHeaderOnly As Boolean = False, Optional ByVal DisplayInOutputWindow As Boolean = False, Optional ByVal TraceKeyString As String = "") As NewProtocolTypeVer7OldStyle
        Dim i As Long
        Dim TempStr As String = ""
        Dim FileNameLength As Int16
        Dim enc As New System.Text.ASCIIEncoding()
        Dim TempBytes() As Byte
        Dim Fobject As FileInfo = My.Computer.FileSystem.GetFileInfo(FileName)
        Dim TempSingle() As Single
        Dim mH As NewProtocolTypeVer7OldStyle
        mH = CreateBlankEpisodeOldStyle()
        Dim TempFileName As String = ""
        Dim IsOldStyleDatFile As Boolean = False

        Dim localmP As New NewProtocolTypeVer7OldStyle
        If FileName = "" Then
            FileName = "r:\zData.dat"
        End If
        If FileName.ToUpper.Trim = "BROWSE" Then
            Dim cmdD = New System.Windows.Forms.OpenFileDialog
            cmdD.ShowDialog()
            FileName = cmdD.FileName
            If System.IO.File.Exists(FileName) Then
                System.IO.File.Copy(FileName, "r:\zData.dat")
                mH.FileName = FileName
                FileName = "r:\zData.dat"
            End If
        End If
        Dim FileNumber As Int32

        TempFileName = "r:\TempData\" + My.Computer.FileSystem.GetName(FileName)
        If Not My.Computer.FileSystem.FileExists(TempFileName) Then
            ' always try looking in ram drive first
            TempFileName = FileName ' not there so revert to regular filename that was passed
        End If

        Application.DoEvents()
        Try
            If My.Computer.FileSystem.GetFileInfo(FileName).Length = 0 Then
                ' empty file 
                '   MsgBox("Hit file name for empty file")
                mH.NumBytesInFile = 0
                Return mH
                Exit Function
            End If
        Catch
            MsgBox(Err.Description)
            If TraceKeyString.Length > 0 Then Err.Clear()
        End Try

        Dim bReaderTest As New BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read))
        With mH
            .ProgramType = bReaderTest.ReadInt16
            .ProgramVersion = bReaderTest.ReadInt32
            .ProtocolBytes = bReaderTest.ReadInt32
            .SweepWindow = bReaderTest.ReadSingle
            .TimePerPoint = bReaderTest.ReadSingle
            If .ProgramType <> 25 Then IsOldStyleDatFile = True
            If Abs(.ProtocolBytes) > 20000 Then IsOldStyleDatFile = True
            If Abs(.TimePerPoint) > 2000 Then IsOldStyleDatFile = True
        End With
        bReaderTest.Close()

        If IsOldStyleDatFile Then
            Dim bReader As New BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read))
            Dim JunkI As Int16
            With mH
                JunkI = bReader.ReadInt16 ' size of array    
                JunkI = bReader.ReadInt16 ' clampMode(0)
                JunkI = bReader.ReadInt16 ' clampMode(1)
                .TimePerPoint = bReader.ReadSingle
                .ProtocolBytes = bReader.ReadInt32
                .SweepWindow = bReader.ReadSingle
                .NumPoints = 1 + bReader.ReadInt32

                .junk = bReader.ReadBytes(168)
                .TTLData(2, 5) = bReader.ReadInt16
                .TTLData(2, 0) = .TTLData(2, 5) ' make global enable a copy
                Dim SIUmode As Int16 = bReader.ReadInt16 ' is 0 for normal single
                .TTLData(2, 6) = bReader.ReadSingle
                Dim SIUdelay2 As Single = bReader.ReadSingle
                .TTLData(2, 7) = -1
                .TTLData(2, 8) = -1
                .TTLData(2, 9) = -1
                .TTLData(2, 10) = 0
                '     .TTLData(2, 11) = .TTLData(2, 5)
                '    .TTLData(2, 12) = .TTLData(2, 6)

                .TTLData(2, 12) = bReader.ReadSingle
                .TTLData(2, 13) = bReader.ReadSingle
                .TTLData(2, 14) = bReader.ReadInt16
                .TTLData(1, 2) = bReader.ReadInt16
                .TTLData(1, 0) = .TTLData(2, 2) ' copy for global enable
                .TTLData(1, 3) = bReader.ReadSingle
                .TTLData(1, 4) = bReader.ReadSingle
                .TTLData(3, 2) = bReader.ReadInt16
                .TTLData(3, 0) = .TTLData(3, 2) ' copy for global enable
                .TTLData(3, 3) = bReader.ReadSingle
                .TTLData(3, 4) = bReader.ReadSingle

                .NumBytesInFile = Fobject.Length
                .NumChannels = CInt(.NumBytesInFile - .ProtocolBytes) / ((.NumPoints - 1) * 4)
                .ProgramType = 25
                .ProgramVersion = 6
                .MsPerPoint = .TimePerPoint / 1000
                .PointsPerMs = 1 / .MsPerPoint
                .DrugTime = 1614
                .WCTime = 1614
                .AnalysisComment = "PreSynapse style header"
                .TempKeys(0) = "Cur ADC0/3"
                .TempKeys(1) = "Volt ADC1/3"
                If .NumChannels > 2 Then
                    For k = 2 To .NumChannels - 1
                        .TempKeys(k) = "Trace" + (k + 1).ToString + " ADC7/3"
                    Next
                End If
                .FileName = FileName
                .GenArray(0) = 1
                .GenArray(1) = .SweepWindow
                .GenArray(2) = .TimePerPoint
                .GenArray(3) = 3
                .GenArray(4) = 1
                For k = 11 To 18
                    .GenArray(k) = 3
                Next
                .GenArray(20) = 1
                .GenArray(27) = 1
                .GenArray(31) = 3
                .GenArray(36) = 2
                .GenArray(37) = 4
                .GenArray(38) = 6
                .GenArray(39) = 1
                .GenArray(40) = 3
                .GenArray(41) = 5
                .GenArray(42) = 7
                .GenArray(43) = 9
                .GenArray(44) = 9
                .GenArray(45) = 9
                .GenArray(46) = 9
                .GenArray(47) = 1
                .GenArray(53) = 200
                .GenArray(55) = 35
            End With
            bReader.Close()

            If Not ReadHeaderOnly Then
                FileNumber = FreeFile()
                FileOpen(FileNumber, FileName, OpenMode.Binary)
                ReDim TempSingle((mH.NumChannels * mH.NumPoints) - 1)
                FileGet(FileNumber, TempSingle, mH.ProtocolBytes)
                Dim TempTrace As Double()
                ReDim TempTrace(mH.NumPoints)
                Dim iCount As Long
                For k As Long = mH.NumChannels - 1 To 0 Step -1
                    iCount = 0
                    For j As Long = k To TempSingle.Length - mH.NumChannels Step mH.NumChannels
                        TempTrace(iCount) = CDbl(TempSingle(j))
                        iCount += 1
                    Next
                    If mH.TempKeys(k).Length > 0 Then
                        mH.AllTracesDict.Add(mH.TempKeys(k), TempTrace.Clone)
                    Else
                        mH.AllTracesDict.Add("Trace" + (k + 1).ToString, TempTrace.Clone)
                    End If

                    ' add init value stuff
                Next
                FileClose(FileNumber)
            End If
        Else
            Dim bReader As New BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read))
            With mH
                .ProgramType = bReader.ReadInt16
                .ProgramVersion = bReader.ReadInt32
                .ProtocolBytes = bReader.ReadInt32
                .SweepWindow = bReader.ReadSingle
                .TimePerPoint = bReader.ReadSingle
                .NumPoints = bReader.ReadInt32
                .WCTime = bReader.ReadSingle
                .DrugTime = bReader.ReadSingle
                .DrugLevel = bReader.ReadSingle
                .SimulatedData = bReader.ReadInt32
                .MsPerPoint = .TimePerPoint / 1000
                .PointsPerMs = 1 / .MsPerPoint
                .junk = bReader.ReadBytes(10) ' advance through some hidden bytes
                ReDim .GenArray(55)
                For i = 0 To 55
                    .GenArray(i) = bReader.ReadSingle
                Next

                ReDim .TTLData(3, 16)
                For j = 0 To 3
                    .junk = bReader.ReadBytes(10)
                    For i = 0 To 16
                        .TTLData(j, i) = bReader.ReadSingle
                    Next
                Next

                ReDim .DacData(3, 41)
                ReDim .DacAWFFileNames(3)
                For j = 0 To 3
                    .junk = bReader.ReadBytes(10)
                    For i = 0 To 41
                        .DacData(j, i) = bReader.ReadSingle
                    Next
                    FileNameLength = bReader.ReadInt16
                    If FileNameLength > 512 Then Stop
                    ReDim TempBytes(FileNameLength - 1)
                    TempBytes = bReader.ReadBytes(FileNameLength)
                    .DacAWFFileNames(j) = enc.GetString(TempBytes)
                Next

                ReDim .StatBox(24)
                ReDim .StatValue(24)
                ReDim .StatName(24)
                For i = 0 To 24
                    .StatBox(i) = bReader.ReadInt32
                Next
                For i = 0 To 24
                    .StatValue(i) = bReader.ReadSingle
                Next
                ' .DrugLevel = .StatValue(6) ' added 8.4.09
                For i = 0 To 24
                    FileNameLength = bReader.ReadInt16
                    If FileNameLength > 512 Then Stop
                    ReDim TempBytes(FileNameLength - 1)
                    TempBytes = bReader.ReadBytes(FileNameLength)
                    .StatName(i) = enc.GetString(TempBytes)
                Next

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .Comment = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .AnalysisComment = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .FileName = enc.GetString(TempBytes)

                ReDim .TempKeys(19)
                For i = 0 To 19
                    FileNameLength = bReader.ReadInt16
                    If FileNameLength > 512 Then Stop
                    ReDim TempBytes(FileNameLength - 1)
                    TempBytes = bReader.ReadBytes(FileNameLength)
                    .TempKeys(i) = enc.GetString(TempBytes)
                Next

                .NumBytesInFile = Fobject.Length
                .NumChannels = CInt(.NumBytesInFile - .ProtocolBytes) / (.NumPoints * 4)
                bReader.Close()

                ReDim .InitValues(19)
                ReDim TempSingle(14)
                FileNumber = FreeFile()
                FileOpen(FileNumber, FileName, OpenMode.Binary)
                For i = 0 To .NumChannels - 1
                    If .TempKeys(i).Length > 0 Then
                        FileGet(FileNumber, TempSingle, .ProtocolBytes + (i * ((.NumPoints + 1) * 4)))
                        .InitValues(i) = VectorMeanVer7(ConvertToDouble(TempSingle))
                    End If
                Next
                FileClose(FileNumber)
            End With

            '    TempStatusString = "Episode is " + (mH.SweepWindow / 1000).ToString + " sec long; acquired at " + ((1.0! / mH.TimePerPoint) * 1000).ToString + " kHz"
            '    frmMain.WriteStatusText(TempStatusString)

            If Not ReadHeaderOnly Then
                If TraceKeyString.Length = 0 Then
                    FileNumber = FreeFile()
                    FileOpen(FileNumber, FileName, OpenMode.Binary)
                    With mH
                        ReDim .Volt(.NumPoints)
                        ReDim .Cur(.NumPoints)
                        ReDim .Stim(.NumPoints)
                        ReDim TempSingle(.NumPoints)

                        FileGet(FileNumber, TempSingle, .ProtocolBytes)
                        .Volt = ConvertToDouble(TempSingle)
                        FileGet(FileNumber, TempSingle)
                        .Cur = ConvertToDouble(TempSingle)
                        If .NumChannels = 3 Then
                            FileGet(FileNumber, TempSingle)
                            .Stim = ConvertToDouble(TempSingle)
                        End If

                    End With
                    FileClose(FileNumber)
                Else
                    If TraceKeyString.ToUpper = "TracesToDict".ToUpper Then
                        FileNumber = FreeFile()
                        FileOpen(FileNumber, FileName, OpenMode.Binary)
                        ReDim TempSingle(mH.NumPoints)
                        For k As Long = 0 To 19
                            If mH.TempKeys(k).Length > 0 Then
                                FileGet(FileNumber, TempSingle, mH.ProtocolBytes + (k * ((mH.NumPoints + 1) * 4)))
                                mH.AllTracesDict.Add(mH.TempKeys(k), TempSingle)
                                ' add init value stuff

                            End If
                        Next
                        FileClose(FileNumber)
                    Else
                        ' get specific trace and put it in extraTrace property
                        Dim RequestedChannelNumber As Long = -1
                        ReDim mH.ExtraTrace(mH.NumPoints)
                        ReDim TempSingle(mH.NumPoints)
                        For k As Long = 0 To 19
                            If mH.TempKeys(k) = TraceKeyString Then
                                RequestedChannelNumber = k
                                Exit For
                            End If
                        Next
                        If RequestedChannelNumber <> -1 Then
                            FileNumber = FreeFile()
                            FileOpen(FileNumber, FileName, OpenMode.Binary)
                            FileGet(FileNumber, TempSingle, mH.ProtocolBytes + (RequestedChannelNumber * ((mH.NumPoints + 1) * 4)))
                            mH.ExtraTrace = ConvertToDouble(TempSingle)
                            FileClose(FileNumber)
                        Else
                            MsgBox("Could not find trace with key = " + TraceKeyString)
                        End If
                    End If ' TracesToDict
                End If ' traceKeyString length
            End If ' read header only
        End If ' isOldStyleDatFile
        Return mH
    End Function
    Friend Function CreateBlankEpisodeOldStyle() As NewProtocolTypeVer7OldStyle
        Dim mTempEpi As New NewProtocolTypeVer7OldStyle
        With mTempEpi
            ReDim .GenArray(55)
            ReDim .TTLData(3, 16)
            ReDim .DacData(3, 41)
            ReDim .DacAWFFileNames(3)
            ReDim .StatBox(24)
            ReDim .StatValue(24)
            ReDim .StatName(24)
            ReDim .TempKeys(19)
            ReDim .InitValues(19)
            For i As Long = 0 To 19
                .TempKeys(i) = ""
            Next
            For i As Long = 0 To 24
                .StatName(i) = ""
            Next
            For i As Long = 0 To 3
                .DacAWFFileNames(i) = ""
            Next
            .FileName = ""
            .Comment = ""
            .AnalysisComment = ""
            .AllTracesDict = New Dictionary(Of String, Single())
            .InitValuesDict = New Dictionary(Of String, Single)
        End With
        Return mTempEpi
    End Function
    Friend Function ReadAcquireDACdata(ByVal ExistingFileName As String, ByVal LastPoint As Integer) As Integer(,)
        If Not My.Computer.FileSystem.FileExists(ExistingFileName) Then
            MsgBox("Cannot find DAC data file: " + ExistingFileName)
            Return Nothing
            Exit Function
        End If
        Dim bReader As New BinaryReader(File.Open(ExistingFileName, FileMode.Open, FileAccess.Read))
        Dim TempA As Integer(,)
        ReDim TempA(3, LastPoint)
        For i As Integer = 0 To LastPoint
            For j = 0 To 3
                TempA(j, i) = bReader.ReadInt16
            Next
        Next
        bReader.Close()

        Return TempA
    End Function
    Friend Function ReadAcquireTTLdata(ByVal ExistingFileName As String, ByVal LastPoint As Integer) As Integer(,)
        If Not My.Computer.FileSystem.FileExists(ExistingFileName) Then
            MsgBox("Cannot find TTL data file: " + ExistingFileName)
            Return Nothing
            Exit Function
        End If
        Dim bReader As New BinaryReader(File.Open(ExistingFileName, FileMode.Open, FileAccess.Read))
        Dim TempA As Integer(,)
        ReDim TempA(3, LastPoint)
        For i As Integer = 0 To LastPoint
            For j As Integer = 0 To 3
                TempA(j, i) = bReader.ReadInt16
            Next
        Next
        bReader.Close()

        Return TempA
    End Function

    Friend Function ReadAuxTTLbuffer() As Long()
        Dim sReader As New StreamReader("R:\AuxTTL.dat")
        Dim TempStr As String
        Dim iCount As Long = 0
        Do
            TempStr = sReader.ReadLine
            iCount += 1
        Loop Until sReader.EndOfStream
        Dim Buffer As Long()
        ReDim Buffer(iCount - 2) ' first line is desc
        sReader.Close()
        Dim sReader2 As New StreamReader("R:\AuxTTL.dat")
        TempStr = sReader2.ReadLine() ' desc
        For i As Long = 0 To Buffer.Length - 1
            Buffer(i) = CLng(sReader2.ReadLine)
        Next
        sReader2.Close()
        Return Buffer
    End Function
    Friend Function ReadAuxTTLdesc() As String
        Dim TempStr As String
        Dim sReader2 As New StreamReader("R:\AuxTTL.dat")
        TempStr = sReader2.ReadLine() ' desc
        sReader2.Close()
        Return TempStr
    End Function
    Private Function ConvertToDouble(ByRef inArray As Single()) As Double()
        Dim i As Int32
        Dim TempArray() As Double
        ReDim TempArray(inArray.Length - 1)
        For i = 0 To inArray.Length - 1
            TempArray(i) = CType(inArray(i), Double)
        Next
        Return TempArray
    End Function
    Private Function VectorMeanVer7(ByVal inArray As Double()) As Double
        Dim acc As Double = 0
        Dim count As Double = 0
        Dim i As Long
        For i = 0 To inArray.Length - 1
            acc = acc + inArray(i)
            count = count + 1
        Next
        Return acc / count
    End Function

End Module
