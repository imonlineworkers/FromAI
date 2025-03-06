ctl-opt dftactgrp(*no) actgrp(*new);

dcl-s @ASSEMNT char(9999);
dcl-s actualLength int(10);

dcl-pi *n;
  @ASSEMNT char(9999);
end-pi;

actualLength = 0;

for i = 9999 downto 1;
    if %subst(@ASSEMNT: i: 1) >= ' ' and %subst(@ASSEMNT: i: 1) <= 'z';
       actualLength = i;
       leave;
    endif;
endfor;

dsply ('Actual Length: ' + %char(actualLength));

*inlr = *on;
return;
