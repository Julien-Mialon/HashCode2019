#!/bin/bash

find . -name "bin" -exec rm -rf {} \;
find . -name "obj" -exec rm -rf {} \;

rm hashcode.zip
zip -r -X hashcode.zip HashCode