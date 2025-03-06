     D @ASSEMNT        S           9999A
     D actualLength    S              5I 0
     D i               S              5I 0
     D char            S              1A

     C     *ENTRY        PLIST
     C                   PARM                    @PNO             11
     C                   PARM                    @ENTITY           2
     C                   PARM                    @ASSEMNT          9999
     C                   PARM                    @RTNCDE           2

      /FREE
        // Cek panjang real @ASSEMNT (dari belakang ke depan)
        actualLength = 0;
        FOR i = 9999 DOWNTO 1;
          char = %SUBST(@ASSEMNT: i: 1);
          IF char >= ' ' AND char <= 'z'; // Cek karakter printable
            actualLength = i;
            LEAVE;
          ENDIF;
        ENDFOR;

        // Display panjang asli
        DSPLY ('Actual Length: ' + %CHAR(actualLength));
        DSPLY ('Content: ' + %SUBST(@ASSEMNT: 1: actualLength));

        *INLR = *ON;
      /END-FREE
