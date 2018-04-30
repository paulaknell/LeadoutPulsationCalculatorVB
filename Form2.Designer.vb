<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
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
        Me.PictureBox_Graph = New System.Windows.Forms.PictureBox()
        CType(Me.PictureBox_Graph, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox_Graph
        '
        Me.PictureBox_Graph.Location = New System.Drawing.Point(2, 4)
        Me.PictureBox_Graph.Name = "PictureBox_Graph"
        Me.PictureBox_Graph.Size = New System.Drawing.Size(364, 734)
        Me.PictureBox_Graph.TabIndex = 0
        Me.PictureBox_Graph.TabStop = False
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(373, 741)
        Me.Controls.Add(Me.PictureBox_Graph)
        Me.Name = "Form2"
        Me.Text = "Graph Display"
        CType(Me.PictureBox_Graph, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox_Graph As System.Windows.Forms.PictureBox
End Class
