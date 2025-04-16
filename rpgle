     H DFTACTGRP(*NO) ACTGRP(*CALLER)

     D @TEXT         S             100A   INZ('halo dunia rpgle!')
     D @RESULT       S             100A
     D @I            S              3S 0
     D @CHAR         S              1A
     D @CAPNEXT      S               N   INZ(*ON)
     D @LOWER        C                   CONST('abcdefghijklmnopqrstuvwxyz')
     D @UPPER        C                   CONST('ABCDEFGHIJKLMNOPQRSTUVWXYZ')
     D @POS          S              3S 0

     C                   EVAL      @RESULT = ''
     C                   EVAL      @CAPNEXT = *ON

     C                   FOR       @I = 1 TO %LEN(%TRIMR(@TEXT))
     C                   EVAL      @CHAR = %SUBST(@TEXT:@I:1)

     C                   IF        @CAPNEXT = *ON AND
     C                             %SCAN(@CHAR:@LOWER) > 0
     C                   EVAL      @POS = %SCAN(@CHAR:@LOWER)
     C                   EVAL      @CHAR = %SUBST(@UPPER:@POS:1)
     C                   ENDIF

     C                   EVAL      @RESULT = %TRIMR(@RESULT) + @CHAR

     C                   IF        @CHAR = ' '
     C                   EVAL      @CAPNEXT = *ON
     C                   ELSE
     C                   EVAL      @CAPNEXT = *OFF
     C                   ENDIF

     C                   ENDFOR

     C                   DSPLY                   @RESULT
     C                   *INLR = *ON
