Imports TheCodeKing.Net.Messaging
Imports System.Threading
Public Class Form1
    Private AcquireListener As IXDListener
    Private JoystickListener As IXDListener
    Private GenericListener As IXDListener
    Private Broadcast As IXDBroadcast
    Private mITC18 As New clsITC18hardware
    Private mCurrentTelegraphs As Single()
    Private mStaticDACValues As Int16()
    Friend mSealTestBaselineVm As Single
    Private mTempText As String = ""
    Private mTempInternalName As String = ""
    Private mTempInternalExtras As String = ""
    Private mLastEpisodeProcessed As String = ""
    Private mLastEpiProcessed As NewProtocolTypeVer7
    Private mOverrideFilename As String = ""
    Friend mUseAuxTTL As Boolean = False
    Private mNextTrialToDo As String = ""
    Private mParts() As String
    Friend mStopRequested As Boolean = False
    Private ProgVer As Single = 2.08 ' last revised 16 June 2012
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        mITC18.CloseITC18()
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not mITC18.DoesITC18DriverExist Then
            MsgBox("Cannot find ITC18 driver (it should be D:\LabWorld\Executables\ITCMM64.dll)")
            End
        End If
        ReDim mCurrentTelegraphs(3)
        For i As Integer = 0 To 3
            mCurrentTelegraphs(i) = 1
        Next
        AcquireListener = XDListener.CreateListener(XDTransportMode.MailSlot)
        AddHandler AcquireListener.MessageReceived, AddressOf MsgFromAcquire
        AcquireListener.RegisterChannel("MsgToITC18FromAcquire")

        JoystickListener = XDListener.CreateListener(XDTransportMode.MailSlot)
        AddHandler JoystickListener.MessageReceived, AddressOf MsgFromJoystick
        JoystickListener.RegisterChannel("MsgToITCHardwareFromJoystick")

        GenericListener = XDListener.CreateListener(XDTransportMode.MailSlot)
        AddHandler GenericListener.MessageReceived, AddressOf GenericAcquisitionMessage
        GenericListener.RegisterChannel("ToGenericAcquisition")

        Broadcast = XDBroadcast.CreateBroadcast(XDTransportMode.MailSlot, False)
        Dim RetCode As Integer = mITC18.OpenITC18()
        Dim FIFOsize As Integer = mITC18.GetITC18FIFOSize
        If RetCode = 0 Then
            Me.Text = "ITC18 Hardware running (ver " + ProgVer.ToString + " with " + Format(FIFOsize / 1024, "F0") + " KB FIFO)"
            mITC18.ZeroITC18Outputs()
        Else
            MsgBox("Problem opening ITC18 (RC=" + RetCode.ToString + ") so program is ending.")
            End
        End If

        ' now work on initializing the GUI stuff

        With ckbExtraInternalStuff
            .Items.Add("Alexa488")
            .Items.Add("Alexa594")
            .Items.Add("Biocytin")
            .Items.Add("OGB-1")
            .Items.Add("OGB-2")
        End With
        With cboInternal
            .Items.Add("#11 K-Methylsulfate no EGTA")
            .Items.Add("#15 CsCl TEA 0.2 EGTA")
            .Items.Add("#19 Cs-Methanesulfonane TEA QX314 0.2 EGTA")
            .Items.Add("#20 K-Methylsulfate 10 EGTA")
            .Items.Add("#22 K-Methylsulfate QX314 no EGTA")
            .Items.Add("#25 CsCl TEA QX314 0.2 EGTA")
            .Items.Add("#27 K-Methylsulfate 0.2 EGTA")
            .Items.Add("Other")
            .SelectedIndex = 6
        End With
        With cboAmps
            .Items.Add("Amp A")
            .Items.Add("Amp B")
            .Items.Add("Amp C")
            .Items.Add("Amp D")
            .SelectedIndex = 0
        End With

        mLastEpiProcessed = Nothing

        ReDim mStaticDACValues(3)
        '   mStaticDACValues(0) = -40 * 3.2 * 50 ' to set at -40
        TelegraphTimer.Enabled = True
        TrialPendingTimer.Enabled = True
    End Sub

    ' Public functions below here

    Friend Function SetSealTestTimer(ByVal newState As Boolean) As Boolean
        SealTestTimer.Enabled = newState
        Return True
    End Function
    Friend Function RestoreStaticDAClevels() As Boolean
        If Not IsNothing(mLastEpiProcessed.FileName) Then
            ' set DACs to correct level based on protocol in last episode acquired
            For AmpNum As Integer = 0 To 3
                mStaticDACValues(AmpNum) = 0 ' default value
                If mLastEpiProcessed.GenArray(27 + AmpNum) = 1 Then
                    ' Amp enabled
                    If mLastEpiProcessed.GenArray(31 + AmpNum) = 2 And mLastEpiProcessed.DacData(AmpNum, 0) = 1 Then
                        ' amp in VC mode and DAC enabled on that channel
                        Dim LastCmdVoltage As Single = mLastEpiProcessed.DacData(AmpNum, 5)
                        mStaticDACValues(AmpNum) = 3.2 * 50 * LastCmdVoltage
                    End If
                End If
            Next
        End If
        mITC18.SetDACValues(mStaticDACValues)
        Return True
    End Function

    ' Private functions below here
    Friend Function UpdateTelegraphs() As Single()
        Dim RetValues As Single()
        ReDim RetValues(1)
        RetValues = mITC18.ReadTelegraphs(txtAmpATelegraphChan.Text, txtAmpBTelegraphChan.Text)
        If RetValues(0) = 0 Or RetValues(1) = 0 Then
            Me.Text = "One telegraph connection appears to be disconnected."
        End If
        If RetValues(0) <> 0 Then mCurrentTelegraphs(0) = RetValues(0)
        If RetValues(1) <> 0 Then mCurrentTelegraphs(1) = RetValues(1)
        If pnlHeartbeat.Text = "|" Then
            pnlHeartbeat.Text = "-"
        Else
            pnlHeartbeat.Text = "|"
        End If
        Return RetValues
    End Function
    Private Function MsgFromAcquire(ByVal sender As Object, ByVal e As XDMessageEventArgs) As Boolean
        Dim MessageWhole As String = e.DataGram.Message.Trim
        '      MsgBox("Message: " + MessageWhole)
        Dim Parts As String() = MessageWhole.Split(" ")
        Dim Argument As String = MessageWhole
        '    Dim Argument As String = MessageWhole.Substring(Parts(0).Length, MessageWhole.Length - Parts(0).Length).Trim

        Select Case Argument.ToUpper
            Case "DoTrial".ToUpper
                mNextTrialToDo = "one"
            Case "StopTrial".ToUpper
                StopTrial()
            Case "SealTest0".ToUpper
            
            Case "SealTest1".ToUpper
               
            Case "SealTest2".ToUpper
            
            Case "SealTest3".ToUpper
            
            Case Else
                MsgBox("Unknown message sent from Acquire: " + Argument)
        End Select
        Return True
    End Function
    Private Function MsgFromJoystick(ByVal sender As Object, ByVal e As XDMessageEventArgs) As Boolean
        Dim MessageWhole As String = e.DataGram.Message
        Dim Parts As String() = MessageWhole.Split("=")
        Select Case Parts(0).ToUpper.Trim
            Case "Telegraphs".ToUpper
                Dim SubParts As String() = Parts(1).Trim.Split(" ")
                For i As Integer = 0 To 3
                    mCurrentTelegraphs(i) = CSng(SubParts(i))
                Next
                '    pnlTelegraphs.Text = MessageWhole

            Case Else
                MsgBox("Unknown message from Joystick: " + MessageWhole)
        End Select
        Return True
    End Function

    Private Function GenericAcquisitionMessage(ByVal sender As Object, ByVal e As XDMessageEventArgs) As Boolean
        Dim MessageWhole As String = e.DataGram.Message
        '   MsgBox("Got it: " + MessageWhole)
        If My.Computer.FileSystem.FileExists("R:\TempHeader.dat") And My.Computer.FileSystem.FileExists("R:\DACstim.dat") And My.Computer.FileSystem.FileExists("R:\TTLstim.dat") Then
            mParts = MessageWhole.Split("|")
            mOverrideFilename = mParts(3).Replace("$", "\")
            mNextTrialToDo = "one"

            Return True
        Else
            MsgBox("Need to run one episode through Acquire before attempting to auto run from Skinner")
            Return False
        End If
    End Function

    Private Sub SetTelegraphText()
        pnlTelegraphs.Text = mTempText
        mTempText = ""
    End Sub
    Private Function DoTrial() As Boolean
        Dim SW As New System.Diagnostics.Stopwatch
        If txtBrainRegion.Text.Contains("?") Or txtBrainRegion.Text = "Brain Region" Then
            MsgBox("You need to specify a brain region for the experiment before acquiring data with the ITC-18 program.")
            Return False
            Exit Function
        End If
        If txtExptGoal.Text.Contains("?") Or txtExptGoal.Text = "Experiment Goal" Then
            MsgBox("You need to specify a goal for the experiment before acquiring data with the ITC-18 program.")
            Return False
            Exit Function
        End If
        If My.Computer.FileSystem.FileExists("R:\TempHeader.dat") And My.Computer.FileSystem.FileExists("R:\DACstim.dat") And My.Computer.FileSystem.FileExists("R:\TTLstim.dat") Then
            TelegraphTimer.Enabled = False
            TrialPendingTimer.Enabled = False
            Dim NewEpi As NewProtocolTypeVer7OldStyle = CreateBlankEpisodeOldStyle()
            NewEpi = ReadEpisodeOldStyle("R:\TempHeader.dat", True)
            Dim Parts As String() = NewEpi.AnalysisComment.Split("=")
            Dim ActualFileName As String = Parts(1).Trim
            If mOverrideFilename.Length > 0 Then ActualFileName = mOverrideFilename
            If chkUseAuxTTLAWF.Checked And My.Computer.FileSystem.FileExists("R:\AuxTTL.dat") Then
                NewEpi.StatValue(15) = 1
                NewEpi.StatName(15) = ReadAuxTTLdesc()
            End If
            '    Try
            SW.Reset()
            SW.Start()
            pnlStatus.Text = "starting to write stim data ..."
            '    System.Threading.Thread.Sleep(10)
            Application.DoEvents()
            Dim TempParms As clsITC18hardware.ITC18ParmType = mITC18.GenerateStimData(NewEpi)
            '      MsgBox("Finished writing stim data")
            TempParms.ExpectedNumMilliseconds = NewEpi.SweepWindow
            If TempParms.StimData.Length > mITC18.GetITC18FIFOSize Then
                pnlStatus.Text = "finished writing stim data and now acquiring using FIFO buffer ..."
            Else
                pnlStatus.Text = "finished writing stim data and now acquiring ..."
            End If
            If chkStartRaster.Checked Then
                Dim TempStr As String
                If chkRasterExtTrig.Checked Then
                    TempStr = "TakeMovieExtTrigger"
                Else
                    TempStr = "TakeMovieNoTrigger"
                End If
                TempStr += "?" + ActualFileName.Substring(0, ActualFileName.Length - 4) + ".Image " ' takes off .dat suffix
                Broadcast.SendToChannel("FromITC18", TempStr)
            End If
            '     System.Threading.Thread.Sleep(100)
            Dim NewData As Int16() = mITC18.RunITC18(TempParms)
            pnlStatus.Text = "acquired data and now writing it to disk ..."
            '       System.Threading.Thread.Sleep(100)

            If mOverrideFilename.Length > 0 Then
                NewEpi.Comment = mParts(4)
            End If
            NewEpi.StatName(11) = txtExptGoal.Text.Trim
            NewEpi.StatName(12) = txtBrainRegion.Text.Trim
            NewEpi.StatName(13) = mTempInternalName
            NewEpi.StatName(14) = mTempInternalExtras
            mITC18.WriteNewITC18dataAsEpisodeNew(NewEpi, TempParms, NewData, ActualFileName, mCurrentTelegraphs)

            mLastEpisodeProcessed = "ITC18: " + My.Computer.FileSystem.GetName(ActualFileName)
            pnlStatus.Text = "saved file: " + ActualFileName + " (" + Format(SW.ElapsedMilliseconds / 1000, "F1") + " sec)."
            SW.Stop()
            '    System.Threading.Thread.Sleep(100)
            Application.DoEvents()
            Broadcast.SendToChannel("AcquireToSynapse", ActualFileName)
            '   Me.Text = "ITC18 Controller - " + ActualFileName
            ' xx   mLastEpiProcessed = NewEpi
            mOverrideFilename = ""
            TelegraphTimer.Enabled = True
            TrialPendingTimer.Enabled = True
            '    Catch ex As Exception
            '   MsgBox("Something went wrong in DoTrial: " + ex.Message)
            '   End Try
            Return True
        Else
            MsgBox("Cannot located temp files on R:\ that should have been created by Acquire.")
            Return False
        End If ' files exist
    End Function
    Private Function StopTrial() As Boolean

        Return True
    End Function
    Private Function SealTest(ByVal AmpNum As Integer) As Boolean
        Dim SealTestDACChan As Integer
        Dim SealTestADCChan As Integer
        Dim SealTestFSvolts As Integer
        If IsNothing(mLastEpiProcessed.FileName) Then
            Select Case AmpNum
                Case 0
                    SealTestADCChan = 0
                    SealTestDACChan = 0
                    SealTestFSvolts = 3
                Case 1
                    SealTestADCChan = 2
                    SealTestDACChan = 1
                    SealTestFSvolts = 3
                Case 2
                    SealTestADCChan = 4
                    SealTestDACChan = 2
                    SealTestFSvolts = 3
                Case 3
                    SealTestADCChan = 6
                    SealTestDACChan = 3
                    SealTestFSvolts = 3
                Case Else
                    MsgBox("Unexpected AmpNum enountered in SealTest: " + AmpNum.ToString)
                    Stop
            End Select
        Else
            SealTestDACChan = AmpNum
            SealTestADCChan = mLastEpiProcessed.GenArray(35 + AmpNum)
            SealTestFSvolts = mLastEpiProcessed.GenArray(11 + SealTestADCChan)
        End If
        Dim ReturnedData As Single()
        Try
            ReturnedData = mITC18.RunSealTest(SealTestADCChan, SealTestDACChan,
                                      SealTestFSvolts, mSealTestBaselineVm, 1 / mCurrentTelegraphs(AmpNum))
        Catch ex As Exception
            MsgBox("Something went wrong in SealTest (ITC): " + ex.Message)
            ReturnedData = Nothing
        End Try
        Try
            frmSealTest.UpdateSealTest(ReturnedData, AmpNum)
        Catch ex As Exception
            MsgBox("Something went wrong in SealTest (Update): " + ex.Message)
        End Try

        Return True
    End Function
   

    ' GUI Code below here


    Private Sub cmdSealTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSealTest.Click
        mSealTestBaselineVm = 0
        frmSealTest.Show()
    End Sub

    Private Sub SealTestTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SealTestTimer.Tick
        SealTest(cboAmps.SelectedIndex)
    End Sub

    Private Sub TelegraphTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TelegraphTimer.Tick
        Dim TempTelegraphs As Single() = UpdateTelegraphs()
        DisplayTelegraphs(TempTelegraphs)
    End Sub

  
    Private Sub chkUseAuxTTLAWF_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkUseAuxTTLAWF.CheckedChanged
        mUseAuxTTL = chkUseAuxTTLAWF.Checked
        Application.DoEvents()
    End Sub

    Private Sub TrialPendingTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrialPendingTimer.Tick
        If mNextTrialToDo.Length > 0 Then
            TrialPendingTimer.Enabled = False
            Application.DoEvents()
            DoTrial()
            mNextTrialToDo = ""
            TrialPendingTimer.Enabled = True
        End If
    End Sub
    Friend Function DisplayTelegraphs(ByVal NewTelegraphs As Single()) As Boolean
        If NewTelegraphs.Length <> 2 Then
            Stop
        Else
            pnlTelegraphA.Text = NewTelegraphs(0).ToString
            pnlTelegraphB.Text = NewTelegraphs(1).ToString
        End If
        Return True
    End Function

 
    Private Sub cmdRequestStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRequestStop.Click
        mStopRequested = True
    End Sub

    Private Sub chkStartRaster_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkStartRaster.CheckedChanged
        If chkStartRaster.Checked Then
            GroupBox1.BackColor = Color.AntiqueWhite
        Else
            GroupBox1.BackColor = SystemColors.Control
        End If
    End Sub
End Class
