     ************************************************************************
     **   A X A  ---   A L L   R I G H T S   R E S E R V E D               **
     ************************************************************************
     **    PROGRAM ID         : RWS025                                     **
     **    #VERSION           : v1.0                                       **
     **    FUNCTION(S)        : WS SFDC RLS                                **
     **    FREQUENCY          : On Request                                 **
     **    PROGRAMMER         : Rulli                                      **
     **    CREATION DATE      : 2025/03/06                                 **
     ************************************************************************
      *BEGIN - SEARCH "*BEGIN" SO AS TO SKIP ALL COMMENT BOX(ES)           **
     ************************************************************************
      /EJECT
     FLFPASSH   UF A E           K DISK
     FSUPMODE   IF   E           K DISK
     FLFPASST   O    E           K DISK    PREFIX(T_)
     FLFPASSD   O    E           K DISK    PREFIX(D_)
     F*RFPSFDCLOGO    E           K DISK
      *
      * Program Status Data Structure
     D PStatus        SDS
     D  PsUser               254    263
      *
     D                 DS
     DPRDATE           DS            60         DTAARA(DADATE)
     D TODAY                   9     16  0
     DCRTTIM           S               Z
     DDATENOW          S             14P 0
     DDATESTR          S             14A
     D@ASSEMNT         S           9999A   VARYING
      ***************************
      * M A I N   P R O G R A M *
      ***************************
     C     *ENTRY        PLIST
     C                   PARM                    @PNO             11
     C                   PARM                    @ENTITY           2
     C                   PARM                    @ASSEMNT
     C                   PARM                    @RTNCDE           2
      *
     C     @ENTITY       CHAIN     SUPMODE
     C                   IF        %FOUND
     C                   IF        MOMODE = 'D'
     C                   EVAL      @RTNCDE = MOMODE
     C                   ELSE
     C                   EXSR      SRWRITE
     C                   ENDIF
     C                   ELSE
     C                   EVAL      @RTNCDE = 'X'
     C                   ENDIF
     C                   EVAL      *INLR = *ON
      **************************************************************
     C     SRWRITE       BEGSR
      * Set initial value
     C                   IN        PRDATE
     C                   EVAL      @SECT = 'OTH'
     C                   EVAL      CRTTIM  = %TIMESTAMP()
     C                   EVAL      DATESTR = %CHAR(CRTTIM:*ISO0)
     C                   EVAL      DATENOW = %DEC(DATESTR:14:0)
      *
      * Assesment header
     C     KYASSH        KLIST
     C                   KFLD                    @PNO
     C                   KFLD                    @SECT             3
     C     KYASSH        CHAIN(n)  LFPASSH                            11
     C     *IN11         IFEQ      *OFF
     C                   CLEAR                   FMASSH
     C                   EVAL      ASPNO  = @PNO
     C                   EVAL      ASSECT = 'OTH'
     C                   EVAL      ASLUPD = DATENOW
     C                   UPDATE    FMASSH
     C                   ELSE
     C                   CLEAR                   FMASSH
     C                   EVAL      ASPNO    = @PNO
     C                   EVAL      ASSECT   = 'OTH'
     C                   EVAL      ASLUPD   = DATENOW
     C                   WRITE     FMASSH
     C                   ENDIF
      * Assesment header for transactions
     C                   CLEAR                   FMASST
     C                   EVAL      T_ASPNO  = @PNO
     C                   EVAL      T_ASSECT = 'OTH'
     C                   EVAL      T_ASREF  = 0
     C                   EVAL      T_ASCRTD = DATENOW
     C                   EVAL      T_ASUSID = PsUser
     C                   WRITE     FMASST
      * Assesment details
     C                   CLEAR                   FMASSD
     C                   EVAL      D_ASPNO  = @PNO
     C                   EVAL      D_ASSECT = 'OTH'
     C                   EVAL      D_ASREF  = 0
     C                   EVAL      D_ASCRTD = DATENOW
      * Set line number and subst detail
     C                   Z-ADD     *ZEROS        XCT               5 0          COUNTER
     C                   Z-ADD     *ZEROS        XLEN              5 0          LENGTH ASSESMENT FROM FE
     C                   Z-ADD     1             XLIN              5 0          LINE NUMBER
     C                   Z-ADD     1             XPOS              5 0          POSITION AFTER SUBST
     C                   EVAL      XLEN = %LEN(%TRIMR(@ASSEMNT))
     C                   IF        XLEN <> 75
     C                   EVAL      XCT  = XLEN / 75
     C                   EVAL      XCT  += 1
     C                   ELSE
     C                   EVAL      XCT  = XLEN / 75
     C                   EVAL      XCT  += 0
     C                   ENDIF
     C                   DOW       XCT  > 0
     C                   EVAL      D_ASLINO = XLIN
     C                   EVAL      D_ASDETL = %SUBST(
     C                                        @ASSEMNT:XPOS:75)
     C                   EVAL      XPOS = XPOS + 75
     C                   WRITE     FMASSD
     C                   EVAL      XLIN  += 1
     C                   EVAL      XCT   -= 1
     C                   ENDDO
     C                   ENDSR
