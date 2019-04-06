

'  Last updated 27 Jan 2012 BWS for newer DAT files
'
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Reflection
Imports System.Security.Permissions
Imports System.IO
Imports System.Math
Imports System.Collections.Generic


<Serializable()> Public Structure EpisodeType ' new style structure as of January 2012
    Dim MicrosecondsPerPoint As Single
    Dim DrugLevel As Single
    Dim DrugName As String
    Dim DrugTime As Single

    Dim GenStr As String
    Dim DACstr As String() ' 0 to 3
    Dim TTLstr As String() ' 0 to 3
    Dim AmpDesc As String() ' 0 to 3

    Dim GenData As Single() ' 0 to 55
    Dim DACdata As Single(,) ' to (3, 41)
    Dim TTLdata As Single(,) ' to (3, 16)

    Dim ExptDesc As String
    Dim WCTime As Single
    Dim Comment As String
    Dim AnalysisComment As String
    Dim ComputerName As String
    Dim SavedFileName As String
    Dim LinkedFileName As String
    Dim AcquisitionDeviceName As String
    Dim ClassVersionNum As Single
    Dim NumPointsInLastTrace As Int32
    Dim ProgramType As Int16
    Dim ProgramVersion As Int32

    Dim TraceKeysInOrder As String
    Dim TraceInitValues As String ' stored as string with spaces between 
    Dim ExtraVectorKeysInOrder As String
    Dim ExtraScalarKeysInOrder As String

    ' Stuff below here is only if reading trace data requested

    Dim Traces As Dictionary(Of String, Int16())
    Dim TraceFactors As Dictionary(Of String, Single) ' value to multiply Int16 numbers to get Singles in correct units
    Dim TraceDesc As Dictionary(Of String, String)
    Dim ExtraVectors As Dictionary(Of String, Single())
    Dim ExtraScalars As Dictionary(Of String, Single)

End Structure

Friend Structure NewProtocolTypeVer7 ' really medium old style outdated as of November 2011
    Friend ProgramType As Int16 ' 25 for Synapse-generation programs
    Friend ProgramVersion As Int32 ' 8 for this version
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
    Friend AllTracesDict As Dictionary(Of String, Double())
    Friend InitValuesDict As Dictionary(Of String, Double)
    Friend CellDescription As String ' statName(0)
    Friend ExptDescription As String ' statName(1)
    Friend InternalSolution As String ' statName(2)
    Friend ExternalSolution As String ' statName(3)
    Friend DrugName As String ' statName(4)
    Friend MsPerPoint As Double
    Friend PointsPerMs As Double
End Structure

NotInheritable Class BenDeserializationBinder
    Inherits SerializationBinder
    Public Overrides Function BindToType(ByVal assemblyName As String, ByVal typeName As String) As System.Type
        Return Type.GetType(typeName + ", " + Assembly.GetExecutingAssembly().FullName)
    End Function
End Class
Public Class clsEpisode
  
    Private mEpi As EpisodeType
    Public Function NewEpisode() As Boolean
        With mEpi
            .ClassVersionNum = 5.19
            .ComputerName = My.Computer.Name
            .ProgramType = 25
            .ProgramVersion = 8
            ReDim .TTLdata(3, 16)
            ReDim .DACdata(3, 41)
            ReDim .GenData(55)
            ReDim .DACstr(3)
            ReDim .TTLstr(3)
            ReDim .AmpDesc(3)
            For i As Long = 0 To 3
                .DACstr(i) = ""
                .TTLstr(i) = ""
                .AmpDesc(i) = ""
            Next
            .Comment = ""
            .AnalysisComment = ""
            .DrugName = ""
            .LinkedFileName = ""
            .SavedFileName = ""
            .ExptDesc = ""
            .AcquisitionDeviceName = "Unknown Device"
            .TraceKeysInOrder = ""
            .ExtraScalarKeysInOrder = ""
            .ExtraVectorKeysInOrder = ""
            .GenStr = "This is a default gen string"
            .Traces = New Dictionary(Of String, Int16())
            .TraceFactors = New Dictionary(Of String, Single)
            .TraceDesc = New Dictionary(Of String, String)
            .TraceInitValues = ""
            .ExtraScalars = New Dictionary(Of String, Single)
            .ExtraVectors = New Dictionary(Of String, Single())
        End With
        Return True
    End Function

    Public Function SaveEpisodeNotUsed(ByVal newFileName As String) As Boolean
        If Strings.Right(newFileName, 4).ToUpper = ".DA4" Then
            mEpi.SavedFileName = newFileName
            Try
                Dim BF As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
                Dim MS As New System.IO.MemoryStream
                BF.Serialize(MS, mEpi)
                My.Computer.FileSystem.WriteAllBytes(newFileName, MS.GetBuffer(), False)
                Return True
            Catch ex As Exception
                MsgBox("Problem in serialization of episode: " + ex.Message)
                Return False
            End Try
        Else
            MsgBox("Only designed to save .DA4 type data files")
            Return False
        End If
    End Function

    Public Function AddExtraVector(ByVal newVectorName As String, ByRef newVector As Single())
        Dim TempArray As Single() = newVector.Clone
        Try
            mEpi.ExtraVectors.Add(CleanUpName(newVectorName), TempArray)
            mEpi.ExtraVectorKeysInOrder = mEpi.ExtraVectorKeysInOrder.Trim + " " + CleanUpName(newVectorName)
            Return True
        Catch ex As Exception
            MsgBox("Problem adding vector " + newVectorName + ": " + ex.Message)
            Return False
        End Try
    End Function
    Public Function AddExtraScalar(ByVal newScalarName As String, ByVal newScalar As Single)
        Try
            mEpi.ExtraScalars.Add(CleanUpName(newScalarName), newScalar)
            mEpi.ExtraScalarKeysInOrder = mEpi.ExtraScalarKeysInOrder.Trim + " " + CleanUpName(newScalarName)
            Return True
        Catch ex As Exception
            MsgBox("Problem adding scalar " + newScalarName + ": " + ex.Message)
            Return False
        End Try
    End Function
    Public Function GetExtraVector(ByVal vecName As String) As Single()
        If mEpi.ExtraVectors.ContainsKey(CleanUpName(vecName)) Then
            Return mEpi.ExtraVectors.Item(CleanUpName(vecName))
        Else
            MsgBox("Could not find extra vector " + vecName + " in episode")
            Return Nothing
        End If
    End Function
    Public Function GetExtraScalar(ByVal scalarName As String) As Single
        If mEpi.ExtraScalars.ContainsKey(CleanUpName(scalarName)) Then
            Return mEpi.ExtraScalars.Item(CleanUpName(scalarName))
        Else
            MsgBox("Could not find extra scalar " + scalarName + " in episode")
            Return Nothing
        End If
    End Function
    Public Function GetNumExtraScalars() As Integer
        Return mEpi.ExtraScalars.Keys.Count()
    End Function
    Public Function GetExtraScalarKeys() As String()
        Return mEpi.ExtraScalarKeysInOrder.Trim.Split(" ")
    End Function
    Public Function GetExtraScalarKeysOneString() As String
        Return mEpi.ExtraScalarKeysInOrder
    End Function
    Public Function GetExtraScalarKeysCSV() As String
        Return ConvertStringArrayToCSVString(GetExtraScalarKeys())
    End Function
    Public Function GetNumExtraVectors() As Integer
        Return mEpi.ExtraVectors.Keys.Count()
    End Function
    Public Function GetExtraVectorKeys() As String()
        Return mEpi.ExtraVectorKeysInOrder.Trim.Split(" ")
    End Function
    Public Function GetExtraVectorKeysOneString() As String
        Return mEpi.ExtraVectorKeysInOrder
    End Function
    Public Function GetExtraVectorsKeysCSV() As String
        Return ConvertStringArrayToCSVString(GetExtraVectorKeys())
    End Function
    Public Function GetNumTraces() As Integer
        Return mEpi.Traces.Keys.Count
    End Function
    Public Function GetTraceKeys() As String()
        Return mEpi.TraceKeysInOrder.Trim.Split(" ")
    End Function
    Public Function GetTraceKeysOneString() As String
        Return mEpi.TraceKeysInOrder
    End Function
    Public Function GetTraceKeysCSV() As String
        Return ConvertStringArrayToCSVString(GetTraceKeys())
    End Function

    Public Function SetMicrosecPerPoint(newValue As Single) As Boolean
        mEpi.MicrosecondsPerPoint = newValue
        Return True
    End Function
    Public Function GetMicrosecPerPoint() As Single
        Return mEpi.MicrosecondsPerPoint
    End Function
    Public Function GetMsPerPoint() As Single
        Return mEpi.MicrosecondsPerPoint / 1000.0
    End Function
    Public Function GetPointsPerMs() As Long
        Return CLng(1.0 / (mEpi.MicrosecondsPerPoint / 1000.0))
    End Function
    Public Function GetSweepWindow() As Single ' in milliseconds
        Return mEpi.NumPointsInLastTrace * (mEpi.MicrosecondsPerPoint / 1000.0)
    End Function
    Public Function GetNumPoints() As Long
        Return mEpi.NumPointsInLastTrace
    End Function
    Public Function GetProgramVersion() As Int32
        Return mEpi.ProgramVersion
    End Function
    Public Function GetProgramType() As Int16
        Return mEpi.ProgramType
    End Function
    Public Function SetProgramType(newType As Int16) As Boolean
        mEpi.ProgramType = newType
        Return True
    End Function
    Public Function GetAcquistionDeviceName() As String
        Return mEpi.AcquisitionDeviceName
    End Function
    Public Function SetAcquisitionDeviceName(NewName As String) As Boolean
        mEpi.AcquisitionDeviceName = NewName
        Return True
    End Function
    Public Function SetDrugLevel(newLevel As Single) As Boolean
        mEpi.DrugLevel = newLevel
        Return True
    End Function
    Public Function GetDrugLevel() As Single
        Return mEpi.DrugLevel
    End Function
    Public Function SetDrugName(newName As String) As Boolean
        mEpi.DrugName = newName
        Return True
    End Function
    Public Function GetDrugName() As String
        Return mEpi.DrugName
    End Function
    Public Function SetDrugTime(newTime As Single) As Boolean
        mEpi.DrugTime = newTime
        Return True
    End Function
    Public Function GetDrugTime() As Single
        Return mEpi.DrugTime
    End Function

    Public Function SetGenData(newData As Single()) As Boolean
        mEpi.GenData = newData.Clone
        Return True
    End Function
    Public Function GetGenData() As Single()
        Return mEpi.GenData
    End Function
    Public Function SetDACdata(newData As Single(,)) As Boolean
        mEpi.DACdata = newData.Clone
        Return True
    End Function
    Public Function GetDACdata(chanNum As Long) As Single()
        Dim TempData As Single()
        ReDim TempData(41)
        For i As Long = 0 To 41
            TempData(i) = mEpi.DACdata(chanNum, i)
        Next
        Return TempData
    End Function
    Public Function GetDACdataAll() As Single(,)
        Return mEpi.DACdata
    End Function
    Public Function SetTTLdata(newData As Single(,)) As Boolean
        mEpi.TTLdata = newData.Clone
        Return True
    End Function
    Public Function GetTTLdata(chanNum As Long) As Single()
        Dim TempData As Single()
        ReDim TempData(16)
        For i As Long = 0 To 16
            TempData(i) = mEpi.TTLdata(chanNum, i)
        Next
        Return TempData
    End Function
    Public Function GetTTLdataAll() As Single(,)
        Return mEpi.TTLdata
    End Function
    Public Function SetExptDesc(newDesc As String) As Boolean
        mEpi.ExptDesc = newDesc
        Return True
    End Function
    Public Function GetExptDesc() As String
        Return mEpi.ExptDesc
    End Function
    Public Function SetWCTime(newTime As Single) As Boolean
        mEpi.WCTime = newTime
        Return True
    End Function
    Public Function GetWCTime() As Single
        Return mEpi.WCTime
    End Function
    Public Function SetComment(newStr As String) As Boolean
        mEpi.Comment = newStr
        Return True
    End Function
    Public Function GetComment() As String
        Return mEpi.Comment
    End Function
    Public Function SetAnalysisComment(newStr As String) As Boolean
        mEpi.AnalysisComment = newStr
        Return True
    End Function
    Public Function GetAnalysisComment() As String
        Return mEpi.AnalysisComment
    End Function
    Public Function SetLinkedFileName(newStr As String) As Boolean
        mEpi.LinkedFileName = newStr
        Return True
    End Function
    Public Function GetLinkedFileName() As String
        Return mEpi.LinkedFileName
    End Function
    Public Function GetSavedFileName() As String
        Return mEpi.SavedFileName
    End Function
    Public Function GetComputerName() As String
        Return mEpi.ComputerName
    End Function
    Public Function GetStoredClassVersionNum() As Single
        Return mEpi.ClassVersionNum
    End Function
    Public Function GetClassVersionNum() As Single
        Return mEpi.ClassVersionNum
    End Function
    Public Function SetGenDescriptionString(newStr As String) As Boolean
        mEpi.GenStr = newStr
        Return True
    End Function
    Public Function GetGenDescriptionString() As String
        Return mEpi.GenStr
    End Function
    Public Function SetDACdescString(chanNum As Long, newStr As String) As Boolean
        mEpi.DACstr(chanNum) = newStr
        Return True
    End Function
    Public Function GetDACdescString(chanNum As Long) As String
        Return mEpi.DACstr(chanNum)
    End Function
    Public Function GetDACdescStringAll() As String()
        Return mEpi.DACstr
    End Function
    Public Function SetTTLdescString(chanNum As Long, newStr As String) As Boolean
        mEpi.TTLstr(chanNum) = newStr
        Return True
    End Function
    Public Function GetTTLdescString(chanNum As Long) As String
        Return mEpi.TTLstr(chanNum)
    End Function
    Public Function GetTTLdescStringAll() As String()
        Return mEpi.TTLstr
    End Function
    Public Function GetAmpDescAll() As String()
        Return mEpi.AmpDesc
    End Function
    Public Function GetAmpDescString(ChanNum As Long) As String
        Return mEpi.AmpDesc(ChanNum)
    End Function
    Public Function SetAmpDescString(ChanNum As Long, NewString As String) As Boolean
        mEpi.AmpDesc(ChanNum) = NewString
        Return True
    End Function
    Public Function GetTraceDesc(TraceKey As String) As String
        If mEpi.TraceDesc.ContainsKey(TraceKey) Then
            Return mEpi.TraceDesc.Item(TraceKey)
        Else
            MsgBox("Could not find key: " + TraceKey + " in dictionary")
            Return Nothing
        End If
    End Function
    Public Function GetTraceAsSingle(TraceKey As String) As Single()
        If mEpi.Traces.ContainsKey(TraceKey) Then
            Dim TempFactor As Single = mEpi.TraceFactors.Item(TraceKey)
            Dim TempDataInt As Int16() = mEpi.Traces.Item(TraceKey)
            Dim TempDataSingle As Single()
            ReDim TempDataSingle(TempDataInt.Length - 1)
            For i As Long = 0 To TempDataInt.Length - 1
                TempDataSingle(i) = TempDataInt(i) * TempFactor
            Next
            Return TempDataSingle
        Else
            MsgBox("Could not find key: " + TraceKey + " in dictionary")
            Return Nothing
        End If
    End Function
    Public Function GetTraceAsDouble(TraceKey As String) As Double()
        If mEpi.Traces.ContainsKey(TraceKey) Then
            Dim TempFactor As Double = CDbl(mEpi.TraceFactors.Item(TraceKey))
            Dim TempDataInt As Int16() = mEpi.Traces.Item(TraceKey)
            Dim TempDataDouble As Double()
            ReDim TempDataDouble(TempDataInt.Length - 1)
            For i As Long = 0 To TempDataInt.Length - 1
                TempDataDouble(i) = TempDataInt(i) * TempFactor
            Next
            Return TempDataDouble
        Else
            MsgBox("Could not find key: " + TraceKey + " in traces dictionary")
            Return Nothing
        End If
    End Function
    Public Function SaveAmpDescription(AmpNum As Long, Desc As String) As Boolean
        Dim TempKey As String = ""
        Select Case AmpNum
            Case 0
                TempKey = "AmpA"
            Case 1
                TempKey = "AmpB"
            Case 2
                TempKey = "AmpC"
            Case 3
                TempKey = "AmpD"
            Case Else
                MsgBox("Improper AmpNum passed. Must be 0-3")
        End Select
        If TempKey.Length > 0 Then
            mEpi.AmpDesc(TempKey) = Desc
            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetAmpDescription(AmpNum As Long) As String
        Dim TempKey As String = ""
        Select Case AmpNum
            Case 0
                TempKey = "AmpA"
            Case 1
                TempKey = "AmpB"
            Case 2
                TempKey = "AmpC"
            Case 3
                TempKey = "AmpD"
            Case Else
                MsgBox("Improper AmpNum passed. Must be 0-3")
        End Select
        If TempKey.Length > 0 Then
            Return mEpi.AmpDesc(TempKey)
        Else
            Return Nothing
        End If

    End Function
    Public Function SetWholeStructure(ByVal newStructure As EpisodeType) As Boolean
        mEpi = newStructure
        Return True
    End Function
    Public Function GetWholeStructure() As EpisodeType
        Return mEpi
    End Function

    Public Function ChanLetterToNumber(chanLetter As String) As Long
        Dim TempLetter As String = chanLetter.Substring(0, 1).ToUpper
        Select Case TempLetter
            Case "A"
                Return 0
            Case "B"
                Return 1
            Case "C"
                Return 2
            Case "D"
                Return 3
            Case Else
                MsgBox("Improper letter passed; only works with A-D")
                Return -1
        End Select
    End Function


    Public Function SaveTrace(TraceKeyIn As String, Data As Int16(), Factor As Single, Desc As String) As Boolean
        Dim RetOkay As Boolean = True
        Dim TraceName As String = TraceKeyIn.Trim
        If TraceName.Contains(" ") Then
            MsgBox("Problem in SaveTrace with traceName containing space")
            RetOkay = False
        Else
            If Factor = 0 Then
                MsgBox("Problem with TraceFactor being zero")
                RetOkay = False
            Else
                Dim TempKey As String = TraceName
                If mEpi.Traces.Keys.Contains(TempKey) Then
                    Dim TempL As Long = 1
                    Dim OldTempKey As String = TempKey
                    Do
                        TempL += 1
                        TempKey = OldTempKey + TempL.ToString
                    Loop Until Not mEpi.Traces.Keys.Contains(TempKey)
                End If
                Dim TempData As Int16() = Data.Clone
                mEpi.Traces.Add(TempKey, TempData)
                mEpi.TraceFactors.Add(TempKey, Factor)
                mEpi.TraceDesc.Add(TempKey, Desc)
                mEpi.NumPointsInLastTrace = TempData.Length
                Dim TempD As Double = 0 ' for initial value of this trace
                If TempData.Length > 9 Then
                    For i As Long = 0 To 9
                        TempD += TempData(i)
                    Next
                    TempD = TempD / 10.0
                Else
                    TempD = TempData(0)
                End If
                mEpi.TraceInitValues = mEpi.TraceInitValues.Trim + " " + Format(TempD * Factor, "F4")
                mEpi.TraceKeysInOrder = mEpi.TraceKeysInOrder.Trim + " " + TempKey
            End If
        End If
        Return RetOkay
    End Function


    ' Private Functions Below Here

    Public Function SaveEpisode(NewFileName As String) As Boolean
        If Not Strings.Right(NewFileName, 4).ToUpper = ".DAT" Then NewFileName = NewFileName + ".dat"
        Dim JunkInt32 As Int32 = 0
        Dim JunkSingle As Single = 0
        Dim JunkByte As Byte = 0
        Dim i As Long, j As Long, k As Long
        Dim FileNameLength As Int16
        Dim TempStr As String

        Dim bWriter As New BinaryWriter(File.Open(NewFileName, FileMode.Create, FileAccess.Write))
        With mEpi
            .SavedFileName = NewFileName
            .ProgramType = 25 ' Synapse generation
            .ProgramVersion = 8 ' always save in new stye
            bWriter.Write(.ProgramType)
            bWriter.Write(.ProgramVersion)
            bWriter.Write(JunkInt32) ' protocolBytes
            bWriter.Write((.NumPointsInLastTrace - 1) * (.MicrosecondsPerPoint / 1000)) ' SweepWindow
            bWriter.Write(.MicrosecondsPerPoint)
            bWriter.Write(.NumPointsInLastTrace)
            bWriter.Write(.WCTime)
            bWriter.Write(.DrugTime)
            bWriter.Write(.DrugLevel)
            bWriter.Write(JunkInt32) ' Simulated data
            For i = 0 To 9
                bWriter.Write(JunkByte)
            Next

            For i = 0 To 55
                bWriter.Write(.GenData(i))
            Next

            For j = 0 To 3
                For k = 0 To 9
                    bWriter.Write(JunkByte)
                Next
                For i = 0 To 16
                    bWriter.Write(.TTLdata(j, i))
                Next
            Next

            For j = 0 To 3
                For k = 0 To 9
                    bWriter.Write(JunkByte)
                Next
                For i = 0 To 41
                    bWriter.Write(.DACdata(j, i))
                Next
                TempStr = .DACstr(j).Trim
                FileNameLength = TempStr.Length
                bWriter.Write(FileNameLength)
                For k = 0 To FileNameLength - 1
                    JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                    bWriter.Write(JunkByte)
                Next
            Next

            ' now we have differences with new style header started in Jan 2012

            bWriter.Write(.ClassVersionNum)

            TempStr = .Comment.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .AnalysisComment.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .DrugName.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .ExptDesc.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .ComputerName.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .SavedFileName.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .LinkedFileName.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .AcquisitionDeviceName.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .TraceKeysInOrder.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .TraceInitValues.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .ExtraScalarKeysInOrder.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .ExtraVectorKeysInOrder.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            TempStr = .GenStr.Trim
            FileNameLength = TempStr.Length
            bWriter.Write(FileNameLength)
            For k = 0 To FileNameLength - 1
                JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                bWriter.Write(JunkByte)
            Next

            For j = 0 To 3
                TempStr = .TTLstr(j).Trim
                FileNameLength = TempStr.Length
                bWriter.Write(FileNameLength)
                For k = 0 To FileNameLength - 1
                    JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                    bWriter.Write(JunkByte)
                Next
            Next

            For j = 0 To 3
                TempStr = .AmpDesc(j).Trim
                FileNameLength = TempStr.Length
                bWriter.Write(FileNameLength)
                For k = 0 To FileNameLength - 1
                    JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                    bWriter.Write(JunkByte)
                Next
            Next

            ' Now write episode data

            Dim TraceKeys As String() = mEpi.TraceKeysInOrder.Trim.Split(" ")
            Dim ExtraScalarKeys As String() = mEpi.ExtraScalarKeysInOrder.Trim.Split(" ")
            Dim ExtraVectorKeys As String() = mEpi.ExtraVectorKeysInOrder.Trim.Split(" ")
            Dim TraceKey As String
            Dim TraceData As Int16()
            Dim TraceFactor As Single
            Dim TraceLength As Int32
            Dim TraceDesc As String
            Dim ExtraScalar As Single
            Dim ExtraVector As Single()

            If mEpi.TraceKeysInOrder.Length > 0 Then
                For TraceNum As Long = 0 To TraceKeys.Length - 1
                    TraceKey = TraceKeys(TraceNum)
                    TraceData = .Traces.Item(TraceKey)
                    TraceLength = TraceData.Length
                    TraceFactor = .TraceFactors.Item(TraceKey)
                    TraceDesc = .TraceDesc.Item(TraceKey)

                    bWriter.Write(TraceFactor)
                    bWriter.Write(TraceLength)
                    TempStr = TraceDesc.Trim
                    FileNameLength = TempStr.Length
                    bWriter.Write(FileNameLength)
                    For k = 0 To FileNameLength - 1
                        JunkByte = CByte(Asc(TempStr.Substring(k, 1)))
                        bWriter.Write(JunkByte)
                    Next
                    For k = 0 To TraceLength - 1
                        bWriter.Write(TraceData(k))
                    Next
                Next
            End If

            If mEpi.ExtraScalarKeysInOrder.Length > 0 Then
                For ExtraNum As Long = 0 To ExtraScalarKeys.Length - 1
                    ExtraScalar = .ExtraScalars.Item(ExtraScalarKeys(ExtraNum))
                    bWriter.Write(ExtraScalar)
                Next
            End If

            If mEpi.ExtraVectorKeysInOrder.Length > 0 Then
                For ExtraNum As Long = 0 To ExtraVectorKeys.Length - 1
                    ExtraVector = .ExtraVectors.Item(ExtraVectorKeys(ExtraNum))
                    TraceLength = ExtraVector.Length
                    bWriter.Write(TraceLength)
                    For k = 0 To TraceLength - 1
                        bWriter.Write(ExtraVector(k))
                    Next
                Next
            End If

            bWriter.Close()

        End With

        Return True
     
    End Function

    Private Function ReadEpisodeDA4NotUsed(ByVal newFileName As String) As Boolean
        Try
            Dim BF As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
            BF.Binder = New BenDeserializationBinder
            Dim bytes As Byte() = My.Computer.FileSystem.ReadAllBytes(newFileName)
            mEpi = DirectCast(BF.Deserialize(New System.IO.MemoryStream(bytes)), EpisodeType)
            Return True
        Catch ex As Exception
            MsgBox("Problem in deserialization of episode: " + ex.Message)
            Return False
        End Try
    End Function

    Public Function ReadDATfile(FileName As String, ReadHeaderOnly As Boolean) As Boolean
        Dim FileNumber As Long
        Dim TempSingle() As Single
        Dim TempTraceInt As Int16()
        Dim FileNameLength As Int16
        Dim TempStr As String = ""
        Dim enc As New System.Text.ASCIIEncoding()
        Dim Fobject As FileInfo = My.Computer.FileSystem.GetFileInfo(FileName)
        Dim TempBytes() As Byte
        Dim bReaderTest As BinaryReader
        Dim mH As NewProtocolTypeVer7
        Dim IsOldStyleDatFile As Boolean = False
        Dim IsNewStyleDatFile As Boolean = False
        Dim CurKey As String
        Dim CurAmpLetter As String
        Dim Parts As String()
        Dim FirstTrace As Boolean = True
        Dim TempFactor As Single
        Dim JunkByteArray As Byte()
        Dim JunkSingle As Single
        Dim JunkInt32 As Int32
        Dim i As Long, j As Long

        If Not My.Computer.FileSystem.FileExists(FileName) Then
            MsgBox("Cannot find data file: " + FileName)
            Return False
        End If
        Try
            bReaderTest = New BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read))
        Catch ex As Exception
            MsgBox("Problem with reading this data file")
            Return Nothing
            Exit Function
        End Try
        Dim OldProgramType As Int16 = bReaderTest.ReadInt16 ' program type
        Dim OldProgramVersion As Int32 = bReaderTest.ReadInt32 ' Program version
        Dim OldProtocolBytes As Int32 = bReaderTest.ReadInt32 ' protocol bytes
        Dim OldSweepWindow As Single = bReaderTest.ReadSingle ' sweepwindow
        Dim OldTimePerPoint As Single = bReaderTest.ReadSingle ' TimePerPoint
        If OldProgramType = 25 And OldProgramVersion >= 8 Then
            IsNewStyleDatFile = True
        Else
            If OldProgramType <> 25 Then IsOldStyleDatFile = True
            If Abs(OldProtocolBytes) > 20000 Then IsOldStyleDatFile = True
            If Abs(OldTimePerPoint) > 2000 Then IsOldStyleDatFile = True
        End If
        bReaderTest.Close()

        If IsNewStyleDatFile Then
            Dim bReader As New BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read))
            With mEpi
                ' This part is the same with medium old header 
                .ProgramType = bReader.ReadInt16
                .ProgramVersion = bReader.ReadInt32
                JunkInt32 = bReader.ReadInt32 ' ProtocolBytes
                JunkSingle = bReader.ReadSingle ' SweepWindow
                .MicrosecondsPerPoint = bReader.ReadSingle
                .NumPointsInLastTrace = bReader.ReadInt32
                .WCTime = bReader.ReadSingle
                .DrugTime = bReader.ReadSingle
                .DrugLevel = bReader.ReadSingle
                JunkInt32 = bReader.ReadInt32 ' Simulated Data
                JunkByteArray = bReader.ReadBytes(10) ' advance through some hidden bytes; left for compatability
                ReDim .GenData(55)
                For i = 0 To 55
                    .GenData(i) = bReader.ReadSingle
                Next

                ReDim .TTLdata(3, 16)
                For j = 0 To 3
                    JunkByteArray = bReader.ReadBytes(10)
                    For i = 0 To 16
                        .TTLdata(j, i) = bReader.ReadSingle
                    Next
                Next

                ReDim .DACdata(3, 41)
                ReDim .DACstr(3)
                For j = 0 To 3
                    JunkByteArray = bReader.ReadBytes(10)
                    For i = 0 To 41
                        .DACdata(j, i) = bReader.ReadSingle
                    Next
                    FileNameLength = bReader.ReadInt16
                    If FileNameLength > 512 Then Stop
                    ReDim TempBytes(FileNameLength - 1)
                    TempBytes = bReader.ReadBytes(FileNameLength)
                    .DACstr(j) = enc.GetString(TempBytes)
                Next

                ' now we have differences with new style header started in Jan 2012

                .ClassVersionNum = bReader.ReadSingle

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .Comment = enc.GetString(TempBytes)
                mEpi.Comment = .Comment

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .AnalysisComment = enc.GetString(TempBytes)
                mEpi.AnalysisComment = .AnalysisComment

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .DrugName = enc.GetString(TempBytes)
                mEpi.DrugName = .DrugName

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .ExptDesc = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .ComputerName = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .SavedFileName = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .LinkedFileName = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .AcquisitionDeviceName = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .TraceKeysInOrder = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .TraceInitValues = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .ExtraScalarKeysInOrder = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .ExtraVectorKeysInOrder = enc.GetString(TempBytes)

                FileNameLength = bReader.ReadInt16
                If FileNameLength > 512 Then Stop
                ReDim TempBytes(FileNameLength - 1)
                TempBytes = bReader.ReadBytes(FileNameLength)
                .GenStr = enc.GetString(TempBytes)

                ReDim .TTLstr(3)
                For j = 0 To 3
                    FileNameLength = bReader.ReadInt16
                    If FileNameLength > 512 Then Stop
                    ReDim TempBytes(FileNameLength - 1)
                    TempBytes = bReader.ReadBytes(FileNameLength)
                    .TTLstr(j) = enc.GetString(TempBytes)
                Next

                ReDim .AmpDesc(3)
                For j = 0 To 3
                    FileNameLength = bReader.ReadInt16
                    If FileNameLength > 512 Then Stop
                    ReDim TempBytes(FileNameLength - 1)
                    TempBytes = bReader.ReadBytes(FileNameLength)
                    .AmpDesc(j) = enc.GetString(TempBytes)
                Next
            End With

            If Not ReadHeaderOnly Then
                ' Now read data into mEpi structure

                Dim TraceKeys As String() = mEpi.TraceKeysInOrder.Trim.Split(" ")
                Dim TraceData As Int16()
                Dim TraceFactor As Single
                Dim TraceLength As Int32
                Dim TraceDesc As String
                Dim TraceIndex As Int32
                Dim ExtraScalar As Single
                Dim ExtraVector As Single()

                If mEpi.TraceKeysInOrder.Length > 0 Then
                    For TraceNum As Long = 0 To TraceKeys.Length - 1
                        TraceFactor = bReader.ReadSingle
                        TraceLength = bReader.ReadInt32
                        FileNameLength = bReader.ReadInt16
                        If FileNameLength > 512 Then Stop
                        ReDim TempBytes(FileNameLength - 1)
                        TempBytes = bReader.ReadBytes(FileNameLength)
                        TraceDesc = enc.GetString(TempBytes)
                        ReDim TraceData(TraceLength - 1)
                        For TraceIndex = 0 To TraceLength - 1
                            TraceData(TraceIndex) = bReader.ReadInt16
                        Next
                        mEpi.Traces.Add(TraceKeys(TraceNum), TraceData)
                        mEpi.TraceFactors.Add(TraceKeys(TraceNum), TraceFactor)
                        mEpi.TraceDesc.Add(TraceKeys(TraceNum), TraceDesc)
                    Next
                End If

                Dim ExtraScalerKeys As String() = mEpi.ExtraScalarKeysInOrder.Trim.Split(" ")
                If mEpi.ExtraScalarKeysInOrder.Length > 0 Then
                    For ExtraNum As Long = 0 To ExtraScalerKeys.Length - 1
                        ExtraScalar = bReader.ReadSingle
                        mEpi.ExtraScalars.Add(ExtraScalerKeys(ExtraNum), ExtraScalar)
                    Next
                End If ' ExtraScalarKeys

                Dim ExtraVectorKeys As String() = mEpi.ExtraVectorKeysInOrder.Trim.Split(" ")
                If mEpi.ExtraVectorKeysInOrder.Length > 0 Then
                    For ExtraNum As Long = 0 To ExtraVectorKeys.Length - 1
                        TraceLength = bReader.ReadInt32
                        ReDim ExtraVector(TraceLength - 1)
                        For TraceIndex = 0 To TraceLength - 1
                            ExtraVector(TraceIndex) = bReader.ReadSingle
                        Next
                        mEpi.ExtraVectors.Add(ExtraVectorKeys(ExtraNum), ExtraVector)
                    Next
                End If ' ExtraVectorKeys

            End If ' ReadHeaderOnly
            bReader.Close()

        Else

            ReDim mH.GenArray(55)
            ReDim mH.TTLData(3, 16)
            ReDim mH.DacData(3, 41)
            ReDim mH.DacAWFFileNames(3)
            For i = 0 To 3
                mH.DacAWFFileNames(i) = ""
            Next
            ReDim mH.TempKeys(19)
            ReDim mH.StatBox(24)
            ReDim mH.StatValue(24)
            ReDim mH.StatName(24)
            For i = 0 To 24
                mH.StatName(i) = ""
            Next
            ReDim mH.InitValues(19)

            If IsOldStyleDatFile Then

                ' really old old style format
                mEpi.GenStr = "Really old style DAT file format"
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
                    .ProgramVersion = 6 ' to differentiate it from current files
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

                    .Comment = ""
                    .AnalysisComment = "Very old style format"

                End With
                bReader.Close()

                If Not ReadHeaderOnly Then
                    Dim TempSingleIn() As Single
                    FileNumber = FreeFile()
                    FileOpen(FileNumber, FileName, OpenMode.Binary)
                    ReDim TempSingleIn((mH.NumChannels * mH.NumPoints) - 1)
                    FileGet(FileNumber, TempSingleIn, mH.ProtocolBytes)

                    ReDim TempSingle(mH.NumPoints)
                    ReDim TempTraceInt(mH.NumPoints)
                    Dim iCount As Long
                    For K As Long = mH.NumChannels - 1 To 0 Step -1
                        iCount = 0
                        For j = K To TempSingle.Length - mH.NumChannels Step mH.NumChannels
                            TempSingle(iCount) = CDbl(TempSingleIn(j))
                            iCount += 1
                        Next
                        If mH.TempKeys(K).Length > 0 Then
                            Parts = ConvertOldKeyToNewKey(mH.TempKeys(K))
                            CurKey = Parts(0)
                            CurAmpLetter = Parts(1)
                        Else
                            CurKey = "Signal" + K.ToString
                            CurAmpLetter = "X"
                        End If
                        TempFactor = DetermineFactor(TempSingle)
                        For L As Long = 0 To TempSingle.Length - 1
                            TempTraceInt(L) = CInt(TempSingle(L) * TempFactor)
                        Next
                        CurKey = GenerateOldStyleKey(CurKey, CurAmpLetter)
                        SaveTrace(CurKey, TempTraceInt, 1.0 / TempFactor, "Very Old Style")
                    Next
                    FileClose(FileNumber)
                End If
            Else ' OldStyle

                ' just medium old style format
                mEpi.GenStr = "Medium old style DAT format"
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
                    For i = 0 To 55
                        .GenArray(i) = bReader.ReadSingle
                    Next

                    For j = 0 To 3
                        .junk = bReader.ReadBytes(10)
                        For i = 0 To 16
                            .TTLData(j, i) = bReader.ReadSingle
                        Next
                    Next

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
                    .AnalysisComment = "Regular Synapse style .dat file"

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

                If Not ReadHeaderOnly Then
                    FileNumber = FreeFile()
                    FileOpen(FileNumber, FileName, OpenMode.Binary)
                    With mH
                        ReDim TempSingle(.NumPoints)
                        ReDim TempTraceInt(.NumPoints)
                        For Trace As Long = 0 To .NumChannels - 1
                            If .TempKeys(Trace).Length > 0 Then
                                Parts = ConvertOldKeyToNewKey(.TempKeys(Trace))
                                CurKey = Parts(0)
                                CurAmpLetter = Parts(1)
                            Else
                                CurKey = "Signal" + Trace.ToString
                                CurAmpLetter = "X"
                            End If
                            If FirstTrace Then
                                FileGet(FileNumber, TempSingle, .ProtocolBytes)
                                FirstTrace = False
                            Else
                                FileGet(FileNumber, TempSingle)
                            End If
                            TempFactor = DetermineFactor(TempSingle)
                            For L As Long = 0 To TempSingle.Length - 1
                                TempTraceInt(L) = CInt(TempSingle(L) * TempFactor)
                            Next
                            CurKey = GenerateOldStyleKey(CurKey, CurAmpLetter)
                            SaveTrace(CurKey, TempTraceInt, 1.0 / TempFactor, "Medium Old Style")
                        Next
                    End With
                    FileClose(FileNumber)
                End If ' read header only
            End If ' isOldStyleDatFile

            ' now transfer from mH to modern structure

            With mEpi
                .GenData = mH.GenArray
                .DACdata = mH.DacData
                .TTLdata = mH.TTLData
                .MicrosecondsPerPoint = mH.TimePerPoint
                .DrugLevel = mH.DrugLevel
                .DrugTime = mH.DrugTime
                .WCTime = mH.WCTime
                .Comment = mH.Comment
                .AnalysisComment = mH.AnalysisComment
                For i = 0 To 3
                    .DACstr(i) = mH.DacAWFFileNames(i)
                Next
                .SavedFileName = mH.FileName
                .AcquisitionDeviceName = "ITC-18"
                .ProgramType = mH.ProgramType
                .ProgramVersion = mH.ProgramVersion
                .ComputerName = "Unknown"
            End With

        End If ' newStyleDatFile

        Return True
    End Function
    Private Function GenerateOldStyleKey(CurKey As String, AmpLetter As String) As String
        Dim NewKey As String = CurKey.Trim + AmpLetter.ToUpper.Trim
        Return NewKey
    End Function
    Private Function DetermineFactor(newArray As Single()) As Single
        Dim Factor As Double = 1
        If newArray.Max < 327 And newArray.Min > -327 Then
            Factor = 100
        Else
            If newArray.Max < 3270 And newArray.Min > -3270 Then
                Factor = 10
            End If
        End If
        Return Factor
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

    Private Function ConvertToDouble(ByRef inArray As Single()) As Double()
        Dim i As Int32
        Dim TempArray() As Double
        ReDim TempArray(inArray.Length - 1)
        For i = 0 To inArray.Length - 1
            TempArray(i) = CType(inArray(i), Double)
        Next
        Return TempArray
    End Function
    Private Function CleanUpName(ByVal inName As String) As String
        Dim TempStr As String
        TempStr = inName.Replace(" ", "").Trim
        TempStr = TempStr.Replace("/", ".")
        TempStr = TempStr.Replace("\", ".")
        Return TempStr
    End Function
    Private Function ConvertStringArrayToCSVString(inArray As String()) As String
        Dim TempStr As String = ""
        For i As Long = 0 To inArray.Length - 1
            TempStr = TempStr + inArray(i).Trim + ","
        Next
        TempStr = TempStr.Substring(0, TempStr.Length - 1) ' to remove last comma
        Return TempStr
    End Function
    Private Function ConvertOldKeyToNewKey(oldKey As String) As String()
        Dim OutStr As String()
        ReDim OutStr(1) ' index0 is newKey and index1 is ampLetter
        Dim Parts As String() = oldKey.Split(" ")
        OutStr(0) = Parts(0)
        If OutStr(0) = "Volt" Or OutStr(0) = "Cur" Then
            Select Case CInt(Parts(1).Substring(3, 1))
                Case 0, 1
                    OutStr(1) = "A"
                Case 2, 3
                    OutStr(1) = "B"
                Case 4, 5
                    OutStr(1) = "C"
                Case 6, 7
                    OutStr(1) = "D"
                Case Else
                    MsgBox("Unexpected amp code in oldKey")
                    OutStr(1) = "X"
            End Select
        Else
            ' for Stimulus Amp A/99 type oldKeys
            OutStr(1) = Parts(2).Substring(0, 1)
        End If
        Return OutStr
    End Function


    Public Sub New()
        NewEpisode()
    End Sub
End Class
