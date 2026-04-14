#!/bin/bash
mode=$1
filename=$2

if [ -z "$mode" ]; then
    echo "Usage: bat_05.sh {create|delete} filename"
    exit 1
fi

if [ -z "$filename" ]; then
    echo "Error: filename not specified"
    exit 1
fi

if [ "$mode" = "create" ]; then
    if [ -e "$filename" ]; then
        echo "File '$filename' already exists."
    else
        touch "$filename"
        echo "File '$filename' created."
    fi
    exit 0
fi

if [ "$mode" = "delete" ]; then
    if [ -e "$filename" ]; then
        rm "$filename"
        echo "File '$filename' deleted."
    else
        echo "File '$filename' does not exist."
    fi
    exit 0
fi

echo "Unknown mode '$mode'. Use 'create' or 'delete'"
exit 1