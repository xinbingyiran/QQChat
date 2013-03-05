Public Class Form1
    Public f2 As Form2 = New Form2
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        f2.p = Me
        f2.Show()
        Me.Hide()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
    End Sub
End Class
