# Template de Texto

Linguagem de template para formatação de texto com elementos extraídos
de um XML.

## Visão Geral

O template é uma linguagem simplificada para extrair, transformar e 
formatar dados extraídos de um XML para produzir um texto de saída,
como o caminho de um arquivo exportado, por exemplo.

O template é um texto simples mesclado com instruções de transformação
de dados.

Uma transformação de dados é uma série de comandos em linha.
Durante o processamento da transformação o resultado de um comando é
passado para o comando seguinte, formando assim, um processamento em
linha.

Um comando é composto de um ou mais blocos de código.

**Notação básica:**

-   A transformação é denotada por chaves (`{...}`).
-   A sequência de comandos da instrução é separada por barra vertical (`|`).
-   A sequência de blocos do comando é separada por ponto-e-vírgula (`;`).
-   Os caracteres `$`, `%`, `{`, `}` e `:` têm significado na
    estruturação da linguagem e por isso são reservados.

**Exemplo do Template:**

A sintaxe da linguagem é detalhada nas próximas seções, mas por
ora, para apresentar a linguagem, considere este template:

    D:\Xmls\{$..emit.CNPJ}\{$..dhEmi;$..dhEvento|%d|{yyyy-MM}}\{$..@Id|%s;[^0-9];}-{$..NFe|?;procNFe;cancNFe}.xml

O que este template faz?

1.  Imprime `D:\Xmls\`
2.  Destaca a tag `emit/CNPJ` e imprime
3.  Imprime `\`
4.  Destaca a tag `dhEmi` se houver, senão, destaca a tag `dhEvento`.
    1.  Converte o texto para data.
    2.  Formata a data como mês e ano.
    3.  Imprime o resultado
5.  Imprime `\`
6.  Destaca o atributo `Id`
    1.  Aplica uma substituição com Regex, substituindo caracteres não
        numéricos por nada.
    2.  Imprime o resultado
7.  Imprime `-`
8.  Destaca a tag `NFe`
    1.  Se a tag existir:
        1.  Então, imprime `procNFe`
        2.  Senão, imprime `cancNFe`
9.  Imprime `.xml`

O resultado é algo como:

    D:\Xmls\51224514000140\2017-06\31070251224514000140560010000245610000125419-procNFe.xml

## 1. Sintaxe

Template
:   Template mesclando textos e pipelines de transformação.

        "A chave da nota {$..NFe..chNFe} é válida"
         ^^^^^^^^^^^^^^^ ^^^^^^^^^^^^^^^ ^^^^^^^^
              texto          pipeline      texto
    
Pipeline
:   Uma sequência de comandos separados por (`|`).\
    Durante o processamento o resultado de um comando é repassado para
    o comando seguinte, criando o processamento em linha.
    
        $..vNF|%f|{#,##.00}
        ^^^^^^ ^^ ^^^^^^^^^
             comandos

Comando
:   Um código que desempenha uma função de extração, transformação ou
    formatação de dados.
    
           # acesso a propriedade:
           $ctx.user.name

           # conversao para data/hora:
           %d

           # substitui "a" por "A":
           %s;a;A

           # extrai do segundo ao nono caractere:
           [2;9]

           # formata uma data
           {dd/MM/yyyy}
    
Argumentos
:   Parâmetros do comando separados por ponto-e-vírgula (`;`):
    
           %s;NFe;55
           ^^ ^^^ ^^
Template Aninhado
:   Um template inserido dentro de um outro template delimitado por parêntesis `(...)`:
    
        %r;NFe;($..mod|%d|{00})
                ^^^^^^^^^^^^^^
               pipeline interno


## 2. Comandos de Extração de Dados

Comandos que extraem dados do contexto de execução do template e
repassam para o comando seguinte.

### 2.1. Tag de XML

Função
:   Extrair tag ou atributo de um XML.

Sintaxe
:       $TERMO

A sintaxe de escrita segue a forma [TagPath] e deve ser iniciado por um
dólar (`$`).

    $enviNFe.NFe.infNFe.ide.nNF

TagPath é usado na detecção de tag por parentesco. Todas as formas de
TagPath são suportadas, mas a forma mais comum é a detecção de tag
independentemente do parentesco:

    $..nNF
    
Em TagPath atributo é referenciado por arroba (`@`):

    $..@Id

Leia mais:

-   [TagPath]

### 2.2. Property

Função
:   Extrair propriedade de objeto anexado ao contexto de execução.

Sintaxe
:       $ctx.TERMO

O método de processamento do template pode oferecer diversas variáveis e
objetos adicionais relacionados com o contexto de execução.
Estes recursos são acessados pelo comando de `$ctx`, na forma simples de
propriedades.

    $ctx.aplicativo.titulo

Na forma simples, as propriedades são expressas em
[camelCase][Convenção de Nome] e separadas por ponto (`.`).
Quando uma propriedade é nula a varredura de profundidade pára e nulo é
retornado para o algoritmo.

As propriedades oferecidas pelo `$ctx` dependem do aplicativo que
utiliza o template.
Por padrão não há propriedades declaradas.


## 3. Comandos de Transformação de Dados

Comandos que aplicam regras de transformação no dado recebido e
repassam o resultado para o comando seguinte.


### 3.1. Coalesce

Função
:   Retornar o primeiro valor não-nulo e não-vazio de uma sequência.

Sintaxe
:       TERMO;TERMO;...

O comando resolve a lista de termos separados por ponto-e-vírgula da
esquerda para a direita e retorna o primeiro item não-nulo e não-vazio,
repassando o resultado para o comando seguinte.

Neste exemplo, o comando extrai a primeira tag CNPJ ou CPF que ele
encontrar no XML:

    $..emit.CNPJ;..emit.CPF

Coalesce e Array são intrinsecamente relacionados.\
O mesmo algoritmo implementa os dois comandos.\
Leia mais na seção [3.1. Coalesce e Array]

### 3.2. Array

Função
:   Repassar um conjunto de termos para o comando seguinte.

Sintaxe
:       TERMO;TERMO;...

Os termos são repassados para o comando seguinte em um Array de índice
iniciado em 1.

Os termos deve ser explicitamente referenciados pelo índice entre
chaves `{I}`.

Exemplo:

    $..nNF;$..serie;$..vNF|O valor da nota de número {1}, série {2} é: {3}

Coalesce e Array são intrinsecamente relacionados.\
O mesmo algoritmo implementa os dois comandos.\
Leia mais na seção [3.1. Coalesce e Array]

### 3.3. Format

Função
:   Aplicar uma formatação de valor.

Sintaxe
:       {INDICE,ALINHAMENTO:FORMATO}

O comando aplica uma formatação de texto de acordo com o padrão
suportado pelo método [String.Format() do C#].

Em resumo:

ÍNDICE
:   Índice do termo do Array a ser formatado, iniciando em 1.

    Exemplo:
    
        $..nNF;$..serie|Nota fiscal número {1}, série {2}

    Os índices a partir de "1" se referem a termos do Array, enquanto o
    índice "0" refere-se à entrada padrão do comando, e pode ser omitida
    se desejado.
    
    Os dois exemplos são equivalentes:
    
        $..nNF|Nota fiscal número {}
        $..nNF|Nota fiscal número {0}
        
    Desta forma, para obter o valor do comando [Coalesce] aplicado sobre
    o [Array] e ainda acessar os termos do Array basta usar o índice "0"
    para o resultado do coalesce e índices a partir de "1" para acessar
    os termos do Array. Leia mais na seção [3.1. Coalesce e Array].
    
    Exemplo:

        ;10;20|Dos termos '{1}', '{2}' e '{3}' o termo '{0}' foi escolhido.
        
        Imprime
          "Dos termos '', '10' e '20' o termo '10' foi escolhido."

ALINHAMENTO
:   Alinhamento do texto quando construindo texto de tamanho fixo.

    Use um número positivo para alinhar à direita e negativo para
    alinhar à esquerda.
    
    Exemplos:
    
        NFe;55|{1,-5}{2,5}
        
        Imprime
          "NFe     55"

FORMATO
:   Aplica uma formatação personalizada para o tipo do argumento.

    O valor a ser formatado deve corresponder ao tipo desejado.
    Por exemplo, para formatar um valor como número decimal é necessário
    convertê-lo para decimal.
    
    Apenas os tipos número e data/hora são suportados e os [comandos de
    conversão] de tipo podem ser usados:
    
    Tipo        Conversão     Exemplo
    ----------  ------------  --------------------------
    Inteiro     %i            `$..nNF|%i|{00000000}`
    Flutuante   %f            `$..vICMS|%f|{#,##0.00}`
    Data/Hora   %d            `$..dhEmi|%d|{dd/MM/yyyy}`
    
    O template suporta os padrões do C# para:
    
    -   [Formatação de números]
    -   [Formatação de data/hora]
    
    Sendo os mais comuns:
    
    Casas decimais
    :       1500|%f|{#,#00.00}
            
            Imprime
              "1.500,00"
            
    Zeros à esquerda
    :       1500|%i|{00000000}

            Imprime
              "00001500"
                
    Data/Hora
    :   Com formatos personalizados para datas, horas e time-zones.
        
        Formato Significado
        ------- --------------------------------------------------------
        `d`     Dia com uma casa.
        `dd`    Dia com duas casas.
        `M`     Mês com uma casa.
        `MM`    Mês com duas casas.
        `yy`    Ano com duas casas.
        `yyyy`  Ano com três casas.
        `h`     Hora com uma casa.
        `hh`    Hora com duas casas.
        `m`     Minuto com uma casa.
        `mm`    Minuto com duas casas.
        `s`     Segundo com uma casa.
        `ss`    Segundo com duas casas.
        `zzz`   Time-zone, no formato `+0:00` e `-0:00`.
        
        Exemplo:
        
            $..dhEmi|{yyyy-MM-ddThh:mm:sszzz}
            
            Imprime:
              "2017-10-03T19:47:42-3:00"

**Exemplo Geral**

Exemplo de formatação composta dos diversos elementos apresentados:

    $..nNF;$..dhEmi|$;%d|Nota fiscal {1} emitida em: {2:dd/MM/yy}
    
    Imprime
      "Nota fiscal 1234 emitida em: 03/10/17"

Explicação:

1.  O número e a data de emissão da nota são destacados do XML.
2.  Usando [Expansão de Array] o primeiro termo é repassado como está e
    o segundo termo é convertido para data.
3.  Os dois termos são aplicados na formatação.


### 3.4. Replace

Função
:   Aplicar uma substituição de texto

Sintaxe
:       %r;TERMO;TERMO

O comando procura por um termo no texto recebido do comando anterior e
aplica uma substituição.

    Lorem ipsum|%r;ipsum;ipsum dolore
    
    Imprime
      "Lorem ipsum dolore"


### 3.5. Search (Regex)

Função
:   Aplicar uma substituição de texto por expressão regular.

Sintaxe
:       %s;REGEX
        %s;REGEX;TERMO

O comando procura pela ocorrência de uma [expressão regular] no texto
recebido do comando anterior e opcionalmente aplica uma substituição.

**Formato de Pequisa**

No formato de pesquisa o comando deve ter duas partes:
O comando `%s` e a expressão regular.

O comando aplica a expressão regular no texto recebido do comando
anterior e repassa para o próximo comando ou o texto capturado ou vazio.

    Lorem ipsum dolore|%s;sum.*lo|Texto: '{}'
    
    Imprime
      Texto: 'sum dolo'

    Lorem ipsum|%s;sum.*lo|Texto: '{}'
    
    Imprime
      Texto: ''
      
O resultado do comando pode ser usado em conjunto com os comandos de
controle de fluxo, como o operador [Ternary]:

    Lorem ipsum|%s;dolore|?;Encontrado;Não encontrado
    
    Imprime
      "Não encontrado"
      
**Formato de Substituição**

No formato de substituição o comando deve ter três partes:
O comando `%s`, a expressão regular e o termo substituto.

O comando aplica uma substituição no texto capturado com a expressão
regular e repassa o texto inteiro modificado para o próximo comando.
Se a expressão regular não capturar qualquer texto o texto inteiro
recebido é passado a diante.

A substituição pode referenciar grupos capturados pela expressão regular
e partes do texto original:

Parte   Significado
------  --------------------------------------------------------
$N      O texto de grupo capturado identificado pelo seu número.
$+      O texto do último grupo capturado.
$&      O texto capturado pelo regex.
$`      O texto antes do regex capturado.
$'      O texto depois do regex capturado.
$_      O texto inteiro, exatamente como recebido.
$$      Escape para imprimir o caractere `$`.

Em [expressão regular], grupos são denotados por parêntesis `(...)`.

    Nota fiscal 451, série 5|%s;([0-9]+).+([0-9]+);$`$2/$1
                ^^^^^^^^^^^^
               regex capturado
    
    Imprime
      "Nota fiscal 5/451"
      

### 3.6. Mid

Função
:   Destacar partes de um texto.

Sintaxe
:       %[N;M]

O comando recebe dois argumentos, o índice inicial inclusivo e o índice
final inclusivo, e destaca do texto os caracteres no intervalo.
Os índices se iniciam em 1.

    Lorem ipsum|%[3;7]
    ^^^^^^^^^^^
    12345678...
      índices
    
    Imprime
      "rem i"
      
Se o primeiro índice for negativo a consulta é feita de traz pra frente:

    Lorem ipsum|%[-3;7]
    ^^^^^^^^^^^
    ...87654321
      índices
    
    Imprime
      "m ips"

Se o segundo parâmetro for omitido o intervalo destacado segue até o
fim da sequência:

    Lorem ipsum|%[3]
    ^^^^^^^^^^^
    12345678...
      índices
    
    Imprime
      "rem ipsum"

    Lorem ipsum|%[-3]
    ^^^^^^^^^^^
    ...87654321
      índices
    
    Imprime
      "Lorem ips"

Se o intervalo é maior que o texto o excesso é apenas desconsiderado:

    Lorem ipsum|%[3;100]
    ^^^^^^^^^^^
    12345678...
      índices
    
    Imprime
      "rem ipsum"
    
    
### 3.7. Casting

Função
:   Converter o valor para um tipo específico.

Sintaxe
:       %X
        Sendo X o identificado do tipo

O comando aplica uma conversão de tipo.\
Os tipos suportados são:

Tipo    Significado
------  -------------------------------------
`%f`    Flutuante. Número com casas decimais.
`%i`    Inteiro. Número sem casas decimais.
`%b`    Booliano. Verdadeiro ou falso.
`%d`    Data/Hora.

A conversão é requerida principalmente pelo comando [Format] para
formatar números e datas.

    $..vICMS|%f|O valor do ICMS é: {#,##0.00}

    Imprime
      O valor do ICMS é: 1.250,00"

**Conversão para Booliano**

Texto
:   Texto vazio resolve para falso.\
    Qualquer outro texto resolve para verdadeiro.

Numero
:   Zero resolve para falso.\
    Qualquer outro número resolve para verdadeiro.\
    O literal "0" é um texto e deve ser convertido para inteiro antes,
    se este for o caso.
    
        0|%b
        1|%b
        0|%i|%b
        1|%i|%b
        
        Imprime
          "true"
          "true"
          "false"
          "true"


Outros
:   Tag de XML inexistente resolve para falso.\
    Propriedade inexistente resolve para falso.\
    Propriedade nula resolve para falso.\
    Texto nulo resolve para falso.\
    Qualquer outro valor resolve para verdadeiro.
    
        $tag.invalida|%b
        $ctx.propriedade.invalida|%b
        
        Imprime
          "false"
          "false"

**Conversão para Data**

O algoritmo de conversão detecta as partes da data/hora pelos padrões
conhecidos. O texto pode conter apenas a data, apenas a hora ou as duas
partes, separadas por espaço ou pela letra T.

Formatos suportados:

-   Data
    -   `dd-MM-yyyy`
    -   `yyyy-MM-dd`
-   Hora
    -   `hh:mm`
    -   `hh:mm:ss`
    -   `hh:mm:ss.nnn`
-   Data-Hora
    -   `dd-MM-yyyy hh:mm`
    -   `dd-MM-yyyy hh:mm:ss`
    -   `dd-MM-yyyy hh:mm:ss.nnn`
    -   `yyyy-MM-dd hh:mm`
    -   `yyyy-MM-dd hh:mm:ss`
    -   `yyyy-MM-dd hh:mm:ss.nnn`
-   Data de XML
    -   `yyyy-MM-ddThh:mm:ss`
    -   `yyyy-MM-ddThh:mm:ss-z:zz`

Notas:

-   "`nnn`" corresponde aos milissegundos.
-   "`z:zz`" corresponde ao timezone, como em "`-3:00`"
-   Dia e mês podem ou não conter zeros à esquerda.
    -   Porém, quando o ano aparece no inicio da data deve ter obrigatoriamente 4 casas.
-   Ano pode ter duas ou quatro casas.
-   O separador de data pode ser: "`/`" ou "`-`"

Exemplo:

    2017-05-01|%d|{dd/MM/yyyy hh:mm:ss}
    
    Imprime
      "01/05/2017 00:00:00"
      


## 4. Comandos de Controle de Fluxo

Comandos que manipulam a sequência de execução dos comandos no
pipeline.

### 4.1. Ternary

Função
:   Executar um comando ou outro de acordo com o valor recebido.

Sintaxe
:       ?;COMANDO;COMANDO

O comando converte para booliano o valor recebido do comando anterior e
decide qual dos dois comandos seguintes deve ser executado.

Caso verdadeiro o primeiro comando subsequente é executado e o seguinte
ignorado.

Caso falso o primeiro comando subsequente é ignorado e o segundo
executado.

O comando repassa para o comando subsequente que será executado o mesmo
valor exatamente como recebido por ele do comando anterior.

    $..NFe|?;$..chNFe;Não existe|Chave da nota: {}
             ^^^^^^^^ ^^^^^^^^^^
               caso      caso
              verdade    falso
              
    Imprime
      "Chave da nota: 33170619406654000148650330000121881000121887"
      ou
      "Chave da nota: Não existe"

    
### 4.2. Comparison

Função
:   Executar um comando ou outro de acordo com o resultado de uma
    comparação.

Sintaxe
:       OPERADOR;TERMO;COMANDO;COMANDO

O comando aplica um operador de comparação entre o valor recebido do
comando anterior e o termo indicado em seguida e decide qual comando
subsequente deve ser executado.

Se o termo for um Array a comparação é feita contra todos os argumentos.\
Se o termo for texto as relações são feitas pela ordem alfabética.

Operador  Contra um valor         Contra um Array
--------  ----------------------  ------------------------------------------
`%eq`     Igual a                 Igual a qualquer
`%ne`     Diferente de            Diferente de todos
`%gt`     Maior que               Maior que todos
`%ge`     Maior ou igual a        Maior que todos ou igual ao maior
`%lt`     Menor que               Menor que todos
`%le`     Menor ou igual a        Menor que todos ou igual ao menor
`%is`     Parecido com            Parecido com qualquer
`%in`     Igual a                 Esta dentro do limite

**Operador "Parecido com" (%is)**

O comando se comporta como o "LIKE" do SQL, comparando
trechos do texto com o curinga `*`, como em:

    Lorem ipsum|%is;*rem*;Sim;Nao
    
    Imprime
      "Sim"
      
**Operador "Esta dentro" (%in)**

O comando espera como termo um Array de dois itens, contendo o menor e
o maior índice. O comando então verifica que o valor está dentro do
intervalo.

    10|%i|%in;(5;15);Sim;Nao
    
    Imprime
      "Sim"

Note que para determinar se um valor esta dentro de um Array o operador
"Igual a" (%eq) deve ser usado. O comando "Esta dentro" (%in) apenas
determina se o valor está dentro do intervalo indicado pelos índices.

    10|%i|%in;(5;15);Sim;Nao
    
    Imprime
      "Sim"     porque %in verifica se o valor está entre 5 e 15.
      
    10|%i|%eq;(5;15);Sim;Nao
    
    Imprime
      "Nao"     porque %eq verifica se o valor está dentro do Array.

**Auto-conversão de tipo**

O termo é automaticamente convertido para o tipo do valor recebido.\
Desta forma, o valor comparado pode ser expresso em texto simples.

    $020|%gt;10;Sim;Nao
    
    Imprime
      "Nao"     porque o texto "020" foi comparado com "10"
    
    $020|%i|%gt;10;Sim;Nao
    
    Imprime
      "Sim"     porque o texto "020" foi convertido para numero
                e o 10 foi convertido implicitamente.
                
    $..dhEmi|%d|%ge;2018-01-01;Recente;Antigo
    
    Imprime
      "Recente"     se dhEmi for uma data a partir de 2018
      "Antigo"      se dhEmi for uma data anterior a 2018
    
**Repasse do valor recebido**

Durante a avaliação do termo o comando repassa como parâmetro o valor
exatamente como recebido do comando anterior. Isto permite usar o valor
recebido como parte da construção do termo.

Por exemplo, o exemplo a seguir sempre retorna "Verdadeiro", já que o
valor recebido pelo comando é mesmo emitido pelo termo:

    $..serie|%eq;$;Verdadeiro;Falso
    ^^^^^^^^   ^
    |          |
    '--> repassa o proprio
         valor como termo
    
    Imprime
      "Verdadeiro"

O comando também repassa o valor recebido para o comando subsequente
escolhido para execução.

Por exemplo, no exemplo a seguir, caso verdade um texto é impresso,
mas caso falso, o valor recebido pelo comando é repassado como está.

    $..vProd|%f|%lt;10;barato;$|O preço é {}
             ^^             ^
             |              |
             '----> repassa o proprio
                     valor comparado
    
    Imprime
      "O preço é barato"      se vProd for menor que 10
      "O preço é 11.5"        se vProd for R$ 11.50


## 5. Sintaxe Avançada

### 5.1. Coalesce e Array

[Coalesce] e [Array] são intrinsecamente
relacionados.
O mesmo algoritmo implementa os dois comandos.

O algoritmo repassa dois parâmetros para o comando seguinte:

**Repassando o coalesce**

1.  A solução do coalesce, como parâmetro padrão.
2.  O Array dos termos, como parâmetro adicional.

A solução do coalesce é repassada na entrada padrão do comando seguinte,
que então, acessa o resultado diretamente.

Como em:

    ;10;20|%i

Onde:

1.  O coalesce resolve para 10, já que o primeiro termo é vazio.
2.  O número 10 é convertido para inteiro.

O coalesce também pode ser acessado pelo índice na forma:

    $0
    
Ou pela emissão do índice na forma:

    $

A forma com índice é recomendada quando no template também é usado o
índice do termos do Array, que começam a partir de 1, conforme explicado
mais abaixo.

Já a forma sem índice é recomendada quando o termo deve ser passado como
está, sem processamento adicional, para o comando seguinte, conforme
explicado na seção [3.1. Expansão de Array].

**Repassando os termos do Array**

Já os itens do Array precisam ser explicitamente referenciados pelo seu
índice, iniciando em 1, na forma:

    $X
    
    Sendo X um índice a partir de 1

Exemplo:

    ;10;20|$3|%i

Onde:

1.  O coalesce resolve para 10, já que o primeiro termo é vazio.
2.  O 10 recebido é descartado e o 20 é impresso no lugar, já que o 20 é
    o terceiro termo a partir de 1.
3.  O número 20 é convertido para inteiro.

**Usando coalesce e os termos do Array**

No exemplo abaixo o índice zero é usado para acessar o resultado do
coalesce, enquanto os índices a partir de 1 são usados para acesso aos
termos do Array. É recomendado usar o índice zero sempre que, no mesmo
comando, seja referenciando também os termos do Array.

Note que o comando de [Format] acessa os termos pelo padrão `{X}`,
em vez do padrão `$X`, usado pelos demais comandos:

    ;10;20|Dos termos '{1}', '{2}' e '{3}' o termo '{0}' foi escolhido.
    
    Imprime
      "Dos termos '', '10' e '20' o termo '10' foi escolhido."

No exemplo abaixo o índice zero é omitido. É recomendado omitir o índice
quando os termos do Array não são referenciados.:

    ;10;20|%i
    ;10;20|O número é {}
    
### 5.2. Expansão de Array

Quando o comando [Array] é posto no início do pipeline não existem
parâmetros de entrada para processamento, porém, quando o Array é posto
a partir da segunda posição do pipeline, a saída do comando anterior é
passado como parâmetro para o Array. Neste caso a expansão de Array é
ativada.

A expansão pode se comportar de duas formas diferentes:

Expansão da parâmetro de entrada
:   Quando o comando anterior emite uma valor em vez de um Array.

    Neste caso, o valor emitido é passado como parâmetro de entrada para
    cada item do Array processado.
    
    Exemplo:
    
        150|$;e;$|{1} {2} {3}
        
        Imprime:
          "150 e 150"
          
    Explicação:
    
    1.  O texto 150 é repassado para o próximo comando.
    2.  Pela expansão de variável o texto 150 é repassado para cada
        termo separadamente.
        1.  No primeiro termo, a referência "$" equivale ao parâmetro de
            entrada (150), que é repassado como está.
        2.  No segundo termo o texto "e" é emitido.
        3.  No terceiro termo, a referência "$" equivale ao parâmetro de
            entrada (150), que é repassado como está.
        4.  O Array com os três termos é emitido para o próximo comando.
    3.  O comando de formatação imprime os três termos encontrados no
        Array.
    
Expansão de termo correspondente
:   Quando o comando anterior emite um Array como saída.

    Neste caso uma correspondência de termo é ativada. Cada item do
    Array emitido pelo comando anterior é passado como entrada no
    processamento do item correspondente no Array atual.
    
    A correspondência é posicional, isto é, o primeiro termo emitido é
    a entrada do primeiro termo processado, e assim por diante.
    
    Caso não haja mais termos no Array emitido, então, o último termo
    dele é repassado para os demais termos do Array atual.
    
    Exemplo:
    
        10;20|$;$;e;$|{1} {2} {3} {4}
        
        Imprime:
          "10 20 e 20"

    Explicação:

    1.  O Array com os termos 10 e 20 é emitido.
    2.  A expansão de Array é ativada:
        1.  No primeiro termo o primeiro termo recebido do Array é
            emitido (10).
        2.  No segundo termo o segundo termo recebido do Array é
            emitido (20).
        3.  No terceiro termo "e" é emitido.
        4.  No quarto termo, como não existe um quarto termo no Array
            recebido, o último termo encontrado nele (20) é emitido.
        5.  Um novo Array é emitido.
    3.  O comando de formatação imprime os quatro termos encontrados no
        Array.
    
---
Out/2017  
Guga Coder
