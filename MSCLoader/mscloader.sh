#!/bin/bash
cd /mnt/d/programowanie_unity/MSCMody/MSCModLoader/MSCLoader/MSCLoader/Properties/
build="$(git rev-list HEAD --count)"
ver="0.4.4."
num=$(($build + 1))
echo $num
sed -i 's/'"$ver$build"'/'"$ver$num"'/g' /mnt/d/programowanie_unity/MSCMody/MSCModLoader/MSCLoader/MSCLoader/Properties/AssemblyInfo.cs
