Projeto de Ícones
=================

Projeto de exportação de ícones para os diversos formatos suportados.

Visão Geral
-----------

O projeto exporta os gráficos do arquivo "icons.svg" em diversos
formatos para suporte à diferente plataformas, como smartphones Android,
iPhones, iPads, Browsers, etc.

Os gráficos são exportados para a pasta de imagens do projeto Javascript:

    ..\static\img\icons

A geração dos gráficos deve seguir duas etapas:

1.  Comece editando o arquivo "icons.svg"

    -   De preferência pelo uso do aplicativo
        [Inkscape](https://inkscape.org/pt-br/), disponível para todos
        os sistemas operacionais.
        
    -   Siga as instruções descritas no arquivo para produzir duas
        versões da logo do aplicativo: Uma colorida e outra preto e
        branca.
    
2.  Em seguida execute o script de exportação de acordo com o sistema
    operacional:
    
    -   export-icons.sh, para Linux
    -   export-icons.bat, para Windows
    
Pré-Requisitos
--------------

Os scripts de exportação precisam de duas ferramentas disponíveis na
linha de comando:

1.  "inkscape"

    -   Obtido do Inkscape
    -   https://inkscape.org/pt-br/
    
2.  "convert"

    -   Obtido do ImageMagick
    -   https://www.imagemagick.org

Os comandos precisam estar disponíveis na variável de ambiente %PATH%.
Geralmente são estas as pastas que se acrescentao ao %PATH%:

    C:\Program Files\Inkscape
    C:\Program Files\ImageMagick

Mas estes nomes podem variar de acordo com as escolhas durante a instalação.
   
---
Jun/2018  
Guga Coder
