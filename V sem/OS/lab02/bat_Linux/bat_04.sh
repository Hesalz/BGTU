#!/bin/bash
a=$1
b=$2
c=$3

result1=$((a - b))
result2=$((a + b))
result3=$((c / b))
result4=$((c * b))
result5=$(( (a - b) * (b - a) ))

echo "a-b = $result1"
echo "a+b = $result2"
echo "c/b = $result3"
echo "c*b = $result4"
echo "(a-b)*(b-a) = $result5"
read -p "Press any key to continue..."
