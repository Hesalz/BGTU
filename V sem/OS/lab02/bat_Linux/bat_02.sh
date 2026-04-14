#!/bin/bash
echo "Batch file name: $(basename $0)"
echo "Full path: $(realpath $0)"
echo "Last modified: $(stat -c %y $0)"
read -p "Press any key to continue..."