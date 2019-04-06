<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSealTest
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
        Me.ZedGraphControl1 = New ZedGraph.ZedGraphControl()
        Me.cmdStop = New System.Windows.Forms.Button()
        Me.cmdRun = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.cmdScale = New System.Windows.Forms.Button()
        Me.pnlResistance = New System.Windows.Forms.Label()
        Me.cmdHyperpolarize = New System.Windows.Forms.Button()
        Me.cmdNeutral = New System.Windows.Forms.Button()
        Me.pnlMembranePot = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'ZedGraphControl1
        '
        Me.ZedGraphControl1.Enabled = False
        Me.ZedGraphControl1.Location = New System.Drawing.Point(12, 12)
        Me.ZedGraphControl1.Name = "ZedGraphControl1"
        Me.ZedGraphControl1.ScrollGrace = 0.0R
        Me.ZedGraphControl1.ScrollMaxX = 0.0R
        Me.ZedGraphControl1.ScrollMaxY = 0.0R
        Me.ZedGraphControl1.ScrollMaxY2 = 0.0R
        Me.ZedGraphControl1.ScrollMinX = 0.0R
        Me.ZedGraphControl1.ScrollMinY = 0.0R
        Me.ZedGraphControl1.ScrollMinY2 = 0.0R
        Me.ZedGraphControl1.Size = New System.Drawing.Size(644, 414)
        Me.ZedGraphControl1.TabIndex = 1
        '
        'cmdStop
        '
        Me.cmdStop.Location = New System.Drawing.Point(674, 27)
        Me.cmdStop.Name = "cmdStop"
        Me.cmdStop.Size = New System.Drawing.Size(56, 22)
        Me.cmdStop.TabIndex = 2
        Me.cmdStop.Text = "Stop"
        Me.cmdStop.UseVisualStyleBackColor = True
        '
        'cmdRun
        '
        Me.cmdRun.Location = New System.Drawing.Point(745, 27)
        Me.cmdRun.Name = "cmdRun"
        Me.cmdRun.Size = New System.Drawing.Size(58, 22)
        Me.cmdRun.TabIndex = 3
        Me.cmdRun.Text = "Run"
        Me.cmdRun.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Interval = 50
        '
        'cmdScale
        '
        Me.cmdScale.Location = New System.Drawing.Point(721, 70)
        Me.cmdScale.Name = "cmdScale"
        Me.cmdScale.Size = New System.Drawing.Size(82, 27)
        Me.cmdScale.TabIndex = 4
        Me.cmdScale.Text = "Freeze Scale"
        Me.cmdScale.UseVisualStyleBackColor = True
        '
        'pnlResistance
        '
        Me.pnlResistance.AutoSize = True
        Me.pnlResistance.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlResistance.ForeColor = System.Drawing.Color.DarkRed
        Me.pnlResistance.Location = New System.Drawing.Point(681, 113)
        Me.pnlResistance.Name = "pnlResistance"
        Me.pnlResistance.Size = New System.Drawing.Size(66, 24)
        Me.pnlResistance.TabIndex = 5
        Me.pnlResistance.Text = "Label1"
        '
        'cmdHyperpolarize
        '
        Me.cmdHyperpolarize.Location = New System.Drawing.Point(737, 250)
        Me.cmdHyperpolarize.Name = "cmdHyperpolarize"
        Me.cmdHyperpolarize.Size = New System.Drawing.Size(66, 27)
        Me.cmdHyperpolarize.TabIndex = 6
        Me.cmdHyperpolarize.Text = "Hyperpolarize"
        Me.cmdHyperpolarize.UseVisualStyleBackColor = True
        '
        'cmdNeutral
        '
        Me.cmdNeutral.Location = New System.Drawing.Point(737, 193)
        Me.cmdNeutral.Name = "cmdNeutral"
        Me.cmdNeutral.Size = New System.Drawing.Size(66, 27)
        Me.cmdNeutral.TabIndex = 7
        Me.cmdNeutral.Text = "Neutral"
        Me.cmdNeutral.UseVisualStyleBackColor = True
        '
        'pnlMembranePot
        '
        Me.pnlMembranePot.AutoSize = True
        Me.pnlMembranePot.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlMembranePot.ForeColor = System.Drawing.Color.DarkRed
        Me.pnlMembranePot.Location = New System.Drawing.Point(706, 147)
        Me.pnlMembranePot.Name = "pnlMembranePot"
        Me.pnlMembranePot.Size = New System.Drawing.Size(73, 24)
        Me.pnlMembranePot.TabIndex = 8
        Me.pnlMembranePot.Text = "at 0 mV"
        '
        'frmSealTest
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(815, 438)
        Me.Controls.Add(Me.pnlMembranePot)
        Me.Controls.Add(Me.cmdNeutral)
        Me.Controls.Add(Me.cmdHyperpolarize)
        Me.Controls.Add(Me.pnlResistance)
        Me.Controls.Add(Me.cmdScale)
        Me.Controls.Add(Me.cmdRun)
        Me.Controls.Add(Me.cmdStop)
        Me.Controls.Add(Me.ZedGraphControl1)
        Me.Name = "frmSealTest"
        Me.Text = "Seal Test"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ZedGraphControl1 As ZedGraph.ZedGraphControl
    Friend WithEvents cmdStop As System.Windows.Forms.Button
    Friend WithEvents cmdRun As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents cmdScale As System.Windows.Forms.Button
    Friend WithEvents pnlResistance As System.Windows.Forms.Label
    Friend WithEvents cmdHyperpolarize As System.Windows.Forms.Button
    Friend WithEvents cmdNeutral As System.Windows.Forms.Button
    Friend WithEvents pnlMembranePot As System.Windows.Forms.Label
End Class
