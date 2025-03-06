     F* No files, just testing @ASSEMNT

     D @ASSEMNT        S           9999A
     D actualLength    S              5I 0
     D i               S              5I 0
     D char            S              1A

     C     *ENTRY        PLIST
     C                   PARM                    @ASSEMNT          9999

     C                   EVAL      actualLength = 0

     C                   FOR       i = 9999 DOWNTO 1
     C                   EVAL      char = %SUBST(@ASSEMNT: i: 1)
     C                   IF        char >= ' ' AND char <= 'z'
     C                   EVAL      actualLength = i
     C                   LEAVE
     C                   ENDIF
     C                   ENDFOR

     C                   DSPLY     'Actual Length: ' + %CHAR(actualLength)

     C                   EVAL      *INLR = *ON
