<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.ckbExtraInternalStuff = New System.Windows.Forms.CheckedListBox()
        Me.cboInternal = New System.Windows.Forms.ComboBox()
        Me.cmdSealTest = New System.Windows.Forms.Button()
        Me.SealTestTimer = New System.Windows.Forms.Timer(Me.components)
        Me.pnlTelegraphs = New System.Windows.Forms.Label()
        Me.TelegraphTimer = New System.Windows.Forms.Timer(Me.components)
        Me.cboAmps = New System.Windows.Forms.ComboBox()
        Me.txtBrainRegion = New System.Windows.Forms.TextBox()
        Me.txtExptGoal = New System.Windows.Forms.TextBox()
        Me.chkUseAuxTTLAWF = New System.Windows.Forms.CheckBox()
        Me.TrialPendingTimer = New System.Windows.Forms.Timer(Me.components)
        Me.pnlStatus = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtAmpATelegraphChan = New System.Windows.Forms.TextBox()
        Me.txtAmpBTelegraphChan = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.pnlTelegraphA = New System.Windows.Forms.Label()
        Me.pnlTelegraphB = New System.Windows.Forms.Label()
        Me.pnlHeartbeat = New System.Windows.Forms.Label()
        Me.cmdRequestStop = New System.Windows.Forms.Button()
        Me.chkStartRaster = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.chkRasterExtTrig = New System.Windows.Forms.CheckBox()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'ckbExtraInternalStuff
        '
        Me.ckbExtraInternalStuff.FormattingEnabled = True
        Me.ckbExtraInternalStuff.Location = New System.Drawing.Point(12, 39)
        Me.ckbExtraInternalStuff.Name = "ckbExtraInternalStuff"
        Me.ckbExtraInternalStuff.Size = New System.Drawing.Size(156, 94)
        Me.ckbExtraInternalStuff.TabIndex = 0
        '
        'cboInternal
        '
        Me.cboInternal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboInternal.FormattingEnabled = True
        Me.cboInternal.Location = New System.Drawing.Point(12, 12)
        Me.cboInternal.Name = "cboInternal"
        Me.cboInternal.Size = New System.Drawing.Size(302, 21)
        Me.cboInternal.TabIndex = 1
        '
        'cmdSealTest
        '
        Me.cmdSealTest.Location = New System.Drawing.Point(186, 41)
        Me.cmdSealTest.Name = "cmdSealTest"
        Me.cmdSealTest.Size = New System.Drawing.Size(59, 25)
        Me.cmdSealTest.TabIndex = 2
        Me.cmdSealTest.Text = "Seal"
        Me.cmdSealTest.UseVisualStyleBackColor = True
        '
        'SealTestTimer
        '
        '
        'pnlTelegraphs
        '
        Me.pnlTelegraphs.AutoSize = True
        Me.pnlTelegraphs.Location = New System.Drawing.Point(185, 79)
        Me.pnlTelegraphs.Name = "pnlTelegraphs"
        Me.pnlTelegraphs.Size = New System.Drawing.Size(60, 13)
        Me.pnlTelegraphs.TabIndex = 3
        Me.pnlTelegraphs.Text = "Telegraphs"
        '
        'TelegraphTimer
        '
        Me.TelegraphTimer.Interval = 1000
        '
        'cboAmps
        '
        Me.cboAmps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboAmps.FormattingEnabled = True
        Me.cboAmps.Location = New System.Drawing.Point(251, 43)
        Me.cboAmps.Name = "cboAmps"
        Me.cboAmps.Size = New System.Drawing.Size(63, 21)
        Me.cboAmps.TabIndex = 5
        '
        'txtBrainRegion
        '
        Me.txtBrainRegion.Location = New System.Drawing.Point(186, 104)
        Me.txtBrainRegion.Name = "txtBrainRegion"
        Me.txtBrainRegion.Size = New System.Drawing.Size(128, 20)
        Me.txtBrainRegion.TabIndex = 6
        Me.txtBrainRegion.Text = "Brain Area?"
        '
        'txtExptGoal
        '
        Me.txtExptGoal.Location = New System.Drawing.Point(12, 153)
        Me.txtExptGoal.Name = "txtExptGoal"
        Me.txtExptGoal.Size = New System.Drawing.Size(302, 20)
        Me.txtExptGoal.TabIndex = 7
        Me.txtExptGoal.Text = "Experiment Goal?"
        '
        'chkUseAuxTTLAWF
        '
        Me.chkUseAuxTTLAWF.AutoSize = True
        Me.chkUseAuxTTLAWF.Location = New System.Drawing.Point(186, 130)
        Me.chkUseAuxTTLAWF.Name = "chkUseAuxTTLAWF"
        Me.chkUseAuxTTLAWF.Size = New System.Drawing.Size(113, 17)
        Me.chkUseAuxTTLAWF.TabIndex = 8
        Me.chkUseAuxTTLAWF.Text = "Use AuxTTL AWF"
        Me.chkUseAuxTTLAWF.UseVisualStyleBackColor = True
        '
        'TrialPendingTimer
        '
        Me.TrialPendingTimer.Interval = 50
        '
        'pnlStatus
        '
        Me.pnlStatus.AutoSize = True
        Me.pnlStatus.Location = New System.Drawing.Point(12, 185)
        Me.pnlStatus.Name = "pnlStatus"
        Me.pnlStatus.Size = New System.Drawing.Size(55, 13)
        Me.pnlStatus.TabIndex = 9
        Me.pnlStatus.Text = "status info"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(330, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(117, 13)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "AmpA Telegraph Chan:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(330, 65)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(117, 13)
        Me.Label2.TabIndex = 11
        Me.Label2.Text = "AmpB Telegraph Chan:"
        '
        'txtAmpATelegraphChan
        '
        Me.txtAmpATelegraphChan.Location = New System.Drawing.Point(451, 35)
        Me.txtAmpATelegraphChan.Name = "txtAmpATelegraphChan"
        Me.txtAmpATelegraphChan.Size = New System.Drawing.Size(26, 20)
        Me.txtAmpATelegraphChan.TabIndex = 12
        Me.txtAmpATelegraphChan.Text = "-1"
        Me.txtAmpATelegraphChan.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtAmpBTelegraphChan
        '
        Me.txtAmpBTelegraphChan.Location = New System.Drawing.Point(451, 61)
        Me.txtAmpBTelegraphChan.Name = "txtAmpBTelegraphChan"
        Me.txtAmpBTelegraphChan.Size = New System.Drawing.Size(26, 20)
        Me.txtAmpBTelegraphChan.TabIndex = 13
        Me.txtAmpBTelegraphChan.Text = "-1"
        Me.txtAmpBTelegraphChan.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(342, 88)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(89, 13)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "(use -1 for ignore)"
        '
        'pnlTelegraphA
        '
        Me.pnlTelegraphA.AutoSize = True
        Me.pnlTelegraphA.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlTelegraphA.ForeColor = System.Drawing.Color.Brown
        Me.pnlTelegraphA.Location = New System.Drawing.Point(483, 37)
        Me.pnlTelegraphA.Name = "pnlTelegraphA"
        Me.pnlTelegraphA.Size = New System.Drawing.Size(16, 16)
        Me.pnlTelegraphA.TabIndex = 16
        Me.pnlTelegraphA.Text = "1"
        '
        'pnlTelegraphB
        '
        Me.pnlTelegraphB.AutoSize = True
        Me.pnlTelegraphB.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlTelegraphB.ForeColor = System.Drawing.Color.Brown
        Me.pnlTelegraphB.Location = New System.Drawing.Point(483, 61)
        Me.pnlTelegraphB.Name = "pnlTelegraphB"
        Me.pnlTelegraphB.Size = New System.Drawing.Size(16, 16)
        Me.pnlTelegraphB.TabIndex = 17
        Me.pnlTelegraphB.Text = "1"
        '
        'pnlHeartbeat
        '
        Me.pnlHeartbeat.AutoSize = True
        Me.pnlHeartbeat.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlHeartbeat.Location = New System.Drawing.Point(460, 90)
        Me.pnlHeartbeat.Name = "pnlHeartbeat"
        Me.pnlHeartbeat.Size = New System.Drawing.Size(10, 13)
        Me.pnlHeartbeat.TabIndex = 18
        Me.pnlHeartbeat.Text = "|"
        '
        'cmdRequestStop
        '
        Me.cmdRequestStop.Location = New System.Drawing.Point(374, 122)
        Me.cmdRequestStop.Name = "cmdRequestStop"
        Me.cmdRequestStop.Size = New System.Drawing.Size(56, 24)
        Me.cmdRequestStop.TabIndex = 19
        Me.cmdRequestStop.Text = "Stop"
        Me.cmdRequestStop.UseVisualStyleBackColor = True
        '
        'chkStartRaster
        '
        Me.chkStartRaster.AutoSize = True
        Me.chkStartRaster.Location = New System.Drawing.Point(6, 3)
        Me.chkStartRaster.Name = "chkStartRaster"
        Me.chkStartRaster.Size = New System.Drawing.Size(93, 17)
        Me.chkStartRaster.TabIndex = 20
        Me.chkStartRaster.Text = "Raster Control"
        Me.chkStartRaster.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.chkRasterExtTrig)
        Me.GroupBox1.Controls.Add(Me.chkStartRaster)
        Me.GroupBox1.Location = New System.Drawing.Point(345, 153)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(109, 45)
        Me.GroupBox1.TabIndex = 21
        Me.GroupBox1.TabStop = False
        '
        'chkRasterExtTrig
        '
        Me.chkRasterExtTrig.AutoSize = True
        Me.chkRasterExtTrig.Location = New System.Drawing.Point(29, 22)
        Me.chkRasterExtTrig.Name = "chkRasterExtTrig"
        Me.chkRasterExtTrig.Size = New System.Drawing.Size(62, 17)
        Me.chkRasterExtTrig.TabIndex = 21
        Me.chkRasterExtTrig.Text = "Ext Trig"
        Me.chkRasterExtTrig.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(519, 207)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.cmdRequestStop)
        Me.Controls.Add(Me.pnlHeartbeat)
        Me.Controls.Add(Me.pnlTelegraphB)
        Me.Controls.Add(Me.pnlTelegraphA)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtAmpBTelegraphChan)
        Me.Controls.Add(Me.txtAmpATelegraphChan)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.pnlStatus)
        Me.Controls.Add(Me.chkUseAuxTTLAWF)
        Me.Controls.Add(Me.txtExptGoal)
        Me.Controls.Add(Me.txtBrainRegion)
        Me.Controls.Add(Me.cboAmps)
        Me.Controls.Add(Me.pnlTelegraphs)
        Me.Controls.Add(Me.cmdSealTest)
        Me.Controls.Add(Me.cboInternal)
        Me.Controls.Add(Me.ckbExtraInternalStuff)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ckbExtraInternalStuff As System.Windows.Forms.CheckedListBox
    Friend WithEvents cboInternal As System.Windows.Forms.ComboBox
    Friend WithEvents cmdSealTest As System.Windows.Forms.Button
    Friend WithEvents SealTestTimer As System.Windows.Forms.Timer
    Friend WithEvents pnlTelegraphs As System.Windows.Forms.Label
    Friend WithEvents TelegraphTimer As System.Windows.Forms.Timer
    Friend WithEvents cboAmps As System.Windows.Forms.ComboBox
    Friend WithEvents txtBrainRegion As System.Windows.Forms.TextBox
    Friend WithEvents txtExptGoal As System.Windows.Forms.TextBox
    Friend WithEvents chkUseAuxTTLAWF As System.Windows.Forms.CheckBox
    Friend WithEvents TrialPendingTimer As System.Windows.Forms.Timer
    Friend WithEvents pnlStatus As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtAmpATelegraphChan As System.Windows.Forms.TextBox
    Friend WithEvents txtAmpBTelegraphChan As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents pnlTelegraphA As System.Windows.Forms.Label
    Friend WithEvents pnlTelegraphB As System.Windows.Forms.Label
    Friend WithEvents pnlHeartbeat As System.Windows.Forms.Label
    Friend WithEvents cmdRequestStop As System.Windows.Forms.Button
    Friend WithEvents chkStartRaster As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents chkRasterExtTrig As System.Windows.Forms.CheckBox

End Class
