Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO

Class MainWindow
    'Initialise variable storage classes



    Dim RotorRadius As Double
    Dim RollerRadius As Double
    Dim TrackRadius As Double
    Dim TubeWall As Double
    Dim TubeID As Double
    Dim NumRollers As Integer
    Dim TrackSweepAngle As Double

    Dim TrackLeadOutAngle As Double
    Dim TrackLeadOutProfile(360) As Double
    Dim TrackLeadOutDivisor As Double

    Dim RPM As Double


    Dim TrackLeadInAngle As Double
    Dim TrackLeadInProfile(360) As Double
    Dim segment_index As Integer

    Dim TrackProfile(720) As Double 'Needs to be over 360 degrees for single rotor system. > 360 + 2 x lead in / lead out. was 360

    Dim PI As Double

    Dim Pulsation_Rotor_Segment(360 * 4) As Double 'big number


    Dim Pulsation_Rotor(360) As Double


    Dim RollerIncrementAngle As Double 'hopefully in 1 degree increments for simplicity 

    Dim frmDialogue As New Form1

    Dim frmDialogue2 As New Form2

    Dim myGraphics As Graphics = frmDialogue.PictureBox1.CreateGraphics

    Dim myGraphics2 As Graphics = frmDialogue2.PictureBox_Graph.CreateGraphics

    Dim Brush As Brush

    Dim Colour As Color


    Dim dx As Double
    Dim dy As Double
    Dim A As Double
    Dim B As Double
    Dim C As Double
    Dim det As Double
    Dim t As Double

    Dim Intersection1Valid As Boolean
    Dim Intersection2Valid As Boolean

    Dim Intersection1X As Double
    Dim Intersection1Y As Double
    Dim Intersection2X As Double
    Dim Intersection2Y As Double





    Private Sub Button_UpdateDrawing_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button_UpdateDrawing.Click
        'Populate public data from textbox inputs into global variables.

        PI = 3.141592653589

        RotorRadius = Val(TextBox_RotorRadius.Text)
        RollerRadius = Val(TextBox_RollerRadius.Text)
        TrackRadius = Val(TextBox_TrackRadius.Text)

        TubeWall = Val(TextBox_TubeWall.Text)

        TubeID = Val(TextBox_TubeID.Text)

        NumRollers = Val(TextBox_NumRollers.Text)

        TrackSweepAngle = Val(TextBox_TrackSweepAngle.Text)

        TrackLeadOutAngle = Val(TextBox_TrackLeadOutAngle.Text)

        TrackLeadOutDivisor = Val(TextBox_Leadout_Divisor.Text)

        TrackLeadInAngle = Val(TextBox_TrackLeadInAngle.Text)

        RPM = Val(TextBox_RPM.Text)


        Dim MyString As System.String



        'now create a string array to store the split string
        Dim OutputArray() As System.String
        MyString = TextBox_TrackLeadOutProfile.Text
        OutputArray = Split(MyString, ",", -1, CompareMethod.Binary)


        'now create a string array to store the split string
        Dim InputArray() As System.String
        MyString = TextBox_TrackLeadInProfile.Text
        InputArray = Split(MyString, ",", -1, CompareMethod.Binary)

        Dim increment = ((TubeID) * 1 / TrackLeadOutAngle) / 4

        For index As Double = 0 To TrackLeadOutAngle Step 1.0 ' OutputArray.Length - 1
            'TrackLeadOutProfile(index) = Val(OutputArray(index)) / 1000 'turn into metres in mm in list

            'shape dependant on circumference squishing - want graduated area roughly exponential.

            If (checkBox.IsChecked = True) Then
                If (index < OutputArray.Length) Then
                    'TrackLeadOutProfile(index) = 0.0 + increment '* ((TubeID) * index) / (TrackLeadOutAngle * 4)
                    TrackLeadOutProfile(index) = Val(OutputArray(index)) / 1000 'turn into metres in mm in list
                Else

                    TrackLeadOutProfile(index) = TubeID
                End If

            Else
                Dim Distance As Double = TubeID - (TrackRadius - (RotorRadius + RollerRadius + (2 * TubeWall)))
                TrackLeadOutProfile(index) = 0.0 + ((Distance) * index) / TrackLeadOutAngle
            End If
            'increment = increment + ((TubeID * index)) / (TrackLeadOutAngle) ' why  ?? * TrackLeadOutDivisor

        Next


        'LeadIn
        MyString = TextBox_TrackLeadInProfile.Text

        'now create a string array to store the split string

        OutputArray = Split(MyString, ",", -1, CompareMethod.Binary)

        For index As Double = 0 To TrackLeadInAngle Step 1.0 '  OutputArray.Length - 1 Step 1
            If (checkBox.IsChecked = True) Then
                If (index < InputArray.Length) Then
                    'TrackLeadOutProfile(index) = 0.0 + increment '* ((TubeID) * index) / (TrackLeadOutAngle * 4)
                    TrackLeadInProfile(index) = Val(InputArray(index)) / 1000 'turn into metres in mm in list
                Else

                    TrackLeadInProfile(index) = 0   'TubeID
                End If

            Else


                'TrackLeadInProfile(index) = Val(OutputArray(index)) / 1000 'turn into metres in mm in list
                Dim Distance As Double = TubeID - (TrackRadius - (RotorRadius + RollerRadius + (2 * TubeWall)))
                TrackLeadInProfile(index) = (Distance - (Distance) * index / TrackLeadInAngle)

            End If

        Next






        'Fill variables with track profile information.
        'LeadOutAngle is numerically lower in the index. There are 361 items indexed from 0. 360 indexed from 1.


        For index As Integer = 0 To TrackSweepAngle Step 1
            TrackProfile(index) = TrackRadius

            If index <= 360 Then  '+ DegreeOffset.ToString + " : "
                TextBox_TrackProfile.Text = TextBox_TrackProfile.Text + TrackProfile(index).ToString + vbCrLf
            End If

        Next

        For index = TrackSweepAngle To (TrackSweepAngle + TrackLeadOutAngle) Step 1
            TrackProfile(index) = TrackRadius + TrackLeadOutProfile(index - TrackSweepAngle)
            If index <= 360 Then  '+ DegreeOffset.ToString + " : "
                TextBox_TrackProfile.Text = TextBox_TrackProfile.Text + TrackProfile(index).ToString + vbCrLf
            End If
        Next

        For index = (TrackSweepAngle + TrackLeadOutAngle) To 360 - TrackLeadInAngle Step 1
            '                     0.0606        0.0088     0.0606         0.3           0.25
            TrackProfile(index) = TrackRadius + TubeID - (TrackRadius - (RotorRadius + RollerRadius + (2 * TubeWall))) '     + (TubeWall * 2)  ' fully open TubeWall already included.
            If index <= 360 Then  '+ DegreeOffset.ToString + " : "
                TextBox_TrackProfile.Text = TextBox_TrackProfile.Text + TrackProfile(index).ToString + vbCrLf
            End If
        Next

        For index = 360 - TrackLeadInAngle To 360 Step 1
            TrackProfile(index) = TrackRadius + TrackLeadInProfile(index - (360 - TrackLeadInAngle))
            If index <= 360 Then  '+ DegreeOffset.ToString + " : "
                TextBox_TrackProfile.Text = TextBox_TrackProfile.Text + TrackProfile(index).ToString + vbCrLf
            End If
        Next


        'All variables assigned at this point 

    End Sub




    Private Sub Button_CalcFlow_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button_CalcFlow.Click
        'Assume all pipe filled 100% with fluid
        'Assume when roller ocludes shape of roller subtracted from tube area for relavant roller height

        Colour = Color.White

        Brush = Brushes.White

        Dim frm As Form1()

        RollerIncrementAngle = 0 'start in zero position.

        'only rollers in contact with track are important and only last two rollers are relavent (when not backstreaming) assume rear roller perfect action

        'Normal volume per increment = area of tube * swept volume.

        Dim Radians_one_degree = 1.0 * (2.0 * PI) / 360.0
        Dim sine_one_degree = Math.Sin(Radians_one_degree)

        'Radius of tube for volume calc should be (TrackRadius - Tubewall) as this is the distance
        'it runs off the rollers.


        'Dim Tube_Centre_Radius = (TrackRadius - ((TubeID + (TubeWall * 2.0)) / 2.0))
        Dim Tube_Centre_Radius = (TrackRadius - TubeWall - (TubeID / 2.0)) 'Squished tube diameter

        REM Dim Swept_Distance = Tube_Centre_Radius * sine_one_degree

        Dim Swept_Distance = Tube_Centre_Radius * 2.0 * PI / 360.0  'one degree disatnce over arc.

        Dim Tube_Area = (TubeID * TubeID * PI) / 4.0

        Dim Volume_per_increment = Tube_Area * Swept_Distance   'Positive roller action per degree

        Dim Volume_increment_cm3 = Volume_per_increment * 1000000 ' Dont use

        TextBox1.Text = TextBox1.Text + "Volume Increment / degree: =" + Volume_per_increment.ToString + vbCrLf

        Dim Internal_circumference = PI * TubeID  'in m

        Dim NewBdim As Double


        'Now sweep through track lead out from 100% occlusion to roller 100% off tube.

        'NewBdim = GetB(0.0001, Internal_circumference)

        'Looks like B is returning ok but not very efficient......


        'Split whole roller into segments 
        'Dim RollerAngle = Math.Asin(RollerRadius / RotorRadius) * 180.0 / (PI)

        Dim RollerAngleInteger As Integer
        'RollerAngleInteger = RollerAngle


        'Dim OneDegreeIncrementDistance = RollerRadius / RollerAngle

        Dim RollerProfile(720) As Double  'RollerAngleInteger * 2 safety factor over 360

        Dim count_area_positive(720) As Integer
        Dim count_area_negative(720) As Integer

        For indexval As Integer = 0 To 719
            count_area_positive(indexval) = 0
            count_area_negative(indexval) = 0
        Next


        'old routine calculating intersection simple approach
        ' Dim index As Integer
        ' index = 0
        ' For x As Double = -RollerRadius To RollerRadius Step OneDegreeIncrementDistance
        'Dim Y As Double
        ' Y = Math.Sqrt((RollerRadius * RollerRadius) - (x * x))
        'If (Y < 0) Then 'make abs
        'Y = -Y
        'End If
        'RollerProfile(index) = Y
        'Dim RollerRepeat = 360 / NumRollers
        'For index2 As Integer = 1 To NumRollers - 1
        'RollerProfile(index + (RollerRepeat * index2)) = Y
        'Next
        'index = index + 1
        'Next

        'New routine

        'setp in degree increments from -60 degees to + 60 degrees = 1/3 of rotation
        Dim index As Integer
        index = 0
        For angle As Integer = -60 To 60 Step 1
            Dim angle_rad As Double = (Math.PI * angle) / 180.0
            Dim p1x = (RotorRadius + RollerRadius) * Math.Sin(angle_rad)
            Dim p1y = (RotorRadius + RollerRadius) * Math.Cos(angle_rad)

            REM  Sub FindLineCircleIntersections(cx As Double, cy As Double, radius As Double,
            REM point1X As Double, point1Y As Double, point2X As Double, point2Y As Double)
            FindLineCircleIntersections(0, RotorRadius, RollerRadius,
            0, 0, p1x, p1y)


            Dim pI3X As Double
            Dim pI3Y As Double
            'If one or two points valid then use point with most positive Y coordinate
            If (Intersection1Valid = True And Intersection2Valid = True) Then
                If (Intersection1Y < Intersection2Y) Then
                    pI3Y = Intersection2Y
                    pI3X = Intersection2X
                Else
                    pI3Y = Intersection1Y
                    pI3X = Intersection1X
                End If
                RollerAngleInteger = angle 'angle
            Else
                If (Intersection1Valid = True) Then
                    pI3Y = Intersection1Y
                    pI3X = Intersection1X
                    RollerAngleInteger = angle 'angle
                ElseIf (Intersection2Valid = True) Then
                    pI3Y = Intersection2Y
                    pI3X = Intersection2X
                    RollerAngleInteger = angle 'angle
                Else
                    'No valid interceptions
                    Dim outofcirc As Boolean = False
                End If

            End If



            Dim Y As Double = 0

            If (Intersection1Valid = True Or Intersection2Valid = True) Then
                'get length of P1 to P3
                Y = GetPositiveLength(p1x, p1y, pI3X, pI3Y)
                RollerProfile(index) = Y
                index = index + 1
            Else
                'length > tube radius x 2 (fully open)
                Y = 2 * TubeID
                'Shouldn't need to set this as not valid
                'RollerProfile(index) = Y
            End If

            'Dim RollerRepeat = 360 / NumRollers

            'Only interested in one roller pass so don't need to repeat this
            'For index2 As Integer = 1 To NumRollers - 1
            'RollerProfile(index + (RollerRepeat * index2)) = Y
            'Next

            'index = index + 1

        Next

        RollerAngleInteger = RollerAngleInteger

        Dim A As Double
        Dim Area As Double
        Dim Summed_Area_Positive(720) As Double
        Dim Summed_Area_Negative(720) As Double

        Dim Angle_Section = (360 / NumRollers)
        'Dim Angle_Section_Start = TrackSweepAngle - (RollerAngleInteger * 2)  'removed TrackLeadOutAngle -

        Dim Angle_Section_Start = TrackSweepAngle - (RollerAngleInteger * 2)  'removed TrackLeadOutAngle -

        If (Angle_Section_Start < 1) Then
            Angle_Section_Start = 1
        End If

        Angle_Section = (360 / NumRollers)
        ' Angle_Section_Start = TrackSweepAngle + 180 - (RollerAngleInteger * 2)

        Dim myPen As Pen
        myPen = New Pen(System.Drawing.Color.Blue, 2)

        Dim r As Rectangle = frmDialogue.PictureBox1.ClientRectangle
        Dim r2 As Rectangle = frmDialogue2.PictureBox_Graph.ClientRectangle

        'r.Top
        'r.Bottom
        'r.Left
        'r.Right

        Dim midx As Integer
        Dim midy As Integer

        midx = (r.Left + r.Right) / 2
        midy = (r.Top + r.Bottom) / 2

        Dim oldx As Integer
        Dim oldy As Integer

        Dim x_pos As Integer
        Dim y_pos As Integer
        '       Dim x_pos2 As Integer
        '       Dim y_pos2 As Integer
        '       Dim x_pos3 As Integer
        '       Dim y_pos3 As Integer
        'Dim x_pos11 As Integer
        'Dim y_pos11 As Integer
        'Dim x_pos21 As Integer
        'Dim y_pos21 As Integer
        'Dim x_pos31 As Integer
        'Dim y_pos31 As Integer
        Dim Track_Pos As Integer
        '     Dim PRE_ROLLER_RAD As Double
        '    PRE_ROLLER_RAD = 0.008
        '     Dim PRE_ROLLER_ROT_RAD As Double
        '    PRE_ROLLER_ROT_RAD = 0.036
        oldx = midx
        oldy = midy

        segment_index = 1

        Dim first_time As Integer

        first_time = 1

        'ProgressBar1.Minimum = 0
        'ProgressBar1.Maximum = Angle_Section
        'ProgressBar1.Value = 0

        Dim Angle_Section_Start_Global = TrackSweepAngle - (RollerAngleInteger * 2)  'removed TrackLeadOutAngle -

        'to allow for enough data points in 120 degrees the whole set needs to be two points longer so
        'instead of -1 needs to be +/-2????

        'get array of roller heights for circle at degree increments
        For DegreeOffset As Integer = Angle_Section_Start_Global - 2 To (Angle_Section_Start_Global + Angle_Section) + 2 Step 1  'was Angle_Section_Start +  angle_Selecton * 2

            If DegreeOffset Mod 4 = 0 Then

                myGraphics.Clear(Colour)
                'Draw Track
                For DegreeTemp As Integer = 1 To 360 Step 1  '(Angle_Section_Start + Angle_Section)

                    Dim Track_Pos_Temp = (DegreeTemp) Mod 360
                    x_pos = midx + Math.Sin(DegreeTemp * 2 * PI / 360) * (TrackProfile(Track_Pos_Temp) * 3000)
                    y_pos = midy + Math.Cos(DegreeTemp * 2 * PI / 360) * (TrackProfile(Track_Pos_Temp) * 3000)
                    myGraphics.DrawLine(myPen, oldx, oldy, x_pos, y_pos)
                    oldx = x_pos
                    oldy = y_pos

                Next

                'Draw cross at mid point.
                myGraphics.DrawLine(myPen, midx, r.Top, midx, r.Top + r.Height)
                myGraphics.DrawLine(myPen, r.Left, midy, r.Left + r.Width, midy)

                'Dim Rollers As Integer

                For Rollers As Integer = 1 To NumRollers

                    Track_Pos = (RollerAngleInteger + DegreeOffset) Mod 360
                    x_pos = midx + Math.Sin((Track_Pos * 2 * PI + ((Rollers - 1) * (360 / NumRollers) * 2 * PI)) / 360) * (RotorRadius * 3000)
                    y_pos = midy + Math.Cos((Track_Pos * 2 * PI + ((Rollers - 1) * (360 / NumRollers) * 2 * PI)) / 360) * (RotorRadius * 3000)

                    Dim cRect2 As System.Drawing.RectangleF
                    cRect2.X = x_pos - RollerRadius * 3000
                    cRect2.Height = RollerRadius * 2 * 3000
                    cRect2.Y = y_pos - RollerRadius * 3000
                    cRect2.Width = RollerRadius * 2 * 3000
                    myGraphics.DrawEllipse(myPen, cRect2)

                Next

                frmDialogue.Invalidate(r)
                frmDialogue.Update()

            End If

            Dim Flow_Val As Double = 0
            Dim full_flow As Double = 0


            'was (RollerAngleInteger * 2) bellow

            'Is index zero based?????????? was 0

            For index = 0 To (RollerAngleInteger * 2) Step 1  'RollerAngleInteger * 2
                'calculate tube height against profile and then area and sum.

                Track_Pos = (index + DegreeOffset) Mod 360

                'Track_Pos = (DegreeOffset + RollerAngleInteger) Mod 360

                'If (DegreeOffset = Angle_Section_Start - 1) Then
                'calculate area for flow calc version 2.
                A = TrackRadius - (RollerRadius - RollerProfile(index)) - RotorRadius - (TubeWall * 2)
                If (A > (TubeID)) Then 'roller height way off tube
                    A = TubeID
                ElseIf (A < 0) Then
                    A = 0
                End If

                If (A = TubeID) Then
                    A = A / 2.0  ' want radius
                    NewBdim = A
                ElseIf (A = 0) Then
                    NewBdim = Internal_circumference / 4 'Value doesn't matter as always zero
                Else
                    A = A / 2.0  ' want radius
                    NewBdim = GetB_Binary(A, Internal_circumference) ' Volume_per_increment
                End If

                Area = PI * A * NewBdim


                Flow_Val = Flow_Val + (4.0 * NumRollers * (Area * Swept_Distance) * RPM * 1000.0)
                full_flow = Volume_per_increment * RPM * 1000.0 * 4.0 * (360 - ((RollerAngleInteger * 2.0) * NumRollers))

                'End If

                A = TrackProfile(Track_Pos) - (RollerRadius - RollerProfile(index)) - RotorRadius - (TubeWall * 2)

                If (A > (TubeID)) Then 'roller height way off tube
                    A = TubeID
                ElseIf (A < 0) Then
                    A = 0
                End If


                If (A = TubeID) Then
                    A = A / 2.0  ' want radius
                    NewBdim = A
                ElseIf (A = 0) Then
                    NewBdim = Internal_circumference / 4 'Value doesn't matter as always zero
                Else
                    A = A / 2.0  ' want radius
                    NewBdim = GetB_Binary(A, Internal_circumference) ' Volume_per_increment
                End If

                Area = PI * A * NewBdim

                'Shouldn't reset in here, as defined outside of this scope
                'Angle_Section = (360 / NumRollers) * 4
                Angle_Section_Start = TrackSweepAngle - (RollerAngleInteger * 2) - TrackLeadOutAngle - TrackLeadInAngle

                'Try the proper start point????
                Angle_Section_Start = TrackSweepAngle - (RollerAngleInteger * 2)

                If (DegreeOffset > Angle_Section_Start) Then
                    If (DegreeOffset <= Angle_Section_Start + Angle_Section) Then
                        Summed_Area_Positive(DegreeOffset) = Summed_Area_Positive(DegreeOffset) + (Area * Swept_Distance) 'must be volume i.e x distance
                        'Else
                        'Summed_Area_Positive(DegreeOffset) = Summed_Area_Positive(DegreeOffset) + (PI * TubeID * TubeID / 4.0)
                        count_area_positive(DegreeOffset) = count_area_positive(DegreeOffset) + 1
                    End If
                End If

                'Shouldn't reset in here, as defined outside of this scope
                'Angle_Section = (360 / NumRollers) * 4
                Angle_Section_Start = 360 - (RollerAngleInteger * 2)                                'TrackSweepAngle + 180 - (RollerAngleInteger * 2)


                'Try the proper start point????
                Angle_Section_Start = TrackSweepAngle - (RollerAngleInteger * 2)

                If (DegreeOffset > Angle_Section_Start) Then
                    If (DegreeOffset <= Angle_Section_Start + Angle_Section) Then
                        Summed_Area_Negative(DegreeOffset) = Summed_Area_Negative(DegreeOffset) + (Area * Swept_Distance) 'must be volume i.e x distance
                        count_area_negative(DegreeOffset) = count_area_negative(DegreeOffset) + 1
                        ' Else
                    End If
                    'Summed_Area_Negative(DegreeOffset) = Summed_Area_Negative(DegreeOffset) + (PI * TubeID * TubeID / 4.0)
                End If

            Next



            Dim Diff_POS As Double
            Dim Diff_NEG As Double

            Dim Average_Flow As Double

            Dim Volume_increment_m3 As Double

            'Still in scope of outer loop don;t reset here
            'Angle_Section = (360 / NumRollers) * 4
            Angle_Section_Start = TrackSweepAngle - (RollerAngleInteger * 2) - TrackLeadOutAngle - TrackLeadInAngle ' starting point??

            'Try the proper start point????
            Angle_Section_Start = TrackSweepAngle - (RollerAngleInteger * 2)


            If (DegreeOffset > Angle_Section_Start) Then
                If (DegreeOffset <= Angle_Section_Start + Angle_Section) Then
                    If (first_time = 1) Then
                        Diff_POS = 0
                        first_time = 0

                    Else

                        Diff_POS = Summed_Area_Positive(DegreeOffset) - Summed_Area_Positive(DegreeOffset - 1) 'was 0 then -1

                    End If
                    'Else
                    'must consider the number of entries in the summed area and normalise
                    'shouldn't need to sa based on the roller size so constant

                    Dim factor_pos As Integer = count_area_positive(DegreeOffset)

                    TextBox1.Text = TextBox1.Text + Summed_Area_Positive(DegreeOffset).ToString + vbCrLf
                        TextBox2.Text = TextBox2.Text + Diff_POS.ToString + vbCrLf
                        TextBox3.Text = TextBox3.Text + (Volume_per_increment - Diff_POS).ToString + vbCrLf

                        Volume_increment_m3 = ((Volume_per_increment - Diff_POS) * RPM * 1000.0 * 360.0)



                        TextBox7.Text = TextBox7.Text + Volume_increment_m3.ToString + vbCrLf

                        Pulsation_Rotor_Segment(segment_index) = Volume_increment_m3

                        Average_Flow = Average_Flow + Volume_increment_m3 / (360 / NumRollers)

                        'uses full calculation
                        TextBox_Average_Flow.Text = (Average_Flow * 4.0).ToString

                        'Flow_Val
                        'uses volume of tube minus rotor occlusion
                        'TextBox_Average_Flow.Text = (Flow_Val + full_flow).ToString

                        segment_index = segment_index + 1


                        'End If

                        Dim gX1 As Integer
                        Dim gY1 As Integer
                        Dim gX2 As Integer
                        Dim gY2 As Integer

                        gX1 = (DegreeOffset - Angle_Section_Start) * 2 + r2.X
                        gY1 = (Volume_per_increment - Diff_POS) * -1500000000 + r2.Y + r2.Height * 2 / 8
                        gX2 = (DegreeOffset - Angle_Section_Start) * 2 + r2.X
                        gY2 = r2.Y + (r2.Height * 2 / 8)

                        myGraphics2.DrawLine(myPen, gX1, gY1, gX2, gY2)

                        'frmDialogue2.Invalidate(r2)

                    End If
                End If

            'Still in scope of for next loop don't reset angle selection start.
            'Angle_Section = (360 / NumRollers) * 4
            Angle_Section_Start = 360 - (RollerAngleInteger * 2)                     'TrackSweepAngle + 180 - (RollerAngleInteger * 2)

            'Try the proper start point????
            Angle_Section_Start = TrackSweepAngle - (RollerAngleInteger * 2)

            If (DegreeOffset > Angle_Section_Start) Then
                If (DegreeOffset <= Angle_Section_Start + Angle_Section) Then
                    Diff_NEG = Summed_Area_Negative(DegreeOffset) - Summed_Area_Negative(DegreeOffset - 1)

                    Dim factor_neg As Integer = count_area_negative(DegreeOffset)
                    TextBox4.Text = TextBox4.Text + Summed_Area_Negative(DegreeOffset).ToString + vbCrLf
                    TextBox5.Text = TextBox5.Text + Diff_NEG.ToString + vbCrLf
                    TextBox6.Text = TextBox6.Text + (-Volume_per_increment - Diff_NEG).ToString + vbCrLf



                    Dim gX1 As Integer
                    Dim gY1 As Integer
                    Dim gX2 As Integer
                    Dim gY2 As Integer

                    gX1 = (DegreeOffset - Angle_Section_Start) * 2 + r2.X
                    gY1 = (-Volume_per_increment - Diff_NEG) * -1500000000 + r2.Y + r2.Height * 6 / 8
                    gX2 = (DegreeOffset - Angle_Section_Start) * 2 + r2.X
                    gY2 = r2.Y + r2.Height * 6 / 8

                    'myGraphics2.DrawLine(myPen, gX1, gY1, gX2, gY2)

                    'frmDialogue2.Invalidate(r2)
                End If
            End If

            frmDialogue2.Update()

            System.Threading.Thread.Sleep(1)

        Next


        For index2 As Integer = 1 To (360 / NumRollers) Step 1 ' 1 DEGREE

            For roller_index As Integer = 0 To (NumRollers * 2) - 1 ' extend dataset
                Pulsation_Rotor_Segment(index2 + (roller_index * (360 / NumRollers))) = Pulsation_Rotor_Segment(index2)
            Next

        Next

        For index2 = 1 To (360) Step 1 ' 1 DEGREE

            'TextBox8.Text = TextBox8.Text + Pulsation_Rotor_Segment(index2).ToString + vbCrLf

        Next


        'Create offset datasets

        For index2 = 1 To (360) Step 1 ' 1 DEGREE
            Pulsation_Rotor(index2) = 0
            For roller_index As Integer = 0 To 4 - 1 ' 4 = number of tubes

                Pulsation_Rotor(index2) = Pulsation_Rotor(index2) + Pulsation_Rotor_Segment(index2 + (roller_index * (360 / NumRollers) / 4)) '4 = number of tubes
            Next

            TextBox8.Text = TextBox8.Text + Pulsation_Rotor(index2).ToString + vbCrLf

        Next

        'frmDialogue.


    End Sub

    Private Function GetB(ByVal a As Double, ByVal circ As Double) As Double
        Dim b As Double
        Dim p As Double
        Dim differror As Double
        Dim exitflag As Boolean

        b = 0
        exitflag = False

        Do

            p = PI * ((3.0 * (a + b)) - Math.Sqrt(((3.0 * a) + b) * (a + (3.0 * b))))

            differror = (p - circ)

            If differror < 0 Then
                differror = -differror
            Else
                'flipped b now correct
                exitflag = True
            End If

            b = b + 0.00002   ' was 0.00005

        Loop Until exitflag = True

        Return b


    End Function

    Private Function GetB_Binary(ByVal a As Double, ByVal circ As Double) As Double
        Dim b As Double
        Dim p As Double
        Dim differror As Double
        Dim exitflag As Boolean

        Dim max_b = circ / 4.0
        Dim loop_index As Integer = 0

        Dim upper_bound As Double = max_b
        Dim lower_bound As Double = 0

        b = max_b / 2.0 ' start with mid point

        'b = 0
        exitflag = False

        Do

            p = PI * ((3.0 * (a + b)) - Math.Sqrt(((3.0 * a) + b) * (a + (3.0 * b))))

            differror = (p - circ)

            If differror > 0 Then
                'assume next iterant needs to be lower
                upper_bound = b
                b = (upper_bound - lower_bound) / 2.0 + lower_bound
            Else
                'assume needs to be higher
                lower_bound = b
                b = (upper_bound - lower_bound) / 2.0 + lower_bound
                differror = -differror
            End If



            If (differror < 0.0000001) Then
                exitflag = True
            End If

            loop_index = loop_index + 1

        Loop Until exitflag = True Or loop_index > 50

        Return b


    End Function


    Private Sub Button_Clear_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button_Clear.Click

        TextBox1.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""

        TextBox4.Text = ""
        TextBox5.Text = ""
        TextBox6.Text = ""
        TextBox7.Text = ""
        TextBox8.Text = ""

        TextBox_TrackProfile.Text = ""

        UpdateLayout()

        myGraphics.Clear(Colour)

        myGraphics2.Clear(Colour)



    End Sub





    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'Dim frmDialogue As New Form1
        frmDialogue.Show()

        frmDialogue2.Show()

        Read_File()

    End Sub

    Private Sub Button_Save_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button_Save.Click


        Dim dir = Directory.GetCurrentDirectory()

        Dim strFile = dir + "\" + "test.txt"

        Dim sw As StreamWriter
        Dim fs As FileStream = Nothing
        'If (Not File.Exists(strFile)) Then
        Try
            fs = File.Create(strFile)
            fs.Close()

            sw = File.AppendText(strFile)

            sw.WriteLine(TextBox_RotorRadius.Text)
            sw.WriteLine(TextBox_TrackRadius.Text)
            sw.WriteLine(TextBox_TubeWall.Text)
            sw.WriteLine(TextBox_TubeID.Text)
            sw.WriteLine(TextBox_NumRollers.Text)
            sw.WriteLine(TextBox_TrackSweepAngle.Text)
            sw.WriteLine(TextBox_TrackLeadOutAngle.Text)
            sw.WriteLine(TextBox_RollerRadius.Text)
            sw.WriteLine(TextBox_TrackLeadInAngle.Text)
            sw.WriteLine(TextBox_TrackLeadOutProfile.Text)
            sw.WriteLine(TextBox_TrackLeadInProfile.Text)

            sw.WriteLine(TextBox_Leadout_Divisor.Text)
            sw.WriteLine(TextBox_RPM.Text)

            If (checkBox.IsChecked = True) Then
                sw.WriteLine("1")
            Else
                sw.WriteLine("0")
            End If
            sw.Close()

        Catch ex As Exception
            MsgBox("Error Creating / writing Log File")
        End Try




    End Sub


    Sub Read_File()

        Dim dir = Directory.GetCurrentDirectory()

        Dim strFile = dir + "\" + "test.txt"

        Dim check_status As String


        'Dim fs As FileStream = Nothing
        If (File.Exists(strFile)) Then
            Dim sw As New System.IO.StreamReader(strFile)

            Try
                'fs = File.OpenRead(strFile)
                'fs.Close()

                'sw = File.ReadLines(strFile)

                TextBox_RotorRadius.Text = sw.ReadLine()
                TextBox_TrackRadius.Text = sw.ReadLine()
                TextBox_TubeWall.Text = sw.ReadLine()
                TextBox_TubeID.Text = sw.ReadLine()
                TextBox_NumRollers.Text = sw.ReadLine()
                TextBox_TrackSweepAngle.Text = sw.ReadLine()
                TextBox_TrackLeadOutAngle.Text = sw.ReadLine()
                TextBox_RollerRadius.Text = sw.ReadLine()
                TextBox_TrackLeadInAngle.Text = sw.ReadLine()

                TextBox_TrackLeadOutProfile.Text = sw.ReadLine()
                TextBox_TrackLeadInProfile.Text = sw.ReadLine()

                TextBox_Leadout_Divisor.Text = sw.ReadLine()

                TextBox_RPM.Text = sw.ReadLine()

                check_status = sw.ReadLine()

                If (Val(check_status) = 1) Then
                    checkBox.IsChecked = True
                Else
                    checkBox.IsChecked = False
                End If



                sw.Close()

            Catch ex As Exception
                MsgBox("Error Reading Log File")
            End Try
        End If
    End Sub


    Private Sub Button_Load_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button_Load.Click

        Read_File()

    End Sub

    Private Sub PauseButton_Click(sender As Object, e As RoutedEventArgs) Handles PauseButton.Click

    End Sub



    'Find the points of intersection.
    Sub FindLineCircleIntersections(cx As Double, cy As Double, radius As Double,
        point1X As Double, point1Y As Double, point2X As Double, point2Y As Double)

        'assume both intersections valid
        Intersection1Valid = True
        Intersection2Valid = True

        REM out PointF intersection1,X,Y out PointF intersection2 , X,Y
        dx = point2X - point1X
        dy = point2Y - point1Y

        A = dx * dx + dy * dy
        B = 2 * (dx * (point1X - cx) + dy * (point1Y - cy))
        C = (point1X - cx) * (point1X - cx) +
                (point1Y - cy) * (point1Y - cy) -
                radius * radius

        det = B * B - 4 * A * C
        If ((A <= 0.0000001) Or (det < 0)) Then

            REM No real solutions.
            Intersection1Valid = False
            Intersection2Valid = False

        ElseIf (det = 0) Then

            REM One solution.
            t = -B / (2 * A)
            Intersection1X = point1X + t * dx
            Intersection1Y = point1Y + t * dy

            Intersection2Valid = False
        Else

            REM Two solutions.
            t = ((-B + Math.Sqrt(det)) / (2 * A))
            Intersection1X = point1X + t * dx
            Intersection1Y = point1Y + t * dy

            t = ((-B - Math.Sqrt(det)) / (2 * A))
            Intersection2X = point1X + t * dx
            Intersection2Y = point1Y + t * dy

        End If
    End Sub

    Public Function GetPositiveLength(p1x As Double, p1y As Double, p3x As Double, p3y As Double) As Double
        GetPositiveLength = Math.Sqrt((p1x - p3x) * (p1x - p3x) + (p1y - p3y) * (p1y - p3y))
    End Function



End Class

