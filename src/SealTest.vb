Imports ZedGraph
Public Class frmSealTest
    Dim LastYMax As Double = 30
    Dim LastYMin As Double = -30
    Dim SW As System.Diagnostics.Stopwatch
    Private mResistanceBuffer As Single()
    Private mBufferIndex As Integer
    Friend Sub UpdateSealTest(ByVal ReturnedData As Single(), ByVal AmpNum As Integer)
        Me.Text = "Seal Test on Amplifier " + Chr(65 + AmpNum)
        Dim MyPane As GraphPane = ZedGraphControl1.GraphPane

        Application.DoEvents()
        Dim RetMax As Single = ReturnedData.Max
        Dim RetMin As Single = ReturnedData.Min
        Dim Outlier As Boolean
        If RetMax < LastYMax And RetMin > LastYMin Then
            Outlier = False
        Else
            Outlier = True
        End If
        Dim RetDiff As Single = RetMax - RetMin
        Dim AutoScaleMode As Boolean = cmdScale.Text = "Freeze Scale"
        If Not AutoScaleMode And Outlier Then
            AutoScaleMode = True
            cmdScale.Text = "Freeze Scale"
            Application.DoEvents()
        End If

        If Outlier Then
            If SW.IsRunning Then
                SW.Stop()
                SW.Reset()
            End If
        Else
            If AutoScaleMode Then
                If SW.IsRunning Then
                    If SW.ElapsedMilliseconds > 3000 Then
                        AutoScaleMode = False
                        cmdScale.Text = "Auto Scale"
                        SW.Stop()
                        SW.Reset()
                    End If
                Else
                    SW.Start()
                End If
            End If
        End If ' outlier


        Dim BaselineCur As Double = 0
        Dim iCount As Single = 0
        For i As Integer = 0 To 99 ' over first 2 ms
            BaselineCur += CDbl(ReturnedData(i))
            iCount += 1
        Next
        BaselineCur = BaselineCur / iCount

        Dim StepCur As Double = 0
        iCount = 0
        For i As Integer = 225 To 300
            StepCur += CDbl(ReturnedData(i))
            iCount += 1
        Next
        StepCur = StepCur / iCount

        Dim SealResistance As Single = 1000.0! * (20.0! / Math.Abs(StepCur - BaselineCur))
        mResistanceBuffer(mBufferIndex) = SealResistance
        mBufferIndex += 1
        If mBufferIndex = mResistanceBuffer.Length Then mBufferIndex = 0
        Dim AverageResistance As Single = 0
        Dim AverageResistanceCount As Single = 0
        For i As Integer = 0 To 9
            If Not Single.IsNaN(mResistanceBuffer(i)) Then
                AverageResistance += mResistanceBuffer(i)
                AverageResistanceCount += 1
            End If
        Next
        AverageResistance = AverageResistance / AverageResistanceCount
        If AverageResistance >= 5100 Then
            pnlResistance.Text = " > 5 GOhms"
        Else
            pnlResistance.Text = Format(AverageResistance, "F1") + " MOhms"
        End If

        With MyPane
            .Title.IsVisible = False
            .Legend.IsVisible = False
            .XAxis.Title.Text = "Time (ms)"
            .YAxis.Title.IsVisible = False
            .YAxis.MajorGrid.IsZeroLine = False
            .YAxis.Title.Text = "Cur (pA)"
            .YAxis.Scale.FormatAuto = False
            If AutoScaleMode Then
                LastYMax = RetMax + (0.2 * RetDiff)
                LastYMin = RetMin - (0.2 * RetDiff)
                .YAxis.Scale.Max = LastYMax
                .YAxis.Scale.Min = LastYMin
            Else
                .YAxis.Scale.Max = LastYMax
                .YAxis.Scale.Min = LastYMin
            End If
            .XAxis.Scale.FormatAuto = False
            .XAxis.Scale.Min = 0
            .XAxis.Scale.Max = 10
            .CurveList.Clear()
            Dim Factor As Double = 0.02 ' ms per point
            Dim List As New PointPairList
            For i As Long = 0 To ReturnedData.Length - 1
                List.Add(CDbl(i * Factor), ReturnedData(i))
            Next
            Dim myCurve As LineItem
            myCurve = .AddCurve("One Curve", List, Color.Blue, SymbolType.None)

            ZedGraphControl1.AxisChange()
            ZedGraphControl1.Invalidate()
            cmdStop.Focus()
            Application.DoEvents()

        End With

    End Sub

    Private Sub frmSealTest_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Timer1.Enabled = False
        Form1.SetSealTestTimer(False)
        Form1.RestoreStaticDAClevels()
    End Sub

    Private Sub frmSealTest_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        SW = New System.Diagnostics.Stopwatch
        Timer1.Enabled = True
    End Sub
  
    Private Sub cmdStop_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cmdStop.MouseDown
        Form1.SetSealTestTimer(False)
    End Sub

    Private Sub cmdRun_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cmdRun.MouseDown
        Form1.SetSealTestTimer(True)
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
        Form1.SetSealTestTimer(True)
    End Sub

   

    Private Sub cmdScale_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cmdScale.MouseDown
        If cmdScale.Text = "Freeze Scale" Then
            cmdScale.Text = "Auto Scale"
        Else
            cmdScale.Text = "Freeze Scale"
        End If
    End Sub
    
    Private Sub cmdNeutral_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cmdNeutral.MouseDown
        Form1.mSealTestBaselineVm = 0
        pnlMembranePot.Text = "at 0 mV"
    End Sub

    Private Sub cmdHyperpolarize_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cmdHyperpolarize.MouseDown
        Form1.mSealTestBaselineVm = -70
        pnlMembranePot.Text = "at -70 mV"
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ReDim mResistanceBuffer(9)
        For i As Integer = 0 To mResistanceBuffer.Length - 1
            mResistanceBuffer(i) = Single.NaN
        Next
        mBufferIndex = 0
    End Sub

   
  
End Class