#!/bin/bash
#
# Script de criação dos ícons do aplicativo nos diversos formados suportados.
#
# Dependências:
# -   Comando "inkscape"
#     -   Aplicativo Inkscape
#     -   https://inkscape.org/pt-br/
# -   Comando "convert"
#     -   Aplicativo ImageMagick
#     -   https://www.imagemagick.org
# 

dist=../static/img/icons

mkdir -p ${dist}

# android
inkscape icons.svg -i color-icon -j -w  96 -h  96 -e ${dist}/android-chrome-96x96.png
inkscape icons.svg -i color-icon -j -w 192 -h 192 -e ${dist}/android-chrome-192x192.png
inkscape icons.svg -i color-icon -j -w 512 -h 512 -e ${dist}/android-chrome-512x512.png

# apple
inkscape icons.svg -i color-icon -j -w 180 -h 180 -e ${dist}/apple-touch-icon.png
inkscape icons.svg -i color-icon -j -w  76 -h  76 -e ${dist}/apple-touch-icon-76x76.png
inkscape icons.svg -i color-icon -j -w  60 -h  60 -e ${dist}/apple-touch-icon-60x60.png
inkscape icons.svg -i color-icon -j -w 180 -h 180 -e ${dist}/apple-touch-icon-180x180.png
inkscape icons.svg -i color-icon -j -w 152 -h 152 -e ${dist}/apple-touch-icon-152x152.png
inkscape icons.svg -i color-icon -j -w 120 -h 120 -e ${dist}/apple-touch-icon-120x120.png

# microsoft
inkscape icons.svg -i color-icon -j -w 144 -h 144 -e ${dist}/msapplication-icon-144x144.png
inkscape icons.svg -i color-icon -j -w 150 -h 150 -e ${dist}/mstile-150x150.png
 
# favicons
inkscape icons.svg -i color-icon -j -w  16 -h  16 -e ${dist}/favicon-16x16.png
inkscape icons.svg -i color-icon -j -w  32 -h  32 -e ${dist}/favicon-32x32.png
inkscape icons.svg -i color-icon -j -w  48 -h  48 -e ${dist}/favicon-48x48.png
convert ${dist}/favicon-48x48.png -define icon:auto-resize=48,32,16 ${dist}/favicon.ico

# safari
cp icons.svg ${dist}/safari-pinned-tab.svg
inkscape ${dist}/safari-pinned-tab.svg --select=black-icon --verb=EditInvert --verb=EditDelete --verb=FileSave --verb=FileQuit
