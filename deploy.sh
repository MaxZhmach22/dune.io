#!/bin/bash

echo Unity3D build start!
Unity -quit -batchmode -nographics -executeMethod BuildScript.Build -projectPath .
echo Unity3D build complete!

echo dune_io start copy!
cd Builds
cp -rv dune_io E:/Work/WebStorm/profile-portal/src/assets
echo dune_io is copied! Bye!

# -t 5: Timeout of 3 seconds
read -s -n 1 -t 3