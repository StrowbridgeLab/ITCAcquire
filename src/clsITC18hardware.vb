Imports System.Runtime.InteropServices
Public Class clsITC18hardware
    Private Declare Auto Function ITC18_GetStructureSize Lib "d:\LabWorld\Executables\ITCMM64.dll" () As Integer
    Private Declare Auto Function ITC18_Open Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr, ByVal DeviceNumber As Integer) As Integer
    Private Declare Auto Function ITC18_Initialize Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr, ByVal ConfigNumber As Integer) As Integer
    Private Declare Auto Function ITC18_GetFIFOSize Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr) As Integer
    Private Declare Auto Function ITC18_Close Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr) As Integer
    Private Declare Auto Function ITC18_SetReadyLight Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr, ByVal Light0or1 As Integer) As Integer
    Private Declare Auto Function ITC18_SetSequence Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr, ByVal Length As Integer, ByVal Instructions As IntPtr) As Integer
    Private Declare Auto Function ITC18_SetSamplingInterval Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr, ByVal TimerTicks As Integer) As Integer
    Private Declare Auto Function ITC18_SetRange Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr, ByVal Range As IntPtr) As Integer
    Private Declare Auto Function ITC18_InitializeAcquisition Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr) As Integer
    Private Declare Auto Function ITC18_WriteFIFO Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr, ByVal Length As Integer, ByVal ITCData As IntPtr) As Integer
    Private Declare Auto Function ITC18_Start Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr, ByVal ExtTrig As Integer, ByVal OutputEnable As Integer) As Integer
    Private Declare Auto Function ITC18_Stop Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr) As Integer
    Private Declare Auto Function ITC18_GetFIFOReadAvailable Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr, ByRef Available As Integer) As Integer
    Private Declare Auto Function ITC18_ReadFIFO Lib "d:\LabWorld\Executables\ITCMM64.dll" (ByVal Device As IntPtr, ByVal Length As Integer, ByVal ITCDataBack As IntPtr) As Integer
    Private ITC() As Byte
    Private ITCpointer As IntPtr
    Friend Structure ITC18ParmType
        Friend SamplingRate As Integer
        Friend ADCranges As Integer()
        Friend ADCnames As String()
        Friend Instructions As Integer()
        Friend WaitForExtTrig As Integer
        Friend StimData As Int16()
        Friend Junk As Integer
        Friend ChannelsDone As Integer()
        Friend ExpectedNumMilliseconds As Long ' added 12 April 2012
    End Structure
    Friend Function DoesITC18DriverExist() As Boolean
        Return My.Computer.FileSystem.FileExists("D:\LabWorld\Executables\ITCMM64.dll")
    End Function
    Friend Function OpenITC18() As Integer
        Dim ITCsize As Integer
        Dim ITCpointerTemp As IntPtr
        Dim Status As Integer
        Try
            ITCsize = ITC18_GetStructureSize()
            ReDim ITC(ITCsize)
            ITCpointerTemp = Marshal.AllocHGlobal(Marshal.SizeOf(ITC(0)) * ITC.Length)
            Marshal.Copy(ITC, 0, ITCpointerTemp, ITC.Length)
            Status = ITC18_Open(ITCpointerTemp, 0)
            If Status <> 0 Then MsgBox("Bad ITC return code on Open: " + Status.ToString)
            Marshal.Copy(ITCpointerTemp, ITC, 0, ITC.Length)
            Status = ITC18_Initialize(ITCpointerTemp, 0)
            If Status <> 0 Then MsgBox("Bad ITC return code on Initialize: " + Status.ToString)
            ITCpointer = ITCpointerTemp
        Catch ex As Exception
            MsgBox("Problem in OpenITC18: " + ex.Message)
        End Try
        Return Status
    End Function
    Friend Function GetITC18FIFOSize() As Integer
        Dim FIFOsize As Integer
        Try
            FIFOsize = ITC18_GetFIFOSize(ITCpointer)
            Return FIFOsize
        Catch ex As Exception
            MsgBox("Problem in GetITC18FIFOSize: " + ex.Message)
            Return -1
        End Try
    End Function
    Friend Function CloseITC18() As Integer
        Dim Status As Integer
        Status = ITC18_SetReadyLight(ITCpointer, 0)
        If Status <> 0 Then MsgBox("Bad ITC return code on SetReadyLight: " + Status.ToString)
        Status = ITC18_Close(ITCpointer)
        If Status <> 0 Then MsgBox("Bad ITC return code on Close: " + Status.ToString)
        Marshal.FreeHGlobal(ITCpointer)
        '   MsgBox("Everything closed up okay.")
        Return Status
    End Function
    Friend Function RunITC18(ByVal ITC As ITC18ParmType) As Int16()
        Dim Status As Integer
        Dim ServiceFIFORequired As Boolean = False
        Dim i As Long, j As Long
        Dim FIFOBlockSize As Integer = -1
        Dim FIFOavailable As Integer
        If ITC.StimData.Length >= ITC18_GetFIFOSize(ITCpointer) Then
            ServiceFIFORequired = True
            FIFOBlockSize = 16 * 1024 ' 16k samples
            '  MsgBox("FIFO issue")
        End If
        Dim mReadData As Int16()
        '   Try
        With ITC
            Dim NumInstructions As Integer = .Instructions.Length
            Dim InstructionsPointer As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(.Instructions(0)) * .Instructions.Length)
            Marshal.Copy(.Instructions, 0, InstructionsPointer, .Instructions.Length)
            Status = ITC18_SetSequence(ITCpointer, NumInstructions, InstructionsPointer)
            If Status <> 0 Then MsgBox("Bad ITC return code on SetSequence: " + Status.ToString)
            Marshal.FreeHGlobal(InstructionsPointer)
            Status = ITC18_SetSamplingInterval(ITCpointer, .SamplingRate)
            If Status <> 0 Then MsgBox("Bad ITC return code on SetSamplingInterval: " + Status.ToString)
            Dim RangePointer As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(.ADCranges(0)) * .ADCranges.Length)
            Marshal.Copy(.ADCranges, 0, RangePointer, .ADCranges.Length)
            Status = ITC18_SetRange(ITCpointer, RangePointer)
            If Status <> 0 Then MsgBox("Bad ITC return code on SetRange: " + Status.ToString)
            Marshal.FreeHGlobal(RangePointer)
            Status = ITC18_InitializeAcquisition(ITCpointer)
            If Status <> 0 Then MsgBox("Bad ITC return code on InitializeAcquisition: " + Status.ToString)
            If ServiceFIFORequired Then
                Dim SW As New System.Diagnostics.Stopwatch
                SW.Reset()
                Dim StimOffset As Long = 0
                Dim ReadOffset As Long = 0
                Dim mReadChuck As Int16()
                Dim mStimChunk As Int16()
                Dim StimDataPointer As IntPtr
                Dim ReadDataPointer As IntPtr
                Dim NumFIFOblocksCompleted As Single = 0
                Dim StimSize As Integer
                ReDim mReadChuck(FIFOBlockSize - 1)
                ReDim mStimChunk(FIFOBlockSize - 1)
                ReDim mReadData(.StimData.Length - 1)

                For i = 0 To FIFOBlockSize - 1 ' load first chunk
                    mStimChunk(i) = .StimData(i + StimOffset)
                Next
                StimOffset = StimOffset + FIFOBlockSize
                StimDataPointer = Marshal.AllocHGlobal(Marshal.SizeOf(mStimChunk(0)) * mStimChunk.Length)
                Marshal.Copy(mStimChunk, 0, StimDataPointer, mStimChunk.Length)
                Status = ITC18_WriteFIFO(ITCpointer, mStimChunk.Length - 1, StimDataPointer)
                If Status <> 0 Then MsgBox("Bad ITC return code on WriteFIFO serviceFIFOmode: " + Status.ToString)
                Marshal.FreeHGlobal(StimDataPointer)

                For i = 0 To FIFOBlockSize - 1 ' load second chunk so there will be data in FIFO while read out first chunck
                    mStimChunk(i) = .StimData(i + StimOffset)
                Next
                StimOffset = StimOffset + FIFOBlockSize
                StimDataPointer = Marshal.AllocHGlobal(Marshal.SizeOf(mStimChunk(0)) * mStimChunk.Length)
                Marshal.Copy(mStimChunk, 0, StimDataPointer, mStimChunk.Length)
                Status = ITC18_WriteFIFO(ITCpointer, mStimChunk.Length - 1, StimDataPointer)
                If Status <> 0 Then MsgBox("Bad ITC return code on WriteFIFO serviceFIFOmode: " + Status.ToString)
                Marshal.FreeHGlobal(StimDataPointer)

                Status = ITC18_Start(ITCpointer, .WaitForExtTrig, 1)
                If Status <> 0 Then MsgBox("Bad ITC return code on Start in FIFOserviceMode: " + Status.ToString)
                SW.Start()

                Do
                    Do
                        Status = ITC18_GetFIFOReadAvailable(ITCpointer, FIFOavailable)
                        If Status <> 0 Then MsgBox("Bad ITC return code on GetFIFOreadAvailable: " + Status.ToString)
                        Application.DoEvents()
                    Loop While FIFOavailable < (FIFOBlockSize + 8) And SW.ElapsedMilliseconds < (3000 + .ExpectedNumMilliseconds)
                    If FIFOavailable >= (FIFOBlockSize + 4) And (ReadOffset + FIFOBlockSize) <= .StimData.Length Then
                        ReDim mReadChuck(FIFOBlockSize - 1)
                        ReadDataPointer = Marshal.AllocHGlobal(Marshal.SizeOf(mReadChuck(0)) * mReadChuck.Length)
                        Status = ITC18_ReadFIFO(ITCpointer, FIFOBlockSize, ReadDataPointer)
                        If Status <> 0 Then MsgBox("Bad ITC return code on ReadFIFO in FIFOserviceMode: " + Status.ToString)
                        Marshal.Copy(ReadDataPointer, mReadChuck, 0, mReadChuck.Length)
                        Marshal.FreeHGlobal(ReadDataPointer)
                        For i = 0 To mReadChuck.Length - 1
                            mReadData(i + ReadOffset) = mReadChuck(i)
                        Next
                        ReadOffset = ReadOffset + FIFOBlockSize
                        NumFIFOblocksCompleted += 1
                        If StimOffset + FIFOBlockSize <= .StimData.Length Then
                            For i = 0 To FIFOBlockSize - 1 ' load next chunk
                                mStimChunk(i) = .StimData(i + StimOffset)
                            Next
                            StimOffset = StimOffset + FIFOBlockSize
                            StimDataPointer = Marshal.AllocHGlobal(Marshal.SizeOf(mStimChunk(0)) * mStimChunk.Length)
                            Marshal.Copy(mStimChunk, 0, StimDataPointer, mStimChunk.Length)
                            Status = ITC18_WriteFIFO(ITCpointer, mStimChunk.Length - 1, StimDataPointer)
                            If Status <> 0 Then MsgBox("Bad ITC return code on WriteFIFO serviceFIFOmode: " + Status.ToString)
                            Marshal.FreeHGlobal(StimDataPointer)
                        End If
                    Else
                        Status = ITC18_Stop(ITCpointer)
                        If Status <> 0 Then MsgBox("Bad ITC return code on Stop in FIFOserviceMode Final: " + Status.ToString)
                        If Not Form1.mStopRequested Then
                            ' get last partial chuck of data
                            Status = ITC18_GetFIFOReadAvailable(ITCpointer, FIFOavailable)
                            If Status <> 0 Then MsgBox("Bad ITC return code on GetFIFOreadAvailable Final: " + Status.ToString)
                            If FIFOavailable > 0 Then
                                ReDim mReadChuck(FIFOavailable - 1)
                                ReadDataPointer = Marshal.AllocHGlobal(Marshal.SizeOf(mReadChuck(0)) * mReadChuck.Length)
                                Status = ITC18_ReadFIFO(ITCpointer, FIFOavailable, ReadDataPointer)
                                If Status <> 0 Then MsgBox("Bad ITC return code on ReadFIFO in FIFOserviceMode Final: " + Status.ToString)
                                Marshal.Copy(ReadDataPointer, mReadChuck, 0, mReadChuck.Length)
                                Marshal.FreeHGlobal(ReadDataPointer)
                                For i = 0 To mReadChuck.Length - 1
                                    If i + ReadOffset > mReadData.Length - 1 Then Exit For
                                    mReadData(i + ReadOffset) = mReadChuck(i)
                                Next
                                NumFIFOblocksCompleted += 0.5
                            End If
                            Exit Do
                        End If
                    End If
                Loop
                '      MsgBox("Num FIFO blocks = " + NumFIFOblocksCompleted.ToString)
                SW.Stop()
                ' End of FIFO servicing option

            Else
                ' Episode will fit in FIFO so FIFO servicing is not required

                Dim StimDataPointer As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(.StimData(0)) * .StimData.Length)
                Marshal.Copy(.StimData, 0, StimDataPointer, .StimData.Length)
                Dim StimSize As Integer = .StimData.Length - 1
                Status = ITC18_WriteFIFO(ITCpointer, StimSize, StimDataPointer)
                If Status <> 0 Then MsgBox("Bad ITC return code on WriteFIFO: " + Status.ToString)
                Marshal.FreeHGlobal(StimDataPointer)
                Status = ITC18_Start(ITCpointer, .WaitForExtTrig, 1)
                If Status <> 0 Then MsgBox("Bad ITC return code on Start: " + Status.ToString)
                Do
                    Status = ITC18_GetFIFOReadAvailable(ITCpointer, FIFOavailable)
                    If Status <> 0 Then MsgBox("Bad ITC return code on GetFIFOreadAvailable: " + Status.ToString)
                    Application.DoEvents()
                Loop While FIFOavailable < StimSize
                Status = ITC18_Stop(ITCpointer)
                If Status <> 0 Then MsgBox("Bad ITC return code on Stop: " + Status.ToString)
                ReDim mReadData(.StimData.Length - 1)
                Dim ReadDataPointer As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(mReadData(0)) * mReadData.Length)
                Status = ITC18_ReadFIFO(ITCpointer, StimSize, ReadDataPointer)
                If Status <> 0 Then MsgBox("Bad ITC return code on ReadFIFO: " + Status.ToString)
                Marshal.Copy(ReadDataPointer, mReadData, 0, mReadData.Length)
                Marshal.FreeHGlobal(ReadDataPointer)
            End If

        End With
        Form1.mStopRequested = False
        Return mReadData
        '   Catch ex As Exception
        '     MsgBox("Internal exception: " + ex.Message)
        '   Return Nothing
        '   End Try
    End Function
    Friend Function ReadTelegraphs(ByVal TelegraphAChannelStr As String, ByVal TelegraphBChannelStr As String) As Single()
        Dim NewTelegraphs As Single()
        Dim TelegraphAChannel As Long = -1
        Dim TelegraphBChannel As Long = -1
        ReDim NewTelegraphs(1)
        Dim TempL As Long
        NewTelegraphs(0) = 1
        NewTelegraphs(1) = 1
        Try
            TempL = CLng(TelegraphAChannelStr)
            If TempL >= 0 And TempL < 8 Then
                TelegraphAChannel = TempL
            End If
        Catch ex As Exception
        End Try
        Try
            TempL = CLng(TelegraphBChannelStr)
            If TempL >= 0 And TempL < 8 Then
                TelegraphBChannel = TempL
            End If
        Catch ex As Exception
        End Try
        Dim TelegraphParms As New ITC18ParmType
        Dim NumInstructions As Integer = 2
       
        ReDim TelegraphParms.Instructions(NumInstructions - 1)
        Dim ADCchanCode As Integer, TeleACode As Integer, TeleBCode As Integer

        Select Case TelegraphAChannel
            Case 0
                ADCchanCode = &H0
            Case 1
                ADCchanCode = &H80
            Case 2
                ADCchanCode = &H100
            Case 3
                ADCchanCode = &H180
            Case 4
                ADCchanCode = &H200
            Case 5
                ADCchanCode = &H280
            Case 6
                ADCchanCode = &H300
            Case 7
                ADCchanCode = &H380
            Case -1
                ADCchanCode = &H780 ' skip
            Case Else
                MsgBox("Unexpected ADCchanCode in SealTest")
                Stop
        End Select
        TeleACode = ADCchanCode


        Select Case TelegraphBChannel
            Case 0
                ADCchanCode = &H0
            Case 1
                ADCchanCode = &H80
            Case 2
                ADCchanCode = &H100
            Case 3
                ADCchanCode = &H180
            Case 4
                ADCchanCode = &H200
            Case 5
                ADCchanCode = &H280
            Case 6
                ADCchanCode = &H300
            Case 7
                ADCchanCode = &H380
            Case -1
                ADCchanCode = &H780 ' skip
            Case Else
                MsgBox("Unexpected ADCchanCode in SealTest")
                Stop
        End Select
        TeleBCode = ADCchanCode
        TelegraphParms.Instructions(0) = TeleACode Or &H3800 ' skip output
        TelegraphParms.Instructions(1) = TeleBCode Or &H3800 Or &H4000 Or &H8000 ' and execute
        TelegraphParms.SamplingRate = CInt((100 / (1.0! * NumInstructions)) / 1.25) ' 100 us per point
        ReDim TelegraphParms.ADCranges(7)
        For i As Long = 0 To 7
            TelegraphParms.ADCranges(i) = 0 ' +/- 10 volts
        Next
        Dim Junk As Integer = 20 * NumInstructions
        TelegraphParms.Junk = Junk
        Dim StimSize As Long = Junk + CLng(NumInstructions * 100) + 11 ' do 100 points
        ReDim TelegraphParms.StimData(StimSize)
        Try
            Dim NewData As Int16() = RunITC18(TelegraphParms)
            Dim RunSumA As Double = 0
            Dim RunSumB As Double = 0
            For i As Long = 41 To 242 Step 2
                RunSumA += NewData(i)
                RunSumB += NewData(i + 1)
            Next
            RunSumA = RunSumA / 100
            RunSumB = RunSumB / 100
            If TelegraphAChannel <> -1 Then
                Select Case RunSumA
                    Case 1290 To 1310
                        NewTelegraphs(0) = 0.5
                    Case 2585 To 2605
                        NewTelegraphs(0) = 1
                    Case 3875 To 3895
                        NewTelegraphs(0) = 2
                    Case 5170 To 5190
                        NewTelegraphs(0) = 5
                    Case 6465 To 6495
                        NewTelegraphs(0) = 10
                    Case 7765 To 7785
                        NewTelegraphs(0) = 20
                    Case 9060 To 9090
                        NewTelegraphs(0) = 50
                    Case 10355 To 10375
                        NewTelegraphs(0) = 100
                End Select
            End If
            If TelegraphBChannel <> -1 Then
                Select Case RunSumB
                    Case 1290 To 1310
                        NewTelegraphs(1) = 0.5
                    Case 2585 To 2605
                        NewTelegraphs(1) = 1
                    Case 3875 To 3895
                        NewTelegraphs(1) = 2
                    Case 5170 To 5190
                        NewTelegraphs(1) = 5
                    Case 6465 To 6495
                        NewTelegraphs(1) = 10
                    Case 7765 To 7785
                        NewTelegraphs(1) = 20
                    Case 9060 To 9090
                        NewTelegraphs(1) = 50
                    Case 10355 To 10375
                        NewTelegraphs(1) = 100
                End Select
            End If

            Return NewTelegraphs
        Catch ex As Exception
            MsgBox("Problem in ReadTelegraphsOnITC18: " + ex.Message)
            Return NewTelegraphs
        End Try
        Return NewTelegraphs
    End Function
    Friend Function ZeroITC18Outputs() As Boolean
        Dim ZeroParms As New ITC18ParmType
        Dim NumInstructions As Integer = 6
        ReDim ZeroParms.Instructions(NumInstructions - 1)
        ZeroParms.Instructions(0) = &H780 Or &H0 ' skip ADC with DAC0
        ZeroParms.Instructions(0) = &H780 Or &H800 ' skip ADC with DAC1
        ZeroParms.Instructions(0) = &H780 Or &H1000 ' skip ADC with DAC2
        ZeroParms.Instructions(0) = &H780 Or &H1800 ' skip ADC with DAC3
        ZeroParms.Instructions(0) = &H780 Or &H2000 ' skip ADC with DigOut0
        ZeroParms.Instructions(0) = &H780 Or &H2800 Or &H4000 Or &H8000 ' skip ADC with DigOut1 and execute
        ZeroParms.SamplingRate = CInt((100 / (1.0! * NumInstructions)) / 1.25) ' 100 us per point
        ReDim ZeroParms.ADCranges(7)
        Dim Junk As Integer = 20 * NumInstructions
        ZeroParms.Junk = Junk
        Dim StimSize As Long = Junk + CLng(NumInstructions * 100) + 11 ' do 100 points
        ReDim ZeroParms.StimData(StimSize)
        Try
            Dim NewData As Int16() = RunITC18(ZeroParms)
            Return True
        Catch ex As Exception
            MsgBox("Problem in ZeroITC18Outputs: " + ex.Message)
            Return False
        End Try
    End Function
    Friend Function SetDACValues(ByVal NewDACValues As Int16()) As Boolean
        Dim DACparms As New ITC18ParmType
        Dim NumInstructions As Integer = 4
        ReDim DACparms.Instructions(NumInstructions - 1)
        DACparms.Instructions(0) = &H780 Or &H0 ' skip ADC with DAC0
        DACparms.Instructions(1) = &H780 Or &H800 ' skip ADC with DAC1
        DACparms.Instructions(2) = &H780 Or &H1000 ' skip ADC with DAC2
        DACparms.Instructions(3) = &H780 Or &H1800 Or &H4000 Or &H8000 ' skip ADC with DAC3 and execute
        DACparms.SamplingRate = CInt((100 / (1.0! * NumInstructions)) / 1.25) ' 100 us per point
        ReDim DACparms.ADCranges(7)
        Dim Junk As Integer = 20 * NumInstructions
        DACparms.Junk = Junk
        Dim StimSize As Long = Junk + CLng(NumInstructions * 100)  ' do 100 points
        ReDim DACparms.StimData(StimSize)
        Dim j As Integer
        For i As Integer = 0 To StimSize - 1 Step NumInstructions
            For j = 0 To NumInstructions - 1
                DACparms.StimData(i + j) = NewDACValues(j)
            Next
        Next
        Try
            Dim NewData As Int16() = RunITC18(DACparms)
            Return True
        Catch ex As Exception
            MsgBox("Problem in SetDACValues: " + ex.Message)
            Return False
        End Try
    End Function
    Friend Function RunSealTest(ByVal ADCchanNumber As Integer, ByVal DACchanNumber As Integer, ByVal ADCFullScaleVoltCode As Integer,
                                ByVal BaselineVoltage As Single, ByVal ADCfactor As Single) As Single()
        Dim ADCchanCode As Integer
        Select Case ADCchanNumber
            Case 0
                ADCchanCode = &H0
            Case 1
                ADCchanCode = &H80
            Case 2
                ADCchanCode = &H100
            Case 3
                ADCchanCode = &H180
            Case 4
                ADCchanCode = &H200
            Case 5
                ADCchanCode = &H280
            Case 6
                ADCchanCode = &H300
            Case 7
                ADCchanCode = &H380
            Case Else
                MsgBox("Unexpected ADCchanCode in SealTest")
                Stop
        End Select
        Dim DACchanCode As Integer
        Select Case DACchanNumber
            Case 0
                DACchanCode = &H0
            Case 1
                DACchanCode = &H800
            Case 2
                DACchanCode = &H1000
            Case 3
                DACchanCode = &H1800
            Case Else
                MsgBox("Unexpected DACchanCode in SealTest")
                Stop
        End Select
        Dim SealDurationMs As Single = 10 ' 10 ms
        Dim SealSampleRateUs As Integer = 20 ' 50 kHz or 20 us
        Dim NumPoints As Long = CLng(SealDurationMs * (1.0# / (SealSampleRateUs / 1000)))

        Dim SealParms As New ITC18ParmType
        Dim NumInstructions As Integer = 1
        ReDim SealParms.Instructions(NumInstructions - 1)
        SealParms.Instructions(0) = ADCchanCode Or DACchanCode Or &H4000 Or &H8000 '  execute
        SealParms.SamplingRate = CInt((SealSampleRateUs / (1.0! * NumInstructions)) / 1.25) ' 100 us per point
        Dim Junk As Integer = 20 * NumInstructions
        SealParms.Junk = Junk
        ReDim SealParms.ADCranges(7)
        Select Case ADCFullScaleVoltCode
            Case 0 ' 1 volt FS
                SealParms.ADCranges(0) = 3
            Case 1 ' 2 volts FS
                SealParms.ADCranges(0) = 2
            Case 2 ' 5 volts FS
                SealParms.ADCranges(0) = 1
            Case 3 ' 10 volts FS
                SealParms.ADCranges(0) = 0
            Case Else
                MsgBox("Unexpected value in select case ADCranges")
                Return Nothing
                Exit Function
        End Select
        SealParms.WaitForExtTrig = 0
        Dim StimSize As Long = Junk + CLng(NumInstructions * NumPoints) + 11
        ReDim SealParms.StimData(StimSize)
        Dim PointsPerMs As Integer = CInt(1.0! / (SealSampleRateUs / 1000))
        Dim PulseStartIndex As Integer = 2 * PointsPerMs
        Dim PulseStopIndex As Integer = 6 * PointsPerMs
        Dim Factor As Single = 3.2 * 50
        Dim PulseAmp As Int16 = 20 * Factor
        If BaselineVoltage <> 0 Then
            For i As Integer = 0 To StimSize - 1
                SealParms.StimData(i) = CInt(BaselineVoltage * Factor)
            Next
        End If
        For i As Integer = Junk + PulseStartIndex To Junk + PulseStopIndex
            SealParms.StimData(i) += PulseAmp
        Next
        Try
            Dim NewData As Int16() = RunITC18(SealParms)
            Dim ReturnData As Single() ' after junk points are removed
            ReDim ReturnData(NumPoints - 1)
            Select Case ADCFullScaleVoltCode
                Case 0
                    Factor = 1024 / 32767
                Case 1
                    Factor = 2048 / 32767
                Case 2
                    Factor = 5120 / 32767
                Case 3
                    Factor = 10240 / 32767
                Case Else
                    MsgBox("Unexpected ADC Full Scale on data read out")
                    Stop
            End Select
            For i As Integer = 0 To NumPoints - 1
                ReturnData(i) = NewData(i + Junk) * Factor * ADCfactor
            Next
            Return ReturnData
        Catch ex As Exception
            MsgBox("Problem in ZeroITC18Outputs: " + ex.Message)
            Return Nothing
        End Try
    End Function
    Friend Function GenerateStimData(ByVal mP As NewProtocolTypeVer7OldStyle) As ITC18ParmType
        Dim ITCparm As New ITC18ParmType
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim NumPointsPerChan As Integer = CInt(mP.SweepWindow * mP.PointsPerMs)
        Dim mDAC As Integer(,) = ReadAcquireDACdata("R:\DACstim.dat", NumPointsPerChan)
        Dim mTTL As Integer(,) = ReadAcquireTTLdata("R:\TTLstim.dat", NumPointsPerChan)
        Dim mAuxTTL As Long() = Nothing

        Application.DoEvents()

        If mP.StatValue(15) = 1 Then
            mAuxTTL = ReadAuxTTLbuffer()
        End If

        Dim NumReadChannels As Integer = 0
        For p As Integer = 0 To 7
            If mP.GenArray(3 + p) > 0 Then NumReadChannels += 1
        Next
        Dim NumWriteChannels As Integer = 1
        For p As Integer = 0 To 3
            If mP.GenArray(27 + p) > 0 Then NumWriteChannels += 1
        Next
        Dim NumInstructions As Integer = NumWriteChannels
        If NumReadChannels > NumWriteChannels Then NumInstructions = NumReadChannels
        ReDim ITCparm.Instructions(NumInstructions - 1)
        ReDim ITCparm.ChannelsDone(7)
        ReDim ITCparm.ADCnames(7)
        Dim TempL As Long
        j = 0
        For i = 0 To NumInstructions - 1
            For k = j To 7
                TempL = -1
                If mP.GenArray(3 + k) > 0 Then
                    Select Case k
                        Case 0
                            TempL = &H0
                            ITCparm.ADCnames(i) = "ADC0"
                            ITCparm.ChannelsDone(k) = 1
                        Case 1
                            TempL = &H80
                            ITCparm.ADCnames(i) = "ADC1"
                            ITCparm.ChannelsDone(k) = 1
                        Case 2
                            TempL = &H100
                            ITCparm.ADCnames(i) = "ADC2"
                            ITCparm.ChannelsDone(k) = 1
                        Case 3
                            TempL = &H180
                            ITCparm.ADCnames(i) = "ADC3"
                            ITCparm.ChannelsDone(k) = 1
                        Case 4
                            TempL = &H200
                            ITCparm.ADCnames(i) = "ADC4"
                            ITCparm.ChannelsDone(k) = 1
                        Case 5
                            TempL = &H280
                            ITCparm.ADCnames(i) = "ADC5"
                            ITCparm.ChannelsDone(k) = 1
                        Case 6
                            TempL = &H300
                            ITCparm.ADCnames(i) = "ADC6"
                            ITCparm.ChannelsDone(k) = 1
                        Case 7
                            TempL = &H380
                            ITCparm.ADCnames(i) = "ADC7"
                            ITCparm.ChannelsDone(k) = 1
                        Case Else
                            MsgBox("Unexpected value in select case for ADC")
                            Return Nothing
                            Exit Function
                    End Select
                    Exit For
                End If
            Next ' on k
            If TempL <> -1 Then
                ITCparm.Instructions(i) = TempL
                j = k + 1
            Else
                ITCparm.Instructions(i) = &H780 ' skip input
            End If
            TempL = -1
        Next ' on i

        ITCparm.Instructions(0) = ITCparm.Instructions(0) Or &H2800 ' digital outs
        j = 1
        For i = 0 To 3
            If mP.GenArray(27 + i) > 0 Then
                Select Case i
                    Case 0
                        TempL = &H0
                    Case 1
                        TempL = &H800
                    Case 2
                        TempL = &H1000
                    Case 3
                        TempL = &H1800
                    Case Else
                        MsgBox("Unexpected values at select DAC")
                        Return Nothing
                        Exit Function
                End Select
                ITCparm.Instructions(j) = ITCparm.Instructions(j) Or TempL
                j += 1
            End If
        Next ' on i
        If j <> NumInstructions Then
            For i = j To NumInstructions - 1
                ITCparm.Instructions(i) = ITCparm.Instructions(i) Or &H3800 ' skip output
            Next
        End If

        TempL = NumInstructions - 1
        ITCparm.Instructions(TempL) = ITCparm.Instructions(TempL) Or &H4000 Or &H8000 ' execute
        ITCparm.SamplingRate = CInt((mP.TimePerPoint / (1.0! * NumInstructions)) / 1.25)
        If ITCparm.SamplingRate < 1 Then
            MsgBox("Problem with sampling rate <1")
            Return Nothing
            Exit Function
        End If

        ReDim ITCparm.ADCranges(7)
        For i = 0 To 7
            Select Case mP.GenArray(11 + i)
                Case 0 ' 1 volt FS
                    ITCparm.ADCranges(i) = 3
                Case 1 ' 2 volts FS
                    ITCparm.ADCranges(i) = 2
                Case 2 ' 5 volts FS
                    ITCparm.ADCranges(i) = 1
                Case 3 ' 10 volts FS
                    ITCparm.ADCranges(i) = 0
                Case Else
                    MsgBox("Unexpected value in select case ADCranges")
                    Return Nothing
                    Exit Function
            End Select
        Next

        If mP.GenArray(52) > 0 Then
            ITCparm.WaitForExtTrig = 1
        Else
            ITCparm.WaitForExtTrig = 0
        End If

        Dim Junk As Integer = 20 * NumInstructions
        ITCparm.Junk = Junk
        Dim StimSize As Long = Junk + CLng(NumInstructions * mP.NumPoints) + 11
        ReDim ITCparm.StimData(StimSize)
        Dim Offset As Integer = 1
        j = 0
        For k = 0 To 3
            If mP.GenArray(27 + k) > 0 Then
                For i = Offset + j To StimSize Step NumInstructions
                    ITCparm.StimData(i) = mDAC(k, 0)
                Next
                j += 1
            End If
        Next ' on k

        TempL = 0
        Offset = 0
        Dim NeedToIncludeAuxTTL As Boolean = False
        If My.Computer.FileSystem.FileExists("R:\AuxTTL.dat") And mP.StatValue(15) = 1 Then NeedToIncludeAuxTTL = True
        For i = Junk + Offset To Junk + Offset + (mP.NumPoints * NumInstructions) Step NumInstructions
            For j = 0 To 3
                If mTTL(j, TempL) > 0 Then
                    Select Case j
                        Case 0
                            ITCparm.StimData(i) = ITCparm.StimData(i) Or 1
                        Case 1
                            ITCparm.StimData(i) = ITCparm.StimData(i) Or 2
                        Case 2
                            ITCparm.StimData(i) = ITCparm.StimData(i) Or 4
                        Case 3
                            ITCparm.StimData(i) = ITCparm.StimData(i) Or 8
                        Case Else
                            MsgBox("Unexpected value in select case mTTL")
                            Return Nothing
                            Exit Function
                    End Select
                End If

                If NeedToIncludeAuxTTL And CLng(i / NumInstructions) >= Junk And j = 0 Then ' changed to just do it for the first channel
                    ITCparm.StimData(i) = ITCparm.StimData(i) Or mAuxTTL((CLng(i / NumInstructions) - Junk))
                End If

            Next ' j

            TempL += 1
        Next ' on i

        Offset = 1
        j = 0
        For k = 0 To 3
            If mP.GenArray(27 + k) > 0 Then
                TempL = 0
                For i = Junk + Offset + j To Junk + Offset + (NumInstructions * mP.NumPoints) Step NumInstructions
                    ITCparm.StimData(i) = mDAC(k, TempL)
                    TempL += 1
                Next
                j += 1
            End If
        Next ' on k

        Return ITCparm
    End Function
    Friend Function WriteNewITC18dataAsEpisodeNew(ByVal mP As NewProtocolTypeVer7OldStyle, ByVal ITCparm As ITC18ParmType,
                                      ByVal ReadData As Int16(), ByVal FileName As String, ByVal Telegraphs As Single()) As Boolean
        Dim NewData As Single()
        Dim NewDataInt As Int16()
        mP.DrugName = mP.StatName(10)
        If mP.GenArray(27) = 0 Then mP.GenArray(47) = 0 ' make sure SaveStim is disabled if amp is disabled
        If mP.GenArray(28) = 0 Then mP.GenArray(48) = 0
        If mP.GenArray(29) = 0 Then mP.GenArray(49) = 0
        If mP.GenArray(30) = 0 Then mP.GenArray(50) = 0

        Dim NumInstructions As Integer = ITCparm.Instructions.Length
        Dim NumPointsPerChan As Integer = CInt(mP.SweepWindow * mP.PointsPerMs)

        Dim mEpi As clsEpisode
        mEpi = New clsEpisode
        With mEpi
            .SetExptDesc(mP.StatName(11))
            .SetGenDescriptionString(mP.StatName(12) + " - " + mP.StatName(13) + " - " + mP.StatName(14))
            .SetGenData(mP.GenArray)
            .SetDACdata(mP.DacData)
            .SetTTLdata(mP.TTLData)
            .SetDrugLevel(mP.DrugLevel)
            .SetDrugName(mP.DrugName)
            .SetDrugTime(mP.DrugTime)
            .SetWCTime(mP.WCTime)
            .SetMicrosecPerPoint(mP.TimePerPoint)
            .SetProgramType(mP.ProgramType)
            .SetAcquisitionDeviceName("ITC18")
            .SetAnalysisComment(mP.AnalysisComment)
            .SetComment(mP.Comment)

        End With

        Dim mDAC As Integer(,) = ReadAcquireDACdata("R:\DACstim.dat", NumPointsPerChan)

        ReDim NewData(mP.NumPoints)
        ReDim NewDataInt(mP.NumPoints)
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim Offset As Integer = 3 ' to compensate for ITC pipeline
        Dim TempL As Long = 0
        Dim Factor As Single = 0
        Dim NewKey As String

        '   mP.AllTracesDict.Clear()
        For i = 0 To 3
            If mP.GenArray(27 + i) > 0 Then

                ' Process Vm trace
                TempL = ActualVmADCChannel(mP, i)
                ITCparm.ChannelsDone(TempL) = 0
                For k = 0 To 7
                    If "ADC" + TempL.ToString = ITCparm.ADCnames(k) Then Exit For
                Next

                Select Case mP.GenArray(11 + TempL)
                    Case 0
                        Factor = 1024.0! / 32767.0!
                    Case 1
                        Factor = 2048.0! / 32767.0!
                    Case 2
                        Factor = 5120.0! / 32767.0!
                    Case 3
                        Factor = 10240.0! / 32767.0!
                    Case Else
                        MsgBox("Unexpected value in select case read ADC fullscale Vm trace")
                        Stop
                End Select

                Select Case mP.GenArray(19 + TempL)
                    Case 0
                        ' do nothing
                    Case 1
                        Factor = Factor / 5.0!
                    Case 2
                        Factor = Factor / 10.0!
                    Case 3
                        Factor = Factor / 20.0!
                    Case 4
                        Factor = Factor / 50.0!
                    Case 5
                        Factor = Factor / 100.0!
                    Case Else
                        MsgBox("Unexpected value in select case read ADC ext gain Vm trace")
                        Stop
                End Select

                Select Case mP.GenArray(31 + i)
                    Case 0 ' AxoClamp 0.1L CC
                        If mP.GenArray(3 + TempL) = 1 Then
                            Factor = Factor / 10.0! ' to compensate for 10xVm output
                        Else
                            ' do nothing if 1xVm output
                        End If
                    Case 1 ' AxoClamp 1L CC
                        If mP.GenArray(3 + TempL) = 1 Then
                            Factor = Factor / 10.0!
                        Else
                            Factor = Factor / 10.0! ' looks like a mistake but was there in the VB6 version
                        End If
                    Case 2 ' AxoPatch VC
                        Factor = Factor / 10.0!
                    Case 3 ' AxoPatch CC
                        Factor = Factor / 10.0!
                    Case Else
                        MsgBox("Non AxoClamp or AxoPatch amp in select case read for Vm trace")
                        Stop
                End Select
                '   Dim TempAcc As Double
                '   TempAcc = 0
                For j = 0 To mP.NumPoints
                    '    NewData(j) = Factor * ReadData(ITCparm.Junk + Offset + (j * NumInstructions) + k)
                    '    TempAcc += NewData(j)
                    NewDataInt(j) = ReadData(ITCparm.Junk + Offset + (j * NumInstructions) + k)
                Next
                '   TempAcc = TempAcc / j
                NewKey = ActualVmKey(mP, i)
                '    mP.AllTracesDict.Add(NewKey, NewData.Clone)
                NewKey = NewKey.Replace(" ", "")
                mEpi.SaveTrace(NewKey, NewDataInt, Factor, "Voltage")

                ' Now process Im trace
                TempL = ActualImADCChannel(mP, i)
                ITCparm.ChannelsDone(TempL) = 0
                For k = 0 To 7
                    If "ADC" + TempL.ToString = ITCparm.ADCnames(k) Then Exit For
                Next

                Select Case mP.GenArray(11 + TempL)
                    Case 0
                        Factor = 1024.0! / 32767.0!
                    Case 1
                        Factor = 2048.0! / 32767.0!
                    Case 2
                        Factor = 5120.0! / 32767.0!
                    Case 3
                        Factor = 10240.0! / 32767.0!
                    Case Else
                        MsgBox("Unexpected value in select case read ADC fullscale Im trace")
                        Stop
                End Select

                Select Case mP.GenArray(19 + TempL)
                    Case 0
                        ' do nothing
                    Case 1
                        Factor = Factor / 5.0!
                    Case 2
                        Factor = Factor / 10.0!
                    Case 3
                        Factor = Factor / 20.0!
                    Case 4
                        Factor = Factor / 50.0!
                    Case 5
                        Factor = Factor / 100.0!
                    Case Else
                        MsgBox("Unexpected value in select case read ADC ext gain Im trace")
                        Stop
                End Select

                Factor = Factor / Telegraphs(i)
                Select Case mP.GenArray(31 + i)
                    Case 0 ' AxoClamp 0.1L CC
                        Factor = Factor * 10.0!
                    Case 1 ' AxoClamp 1L CC
                        Factor = Factor * 10.0!
                    Case 2 ' AxoPatch VC
                        ' do nothing
                    Case 3 ' AxoPatch CC
                        ' do nothing
                    Case Else
                        MsgBox("Non AxoClamp or AxoPatch amp in select case read for Im trace")
                        Stop
                End Select

                '  TempAcc = 0
                For j = 0 To mP.NumPoints
                    '   NewData(j) = Factor * ReadData(ITCparm.Junk + Offset + (j * NumInstructions) + k)
                    '       TempAcc += NewData(j)
                    NewDataInt(j) = ReadData(ITCparm.Junk + Offset + (j * NumInstructions) + k)
                Next
                '     TempAcc = TempAcc / j

                NewKey = ActualImKey(mP, i)
                NewKey = NewKey.Replace(" ", "")
                '   mP.AllTracesDict.Add(NewKey, NewData.Clone)
                mEpi.SaveTrace(NewKey, NewDataInt, Factor, "Current")

                ' now process Stimulus trace
                If mP.GenArray(47 + i) > 0 Then
                    NewKey = "Stimulus Amp " + Chr(65 + i) + "/99"
                    Select Case mP.GenArray(31 + i)
                        Case 0
                            Factor = 3.2
                        Case 1
                            Factor = 0.32
                        Case 2
                            Factor = 3.2 * 50.0!
                        Case 3
                            Factor = 3.2 * 5.0!
                        Case Else
                            MsgBox("Unexpected value in select case for read Stimulus case factor")
                            Stop
                    End Select
                    For j = 0 To mP.NumPoints - 2
                        NewDataInt(j + 2) = mDAC(i, j)
                    Next
                    NewDataInt(0) = NewDataInt(2)
                    NewDataInt(1) = NewDataInt(2)
                    NewKey = NewKey.Replace(" ", "")
                    mEpi.SaveTrace(NewKey, NewDataInt, 1 / Factor, "Stimulus")
                    '   mP.AllTracesDict.Add(NewKey, NewData.Clone)
                End If ' mP.GenArray(47,i)
            End If  ' mp.GenArray(27+i)
        Next ' on i

        For i = 0 To 7
            If ITCparm.ChannelsDone(i) <> 0 Then
                TempL = i
                For k = 0 To 7
                    If "ADC" + TempL.ToString = ITCparm.ADCnames(k) Then Exit For
                Next
                Select Case mP.GenArray(11 + TempL)
                    Case 0
                        Factor = 1024.0! / 32767.0!
                    Case 1
                        Factor = 2048.0! / 32767.0!
                    Case 2
                        Factor = 5120.0! / 32767.0!
                    Case 3
                        Factor = 10240.0! / 32767.0!
                    Case Else
                        MsgBox("Unexpected value in select case read ADC fullscale random channel")
                        Stop
                End Select

                Select Case mP.GenArray(19 + TempL)
                    Case 0
                        ' do nothing
                    Case 1
                        Factor = Factor / 5.0!
                    Case 2
                        Factor = Factor / 10.0!
                    Case 3
                        Factor = Factor / 20.0!
                    Case 4
                        Factor = Factor / 50.0!
                    Case 5
                        Factor = Factor / 100.0!
                    Case Else
                        MsgBox("Unexpected value in select case read ADC ext gain random channel")
                        Stop
                End Select

                NewKey = ""
                Select Case mP.GenArray(3 + 1)
                    Case 0 ' disabled
                        Factor = 0
                        NewKey = "Disabled"
                    Case 1
                        Factor = Factor / 10.0!
                        NewKey = "Volt"
                    Case 2
                        NewKey = "Cur"
                    Case 3
                        NewKey = "Cur"
                    Case 4
                        NewKey = "Volt"
                    Case 7
                        Factor = Factor / 100.0!
                        NewKey = "Field"
                    Case 8
                        Factor = Factor / 1000.0!
                        NewKey = "Field"
                    Case 9
                        Factor = Factor / 10000.0!
                        NewKey = "Field"
                    Case 10
                        NewKey = "Photodiode"
                    Case 11
                        NewKey = "Poly2 Lamda"
                    Case 12
                        NewKey = "Frame Clock"
                    Case Else
                        MsgBox("Unknown channel type in read random channel")
                        Stop
                End Select
                NewKey = NewKey + " ADC" + TempL.ToString + "/" + mP.GenArray(3 + i).ToString
                NewKey = NewKey.Replace(" ", "")
                For j = 0 To mP.NumPoints
                    '     NewData(j) = Factor * ReadData(ITCparm.Junk + Offset + (j * NumInstructions) + k)
                    NewDataInt(j) = ReadData(ITCparm.Junk + Offset + (j * NumInstructions) + k)
                Next
                mEpi.SaveTrace(NewKey, NewDataInt, Factor, "Other")
                '    mP.AllTracesDict.Add(NewKey, NewData.Clone)

            End If ' channelsDone
        Next ' on i 0-7
        mEpi.SaveEpisode(FileName)

        'xx   Dim RetOkay As Boolean = WriteEpisode(mP, FileName)
        'xx   If Not RetOkay Then MsgBox("Problem on WriteEpisode")
        Return True
    End Function

    ' Private functions below here
    Private Function GetInitValues(ByRef TraceData As Double()) As Double
        Dim TempD As Double = 0
        For i As Integer = 0 To 9
            TempD += TraceData(i)
        Next
        Return TempD / 10.0
    End Function
    Private Function ActualImADCChannel(ByVal mP As NewProtocolTypeVer7OldStyle, ByVal Index As Long) As Long
        Select Case mP.GenArray(31 + Index)
            Case 0, 1, 2, 3, 8, 9
                Return mP.GenArray(35 + Index)
            Case 4, 5, 6, 7
                Return -1
            Case Else
                Return -1
        End Select
    End Function
    Private Function ActualVmADCChannel(ByVal mP As NewProtocolTypeVer7OldStyle, ByVal Index As Long) As Long
        Select Case mP.GenArray(31 + Index)
            Case 0, 1, 2, 3, 8, 9
                Return mP.GenArray(39 + Index)
            Case 4, 5, 6, 7
                Return -1
            Case Else
                Return -1
        End Select
    End Function
    Private Function ActualImKey(ByVal mP As NewProtocolTypeVer7OldStyle, ByVal Index As Long) As String
        Dim ADC As Long = ActualImADCChannel(mP, Index)
        Select Case mP.GenArray(31 + Index)
            Case 0, 1, 2, 3, 8, 9
                Return "Cur ADC" + ADC.ToString + "/" + mP.GenArray(3 + ADC).ToString
            Case 4, 5, 6, 7
                Return -1
            Case Else
                Return -1
        End Select
    End Function
    Private Function ActualVmKey(ByVal mP As NewProtocolTypeVer7OldStyle, ByVal Index As Long) As String
        Dim ADC As Long = ActualVmADCChannel(mP, Index)
        Select Case mP.GenArray(31 + Index)
            Case 0, 1, 2, 3, 8, 9
                Return "Volt ADC" + ADC.ToString + "/" + mP.GenArray(3 + Index).ToString
            Case 4, 5, 6, 7
                Return -1.ToString
            Case Else
                Return -1.ToString
        End Select
    End Function
End Class
