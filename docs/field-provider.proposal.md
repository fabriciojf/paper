Field Provider
==============

Habilita uma caixa de pesquisa combinada por consulta no servidor.

- - -

O campo tem a propriedade "__provider" com a estrutura:

    {
      ...
      "actions": [
        {
          "fields": [
            {
              ...
              "__provider": {
                "href": "http://localhost/api/1/users/current/empresas",
                "keys": [ "id" ]
              }
            }
          ]
        }
      ]
    }
    
Onde:

-   Tanto "href" quanto "keys" devem ser indicados.
-   "href" é usado para a construção da caixa de lista.
-   É esperado que "href" aponte para uma entidade da classe "list"
-   "keys" é usado para determinar quais propriedades dos items selecionados
    subirão para o servidor.
    
A entidade retornada por "href" deve ter pelo menos a estrutura:

    {
      "class": [ "list" ]
      "entities":
      [
        {
          "class": [ "badge" ]
          "rel: [ "listItem" ]
          "properties: { ... }
        }
        , ...
      ]
    }

- - -
    
A seleção deve ser enviada para o servidor nos formatos:

1.  Quando seleção única e apenas uma chave em "keys":

    {
      "field": <value>
    }

Exemplo:
    
    {
      "empresa": 7
    }

2.  Quando seleção única e mais de uma chave em "keys":

    {
      "field": { "key": <value>, ... }
    }

Exemplo:
    
    {
      "nota": { "serie": 7, "numero": 1943 }
    }
    
3.  Quando seleção múltipla e apenas uma chave em "keys":

    {
      "field": [ <value>, ... ]
    }

Exemplo:
    
    {
      "empresas": [ 1, 2, 3 ]
    }
    
4.  Quando seleção múltipla e mais de uma chave em "keys":

    {
      "field": [
        { "key": <value>, ... }
        , ...
      ]
    }

Exemplo:
    
    {
      "notas": [
        { "serie": 7, "numero": 1942 },
        { "serie": 7, "numero": 1943 }
      ]
    }

---
Jul/2018  
Guga Coder
