Public Class Utils
    Const sBASE_64_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
    ' --------------------------------------
    ' ---------------------------------------
    Shared Function Base64encode(ByVal asContents As String) As String
        Dim lnPosition
        Dim lsResult
        Dim Char1
        Dim Char2
        Dim Char3
        Dim Char4
        Dim Byte1
        Dim Byte2
        Dim Byte3
        Dim SaveBits1
        Dim SaveBits2
        Dim lsGroupBinary
        Dim lsGroup64

        If Len(asContents) Mod 3 > 0 Then asContents = asContents & New String(" ", 3 - (Len(asContents) Mod 3))
        lsResult = ""

        For lnPosition = 1 To Len(asContents) Step 3
            lsGroup64 = ""
            lsGroupBinary = Mid(asContents, lnPosition, 3)

            Byte1 = Asc(Mid(lsGroupBinary, 1, 1)) : SaveBits1 = Byte1 And 3
            Byte2 = Asc(Mid(lsGroupBinary, 2, 1)) : SaveBits2 = Byte2 And 15
            Byte3 = Asc(Mid(lsGroupBinary, 3, 1))

            Char1 = Mid(sBASE_64_CHARACTERS, ((Byte1 And 252) \ 4) + 1, 1)
            Char2 = Mid(sBASE_64_CHARACTERS, (((Byte2 And 240) \ 16) Or (SaveBits1 * 16) And &HFF) + 1, 1)
            Char3 = Mid(sBASE_64_CHARACTERS, (((Byte3 And 192) \ 64) Or (SaveBits2 * 4) And &HFF) + 1, 1)
            Char4 = Mid(sBASE_64_CHARACTERS, (Byte3 And 63) + 1, 1)
            lsGroup64 = Char1 & Char2 & Char3 & Char4

            lsResult = lsResult + lsGroup64
        Next

        Base64encode = lsResult
    End Function
End Class
