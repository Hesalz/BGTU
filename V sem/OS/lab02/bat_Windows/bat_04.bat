@echo off
set a=%1
set b=%2
set c=%3

set /a result1=a-b
set /a result2=a+b
set /a result3=c/b
set /a result4=c*b
set /a result5=(a-b)*(b-a)

echo a-b = %result1%
echo a+b = %result2%
echo c/b = %result3%
echo c*b = %result4%
echo (a-b)*(b-a) = %result5%
pause
