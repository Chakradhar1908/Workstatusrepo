﻿Module modEmail
    Public Function SendSimpleEmail(ByVal From As String, ByVal FromName As String, ByVal T As String, ByVal TName As String, ByVal Subject As String, ByVal Body As String, Optional ByVal CC As String = "", Optional ByVal BCC As String = "", Optional ByVal Attachments As String = "") As String
        Dim Host As String

        ' First, we allow DEV MODE to skip all emails
        If IsDevelopment() Then
            If MsgBox("DEVELOPER: Skip email send?", vbYesNo, "DEVELOPER SKIP") = vbYes Then
                Dim TF As String
                SendSimpleEmail = "Email Send SKIPPED by developer"
                TF = TempFile(DevOutputFolder, Slug(Subject, 15) & "-", ".htm")
                WriteFile(TF, Body)

                LogFile("email", "SendSimpleEmail: """ & FromName & " <" & From & ">"": " & Subject)
                MsgBox("Email body written to: " & vbCrLf & TF, vbInformation, "DEVELOPER INFORMATION")
                Exit Function
            End If
        End If

        ' If they are using any email client, run this.
        If Val(GetEmailSetting("SOutlook")) = 0 Then
            'SendSimpleEmail = SendThroughOutlook(From, FromName, T, TName, Subject, Body, CC, BCC, Attachments) ERROR
            Exit Function
        End If

        ' If they have the new DLL, use it.
        If CBool(GetEmailSetting("uchilkat")) Then
            'SendSimpleEmail = SendMailSMTP_Chilkat(From, FromName, T, TName, Subject, Body, CC, BCC, Attachments) ERROR
            Exit Function
        End If

        ' Original standby...
        SendSimpleEmail = SendMailSMTP_VBAccelerator(From, FromName, T, TName, Subject, Body, CC, BCC, Attachments)
    End Function
    Public Function SendMailSMTP_VBAccelerator(ByVal From As String, ByVal FromName As String, ByVal T As String, ByVal TName As String, ByVal Subject As String, ByVal Body As String, Optional ByVal CC As String = "", Optional ByVal BCC As String = "", Optional ByVal Attachments As String = "") As String
        Dim F As frmSendMail
        F = New frmSendMail
        SendMailSMTP_VBAccelerator = F.DoSimpleSendMail(From, FromName, T, TName, Subject, Body, CC, BCC, Attachments)
        'Unload F
        F.Close()

        F = Nothing
    End Function

    '   --------> NOTE: TEMPORARILY COMMENTED. NEED TO FIND OUT Chilkat_v9_5_0          <------------
    '    Public Function SendMailSMTP_Chilkat(ByVal From As String, ByVal FromName As String, ByVal T As String, ByVal TName As String, ByVal Subject As String, ByVal Body As String, Optional ByVal CC As String = "", Optional ByVal BCC As String = "", Optional ByVal Attachments As String = "") As String
    '        Dim Success as integer
    '        Dim L As Object
    '        Dim MM As Chilkat_v9_5_0.ChilkatMailMan
    '        Dim Mail As Chilkat_v9_5_0.ChilkatEmail
    '        MM = New Chilkat_v9_5_0.ChilkatMailMan
    '        Mail = New Chilkat_v9_5_0.ChilkatEmail

    '        Success = MM.UnlockComponent(CHILKAT_LICENSE)
    '        If Success <> 1 Then
    '            DevErr("Chilkat Email Component Licensure failure.")
    '            SendMailSMTP_Chilkat = "Chilkat Email Component Licensure failure."
    '            Exit Function
    '        End If

    '        MM.SMTPHost = GetEmailSetting("smtphost")
    '        MM.SMTPPort = GetEmailSetting("smtpport")
    '        MM.SmtpUsername = GetEmailSetting("smtpuser")
    '        MM.SmtpPassword = GetEmailSetting("smtppass")
    '        '    .SmtpPassword = "mawdyweqinofhdke"

    '        Mail.FromAddress = From
    '        Mail.FromName = FromName
    '        Mail.AddTo TName, T
    '  Mail.Subject = Subject

    '        Mail.Body = Body

    '        If CC <> "" Then Mail.AddMultipleCC CC
    '  If BCC <> "" Then Mail.AddMultipleBcc BCC

    '  If Attachments <> "" Then
    '            For Each L In Split(Attachments, ",")
    '                Mail.AddFileAttachment PXfile(L)
    '    Next
    '        End If

    '        ProgressForm 0, 1, "Sending Email...", , , , prgIndefinite
    '  Success = MM.SendEmail(Mail)
    '        ProgressForm()
    '        If Success <> 1 Then SendMailSMTP_Chilkat = "Sending failed (" & MM.SmtpFailReason & ")."

    '        DisposeDA Mail
    'End Function

    Public Function GetEmailSetting(ByVal Key As String) As String
        Dim T As String
        T = EmailSettingAlt(Key)
        If T <> "" Then GetEmailSetting = T : Exit Function   ' allows homogenous interface over multiple data sources
        Select Case Key
            Case "uchilkat"
                '###SENDMAIL
                GetEmailSetting = False
                '      GetEmailSetting = FileExists(GetWindowsSystemDir() & DIR_SEP & "ChilkatAx-9.5.0-win32.dll")
            Case Else : GetEmailSetting = GetConfigTableValue(EmailSettingKey(Key), EmailSettingDef(Key))
        End Select
    End Function
    Public Function EmailSettingAlt(ByRef Key As String) As String
        EmailSettingKey(Key, , EmailSettingAlt)
    End Function
    Public Function EmailSettingDef(ByRef Key As String) As String
        EmailSettingKey(Key, EmailSettingDef)
    End Function

    Public Function EmailSettingKey(ByRef KeyIn As String, Optional ByRef EmailSettingDef As String = "", Optional ByRef EmailSettingAlt As String = "") As String
        Select Case LCase(KeyIn)
            Case "emailcfg" : EmailSettingKey = "EPO_EMailCFG" : EmailSettingDef = ""

            Case "fromname" : EmailSettingKey = "EPO_FromName" : EmailSettingAlt = StoreSettings.Name
            Case "fromaddr" : EmailSettingKey = "EPO_FromAddr" : EmailSettingAlt = StoreSettings.Email

            Case "smtphost" : EmailSettingKey = "EPO_SMTPHost" : EmailSettingDef = ""
            Case "smtpport" : EmailSettingKey = "EPO_SMTPPort" : EmailSettingDef = ""
            Case "smtpsecr" : EmailSettingKey = "EPO_SMTPSecr" : EmailSettingDef = "0"
            Case "smtpuser" : EmailSettingKey = "EPO_SMTPUser" : EmailSettingDef = ""
            Case "smtppass" : EmailSettingKey = "EPO_SMTPPass" : EmailSettingDef = ""
            Case "soutlook" : EmailSettingKey = "EPO_SOutlook" : EmailSettingDef = "0"
            Case "woutlook" : EmailSettingKey = "EPO_WOutlook" : EmailSettingDef = "3"
            Case "username" : EmailSettingKey = "EPO_OutlkUsr" : EmailSettingDef = ""
            Case "password" : EmailSettingKey = "EPO_OutlkPwd" : EmailSettingDef = ""
            Case "uchilkat" : EmailSettingKey = "EPO_UChilKat" : EmailSettingDef = ""
            Case Else : DevErr("Program error: Invalid Email Setting [" & KeyIn & "]", vbCritical, "Program Error")
        End Select
    End Function

    '    Private Function SendThroughOutlook(ByVal From As String, ByVal FromName As String, ByVal T As String, ByVal TName As String, ByVal Subject As String, ByVal Body As String, Optional ByVal CC As String = "", Optional ByVal BCC As String = "", Optional ByVal Attachments As Object = Nothing) As String
    '        Dim olOutlookApp As Object, olEMail As Object
    '        Dim NeedQuit As Boolean
    '        Dim TT as integer
    '        Dim L As Object, A As Object, B As Object, N as integer
    '        Dim CID As Object, tCID As String, img As String, ImgID As String, ImgNo as integer, FS As Object
    '        Dim mCID As Object '@NO-LINT-NTYP
    '        Dim Res As String
    '        TT = GetEmailSetting("WOutlook") ' Which Outlook?
    '        If TT = 0 Then
    '            MsgBox("No outlook program is specified in setup.  Please either select Outlook or Outlook Express or both if you wish to send through MS Outlook", vbExclamation, "No Program Specified")
    '            Exit Function
    '        End If

    '        On Error Resume Next
    '        If (TT And 1) <> 0 Then
    '            olOutlookApp = GetObject(, "Outlook.Application")
    '            If Err() <> 0 Then    '   Outlook not running?
    '                olOutlookApp = CreateObject("Outlook.Application")
    '                NeedQuit = True
    '            End If
    '        End If

    '        If olOutlookApp Is Nothing Then  ' try outlook express..?
    '            SendThroughOutlook = "MS Outlook not available."
    '            If (TT And 2) <> 0 Then
    '                ' we have 2 ways to try to send through MAPI....  one uses MSMAPI.MAPISession, the other MAPI.Session......  Hopefully one will work!!
    '                SendThroughOutlook = SendThroughMAPI(From, FromName, T, TName, Subject, Body, CC, BCC, Attachments)
    '                If SendThroughOutlook <> "" Then ActiveLog "frmEmail::SendThroughMAPI Failure: " & SendThroughOutlook, 3

    '      If SendThroughOutlook <> "" Then SendThroughOutlook = SendThroughMAPI2(From, FromName, T, TName, Subject, Body, CC, BCC, Attachments)
    '                If SendThroughOutlook <> "" Then ActiveLog "frmEmail::SendThroughMAPI2 Failure: " & SendThroughOutlook, 3
    '      If SendThroughOutlook = "" Then
    '                    Log("Email Sent Through MAPI: " & TName)
    '                    '        If Mode = emSimple Then
    '                    '          RaiseEvent SimpleEmailDone(True, "Email Sent.")
    '                    '        Else
    '                    '          MsgBox "Message sent.", vbInformation, "Email Sent"
    '                    '        End If
    '                Else
    '                    If (TT And 1) Then GoTo ViaOutlook
    '                    Log("Email FAILED to Send Through MAPI: " & TName)
    '                    SendThroughOutlook = "Could not send mail through Outlook Express: " & SendThroughOutlook
    '                End If
    '            End If
    '            Exit Function
    '        End If

    'ViaOutlook:
    '        '   Create E-mail
    '        olEMail = olOutlookApp.CreateItem(0) ' (olMailItem)
    '        '    olEMail.Recipients.Add T
    '        olEMail.To = T
    '        olEMail.Subject = Subject
    '        '    olEMail.BodyFormat = 2 ' olFormatHTML
    '        Dim BP As String
    '        If IsFormLoaded("MailBookEmail") Then BP = GetFilePath(MailBookEmail.txtBodyFile)
    '        Body = replaceHTMLimages(Body, BP, tCID, img)
    '        CID = tCID ' for the attachment to work, the CID has to be untyped.  Byref, above, needs typed
    '        '    olEMail.Body = "HTML Message Enclosed"
    '        olEMail.HTMLBody = Body
    '        If CC <> "" Then olEMail.CC = CC
    '        If BCC <> "" Then olEMail.BCC = BCC

    '        If img <> "" Then
    '            ImgNo = 0
    '            '      ImgID = olEMail.EntryID
    '            FS = Split(CID, ";")
    '            For Each L In Split(img, ";")
    '                Dim Att As Object, oPA As Object
    '                ImgNo = ImgNo + 1
    '                Att = olEMail.Attachments.Add(L)
    '                oPA = Att.PropertyAccessor
    '                oPA.SetProperty PR_ATTACH_MIME_TAG, HTTPImageType(L)
    '      mCID = FS(ImgNo - 1) ' This variable must be un-typed for the attachment
    '                oPA.SetProperty PR_ATTACH_CONTENT_ID, mCID 'change myident for another other image
    '            Next
    '        End If

    '        If Attachments <> "" Then
    '            For Each L In Split(Attachments, ";")
    '                olEMail.Attachments.Add L
    '    Next
    '        End If

    '        olEMail.Send
    '        Log("Email Sent through Outlook: " & TName)
    '        '    If Mode = emSimple Then
    '        '      RaiseEvent SimpleEmailDone(True, "Email Sent.")
    '        '    Else
    '        '      MsgBox "Message sent.", vbInformation, "Email Sent"
    '        '    End If


    '        If NeedQuit Then olOutlookApp.Quit
    '        olOutlookApp = Nothing
    '    End Function
    Public Function replaceHTMLimages(ByVal Doc As String, Optional ByVal BasePath As String = "", Optional ByRef C As String = "", Optional ByRef I As String = "") As String
        Dim X as integer, Y as integer, R As String, S As String, QC As String, CID As String
        Dim Src As String, F As String, Src1 As String
        If BasePath = "" Then BasePath = PXFolder()

        X = 1
        Do While True
            X = InStr(X, Doc, "<img ", vbTextCompare)
            If X = 0 Then Exit Do
            'Y = InStr(X, Doc, ">")   ERROR
            R = Mid(Doc, X, Y - X + 1)

            'Src1 = GetHTMLTagArgument(R, "src", QC)   ERROR
            Src = Replace(Src1, "/", "\\")
            F = CleanPath(Src, BasePath)
            If FileExists(F) Then
                'CID = Right(CreateUniqueID("{-}"), 15)   ERROR
                C = C & IIf(Len(C) > 0, ";", "") & CID
                I = I & IIf(Len(I) > 0, ";", "") & F
                S = Replace(R, QC & Src1 & QC, """" & "cid:" & CID & """")
                '      S = Replace(R, QC & Src & QC, """" & ImageToDataURI(F) & """")  ' could have worked, but IE7 disallows inline data
                Doc = Replace(Doc, R, S, , , vbTextCompare)
            End If

            X = X + Len(R)
        Loop
        replaceHTMLimages = Doc
    End Function

End Module
