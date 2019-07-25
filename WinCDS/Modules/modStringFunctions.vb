﻿Module modStringFunctions
    Public Const STR_CHR_UCASE As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    Public Const STR_CHR_LCASE As String = "abcdefghijklmnopqrstuvwxyz"
    Public Const STR_CHR_DIGIT As String = "0123456789"

    Public Function QuoteString(ByVal S As String, Optional ByVal vSingle As Boolean = False, Optional ByVal vEscapeContents As Boolean = True, Optional ByVal vForce As Boolean = True) As String
        '::::QuoteString
        ':::SUMMARY
        ': Wrap a string in quotes (single or double)
        ':::DESCRIPTION
        ': Wraps the specified string in either single or double quotes, optionally escaping the contents.
        ':::PARAMETERS
        ': - S - Source string to be modified
        ': - [vSingle] - True for single quotes, False for double.  Default = False
        ': - [vEscapeContents] - If True, turns ' into '' and " into "", depending on the value of vSingle. Default = True
        ': - [vForce] - If true, puts the string in quotes even if it already IS in quotes.  Otherwise, performs smart checking and doesn't re-quote. Default = True
        ':::EXAMPLE
        ': - QuoteString("Test String") ==> """TestString"""
        ': - QuoteString("'Test String'", True) ==> "'TestString'" ' Doesn't re-wrap already quoted strings (using Single quotes).
        ':::RETURN
        ':  String - The slug generated from the source.
        ':::SEE ALSO
        ':  ArrangeString, ReduceString, ProtectSQL
        Dim Q As String
        Q = IIf(vSingle, "'", """")
        If vForce Or (Left(S, 1) <> Q Or Right(S, 1) <> Q) Then
            If vEscapeContents Then S = Replace(S, Q, Q & Q)
            S = Q & S & Q
        End If
        QuoteString = S
    End Function

    Public Function IsInStr(ByVal sHaystack As String, ByVal sNeedle As String) As Boolean
        '::::IsInStr
        ':::SUMMARY
        ': Determine whether a string contains sub-string
        ':::DESCRIPTION
        ': Returns True if string <sHaystack> contains sub-string <sNeedle).
        ':
        ': This is the equivalent, yet shorter than, (InStr(sHaystack, sNeedle) <> 0).  It is considered a VB6 shortcoming
        ': that their library is not extended to this simple functionality.
        ':::PARAMETERS
        ': - sHaystack - The string to search in.
        ': - sNeedle - The string to search for.
        ': - ReturnFirstOrLast - True makes function return the first name.  False makes it return the last name.
        ':::EXAMPLE
        ': - IsInStr("abcdefghijk", "def") == True
        ': - IsInStr("abcdefghijk", "deg") == False
        ':::RETURN
        ':  Boolean
        ':::SEE ALSO
        ': SplitWord, FirstLast
        ':
        '::Antonym
        ': IsNotInStr, NotIsInStr
        IsInStr = InStr(sHaystack, sNeedle) <> 0
    End Function

    Public Function IsInStrArray(ByVal Source As String, ParamArray Arr() As Object) As Boolean
        On Error Resume Next
        Dim L As Object
        For Each L In Arr
            If IsInStr(Source, L) Then IsInStrArray = True : Exit Function
        Next
    End Function
    Public Function AlignString(ByVal Source As String, ByVal DesiredLength As Integer, Optional ByVal Alignment As ContentAlignment = ContentAlignment.MiddleRight, Optional ByVal Truncate As Boolean = False) As String
        '::::AlignString
        ':::SUMMARY
        ': Align a string
        ':::DESCRIPTION
        ': Aligns a string based on relevant criteria.  Useful for forcing a fixed-width or left or right alignment.
        ':::RETURN
        ':  String
        ':::SEE ALSO
        ':  WrapLongTextByPrintWidth, WrapLongText, SplitLongText
        '::Aliases
        ': ArrangeString
        ' If the string exceeds the de

        If Len(Source) >= DesiredLength Then
            If Truncate Then
                AlignString = Left(Source, DesiredLength)
            Else
                AlignString = Source
            End If
        Else
            AlignString = Space(DesiredLength - Len(Source))
            Select Case Alignment
                'Case vbAlignLeft
                Case ContentAlignment.MiddleLeft
                    AlignString = Source & AlignString
                    'AlignString = Source
                    'Case vbAlignRight, vbAlignNone
                Case ContentAlignment.MiddleRight
                    AlignString = AlignString & Source
                    'AlignString = Source & AlignString
                Case ContentAlignment.MiddleCenter
                    AlignString = AlignString & Source
                Case Else
                    ' Bottom, Top: Invalid choices
            End Select
        End If
    End Function
    Public Function WrapLongText(ByVal Inp As String, ByVal MaxLen as integer, Optional ByVal NL As String = vbCrLf, Optional ByVal NavigateWordSplit As Boolean = False) As String
        '::::WrapLongText
        ':::SUMMARY
        ': Wraps long text
        ':::DESCRIPTION
        ': Wraps long text based on Maximum length
        ':::PARAMETERS
        ': - Inp
        ': - MaxLen
        ': - [nl] - What text to use as a line break (Default = vbCrLf)
        ': - [NavigateWordSplit]
        ':::RETURN
        ':  String - The wrapped output
        ':::SEE ALSO
        ':  WrapLongTextByPrintWidth
        On Error Resume Next
        Dim F As Object
        F = SplitLongText(Inp, MaxLen, NavigateWordSplit)
        WrapLongText = Join(F, NL)
    End Function
    Public Function ArrangeString(ByVal Source As String, ByVal Length as integer, Optional ByVal Alignment As ContentAlignment = ContentAlignment.MiddleLeft, Optional ByVal Truncate As Boolean = True) As String
        ArrangeString = AlignString(Source, Length, Alignment, Truncate)
    End Function
    Public Function SplitLongText(ByVal Inp As String, ByVal MaxLen as integer, Optional ByVal CareAboutWords As Boolean = True) As Object
        '::::SplitLongText
        ':::SUMMARY
        ': Splits a string based on MaxLength
        ':::DESCRIPTION
        ': Wraps long text based on Maximum length
        ':::PARAMETERS
        ': - Inp
        ': - MaxLen
        ': - [CareAboutWords] - What text to use as a line break (Default = vbCrLf)
        ':::RETURN
        ':  String()
        ':::SEE ALSO
        ':  WrapLongTextByPrintWidth, WrapLongText
        Dim Arr As Object, Out As Object, I as integer
        Arr = Split(Inp, vbCrLf)
        For I = LBound(Arr) To UBound(Arr)
            If Len(Arr(I)) > MaxLen Then
                Do While (Len(Arr(I)) > 0)
                    Dim LastSpace as integer
                    If CareAboutWords Then
                        LastSpace = LastWhiteSpace(CStr(Arr(I)), MaxLen)
                    Else
                        LastSpace = 0
                    End If
                    If LastSpace = 0 Then LastSpace = MaxLen
                    AddToArray(Out, Left(Arr(I), LastSpace))
                    Arr(I) = Mid(Arr(I), LastSpace + 1)
                Loop
            Else
                AddToArray(Out, Arr(I))
            End If
        Next
        SplitLongText = Out
    End Function
    Public Function LastWhiteSpace(ByVal Inp As String, Optional ByVal nPos as integer = 0) as integer
        '::::LastWhiteSpace
        ':::SUMMARY
        ': Returns position of last whitespace in a string
        ':::DESCRIPTION
        ': Locates and returns string position of last whitespace character in string.
        ':
        ': These include:
        ': - Space
        ': - TAB
        ': - Carriage Return
        ': - Line Feed
        ':::PARAMETERS
        ': - Inp
        ': - [Pos] - Starting postition
        ':::RETURN
        ':  String
        ':::SEE ALSO
        ':  WrapLongTextByPrintWidth, WrapLongText, SplitLongText
        ' Whitespace includes spaces, tabs, and cr/lf.
        Dim LS as integer, Lt as integer, LC as integer, LL as integer
        If nPos > 0 Then
            LS = InStrRev(Inp, " ", nPos)
            Lt = InStrRev(Inp, vbTab, nPos)
            LC = InStrRev(Inp, vbCr, nPos)
            LL = InStrRev(Inp, vbLf, nPos)
        Else
            LS = InStrRev(Inp, " ")
            Lt = InStrRev(Inp, vbTab)
            LC = InStrRev(Inp, vbCr)
            LL = InStrRev(Inp, vbLf)
        End If

        LastWhiteSpace = LS
        If Lt > LastWhiteSpace Then LastWhiteSpace = Lt
        If LC > LastWhiteSpace Then LastWhiteSpace = LC
        If LL > LastWhiteSpace Then LastWhiteSpace = LL
        If LastWhiteSpace < 0 Then LastWhiteSpace = 0
    End Function
    Public Function NLTrim(ByVal Str As String) As String
        '::::NLTrim
        ':::SUMMARY
        ': Trims any new lines (vbCr or vbLf) from start and end of string.
        ':::DESCRIPTION
        ': Trims any new lines (vbCr or vbLf) from start and end of string.
        ':::PARAMETERS
        ': - str
        ':::RETURN
        ':  String
        ':::SEE ALSO
        ':  WrapLongTextByPrintWidth, WrapLongText, SplitLongText
        Dim C As String
        Do While Len(Str) > 0
            C = Left(Str, 1)
            If C = " " Or C = vbTab Or C = vbCr Or C = vbLf Then Str = Mid(Str, 2) Else Exit Do
        Loop
        Do While Len(Str) > 0
            C = Right(Str, 1)
            If C = " " Or C = vbTab Or C = vbCr Or C = vbLf Then Str = Mid(Str, 1, Len(Str) - 1) Else Exit Do
        Loop
        NLTrim = Str
    End Function
    Public Function Slug(ByVal S As String, Optional ByVal MaxLen as integer = 0) As String
        '::::slug
        ':::SUMMARY
        ': Convert a string into a slug
        ':::DESCRIPTION
        ':  Returns a slug-ified version of the source string.
        ':  Slugs are useful in URLs and other places where special characters are not allowed.
        ':
        ':  The exact implementation isn't nearly as important as the simple fact that it's there, it's a slug, and it's usable.
        ':::PARAMETERS
        ': - S - Source string to be slug-ified.
        ': - [MaxLen] - To truncate the slug, if desired.  Leave blank (0) for no truncation.
        ':::EXAMPLE
        ': - slug("Something To be 'slugified'!!!") == "something-to-be-slugified"
        ':::RETURN
        ':  String - The slug generated from the source.
        ':::SEE ALSO
        ':  ArrangeString, StringNumerals, ReduceString
        Slug = ReduceString(S)
    End Function
    Public Function AugmentByRightLetter(ByVal CurrentArNo As String, Optional ByVal UpperCase As Boolean = True) As String
        '::::AugmentByRightLetter
        ':::SUMMARY
        ': Augment a string by either adding or incrementing its last letter.
        ':::DESCRIPTION
        ': Returns an augmented string that is useful for slowly increasing account numbers.
        ': Rather than simply adding another "A" ont he end of the string, resulting in 000-A, 000-AA, 000-AAA, this
        ': functon is designed to produce the intuitive 000, 000-A, 000-B, 000-Z, until it can't increase, and then, 000-ZA
        ':
        ':::PARAMETERS
        ': - S - Source string to be slug-ified.
        ': - [MaxLen] - To truncate the slug, if desired.  Leave blank (0) for no truncation.
        ':::EXAMPLE
        ': - slug("Something To be 'slugified'!!!") == "something-to-be-slugified"
        ':::RETURN
        ':  String - The slug generated from the source.
        ':::SEE ALSO
        ':  ArrangeString, StringNumerals
        Dim C As String
        C = Right(CurrentArNo, 1)
        If IsNumeric(C) Or UCase(C) >= "Z" Then
            AugmentByRightLetter = CurrentArNo & IIf(UpperCase, "A", "a")
        Else
            AugmentByRightLetter = Left(CurrentArNo, Len(CurrentArNo) - 1) & Chr(Asc(C) + 1)
        End If
    End Function
    Public Function ReduceString(ByVal Src As String, Optional ByVal Allowed As String = "", Optional ByVal Subst As String = "-", Optional ByVal MaxLen as integer = 0, Optional ByVal bLCase As Boolean = True) As String
        '::::ReduceString
        ':::SUMMARY
        ': Reduces a string by removing non-allowed characters, optionally replacing them with a substitute.
        ':::DESCRIPTION
        ': Non-allowed characters are removed, and, if supplied, replaced with a substitute.
        ': Substitutes are trimmed from either end, and all duplicated substitutes are remvoed.
        ':
        ': After this process, the string can be given LCase (default) or truncated (not default), if desired.
        ':
        ': This is effectively a slug maker, although it is somewhat adaptable to any cleaning routine.
        ':::PARAMETERS
        ': - Src - Source string to be reduced
        ': - [Allowed] - The list of allowable characters.  Defaults to [A-Za-z0-9]*
        ': - [Subst] - If specified, the character to replace non-allowed characters with (default == "-")
        ': - [MaxLen] - If passed, truncates longer strings to this length.  Default = 0
        ': - [bLCase] - Convert string to lower case after operation.  Default = True
        ':::EXAMPLE
        ': - ReduceString("   Something To be 'slugified'!!!****") == "something-to-be-slugified"
        ':::RETURN
        ':  String - The slug generated from the source.
        ':::AUTHOR
        ': Benjamin - 2018.04.28
        ':::SEE ALSO
        ':  ArrangeString, StringNumerals, slug, CleanANI
        Dim I as integer, N as integer, C As String
        If Allowed = "" Then Allowed = STR_CHR_UCASE & STR_CHR_LCASE & STR_CHR_DIGIT
        ReduceString = ""
        N = Len(Src)
        For I = 1 To N
            C = Mid(Src, I, 1)
            ReduceString = ReduceString & IIf(IsInStr(Allowed, C), C, Subst)
        Next

        If Subst <> "" Then
            Do While IsInStr(ReduceString, Subst & Subst) : ReduceString = Replace(ReduceString, Subst & Subst, Subst) : Loop
            Do While Left(ReduceString, Len(Subst)) = Subst : ReduceString = Mid(ReduceString, Len(Subst) + 1) : Loop
            Do While Right(ReduceString, Len(Subst)) = Subst : ReduceString = Left(ReduceString, Len(ReduceString) - Len(Subst)) : Loop
        End If

        If MaxLen > 0 Then ReduceString = Left(ReduceString, MaxLen)
        If bLCase Then ReduceString = LCase(ReduceString)
    End Function

End Module